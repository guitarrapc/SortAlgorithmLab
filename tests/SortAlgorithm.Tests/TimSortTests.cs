using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;

namespace SortAlgorithm.Tests;

public class TimSortTests
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

        TimSort.Sort(array.AsSpan(), stats);

        // Check is sorted
        for (int i = 0; i < array.Length - 1; i++)
            Assert.True(array[i] <= array[i + 1]);

        // Check element counts match
        var sortedCounts = array.GroupBy(x => x).ToDictionary(g => g.Key, g => g.Count());
        Assert.Equal(originalCounts, sortedCounts);
    }

    [Theory]
    [ClassData(typeof(MockStabilityData))]
    public void StabilityTest(StabilityTestItem[] items)
    {
        // Test stability: equal elements should maintain relative order
        var stats = new StatisticsContext();

        TimSort.Sort(items.AsSpan(), stats);

        // Verify sorting correctness - values should be in ascending order
        Assert.Equal(MockStabilityData.Sorted, items.Select(x => x.Value).ToArray());

        // Verify stability: for each group of equal values, original order is preserved
        var value1Indices = items.Where(x => x.Value == 1).Select(x => x.OriginalIndex).ToArray();
        var value2Indices = items.Where(x => x.Value == 2).Select(x => x.OriginalIndex).ToArray();
        var value3Indices = items.Where(x => x.Value == 3).Select(x => x.OriginalIndex).ToArray();

        // Value 1 appeared at original indices 0, 2, 4 - should remain in this order
        Assert.Equal(MockStabilityData.Sorted1, value1Indices);

        // Value 2 appeared at original indices 1, 5 - should remain in this order
        Assert.Equal(MockStabilityData.Sorted2, value2Indices);

        // Value 3 appeared at original index 3
        Assert.Equal(MockStabilityData.Sorted3, value3Indices);
    }

    [Theory]
    [ClassData(typeof(MockStabilityWithIdData))]
    public void StabilityTestWithComplex(StabilityTestItemWithId[] items)
    {
        // Test stability with more complex scenario - multiple equal values
        var stats = new StatisticsContext();

        TimSort.Sort(items.AsSpan(), stats);

        // Expected: [2:B, 2:D, 2:F, 5:A, 5:C, 5:G, 8:E]
        // Keys are sorted, and elements with the same key maintain original order

        for (var i = 0; i < items.Length; i++)
        {
            Assert.Equal(MockStabilityWithIdData.Sorted[i].Key, items[i].Key);
            Assert.Equal(MockStabilityWithIdData.Sorted[i].Id, items[i].Id);
        }
    }

    [Theory]
    [ClassData(typeof(MockStabilityAllEqualsData))]
    public void StabilityTestWithAllEqual(StabilityTestItem[] items)
    {
        // Edge case: all elements have the same value
        // They should remain in original order
        var stats = new StatisticsContext();

        TimSort.Sort(items.AsSpan(), stats);

        // All values are 1
        Assert.All(items, item => Assert.Equal(1, item.Value));

        // Original order should be preserved: 0, 1, 2, 3, 4
        Assert.Equal(MockStabilityAllEqualsData.Sorted, items.Select(x => x.OriginalIndex).ToArray());
    }

#if DEBUG

    [Theory]
    [ClassData(typeof(MockSortedData))]
    public void StatisticsSortedTest(IInputSample<int> inputSample)
    {
        var stats = new StatisticsContext();
        var array = inputSample.Samples.ToArray();
        TimSort.Sort(array.AsSpan(), stats);

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
        TimSort.Sort(sorted.AsSpan(), stats);

        // TimSort for sorted data:
        // Small arrays (n < MIN_MERGE=32) use BinaryInsertSort directly,
        // which requires O(n log n) comparisons for binary search.
        // Larger arrays are detected as a single ascending run with O(n) comparisons.
        //
        // Actual observations for sorted data:
        // n=10:  19 comparisons   (BinaryInsertSort: binary search comparisons)
        // n=20:  54 comparisons   (BinaryInsertSort: binary search comparisons)
        // n=50:  50 comparisons   (Single run detection: n comparisons)
        // n=100: 100 comparisons  (Single run detection: n comparisons)
        //
        // Pattern: For n < 32, uses BinaryInsertSort; for n >= 32, run detection
        ulong minCompares, maxCompares;
        if (n < 32)
        {
            // BinaryInsertSort has variable comparisons due to binary search
            minCompares = (ulong)(n);
            maxCompares = (ulong)(n * Math.Ceiling(Math.Log2(n + 1)));
        }
        else
        {
            // Single run detection
            minCompares = (ulong)(n - 1);
            maxCompares = (ulong)(n + 1);
        }

        // For sorted data, no writes needed (already sorted)
        var minWrites = 0UL;
        var maxWrites = 0UL;

        Assert.InRange(stats.CompareCount, minCompares, maxCompares);
        Assert.InRange(stats.IndexWriteCount, minWrites, maxWrites);
        Assert.True(stats.IndexReadCount > 0,
            $"IndexReadCount ({stats.IndexReadCount}) should be > 0");
        Assert.Equal(0UL, stats.SwapCount); // Sorted data doesn't need swaps
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
        TimSort.Sort(reversed.AsSpan(), stats);

        // TimSort for reversed data:
        // Small arrays (n < MIN_MERGE=32) use BinaryInsertSort: O(n²) writes, O(n log n) comparisons
        // Larger arrays detect single descending run, reverse it with swaps: O(n) comparisons, O(n/2) swaps
        //
        // Actual observations for reversed data:
        // n=10:  25 comparisons, 54 writes, 0 swaps   (BinaryInsertSort)
        // n=20:  69 comparisons, 209 writes, 0 swaps  (BinaryInsertSort)
        // n=50:  50 comparisons, 50 writes, 25 swaps  (Single descending run + reverse)
        // n=100: 100 comparisons, 100 writes, 50 swaps (Single descending run + reverse)
        //
        // Pattern: For n < 32, uses BinaryInsertSort; for n >= 32, run detection + reverse
        ulong minCompares, maxCompares, minWrites, maxWrites, minSwaps, maxSwaps;
        if (n < 32)
        {
            // BinaryInsertSort
            minCompares = (ulong)(n);
            maxCompares = (ulong)(n * Math.Ceiling(Math.Log2(n + 1)));
            minWrites = (ulong)(n * (n - 1) / 4); // At least n²/4 writes
            maxWrites = (ulong)(n * (n + 1) / 2); // Up to n²/2 writes
            minSwaps = 0UL;
            maxSwaps = 0UL; // BinaryInsertSort doesn't use swaps
        }
        else
        {
            // Single descending run + reverse
            minCompares = (ulong)(n - 1);
            maxCompares = (ulong)(n + 1);
            minWrites = (ulong)(n - 1);
            maxWrites = (ulong)(n + 1);
            minSwaps = (ulong)(n / 2 - 1);
            maxSwaps = (ulong)(n / 2 + 1);
        }

        Assert.InRange(stats.CompareCount, minCompares, maxCompares);
        Assert.InRange(stats.IndexWriteCount, minWrites, maxWrites);
        Assert.InRange(stats.SwapCount, minSwaps, maxSwaps);
        Assert.True(stats.IndexReadCount > 0,
            $"IndexReadCount ({stats.IndexReadCount}) should be > 0");
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
        TimSort.Sort(random.AsSpan(), stats);

        // TimSort for random data:
        // Performance depends on natural run detection and merging.
        // Random data may contain some ascending/descending runs that are exploited.
        //
        // Observed range for random data (10 trial average):
        // n=10:  ~22 comparisons, ~30 writes, ~0 swaps
        // n=20:  ~63 comparisons, ~118 writes, ~0 swaps
        // n=50:  ~220 comparisons, ~416 writes, ~1 swap
        // n=100: ~542 comparisons, ~947 writes, ~2 swaps
        //
        // Pattern: approximately 0.4 * n * log₂(n) to 1.5 * n * log₂(n)
        // Swaps occur when descending runs are detected and reversed
        var logN = Math.Log2(n);
        var minCompares = (ulong)(n * logN * 0.4);
        var maxCompares = (ulong)(n * logN * 1.5);

        var minWrites = (ulong)(n * logN * 0.3);
        var maxWrites = (ulong)(n * logN * 2.5);

        Assert.InRange(stats.CompareCount, minCompares, maxCompares);
        Assert.InRange(stats.IndexWriteCount, minWrites, maxWrites);
        Assert.True(stats.IndexReadCount > 0,
            $"IndexReadCount ({stats.IndexReadCount}) should be > 0");
        // Random data may have descending runs that get reversed with swaps
        Assert.True(stats.SwapCount < (ulong)(n / 4),
            $"SwapCount ({stats.SwapCount}) should be less than n/4 ({n / 4})");
    }

#endif

}
