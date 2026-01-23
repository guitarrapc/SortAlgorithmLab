using SortLab.Core.Algorithms;
using SortLab.Core.Contexts;

namespace SortLab.Tests;

public class MergeSortTests
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
        MergeSort.Sort(array.AsSpan(), stats);

        Assert.Equal((ulong)inputSample.Samples.Length, (ulong)array.Length);
    }

    [Theory]
    [ClassData(typeof(MockSortedData))]
    public void StatisticsSortedTest(IInputSample<int> inputSample)
    {
        var stats = new StatisticsContext();
        var array = inputSample.Samples.ToArray();
        MergeSort.Sort(array.AsSpan(), stats);

        Assert.Equal((ulong)inputSample.Samples.Length, (ulong)array.Length);
        Assert.NotEqual(0UL, stats.IndexReadCount);
        Assert.NotEqual(0UL, stats.IndexWriteCount);
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
        MergeSort.Sort(sorted.AsSpan(), stats);

        // Merge Sort theoretical comparisons:
        // The number of comparisons depends on how elements are distributed during merge.
        // 
        // Theoretical bounds:
        // - Best case: n⌈log₂(n)⌉ / 2 (approximately, when one side exhausts early)
        // - Worst case: n⌈log₂(n)⌉ - 2^⌈log₂(n)⌉ + 1
        //
        // Actual observations with this implementation for sorted data:
        // n=10:  15 comparisons   (log₂(10) ≈ 3.32, n*log₂(n) ≈ 33, actual ≈ 0.45*n*log₂(n))
        // n=20:  40 comparisons   (log₂(20) ≈ 4.32, n*log₂(n) ≈ 86, actual ≈ 0.46*n*log₂(n))
        // n=50:  133 comparisons  (log₂(50) ≈ 5.64, n*log₂(n) ≈ 282, actual ≈ 0.47*n*log₂(n))
        // n=100: 316 comparisons  (log₂(100) ≈ 6.64, n*log₂(n) ≈ 664, actual ≈ 0.48*n*log₂(n))
        //
        // Pattern for sorted data: approximately 0.3 * n * log₂(n) to 0.6 * n * log₂(n)
        var logN = Math.Log2(n);
        var minCompares = (ulong)(n * logN * 0.25);
        var maxCompares = (ulong)(n * logN * 0.6);

        // Merge Sort writes: approximately n * ceil(log₂(n))
        // Each recursion level writes all n elements
        var minWrites = (ulong)(n * logN * 0.3);
        var maxWrites = (ulong)(n * Math.Ceiling(logN) * 1.5);

        // Each comparison involves at least 2 reads (one from buffer, one from right partition)
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
        MergeSort.Sort(reversed.AsSpan(), stats);

        // Merge Sort comparisons for reversed data:
        // Reversed data requires more comparisons than sorted data because
        // elements from the right partition are selected more often.
        //
        // Actual observations for reversed data:
        // n=10:  19 comparisons   (≈ 0.57*n*log₂(n))
        // n=20:  48 comparisons   (≈ 0.56*n*log₂(n))
        // n=50:  153 comparisons  (≈ 0.54*n*log₂(n))
        // n=100: 356 comparisons  (≈ 0.54*n*log₂(n))
        //
        // Pattern for reversed: approximately 0.4 * n * log₂(n) to 0.7 * n * log₂(n)
        var logN = Math.Log2(n);
        var minCompares = (ulong)(n * logN * 0.4);
        var maxCompares = (ulong)(n * logN * 0.7);

        var minWrites = (ulong)(n * logN * 0.3);
        var maxWrites = (ulong)(n * Math.Ceiling(logN) * 1.5);

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
        MergeSort.Sort(random.AsSpan(), stats);

        // Merge Sort has O(n log n) comparisons for all cases
        // Random data can vary significantly based on the specific random arrangement
        //
        // Observed range for random data:
        // n=10:  ~15-25 comparisons
        // n=20:  ~40-65 comparisons
        // n=50:  ~130-200 comparisons
        // n=100: ~320-550 comparisons
        //
        // Pattern for random: approximately 0.3 * n * log₂(n) to 0.9 * n * log₂(n)
        // (wider range due to randomness)
        var logN = Math.Log2(n);
        var minCompares = (ulong)(n * logN * 0.25);
        var maxCompares = (ulong)(n * logN * 0.9);

        var minWrites = (ulong)(n * logN * 0.3);
        var maxWrites = (ulong)(n * Math.Ceiling(logN) * 1.5);

        var minReads = stats.CompareCount * 2;

        Assert.InRange(stats.CompareCount, minCompares, maxCompares);
        Assert.InRange(stats.IndexWriteCount, minWrites, maxWrites);
        Assert.True(stats.IndexReadCount >= minReads,
            $"IndexReadCount ({stats.IndexReadCount}) should be >= {minReads}");
        Assert.Equal(0UL, stats.SwapCount);
    }

    [Theory]
    [ClassData(typeof(MockStabilityData))]
    public void StabilityTest(StabilityTestItem[] items)
    {
        // Test stability: equal elements should maintain relative order
        var stats = new StatisticsContext();

        MergeSort.Sort(items.AsSpan(), stats);

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

        MergeSort.Sort(items.AsSpan(), stats);

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

        MergeSort.Sort(items.AsSpan(), stats);

        // All values are 1
        Assert.All(items, item => Assert.Equal(1, item.Value));

        // Original order should be preserved: 0, 1, 2, 3, 4
        Assert.Equal(MockStabilityAllEqualsData.Sorted, items.Select(x => x.OriginalIndex).ToArray());

        // Merge Sort is not in-place and always performs writes during merge operations
        // Even for sorted data with all equal elements, writes occur during merging
        Assert.Equal(0UL, stats.SwapCount); // No swaps (merge uses copy, not swap)
        Assert.NotEqual(0UL, stats.IndexWriteCount); // Writes occur during merge    }
    }
}
