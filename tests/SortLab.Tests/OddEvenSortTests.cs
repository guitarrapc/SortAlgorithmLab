using SortLab.Core.Algorithms;
using SortLab.Core.Contexts;

namespace SortLab.Tests;

public class OddEvenSortTests
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
        OddEvenSort.Sort(array.AsSpan());
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
        OddEvenSort.Sort(array.AsSpan(), stats);

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
        OddEvenSort.Sort(array.AsSpan(), stats);

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
        OddEvenSort.Sort(array.AsSpan(), stats);

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
        OddEvenSort.Sort(sorted.AsSpan(), stats);

        // Odd-Even Sort performs one pass through the array for sorted data
        // - Odd-even pass: floor(n/2) comparisons (positions 0-1, 2-3, 4-5, ...)
        // - Even-odd pass: floor((n-1)/2) comparisons (positions 1-2, 3-4, 5-6, ...)
        // Total comparisons per pass ≈ n-1
        // For sorted data, one pass is enough to verify, so approximately n-1 comparisons
        var expectedCompares = (ulong)(n - 1);
        var expectedSwaps = 0UL;
        var expectedWrites = 0UL; // No swaps = no writes

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
        OddEvenSort.Sort(reversed.AsSpan(), stats);

        // Odd-Even Sort worst case (reversed data):
        // The algorithm needs approximately n/2 phases to sort reversed data
        // Each phase does approximately n-1 comparisons:
        // - Odd-even pass: floor(n/2) comparisons
        // - Even-odd pass: floor((n-1)/2) comparisons
        // Total comparisons ≈ (n/2) * (n-1) ≈ n²/2
        // 
        // Empirical observations:
        // - n=10: 54 comparisons
        // - n=20: 209 comparisons
        // - n=50: 1274 comparisons
        // - n=100: 5049 comparisons
        //
        // Pattern: approximately n²/2 comparisons
        var minCompares = (ulong)(n * n / 2 * 0.95); // Allow 5% below
        var maxCompares = (ulong)(n * n / 2 * 1.10); // Allow 10% above
        
        var expectedSwaps = (ulong)(n * (n - 1) / 2);
        var expectedWrites = expectedSwaps * 2; // Each swap writes 2 elements

        // Each comparison reads 2 elements
        var minIndexReads = minCompares * 2;

        Assert.InRange(stats.CompareCount, minCompares, maxCompares);
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
        OddEvenSort.Sort(random.AsSpan(), stats);

        // Odd-Even Sort performs approximately n/4 to n/2 phases for random data
        // Each phase does approximately n-1 comparisons
        // Average comparisons ≈ n²/4 to n²/2
        //
        // Empirical observations suggest random data requires fewer comparisons
        // than worst case but more than best case
        var minCompares = (ulong)(n * n / 4);
        var maxCompares = (ulong)(n * n / 2 * 1.1); // Allow 10% above worst case average

        // For random data, swap count varies significantly
        // Average case: approximately n²/8 to n(n-1)/2
        var minSwaps = (ulong)(n * n / 8);
        var maxSwaps = (ulong)(n * (n - 1) / 2);

        // Each comparison reads 2 elements
        var minIndexReads = minCompares * 2;

        Assert.InRange(stats.CompareCount, minCompares, maxCompares);
        Assert.InRange(stats.SwapCount, minSwaps, maxSwaps);
        Assert.True(stats.IndexReadCount >= minIndexReads,
            $"IndexReadCount ({stats.IndexReadCount}) should be >= {minIndexReads}");
    }
}
