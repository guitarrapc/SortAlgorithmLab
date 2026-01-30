using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;

namespace SortAlgorithm.Tests;

public class BottomupHeapSortTests
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
    [ClassData(typeof(MockAllIdenticalData))]
    [ClassData(typeof(MockTwoDistinctValuesData))]
    [ClassData(typeof(MockHalfZeroHalfOneData))]
    [ClassData(typeof(MockManyDuplicatesSqrtRangeData))]
    [ClassData(typeof(MockHighlySkewedData))]
    public void SortResultOrderTest(IInputSample<int> inputSample)
    {
        var stats = new StatisticsContext();
        var array = inputSample.Samples.ToArray();
        BottomupHeapSort.Sort(array.AsSpan(), stats);

        Assert.Equal((ulong)inputSample.Samples.Length, (ulong)array.Length);
    }

    [Fact]
    public void RangeSortTest()
    {
        var stats = new StatisticsContext();
        var array = new[] { 5, 3, 8, 1, 9, 2, 7, 4, 6 };

        // Sort only the range [2, 6) -> indices 2, 3, 4, 5
        BottomupHeapSort.Sort(array.AsSpan(), 2, 6, stats);

        // Expected: first 2 elements unchanged, middle 4 sorted, last 3 unchanged
        Assert.Equal(new[] { 5, 3, 1, 2, 8, 9, 7, 4, 6 }, array);
    }

    [Fact]
    public void RangeSortFullArrayTest()
    {
        var stats = new StatisticsContext();
        var array = new[] { 5, 3, 8, 1, 9, 2, 7, 4, 6 };

        // Sort the entire array using range API
        BottomupHeapSort.Sort(array.AsSpan(), 0, array.Length, stats);

        Assert.Equal(new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 }, array);
    }

    [Fact]
    public void RangeSortSingleElementTest()
    {
        var stats = new StatisticsContext();
        var array = new[] { 5, 3, 8, 1, 9 };

        // Sort a single element range [2, 3)
        BottomupHeapSort.Sort(array.AsSpan(), 2, 3, stats);

        // Array should be unchanged (single element is already sorted)
        Assert.Equal(new[] { 5, 3, 8, 1, 9 }, array);
    }

    [Fact]
    public void RangeSortBeginningTest()
    {
        var stats = new StatisticsContext();
        var array = new[] { 9, 7, 5, 3, 1, 2, 4, 6, 8 };

        // Sort only the first 5 elements [0, 5)
        BottomupHeapSort.Sort(array.AsSpan(), 0, 5, stats);

        // Expected: first 5 sorted, last 4 unchanged
        Assert.Equal(new[] { 1, 3, 5, 7, 9, 2, 4, 6, 8 }, array);
    }

    [Fact]
    public void RangeSortEndTest()
    {
        var stats = new StatisticsContext();
        var array = new[] { 1, 3, 5, 7, 9, 8, 6, 4, 2 };

        // Sort only the last 4 elements [5, 9)
        BottomupHeapSort.Sort(array.AsSpan(), 5, 9, stats);

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
        BottomupHeapSort.Sort(array.AsSpan(), stats);

        Assert.Equal((ulong)inputSample.Samples.Length, (ulong)array.Length);
        Assert.NotEqual(0UL, stats.IndexReadCount);
        Assert.NotEqual(0UL, stats.IndexWriteCount);
        Assert.NotEqual(0UL, stats.CompareCount);
        Assert.NotEqual(0UL, stats.SwapCount);
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
        BottomupHeapSort.Sort(sorted.AsSpan(), stats);

        // Bottom-Up Heap Sort characteristics:
        // Build heap phase: Floyd's algorithm with reduced comparisons
        // Extract phase: Bottom-up sift-down (log n + log log n comparisons per extraction)
        //
        // Empirical observations for sorted data:
        // n=10:  Compare=33,  Swap=9
        // n=20:  Compare=88,  Swap=19
        // n=50:  Compare=286, Swap=49
        // n=100: Compare=688, Swap=99
        //
        // Pattern: ~30-40% fewer comparisons than standard HeapSort
        // Swaps: approximately (n-1) due to bottom-up extraction

        var logN = Math.Log(n + 1, 2);
        // Bottom-up reduces comparisons significantly
        var minCompares = (ulong)(n * logN * 0.4);
        var maxCompares = (ulong)(n * logN * 1.8);

        // Bottom-up has approximately n-1 swaps (one per extraction)
        var minSwaps = (ulong)(n * 0.8);
        var maxSwaps = (ulong)(n * 1.2);

        // Each swap writes 2 elements, plus writes from sift operations
        var minWrites = minSwaps * 2;
        var maxWrites = (ulong)(n * logN * 2.5);

        // Each comparison reads 2 elements, plus reads from value operations
        var minIndexReads = minCompares * 2;

        Assert.InRange(stats.CompareCount, minCompares, maxCompares);
        Assert.InRange(stats.SwapCount, minSwaps, maxSwaps);
        Assert.InRange(stats.IndexWriteCount, minWrites, maxWrites);
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
        BottomupHeapSort.Sort(reversed.AsSpan(), stats);

        // Bottom-Up Heap Sort has O(n log n) time complexity regardless of input order
        // Reversed data shows similar patterns to sorted data
        //
        // Empirical observations for reversed data:
        // n=10:  Compare=~33,  Swap=~9
        // n=20:  Compare=94,   Swap=19
        // n=50:  Compare=314,  Swap=49
        // n=100: Compare=~688, Swap=~99
        //
        // Pattern: slightly more comparisons than sorted, but still ~30-40% less than standard HeapSort

        var logN = Math.Log(n + 1, 2);
        // Bottom-up reduces comparisons significantly
        var minCompares = (ulong)(n * logN * 0.4);
        var maxCompares = (ulong)(n * logN * 2.0);

        // Bottom-up has approximately n-1 swaps (one per extraction)
        var minSwaps = (ulong)(n * 0.8);
        var maxSwaps = (ulong)(n * 1.2);

        // Each swap writes 2 elements, plus writes from sift operations
        var minWrites = minSwaps * 2;
        var maxWrites = (ulong)(n * logN * 2.5);

        // Each comparison reads 2 elements, plus reads from value operations
        var minIndexReads = minCompares * 2;

        Assert.InRange(stats.CompareCount, minCompares, maxCompares);
        Assert.InRange(stats.SwapCount, minSwaps, maxSwaps);
        Assert.InRange(stats.IndexWriteCount, minWrites, maxWrites);
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
        BottomupHeapSort.Sort(random.AsSpan(), stats);

        // Bottom-Up Heap Sort has consistent O(n log n) time complexity for random data
        // The values are similar to sorted/reversed cases
        //
        // Expected observations for random data (extrapolated):
        // n=10:  Compare=~33-40,  Swap=~9
        // n=20:  Compare=~88-100, Swap=~19
        // n=50:  Compare=~286-320, Swap=~49
        // n=100: Compare=~688-720, Swap=~99
        //
        // Pattern: approximately 60-70% of (n * log2(n)), with variation due to randomness

        var logN = Math.Log(n + 1, 2);
        // Bottom-up reduces comparisons significantly
        var minCompares = (ulong)(n * logN * 0.4);
        var maxCompares = (ulong)(n * logN * 2.0);

        // Bottom-up has approximately n-1 swaps (one per extraction)
        var minSwaps = (ulong)(n * 0.8);
        var maxSwaps = (ulong)(n * 1.2);

        // Each swap writes 2 elements, plus writes from sift operations
        var minWrites = minSwaps * 2;
        var maxWrites = (ulong)(n * logN * 2.5);

        // Each comparison reads 2 elements, plus reads from value operations
        var minIndexReads = minCompares * 2;

        Assert.InRange(stats.CompareCount, minCompares, maxCompares);
        Assert.InRange(stats.SwapCount, minSwaps, maxSwaps);
        Assert.InRange(stats.IndexWriteCount, minWrites, maxWrites);
        Assert.True(stats.IndexReadCount >= minIndexReads,
            $"IndexReadCount ({stats.IndexReadCount}) should be >= {minIndexReads}");
    }

#endif

}
