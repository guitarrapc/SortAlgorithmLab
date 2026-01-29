using System.Buffers;
using SortAlgorithm.VisualizationWeb.Models;

namespace SortAlgorithm.VisualizationWeb.Services;

/// <summary>
/// 再生制御とシーク処理を行うサービス（Task.Run高速ループ版）
/// </summary>
public class PlaybackService : IDisposable
{
    private List<SortOperation> _operations = [];
    
    // ArrayPoolで配列を再利用
    private int[] _pooledArray;
    private int _currentArraySize;
    private int[] _initialArray = [];
    private Dictionary<int, int[]> _initialBuffers = new();
    
    private const int TARGET_FPS = 60; // ベースフレームレート
    private const int MAX_ARRAY_SIZE = 4096; // 最大配列サイズ
    private const double RENDER_INTERVAL_MS = 16.67; // UI更新間隔（60 FPS）
    
    // Task.Run用のフィールド
    private CancellationTokenSource? _cancellationTokenSource;
    private Task? _playbackTask;
    
    /// <summary>現在の状態</summary>
    public VisualizationState State { get; private set; } = new();
    
    /// <summary>1フレームあたりの操作数（1-1000）</summary>
    public int OperationsPerFrame { get; set; } = 1;
    
    /// <summary>速度倍率（0.1x - 100x）</summary>
    public double SpeedMultiplier { get; set; } = 10.0;
    
    /// <summary>ソート完了時に自動的にリセットするか</summary>
    public bool AutoReset { get; set; } = false;
    
    /// <summary>描画なし超高速モード</summary>
    public bool InstantMode { get; set; } = false;
    
    /// <summary>状態が変更されたときのイベント</summary>
    public event Action? StateChanged;
    
    public PlaybackService()
    {
        // 最大サイズの配列をArrayPoolからレンタル
        _pooledArray = ArrayPool<int>.Shared.Rent(MAX_ARRAY_SIZE);
        _currentArraySize = 0;
    }
    
    /// <summary>
    /// ソート操作をロードする
    /// </summary>
    public void LoadOperations(ReadOnlySpan<int> initialArray, List<SortOperation> operations)
    {
        Stop();
        _operations = operations;
        _currentArraySize = initialArray.Length;
        
        // プールされた配列の必要な部分だけを使用
        initialArray.CopyTo(_pooledArray.AsSpan(0, _currentArraySize));
        _initialArray = _pooledArray.AsSpan(0, _currentArraySize).ToArray(); // 初期状態のコピーを保持
        _initialBuffers.Clear();
        
        State = new VisualizationState
        {
            MainArray = _pooledArray.AsSpan(0, _currentArraySize).ToArray(), // 現在の状態用のコピー
            TotalOperations = operations.Count,
            CurrentOperationIndex = 0,
            PlaybackState = PlaybackState.Stopped
        };
        
        StateChanged?.Invoke();
    }
    
    /// <summary>
    /// 再生開始
    /// </summary>
    public void Play()
    {
        if (State.PlaybackState == PlaybackState.Playing) return;
        
        State.PlaybackState = PlaybackState.Playing;
        
        // 既存のタスクをキャンセル
        _cancellationTokenSource?.Cancel();
        
        // 新しいキャンセルトークンを作成
        _cancellationTokenSource = new CancellationTokenSource();
        
        // 描画なしモードの場合は即座に完了
        if (InstantMode)
        {
            PlayInstant();
            return;
        }
        
        // バックグラウンドで高速ループを開始
        _playbackTask = Task.Run(() => PlaybackLoopAsync(_cancellationTokenSource.Token));
        
        StateChanged?.Invoke();
    }
    
    /// <summary>
    /// 描画なし超高速実行
    /// </summary>
    private void PlayInstant()
    {
        // UI更新を完全スキップして全操作を処理
        while (State.CurrentOperationIndex < _operations.Count)
        {
            var operation = _operations[State.CurrentOperationIndex];
            ApplyOperation(operation, applyToArray: true, updateStats: true);
            State.CurrentOperationIndex++;
        }
        
        // 完了
        if (AutoReset)
        {
            Stop();
        }
        else
        {
            State.PlaybackState = PlaybackState.Paused;
        }
        
        // 最終状態のみ描画
        StateChanged?.Invoke();
    }
    
    /// <summary>
    /// 一時停止
    /// </summary>
    public void Pause()
    {
        if (State.PlaybackState != PlaybackState.Playing) return;
        
        State.PlaybackState = PlaybackState.Paused;
        _cancellationTokenSource?.Cancel();
        StateChanged?.Invoke();
    }
    
    /// <summary>
    /// 停止してリセット
    /// </summary>
    public void Stop()
    {
        _cancellationTokenSource?.Cancel();
        State.CurrentOperationIndex = 0;
        
        // プールされた配列を再利用
        if (_currentArraySize > 0)
        {
            _initialArray.AsSpan().CopyTo(_pooledArray.AsSpan(0, _currentArraySize));
            State.MainArray = _pooledArray.AsSpan(0, _currentArraySize).ToArray();
        }
        
        State.BufferArrays.Clear();
        State.PlaybackState = PlaybackState.Stopped;
        ClearHighlights();
        ResetStatistics();
        StateChanged?.Invoke();
    }
    
    /// <summary>
    /// 高速再生ループ（SpinWait高精度版）
    /// </summary>
    private async Task PlaybackLoopAsync(CancellationToken cancellationToken)
    {
        var lastRenderTime = DateTime.UtcNow;
        var sw = System.Diagnostics.Stopwatch.StartNew();
        var nextFrameTime = 0.0;
        
        try
        {
            while (!cancellationToken.IsCancellationRequested && State.CurrentOperationIndex < _operations.Count)
            {
                // フレーム間隔を計算（ミリ秒）
                var frameInterval = 1000.0 / (TARGET_FPS * SpeedMultiplier);
                
                // 次のフレーム時刻まで待機
                var currentTime = sw.Elapsed.TotalMilliseconds;
                if (currentTime < nextFrameTime)
                {
                    // 高精度待機: SpinWait
                    var spinWait = new SpinWait();
                    while (sw.Elapsed.TotalMilliseconds < nextFrameTime && !cancellationToken.IsCancellationRequested)
                    {
                        spinWait.SpinOnce(); // CPUビジーウェイト
                    }
                }
                
                nextFrameTime = sw.Elapsed.TotalMilliseconds + frameInterval;
                
                // 操作を処理
                ClearHighlights();
                
                int opsToProcess = Math.Min(OperationsPerFrame, _operations.Count - State.CurrentOperationIndex);
                for (int i = 0; i < opsToProcess && State.CurrentOperationIndex < _operations.Count; i++)
                {
                    if (cancellationToken.IsCancellationRequested) break;
                    
                    var operation = _operations[State.CurrentOperationIndex];
                    ApplyOperation(operation, applyToArray: true, updateStats: true);
                    State.CurrentOperationIndex++;
                }
                
                // ハイライト更新
                if (State.CurrentOperationIndex > 0 && State.CurrentOperationIndex < _operations.Count)
                {
                    var lastOperation = _operations[State.CurrentOperationIndex - 1];
                    ApplyOperation(lastOperation, applyToArray: false, updateStats: false);
                }
                
                // UI更新（60 FPS制限）
                var now = DateTime.UtcNow;
                var renderElapsed = (now - lastRenderTime).TotalMilliseconds;
                
                if (renderElapsed >= RENDER_INTERVAL_MS)
                {
                    lastRenderTime = now;
                    StateChanged?.Invoke();
                    await Task.Yield(); // UIスレッドに処理を譲る
                }
            }
            
            // 完了処理
            if (State.CurrentOperationIndex >= _operations.Count)
            {
                if (AutoReset)
                {
                    Stop();
                }
                else
                {
                    State.PlaybackState = PlaybackState.Paused;
                }
                
                StateChanged?.Invoke();
            }
        }
        catch (OperationCanceledException)
        {
            // キャンセル時は何もしない
        }
    }
    
    /// <summary>
    /// 再生/一時停止を切り替え
    /// </summary>
    public void TogglePlayPause()
    {
        if (State.PlaybackState == PlaybackState.Playing)
        {
            Pause();
        }
        else
        {
            Play();
        }
    }
    
    /// <summary>
    /// 指定位置にシーク
    /// </summary>
    public void SeekTo(int operationIndex)
    {
        if (operationIndex < 0 || operationIndex > _operations.Count)
            return;
        
        // 初期状態から指定位置まで操作を適用
        State.MainArray = [.. _initialArray];
        State.BufferArrays.Clear();
        ResetStatistics();
        ClearHighlights();
        
        for (int i = 0; i < operationIndex && i < _operations.Count; i++)
        {
            ApplyOperation(_operations[i], applyToArray: true, updateStats: true);
        }
        
        State.CurrentOperationIndex = operationIndex;
        
        // 現在の操作をハイライト
        if (operationIndex < _operations.Count)
        {
            ApplyOperation(_operations[operationIndex], applyToArray: false, updateStats: false);
        }
        
        StateChanged?.Invoke();
    }
    
    private void ApplyOperation(SortOperation operation, bool applyToArray, bool updateStats)
    {
        switch (operation.Type)
        {
            case OperationType.Compare:
                State.CompareIndices.Add(operation.Index1);
                State.CompareIndices.Add(operation.Index2);
                if (updateStats) State.CompareCount++;
                break;
                
            case OperationType.Swap:
                State.SwapIndices.Add(operation.Index1);
                State.SwapIndices.Add(operation.Index2);
                if (applyToArray)
                {
                    var arr = GetArray(operation.BufferId1).AsSpan();
                    (arr[operation.Index1], arr[operation.Index2]) = (arr[operation.Index2], arr[operation.Index1]);
                }
                if (updateStats) State.SwapCount++;
                break;
                
            case OperationType.IndexRead:
                State.ReadIndices.Add(operation.Index1);
                if (updateStats) State.IndexReadCount++;
                break;
                
            case OperationType.IndexWrite:
                State.WriteIndices.Add(operation.Index1);
                if (applyToArray && operation.Value.HasValue)
                {
                    var arr = GetArray(operation.BufferId1).AsSpan();
                    if (operation.Index1 >= 0 && operation.Index1 < arr.Length)
                    {
                        arr[operation.Index1] = operation.Value.Value;
                    }
                }
                if (updateStats) State.IndexWriteCount++;
                break;
                
            case OperationType.RangeCopy:
                // ハイライト表示: sourceとdestinationの範囲をハイライト
                for (int i = 0; i < operation.Length; i++)
                {
                    if (operation.Index1 >= 0)
                    {
                        State.ReadIndices.Add(operation.Index1 + i);
                    }
                    if (operation.Index2 >= 0)
                    {
                        State.WriteIndices.Add(operation.Index2 + i);
                    }
                }
                
                if (applyToArray)
                {
                    var sourceArr = GetArray(operation.BufferId1);
                    var destArr = GetArray(operation.BufferId2);
                    
                    var sourceSpan = sourceArr.AsSpan();
                    var destSpan = destArr.AsSpan();
                    
                    if (operation.Index1 >= 0 && operation.Index2 >= 0 && 
                        operation.Length > 0 &&
                        operation.Index1 + operation.Length <= sourceSpan.Length &&
                        operation.Index2 + operation.Length <= destSpan.Length)
                    {
                        sourceSpan.Slice(operation.Index1, operation.Length)
                            .CopyTo(destSpan.Slice(operation.Index2, operation.Length));
                    }
                }
                if (updateStats)
                {
                    State.IndexReadCount += (ulong)operation.Length;
                    State.IndexWriteCount += (ulong)operation.Length;
                }
                break;
        }
    }
    
    private int[] GetArray(int bufferId)
    {
        if (bufferId == 0) return State.MainArray;
        
        if (!State.BufferArrays.ContainsKey(bufferId))
        {
            State.BufferArrays[bufferId] = new int[State.MainArray.Length];
        }
        return State.BufferArrays[bufferId];
    }
    
    private void ClearHighlights()
    {
        State.CompareIndices.Clear();
        State.SwapIndices.Clear();
        State.ReadIndices.Clear();
        State.WriteIndices.Clear();
    }
    
    private void ResetStatistics()
    {
        State.CompareCount = 0;
        State.SwapCount = 0;
        State.IndexReadCount = 0;
        State.IndexWriteCount = 0;
    }
    
    public void Dispose()
    {
        // 再生中のタスクをキャンセル
        _cancellationTokenSource?.Cancel();
        _cancellationTokenSource?.Dispose();
        
        // タスクの完了を待機（最大1秒）
        _playbackTask?.Wait(TimeSpan.FromSeconds(1));
        
        // ArrayPoolに配列を返却
        if (_pooledArray != null)
        {
            ArrayPool<int>.Shared.Return(_pooledArray, clearArray: true);
        }
    }
}
