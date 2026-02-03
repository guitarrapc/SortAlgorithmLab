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
            ArrayPattern.NaiveShuffle => GenerateNaiveShuffle(size, random),
            ArrayPattern.SingleElementMoved => GenerateSingleElementMoved(size, random),
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
            
            // Advanced/Fractal
            ArrayPattern.CirclePass => GenerateCirclePass(size, random),
            ArrayPattern.PairwisePass => GeneratePairwisePass(size, random),
            ArrayPattern.RecursiveReversal => GenerateRecursiveReversal(size),
            ArrayPattern.GrayCodeFractal => GenerateGrayCodeFractal(size),
            ArrayPattern.SierpinskiTriangle => GenerateSierpinskiTriangle(size),
            ArrayPattern.Triangular => GenerateTriangular(size),
            
            // Adversarial
            ArrayPattern.QuickSortAdversary => GenerateQuickSortAdversary(size),
            ArrayPattern.PdqSortAdversary => GeneratePdqSortAdversary(size),
            ArrayPattern.GrailSortAdversary => GenerateGrailSortAdversary(size, random),
            ArrayPattern.ShuffleMergeAdversary => GenerateShuffleMergeAdversary(size),
            
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
            ArrayPattern.NaiveShuffle => "🔀 Naive Shuffle",
            ArrayPattern.SingleElementMoved => "➡️ Single Element Moved",
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
            
            // Advanced/Fractal
            ArrayPattern.CirclePass => "⭕ Circle Sort Pass",
            ArrayPattern.PairwisePass => "🔗 Pairwise Pass",
            ArrayPattern.RecursiveReversal => "🔄 Recursive Reversal",
            ArrayPattern.GrayCodeFractal => "🔲 Gray Code Fractal",
            ArrayPattern.SierpinskiTriangle => "🔺 Sierpinski Triangle",
            ArrayPattern.Triangular => "🔻 Triangular",
            
            // Adversarial
            ArrayPattern.QuickSortAdversary => "⚔️ QuickSort Adversary",
            ArrayPattern.PdqSortAdversary => "⚔️ PDQ Adversary",
            ArrayPattern.GrailSortAdversary => "⚔️ Grail Adversary",
            ArrayPattern.ShuffleMergeAdversary => "⚔️ ShuffleMerge Adversary",
            
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
    /// ナイーブシャッフル（各要素を順番にランダム位置とスワップ）
    /// Fisher-Yatesの間違った実装パターン
    /// </summary>
    private int[] GenerateNaiveShuffle(int size, Random random)
    {
        var array = Enumerable.Range(1, size).ToArray();
        
        // Naive shuffle: swap each element with a random position (including itself)
        // This is NOT the correct Fisher-Yates algorithm
        for (var i = 0; i < size; i++)
        {
            var randomIndex = random.Next(size);
            (array[i], array[randomIndex]) = (array[randomIndex], array[i]);
        }
        
        return array;
    }

    /// <summary>
    /// 単一要素移動（ソート済みから1つの要素だけをランダム位置に移動）
    /// </summary>
    private int[] GenerateSingleElementMoved(int size, Random random)
    {
        var array = Enumerable.Range(1, size).ToArray();
        
        if (size < 2) return array;
        
        // Pick a random element to move
        var sourceIndex = random.Next(size);
        var destIndex = random.Next(size);
        
        if (sourceIndex == destIndex) return array;
        
        // Move element using rotation
        var element = array[sourceIndex];
        
        if (destIndex < sourceIndex)
        {
            // Shift elements right
            Array.Copy(array, destIndex, array, destIndex + 1, sourceIndex - destIndex);
            array[destIndex] = element;
        }
        else
        {
            // Shift elements left
            Array.Copy(array, sourceIndex + 1, array, sourceIndex, destIndex - sourceIndex);
            array[destIndex] = element;
        }
        
        return array;
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

    // Advanced/Fractal Patterns

    /// <summary>
    /// サークルソート初回パス（シャッフル後にサークルソート1パスを適用）
    /// </summary>
    private int[] GenerateCirclePass(int size, Random random)
    {
        var array = Enumerable.Range(1, size).OrderBy(_ => random.Next()).ToArray();
        
        // Calculate power of 2 >= size
        var n = 1;
        while (n < size) n *= 2;
        
        CircleSortRoutine(array, 0, n - 1, size);
        
        return array;
        
        static void CircleSortRoutine(int[] arr, int lo, int hi, int end)
        {
            if (lo == hi) return;
            
            var low = lo;
            var high = hi;
            var mid = (hi - lo) / 2;
            
            while (lo < hi)
            {
                if (hi < end && arr[lo] > arr[hi])
                    (arr[lo], arr[hi]) = (arr[hi], arr[lo]);
                lo++;
                hi--;
            }
            
            CircleSortRoutine(arr, low, low + mid, end);
            if (low + mid + 1 < end)
            {
                CircleSortRoutine(arr, low + mid + 1, high, end);
            }
        }
    }

    /// <summary>
    /// ペアワイズ最終パス（隣接ペアがソート済み、全体としてはランダム）
    /// </summary>
    private int[] GeneratePairwisePass(int size, Random random)
    {
        var array = Enumerable.Range(1, size).OrderBy(_ => random.Next()).ToArray();
        
        // Sort adjacent pairs
        for (var i = 1; i < size; i += 2)
        {
            if (array[i - 1] > array[i])
            {
                (array[i - 1], array[i]) = (array[i], array[i - 1]);
            }
        }
        
        // Use pigeonhole sort on even/odd indices separately
        // Values are 1..size, so we need counts array of size+1
        for (var m = 0; m < 2; m++)
        {
            var counts = new int[size + 1];

            // Count occurrences
            for (var k = m; k < size; k += 2)
            {
                counts[array[k]]++;
            }
            
            // Place elements back
            var j = m;
            for (var i = 1; i <= size; i++)
            {
                while (counts[i] > 0 && j < size)
                {
                    array[j] = i;
                    j += 2;
                    counts[i]--;
                }
            }
        }
        
        return array;
    }

    /// <summary>
    /// 再帰的反転（配列全体を反転後、再帰的に半分ずつ反転）
    /// </summary>
    private int[] GenerateRecursiveReversal(int size)
    {
        var array = Enumerable.Range(1, size).ToArray();
        ReversalRecursive(array, 0, size);
        return array;
        
        static void ReversalRecursive(int[] arr, int a, int b)
        {
            if (b - a < 2) return;
            
            Array.Reverse(arr, a, b - a);
            
            var m = (a + b) / 2;
            ReversalRecursive(arr, a, m);
            ReversalRecursive(arr, m, b);
        }
    }

    /// <summary>
    /// グレイコードフラクタル（グレイコードに基づく再帰的反転パターン）
    /// </summary>
    private int[] GenerateGrayCodeFractal(int size)
    {
        var array = Enumerable.Range(1, size).ToArray();
        GrayCodeRecursive(array, 0, size, false);
        return array;
        
        static void GrayCodeRecursive(int[] arr, int a, int b, bool backward)
        {
            if (b - a < 3) return;
            
            var m = (a + b) / 2;

            if (backward)
            {
                Array.Reverse(arr, a, m - a);
            }
            else
            {
                Array.Reverse(arr, m, b - m);
            }
            
            GrayCodeRecursive(arr, a, m, false);
            GrayCodeRecursive(arr, m, b, true);
        }
    }

    /// <summary>
    /// シェルピンスキー三角形（フラクタルパターン）
    /// </summary>
    private int[] GenerateSierpinskiTriangle(int size)
    {
        var triangle = new int[size];
        TriangleRecursive(triangle, 0, size);
        
        var sorted = Enumerable.Range(1, size).ToArray();
        var result = new int[size];

        for (var i = 0; i < size; i++)
        {
            result[i] = sorted[triangle[i]];
        }
        
        return result;
        
        static void TriangleRecursive(int[] arr, int a, int b)
        {
            if (b - a < 2) return;
            if (b - a == 2)
            {
                arr[a + 1]++;
                return;
            }
            
            var h = (b - a) / 3;
            var t1 = (a + a + b) / 3;
            var t2 = (a + b + b + 2) / 3;
            
            for (var i = a; i < t1; i++) arr[i] += h;
            for (var i = t1; i < t2; i++) arr[i] += 2 * h;
            
            TriangleRecursive(arr, a, t1);
            TriangleRecursive(arr, t1, t2);
            TriangleRecursive(arr, t2, b);
        }
    }

    /// <summary>
    /// 三角数配列（三角数の階層構造）
    /// </summary>
    private int[] GenerateTriangular(int size)
    {
        var triangle = new int[size];
        var j = 0;
        var k = 2;
        var max = 0;
        
        for (var i = 1; i < size; i++, j++)
        {
            if (i == k)
            {
                j = 0;
                k *= 2;
            }
            triangle[i] = triangle[j] + 1;
            if (triangle[i] > max) max = triangle[i];
        }
        
        // Counting sort to get indices
        var counts = new int[max + 1];
        for (var i = 0; i < size; i++)
            counts[triangle[i]]++;
        
        for (var i = 1; i < counts.Length; i++)
            counts[i] += counts[i - 1];
        
        for (var i = size - 1; i >= 0; i--)
            triangle[i] = --counts[triangle[i]];
        
        var sorted = Enumerable.Range(1, size).ToArray();
        var result = new int[size];
        
        for (var i = 0; i < size; i++)
            result[i] = sorted[triangle[i]];
        
        return result;
    }

    // Adversarial Patterns

    /// <summary>
    /// QuickSort最悪ケース（median-of-3 pivot選択用）
    /// </summary>
    private int[] GenerateQuickSortAdversary(int size)
    {
        var array = Enumerable.Range(1, size).ToArray();
        
        // Swap elements to create worst case for median-of-3 quicksort
        for (int j = size - size % 2 - 2, i = j - 1; i >= 0; i -= 2, j--)
            (array[i], array[j]) = (array[j], array[i]);
        
        return array;
    }

    /// <summary>
    /// PDQソート最悪ケース（Pattern-defeating QuickSort用）
    /// 注：完全な実装は非常に複雑なため、簡略版
    /// </summary>
    private int[] GeneratePdqSortAdversary(int size)
    {
        // Simplified PDQ adversary: reverse sorted with strategic swaps
        var array = Enumerable.Range(1, size).Reverse().ToArray();
        
        // Create imbalance patterns that PDQ struggles with
        var blockSize = Math.Max(1, size / 8);
        for (var i = 0; i + blockSize < size; i += blockSize * 2)
        {
            var end = Math.Min(i + blockSize, size);
            Array.Sort(array, i, end - i);
        }
        
        return array;
    }

    /// <summary>
    /// Grailソート最悪ケース
    /// </summary>
    private int[] GenerateGrailSortAdversary(int size, Random random)
    {
        if (size <= 16)
        {
            return Enumerable.Range(1, size).Reverse().ToArray();
        }
        
        var blockLen = 1;
        while (blockLen * blockLen < size)
        {
            blockLen *= 2;
        }
        
        var numKeys = (size - 1) / blockLen + 1;
        var keys = blockLen + numKeys;
        
        var array = Enumerable.Range(1, size).OrderBy(_ => random.Next()).ToArray();
        
        // Sort and reverse the keys section
        Array.Sort(array, 0, keys);
        Array.Reverse(array, 0, keys);
        
        // Sort the remaining section
        Array.Sort(array, keys, size - keys);
        
        return array;
    }

    /// <summary>
    /// ShuffleMerge最悪ケース
    /// </summary>
    private int[] GenerateShuffleMergeAdversary(int size)
    {
        var array = Enumerable.Range(1, size).ToArray();
        var temp = new int[size];
        var d = 2;
        var end = 1 << (int)(Math.Log(size - 1) / Math.Log(2) + 1);
        
        while (d <= end)
        {
            var i = 0;
            var dec = 0;
            
            while (i < size)
            {
                var j = i;
                dec += size;
                while (dec >= d)
                {
                    dec -= d;
                    j++;
                }
                
                var k = j;
                dec += size;
                while (dec >= d)
                {
                    dec -= d;
                    k++;
                }
                
                // Reverse merge the sections
                var mid = j;
                Array.Copy(array, i, temp, i, mid - i);
                Array.Copy(array, mid, temp, mid, k - mid);
                Array.Reverse(temp, i, mid - i);
                Array.Copy(temp, i, array, i, k - i);
                
                i = k;
            }
            d *= 2;
        }
        
        return array;
    }
}
