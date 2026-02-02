using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;

namespace SortAlgorithm.Tests;

public class RotateMergeSortNonOptimizedNonOptimizedTests
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
        if (inputSample.Samples.Length > 1024)
            return;

        var stats = new StatisticsContext();
        var array = inputSample.Samples.ToArray();
        var originalCounts = array.GroupBy(x => x).ToDictionary(g => g.Key, g => g.Count());

        RotateMergeSortNonOptimized.Sort(array.AsSpan(), stats);

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

        RotateMergeSortNonOptimized.Sort(items.AsSpan(), stats);

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

        RotateMergeSortNonOptimized.Sort(items.AsSpan(), stats);

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

        var originalOrder = items.Select(x => x.OriginalIndex).ToArray();

        RotateMergeSortNonOptimized.Sort(items.AsSpan(), stats);

        var resultOrder = items.Select(x => x.OriginalIndex).ToArray();
        Assert.Equal(originalOrder, resultOrder);
    }

#if DEBUG

    [Theory]
    [ClassData(typeof(MockSortedData))]
    public void StatisticsSortedTest(IInputSample<int> inputSample)
    {
        if (inputSample.Samples.Length > 1024)
            return;

        var stats = new StatisticsContext();
        var array = inputSample.Samples.ToArray();
        RotateMergeSortNonOptimized.Sort(array.AsSpan(), stats);

        Assert.Equal((ulong)inputSample.Samples.Length, (ulong)array.Length);
        Assert.NotEqual(0UL, stats.IndexReadCount);
        Assert.Equal(0UL, stats.IndexWriteCount);  // Sorted data: no writes needed
        Assert.NotEqual(0UL, stats.CompareCount);
        Assert.Equal(0UL, stats.SwapCount);  // Sorted data: no swaps needed
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
        RotateMergeSortNonOptimized.Sort(sorted.AsSpan(), stats);

        // Rotate Merge Sort with optimization for sorted data:
        // With the "skip merge if already sorted" optimization,
        // sorted data only requires skip-check comparisons (one per recursive call).
        //
        // Theoretical bounds with optimization:
        // - Sorted data: n-1 comparisons (one skip-check per partition boundary)
        //   At each recursion level with k partitions, we do k-1 skip checks.
        //   Total: (n-1) comparisons for completely sorted data
        //
        // Actual observations with optimization for sorted data:
        // n=10:  9 comparisons    (n-1)
        // n=20:  19 comparisons   (n-1)
        // n=50:  49 comparisons   (n-1)
        // n=100: 99 comparisons   (n-1)
        //
        // Pattern for sorted data: n-1 comparisons (skip checks only)
        var minCompares = (ulong)(n - 1);
        var maxCompares = (ulong)(n);

        // Rotate Merge Sort writes with optimization:
        // For sorted data, merges are skipped, so writes = 0
        var minWrites = 0UL;
        var maxWrites = 0UL;

        // Reads for sorted data: Only skip-check comparisons
        // Each comparison reads 2 elements
        var minReads = stats.CompareCount * 2;

        Assert.InRange(stats.CompareCount, minCompares, maxCompares);
        Assert.InRange(stats.IndexWriteCount, minWrites, maxWrites);
        Assert.True(stats.IndexReadCount >= minReads,
            $"IndexReadCount ({stats.IndexReadCount}) should be >= {minReads}");
        Assert.Equal(0UL, stats.SwapCount); // Sorted data: no rotation needed
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
        RotateMergeSortNonOptimized.Sort(reversed.AsSpan(), stats);

        // Rotate Merge Sort comparisons for reversed data:
        // Reversed data requires all merge operations with binary search + rotation.
        // Time complexity: O(n log² n) due to binary search (log n) at each merge level (log n).
        // Block rotation optimization processes consecutive elements together, reducing comparisons.
        //
        // Actual observations for reversed data with block rotation optimization:
        // n=10:  36-57 comparisons    (~1.1-1.7 * n * log₂(n))
        // n=20:  86-156 comparisons   (~1.0-1.8 * n * log₂(n))
        // n=50:  252-547 comparisons  (~0.9-1.9 * n * log₂(n))
        // n=100: 560-1396 comparisons (~0.8-2.1 * n * log₂(n))
        //
        // Pattern for reversed with block optimization: approximately 0.8 * n * log₂(n) to 2.5 * n * log₂(n)
        // (varies based on how effectively consecutive elements can be grouped)
        var logN = Math.Log2(n);
        var minCompares = (ulong)(n * logN * 0.8);
        var maxCompares = (ulong)(n * logN * 2.5);

        // Writes are reduced due to block rotation optimization
        // n=10:  50-90 writes       (~1.5-2.7 * n * log₂(n))
        // n=20:  140-380 writes     (~1.6-4.4 * n * log₂(n))
        // n=50:  482-2450 writes    (~1.7-8.7 * n * log₂(n))
        // n=100: 1164-9900 writes   (~1.8-14.9 * n * log₂(n))
        var minWrites = (ulong)(n * logN * 1.5);
        var maxWrites = (ulong)(n * logN * 20.0);

        // Swaps reduced by block rotation (fewer, larger rotations)
        // n=10:  25-45 swaps
        // n=20:  70-190 swaps
        // n=50:  241-1225 swaps
        // n=100: 582-4950 swaps
        var minSwaps = (ulong)(n * logN * 0.5);
        var maxSwaps = (ulong)(n * logN * 10.0);

        var minReads = stats.CompareCount * 2;

        Assert.InRange(stats.CompareCount, minCompares, maxCompares);
        Assert.InRange(stats.IndexWriteCount, minWrites, maxWrites);
        Assert.InRange(stats.SwapCount, minSwaps, maxSwaps);
        Assert.True(stats.IndexReadCount >= minReads,
            $"IndexReadCount ({stats.IndexReadCount}) should be >= {minReads}");
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
        RotateMergeSortNonOptimized.Sort(random.AsSpan(), stats);

        // Rotate Merge Sort (NonOptimized) for random data:
        // This version has block optimization (linear search for consecutive elements)
        // but no insertion sort, no galloping, uses 3-reversal rotation.
        //
        // Observed range for random data with block optimization only:
        // n=10:  ~34-51 comparisons   (can vary widely with randomness)
        // n=20:  ~115-125 comparisons (~1.3-1.5 * n * log₂(n))
        // n=50:  ~397-447 comparisons (~1.4-1.6 * n * log₂(n))
        // n=100: ~1040-1119 comparisons (~1.6-1.7 * n * log₂(n))
        //
        // Pattern for random: approximately 0.8 * n * log₂(n) to 2.5 * n * log₂(n)
        // (wider range due to randomness - set lower bound conservatively)
        var logN = Math.Log2(n);
        var minCompares = (ulong)(n * logN * 0.6);  // More conservative lower bound
        var maxCompares = (ulong)(n * logN * 2.5);

        // Writes vary based on how much rotation is needed
        // n=10:  ~20-50 writes
        // n=20:  ~150-250 writes
        // n=50:  ~800-1200 writes
        // n=100: ~4000-6500 writes
        var minWrites = (ulong)(n * logN * 0.5);
        var maxWrites = (ulong)(n * logN * 15.0);

        // Swaps occur during Reverse operations (part of rotation)
        // Range is similar to reversed but generally less
        var minSwaps = 0UL;  // Could be low if data is partially sorted
        var maxSwaps = (ulong)(n * logN * 8.0);

        var minReads = stats.CompareCount * 2;

        Assert.InRange(stats.CompareCount, minCompares, maxCompares);
        Assert.InRange(stats.IndexWriteCount, minWrites, maxWrites);
        Assert.InRange(stats.SwapCount, minSwaps, maxSwaps);
        Assert.True(stats.IndexReadCount >= minReads,
            $"IndexReadCount ({stats.IndexReadCount}) should be >= {minReads}");
    }

#endif

}
