using SortAlgorithm.VisualizationWeb.Models;

namespace SortAlgorithm.VisualizationWeb.Services;

/// <summary>
/// 配列生成パターンに基づいて配列を生成するサービス
/// </summary>
public class ArrayPatternGenerator
{
    /// <summary>
    /// 指定されたパターンで配列を生成
    /// </summary>
    /// <param name="size">配列のサイズ</param>
    /// <param name="pattern">生成パターン</param>
    /// <param name="seed">乱数のシード（nullの場合は現在時刻を使用）</param>
    /// <returns>生成された配列</returns>
    public int[] Generate(int size, ArrayPattern pattern, int? seed = null)
    {
        var random = seed.HasValue ? new Random(seed.Value) : new Random();

        return pattern switch
        {
            ArrayPattern.Random => GenerateRandom(size, random),
            ArrayPattern.Sorted => GenerateSorted(size),
            ArrayPattern.Reversed => GenerateReversed(size),
            ArrayPattern.NearlySorted => GenerateNearlySorted(size, random),
            ArrayPattern.FewUnique => GenerateFewUnique(size, random),
            ArrayPattern.MountainShape => GenerateMountainShape(size),
            ArrayPattern.ValleyShape => GenerateValleyShape(size),
            _ => GenerateRandom(size, random)
        };
    }

    /// <summary>
    /// パターンの表示名を取得
    /// </summary>
    public string GetDisplayName(ArrayPattern pattern)
    {
        return pattern switch
        {
            ArrayPattern.Random => "🎲 Random",
            ArrayPattern.Sorted => "↗️ Sorted (Ascending)",
            ArrayPattern.Reversed => "↘️ Reversed (Descending)",
            ArrayPattern.NearlySorted => "≈ Nearly Sorted",
            ArrayPattern.FewUnique => "🔢 Few Unique Values",
            ArrayPattern.MountainShape => "⛰️ Mountain Shape",
            ArrayPattern.ValleyShape => "🏞️ Valley Shape",
            _ => pattern.ToString()
        };
    }

    /// <summary>
    /// ランダム配列を生成
    /// </summary>
    private int[] GenerateRandom(int size, Random random)
    {
        return Enumerable.Range(1, size).OrderBy(_ => random.Next()).ToArray();
    }

    /// <summary>
    /// ソート済み配列を生成（昇順）
    /// </summary>
    private int[] GenerateSorted(int size)
    {
        return Enumerable.Range(1, size).ToArray();
    }

    /// <summary>
    /// 逆順配列を生成（降順）
    /// </summary>
    private int[] GenerateReversed(int size)
    {
        return Enumerable.Range(1, size).Reverse().ToArray();
    }

    /// <summary>
    /// ほぼソート済み配列を生成（要素の10%をランダムに入れ替え）
    /// </summary>
    private int[] GenerateNearlySorted(int size, Random random)
    {
        var array = Enumerable.Range(1, size).ToArray();
        
        // 要素の10%をランダムに入れ替え
        var swapCount = Math.Max(1, size / 10);
        for (int i = 0; i < swapCount; i++)
        {
            var index1 = random.Next(size);
            var index2 = random.Next(size);
            (array[index1], array[index2]) = (array[index2], array[index1]);
        }
        
        return array;
    }

    /// <summary>
    /// 重複要素を多く含む配列を生成（ユニーク値は配列サイズの10%程度）
    /// </summary>
    private int[] GenerateFewUnique(int size, Random random)
    {
        var uniqueCount = Math.Max(1, size / 10);
        return Enumerable.Range(0, size)
            .Select(_ => random.Next(1, uniqueCount + 1))
            .ToArray();
    }

    /// <summary>
    /// 前半ソート済み、後半逆順の配列を生成
    /// </summary>
    private int[] GenerateMountainShape(int size)
    {
        var mid = size / 2;
        var firstHalf = Enumerable.Range(1, mid);
        var secondHalf = Enumerable.Range(mid + 1, size - mid).Reverse();
        return firstHalf.Concat(secondHalf).ToArray();
    }

    /// <summary>
    /// 前半逆順、後半ソート済みの配列を生成
    /// </summary>
    private int[] GenerateValleyShape(int size)
    {
        var mid = size / 2;
        var firstHalf = Enumerable.Range(mid + 1, size - mid).Reverse();
        var secondHalf = Enumerable.Range(1, mid);
        return firstHalf.Concat(secondHalf).ToArray();
    }
}
