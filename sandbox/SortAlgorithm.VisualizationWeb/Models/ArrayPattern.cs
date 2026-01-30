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
    /// 重複要素を多く含む配列（ユニーク値は配列サイズの10%程度）
    /// </summary>
    FewUnique,

    /// <summary>
    /// 前半ソート済み、後半逆順
    /// </summary>
    MountainShape,

    /// <summary>
    /// 前半逆順、後半ソート済み
    /// </summary>
    ValleyShape
}
