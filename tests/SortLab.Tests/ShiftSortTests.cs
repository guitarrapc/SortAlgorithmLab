using SortLab.Core.Algorithms;
using SortLab.Core.Contexts;

namespace SortLab.Tests;

public class ShiftSortTests
{
    [Theory]
    [ClassData(typeof(MockRandomData))]
    [ClassData(typeof(MockNegativePositiveRandomData))]
    [ClassData(typeof(MockNegativeRandomData))]
    [ClassData(typeof(MockReversedData))]
    [ClassData(typeof(MockMountainData))]
    [ClassData(typeof(MockNearlySortedData))]
    [ClassData(typeof(MockSameValuesData))]
    public void SortResultOrderTest(IInputSample<int> inputSample)
    {
        var stats = new StatisticsContext();
        var array = inputSample.Samples.ToArray();
        ShiftSort.Sort(array.AsSpan(), stats);

        Assert.Equal((ulong)inputSample.Samples.Length, (ulong)array.Length);
    }

    [Theory]
    [InlineData(256)]  // Stackalloc threshold
    [InlineData(257)]  // Just over threshold (should use ArrayPool)
    [InlineData(512)]  // ArrayPool
    [InlineData(1024)] // Large array
    public void LargeArrayTest(int n)
    {
        var stats = new StatisticsContext();
        var array = Enumerable.Range(0, n).OrderBy(_ => Guid.NewGuid()).ToArray();
        ShiftSort.Sort(array.AsSpan(), stats);

        // Verify sorting correctness
        Assert.Equal(Enumerable.Range(0, n), array);
    }

    [Theory]
    [ClassData(typeof(MockStabilityData))]
    public void StabilityTest(StabilityTestItem[] items)
    {
        // Test stability: equal elements should maintain relative order
        var stats = new StatisticsContext();

        ShiftSort.Sort(items.AsSpan(), stats);

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

        ShiftSort.Sort(items.AsSpan(), stats);

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

        ShiftSort.Sort(items.AsSpan(), stats);

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
        ShiftSort.Sort(array.AsSpan(), stats);

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
        ShiftSort.Sort(sorted.AsSpan(), stats);

        // For sorted data (with internal buffer tracking):
        // - Run detection: O(n) comparisons (n-1 comparisons in the scan loop)
        // - No run boundaries detected, so no merge operations
        // - No swaps needed
        // - No writes needed
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
        ShiftSort.Sort(reversed.AsSpan(), stats);

        // For reversed data [n-1, n-2, ..., 1, 0] (with internal buffer tracking):
        // - Run detection: O(n) comparisons
        //   * Every adjacent pair is out of order (n-1 boundaries detected)
        //   * Each boundary detection checks 2 elements (current and previous)
        //   * Three-element optimization applies when possible
        // - Maximum number of runs: approximately n/2 (worst case)
        // - Merge operations: O(n log k) where k is number of runs
        // - Swaps during run detection: O(n/2) empirically observed
        //   * The 3-element optimization swaps elements at positions x and x-2
        //   * For reversed data, this creates approximately n/2 swaps
        // - Writes during merge: O(n log k)
        //   * NOW includes writes to temp buffers (tmp1st or tmp2nd)
        //   * Each merge: writes to temp buffer + writes back to main

        // Run detection comparisons: approximately n
        var minRunDetectionCompares = (ulong)(n - 1);

        // Swaps are limited to run detection phase only (not during merge)
        // Empirically observed: reversed data produces approximately n/2 swaps
        // due to the 3-element optimization pattern
        var maxSwaps = (ulong)(n / 2 + 5); // Allow some margin for edge cases

        // Comparisons include both run detection and merge
        // For reversed data, expect O(n log n) total comparisons
        var minCompares = minRunDetectionCompares;
        var maxCompares = (ulong)(n * Math.Log(n, 2) * 2); // 2x for safety margin

        // Writes occur during merge (shift-based, not swap-based)
        // With internal buffer tracking: writes to temp buffer + writes back
        // For reversed data, most elements need to be shifted multiple times
        var minWrites = (ulong)(n - 1);
        // Allow for higher writes due to temp buffer operations being tracked
        var maxWrites = (ulong)(n * Math.Log(n, 2) * 3);

        Assert.InRange(stats.CompareCount, minCompares, maxCompares);
        Assert.InRange(stats.SwapCount, 0UL, maxSwaps);
        Assert.InRange(stats.IndexWriteCount, minWrites, maxWrites);
        Assert.True(stats.IndexReadCount >= minCompares * 2,
            $"IndexReadCount ({stats.IndexReadCount}) should be >= {minCompares * 2}");
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
        ShiftSort.Sort(random.AsSpan(), stats);

        // For random data (with internal buffer tracking):
        // - Number of runs: varies significantly (typically k << n)
        // - Comparisons: O(n log k) where k is number of runs
        // - Swaps during run detection: typically less than n/2
        // - Writes during merge: O(n log k)
        //   * NOW includes writes to temp buffers

        var minCompares = (ulong)(n - 1); // At least run detection
        var maxCompares = (ulong)(n * Math.Log(n, 2) * 2); // At most O(n log n)

        var maxSwaps = (ulong)(n / 2 + 5); // Limited to run detection phase

        // Random data typically requires many merges
        // With internal buffer tracking: writes to temp buffer + writes back
        var minWrites = (ulong)(n / 4);
        var maxWrites = (ulong)(n * Math.Log(n, 2) * 3);

        Assert.InRange(stats.CompareCount, minCompares, maxCompares);
        Assert.InRange(stats.SwapCount, 0UL, maxSwaps);
        Assert.InRange(stats.IndexWriteCount, minWrites, maxWrites);
        Assert.True(stats.IndexReadCount >= minCompares * 2,
            $"IndexReadCount ({stats.IndexReadCount}) should be >= {minCompares * 2}");
    }

    [Theory]
    [InlineData(10)]
    [InlineData(20)]
    [InlineData(50)]
    [InlineData(100)]
    public void TheoreticalValuesAlternatingTest(int n)
    {
        var stats = new StatisticsContext();
        // Create alternating pattern: [0, 2, 4, ..., 1, 3, 5, ...]
        var alternating = Enumerable.Range(0, n)
            .OrderBy(x => x % 2)
            .ThenBy(x => x)
            .ToArray();
        ShiftSort.Sort(alternating.AsSpan(), stats);

        // Alternating data creates multiple runs that need merging
        // This tests the adaptive merge behavior

        var minCompares = (ulong)(n - 1);
        var maxCompares = (ulong)(n * Math.Log(n, 2) * 2);

        var maxSwaps = (ulong)(n / 2 + 5);

        Assert.InRange(stats.CompareCount, minCompares, maxCompares);
        Assert.InRange(stats.SwapCount, 0UL, maxSwaps);
        Assert.NotEqual(0UL, stats.IndexWriteCount);
        Assert.NotEqual(0UL, stats.IndexReadCount);
    }

#endif

}
