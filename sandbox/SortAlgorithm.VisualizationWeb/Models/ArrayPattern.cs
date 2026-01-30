namespace SortAlgorithm.VisualizationWeb.Models;

/// <summary>
/// 配列生成パターンの種類
/// </summary>
public enum ArrayPattern
{
    /// <summary>
    /// ランダム配列
    /// </summary>
    Random,

    /// <summary>
    /// ソート済み配列（昇順）
    /// </summary>
    Sorted,

    /// <summary>
    /// 逆順配列（降順）
    /// </summary>
    Reversed,

    /// <summary>
    /// ほぼソート済み配列（要素の10%をランダムに入れ替え）
    /// </summary>
    NearlySorted,

    /// <summary>
    /// ほぼソート済み配列（最後の10%のみシャッフル）
    /// </summary>
    NearlySortedLast,

    /// <summary>
    /// ほぼソート済み配列（最初の10%のみシャッフル）
    /// </summary>
    NearlySortedStart,

    /// <summary>
    /// 重複要素を多く含む配列（ユニーク値は配列サイズの10%程度）
    /// </summary>
    FewUnique,

    /// <summary>
    /// 重複多数（ユニーク値は配列サイズの20%程度、より多くの重複）
    /// </summary>
    ManyDuplicates,

    /// <summary>
    /// 山型配列（中央が最大値、両端が小さい値）
    /// </summary>
    MountainShape,

    /// <summary>
    /// 谷型配列（中央が最小値、両端が大きい値）
    /// </summary>
    ValleyShape,

    /// <summary>
    /// ジグザグパターン（交互に上下する）
    /// </summary>
    Zigzag,

    /// <summary>
    /// 半分ソート済み（前半のみソート済み、後半はランダム）
    /// </summary>
    HalfSorted
}
