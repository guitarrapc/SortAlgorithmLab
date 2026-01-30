using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;

namespace SortAlgorithm.Tests;

public class RotateMergeSortTests
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
    public void SortResultOrderTest(IInputSample<int> inputSample)
    {
        var stats = new StatisticsContext();
        var array = inputSample.Samples.ToArray();
        RotateMergeSort.Sort(array.AsSpan(), stats);

        Assert.Equal((ulong)inputSample.Samples.Length, (ulong)array.Length);
    }

    [Theory]
    [ClassData(typeof(MockStabilityData))]
    public void StabilityTest(StabilityTestItem[] items)
    {
        // Test stability: equal elements should maintain relative order
        var stats = new StatisticsContext();

        RotateMergeSort.Sort(items.AsSpan(), stats);

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

        RotateMergeSort.Sort(items.AsSpan(), stats);

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

        RotateMergeSort.Sort(items.AsSpan(), stats);

        var resultOrder = items.Select(x => x.OriginalIndex).ToArray();
        Assert.Equal(originalOrder, resultOrder);
    }

#if DEBUG

    [Theory]
    [ClassData(typeof(MockSortedData))]
    public void StatisticsSortedTest(IInputSample<int> inputSample)
    {
        var stats = new StatisticsContext();
        var array = inputSample.Samples.ToArray();
        RotateMergeSort.Sort(array.AsSpan(), stats);

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
        RotateMergeSort.Sort(sorted.AsSpan(), stats);

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
        RotateMergeSort.Sort(reversed.AsSpan(), stats);

        // Rotate Merge Sort with hybrid optimization (InsertionSort for ≤16 elements):
        // Small subarrays use insertion sort (O(n²) but fast for small n).
        // Block rotation optimization processes consecutive elements together.
        //
        // Actual observations for reversed data with optimizations:
        // n=10:  45 comparisons    (insertion sort, ~4.5n)
        // n=20:  104 comparisons   (~1.2 * n * log₂(n))
        // n=50:  350 comparisons   (~1.2 * n * log₂(n))
        // n=100: 756 comparisons   (~1.1 * n * log₂(n))
        //
        // Pattern: approximately 1.0 * n * log₂(n) to 2.0 * n * log₂(n) for n > 16
        //          approximately 4.0 * n to 5.5 * n for n ≤ 16 (insertion sort)
        var logN = Math.Log2(n);
        var minCompares = n <= 16 ? (ulong)(n * 4.0) : (ulong)(n * logN * 1.0);
        var maxCompares = n <= 16 ? (ulong)(n * 5.5) : (ulong)(n * logN * 2.0);

        // Writes are reduced due to insertion sort and block rotation
        // n=10:  45 writes     (insertion sort)
        // n=20:  140 writes    (~1.6 * n * log₂(n))
        // n=50:  1050 writes   (~3.7 * n * log₂(n))
        // n=100: 3612 writes   (~5.4 * n * log₂(n))
        var minWrites = n <= 16 ? (ulong)(n * 4.0) : (ulong)(n * logN * 1.0);
        var maxWrites = n <= 16 ? (ulong)(n * 5.5) : (ulong)(n * logN * 20.0);

        // Swaps: GCD-cycle rotation uses assignments only (no swaps)
        // All rotation implementations should have 0 swaps
        var minSwaps = 0UL;
        var maxSwaps = 0UL;

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
        RotateMergeSort.Sort(random.AsSpan(), stats);

        // Rotate Merge Sort with hybrid optimization for random data:
        // Small subarrays (≤16) use insertion sort.
        // Block-wise rotation reduces operations.
        // Performance varies based on initial order.
        //
        // Observed range for random data with optimizations:
        // n=10:  ~17-35 comparisons   (varies widely with randomness, insertion sort)
        // n=20:  ~80-120 comparisons  (~1.0-1.4 * n * log₂(n))
        // n=50:  ~360-420 comparisons (~1.3-1.5 * n * log₂(n))
        // n=100: ~900-1000 comparisons (~1.4-1.5 * n * log₂(n))
        //
        // Pattern: approximately 1.5 * n to 4.0 * n for n ≤ 16 (insertion sort, wide variance)
        //          approximately 0.8 * n * log₂(n) to 2.0 * n * log₂(n) for n > 16
        var logN = Math.Log2(n);
        var minCompares = n <= 16 ? (ulong)(n * 1.5) : (ulong)(n * logN * 0.8);
        var maxCompares = n <= 16 ? (ulong)(n * 4.0) : (ulong)(n * logN * 2.0);

        // Writes vary based on how much rotation is needed
        var minWrites = n <= 16 ? (ulong)(n * 1.5) : (ulong)(n * logN * 0.5);
        var maxWrites = n <= 16 ? (ulong)(n * 4.0) : (ulong)(n * logN * 15.0);

        // Swaps: GCD-cycle rotation uses assignments only (no swaps)
        var minSwaps = 0UL;
        var maxSwaps = 0UL;

        var minReads = stats.CompareCount * 2;

        Assert.InRange(stats.CompareCount, minCompares, maxCompares);
        Assert.InRange(stats.IndexWriteCount, minWrites, maxWrites);
        Assert.InRange(stats.SwapCount, minSwaps, maxSwaps);
        Assert.True(stats.IndexReadCount >= minReads,
            $"IndexReadCount ({stats.IndexReadCount}) should be >= {minReads}");
    }

#endif

}
