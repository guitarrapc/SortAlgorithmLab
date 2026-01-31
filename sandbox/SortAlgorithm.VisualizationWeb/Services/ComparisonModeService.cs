using SortAlgorithm.VisualizationWeb.Models;

namespace SortAlgorithm.VisualizationWeb.Services;

/// <summary>
/// 複数アルゴリズムの同期再生を管理するサービス
/// </summary>
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
    
    /// <summary>
    /// 比較モードを有効化し、初期配列を設定
    /// </summary>
    public void Enable(int[] initialArray)
    {
        _state.IsEnabled = true;
        _state.InitialArray = initialArray.ToArray(); // コピーして保持
        NotifyStateChanged();
    }
    
    /// <summary>
    /// 比較モードを無効化
    /// </summary>
    public void Disable()
    {
        Stop();
        
        // すべての PlaybackService を破棄
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
        Console.WriteLine($"[ComparisonModeService] Current instance count: {_state.Instances.Count}/{ComparisonState.MaxComparisons}");
        Console.WriteLine($"[ComparisonModeService] Initial array length: {_state.InitialArray.Length}");
        Console.WriteLine($"[ComparisonModeService] IsEnabled: {_state.IsEnabled}");
        
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
        
        // ソート実行と操作記録
        Console.WriteLine($"[ComparisonModeService] Executing sort and recording operations...");
        var operations = _executor.ExecuteAndRecord(_state.InitialArray, metadata);
        Console.WriteLine($"[ComparisonModeService] Recorded {operations.Count} operations");
        
        // PlaybackService作成とロード
        var playback = new PlaybackService();
        playback.LoadOperations(_state.InitialArray, operations);
        playback.StateChanged += NotifyStateChanged;
        
        _playbackServices.Add(playback);
        
        // ComparisonInstance作成
        var instance = new ComparisonInstance
        {
            AlgorithmName = algorithmName,
            State = playback.State, // PlaybackServiceの状態を参照
            Metadata = metadata
        };
        
        _state.Instances.Add(instance);
        Console.WriteLine($"[ComparisonModeService] Successfully added {algorithmName}. Total instances: {_state.Instances.Count}");
        
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
        
        NotifyStateChanged();
    }
    
    /// <summary>
    /// すべて再生（同期）
    /// </summary>
    public void Play()
    {
        foreach (var playback in _playbackServices)
        {
            playback.Play();
        }
        NotifyStateChanged();
    }
    
    /// <summary>
    /// すべて一時停止（同期）
    /// </summary>
    public void Pause()
    {
        foreach (var playback in _playbackServices)
        {
            playback.Pause();
        }
        NotifyStateChanged();
    }
    
    /// <summary>
    /// すべて停止（同期）
    /// </summary>
    public void Stop()
    {
        foreach (var playback in _playbackServices)
        {
            playback.Stop();
        }
        NotifyStateChanged();
    }
    
    /// <summary>
    /// すべてリセット（同期）
    /// </summary>
    public void Reset()
    {
        foreach (var playback in _playbackServices)
        {
            playback.Stop(); // Stop がリセット機能を兼ねている
        }
        NotifyStateChanged();
    }
    
    /// <summary>
    /// すべてシーク（同期）
    /// </summary>
    public void SeekAll(int targetIndex)
    {
        foreach (var playback in _playbackServices)
        {
            playback.SeekTo(targetIndex, throttle: false);
        }
        NotifyStateChanged();
    }
    
    /// <summary>
    /// 再生速度を設定（すべて同期）
    /// </summary>
    public void SetSpeedForAll(int opsPerFrame, double speedMultiplier)
    {
        foreach (var playback in _playbackServices)
        {
            playback.OperationsPerFrame = opsPerFrame;
            playback.SpeedMultiplier = speedMultiplier;
        }
        NotifyStateChanged();
    }
    
    /// <summary>
    /// AutoResetを設定（すべて同期）
    /// </summary>
    public void SetAutoResetForAll(bool autoReset)
    {
        foreach (var playback in _playbackServices)
        {
            playback.AutoReset = autoReset;
        }
        NotifyStateChanged();
    }
    
    /// <summary>
    /// 現在の再生状態を取得
    /// </summary>
    public bool IsPlaying()
    {
        return _state.Instances.Any(x => x.State.PlaybackState == PlaybackState.Playing);
    }
    
    private void NotifyStateChanged() => OnStateChanged?.Invoke();
    
    public void Dispose()
    {
        foreach (var playback in _playbackServices)
        {
            playback.Dispose();
        }
        _playbackServices.Clear();
        _state.Instances.Clear();
    }
}

