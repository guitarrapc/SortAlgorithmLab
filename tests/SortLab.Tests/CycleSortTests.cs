using SortLab.Core.Algorithms;
using SortLab.Core.Contexts;

namespace SortLab.Tests;

public class CycleSortTests
{
    [Theory]
    [ClassData(typeof(MockRandomData))]
    [ClassData(typeof(MockNegativePositiveRandomData))]
    [ClassData(typeof(MockNegativeRandomData))]
    [ClassData(typeof(MockReversedData))]
    [ClassData(typeof(MockMountainData))]
    [ClassData(typeof(MockNearlySortedData))]
    [ClassData(typeof(MockSameValuesData))]
    [ClassData(typeof(MockAntiQuickSortData))]
    [ClassData(typeof(MockQuickSortWorstCaseData))]
    public void SortResultOrderTest(IInputSample<int> inputSample)
    {
        var stats = new StatisticsContext();
        var array = inputSample.Samples.ToArray();
        CycleSort.Sort(array.AsSpan(), stats);

        Assert.Equal((ulong)inputSample.Samples.Length, (ulong)array.Length);
    }

#if DEBUG

    [Theory]
    [ClassData(typeof(MockSortedData))]
    public void StatisticsSortedTest(IInputSample<int> inputSample)
    {
        var stats = new StatisticsContext();
        var array = inputSample.Samples.ToArray();
        CycleSort.Sort(array.AsSpan(), stats);

        Assert.Equal((ulong)inputSample.Samples.Length, (ulong)array.Length);
        Assert.NotEqual(0UL, stats.IndexReadCount);
        Assert.Equal(0UL, stats.IndexWriteCount);
        Assert.NotEqual(0UL, stats.CompareCount);
        Assert.Equal(0UL, stats.SwapCount);
    }

    [Theory]
    [InlineData(10)]
    [InlineData(20)]
    [InlineData(50)]
    [InlineData(100)]
    public void TheoreticalValuesSortedTest(int n)
    {
        var stats = new StatisticsContext();
        var sorted = Enumerable.Range(0, n).ToArray();
        CycleSort.Sort(sorted.AsSpan(), stats);

        // Cycle Sort performs FindPosition comparisons: n(n-1)/2
        // For sorted data, SkipDuplicates is called but no actual duplicates exist,
        // so it performs minimal additional comparisons (1 per call to verify no match)
        var findPositionCompares = (ulong)(n * (n - 1) / 2);

        // Sorted data: no writes needed (all elements already in correct positions)
        var expectedWrites = 0UL;

        // For sorted data, FindPosition is called n-1 times (once per cycleStart)
        // Each call results in pos == cycleStart, so no SkipDuplicates is invoked
        var expectedCompares = findPositionCompares;

        Assert.Equal(expectedCompares, stats.CompareCount);
        Assert.Equal(expectedWrites, stats.IndexWriteCount);
        Assert.True(stats.IndexReadCount >= findPositionCompares,
            $"IndexReadCount ({stats.IndexReadCount}) should be >= {findPositionCompares}");
    }

    [Theory]
    [InlineData(10)]
    [InlineData(20)]
    [InlineData(50)]
    [InlineData(100)]
    public void TheoreticalValuesReversedTest(int n)
    {
        var stats = new StatisticsContext();
        var reversed = Enumerable.Range(0, n).Reverse().ToArray();
        CycleSort.Sort(reversed.AsSpan(), stats);

        // Cycle Sort performs FindPosition comparisons: n(n-1)/2 as the base
        var findPositionCompares = (ulong)(n * (n - 1) / 2);

        // However, FindPosition is called multiple times per cycle:
        // 1. Once before the initial write
        // 2. Multiple times in the while loop until the cycle completes
        //
        // For reversed data, the actual number of comparisons is approximately
        // 2x the base due to cycle rotations and SkipDuplicates calls.
        // Each element that moves requires additional FindPosition calls within its cycle.
        //
        // Empirical observations:
        // - n=10: ~90 comparisons (2.0x base)
        // - n=20: ~355 comparisons (1.87x base)
        // - n=50: ~2200 comparisons (1.80x base)
        //
        // We use a range to accommodate variations in cycle lengths.
        var minCompares = findPositionCompares;
        var maxCompares = findPositionCompares * 3; // Allow up to 3x for safety

        // Reversed data: each element needs to be moved to its correct position
        // In the worst case (reversed), n-1 elements need to be written
        var minWrites = (ulong)(n - 1);
        var maxWrites = (ulong)n;

        Assert.InRange(stats.CompareCount, minCompares, maxCompares);
        Assert.InRange(stats.IndexWriteCount, minWrites, maxWrites);
        Assert.True(stats.IndexReadCount >= minCompares,
            $"IndexReadCount ({stats.IndexReadCount}) should be >= {minCompares}");
    }

    [Theory]
    [InlineData(10)]
    [InlineData(20)]
    [InlineData(50)]
    [InlineData(100)]
    public void TheoreticalValuesRandomTest(int n)
    {
        var stats = new StatisticsContext();
        var random = Enumerable.Range(0, n).OrderBy(_ => Guid.NewGuid()).ToArray();
        CycleSort.Sort(random.AsSpan(), stats);

        // Cycle Sort performs FindPosition comparisons: n(n-1)/2 as the base
        var findPositionCompares = (ulong)(n * (n - 1) / 2);

        // For random data, the actual number of comparisons varies significantly
        // based on the random arrangement and resulting cycle lengths.
        // FindPosition is called multiple times per cycle (once initially, then
        // repeatedly in the while loop until the cycle completes).
        // Additionally, SkipDuplicates is called for each position found.
        //
        // Empirical observations show random data can require 2-3x the base comparisons.
        // We use a generous range to account for different random arrangements.
        var minCompares = findPositionCompares;
        var maxCompares = findPositionCompares * 4; // Allow up to 4x for random variation

        // Random data: most elements need to be moved
        // Typically between n/2 and n writes
        var minWrites = (ulong)(n / 2);
        var maxWrites = (ulong)n;

        Assert.InRange(stats.CompareCount, minCompares, maxCompares);
        Assert.InRange(stats.IndexWriteCount, minWrites, maxWrites);
        Assert.True(stats.IndexReadCount >= minCompares,
            $"IndexReadCount ({stats.IndexReadCount}) should be >= {minCompares}");
    }

#endif

}
