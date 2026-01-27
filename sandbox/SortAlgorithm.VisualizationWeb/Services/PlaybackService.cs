using System.Timers;
using SortAlgorithm.VisualizationWeb.Models;
using Timer = System.Timers.Timer;

namespace SortAlgorithm.VisualizationWeb.Services;

/// <summary>
/// 再生制御とシーク処理を行うサービス
/// </summary>
public class PlaybackService : IDisposable
{
    private readonly Timer _timer;
    private List<SortOperation> _operations = [];
    private int[] _initialArray = [];
    private Dictionary<int, int[]> _initialBuffers = new();
    
    private const int TARGET_FPS = 60; // 固定フレームレート
    
    /// <summary>現在の状態</summary>
    public VisualizationState State { get; private set; } = new();
    
    /// <summary>1フレームあたりの操作数（1-1000）</summary>
    public int OperationsPerFrame { get; set; } = 10;
    
    /// <summary>状態が変更されたときのイベント</summary>
    public event Action? StateChanged;
    
    public PlaybackService()
    {
        _timer = new Timer();
        _timer.Interval = 1000.0 / TARGET_FPS; // 60 FPS = 16.67ms
        _timer.Elapsed += OnTimerElapsed;
    }
    
    /// <summary>
    /// ソート操作をロードする
    /// </summary>
    public void LoadOperations(int[] initialArray, List<SortOperation> operations)
    {
        Stop();
        _operations = operations;
        _initialArray = [.. initialArray];
        _initialBuffers.Clear();
        
        State = new VisualizationState
        {
            MainArray = [.. initialArray],
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
        _timer.Start();
        StateChanged?.Invoke();
    }
    
    /// <summary>
    /// 一時停止
    /// </summary>
    public void Pause()
    {
        if (State.PlaybackState != PlaybackState.Playing) return;
        
        State.PlaybackState = PlaybackState.Paused;
        _timer.Stop();
        StateChanged?.Invoke();
    }
    
    /// <summary>
    /// 停止してリセット
    /// </summary>
    public void Stop()
    {
        _timer.Stop();
        State.CurrentOperationIndex = 0;
        State.MainArray = [.. _initialArray];
        State.BufferArrays.Clear();
        State.PlaybackState = PlaybackState.Stopped;
        ClearHighlights();
        ResetStatistics();
        StateChanged?.Invoke();
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
    
    private void OnTimerElapsed(object? sender, ElapsedEventArgs e)
    {
        if (State.CurrentOperationIndex >= _operations.Count)
        {
            Stop();
            return;
        }
        
        ClearHighlights();
        
        // 1フレームで複数の操作を処理
        int operationsToProcess = Math.Min(OperationsPerFrame, _operations.Count - State.CurrentOperationIndex);
        
        for (int i = 0; i < operationsToProcess; i++)
        {
            var operation = _operations[State.CurrentOperationIndex];
            ApplyOperation(operation, applyToArray: true, updateStats: true);
            State.CurrentOperationIndex++;
            
            if (State.CurrentOperationIndex >= _operations.Count)
            {
                Stop();
                StateChanged?.Invoke();
                return;
            }
        }
        
        // 最後の操作をハイライト表示
        if (State.CurrentOperationIndex < _operations.Count)
        {
            var lastOperation = _operations[State.CurrentOperationIndex - 1];
            ApplyOperation(lastOperation, applyToArray: false, updateStats: false);
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
                    var arr = GetArray(operation.BufferId1);
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
                if (updateStats) State.IndexWriteCount++;
                break;
                
            case OperationType.RangeCopy:
                // RangeCopyは簡略化（実際の実装では配列コピーが必要）
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
        _timer?.Dispose();
    }
}
