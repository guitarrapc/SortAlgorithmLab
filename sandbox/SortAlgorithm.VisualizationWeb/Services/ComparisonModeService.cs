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
        foreach (var playback in _playbackServices)
        {
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
    public void AddAlgorithm(string algorithmName, AlgorithmMetadata metadata)
    {
        if (_state.Instances.Count >= ComparisonState.MaxComparisons || _state.InitialArray.Length == 0)
            return;

        var operations = _executor.ExecuteAndRecord(_state.InitialArray, metadata);
        var playback = new PlaybackService();
        playback.LoadOperations(_state.InitialArray, operations);
        _playbackServices.Add(playback);
        _state.Instances.Add(new ComparisonInstance
        {
            AlgorithmName = algorithmName,
            State = playback.State,
            Metadata = metadata,
            Playback = playback
        });

        NotifyStateChanged();
    }
    public void RemoveAlgorithm(int index)
    {
        if (index >= 0 && index < _state.Instances.Count)
        {
            _playbackServices[index].Dispose();
            _playbackServices.RemoveAt(index);
            _state.Instances.RemoveAt(index);
            NotifyStateChanged();
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
    private void NotifyStateChanged() => OnStateChanged?.Invoke();
    public void Dispose()
    {
        foreach (var p in _playbackServices)
        {
            p.Dispose();
        }

        _playbackServices.Clear();
        _state.Instances.Clear();
    }
}
