namespace SortAlgorithm.VisualizationWeb.Models;

/// <summary>
/// 可視化の状態を保持するクラス
/// </summary>
public class VisualizationState
{
    /// <summary>メイン配列</summary>
    public int[] MainArray { get; set; } = [];
    
    /// <summary>バッファー配列（BufferId -> 配列）</summary>
    public Dictionary<int, int[]> BufferArrays { get; set; } = new();
    
    /// <summary>比較操作中のインデックス</summary>
    public HashSet<int> CompareIndices { get; set; } = [];
    
    /// <summary>スワップ操作中のインデックス</summary>
    public HashSet<int> SwapIndices { get; set; } = [];
    
    /// <summary>読み込み操作中のインデックス</summary>
    public HashSet<int> ReadIndices { get; set; } = [];
    
    /// <summary>書き込み操作中のインデックス</summary>
    public HashSet<int> WriteIndices { get; set; } = [];
    
    /// <summary>現在の操作インデックス</summary>
    public int CurrentOperationIndex { get; set; }
    
    /// <summary>総操作数</summary>
    public int TotalOperations { get; set; }
    
    /// <summary>可視化モード</summary>
    public VisualizationMode Mode { get; set; } = VisualizationMode.BarChart;
    
    /// <summary>再生状態</summary>
    public PlaybackState PlaybackState { get; set; } = PlaybackState.Stopped;
    
    /// <summary>比較回数</summary>
    public ulong CompareCount { get; set; }
    
    /// <summary>スワップ回数</summary>
    public ulong SwapCount { get; set; }
    
    /// <summary>読み込み回数</summary>
    public ulong IndexReadCount { get; set; }
    
    /// <summary>書き込み回数</summary>
    public ulong IndexWriteCount { get; set; }
}
