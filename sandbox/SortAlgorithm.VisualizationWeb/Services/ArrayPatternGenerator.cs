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
            // Basic
            ArrayPattern.Random => GenerateRandom(size, random),
            ArrayPattern.Sorted => GenerateSorted(size),
            ArrayPattern.Reversed => GenerateReversed(size),
            
            // Nearly Sorted
            ArrayPattern.AlmostSorted => GenerateAlmostSorted(size, random),
            ArrayPattern.NearlySorted => GenerateNearlySorted(size, random),
            ArrayPattern.ScrambledTail => GenerateScrambledTail(size, random),
            ArrayPattern.ScrambledHead => GenerateScrambledHead(size, random),
            ArrayPattern.Noisy => GenerateNoisy(size, random),
            
            // Merge Patterns
            ArrayPattern.FinalMerge => GenerateFinalMerge(size),
            ArrayPattern.ShuffledFinalMerge => GenerateShuffledFinalMerge(size, random),
            ArrayPattern.Sawtooth => GenerateSawtooth(size),
            
            // Partitioned
            ArrayPattern.Partitioned => GeneratePartitioned(size, random),
            ArrayPattern.HalfSorted => GenerateHalfSorted(size, random),
            ArrayPattern.HalfReversed => GenerateHalfReversed(size),
            
            // Shape
            ArrayPattern.PipeOrgan => GeneratePipeOrgan(size),
            ArrayPattern.MountainShape => GenerateMountainShape(size),
            ArrayPattern.ValleyShape => GenerateValleyShape(size),
            
            // Radix/Interleaved
            ArrayPattern.FinalRadix => GenerateFinalRadix(size),
            ArrayPattern.Interlaced => GenerateInterlaced(size),
            ArrayPattern.Zigzag => GenerateZigzag(size),
            
            // Tree/Heap
            ArrayPattern.BstTraversal => GenerateBstTraversal(size, random),
            ArrayPattern.Heapified => GenerateHeapified(size),
            
            // Duplicates
            ArrayPattern.FewUnique => GenerateFewUnique(size, random),
            ArrayPattern.ManyDuplicates => GenerateManyDuplicates(size, random),
            ArrayPattern.AllEqual => GenerateAllEqual(size),
            
            // Distributions
            ArrayPattern.SineWave => GenerateSineWave(size),
            ArrayPattern.CosineWave => GenerateCosineWave(size),
            ArrayPattern.BellCurve => GenerateBellCurve(size),
            ArrayPattern.PerlinNoiseCurve => GeneratePerlinNoiseCurve(size, random),
            
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
            // Basic
            ArrayPattern.Random => "🎲 Random",
            ArrayPattern.Sorted => "↗️ Sorted (Ascending)",
            ArrayPattern.Reversed => "↘️ Reversed (Descending)",
            
            // Nearly Sorted
            ArrayPattern.AlmostSorted => "≈ Almost Sorted (5% Pair Swaps)",
            ArrayPattern.NearlySorted => "≈ Nearly Sorted (10% Random)",
            ArrayPattern.ScrambledTail => "📍 Scrambled Tail (14% at End)",
            ArrayPattern.ScrambledHead => "📍 Scrambled Head (14% at Start)",
            ArrayPattern.Noisy => "🔊 Noisy (Block Shuffled)",
            
            // Merge Patterns
            ArrayPattern.FinalMerge => "🔗 Final Merge (Even/Odd Sorted)",
            ArrayPattern.ShuffledFinalMerge => "🔗 Shuffled Final Merge",
            ArrayPattern.Sawtooth => "⚙️ Sawtooth (4-way Interleaved)",
            
            // Partitioned
            ArrayPattern.Partitioned => "📐 Partitioned (Halves Shuffled)",
            ArrayPattern.HalfSorted => "📊 Half Sorted",
            ArrayPattern.HalfReversed => "↕️ Half Reversed",
            
            // Shape
            ArrayPattern.PipeOrgan => "🎹 Pipe Organ",
            ArrayPattern.MountainShape => "⛰️ Mountain Shape",
            ArrayPattern.ValleyShape => "🏞️ Valley Shape",
            
            // Radix/Interleaved
            ArrayPattern.FinalRadix => "🔢 Final Radix Pass",
            ArrayPattern.Interlaced => "🔀 Interlaced",
            ArrayPattern.Zigzag => "〰️ Zigzag Pattern",
            
            // Tree/Heap
            ArrayPattern.BstTraversal => "🌳 BST In-Order Traversal",
            ArrayPattern.Heapified => "📚 Heapified (Max-Heap)",
            
            // Duplicates
            ArrayPattern.FewUnique => "🔢 Few Unique (3 Values)",
            ArrayPattern.ManyDuplicates => "🔢 Many Duplicates (20%)",
            ArrayPattern.AllEqual => "⚪ All Equal",
            
            // Distributions
            ArrayPattern.SineWave => "〰️ Sine Wave",
            ArrayPattern.CosineWave => "〰️ Cosine Wave",
            ArrayPattern.BellCurve => "🔔 Bell Curve (Normal)",
            ArrayPattern.PerlinNoiseCurve => "🌊 Perlin Noise Curve",
            
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

    /// <summary>
    /// ほぼソート済み配列（5%のペアをランダムスワップ）
    /// </summary>
    private int[] GenerateAlmostSorted(int size, Random random)
    {
        var array = Enumerable.Range(1, size).ToArray();
        var swapCount = Math.Max(1, size / 20);
        
        for (var i = 0; i < swapCount; i++)
        {
            var idx1 = random.Next(size);
            var idx2 = random.Next(size);
            (array[idx1], array[idx2]) = (array[idx2], array[idx1]);
        }
        
        return array;
    }

    /// <summary>
    /// スクランブル末尾（約14%の要素を末尾に抽出してシャッフル）
    /// </summary>
    private int[] GenerateScrambledTail(int size, Random random)
    {
        var array = Enumerable.Range(1, size).ToArray();
        var extracted = new List<int>();
        var kept = new List<int>();
        
        for (var i = 0; i < size; i++)
        {
            if (random.NextDouble() < 1.0 / 7.0)
                extracted.Add(array[i]);
            else
                kept.Add(array[i]);
        }
        
        // Shuffle extracted elements
        var shuffled = extracted.OrderBy(_ => random.Next()).ToArray();
        
        return [.. kept, .. shuffled];
    }

    /// <summary>
    /// スクランブル先頭（約14%の要素を先頭に抽出してシャッフル）
    /// </summary>
    private int[] GenerateScrambledHead(int size, Random random)
    {
        var array = Enumerable.Range(1, size).ToArray();
        var extracted = new List<int>();
        var kept = new List<int>();
        
        for (var i = size - 1; i >= 0; i--)
        {
            if (random.NextDouble() < 1.0 / 7.0)
                extracted.Add(array[i]);
            else
                kept.Insert(0, array[i]);
        }
        
        // Shuffle extracted elements
        var shuffled = extracted.OrderBy(_ => random.Next()).ToArray();
        
        return [.. shuffled, .. kept];
    }

    /// <summary>
    /// ノイズ入り（小ブロックごとにシャッフル）
    /// </summary>
    private int[] GenerateNoisy(int size, Random random)
    {
        var array = Enumerable.Range(1, size).ToArray();
        var blockSize = Math.Max(4, (int)(Math.Sqrt(size) / 2));
        
        for (var i = 0; i + blockSize <= size; i += random.Next(blockSize - 1) + 1)
        {
            var end = Math.Min(i + blockSize, size);
            var block = array[i..end].OrderBy(_ => random.Next()).ToArray();
            Array.Copy(block, 0, array, i, end - i);
        }
        
        return array;
    }

    /// <summary>
    /// 最終マージ状態（偶数・奇数インデックスが別々にソート済み）
    /// </summary>
    private int[] GenerateFinalMerge(int size)
    {
        var array = new int[size];
        var sorted = Enumerable.Range(1, size).ToArray();
        
        // Even indices get first half, odd indices get second half
        var evenIdx = 0;
        var oddIdx = 0;
        
        for (var i = 0; i < size; i++)
        {
            if (i % 2 == 0)
            {
                array[i] = sorted[evenIdx++];
            }
            else
            {
                array[i] = sorted[size / 2 + oddIdx++];
            }
        }
        
        return array;
    }

    /// <summary>
    /// シャッフル後最終マージ（全体をシャッフル後、前半と後半を別々にソート）
    /// </summary>
    private int[] GenerateShuffledFinalMerge(int size, Random random)
    {
        var array = Enumerable.Range(1, size).OrderBy(_ => random.Next()).ToArray();
        var mid = size / 2;
        
        Array.Sort(array, 0, mid);
        Array.Sort(array, mid, size - mid);
        
        return array;
    }

    /// <summary>
    /// ソートギア状（4-wayインターリーブでソート済み）
    /// </summary>
    private int[] GenerateSawtooth(int size)
    {
        var array = new int[size];
        var sorted = Enumerable.Range(1, size).ToArray();
        var indices = new[] { 0, 0, 0, 0 };
        
        for (var i = 0; i < size; i++)
        {
            var group = i % 4;
            var sourceIdx = group * (size / 4) + indices[group]++;
            array[i] = sorted[Math.Min(sourceIdx, size - 1)];
        }
        
        return array;
    }

    /// <summary>
    /// パーティション済み（ソート後、前半と後半を別々にシャッフル）
    /// </summary>
    private int[] GeneratePartitioned(int size, Random random)
    {
        var array = Enumerable.Range(1, size).ToArray();
        var mid = size / 2;
        
        var firstHalf = array[..mid].OrderBy(_ => random.Next()).ToArray();
        var secondHalf = array[mid..].OrderBy(_ => random.Next()).ToArray();
        
        return [.. firstHalf, .. secondHalf];
    }

    /// <summary>
    /// 半分反転（後半が逆順）
    /// </summary>
    private int[] GenerateHalfReversed(int size)
    {
        var array = Enumerable.Range(1, size).ToArray();
        var mid = size / 2;
        
        Array.Reverse(array, mid, size - mid);
        
        return array;
    }

    /// <summary>
    /// パイプオルガン型（偶数要素が前半、奇数要素が後半逆順）
    /// </summary>
    private int[] GeneratePipeOrgan(int size)
    {
        var array = new int[size];
        var sorted = Enumerable.Range(1, size).ToArray();
        var left = 0;
        var right = size - 1;
        
        for (var i = 0; i < size; i++)
        {
            if (i % 2 == 0)
            {
                array[left++] = sorted[i];
            }
            else
            {
                array[right--] = sorted[i];
            }
        }
        
        return array;
    }

    /// <summary>
    /// 最終基数パス（偶数・奇数要素が交互配置）
    /// </summary>
    private int[] GenerateFinalRadix(int size)
    {
        var array = new int[size];
        var sorted = Enumerable.Range(1, size).ToArray();
        var mid = size / 2;
        
        for (var i = 0; i < mid; i++)
        {
            array[i * 2] = sorted[mid + i];
            if (i * 2 + 1 < size)
                array[i * 2 + 1] = sorted[i];
        }
        
        return array;
    }

    /// <summary>
    /// インターレース（最小値を先頭、残りを両端から交互配置）
    /// </summary>
    private int[] GenerateInterlaced(int size)
    {
        var array = new int[size];
        var sorted = Enumerable.Range(1, size).ToArray();
        
        array[0] = sorted[0];
        var left = 1;
        var right = size - 1;
        
        for (var i = 1; i < size; i++)
        {
            if (i % 2 == 1)
                array[i] = sorted[right--];
            else
                array[i] = sorted[left++];
        }
        
        return array;
    }

    /// <summary>
    /// 二分探索木中順走査（ランダム挿入からの中順走査結果）
    /// </summary>
    private int[] GenerateBstTraversal(int size, Random random)
    {
        var values = Enumerable.Range(1, size).OrderBy(_ => random.Next()).ToArray();
        var bst = new SortedSet<int>();
        
        foreach (var value in values)
            bst.Add(value);
        
        return [.. bst];
    }

    /// <summary>
    /// ヒープ化済み（max-heap構造）
    /// </summary>
    private int[] GenerateHeapified(int size)
    {
        var array = Enumerable.Range(1, size).ToArray();
        
        // Build max-heap
        for (var i = size / 2 - 1; i >= 0; i--)
            Heapify(array, size, i);
        
        return array;
        
        static void Heapify(int[] arr, int n, int i)
        {
            var largest = i;
            var left = 2 * i + 1;
            var right = 2 * i + 2;
            
            if (left < n && arr[left] > arr[largest])
                largest = left;
            
            if (right < n && arr[right] > arr[largest])
                largest = right;
            
            if (largest != i)
            {
                (arr[i], arr[largest]) = (arr[largest], arr[i]);
                Heapify(arr, n, largest);
            }
        }
    }

    /// <summary>
    /// 少数ユニーク値（3種類の値）
    /// </summary>
    private int[] GenerateFewUnique(int size, Random random)
    {
        var values = new[] { size / 4, size / 2, size * 3 / 4 };
        var counts = new int[3];
        
        // Randomly distribute counts
        for (var i = 0; i < Math.Min(size, 8); i++)
        {
            if (random.NextDouble() < 0.5)
                counts[0]++;
        }
        counts[2] = size - counts[0];
        var remaining = Math.Min(size, 8) - counts[0];
        counts[2] = remaining;
        counts[1] = size - counts[0] - counts[2];
        
        var result = new List<int>();
        for (var i = 0; i < 3; i++)
            result.AddRange(Enumerable.Repeat(values[i], counts[i]));
        
        return [.. result];
    }

    /// <summary>
    /// 重複多数（ユニーク値は配列サイズの20%程度）
    /// </summary>
    private int[] GenerateManyDuplicates(int size, Random random)
    {
        var uniqueCount = Math.Max(10, Math.Min(40, size / 5));
        return Enumerable.Range(0, size)
            .Select(_ => random.Next(1, uniqueCount + 1))
            .ToArray();
    }

    /// <summary>
    /// 全要素同一
    /// </summary>
    private int[] GenerateAllEqual(int size)
    {
        return Enumerable.Repeat(size / 2, size).ToArray();
    }

    /// <summary>
    /// 正弦波分布
    /// </summary>
    private int[] GenerateSineWave(int size)
    {
        var array = new int[size];
        var n = size - 1;
        var c = 2 * Math.PI / n;

        for (var i = 0; i < size; i++)
        {
            array[i] = (int)(n * (Math.Sin(c * i) + 1) / 2) + 1;
        }
        
        return array;
    }

    /// <summary>
    /// 余弦波分布
    /// </summary>
    private int[] GenerateCosineWave(int size)
    {
        var array = new int[size];
        var n = size - 1;
        var c = 2 * Math.PI / n;

        for (var i = 0; i < size; i++)
        {
            array[i] = (int)(n * (Math.Cos(c * i) + 1) / 2) + 1;
        }
        
        return array;
    }

    /// <summary>
    /// ベル曲線分布（正規分布）
    /// </summary>
    private int[] GenerateBellCurve(int size)
    {
        var array = new int[size];
        var step = 8.0 / size;
        var position = -4.0;
        var constant = 1264;
        var factor = size / 512.0;
        
        for (var i = 0; i < size; i++)
        {
            var square = Math.Pow(position, 2);
            var halfNegSquare = -square / 2.0;
            var numerator = constant * factor * Math.Pow(Math.E, halfNegSquare);
            var denominator = Math.Sqrt(2 * Math.PI);
            
            array[i] = Math.Max(1, (int)(numerator / denominator));
            position += step;
        }
        
        return array;
    }

    /// <summary>
    /// パーリンノイズ曲線
    /// </summary>
    private int[] GeneratePerlinNoiseCurve(int size, Random random)
    {
        var array = new int[size];
        
        for (var i = 0; i < size; i++)
        {
            var x = (double)i / size;
            var noise = PerlinNoise(x, random);
            array[i] = Math.Max(1, Math.Min(size, (int)(noise * size)));
        }
        
        return array;
        
        static double PerlinNoise(double x, Random rnd)
        {
            var xi = (int)Math.Floor(x) & 255;
            var xf = x - Math.Floor(x);
            var u = Fade(xf);
            
            var a = rnd.Next(256);
            var b = rnd.Next(256);
            
            return Lerp(u, Grad(a, xf), Grad(b, xf - 1));
            
            static double Fade(double t) => t * t * t * (t * (t * 6 - 15) + 10);
            static double Lerp(double t, double a, double b) => a + t * (b - a);
            static double Grad(int hash, double x) => (hash & 1) == 0 ? x : -x;
        }
    }
}
