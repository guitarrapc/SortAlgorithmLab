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
            ArrayPattern.NearlySortedLast => GenerateNearlySortedLast(size, random),
            ArrayPattern.NearlySortedStart => GenerateNearlySortedStart(size, random),
            ArrayPattern.FewUnique => GenerateFewUnique(size, random),
            ArrayPattern.ManyDuplicates => GenerateManyDuplicates(size, random),
            ArrayPattern.MountainShape => GenerateMountainShape(size),
            ArrayPattern.ValleyShape => GenerateValleyShape(size),
            ArrayPattern.Zigzag => GenerateZigzag(size),
            ArrayPattern.HalfSorted => GenerateHalfSorted(size, random),
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
            ArrayPattern.NearlySorted => "≈ Nearly Sorted (10% Random)",
            ArrayPattern.NearlySortedLast => "≈ Nearly Sorted (Last 10% Shuffled)",
            ArrayPattern.NearlySortedStart => "≈ Nearly Sorted (Start 10% Shuffled)",
            ArrayPattern.FewUnique => "🔢 Few Unique (Max 20 Values)",
            ArrayPattern.ManyDuplicates => "🔢 Many Duplicates (Max 40 Values)",
            ArrayPattern.MountainShape => "⛰️ Mountain Shape",
            ArrayPattern.ValleyShape => "🏞️ Valley Shape",
            ArrayPattern.Zigzag => "〰️ Zigzag Pattern",
            ArrayPattern.HalfSorted => "📊 Half Sorted",
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
    /// ほぼソート済み配列を生成（最後の10%のみシャッフル）
    /// </summary>
    private int[] GenerateNearlySortedLast(int size, Random random)
    {
        var array = Enumerable.Range(1, size).ToArray();

        // 最後の10%をシャッフル
        var shuffleStart = size - Math.Max(1, size / 10);
        var shuffleCount = size - shuffleStart;

        for (int i = 0; i < shuffleCount; i++)
        {
            var index1 = random.Next(shuffleStart, size);
            var index2 = random.Next(shuffleStart, size);
            (array[index1], array[index2]) = (array[index2], array[index1]);
        }

        return array;
    }

    /// <summary>
    /// ほぼソート済み配列を生成（最初の10%のみシャッフル）
    /// </summary>
    private int[] GenerateNearlySortedStart(int size, Random random)
    {
        var array = Enumerable.Range(1, size).ToArray();

        // 最初の10%をシャッフル
        var shuffleEnd = Math.Max(1, size / 10);

        for (int i = 0; i < shuffleEnd; i++)
        {
            var index1 = random.Next(0, shuffleEnd);
            var index2 = random.Next(0, shuffleEnd);
            (array[index1], array[index2]) = (array[index2], array[index1]);
        }

        return array;
    }

    /// <summary>
    /// 重複要素を多く含む配列を生成（ユニーク値は最大20個程度）
    /// </summary>
    private int[] GenerateFewUnique(int size, Random random)
    {
        // ユニーク値の数を最大20個に制限（小さい配列では10%を使用）
        var uniqueCount = Math.Max(5, Math.Min(20, size / 10));
        return Enumerable.Range(0, size)
            .Select(_ => random.Next(1, uniqueCount + 1))
            .ToArray();
    }

    /// <summary>
    /// 重複多数の配列を生成（ユニーク値は最大40個程度）
    /// </summary>
    private int[] GenerateManyDuplicates(int size, Random random)
    {
        // ユニーク値の数を最大40個に制限（小さい配列では20%を使用）
        var uniqueCount = Math.Max(10, Math.Min(40, size / 5));
        return Enumerable.Range(0, size)
            .Select(_ => random.Next(1, uniqueCount + 1))
            .ToArray();
    }

    /// <summary>
    /// 山型の配列を生成（中央が最大値）
    /// </summary>
    private int[] GenerateMountainShape(int size)
    {
        var array = new int[size];
        var values = Enumerable.Range(1, size).ToArray();
        
        // 小さい値から大きい値へ、そして大きい値から小さい値へ
        int left = 0;
        int right = size - 1;
        
        for (int i = 0; i < size; i++)
        {
            if (i % 2 == 0)
            {
                // 左側に小さい値を配置
                array[left++] = values[i];
            }
            else
            {
                // 右側に小さい値を配置
                array[right--] = values[i];
            }
        }
        
        return array;
    }

    /// <summary>
    /// 谷型の配列を生成（中央が最小値）
    /// </summary>
    private int[] GenerateValleyShape(int size)
    {
        var array = new int[size];
        var values = Enumerable.Range(1, size).Reverse().ToArray();
        
        // 大きい値から小さい値へ、そして小さい値から大きい値へ
        int left = 0;
        int right = size - 1;
        
        for (int i = 0; i < size; i++)
        {
            if (i % 2 == 0)
            {
                // 左側に大きい値を配置
                array[left++] = values[i];
            }
            else
            {
                // 右側に大きい値を配置
                array[right--] = values[i];
            }
        }
        
        return array;
    }

    /// <summary>
    /// ジグザグパターンの配列を生成（交互に上下する）
    /// </summary>
    private int[] GenerateZigzag(int size)
    {
        var array = new int[size];
        
        // 小さい値と大きい値を交互に配置
        var lowValues = Enumerable.Range(1, size / 2).ToList();
        var highValues = Enumerable.Range(size / 2 + 1, size - size / 2).ToList();
        
        for (int i = 0; i < size; i++)
        {
            if (i % 2 == 0)
            {
                // 偶数インデックス: 小さい値
                var index = i / 2;
                array[i] = index < lowValues.Count ? lowValues[index] : highValues[i - lowValues.Count];
            }
            else
            {
                // 奇数インデックス: 大きい値
                var index = i / 2;
                array[i] = index < highValues.Count ? highValues[index] : lowValues[i - highValues.Count];
            }
        }
        
        return array;
    }

    /// <summary>
    /// 半分ソート済みの配列を生成（前半のみソート済み、後半はランダム）
    /// </summary>
    private int[] GenerateHalfSorted(int size, Random random)
    {
        var mid = size / 2;
        var firstHalf = Enumerable.Range(1, mid).ToArray();
        var secondHalf = Enumerable.Range(mid + 1, size - mid).OrderBy(_ => random.Next()).ToArray();
        return firstHalf.Concat(secondHalf).ToArray();
    }
}
