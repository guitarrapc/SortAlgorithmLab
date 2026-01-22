using SortLab.Core.Algorithms;
using SortLab.Core.Contexts;

namespace SortLab.Tests;

public class InsertionNonOptimizedSortTests
{
    [Theory]
    [ClassData(typeof(MockRandomData))]
    [ClassData(typeof(MockNegativePositiveRandomData))]
    [ClassData(typeof(MockNegativeRandomData))]
    [ClassData(typeof(MockReversedData))]
    [ClassData(typeof(MockMountainData))]
    [ClassData(typeof(MockNearlySortedData))]
    [ClassData(typeof(MockSortedData))]
    [ClassData(typeof(MockSameValuesData))]
    public void SortResultOrderTest(IInputSample<int> inputSample)
    {
        var array = inputSample.Samples.ToArray();
        InsertionNonOptimizedSort.Sort(array.AsSpan());
        Assert.Equal(inputSample.Samples.OrderBy(x => x), array);
    }

    [Theory]
    [ClassData(typeof(MockRandomData))]
    [ClassData(typeof(MockNegativePositiveRandomData))]
    [ClassData(typeof(MockNegativeRandomData))]
    [ClassData(typeof(MockReversedData))]
    [ClassData(typeof(MockMountainData))]
    [ClassData(typeof(MockNearlySortedData))]
    [ClassData(typeof(MockSameValuesData))]
    public void StatisticsTest(IInputSample<int> inputSample)
    {
        var stats = new StatisticsContext();
        var array = inputSample.Samples.ToArray();
        InsertionNonOptimizedSort.Sort(array.AsSpan(), stats);

        Assert.Equal((ulong)inputSample.Samples.Length, (ulong)array.Length);
        Assert.NotEqual(0UL, stats.IndexReadCount);
        Assert.NotEqual(0UL, stats.IndexWriteCount);
        Assert.NotEqual(0UL, stats.CompareCount);
        Assert.NotEqual(0UL, stats.SwapCount); // Non-optimized version uses swaps
    }

    [Theory]
    [ClassData(typeof(MockSortedData))]
    public void StatisticsSortedTest(IInputSample<int> inputSample)
    {
        var stats = new StatisticsContext();
        var array = inputSample.Samples.ToArray();
        InsertionNonOptimizedSort.Sort(array.AsSpan(), stats);

        Assert.Equal((ulong)inputSample.Samples.Length, (ulong)array.Length);
        Assert.NotEqual(0UL, stats.IndexReadCount);
        Assert.Equal(0UL, stats.IndexWriteCount); // Already sorted, no writes needed
        Assert.Equal((ulong)(inputSample.Samples.Length - 1), stats.CompareCount);
        Assert.Equal(0UL, stats.SwapCount);
    }

    [Theory]
    [ClassData(typeof(MockRandomData))]
    public void StatisticsResetTest(IInputSample<int> inputSample)
    {
        var stats = new StatisticsContext();
        var array = inputSample.Samples.ToArray();
        InsertionNonOptimizedSort.Sort(array.AsSpan(), stats);

        stats.Reset();
        Assert.Equal(0UL, stats.IndexReadCount);
        Assert.Equal(0UL, stats.IndexWriteCount);
        Assert.Equal(0UL, stats.CompareCount);
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
        InsertionNonOptimizedSort.Sort(sorted.AsSpan(), stats);

        // Insertion Sort (Non-Optimized) on sorted data: best case O(n)
        // - For each position i (from 1 to n-1), we compare once with the previous element
        // - Since the current element is >= the previous element, no swapping occurs
        // - Total comparisons: n-1
        // - Total swaps: 0 (already sorted)
        // - Total writes: 0 (no swaps = no writes)
        var expectedCompares = (ulong)(n - 1);
        var expectedSwaps = 0UL;
        var expectedWrites = 0UL;

        // Each comparison reads 2 elements
        var minIndexReads = expectedCompares * 2;

        Assert.Equal(expectedCompares, stats.CompareCount);
        Assert.Equal(expectedSwaps, stats.SwapCount);
        Assert.Equal(expectedWrites, stats.IndexWriteCount);
        Assert.True(stats.IndexReadCount >= minIndexReads,
            $"IndexReadCount ({stats.IndexReadCount}) should be >= {minIndexReads}");
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
        InsertionNonOptimizedSort.Sort(reversed.AsSpan(), stats);

        // Insertion Sort (Non-Optimized) on reversed data: worst case O(n^2)
        // - Position 1: 1 comparison, 1 swap
        // - Position 2: 2 comparisons, 2 swaps
        // - ...
        // - Position n-1: (n-1) comparisons, (n-1) swaps
        // - Total comparisons: 1 + 2 + ... + (n-1) = n(n-1)/2
        // - Total swaps: same as comparisons = n(n-1)/2
        // - Each swap writes 2 elements, so total writes = 2 * swaps = n(n-1)
        var expectedCompares = (ulong)(n * (n - 1) / 2);
        var expectedSwaps = (ulong)(n * (n - 1) / 2);
        var expectedWrites = (ulong)(n * (n - 1)); // 2 writes per swap

        // Each comparison reads 2 elements, each swap reads 2 elements
        // Total reads = 2 * compares + 2 * swaps = 2 * (compares + swaps)
        var minIndexReads = 2 * (expectedCompares + expectedSwaps);

        Assert.Equal(expectedCompares, stats.CompareCount);
        Assert.Equal(expectedSwaps, stats.SwapCount);
        Assert.Equal(expectedWrites, stats.IndexWriteCount);
        Assert.True(stats.IndexReadCount >= minIndexReads,
            $"IndexReadCount ({stats.IndexReadCount}) should be >= {minIndexReads}");
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
        InsertionNonOptimizedSort.Sort(random.AsSpan(), stats);

        // Insertion Sort (Non-Optimized) on random data: average case O(n^2)
        // - Average comparisons: approximately n(n-1)/4
        // - Average swaps: approximately n(n-1)/4
        // - For random data, on average, each element moves halfway through the sorted portion
        var minCompares = (ulong)(n - 1); // Best case (already sorted by chance)
        var maxCompares = (ulong)(n * (n - 1) / 2); // Worst case (reverse sorted by chance)
        
        var minSwaps = 0UL; // Best case
        var maxSwaps = (ulong)(n * (n - 1) / 2); // Worst case

        // Each comparison reads 2 elements
        var minIndexReads = minCompares * 2;

        Assert.InRange(stats.CompareCount, minCompares, maxCompares);
        Assert.InRange(stats.SwapCount, minSwaps, maxSwaps);
        Assert.True(stats.IndexReadCount >= minIndexReads,
            $"IndexReadCount ({stats.IndexReadCount}) should be >= {minIndexReads}");
        
        // Each swap writes 2 elements
        Assert.Equal(stats.SwapCount * 2, stats.IndexWriteCount);
    }
}

