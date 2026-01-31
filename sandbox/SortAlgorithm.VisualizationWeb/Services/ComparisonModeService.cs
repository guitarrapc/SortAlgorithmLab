using SortAlgorithm.VisualizationWeb.Models;

namespace SortAlgorithm.VisualizationWeb.Services;

/// <summary>
/// 複数アルゴリズムの同期再生を管理するサービス（統一タイマー版）
/// </summary>
public class ComparisonModeService : IDisposable
{
    private readonly SortExecutor _executor;
    private readonly ComparisonState _state = new();
    private readonly List<PlaybackService> _playbackServices = new();
    
    // 統一的な再生制御用
    private CancellationTokenSource? _cancellationTokenSource;
    private Task? _playbackTask;
    private bool _isPlaying = false;
    private DateTime _lastRenderTime = DateTime.MinValue;
    private const double RENDER_INTERVAL_MS = 16.67; // 60 FPS
    
    public ComparisonState State => _state;
    public event Action? OnStateChanged;
    
    public ComparisonModeService(SortExecutor executor)
    {
        _executor = executor;
    }
    
    /// <summary>
    /// 比較モードを有効化し、初期配列を設定
    /// </summary>
    public void Enable(int[] initialArray)
    {
        Console.WriteLine($"[ComparisonModeService] Enable called with array length: {initialArray.Length}");
        _state.IsEnabled = true;
        _state.InitialArray = initialArray.ToArray();
        Console.WriteLine($"[ComparisonModeService] Enabled. InitialArray length: {_state.InitialArray.Length}");
        NotifyStateChanged();
    }
    
    /// <summary>
    /// 比較モードを無効化
    /// </summary>
    public void Disable()
    {
        Stop();
        
        foreach (var playback in _playbackServices)
        {
            playback.Dispose();
        }
        
        _playbackServices.Clear();
        _state.Instances.Clear();
        _state.IsEnabled = false;
        _state.InitialArray = Array.Empty<int>();
        
        NotifyStateChanged();
    }
    
    /// <summary>
    /// アルゴリズムを追加
    /// </summary>
    public void AddAlgorithm(string algorithmName, AlgorithmMetadata metadata)
    {
        Console.WriteLine($"[ComparisonModeService] AddAlgorithm called: {algorithmName}");
        
        if (_state.Instances.Count >= ComparisonState.MaxComparisons)
        {
            Console.WriteLine($"[ComparisonModeService] Cannot add: max comparisons reached");
            return;
        }
            
        if (_state.InitialArray.Length == 0)
        {
            Console.WriteLine($"[ComparisonModeService] Cannot add: initial array is empty");
            return;
        }
        
        var operations = _executor.ExecuteAndRecord(_state.InitialArray, metadata);
        Console.WriteLine($"[ComparisonModeService] Recorded {operations.Count} operations");
        
        var playback = new PlaybackService();
        playback.LoadOperations(_state.InitialArray, operations);
        
        // StateChanged イベントは購読しない（ComparisonGridItemが直接購読する）
        
        _playbackServices.Add(playback);
        
        var instance = new ComparisonInstance
        {
            AlgorithmName = algorithmName,
            State = playback.State,
            Metadata = metadata,
            Playback = playback // 🔧 PlaybackServiceを公開
        };
        
        _state.Instances.Add(instance);
        Console.WriteLine($"[ComparisonModeService] Successfully added {algorithmName}. Total: {_state.Instances.Count}");
        
        // アイテム追加時のみ通知（再生中の StateChanged は通知しない）
        NotifyStateChanged();
    }
    
    /// <summary>
    /// アルゴリズムを削除
    /// </summary>
    public void RemoveAlgorithm(int index)
    {
        if (index < 0 || index >= _state.Instances.Count)
            return;
        
        var playback = _playbackServices[index];
        playback.Dispose();
        
        _playbackServices.RemoveAt(index);
        _state.Instances.RemoveAt(index);
        
        // アイテム削除時のみ通知
        NotifyStateChanged();
    }
    
    /// <summary>
    /// すべて再生（1つの場合は最適化、複数の場合は統一タイマー）
    /// </summary>
    public void Play()
    {
        Console.WriteLine($"[ComparisonMode] Play() called. _isPlaying: {_isPlaying}, Count: {_playbackServices.Count}");
        
        if (_isPlaying) return;
        if (!_playbackServices.Any()) return;
        
        _isPlaying = true;
        
        // ✅ 1つだけの場合は、通常のPlaybackService.Play()を使う（パフォーマンス最適化）
        if (_playbackServices.Count == 1)
        {
            Console.WriteLine($"[ComparisonMode] ✅ Single playback, using native PlaybackService.Play()");
            _playbackServices[0].Play();
            NotifyStateChanged();
            return;
        }
        
        // 複数の場合は統一タイマーを使用
        Console.WriteLine($"[ComparisonMode] Multiple playbacks ({_playbackServices.Count}), using unified timer");
        foreach (var playback in _playbackServices)
        {
            playback.State.PlaybackState = PlaybackState.Playing;
        }
        
        _cancellationTokenSource = new CancellationTokenSource();
        _playbackTask = Task.Run(() => UnifiedPlaybackLoopAsync(_cancellationTokenSource.Token));
        
        NotifyStateChanged();
    }
    
    /// <summary>
    /// 統一的な再生ループ（すべてのPlaybackServiceを同期）
    /// </summary>
    private async Task UnifiedPlaybackLoopAsync(CancellationToken cancellationToken)
    {
        var sw = System.Diagnostics.Stopwatch.StartNew();
        var nextFrameTime = 0.0;
        _lastRenderTime = DateTime.UtcNow;
        
        var opsPerFrame = _playbackServices.FirstOrDefault()?.OperationsPerFrame ?? 1;
        var speedMultiplier = _playbackServices.FirstOrDefault()?.SpeedMultiplier ?? 10.0;
        var frameInterval = 1000.0 / (60 * speedMultiplier);
        
        Console.WriteLine($"[ComparisonMode] Starting playback loop. OpsPerFrame: {opsPerFrame}, SpeedMultiplier: {speedMultiplier}, FrameInterval: {frameInterval}ms");
        
        int frameCount = 0;
        var lastLogTime = DateTime.UtcNow;
        
        try
        {
            while (!cancellationToken.IsCancellationRequested && 
                   _playbackServices.Any(p => p.State.CurrentOperationIndex < p.State.TotalOperations))
            {
                var currentTime = sw.Elapsed.TotalMilliseconds;
                
                // SpinWaitの代わりにTask.Delayを使用（CPU消費を抑える）
                if (currentTime < nextFrameTime)
                {
                    var delayMs = (int)(nextFrameTime - currentTime);
                    if (delayMs > 0)
                    {
                        await Task.Delay(delayMs, cancellationToken);
                    }
                }
                
                nextFrameTime = sw.Elapsed.TotalMilliseconds + frameInterval;
                
                // すべてのPlaybackServiceを同期的に進行
                foreach (var playback in _playbackServices)
                {
                    if (playback.State.CurrentOperationIndex < playback.State.TotalOperations)
                    {
                        playback.AdvanceFrame(opsPerFrame);
                    }
                }
                
                // UI更新（60 FPS制限）
                var now = DateTime.UtcNow;
                var renderElapsed = (now - _lastRenderTime).TotalMilliseconds;
                
                if (renderElapsed >= RENDER_INTERVAL_MS)
                {
                    _lastRenderTime = now;
                    NotifyStateChanged();
                    await Task.Yield();
                    frameCount++;
                }
                
                // 1秒ごとにFPSをログ出力
                var logElapsed = (now - lastLogTime).TotalSeconds;
                if (logElapsed >= 1.0)
                {
                    Console.WriteLine($"[ComparisonMode] FPS: {frameCount / logElapsed:F1}");
                    frameCount = 0;
                    lastLogTime = now;
                }
            }
            
            // 完了処理
            _isPlaying = false;
            foreach (var playback in _playbackServices)
            {
                if (playback.State.CurrentOperationIndex >= playback.State.TotalOperations)
                {
                    playback.State.IsSortCompleted = true;
                    playback.State.ShowCompletionHighlight = true;
                }
                playback.State.PlaybackState = PlaybackState.Paused;
            }
            
            Console.WriteLine($"[ComparisonMode] Playback completed");
            NotifyStateChanged();
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine($"[ComparisonMode] Playback cancelled");
        }
    }
    
    public void Pause()
    {
        _isPlaying = false;
        _cancellationTokenSource?.Cancel();
        
        foreach (var playback in _playbackServices)
        {
            // 1つだけの場合は、通常のPause()を呼ぶ
            if (_playbackServices.Count == 1)
            {
                playback.Pause();
            }
            else
            {
                playback.State.PlaybackState = PlaybackState.Paused;
            }
        }
        NotifyStateChanged();
    }
    
    public void Stop()
    {
        _isPlaying = false;
        _cancellationTokenSource?.Cancel();
        
        foreach (var playback in _playbackServices)
        {
            playback.Stop();
        }
        NotifyStateChanged();
    }
    
    public void Reset()
    {
        Stop();
    }
    
    public void SeekAll(int targetIndex)
    {
        foreach (var playback in _playbackServices)
        {
            playback.SeekTo(targetIndex, throttle: false);
        }
        NotifyStateChanged();
    }
    
    public void SetSpeedForAll(int opsPerFrame, double speedMultiplier)
    {
        foreach (var playback in _playbackServices)
        {
            playback.OperationsPerFrame = opsPerFrame;
            playback.SpeedMultiplier = speedMultiplier;
        }
        NotifyStateChanged();
    }
    
    public void SetAutoResetForAll(bool autoReset)
    {
        foreach (var playback in _playbackServices)
        {
            playback.AutoReset = autoReset;
        }
        NotifyStateChanged();
    }
    
    public bool IsPlaying()
    {
        // 1つの場合は、PlaybackServiceの状態を直接参照
        if (_playbackServices.Count == 1)
        {
            return _playbackServices[0].State.PlaybackState == PlaybackState.Playing;
        }
        return _isPlaying;
    }
    
    private void NotifyStateChanged() => OnStateChanged?.Invoke();
    
    public void Dispose()
    {
        _cancellationTokenSource?.Cancel();
        _cancellationTokenSource?.Dispose();
        
        foreach (var playback in _playbackServices)
        {
            playback.Dispose();
        }
        _playbackServices.Clear();
        _state.Instances.Clear();
    }
}
