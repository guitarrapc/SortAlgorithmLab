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
            ArrayPattern.SingleElementMoved => GenerateSingleElementMoved(size, random),
            ArrayPattern.AlmostSorted => GenerateAlmostSorted(size, random),
            ArrayPattern.NearlySorted => GenerateNearlySorted(size, random),
            ArrayPattern.ScrambledTail => GenerateScrambledTail(size, random),
            ArrayPattern.ScrambledHead => GenerateScrambledHead(size, random),
            ArrayPattern.Noisy => GenerateNoisy(size, random),
            ArrayPattern.ShuffledOdds => GenerateShuffledOdds(size, random),
            ArrayPattern.ShuffledHalf => GenerateShuffledHalf(size, random),
            ArrayPattern.EvensReversedOddsInOrder => GenerateEvensReversedOddsInOrder(size),
            ArrayPattern.DoubleLayered => GenerateDoubleLayered(size),

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
            ArrayPattern.RealFinalRadix => GenerateRealFinalRadix(size),
            ArrayPattern.RecursiveFinalRadix => GenerateRecursiveFinalRadix(size),
            ArrayPattern.FinalBitonicPass => GenerateFinalBitonicPass(size),
            ArrayPattern.BitReversal => GenerateBitReversal(size),
            ArrayPattern.BlockRandomly => GenerateBlockRandomly(size, random),
            ArrayPattern.BlockReverse => GenerateBlockReverse(size),
            ArrayPattern.Interlaced => GenerateInterlaced(size),
            ArrayPattern.Zigzag => GenerateZigzag(size),

            // Tree/Heap
            ArrayPattern.BstTraversal => GenerateBstTraversal(size, random),
            ArrayPattern.InvertedBst => GenerateInvertedBst(size),
            ArrayPattern.LogarithmicSlopes => GenerateLogarithmicSlopes(size),
            ArrayPattern.HalfRotation => GenerateHalfRotation(size),
            ArrayPattern.Heapified => GenerateHeapified(size),
            ArrayPattern.PoplarHeapified => GeneratePoplarHeapified(size),
            ArrayPattern.TriangularHeapified => GenerateTriangularHeapified(size),

            // Duplicates
            ArrayPattern.FewUnique => GenerateFewUnique(size, random),
            ArrayPattern.ManyDuplicates => GenerateManyDuplicates(size, random),
            ArrayPattern.AllEqual => GenerateAllEqual(size),

            // Distributions
            ArrayPattern.QuadraticDistribution => GenerateQuadraticDistribution(size),
            ArrayPattern.SquareRootDistribution => GenerateSquareRootDistribution(size),
            ArrayPattern.CubicDistribution => GenerateCubicDistribution(size),
            ArrayPattern.QuinticDistribution => GenerateQuinticDistribution(size),
            ArrayPattern.CubeRootDistribution => GenerateCubeRootDistribution(size),
            ArrayPattern.FifthRootDistribution => GenerateFifthRootDistribution(size),
            ArrayPattern.SineWave => GenerateSineWave(size),
            ArrayPattern.CosineWave => GenerateCosineWave(size),
            ArrayPattern.BellCurve => GenerateBellCurve(size),
            ArrayPattern.PerlinNoiseCurve => GeneratePerlinNoiseCurve(size, random),
            ArrayPattern.RulerDistribution => GenerateRulerDistribution(size),
            ArrayPattern.BlancmangeDistribution => GenerateBlancmangeDistribution(size),
            ArrayPattern.CantorDistribution => GenerateCantorDistribution(size),
            ArrayPattern.DivisorsDistribution => GenerateDivisorsDistribution(size),
            ArrayPattern.FsdDistribution => GenerateFsdDistribution(size),
            ArrayPattern.ReverseLogDistribution => GenerateReverseLogDistribution(size, random),
            ArrayPattern.ModuloDistribution => GenerateModuloDistribution(size),
            ArrayPattern.TotientDistribution => GenerateTotientDistribution(size),

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
            ArrayPattern.SingleElementMoved => "➡️ Single Element Moved",
            ArrayPattern.AlmostSorted => "≈ Almost Sorted (5% Pair Swaps)",
            ArrayPattern.NearlySorted => "≈ Nearly Sorted (10% Random)",
            ArrayPattern.ScrambledTail => "📍 Scrambled Tail (14% at End)",
            ArrayPattern.ScrambledHead => "📍 Scrambled Head (14% at Start)",
            ArrayPattern.Noisy => "🔊 Noisy (Block Shuffled)",
            ArrayPattern.ShuffledOdds => "🔢 Shuffled Odds Only",
            ArrayPattern.ShuffledHalf => "📊 Shuffled Half (Front Sorted)",
            ArrayPattern.EvensReversedOddsInOrder => "⇅ Evens Reversed, Odds In-Order",
            ArrayPattern.DoubleLayered => "🔄 Double Layered (Symmetric Swap)",

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
            ArrayPattern.RealFinalRadix => "🔢 Real Final Radix (Bitmask)",
            ArrayPattern.RecursiveFinalRadix => "🔢 Recursive Final Radix",
            ArrayPattern.FinalBitonicPass => "🔄 Final Bitonic Pass",
            ArrayPattern.BitReversal => "🔁 Bit Reversal (FFT)",
            ArrayPattern.BlockRandomly => "🧱 Block Randomly Shuffled",
            ArrayPattern.BlockReverse => "🧱 Block Reversed",
            ArrayPattern.Interlaced => "🔀 Interlaced",
            ArrayPattern.Zigzag => "〰️ Zigzag Pattern",

            // Tree/Heap
            ArrayPattern.BstTraversal => "🌳 BST In-Order Traversal",
            ArrayPattern.InvertedBst => "🌳 Inverted BST",
            ArrayPattern.LogarithmicSlopes => "📈 Logarithmic Slopes",
            ArrayPattern.HalfRotation => "🔄 Half Rotation",
            ArrayPattern.Heapified => "📚 Heapified (Max-Heap)",
            ArrayPattern.PoplarHeapified => "📚 Poplar Heapified",
            ArrayPattern.TriangularHeapified => "📚 Triangular Heapified",

            // Duplicates
            ArrayPattern.FewUnique => "🔢 Few Unique (3 Values)",
            ArrayPattern.ManyDuplicates => "🔢 Many Duplicates (20%)",
            ArrayPattern.AllEqual => "⚪ All Equal",

            // Distributions
            ArrayPattern.QuadraticDistribution => "📊 Quadratic (x²)",
            ArrayPattern.SquareRootDistribution => "📊 Square Root (√x)",
            ArrayPattern.CubicDistribution => "📊 Cubic (x³ Centered)",
            ArrayPattern.QuinticDistribution => "📊 Quintic (x⁵ Centered)",
            ArrayPattern.CubeRootDistribution => "📊 Cube Root (∛x)",
            ArrayPattern.FifthRootDistribution => "📊 Fifth Root (⁵√x)",
            ArrayPattern.SineWave => "〰️ Sine Wave",
            ArrayPattern.CosineWave => "〰️ Cosine Wave",
            ArrayPattern.BellCurve => "🔔 Bell Curve (Normal)",
            ArrayPattern.PerlinNoiseCurve => "🌊 Perlin Noise Curve",
            ArrayPattern.RulerDistribution => "📐 Ruler Function",
            ArrayPattern.BlancmangeDistribution => "🍮 Blancmange Curve",
            ArrayPattern.CantorDistribution => "∞ Cantor Function",
            ArrayPattern.DivisorsDistribution => "➗ Sum of Divisors",
            ArrayPattern.FsdDistribution => "✈️ Fly Straight Dangit",
            ArrayPattern.ReverseLogDistribution => "📉 Reverse Log",
            ArrayPattern.ModuloDistribution => "% Modulo Function",
            ArrayPattern.TotientDistribution => "φ Euler Totient",

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
            {
                extracted.Add(array[i]);
            }
            else
            {
                kept.Add(array[i]);
            }
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
            {
                extracted.Add(array[i]);
            }
            else
            {
                kept.Insert(0, array[i]);
            }
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
    /// 奇数インデックスのみシャッフル（偶数インデックスはソート済み）
    /// </summary>
    private int[] GenerateShuffledOdds(int size, Random random)
    {
        var array = Enumerable.Range(1, size).ToArray();

        // Fisher-Yates shuffle but only for odd indices
        for (var i = 1; i < size; i += 2)
        {
            // Random odd index from current position to end
            var randomOddIndex = (random.Next((size - i) / 2) * 2) + i;
            (array[i], array[randomOddIndex]) = (array[randomOddIndex], array[i]);
        }

        return array;
    }

    /// <summary>
    /// 半分シャッフル（全体をシャッフル後、前半のみソート）
    /// </summary>
    private int[] GenerateShuffledHalf(int size, Random random)
    {
        // Shuffle entire array
        var array = Enumerable.Range(1, size).OrderBy(_ => random.Next()).ToArray();

        // Sort only the first half
        var mid = size / 2;
        Array.Sort(array, 0, mid);

        return array;
    }

    /// <summary>
    /// ダブルレイヤー（偶数インデックスを対称位置とスワップ）
    /// </summary>
    private int[] GenerateDoubleLayered(int size)
    {
        var array = Enumerable.Range(1, size).ToArray();

        // Swap even indices with their symmetric positions
        for (var i = 0; i < size / 2; i += 2)
        {
            (array[i], array[size - i - 1]) = (array[size - i - 1], array[i]);
        }

        return array;
    }

    /// <summary>
    /// 偶数値逆順・奇数値順序（偶数の値を逆順に、奇数の値を順序通りに配置）
    /// </summary>
    private int[] GenerateEvensReversedOddsInOrder(int size)
    {
        var evens = new List<int>();
        var odds = new List<int>();

        // Separate even and odd values
        for (var i = 1; i <= size; i++)
        {
            if (i % 2 == 0)
            {
                evens.Add(i);
            }
            else
            {
                odds.Add(i);
            }
        }

        // Reverse even values
        evens.Reverse();

        // Interleave odds (in order) and evens (reversed)
        var array = new int[size];
        var evenIdx = 0;
        var oddIdx = 0;

        for (var i = 0; i < size; i++)
        {
            if ((i + 1) % 2 == 0 && evenIdx < evens.Count)
            {
                // Position for even value
                array[i] = evens[evenIdx++];
            }
            else if (oddIdx < odds.Count)
            {
                // Position for odd value
                array[i] = odds[oddIdx++];
            }
            else if (evenIdx < evens.Count)
            {
                // Fill remaining with evens
                array[i] = evens[evenIdx++];
            }
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
    /// ArrayVのSawtoothパターン：4つの連続した上昇グループを生成
    /// </summary>
    private int[] GenerateSawtooth(int size)
    {
        var sorted = Enumerable.Range(1, size).ToArray();
        var result = new int[size];
        const int count = 4;
        var k = 0;

        // 4-wayインターリーブ：各グループの要素を順番に収集
        for (var j = 0; j < count; j++)
        {
            for (var i = j; i < size; i += count)
            {
                result[k++] = sorted[i];
            }
        }

        return result;
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
            {
                array[i * 2 + 1] = sorted[i];
            }
        }

        return array;
    }

    /// <summary>
    /// 真の最終基数パス（ビットマスクベースの基数ソート）
    /// </summary>
    private int[] GenerateRealFinalRadix(int size)
    {
        var array = Enumerable.Range(1, size).ToArray();

        // Calculate bit mask (highest bit position)
        var mask = 0;
        for (var i = 0; i < size; i++)
        {
            while (mask < array[i])
            {
                mask = (mask << 1) + 1;
            }
        }
        mask >>= 1;

        // Counting sort by masked bits
        var counts = new int[mask + 2];
        var temp = new int[size];
        Array.Copy(array, temp, size);

        for (var i = 0; i < size; i++)
        {
            counts[(array[i] & mask) + 1]++;
        }

        for (var i = 1; i < counts.Length; i++)
        {
            counts[i] += counts[i - 1];
        }

        var result = new int[size];
        for (var i = 0; i < size; i++)
        {
            result[counts[temp[i] & mask]++] = temp[i];
        }

        return result;
    }

    /// <summary>
    /// 再帰的最終基数パス（再帰的インターリーブ）
    /// </summary>
    private int[] GenerateRecursiveFinalRadix(int size)
    {
        var array = Enumerable.Range(1, size).ToArray();
        WeaveRecursive(array, 0, size, 1);
        return array;

        static void WeaveRecursive(int[] arr, int pos, int length, int gap)
        {
            if (length < 2) return;

            var mod2 = length % 2;
            length -= mod2;
            var mid = length / 2;
            var temp = new int[mid];

            // Extract first half
            for (int i = pos, j = 0; i < pos + gap * mid; i += gap, j++)
            {
                temp[j] = arr[i];
            }

            // Interleave
            for (int i = pos + gap * mid, j = pos, k = 0; i < pos + gap * length; i += gap, j += 2 * gap, k++)
            {
                arr[j] = arr[i];
                arr[j + gap] = temp[k];
            }

            WeaveRecursive(arr, pos, mid + mod2, 2 * gap);
            WeaveRecursive(arr, pos + gap, mid, 2 * gap);
        }
    }

    /// <summary>
    /// 最終バイトニックパス（配列を反転後にPipe Organ配置）
    /// </summary>
    private int[] GenerateFinalBitonicPass(int size)
    {
        var array = Enumerable.Range(1, size).ToArray();

        // Reverse the array
        Array.Reverse(array);

        // Create pipe organ pattern (even indices go to front, odd indices go to back reversed)
        var temp = new int[size];
        var front = 0;
        var back = size;

        for (var i = 0; i < size; i++)
        {
            if (i % 2 == 0)
            {
                temp[front++] = array[i];
            }
            else
            {
                temp[--back] = array[i];
            }
        }

        return temp;
    }

    /// <summary>
    /// ビット反転順序（FFT用のビット反転配列）
    /// </summary>
    private int[] GenerateBitReversal(int size)
    {
        var len = 1 << (int)(Math.Log(size) / Math.Log(2));
        var temp = Enumerable.Range(1, size).ToArray();
        var array = new int[size];

        // Initialize with indices
        for (var i = 0; i < len; i++)
        {
            array[i] = i;
        }

        // Bit reversal permutation
        var m = 0;
        var d1 = len >> 1;
        var d2 = d1 + (d1 >> 1);

        for (var i = 1; i < len - 1; i++)
        {
            var j = d1;

            for (int k = i, n = d2; (k & 1) == 0; j -= n, k >>= 1, n >>= 1)
            { }

            m += j;
            if (m > i)
            {
                (array[i], array[m]) = (array[m], array[i]);
            }
        }

        // Map back to values
        var result = new int[size];
        for (var i = 0; i < len && i < size; i++)
        {
            result[i] = temp[array[i] % size];
        }

        for (var i = len; i < size; i++)
        {
            result[i] = temp[i];
        }

        return result;
    }

    /// <summary>
    /// ブロックごとにランダムシャッフル
    /// </summary>
    private int[] GenerateBlockRandomly(int size, Random random)
    {
        var array = Enumerable.Range(1, size).ToArray();
        var blockSize = Pow2LessThanOrEqual((int)Math.Sqrt(size));
        var adjustedSize = size - (size % blockSize);

        // Fisher-Yates shuffle but on blocks
        for (var i = 0; i < adjustedSize; i += blockSize)
        {
            var randomBlock = random.Next((adjustedSize - i) / blockSize) * blockSize + i;
            BlockSwap(array, i, randomBlock, blockSize);
        }

        return array;

        static void BlockSwap(int[] arr, int a, int b, int length)
        {
            for (var i = 0; i < length; i++)
            {
                (arr[a + i], arr[b + i]) = (arr[b + i], arr[a + i]);
            }
        }

        static int Pow2LessThanOrEqual(int value)
        {
            var val = 1;
            while (val <= value)
            {
                val <<= 1;
            }
            return val >> 1;
        }
    }

    /// <summary>
    /// ブロックごとに反転（ブロック順序を反転）
    /// </summary>
    private int[] GenerateBlockReverse(int size)
    {
        var array = Enumerable.Range(1, size).ToArray();
        var blockSize = Pow2LessThanOrEqual((int)Math.Sqrt(size));
        var adjustedSize = size - (size % blockSize);

        var i = 0;
        var j = adjustedSize - blockSize;

        while (i < j)
        {
            BlockSwap(array, i, j, blockSize);
            i += blockSize;
            j -= blockSize;
        }

        return array;

        static void BlockSwap(int[] arr, int a, int b, int length)
        {
            for (var k = 0; k < length; k++)
            {
                (arr[a + k], arr[b + k]) = (arr[b + k], arr[a + k]);
            }
        }

        static int Pow2LessThanOrEqual(int value)
        {
            var val = 1;
            while (val <= value) val <<= 1;
            return val >> 1;
        }
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
            {
                array[i] = sorted[right--];
            }
            else
            {
                array[i] = sorted[left++];
            }
        }

        return array;
    }

    /// <summary>
    /// 二分探索木レベル順走査（Level-Order Traversal / BFS）
    /// ソート済み配列からBSTを構築し、レベル順で再配置
    /// </summary>
    private int[] GenerateBstTraversal(int size, Random random)
    {
        var temp = Enumerable.Range(1, size).ToArray();
        var array = new int[size];
        
        // BFS (Level-Order Traversal) using queue
        var queue = new Queue<(int start, int end)>();
        queue.Enqueue((0, size));
        var i = 0;
        
        while (queue.Count > 0)
        {
            var (start, end) = queue.Dequeue();
            if (start != end)
            {
                var mid = (start + end) / 2;
                array[i++] = temp[mid];
                queue.Enqueue((start, mid));
                queue.Enqueue((mid + 1, end));
            }
        }
        
        return array;
    }

    /// <summary>
    /// 逆BST（レベル順 → 中順変換の逆操作）
    /// BSTのレベル順走査インデックスを生成し、それを使って配列を再配置
    /// </summary>
    private int[] GenerateInvertedBst(int size)
    {
        var array = Enumerable.Range(1, size).ToArray();
        var levelOrderIndices = new int[size];

        // Generate level-order traversal indices using queue
        var queue = new Queue<(int start, int end)>();
        queue.Enqueue((0, size));
        var i = 0;

        while (queue.Count > 0)
        {
            var (start, end) = queue.Dequeue();
            if (start != end)
            {
                var mid = (start + end) / 2;
                levelOrderIndices[i++] = mid;
                queue.Enqueue((start, mid));
                queue.Enqueue((mid + 1, end));
            }
        }

        // Rearrange array using level-order indices
        var temp = new int[size];
        Array.Copy(array, temp, size);

        for (i = 0; i < size; i++)
        {
            array[levelOrderIndices[i]] = temp[i];
        }

        return array;
    }

    /// <summary>
    /// 対数スロープ（2のべき乗ベースの配置）
    /// 各インデックスiに対して、log2(i)に基づいた位置から値を取得
    /// </summary>
    private int[] GenerateLogarithmicSlopes(int size)
    {
        var temp = Enumerable.Range(1, size).ToArray();
        var array = new int[size];

        array[0] = temp[0];

        for (var i = 1; i < size; i++)
        {
            // Calculate log base 2
            var log = (int)(Math.Log(i) / Math.Log(2));
            var power = (int)Math.Pow(2, log);

            // Get value from position based on formula: 2 * (i - power) + 1
            var sourceIndex = 2 * (i - power) + 1;
            array[i] = sourceIndex < size ? temp[sourceIndex] : temp[i];
        }

        return array;
    }

    /// <summary>
    /// 半分回転（前半と後半を入れ替え）
    /// 配列を中央で分割し、各要素を対応する位置と入れ替え
    /// </summary>
    private int[] GenerateHalfRotation(int size)
    {
        var array = Enumerable.Range(1, size).ToArray();
        var mid = (size + 1) / 2;

        if (size % 2 == 0)
        {
            // Even size: simple swap
            for (int a = 0, m = mid; m < size; a++, m++)
            {
                (array[a], array[m]) = (array[m], array[a]);
            }
        }
        else
        {
            // Odd size: cyclic rotation
            var temp = array[0];
            var a = 0;
            var m = mid;

            while (m < size)
            {
                array[a++] = array[m];
                array[m++] = array[a];
            }
            array[a] = temp;
        }

        return array;
    }

    /// <summary>
    /// ヒープ化済み（max-heap構造）
    /// </summary>
    private int[] GenerateHeapified(int size)
    {
        var array = Enumerable.Range(1, size).ToArray();

        // Build max-heap
        for (var i = size / 2 - 1; i >= 0; i--)
        {
            Heapify(array, size, i);
        }

        return array;

        static void Heapify(int[] arr, int n, int i)
        {
            var largest = i;
            var left = 2 * i + 1;
            var right = 2 * i + 2;

            if (left < n && arr[left] > arr[largest])
            {
                largest = left;
            }

            if (right < n && arr[right] > arr[largest])
            {
                largest = right;
            }

            if (largest != i)
            {
                (arr[i], arr[largest]) = (arr[largest], arr[i]);
                Heapify(arr, n, largest);
            }
        }
    }

    /// <summary>
    /// ポプラヒープ化済み（Poplar Heapソート用）
    /// 複数の完全二分木の森を形成
    /// </summary>
    private int[] GeneratePoplarHeapified(int size)
    {
        var array = Enumerable.Range(1, size).ToArray();

        // Poplar heap: forest of complete binary trees
        // Each tree has size 2^k - 1 (1, 3, 7, 15, 31, ...)
        var pos = 0;

        while (pos < size)
        {
            // Calculate largest tree size that fits: 2^k - 1
            var treeSize = 1;
            while ((treeSize * 2 + 1) <= size - pos)
            {
                treeSize = treeSize * 2 + 1;  // 1 → 3 → 7 → 15 → 31 ...
            }

            // Build max-heap for this tree
            var end = Math.Min(pos + treeSize, size);
            for (var i = (end - pos) / 2 - 1; i >= 0; i--)
            {
                PoplarHeapify(array, pos, end, pos + i);
            }

            pos = end;
        }

        return array;

        static void PoplarHeapify(int[] arr, int start, int end, int i)
        {
            var largest = i;
            var left = start + 2 * (i - start) + 1;
            var right = start + 2 * (i - start) + 2;

            if (left < end && arr[left] > arr[largest])
                largest = left;

            if (right < end && arr[right] > arr[largest])
                largest = right;

            if (largest != i)
            {
                (arr[i], arr[largest]) = (arr[largest], arr[i]);
                PoplarHeapify(arr, start, end, largest);
            }
        }
    }

    /// <summary>
    /// 三角ヒープ化済み（Triangular Heapソート用）
    /// 三角数ベースのヒープ構造（簡略版）
    /// </summary>
    private int[] GenerateTriangularHeapified(int size)
    {
        var array = Enumerable.Range(1, size).ToArray();
        
        // Shuffle first to create a non-sorted starting point
        var random = new Random(43); // Use different seed from Smooth
        for (var i = size - 1; i > 0; i--)
        {
            var j = random.Next(i + 1);
            (array[i], array[j]) = (array[j], array[i]);
        }
        
        // Triangular heap: each row has k elements (1, 2, 3, 4, ...)
        // Triangular numbers: 1, 3, 6, 10, 15, 21, 28, 36, 45, 55...
        // T(n) = n(n+1)/2
        
        // Build triangular heap structure
        // We'll create a simpler version: divide into triangular sections and heapify each
        var triangularSizes = new List<int>();
        var sum = 0;
        for (var i = 1; sum < size; i++)
        {
            var triangularSize = i; // Row size: 1, 2, 3, 4...
            if (sum + triangularSize > size)
                triangularSize = size - sum;
            
            triangularSizes.Add(triangularSize);
            sum += triangularSize;
            
            if (sum >= size) break;
        }
        
        // Heapify each triangular section
        var pos = 0;
        foreach (var sectionSize in triangularSizes)
        {
            if (pos >= size) break;
            
            var end = Math.Min(pos + sectionSize, size);
            
            // Build max-heap for this section
            for (var i = (end - pos) / 2 - 1; i >= 0; i--)
            {
                TriangularHeapify(array, pos, end, pos + i);
            }
            
            pos = end;
        }
        
        return array;
        
        static void TriangularHeapify(int[] arr, int start, int end, int i)
        {
            var largest = i;
            var left = start + 2 * (i - start) + 1;
            var right = start + 2 * (i - start) + 2;
            
            if (left < end && arr[left] > arr[largest])
                largest = left;
            
            if (right < end && arr[right] > arr[largest])
                largest = right;
            
            if (largest != i)
            {
                (arr[i], arr[largest]) = (arr[largest], arr[i]);
                TriangularHeapify(arr, start, end, largest);
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
            {
                counts[0]++;
            }
        }
        counts[2] = size - counts[0];
        var remaining = Math.Min(size, 8) - counts[0];
        counts[2] = remaining;
        counts[1] = size - counts[0] - counts[2];

        var result = new List<int>();
        for (var i = 0; i < 3; i++)
        {
            result.AddRange(Enumerable.Repeat(values[i], counts[i]));
        }

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

    // Additional Mathematical Distributions

    /// <summary>
    /// 二次曲線分布（x²）
    /// </summary>
    private int[] GenerateQuadraticDistribution(int size)
    {
        var array = new int[size];
        var n = size - 1;
        
        for (var i = 0; i < size; i++)
        {
            var x = (double)i / n;
            array[i] = (int)(n * x * x) + 1;
        }
        
        // Shuffle to randomize order while keeping value distribution
        return ShuffleArray(array);
    }

    /// <summary>
    /// 平方根曲線分布（√x）
    /// </summary>
    private int[] GenerateSquareRootDistribution(int size)
    {
        var array = new int[size];
        var n = size - 1;
        
        for (var i = 0; i < size; i++)
        {
            var x = (double)i / n;
            array[i] = (int)(n * Math.Sqrt(x)) + 1;
        }
        
        return ShuffleArray(array);
    }

    /// <summary>
    /// 三次曲線分布（x³ 中心）
    /// </summary>
    private int[] GenerateCubicDistribution(int size)
    {
        var array = new int[size];
        var h = size / 2.0;
        
        for (var i = 0; i < size; i++)
        {
            var val = i / h - 1;
            var cubic = val * val * val;
            array[i] = (int)(h * (cubic + 1));
        }
        
        return ShuffleArray(array);
    }

    /// <summary>
    /// 五次曲線分布（x⁵ 中心）
    /// </summary>
    private int[] GenerateQuinticDistribution(int size)
    {
        var array = new int[size];
        var h = size / 2.0;
        
        for (var i = 0; i < size; i++)
        {
            var val = i / h - 1;
            var quintic = Math.Pow(val, 5);
            array[i] = (int)(h * (quintic + 1));
        }
        
        return ShuffleArray(array);
    }

    /// <summary>
    /// 立方根曲線分布（∛x）
    /// </summary>
    private int[] GenerateCubeRootDistribution(int size)
    {
        var array = new int[size];
        var h = size / 2.0;
        
        for (var i = 0; i < size; i++)
        {
            var val = i / h - 1;
            var root = val < 0 ? -Math.Pow(-val, 1.0 / 3.0) : Math.Pow(val, 1.0 / 3.0);
            array[i] = (int)(h * (root + 1));
        }
        
        return ShuffleArray(array);
    }

    /// <summary>
    /// 五乗根曲線分布（⁵√x）
    /// </summary>
    private int[] GenerateFifthRootDistribution(int size)
    {
        var array = new int[size];
        var h = size / 2.0;
        
        for (var i = 0; i < size; i++)
        {
            var val = i / h - 1;
            var root = val < 0 ? -Math.Pow(-val, 1.0 / 5.0) : Math.Pow(val, 1.0 / 5.0);
            array[i] = (int)(h * (root + 1));
        }
        
        return ShuffleArray(array);
    }

    /// <summary>
    /// Fisher-Yatesシャッフル（配列をランダム化）
    /// </summary>
    private int[] ShuffleArray(int[] array)
    {
        var random = new Random();
        for (var i = array.Length - 1; i > 0; i--)
        {
            var j = random.Next(i + 1);
            (array[i], array[j]) = (array[j], array[i]);
        }
        return array;
    }

    /// <summary>
    /// ルーラー関数分布
    /// </summary>
    private int[] GenerateRulerDistribution(int size)
    {
        var array = new int[size];
        var step = Math.Max(1, size / 256);
        var floorLog2 = (int)(Math.Log(size / (double)step) / Math.Log(2));
        var lowest = step;
        while (2 * lowest <= size / 4)
        {
            lowest *= 2;
        }

        var digits = new bool[floorLog2 + 2];
        int i, j;

        for (i = 0; i + step <= size; i += step)
        {
            for (j = 0; j < digits.Length && digits[j]; j++)
            { }

            digits[j] = true;

            for (var k = 0; k < step; k++)
            {
                var value = size / 2 - Math.Min((1 << j) * step, lowest);
                array[i + k] = value;
            }

            for (var k = 0; k < j; k++)
            {
                digits[k] = false;
            }
        }

        for (j = 0; j < digits.Length && digits[j]; j++)
        { }

        digits[j] = true;

        while (i < size)
        {
            var value = Math.Max(size / 2 - (1 << j) * step, size / 4);
            array[i++] = value;
        }

        return array;
    }

    /// <summary>
    /// ブランマンジェ曲線分布
    /// </summary>
    private int[] GenerateBlancmangeDistribution(int size)
    {
        var array = new int[size];
        var floorLog2 = (int)(Math.Log(size) / Math.Log(2));

        for (var i = 0; i < size; i++)
        {
            var value = (int)(size * CurveSum(floorLog2, (double)i / size));
            array[i] = value;
        }

        return array;

        static double CurveSum(int n, double x)
        {
            var sum = 0.0;
            while (n >= 0)
            {
                sum += Curve(n--, x);
            }
            return sum;
        }

        static double Curve(int n, double x)
        {
            return TriangleWave((1 << n) * x) / (1 << n);
        }

        static double TriangleWave(double x)
        {
            return Math.Abs(x - (int)(x + 0.5));
        }
    }

    /// <summary>
    /// カントール関数分布
    /// </summary>
    private int[] GenerateCantorDistribution(int size)
    {
        var array = new int[size];
        CantorRecursive(array, 0, size, 0, size - 1);
        return array;

        static void CantorRecursive(int[] arr, int a, int b, int min, int max)
        {
            if (b - a < 1 || max == min) return;

            var mid = (min + max) / 2;
            if (b - a == 1)
            {
                arr[a] = mid;
                return;
            }

            var t1 = (a + a + b) / 3;
            var t2 = (a + b + b + 2) / 3;

            for (var i = t1; i < t2; i++)
            {
                arr[i] = mid;
            }

            CantorRecursive(arr, a, t1, min, mid);
            CantorRecursive(arr, t2, b, mid + 1, max);
        }
    }

    /// <summary>
    /// 約数の和関数分布
    /// </summary>
    private int[] GenerateDivisorsDistribution(int size)
    {
        var n = new int[size];
        n[0] = 0;
        if (size > 1)
        {
            n[1] = 1;
        }

        var max = 1.0;
        for (var i = 2; i < size; i++)
        {
            n[i] = SumDivisors(i);
            if (n[i] > max)
            {
                max = n[i];
            }
        }

        var scale = Math.Min((size - 1) / max, 1);
        var array = new int[size];
        for (var i = 0; i < size; i++)
        {
            array[i] = (int)(n[i] * scale);
        }

        return array;

        static int SumDivisors(int num)
        {
            var sum = num + 1;
            for (var i = 2; i <= (int)Math.Sqrt(num); i++)
            {
                if (num % i == 0)
                {
                    if (i == num / i)
                    {
                        sum += i;
                    }
                    else
                    {
                        sum += i + num / i;
                    }
                }
            }
            return sum;
        }
    }

    /// <summary>
    /// FSD分布（Fly Straight Dangit - OEIS A133058）
    /// </summary>
    private int[] GenerateFsdDistribution(int size)
    {
        var fsd = new int[size];
        fsd[0] = 1;
        if (size > 1)
        {
            fsd[1] = 1;
        }

        var max = 1.0;
        for (var i = 2; i < size; i++)
        {
            var g = Gcd(fsd[i - 1], i);
            fsd[i] = fsd[i - 1] / g + (g == 1 ? i + 1 : 0);
            if (fsd[i] > max)
            {
                max = fsd[i];
            }
        }

        var scale = Math.Min((size - 1) / max, 1);
        var array = new int[size];
        for (var i = 0; i < size; i++)
        {
            array[i] = (int)(fsd[i] * scale);
        }

        return array;

        static int Gcd(int a, int b)
        {
            if (b == 0)
            {
                return a;
            }
            return Gcd(b, a % b);
        }
    }

    /// <summary>
    /// 逆対数分布（減少ランダム）
    /// </summary>
    private int[] GenerateReverseLogDistribution(int size, Random random)
    {
        var array = new int[size];

        for (var i = 0; i < size; i++)
        {
            var r = random.Next(size - i) + i;
            array[i] = r + 1;
        }

        return array;
    }

    /// <summary>
    /// モジュロ関数分布
    /// </summary>
    private int[] GenerateModuloDistribution(int size)
    {
        var array = new int[size];

        for (var i = 0; i < size; i++)
        {
            array[i] = 2 * (size % (i + 1));
        }

        return array;
    }

    /// <summary>
    /// オイラーのトーシェント関数分布
    /// </summary>
    private int[] GenerateTotientDistribution(int size)
    {
        var array = new int[size];
        var minPrimeFactors = new int[size];
        var primes = new List<int>();

        array[0] = 0;
        if (size > 1)
        {
            array[1] = 1;
        }

        for (var i = 2; i < size; i++)
        {
            if (minPrimeFactors[i] == 0)
            {
                primes.Add(i);
                minPrimeFactors[i] = i;
                array[i] = i - 1;
            }

            foreach (var prime in primes)
            {
                if (i * prime >= size) break;

                var last = prime == minPrimeFactors[i];

                minPrimeFactors[i * prime] = prime;
                array[i * prime] = array[i] * (last ? prime : prime - 1);

                if (last) break;
            }
        }

        return array;
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
        while (n < size)
        {
            n *= 2;
        }

        CircleSortRoutine(array, 0, n - 1, size);

        return array;

        static void CircleSortRoutine(int[] arr, int lo, int hi, int end)
        {
            if (lo == hi)
                return;

            var low = lo;
            var high = hi;
            var mid = (hi - lo) / 2;

            while (lo < hi)
            {
                if (hi < end && arr[lo] > arr[hi])
                {
                    (arr[lo], arr[hi]) = (arr[hi], arr[lo]);
                }
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
            if (b - a < 2)
                return;
            if (b - a == 2)
            {
                arr[a + 1]++;
                return;
            }

            var h = (b - a) / 3;
            var t1 = (a + a + b) / 3;
            var t2 = (a + b + b + 2) / 3;

            for (var i = a; i < t1; i++)
            {
                arr[i] += h;
            }
            for (var i = t1; i < t2; i++)
            {
                arr[i] += 2 * h;
            }

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
        {
            counts[triangle[i]]++;
        }

        for (var i = 1; i < counts.Length; i++)
        {
            counts[i] += counts[i - 1];
        }

        for (var i = size - 1; i >= 0; i--)
        {
            triangle[i] = --counts[triangle[i]];
        }

        var sorted = Enumerable.Range(1, size).ToArray();
        var result = new int[size];

        for (var i = 0; i < size; i++)
        {
            result[i] = sorted[triangle[i]];
        }

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
        {
            (array[i], array[j]) = (array[j], array[i]);
        }

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
