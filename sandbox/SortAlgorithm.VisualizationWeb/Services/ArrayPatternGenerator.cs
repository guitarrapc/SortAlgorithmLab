using SortAlgorithm.Utils;
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
            ArrayPattern.Random => ArrayPatterns.GenerateRandom(size, random),
            ArrayPattern.Sorted => ArrayPatterns.GenerateSorted(size),
            ArrayPattern.Reversed => ArrayPatterns.GenerateReversed(size),

            // Nearly Sorted
            ArrayPattern.SingleElementMoved => ArrayPatterns.GenerateSingleElementMoved(size, random),
            ArrayPattern.AlmostSorted => ArrayPatterns.GenerateAlmostSorted(size, random),
            ArrayPattern.NearlySorted => ArrayPatterns.GenerateNearlySorted(size, random),
            ArrayPattern.ScrambledTail => ArrayPatterns.GenerateScrambledTail(size, random),
            ArrayPattern.ScrambledHead => ArrayPatterns.GenerateScrambledHead(size, random),
            ArrayPattern.Noisy => ArrayPatterns.GenerateNoisy(size, random),
            ArrayPattern.ShuffledOdds => ArrayPatterns.GenerateShuffledOdds(size, random),
            ArrayPattern.ShuffledHalf => ArrayPatterns.GenerateShuffledHalf(size, random),
            ArrayPattern.EvensReversedOddsInOrder => ArrayPatterns.GenerateEvensReversedOddsInOrder(size),
            ArrayPattern.EvensInOrderScrambledOdds => ArrayPatterns.GenerateEvensInOrderScrambledOdds(size, random),
            ArrayPattern.DoubleLayered => ArrayPatterns.GenerateDoubleLayered(size),

            // Merge Patterns
            ArrayPattern.FinalMerge => ArrayPatterns.GenerateFinalMerge(size),
            ArrayPattern.ShuffledFinalMerge => ArrayPatterns.GenerateShuffledFinalMerge(size, random),
            ArrayPattern.Sawtooth => ArrayPatterns.GenerateSawtooth(size),

            // Partitioned
            ArrayPattern.Partitioned => ArrayPatterns.GeneratePartitioned(size, random),
            ArrayPattern.HalfSorted => ArrayPatterns.GenerateHalfSorted(size, random),
            ArrayPattern.HalfReversed => ArrayPatterns.GenerateHalfReversed(size),

            // Shape
            ArrayPattern.PipeOrgan => ArrayPatterns.GeneratePipeOrgan(size),
            ArrayPattern.MountainShape => ArrayPatterns.GenerateMountainShape(size),
            ArrayPattern.ValleyShape => ArrayPatterns.GenerateValleyShape(size),

            // Radix/Interleaved
            ArrayPattern.FinalRadix => ArrayPatterns.GenerateFinalRadix(size),
            ArrayPattern.RealFinalRadix => ArrayPatterns.GenerateRealFinalRadix(size),
            ArrayPattern.RecursiveFinalRadix => ArrayPatterns.GenerateRecursiveFinalRadix(size),
            ArrayPattern.FinalBitonicPass => ArrayPatterns.GenerateFinalBitonicPass(size),
            ArrayPattern.BitReversal => ArrayPatterns.GenerateBitReversal(size),
            ArrayPattern.BlockRandomly => ArrayPatterns.GenerateBlockRandomly(size, random),
            ArrayPattern.BlockReverse => ArrayPatterns.GenerateBlockReverse(size),
            ArrayPattern.Interlaced => ArrayPatterns.GenerateInterlaced(size),
            ArrayPattern.Zigzag => ArrayPatterns.GenerateZigzag(size),

            // Tree/Heap
            ArrayPattern.BstTraversal => ArrayPatterns.GenerateBstTraversal(size, random),
            ArrayPattern.InvertedBst => ArrayPatterns.GenerateInvertedBst(size),
            ArrayPattern.LogarithmicSlopes => ArrayPatterns.GenerateLogarithmicSlopes(size),
            ArrayPattern.HalfRotation => ArrayPatterns.GenerateHalfRotation(size),
            ArrayPattern.Heapified => ArrayPatterns.GenerateHeapified(size),
            ArrayPattern.PoplarHeapified => ArrayPatterns.GeneratePoplarHeapified(size),
            ArrayPattern.TriangularHeapified => ArrayPatterns.GenerateTriangularHeapified(size),

            // Duplicates
            ArrayPattern.FewUnique => ArrayPatterns.GenerateFewUnique(size, random),
            ArrayPattern.ManyDuplicates => ArrayPatterns.GenerateManyDuplicates(size, random),
            ArrayPattern.AllEqual => ArrayPatterns.GenerateAllEqual(size),

            // Distributions
            ArrayPattern.QuadraticDistribution => ArrayPatterns.GenerateQuadraticDistribution(size),
            ArrayPattern.SquareRootDistribution => ArrayPatterns.GenerateSquareRootDistribution(size),
            ArrayPattern.CubicDistribution => ArrayPatterns.GenerateCubicDistribution(size),
            ArrayPattern.QuinticDistribution => ArrayPatterns.GenerateQuinticDistribution(size),
            ArrayPattern.CubeRootDistribution => ArrayPatterns.GenerateCubeRootDistribution(size),
            ArrayPattern.FifthRootDistribution => ArrayPatterns.GenerateFifthRootDistribution(size),
            ArrayPattern.SineWave => ArrayPatterns.GenerateSineWave(size),
            ArrayPattern.CosineWave => ArrayPatterns.GenerateCosineWave(size),
            ArrayPattern.BellCurve => ArrayPatterns.GenerateBellCurve(size),
            ArrayPattern.PerlinNoiseCurve => ArrayPatterns.GeneratePerlinNoiseCurve(size, random),
            ArrayPattern.RulerDistribution => ArrayPatterns.GenerateRulerDistribution(size),
            ArrayPattern.BlancmangeDistribution => ArrayPatterns.GenerateBlancmangeDistribution(size),
            ArrayPattern.CantorDistribution => ArrayPatterns.GenerateCantorDistribution(size),
            ArrayPattern.DivisorsDistribution => ArrayPatterns.GenerateDivisorsDistribution(size),
            ArrayPattern.FsdDistribution => ArrayPatterns.GenerateFsdDistribution(size),
            ArrayPattern.ReverseLogDistribution => ArrayPatterns.GenerateReverseLogDistribution(size, random),
            ArrayPattern.ModuloDistribution => ArrayPatterns.GenerateModuloDistribution(size),
            ArrayPattern.TotientDistribution => ArrayPatterns.GenerateTotientDistribution(size),

            // Advanced/Fractal
            ArrayPattern.CirclePass => ArrayPatterns.GenerateCirclePass(size, random),
            ArrayPattern.PairwisePass => ArrayPatterns.GeneratePairwisePass(size, random),
            ArrayPattern.RecursiveReversal => ArrayPatterns.GenerateRecursiveReversal(size),
            ArrayPattern.GrayCodeFractal => ArrayPatterns.GenerateGrayCodeFractal(size),
            ArrayPattern.SierpinskiTriangle => ArrayPatterns.GenerateSierpinskiTriangle(size),
            ArrayPattern.Triangular => ArrayPatterns.GenerateTriangular(size),

            // Adversarial
            ArrayPattern.QuickSortAdversary => ArrayPatterns.GenerateQuickSortAdversary(size),
            ArrayPattern.PdqSortAdversary => ArrayPatterns.GeneratePdqSortAdversary(size),
            ArrayPattern.GrailSortAdversary => ArrayPatterns.GenerateGrailSortAdversary(size, random),
            ArrayPattern.ShuffleMergeAdversary => ArrayPatterns.GenerateShuffleMergeAdversary(size),

            _ => throw new NotImplementedException($"{nameof(pattern)} not implemented")
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
            ArrayPattern.EvensReversedOddsInOrder => "🎲 Evens Reversed, Odds In-Order",
            ArrayPattern.EvensInOrderScrambledOdds => "🎲 Evens In-Order, Scrambled Odds",
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
}
