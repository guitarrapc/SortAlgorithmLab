using SortAlgorithm.Contexts;

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

    /// <summary>統計情報（StatisticsContextから取得した正確な値）</summary>
    public StatisticsContext? Statistics { get; set; }

    /// <summary>比較回数（StatisticsContextがある場合はそれを使用、なければレガシー値）</summary>
    public ulong CompareCount => Statistics?.CompareCount ?? 0;

    /// <summary>スワップ回数（StatisticsContextがある場合はそれを使用、なければレガシー値）</summary>
    public ulong SwapCount => Statistics?.SwapCount ?? 0;

    /// <summary>読み込み回数（StatisticsContextがある場合はそれを使用、なければレガシー値）</summary>
    public ulong IndexReadCount => Statistics?.IndexReadCount ?? 0;

    /// <summary>書き込み回数（StatisticsContextがある場合はそれを使用、なければレガシー値）</summary>
    public ulong IndexWriteCount => Statistics?.IndexWriteCount ?? 0;

    /// <summary>ソートが完了したかどうか</summary>
    public bool IsSortCompleted { get; set; }

    /// <summary>ソート完了ハイライトを表示するかどうか（2秒間のみ）</summary>
    public bool ShowCompletionHighlight { get; set; }
}
