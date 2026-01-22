using SortLab.Core.Algorithms;
using SortLab.Core.Contexts;
using SortLab.Tests.Attributes;

namespace SortLab.Tests;

public class CocktailShakerSortNonOptimizedTests
{
    [CISkippableTheory]
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
        CocktailShakerSortNonOptimized.Sort(array.AsSpan());
        Assert.Equal(inputSample.Samples.OrderBy(x => x), array);
    }

    [CISkippableTheory]
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
        CocktailShakerSortNonOptimized.Sort(array.AsSpan(), stats);

        Assert.Equal((ulong)inputSample.Samples.Length, (ulong)array.Length);
        Assert.NotEqual(0UL, stats.IndexReadCount);
        Assert.NotEqual(0UL, stats.IndexWriteCount);
        Assert.NotEqual(0UL, stats.CompareCount);
        Assert.NotEqual(0UL, stats.SwapCount);
    }

    [CISkippableTheory]
    [ClassData(typeof(MockSortedData))]
    public void StatisticsSortedTest(IInputSample<int> inputSample)
    {
        var stats = new StatisticsContext();
        var array = inputSample.Samples.ToArray();
        CocktailShakerSortNonOptimized.Sort(array.AsSpan(), stats);

        Assert.Equal((ulong)inputSample.Samples.Length, (ulong)array.Length);
        Assert.NotEqual(0UL, stats.IndexReadCount);
        Assert.Equal(0UL, stats.IndexWriteCount);
        Assert.NotEqual(0UL, stats.CompareCount);
        Assert.Equal(0UL, stats.SwapCount);
    }

    [CISkippableTheory]
    [ClassData(typeof(MockRandomData))]
    public void StatisticsResetTest(IInputSample<int> inputSample)
    {
        var stats = new StatisticsContext();
        var array = inputSample.Samples.ToArray();
        CocktailShakerSortNonOptimized.Sort(array.AsSpan(), stats);

        stats.Reset();
        Assert.Equal(0UL, stats.IndexReadCount);
        Assert.Equal(0UL, stats.IndexWriteCount);
        Assert.Equal(0UL, stats.CompareCount);
        Assert.Equal(0UL, stats.SwapCount);
    }

    [CISkippableTheory]
    [InlineData(10)]
    [InlineData(20)]
    [InlineData(50)]
    [InlineData(100)]
    public void TheoreticalValuesSortedTest(int n)
    {
        var stats = new StatisticsContext();
        var sorted = Enumerable.Range(0, n).ToArray();
        CocktailShakerSortNonOptimized.Sort(sorted.AsSpan(), stats);

        // Cocktail Shaker Sort (NonOptimized) - Sorted case:
        // Iteration i=0: forward (n-1 comparisons) + backward (n-2 comparisons)
        // Total: (n-1) + (n-2) = 2n-3 comparisons
        // Early termination on first iteration (swapped = false)
        // For n=10: 9 + 8 = 17 comparisons
        var expectedCompares = (ulong)(2 * n - 3);
        var expectedSwaps = 0UL;
        var expectedWrites = 0UL;

        // Each comparison reads 2 elements
        // No swaps, so reads only from comparisons
        var expectedReads = expectedCompares * 2;

        Assert.Equal(expectedCompares, stats.CompareCount);
        Assert.Equal(expectedSwaps, stats.SwapCount);
        Assert.Equal(expectedWrites, stats.IndexWriteCount);
        Assert.Equal(expectedReads, stats.IndexReadCount);
    }

    [CISkippableTheory]
    [InlineData(10)]
    [InlineData(20)]
    [InlineData(50)]
    [InlineData(100)]
    public void TheoreticalValuesReversedTest(int n)
    {
        var stats = new StatisticsContext();
        var reversed = Enumerable.Range(0, n).Reverse().ToArray();
        CocktailShakerSortNonOptimized.Sort(reversed.AsSpan(), stats);

        // Cocktail Shaker Sort (NonOptimized) - Reversed case (worst case):
        // Same as bubble sort: n(n-1)/2 comparisons and swaps
        // Each swap writes 2 elements
        var expectedCompares = (ulong)(n * (n - 1) / 2);
        var expectedSwaps = (ulong)(n * (n - 1) / 2);
        var expectedWrites = expectedSwaps * 2;

        // Each comparison reads 2 elements: Compare(i, j) reads i and j
        // Each swap also reads 2 elements: Swap(i, j) reads i and j before writing
        // Total reads = (compares * 2) + (swaps * 2)
        var expectedReads = (expectedCompares * 2) + (expectedSwaps * 2);

        Assert.Equal(expectedCompares, stats.CompareCount);
        Assert.Equal(expectedSwaps, stats.SwapCount);
        Assert.Equal(expectedWrites, stats.IndexWriteCount);
        Assert.Equal(expectedReads, stats.IndexReadCount);
    }

    [CISkippableTheory]
    [InlineData(10)]
    [InlineData(20)]
    [InlineData(50)]
    [InlineData(100)]
    public void TheoreticalValuesRandomTest(int n)
    {
        var stats = new StatisticsContext();
        var random = Enumerable.Range(0, n).OrderBy(_ => Guid.NewGuid()).ToArray();
        CocktailShakerSortNonOptimized.Sort(random.AsSpan(), stats);

        // Cocktail Shaker Sort (NonOptimized) - Random case:
        // Has early termination (if !swapped break)
        // Best case (sorted): 2n-3 comparisons
        // Worst case (no early termination): n(n-1)/2 comparisons
        // Swaps: Average n(n-1)/4 for random data
        var minCompares = (ulong)(2 * n - 3);  // Best case (already sorted)
        var maxCompares = (ulong)(n * (n - 1) / 2);  // Worst case
        var maxSwaps = (ulong)(n * (n - 1) / 2);

        Assert.InRange(stats.CompareCount, minCompares, maxCompares);
        Assert.InRange(stats.SwapCount, 0UL, maxSwaps);
        
        // IndexReadCount = (CompareCount * 2) + (SwapCount * 2)
        // Because both Compare and Swap read 2 elements each
        var expectedReads = (stats.CompareCount * 2) + (stats.SwapCount * 2);
        Assert.Equal(expectedReads, stats.IndexReadCount);
        Assert.Equal(stats.SwapCount * 2, stats.IndexWriteCount);
    }
}
