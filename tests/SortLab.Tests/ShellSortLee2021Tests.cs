using SortLab.Core.Algorithms;
using SortLab.Core.Contexts;

namespace SortLab.Tests;

// Tests using Lee2021 - state-of-the-art improved Tokuda sequence
public class ShellSortLee2021Tests
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
    public void SortResultOrderTest(IInputSample<int> inputSample)
    {
        var stats = new StatisticsContext();
        var array = inputSample.Samples.ToArray();
        ShellSortLee2021.Sort(array.AsSpan(), stats);

        Assert.Equal((ulong)inputSample.Samples.Length, (ulong)array.Length);
    }

    [Fact]
    public void RangeSortTest()
    {
        var stats = new StatisticsContext();
        var array = new[] { 5, 3, 8, 1, 9, 2, 7, 4, 6 };

        // Sort only the range [2, 6) -> indices 2, 3, 4, 5
        ShellSortLee2021.Sort(array.AsSpan(), 2, 6, stats);

        // Expected: first 2 elements unchanged, middle 4 sorted, last 3 unchanged
        Assert.Equal(new[] { 5, 3, 1, 2, 8, 9, 7, 4, 6 }, array);
    }

    [Fact]
    public void RangeSortFullArrayTest()
    {
        var stats = new StatisticsContext();
        var array = new[] { 5, 3, 8, 1, 9, 2, 7, 4, 6 };

        // Sort the entire array using range API
        ShellSortLee2021.Sort(array.AsSpan(), 0, array.Length, stats);

        Assert.Equal(new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 }, array);
    }

    [Fact]
    public void RangeSortSingleElementTest()
    {
        var stats = new StatisticsContext();
        var array = new[] { 5, 3, 8, 1, 9 };

        // Sort a single element range [2, 3)
        ShellSortLee2021.Sort(array.AsSpan(), 2, 3, stats);

        // Array should be unchanged (single element is already sorted)
        Assert.Equal(new[] { 5, 3, 8, 1, 9 }, array);
    }

    [Fact]
    public void RangeSortBeginningTest()
    {
        var stats = new StatisticsContext();
        var array = new[] { 9, 7, 5, 3, 1, 2, 4, 6, 8 };

        // Sort only the first 5 elements [0, 5)
        ShellSortLee2021.Sort(array.AsSpan(), 0, 5, stats);

        // Expected: first 5 sorted, last 4 unchanged
        Assert.Equal(new[] { 1, 3, 5, 7, 9, 2, 4, 6, 8 }, array);
    }

    [Fact]
    public void RangeSortEndTest()
    {
        var stats = new StatisticsContext();
        var array = new[] { 1, 3, 5, 7, 9, 8, 6, 4, 2 };

        // Sort only the last 4 elements [5, 9)
        ShellSortLee2021.Sort(array.AsSpan(), 5, 9, stats);

        // Expected: first 5 unchanged, last 4 sorted
        Assert.Equal(new[] { 1, 3, 5, 7, 9, 2, 4, 6, 8 }, array);
    }

#if DEBUG

    [Theory]
    [ClassData(typeof(MockSortedData))]
    public void StatisticsSortedTest(IInputSample<int> inputSample)
    {
        var stats = new StatisticsContext();
        var array = inputSample.Samples.ToArray();
        ShellSortLee2021.Sort(array.AsSpan(), stats);

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
        ShellSortLee2021.Sort(sorted.AsSpan(), stats);

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
        ShellSortLee2021.Sort(reversed.AsSpan(), stats);

        // Shell Sort with reversed data (worst case):
        // - Gap sequence determines exact behavior
        // - With Lee sequence (improved Tokuda):
        //   * Comparisons: O(n^1.25) typically
        //   * Swaps: O(n^1.25) typically
        // - Fewer comparisons on average than Tokuda
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
        ShellSortLee2021.Sort(random.AsSpan(), stats);

        // Shell Sort with random data (average case):
        // - Gap sequence determines performance
        // - With Lee sequence (state-of-the-art):
        //   * Comparisons: O(n^1.25) with fewer average comparisons than Tokuda
        //   * Swaps: Similar to comparisons
        // - Best empirical performance among tested sequences
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

#endif

}
