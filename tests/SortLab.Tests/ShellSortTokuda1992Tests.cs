using SortLab.Core.Algorithms;
using SortLab.Core.Contexts;

namespace SortLab.Tests;

// Tests using Tokuda1992 - empirically optimized sequence with excellent practical performance
public class ShellSortTokuda1992Tests
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
        ShellSortTokuda1992.Sort(array.AsSpan());
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
        ShellSortTokuda1992.Sort(array.AsSpan(), stats);

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
        ShellSortTokuda1992.Sort(array.AsSpan(), stats);

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
        ShellSortTokuda1992.Sort(array.AsSpan(), stats);

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
        ShellSortTokuda1992.Sort(sorted.AsSpan(), stats);

        // Shell Sort with sorted data:
        // - No swaps needed (all elements already in correct positions)
        // - Comparisons depend on gap sequence, but final h=1 pass requires at least n-1 comparisons
        // - Each comparison reads 2 elements
        var expectedSwaps = 0UL;
        var expectedWrites = 0UL; // No swaps = no writes
        var minCompares = (ulong)(n - 1); // Final h=1 pass minimum

        Assert.Equal(expectedSwaps, stats.SwapCount);
        Assert.Equal(expectedWrites, stats.IndexWriteCount);
        Assert.True(stats.CompareCount >= minCompares,
            $"CompareCount ({stats.CompareCount}) should be >= {minCompares}");
        
        // Each comparison reads 2 elements, each swap also reads 2 elements
        var expectedReads = stats.CompareCount * 2 + stats.SwapCount * 2;
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
        ShellSortTokuda1992.Sort(reversed.AsSpan(), stats);

        // Shell Sort with reversed data (worst case):
        // - Gap sequence determines exact behavior
        // - With Tokuda sequence (h_k = ⌈(9/4)^k⌉):
        //   * Comparisons: O(n^1.25) typically
        //   * Swaps: O(n^1.25) typically
        // - Better than Knuth, especially for reversed data
        var minSwaps = 1UL; // At least 1 swap is needed
        var maxSwaps = (ulong)(n * n); // Upper bound (pessimistic)
        var minCompares = (ulong)n; // At least n comparisons
        
        Assert.InRange(stats.SwapCount, minSwaps, maxSwaps);
        Assert.True(stats.CompareCount >= minCompares,
            $"CompareCount ({stats.CompareCount}) should be >= {minCompares}");
        
        // Each swap writes 2 elements
        var expectedWrites = stats.SwapCount * 2;
        Assert.Equal(expectedWrites, stats.IndexWriteCount);
        
        // Each comparison reads 2 elements, each swap also reads 2 elements
        var expectedReads = stats.CompareCount * 2 + stats.SwapCount * 2;
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
        ShellSortTokuda1992.Sort(random.AsSpan(), stats);

        // Shell Sort with random data (average case):
        // - Gap sequence determines performance
        // - With Tokuda sequence (empirically optimized):
        //   * Comparisons: O(n^1.25)
        //   * Swaps: Similar to comparisons
        // - Better practical performance than Knuth
        var minSwaps = 0UL; // Could be sorted by chance
        var maxSwaps = (ulong)(n * n); // Upper bound
        var minCompares = (ulong)(n - 1); // At least n-1 comparisons in final pass
        
        Assert.InRange(stats.SwapCount, minSwaps, maxSwaps);
        Assert.True(stats.CompareCount >= minCompares,
            $"CompareCount ({stats.CompareCount}) should be >= {minCompares}");
        
        // Each swap writes 2 elements
        var expectedWrites = stats.SwapCount * 2;
        Assert.Equal(expectedWrites, stats.IndexWriteCount);
        
        // Each comparison reads 2 elements, each swap also reads 2 elements
        var expectedReads = stats.CompareCount * 2 + stats.SwapCount * 2;
        Assert.Equal(expectedReads, stats.IndexReadCount);
    }
}
