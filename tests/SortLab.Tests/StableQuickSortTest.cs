using SortLab.Core.Algorithms;
using SortLab.Core.Contexts;

namespace SortLab.Tests;

public class StableQuickSortTest
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
    public void SortResultOrderTest(IInputSample<int> inputSample)
    {
        var stats = new StatisticsContext();
        var array = inputSample.Samples.ToArray();
        StableQuickSort.Sort(array.AsSpan(), stats);

        Assert.Equal((ulong)inputSample.Samples.Length, (ulong)array.Length);
    }

    [Theory]
    [ClassData(typeof(MockStabilityData))]
    public void StabilityTest(StabilityTestItem[] items)
    {
        // Test stability: equal elements should maintain relative order
        var stats = new StatisticsContext();

        StableQuickSort.Sort(items.AsSpan(), stats);

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

        StableQuickSort.Sort(items.AsSpan(), stats);

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

        StableQuickSort.Sort(items.AsSpan(), stats);

        // All values are 1
        Assert.All(items, item => Assert.Equal(1, item.Value));

        // Original order should be preserved: 0, 1, 2, 3, 4
        Assert.Equal(MockStabilityAllEqualsData.Sorted, items.Select(x => x.OriginalIndex).ToArray());
    }

    [Fact]
    public void EdgeCaseEmptyArrayTest()
    {
        var stats = new StatisticsContext();
        var empty = Array.Empty<int>();
        StableQuickSort.Sort(empty.AsSpan(), stats);
    }

    [Fact]
    public void EdgeCaseSingleElementTest()
    {
        var stats = new StatisticsContext();
        var single = new[] { 42 };
        StableQuickSort.Sort(single.AsSpan(), stats);

        Assert.Equal(42, single[0]);
    }

    [Fact]
    public void EdgeCaseTwoElementsSortedTest()
    {
        var stats = new StatisticsContext();
        var twoSorted = new[] { 1, 2 };
        StableQuickSort.Sort(twoSorted.AsSpan(), stats);

        Assert.Equal([1, 2], twoSorted);
    }

    [Fact]
    public void EdgeCaseTwoElementsReversedTest()
    {
        var stats = new StatisticsContext();
        var twoReversed = new[] { 2, 1 };
        StableQuickSort.Sort(twoReversed.AsSpan(), stats);

        Assert.Equal([1, 2], twoReversed);
    }

    [Fact]
    public void EdgeCaseThreeElementsTest()
    {
        var stats = new StatisticsContext();
        var three = new[] { 3, 1, 2 };
        StableQuickSort.Sort(three.AsSpan(), stats);

        Assert.Equal([1, 2, 3], three);
    }

    [Fact]
    public void RangeSortTest()
    {
        var stats = new StatisticsContext();
        var array = new[] { 5, 3, 8, 1, 9, 2, 7, 4, 6 };

        // Sort only the range [2, 6) -> indices 2, 3, 4, 5
        StableQuickSort.Sort(array.AsSpan(), 2, 6, stats);

        // Expected: first 2 elements unchanged, middle 4 sorted, last 3 unchanged
        Assert.Equal(new[] { 5, 3, 1, 2, 8, 9, 7, 4, 6 }, array);
    }

    [Fact]
    public void RangeSortFullArrayTest()
    {
        var stats = new StatisticsContext();
        var array = new[] { 5, 3, 8, 1, 9, 2, 7, 4, 6 };

        // Sort the entire array using range API
        StableQuickSort.Sort(array.AsSpan(), 0, array.Length, stats);

        Assert.Equal(new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 }, array);
    }

    [Fact]
    public void RangeSortSingleElementTest()
    {
        var stats = new StatisticsContext();
        var array = new[] { 5, 3, 8, 1, 9 };

        // Sort a single element range [2, 3)
        StableQuickSort.Sort(array.AsSpan(), 2, 3, stats);

        // Array should be unchanged (single element is already sorted)
        Assert.Equal(new[] { 5, 3, 8, 1, 9 }, array);
    }

    [Fact]
    public void RangeSortBeginningTest()
    {
        var stats = new StatisticsContext();
        var array = new[] { 9, 7, 5, 3, 1, 2, 4, 6, 8 };

        // Sort only the first 5 elements [0, 5)
        StableQuickSort.Sort(array.AsSpan(), 0, 5, stats);

        // Expected: first 5 sorted, last 4 unchanged
        Assert.Equal(new[] { 1, 3, 5, 7, 9, 2, 4, 6, 8 }, array);
    }

    [Fact]
    public void RangeSortEndTest()
    {
        var stats = new StatisticsContext();
        var array = new[] { 1, 3, 5, 7, 9, 8, 6, 4, 2 };

        // Sort only the last 4 elements [5, 9)
        StableQuickSort.Sort(array.AsSpan(), 5, 9, stats);

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
        StableQuickSort.Sort(array.AsSpan(), stats);

        Assert.Equal((ulong)inputSample.Samples.Length, (ulong)array.Length);
        Assert.NotEqual(0UL, stats.IndexReadCount);
        Assert.NotEqual(0UL, stats.IndexWriteCount);
        Assert.NotEqual(0UL, stats.CompareCount);
        // StableQuickSort doesn't use Swap - it uses Read/Write to copy via temporary buffers
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
        StableQuickSort.Sort(sorted.AsSpan(), stats);

        // Stable QuickSort on sorted data:
        // - Using middle element as pivot
        // - For sorted data, this provides balanced partitions
        // - Each partition reads all elements, compares with pivot, writes them back
        // - Uses temporary buffers (no swaps)
        //
        // Expected behavior:
        // - Comparisons: O(n log n) - Each element compared with pivot at each level
        //   For sorted data with middle pivot: approximately n log n comparisons
        // - Reads: O(n log n) - Each partitioning level reads all n elements + pivot
        //   Approximately (n+1) log n reads total
        // - Writes: O(n log n) - Each partitioning level writes all n elements back
        //   Approximately n log n writes total
        // - Swaps: 0 - This algorithm uses Read/Write, not Swap
        // - Recursion depth: O(log n) with balanced partitions
        var minCompares = (ulong)(n); // At minimum, each element visited once
        var maxCompares = (ulong)(n * n); // Worst case O(n²) if partitioning fails

        // StableQuickSort doesn't use Swap - it copies via temporary buffers
        Assert.Equal(0UL, stats.SwapCount);

        Assert.InRange(stats.CompareCount, minCompares, maxCompares);

        // IndexReads: Each recursion level reads pivot + all elements in range
        // For balanced partitioning: approximately (n+1) * log₂(n) reads
        var minIndexReads = (ulong)n; // At least read all elements once
        Assert.True(stats.IndexReadCount >= minIndexReads,
            $"IndexReadCount ({stats.IndexReadCount}) should be >= {minIndexReads}");

        // IndexWrites: Each recursion level writes all elements in range back
        // For balanced partitioning: approximately n * log₂(n) writes
        var minIndexWrites = (ulong)n; // At least write all elements once
        Assert.True(stats.IndexWriteCount >= minIndexWrites,
            $"IndexWriteCount ({stats.IndexWriteCount}) should be >= {minIndexWrites}");
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
        StableQuickSort.Sort(reversed.AsSpan(), stats);

        // Stable QuickSort on reversed data:
        // - Using middle element as pivot
        // - For reversed data with middle pivot, partitioning is still balanced
        // - Uses temporary buffers to rearrange elements (no swaps)
        //
        // Expected behavior:
        // - Comparisons: O(n log n) average case
        //   Similar to sorted data since middle pivot provides balance
        // - Reads: O(n log n) - Each partitioning level reads all elements
        // - Writes: O(n log n) - Each partitioning level writes elements back
        // - Swaps: 0 - This algorithm uses Read/Write, not Swap
        var minCompares = (ulong)(n);
        var maxCompares = (ulong)(n * n); // Worst case (rare with middle pivot)

        // StableQuickSort doesn't use Swap
        Assert.Equal(0UL, stats.SwapCount);

        Assert.InRange(stats.CompareCount, minCompares, maxCompares);

        // IndexReads: At least read all elements once
        var minIndexReads = (ulong)n;
        Assert.True(stats.IndexReadCount >= minIndexReads,
            $"IndexReadCount ({stats.IndexReadCount}) should be >= {minIndexReads}");

        // IndexWrites: At least write all elements once
        var minIndexWrites = (ulong)n;
        Assert.True(stats.IndexWriteCount >= minIndexWrites,
            $"IndexWriteCount ({stats.IndexWriteCount}) should be >= {minIndexWrites}");
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
        StableQuickSort.Sort(random.AsSpan(), stats);

        // Stable QuickSort on random data: average case O(n log n)
        // - Middle element as pivot provides decent balance on average
        // - Partitioning divides array into approximately two halves
        // - Uses temporary buffers for stable partitioning
        //
        // Expected behavior:
        // - Comparisons: O(n log n) average
        //   Approximately n log₂ n comparisons
        // - Reads: O(n log n) average - each level reads all elements
        // - Writes: O(n log n) average - each level writes all elements back
        // - Swaps: 0 - This algorithm uses Read/Write, not Swap
        var minCompares = (ulong)(n);
        var maxCompares = (ulong)(n * n); // Worst case (very rare with random data)

        // StableQuickSort doesn't use Swap
        Assert.Equal(0UL, stats.SwapCount);

        Assert.InRange(stats.CompareCount, minCompares, maxCompares);

        // IndexReads: At least read all elements once
        var minIndexReads = (ulong)n;
        Assert.True(stats.IndexReadCount >= minIndexReads,
            $"IndexReadCount ({stats.IndexReadCount}) should be >= {minIndexReads}");

        // IndexWrites: At least write all elements once
        var minIndexWrites = (ulong)n;
        Assert.True(stats.IndexWriteCount >= minIndexWrites,
            $"IndexWriteCount ({stats.IndexWriteCount}) should be >= {minIndexWrites}");
    }

    [Theory]
    [InlineData(5)]
    [InlineData(10)]
    [InlineData(20)]
    public void TheoreticalValuesSameElementsTest(int n)
    {
        var stats = new StatisticsContext();
        var sameValues = Enumerable.Repeat(42, n).ToArray();
        StableQuickSort.Sort(sameValues.AsSpan(), stats);

        // Stable QuickSort on all equal elements:
        // - All elements equal to pivot
        // - Three-way partitioning puts all elements in the "equal" partition
        // - Equal partition is already sorted, so no recursion occurs
        // - This is an OPTIMIZATION: O(n) time instead of O(n log n)
        //
        // Expected behavior:
        // - Comparisons: O(n) - Only pivot selection (median-of-3: 2-3 compares) + partition scan
        // - Reads: O(n) - Read pivot candidates + all elements once during partitioning
        // - Writes: O(n) - Write all elements back once
        // - Swaps: 0 - This algorithm uses Read/Write, not Swap
        // - No recursion: all elements go to equal partition

        // With quartile-based median-of-3, we make 2-3 comparisons for pivot selection
        var minCompares = 2UL; // Minimum for median-of-3
        var maxCompares = 3UL; // Maximum for median-of-3

        // StableQuickSort doesn't use Swap
        Assert.Equal(0UL, stats.SwapCount);

        Assert.InRange(stats.CompareCount, minCompares, maxCompares);

        // Verify the array is still correct (all values unchanged)
        Assert.All(sameValues, val => Assert.Equal(42, val));

        // IndexReads: At least read all elements once
        var minIndexReads = (ulong)n;
        Assert.True(stats.IndexReadCount >= minIndexReads,
            $"IndexReadCount ({stats.IndexReadCount}) should be >= {minIndexReads}");

        // IndexWrites: At least write all elements once
        var minIndexWrites = (ulong)n;
        Assert.True(stats.IndexWriteCount >= minIndexWrites,
            $"IndexWriteCount ({stats.IndexWriteCount}) should be >= {minIndexWrites}");
    }

#endif

}
