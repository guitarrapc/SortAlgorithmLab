using SortAlgorithm.VisualizationWeb.Models;
namespace SortAlgorithm.VisualizationWeb.Services;

public class ComparisonModeService : IDisposable
{
    private readonly SortExecutor _executor;
    private readonly ComparisonState _state = new();
    private readonly List<PlaybackService> _playbackServices = new();

    public ComparisonState State => _state;
    public event Action? OnStateChanged;

    public ComparisonModeService(SortExecutor executor)
    {
        _executor = executor;
    }

    public void Enable(int[] initialArray)
    {
        Stop();
        
        // すべてのPlaybackServiceのイベント購読解除
        foreach (var playback in _playbackServices)
        {
            playback.StateChanged -= OnPlaybackStateChanged;
            playback.Dispose();
        }

        _playbackServices.Clear();
        _state.Instances.Clear();
        _state.IsEnabled = true;
        _state.InitialArray = initialArray.ToArray();
        NotifyStateChanged();
    }
    public void Disable()
    {
        Stop();
        
        // すべてのPlaybackServiceのイベント購読解除
        foreach (var playback in _playbackServices)
        {
            playback.StateChanged -= OnPlaybackStateChanged;
            playback.Dispose();
        }

        _playbackServices.Clear();
        _state.Instances.Clear();
        _state.IsEnabled = false;
        _state.InitialArray = Array.Empty<int>();
        NotifyStateChanged();
    }
    public void AddAlgorithm(string algorithmName, AlgorithmMetadata metadata)
    {
        if (_state.Instances.Count >= ComparisonState.MaxComparisons || _state.InitialArray.Length == 0)
            return;

        try
        {
            var operations = _executor.ExecuteAndRecord(_state.InitialArray, metadata);
            var playback = new PlaybackService();
            playback.LoadOperations(_state.InitialArray, operations);
            
            // PlaybackServiceのStateChangedイベントを購読
            playback.StateChanged += OnPlaybackStateChanged;
            
            _playbackServices.Add(playback);
            _state.Instances.Add(new ComparisonInstance
            {
                AlgorithmName = algorithmName,
                State = playback.State,
                Metadata = metadata,
                Playback = playback
            });

            Console.WriteLine($"[ComparisonMode] Added {algorithmName}: {operations.Count} operations");
            NotifyStateChanged();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ComparisonMode] ERROR adding {algorithmName}: {ex.Message}");
            // エラーが発生しても他のアルゴリズムに影響しないように続行
        }
    }
    public void RemoveAlgorithm(int index)
    {
        Console.WriteLine($"[ComparisonModeService] RemoveAlgorithm called for index: {index}");
        Console.WriteLine($"[ComparisonModeService] Before removal, _playbackServices.Count: {_playbackServices.Count}, _state.Instances.Count: {_state.Instances.Count}");
        
        if (index >= 0 && index < _state.Instances.Count)
        {
            var algorithmName = _state.Instances[index].AlgorithmName;
            Console.WriteLine($"[ComparisonModeService] Removing algorithm: {algorithmName}");
            
            try
            {
                // イベント購読解除
                _playbackServices[index].StateChanged -= OnPlaybackStateChanged;
                
                _playbackServices[index].Dispose();
                Console.WriteLine($"[ComparisonModeService] PlaybackService disposed successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ComparisonModeService] ERROR disposing PlaybackService: {ex.Message}");
            }
            
            _playbackServices.RemoveAt(index);
            _state.Instances.RemoveAt(index);
            
            Console.WriteLine($"[ComparisonModeService] After removal, _playbackServices.Count: {_playbackServices.Count}, _state.Instances.Count: {_state.Instances.Count}");
            
            NotifyStateChanged();
        }
        else
        {
            Console.WriteLine($"[ComparisonModeService] ERROR: Invalid index {index} (valid range: 0-{_state.Instances.Count - 1})");
        }
    }
    public void Play()
    {
        foreach (var p in _playbackServices)
        {
            p.Play();
        }

        NotifyStateChanged();
    }
    public void Pause()
    {
        foreach (var p in _playbackServices)
        {
            p.Pause();
        }

        NotifyStateChanged();
    }
    public void Stop()
    {
        foreach (var p in _playbackServices)
        {
            p.Stop();
        }

        NotifyStateChanged();
    }
    public void Reset() => Stop();
    public void SeekAll(int i)
    {
        foreach (var p in _playbackServices)
        {
            p.SeekTo(i, false);
        }

        NotifyStateChanged();
    }
    public void SetSpeedForAll(int ops, double speed)
    {
        foreach (var p in _playbackServices)
        {
            p.OperationsPerFrame = ops; p.SpeedMultiplier = speed;
        }

        NotifyStateChanged();
    }
    public void SetAutoResetForAll(bool auto)
    {
        foreach (var p in _playbackServices)
        {
            p.AutoReset = auto;
        }

        NotifyStateChanged();
    }
    
    public bool IsPlaying() => _playbackServices.Any(p => p.State.PlaybackState == PlaybackState.Playing);
    
    /// <summary>
    /// PlaybackServiceの状態変更を受け取り、通知を伝播
    /// </summary>
    private void OnPlaybackStateChanged()
    {
        // 完了状態をチェック
        CheckCompletionStatus();
        
        // 個々のPlaybackServiceの状態変更をComparisonModeの状態変更として通知
        NotifyStateChanged();
    }
    
    /// <summary>
    /// すべてのアルゴリズムの完了状態をチェック
    /// </summary>
    private void CheckCompletionStatus()
    {
        var completedCount = _state.Instances.Count(x => x.State.IsSortCompleted);
        var totalCount = _state.Instances.Count;
        
        if (completedCount > 0 && completedCount == totalCount)
        {
            // すべて完了した
            Console.WriteLine($"[ComparisonMode] 🎉 All {totalCount} algorithms completed!");
            
            // 各アルゴリズムの統計を出力
            foreach (var instance in _state.Instances.OrderBy(x => x.State.CompareCount))
            {
                Console.WriteLine($"  - {instance.AlgorithmName}: Compares={instance.State.CompareCount:N0}, Swaps={instance.State.SwapCount:N0}");
            }
        }
    }
    
    private void NotifyStateChanged() => OnStateChanged?.Invoke();
    
    public void Dispose()
    {
        // すべてのPlaybackServiceのイベント購読解除
        foreach (var p in _playbackServices)
        {
            p.StateChanged -= OnPlaybackStateChanged;
            p.Dispose();
        }

        _playbackServices.Clear();
        _state.Instances.Clear();
    }
}
