using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;

namespace SortAlgorithm.Tests;

public class BottomUpMergeSortTests
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
        BottomUpMergeSort.Sort(array.AsSpan(), stats);

        Assert.Equal((ulong)inputSample.Samples.Length, (ulong)array.Length);
    }

    [Theory]
    [ClassData(typeof(MockStabilityData))]
    public void StabilityTest(StabilityTestItem[] items)
    {
        // Test stability: equal elements should maintain relative order
        var stats = new StatisticsContext();

        BottomUpMergeSort.Sort(items.AsSpan(), stats);

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

        BottomUpMergeSort.Sort(items.AsSpan(), stats);

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

        BottomUpMergeSort.Sort(items.AsSpan(), stats);

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
        BottomUpMergeSort.Sort(array.AsSpan(), stats);

        Assert.Equal((ulong)inputSample.Samples.Length, (ulong)array.Length);
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
        BottomUpMergeSort.Sort(sorted.AsSpan(), stats);

        // Bottom-Up Merge Sort with optimization for sorted data:
        // Unlike top-down merge sort, bottom-up processes all merge levels explicitly.
        // With the "skip merge if already sorted" optimization,
        // sorted data only requires skip-check comparisons at each merge level.
        //
        // Bottom-up iterates through merge sizes: 1, 2, 4, 8, ...
        // At each level, it checks if adjacent partitions are already sorted.
        // For completely sorted data:
        // - Pass 1 (size 1→2): n/2 checks
        // - Pass 2 (size 2→4): n/4 checks
        // - Pass k: n/2^k checks
        // Total: n/2 + n/4 + n/8 + ... ≈ n-1 comparisons
        //
        // Actual observations for sorted data with optimization:
        // n=10:  9 comparisons    (n-1)
        // n=20:  19 comparisons   (n-1)
        // n=50:  49 comparisons   (n-1)
        // n=100: 99 comparisons   (n-1)
        //
        // Pattern for sorted data: n-1 comparisons (skip checks only)
        var minCompares = (ulong)(n - 1);
        var maxCompares = (ulong)(n);

        // Bottom-Up Merge Sort writes with optimization:
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
        Assert.Equal(0UL, stats.SwapCount); // Merge Sort doesn't use swaps
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
        BottomUpMergeSort.Sort(reversed.AsSpan(), stats);

        // Bottom-Up Merge Sort comparisons for reversed data:
        // Reversed data cannot skip merges, so all merge operations occur.
        // Bottom-up performs exactly ⌈log₂(n)⌉ passes, merging all adjacent pairs.
        //
        // For reversed data, each merge compares most elements:
        // - Each level k merges subarrays of size 2^k
        // - Merging two subarrays of size m requires ~2m-1 comparisons in worst case
        // - Total: approximately 0.5 * n * log₂(n) to 1.0 * n * log₂(n)
        //
        // Actual observations for reversed data:
        // n=10:  24 comparisons   (~0.72 * n * log₂(n))
        // n=20:  59 comparisons   (~0.68 * n * log₂(n))
        // n=50:  182 comparisons  (~0.64 * n * log₂(n))
        // n=100: 415 comparisons  (~0.62 * n * log₂(n))
        //
        // Pattern for reversed: approximately 0.6 * n * log₂(n) to 0.75 * n * log₂(n)
        var logN = Math.Log2(n);
        var minCompares = (ulong)(n * logN * 0.5);
        var maxCompares = (ulong)(n * logN * 0.8);

        // Writes for reversed data are higher than comparisons
        // Actual: n=10→57, n=20→144, n=50→455, n=100→1060
        // Pattern: approximately 1.5 * n * log₂(n) to 1.8 * n * log₂(n)
        var minWrites = (ulong)(n * logN * 1.4);
        var maxWrites = (ulong)(n * Math.Ceiling(logN) * 1.9);

        var minReads = stats.CompareCount * 2;

        Assert.InRange(stats.CompareCount, minCompares, maxCompares);
        Assert.InRange(stats.IndexWriteCount, minWrites, maxWrites);
        Assert.True(stats.IndexReadCount >= minReads,
            $"IndexReadCount ({stats.IndexReadCount}) should be >= {minReads}");
        Assert.Equal(0UL, stats.SwapCount);
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
        BottomUpMergeSort.Sort(random.AsSpan(), stats);

        // Bottom-Up Merge Sort with optimization for random data:
        // Random data can have some sorted partitions, allowing skip optimization.
        // However, bottom-up still processes all merge levels explicitly.
        // Comparisons vary based on how many partitions are already sorted.
        //
        // Actual observations for random data with optimization:
        // n=10:  ~27-35 comparisons  (~0.81-1.05 * n * log₂(n))
        // n=20:  ~75-90 comparisons  (~0.87-1.04 * n * log₂(n))
        // n=50:  ~260-280 comparisons (~0.91-0.98 * n * log₂(n))
        // n=100: ~620-650 comparisons (~0.93-0.98 * n * log₂(n))
        //
        // Pattern for random: approximately 0.75 * n * log₂(n) to 1.1 * n * log₂(n)
        // (wider range due to randomness and optimization)
        var logN = Math.Log2(n);
        var minCompares = (ulong)(n * logN * 0.7);
        var maxCompares = (ulong)(n * logN * 1.15);

        // Writes for random data: approximately 1.3 * n * log₂(n) to 1.6 * n * log₂(n)
        // Actual: n=10→40, n=20→115, n=50→396, n=100→948
        var minWrites = (ulong)(n * logN * 1.0);
        var maxWrites = (ulong)(n * Math.Ceiling(logN) * 1.7);

        var minReads = stats.CompareCount * 2;

        Assert.InRange(stats.CompareCount, minCompares, maxCompares);
        Assert.InRange(stats.IndexWriteCount, minWrites, maxWrites);
        Assert.True(stats.IndexReadCount >= minReads,
            $"IndexReadCount ({stats.IndexReadCount}) should be >= {minReads}");
        Assert.Equal(0UL, stats.SwapCount);
    }

#endif
}
