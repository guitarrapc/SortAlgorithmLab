using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;
using SortAlgorithm.Tests.Attributes;

namespace SortAlgorithm.Tests;

public class InsertionSortNonOptimizedTests
{
    [CISkippableTheory]
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
        if (inputSample.Samples.Length > 512)
            return;

        var stats = new StatisticsContext();
        var array = inputSample.Samples.ToArray();
        var originalCounts = array.GroupBy(x => x).ToDictionary(g => g.Key, g => g.Count());

        InsertionSortNonOptimized.Sort(array.AsSpan(), stats);

        // Check is sorted
        for (int i = 0; i < array.Length - 1; i++)
            Assert.True(array[i] <= array[i + 1]);

        // Check element counts match
        var sortedCounts = array.GroupBy(x => x).ToDictionary(g => g.Key, g => g.Count());
        Assert.Equal(originalCounts, sortedCounts);
    }

    [CISkippableTheory]
    [ClassData(typeof(MockStabilityData))]
    public void StabilityTest(StabilityTestItem[] items)
    {
        // Test stability: equal elements should maintain relative order
        var stats = new StatisticsContext();

        InsertionSortNonOptimized.Sort(items.AsSpan(), stats);

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

    [CISkippableTheory]
    [ClassData(typeof(MockStabilityWithIdData))]
    public void StabilityTestWithComplex(StabilityTestItemWithId[] items)
    {
        // Test stability with more complex scenario - multiple equal values
        var stats = new StatisticsContext();

        InsertionSortNonOptimized.Sort(items.AsSpan(), stats);

        // Expected: [2:B, 2:D, 2:F, 5:A, 5:C, 5:G, 8:E]
        // Keys are sorted, and elements with the same key maintain original order

        for (var i = 0; i < items.Length; i++)
        {
            Assert.Equal(MockStabilityWithIdData.Sorted[i].Key, items[i].Key);
            Assert.Equal(MockStabilityWithIdData.Sorted[i].Id, items[i].Id);
        }
    }

    [CISkippableTheory]
    [ClassData(typeof(MockStabilityAllEqualsData))]
    public void StabilityTestWithAllEqual(StabilityTestItem[] items)
    {
        // Edge case: all elements have the same value
        // They should remain in original order
        var stats = new StatisticsContext();

        InsertionSortNonOptimized.Sort(items.AsSpan(), stats);

        // All values are 1
        Assert.All(items, item => Assert.Equal(1, item.Value));

        // Original order should be preserved: 0, 1, 2, 3, 4
        Assert.Equal(MockStabilityAllEqualsData.Sorted, items.Select(x => x.OriginalIndex).ToArray());
    }

    [Fact]
    public void RangeSortTest()
    {
        var stats = new StatisticsContext();
        var array = new[] { 5, 3, 8, 1, 9, 2, 7, 4, 6 };

        // Sort only the range [2, 6) -> indices 2, 3, 4, 5
        InsertionSortNonOptimized.Sort(array.AsSpan(), 2, 6, stats);

        // Expected: first 2 elements unchanged, middle 4 sorted, last 3 unchanged
        Assert.Equal(new[] { 5, 3, 1, 2, 8, 9, 7, 4, 6 }, array);
    }

    [Fact]
    public void RangeSortFullArrayTest()
    {
        var stats = new StatisticsContext();
        var array = new[] { 5, 3, 8, 1, 9, 2, 7, 4, 6 };

        // Sort the entire array using range API
        InsertionSortNonOptimized.Sort(array.AsSpan(), 0, array.Length, stats);

        Assert.Equal(new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 }, array);
    }

    [Fact]
    public void RangeSortSingleElementTest()
    {
        var stats = new StatisticsContext();
        var array = new[] { 5, 3, 8, 1, 9 };

        // Sort a single element range [2, 3)
        InsertionSortNonOptimized.Sort(array.AsSpan(), 2, 3, stats);

        // Array should be unchanged (single element is already sorted)
        Assert.Equal(new[] { 5, 3, 8, 1, 9 }, array);
    }

    [Fact]
    public void RangeSortBeginningTest()
    {
        var stats = new StatisticsContext();
        var array = new[] { 9, 7, 5, 3, 1, 2, 4, 6, 8 };

        // Sort only the first 5 elements [0, 5)
        InsertionSortNonOptimized.Sort(array.AsSpan(), 0, 5, stats);

        // Expected: first 5 sorted, last 4 unchanged
        Assert.Equal(new[] { 1, 3, 5, 7, 9, 2, 4, 6, 8 }, array);
    }

    [Fact]
    public void RangeSortEndTest()
    {
        var stats = new StatisticsContext();
        var array = new[] { 1, 3, 5, 7, 9, 8, 6, 4, 2 };

        // Sort only the last 4 elements [5, 9)
        InsertionSortNonOptimized.Sort(array.AsSpan(), 5, 9, stats);

        // Expected: first 5 unchanged, last 4 sorted
        Assert.Equal(new[] { 1, 3, 5, 7, 9, 2, 4, 6, 8 }, array);
    }

#if DEBUG

    [CISkippableTheory]
    [ClassData(typeof(MockSortedData))]
    public void StatisticsSortedTest(IInputSample<int> inputSample)
    {
        if (inputSample.Samples.Length > 1024)
            return;

        var stats = new StatisticsContext();
        var array = inputSample.Samples.ToArray();
        InsertionSortNonOptimized.Sort(array.AsSpan(), stats);

        Assert.Equal((ulong)inputSample.Samples.Length, (ulong)array.Length);
        Assert.NotEqual(0UL, stats.IndexReadCount);
        Assert.Equal(0UL, stats.IndexWriteCount); // Already sorted, no writes needed
        Assert.Equal((ulong)(inputSample.Samples.Length - 1), stats.CompareCount);
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
        InsertionSortNonOptimized.Sort(sorted.AsSpan(), stats);

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

    [CISkippableTheory]
    [InlineData(10)]
    [InlineData(20)]
    [InlineData(50)]
    [InlineData(100)]
    public void TheoreticalValuesReversedTest(int n)
    {
        var stats = new StatisticsContext();
        var reversed = Enumerable.Range(0, n).Reverse().ToArray();
        InsertionSortNonOptimized.Sort(reversed.AsSpan(), stats);

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

    [CISkippableTheory]
    [InlineData(10)]
    [InlineData(20)]
    [InlineData(50)]
    [InlineData(100)]
    public void TheoreticalValuesRandomTest(int n)
    {
        var stats = new StatisticsContext();
        var random = Enumerable.Range(0, n).OrderBy(_ => Guid.NewGuid()).ToArray();
        InsertionSortNonOptimized.Sort(random.AsSpan(), stats);

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

#endif

}
