using SortLab.Core.Algorithms;
using SortLab.Core.Contexts;

namespace SortLab.Tests;

public class BubbleSortTests
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
        BubbleSort.Sort(array.AsSpan());
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
        BubbleSort.Sort(array.AsSpan(), stats);

        Assert.Equal((ulong)inputSample.Samples.Length, (ulong)array.Length);
        Assert.NotEqual(0UL, stats.IndexReadCount);
        Assert.NotEqual(0UL, stats.IndexWriteCount);
        Assert.NotEqual(0UL, stats.CompareCount);
        Assert.NotEqual(0UL, stats.SwapCount);
    }

    [Theory]
    [ClassData(typeof(MockSortedData))]
    public void StatisticsSortedTest(IInputSample<int> inputSample)
    {
        var stats = new StatisticsContext();
        var array = inputSample.Samples.ToArray();
        BubbleSort.Sort(array.AsSpan(), stats);

        Assert.Equal((ulong)inputSample.Samples.Length, (ulong)array.Length);
        Assert.NotEqual(0UL, stats.IndexReadCount);
        Assert.Equal(0UL, stats.IndexWriteCount);
        Assert.NotEqual(0UL, stats.CompareCount);
        Assert.Equal(0UL, stats.SwapCount);
    }

    [Theory]
    [ClassData(typeof(MockRandomData))]
    public void StatisticsResetTest(IInputSample<int> inputSample)
    {
        var stats = new StatisticsContext();
        var array = inputSample.Samples.ToArray();
        BubbleSort.Sort(array.AsSpan(), stats);

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
        BubbleSort.Sort(sorted.AsSpan(), stats);

        // Bubble Sort always performs n(n-1)/2 comparisons regardless of input order
        // For sorted data, no swaps are needed since all elements are already in order
        var expectedCompares = (ulong)(n * (n - 1) / 2);
        var expectedSwaps = 0UL;
        var expectedWrites = 0UL; // No swaps = no writes

        // Each comparison reads 2 elements (positions j and j-1)
        // Total reads = 2 * number of comparisons
        var expectedReads = expectedCompares * 2;

        Assert.Equal(expectedCompares, stats.CompareCount);
        Assert.Equal(expectedSwaps, stats.SwapCount);
        Assert.Equal(expectedWrites, stats.IndexWriteCount);
        Assert.Equal(expectedReads, stats.IndexReadCount);
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
        BubbleSort.Sort(reversed.AsSpan(), stats);

        // Bubble Sort worst case: reversed array
        // Comparisons: n(n-1)/2 (all adjacent pairs are compared)
        // Swaps: n(n-1)/2 (every comparison results in a swap)
        var expectedCompares = (ulong)(n * (n - 1) / 2);
        var expectedSwaps = (ulong)(n * (n - 1) / 2);

        // Each swap writes 2 elements (swap reads and writes both positions)
        var expectedWrites = expectedSwaps * 2;

        // Each comparison reads 2 elements + each swap reads 2 elements
        // Total reads = 2 * comparisons + 2 * swaps = 4 * n(n-1)/2
        var expectedReads = expectedCompares * 2 + expectedSwaps * 2;

        Assert.Equal(expectedCompares, stats.CompareCount);
        Assert.Equal(expectedSwaps, stats.SwapCount);
        Assert.Equal(expectedWrites, stats.IndexWriteCount);
        Assert.Equal(expectedReads, stats.IndexReadCount);
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
        BubbleSort.Sort(random.AsSpan(), stats);

        // Bubble Sort always performs n(n-1)/2 comparisons regardless of input
        // For random data, swap count varies based on the number of inversions
        // - Best case: 0 swaps (already sorted by chance)
        // - Average case: n(n-1)/4 swaps (approximately half of comparisons result in swaps)
        // - Worst case: n(n-1)/2 swaps (reversed)
        var expectedCompares = (ulong)(n * (n - 1) / 2);
        var minSwaps = 0UL;
        var maxSwaps = (ulong)(n * (n - 1) / 2);

        // Each comparison reads 2 elements
        var minReads = expectedCompares * 2;
        // Maximum reads occur when all comparisons result in swaps
        var maxReads = expectedCompares * 2 + maxSwaps * 2;

        Assert.Equal(expectedCompares, stats.CompareCount);
        Assert.InRange(stats.SwapCount, minSwaps, maxSwaps);
        Assert.InRange(stats.IndexReadCount, minReads, maxReads);

        // Writes = 2 * swaps (each swap writes 2 elements)
        var expectedMinWrites = minSwaps * 2;
        var expectedMaxWrites = maxSwaps * 2;
        Assert.InRange(stats.IndexWriteCount, expectedMinWrites, expectedMaxWrites);
    }
}


