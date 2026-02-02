using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;

namespace SortAlgorithm.Tests;

public class HeapSortTests
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
        var originalCounts = array.GroupBy(x => x).ToDictionary(g => g.Key, g => g.Count());

        HeapSort.Sort(array.AsSpan(), stats);

        // Check is sorted
        for (int i = 0; i < array.Length - 1; i++)
            Assert.True(array[i] <= array[i + 1]);

        // Check element counts match
        var sortedCounts = array.GroupBy(x => x).ToDictionary(g => g.Key, g => g.Count());
        Assert.Equal(originalCounts, sortedCounts);
    }

    [Fact]
    public void RangeSortTest()
    {
        var stats = new StatisticsContext();
        var array = new[] { 5, 3, 8, 1, 9, 2, 7, 4, 6 };

        // Sort only the range [2, 6) -> indices 2, 3, 4, 5
        HeapSort.Sort(array.AsSpan(), 2, 6, stats);

        // Expected: first 2 elements unchanged, middle 4 sorted, last 3 unchanged
        Assert.Equal(new[] { 5, 3, 1, 2, 8, 9, 7, 4, 6 }, array);
    }

    [Fact]
    public void RangeSortFullArrayTest()
    {
        var stats = new StatisticsContext();
        var array = new[] { 5, 3, 8, 1, 9, 2, 7, 4, 6 };

        // Sort the entire array using range API
        HeapSort.Sort(array.AsSpan(), 0, array.Length, stats);

        Assert.Equal(new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 }, array);
    }

    [Fact]
    public void RangeSortSingleElementTest()
    {
        var stats = new StatisticsContext();
        var array = new[] { 5, 3, 8, 1, 9 };

        // Sort a single element range [2, 3)
        HeapSort.Sort(array.AsSpan(), 2, 3, stats);

        // Array should be unchanged (single element is already sorted)
        Assert.Equal(new[] { 5, 3, 8, 1, 9 }, array);
    }

    [Fact]
    public void RangeSortBeginningTest()
    {
        var stats = new StatisticsContext();
        var array = new[] { 9, 7, 5, 3, 1, 2, 4, 6, 8 };

        // Sort only the first 5 elements [0, 5)
        HeapSort.Sort(array.AsSpan(), 0, 5, stats);

        // Expected: first 5 sorted, last 4 unchanged
        Assert.Equal(new[] { 1, 3, 5, 7, 9, 2, 4, 6, 8 }, array);
    }

    [Fact]
    public void RangeSortEndTest()
    {
        var stats = new StatisticsContext();
        var array = new[] { 1, 3, 5, 7, 9, 8, 6, 4, 2 };

        // Sort only the last 4 elements [5, 9)
        HeapSort.Sort(array.AsSpan(), 5, 9, stats);

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
        HeapSort.Sort(array.AsSpan(), stats);

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
        HeapSort.Sort(sorted.AsSpan(), stats);

        // Heap Sort characteristics:
        // Build heap phase: O(n) comparisons with some swaps even for sorted data
        // Extract phase: (n-1) extractions, each requiring O(log n) heapify
        //
        // Empirical observations for sorted data:
        // n=10:  Compare=41,  Swap=30
        // n=20:  Compare=121, Swap=80
        // n=50:  Compare=405, Swap=238
        // n=100: Compare=1031, Swap=581
        //
        // Pattern: approximately n * log2(n) for both compares and swaps

        var logN = Math.Log(n + 1, 2);
        var minCompares = (ulong)(n * logN * 0.5);
        var maxCompares = (ulong)(n * logN * 2.5 + n);

        var minSwaps = (ulong)(n * logN * 0.4);
        var maxSwaps = (ulong)(n * logN * 1.5);

        // Each swap writes 2 elements
        var minWrites = minSwaps * 2;
        var maxWrites = maxSwaps * 2;

        // Each comparison reads 2 elements
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
        HeapSort.Sort(reversed.AsSpan(), stats);

        // Heap Sort has O(n log n) time complexity regardless of input order
        // Reversed data shows similar patterns to sorted data due to heap property
        //
        // Empirical observations for reversed data:
        // n=10:  Compare=38,  Swap=27
        // n=20:  Compare=115, Swap=74
        // n=50:  Compare=401, Swap=234
        // n=100: Compare=1023, Swap=573
        //
        // Pattern: approximately n * log2(n) for both compares and swaps

        var logN = Math.Log(n + 1, 2);
        var minCompares = (ulong)(n * logN * 0.5);
        var maxCompares = (ulong)(n * logN * 2.5 + n);

        var minSwaps = (ulong)(n * logN * 0.4);
        var maxSwaps = (ulong)(n * logN * 1.5);

        // Each swap writes 2 elements
        var minWrites = minSwaps * 2;
        var maxWrites = maxSwaps * 2;

        // Each comparison reads 2 elements
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
        HeapSort.Sort(random.AsSpan(), stats);

        // Heap Sort has consistent O(n log n) time complexity for random data
        // The values are similar to sorted/reversed cases
        //
        // Empirical observations for random data (example):
        // n=10:  Compare=38,  Swap=27
        // n=20:  Compare=113, Swap=72
        // n=50:  Compare=405, Swap=238
        // n=100: Compare=1031, Swap=581
        //
        // Pattern: approximately n * log2(n), with variation due to randomness

        var logN = Math.Log(n + 1, 2);
        var minCompares = (ulong)(n * logN * 0.5);
        var maxCompares = (ulong)(n * logN * 2.5 + n);

        var minSwaps = (ulong)(n * logN * 0.4);
        var maxSwaps = (ulong)(n * logN * 1.5);

        // Each swap writes 2 elements
        var minWrites = minSwaps * 2;
        var maxWrites = maxSwaps * 2;

        // Each comparison reads 2 elements
        var minIndexReads = minCompares * 2;

        Assert.InRange(stats.CompareCount, minCompares, maxCompares);
        Assert.InRange(stats.SwapCount, minSwaps, maxSwaps);
        Assert.InRange(stats.IndexWriteCount, minWrites, maxWrites);
        Assert.True(stats.IndexReadCount >= minIndexReads,
            $"IndexReadCount ({stats.IndexReadCount}) should be >= {minIndexReads}");
    }

#endif

}
