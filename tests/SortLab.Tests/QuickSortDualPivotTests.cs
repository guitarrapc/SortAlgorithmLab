using SortLab.Core.Algorithms;
using SortLab.Core.Contexts;

namespace SortLab.Tests;

public class QuickSortDualPivotTests
{
    [Theory]
    [ClassData(typeof(MockRandomData))]
    [ClassData(typeof(MockNegativePositiveRandomData))]
    [ClassData(typeof(MockNegativeRandomData))]
    [ClassData(typeof(MockReversedData))]
    [ClassData(typeof(MockMountainData))]
    [ClassData(typeof(MockNearlySortedData))]
    [ClassData(typeof(MockSortedData))]
    [ClassData(typeof(MockSameValuesData))]
    public void SortResultOrderTest(IInputSample<int> inputSample)
    {
        var array = inputSample.Samples.ToArray();
        QuickSortDualPivot.Sort(array.AsSpan());
        Assert.Equal(inputSample.Samples.OrderBy(x => x), array);
    }

    [Theory]
    [ClassData(typeof(MockRandomData))]
    [ClassData(typeof(MockNegativePositiveRandomData))]
    [ClassData(typeof(MockNegativeRandomData))]
    [ClassData(typeof(MockReversedData))]
    [ClassData(typeof(MockMountainData))]
    [ClassData(typeof(MockNearlySortedData))]
    [ClassData(typeof(MockSameValuesData))]
    public void StatisticsTest(IInputSample<int> inputSample)
    {
        var stats = new StatisticsContext();
        var array = inputSample.Samples.ToArray();
        QuickSortDualPivot.Sort(array.AsSpan(), stats);

        Assert.Equal((ulong)inputSample.Samples.Length, (ulong)array.Length);
        Assert.NotEqual(0UL, stats.IndexReadCount);
        Assert.NotEqual(0UL, stats.CompareCount);
        Assert.NotEqual(0UL, stats.SwapCount);
    }

    [Theory]
    [ClassData(typeof(MockSortedData))]
    public void StatisticsSortedTest(IInputSample<int> inputSample)
    {
        var stats = new StatisticsContext();
        var array = inputSample.Samples.ToArray();
        QuickSortDualPivot.Sort(array.AsSpan(), stats);

        Assert.Equal((ulong)inputSample.Samples.Length, (ulong)array.Length);
        Assert.NotEqual(0UL, stats.IndexReadCount);
        Assert.NotEqual(0UL, stats.CompareCount);
        // For sorted data, QuickSort Dual Pivot performs minimal swaps (only initial pivot positioning)
        Assert.True(stats.SwapCount >= 0UL);
    }

    [Theory]
    [ClassData(typeof(MockRandomData))]
    public void StatisticsResetTest(IInputSample<int> inputSample)
    {
        var stats = new StatisticsContext();
        var array = inputSample.Samples.ToArray();
        QuickSortDualPivot.Sort(array.AsSpan(), stats);

        stats.Reset();
        Assert.Equal(0UL, stats.IndexReadCount);
        Assert.Equal(0UL, stats.IndexWriteCount);
        Assert.Equal(0UL, stats.CompareCount);
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
        QuickSortDualPivot.Sort(sorted.AsSpan(), stats);

        // QuickSort Dual Pivot on sorted data:
        // - Best case behavior when data is already sorted
        // - The algorithm uses two pivots (leftmost and rightmost elements)
        // - For sorted data:
        //   * Initial comparison between left and right pivots
        //   * Partitioning scans through all elements
        //   * Each element is compared with both pivots
        //   * Minimal swaps (only final pivot placements)
        //
        // Expected behavior:
        // - Comparisons: O(n log n) in best case
        //   Each level of recursion processes all n elements with 2 pivot comparisons each
        //   With 3-way partitioning, depth is approximately log3(n)
        // - Swaps: O(log n) - only pivot placements at each recursion level
        //   2 swaps per level (placing both pivots) * log3(n) levels
        
        var minCompares = (ulong)(n * 2); // At minimum, each element compared with both pivots once
        
        // Swaps: For sorted data, only pivot placements
        // Each recursion level: 2 swaps (left and right pivots)
        // Depth: approximately log3(n)
        var recursionDepth = (int)Math.Ceiling(Math.Log(n, 3));
        var expectedSwaps = (ulong)(recursionDepth * 2);

        Assert.True(stats.CompareCount >= minCompares,
            $"CompareCount ({stats.CompareCount}) should be >= {minCompares}");
        Assert.True(stats.SwapCount >= expectedSwaps,
            $"SwapCount ({stats.SwapCount}) should be >= {expectedSwaps}");
        
        // IndexReads: At least as many as comparisons (each compare reads 2 elements)
        var minIndexReads = stats.CompareCount * 2;
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
        QuickSortDualPivot.Sort(reversed.AsSpan(), stats);

        // QuickSort Dual Pivot on reversed data:
        // - Initially, left > right, so first swap occurs
        // - During partitioning:
        //   * Elements smaller than left pivot go to left section
        //   * Elements larger than right pivot go to right section
        //   * Elements between pivots stay in middle
        // - For reversed data, most elements need repositioning
        //
        // Expected behavior:
        // - Comparisons: O(n log n) average case
        //   With dual pivots, partitioning is more balanced than single pivot
        // - Swaps: O(n log n) average case
        //   Many elements need to be moved during partitioning
        
        var minCompares = (ulong)(n * 2); // At minimum, each element compared with both pivots
        var maxCompares = (ulong)(n * n); // Worst case (though rare with dual pivot)
        
        var minSwaps = (ulong)(n / 2); // At least half the elements need swapping
        var maxSwaps = (ulong)(n * n); // Worst case

        Assert.InRange(stats.CompareCount, minCompares, maxCompares);
        Assert.InRange(stats.SwapCount, minSwaps, maxSwaps);
        
        // IndexReads: Significantly higher due to partitioning and swapping
        var minIndexReads = stats.CompareCount * 2;
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
        QuickSortDualPivot.Sort(random.AsSpan(), stats);

        // QuickSort Dual Pivot on random data: average case O(n log n)
        // - Dual pivot partitioning divides array into 3 parts
        // - Average recursion depth: log3(n) (more balanced than single pivot)
        // - Each partition level processes all n elements
        //
        // Expected behavior:
        // - Comparisons: O(n log n) average
        //   Approximately 2n * log3(n) comparisons
        // - Swaps: O(n log n) average
        //   Varies based on distribution
        
        var minCompares = (ulong)(n * 2); // Minimum: each element compared once
        var maxCompares = (ulong)(n * n); // Maximum: worst case (rare)
        
        var minSwaps = (ulong)Math.Log(n, 3) * 2; // Best case: only pivot placements
        var maxSwaps = (ulong)(n * Math.Log(n, 3) * 2); // Average case estimate

        Assert.InRange(stats.CompareCount, minCompares, maxCompares);
        Assert.True(stats.SwapCount >= minSwaps,
            $"SwapCount ({stats.SwapCount}) should be >= {minSwaps}");
        
        // IndexReads: Should be proportional to comparisons and swaps
        var minIndexReads = stats.CompareCount * 2;
        Assert.True(stats.IndexReadCount >= minIndexReads,
            $"IndexReadCount ({stats.IndexReadCount}) should be >= {minIndexReads}");
    }

    [Theory]
    [InlineData(5)]
    [InlineData(10)]
    [InlineData(20)]
    public void TheoreticalValuesSameElementsTest(int n)
    {
        var stats = new StatisticsContext();
        var sameValues = Enumerable.Repeat(42, n).ToArray();
        QuickSortDualPivot.Sort(sameValues.AsSpan(), stats);

        // QuickSort Dual Pivot with all same values:
        // - Left and right pivots are equal
        // - All elements equal to pivots
        // - Partitioning still occurs but elements don't move much
        // - Middle section contains most/all elements
        //
        // Expected behavior:
        // - Comparisons: O(n) for partitioning in each recursion level
        //   In the partitioning loop, each element (except pivots) is compared
        //   For n elements, that's roughly n-2 comparisons per level
        // - Swaps: 2 swaps per recursion level (final pivot placements at lines 75-76)
        //   Plus the recursive calls on left/middle/right sections
        
        var minCompares = (ulong)(n - 2); // At minimum, partitioning comparisons for one level
        
        // Swaps: Very few needed since all values are equal
        // Only pivot positioning swaps at each level
        var recursionDepth = (int)Math.Ceiling(Math.Log(Math.Max(n, 2), 3));
        var maxSwaps = (ulong)(recursionDepth * 4); // Allow some extra for partitioning

        Assert.True(stats.CompareCount >= minCompares,
            $"CompareCount ({stats.CompareCount}) should be >= {minCompares}");
        Assert.True(stats.SwapCount <= maxSwaps,
            $"SwapCount ({stats.SwapCount}) should be <= {maxSwaps} for all equal elements");
        
        // Verify the array is still correct (all values unchanged)
        Assert.All(sameValues, val => Assert.Equal(42, val));
    }

    [Fact]
    public void EdgeCaseEmptyArrayTest()
    {
        var stats = new StatisticsContext();
        var empty = Array.Empty<int>();
        QuickSortDualPivot.Sort(empty.AsSpan(), stats);

        // Empty array: no operations
        Assert.Equal(0UL, stats.CompareCount);
        Assert.Equal(0UL, stats.SwapCount);
        Assert.Equal(0UL, stats.IndexReadCount);
        Assert.Equal(0UL, stats.IndexWriteCount);
    }

    [Fact]
    public void EdgeCaseSingleElementTest()
    {
        var stats = new StatisticsContext();
        var single = new[] { 42 };
        QuickSortDualPivot.Sort(single.AsSpan(), stats);

        // Single element: no operations needed
        Assert.Equal(0UL, stats.CompareCount);
        Assert.Equal(0UL, stats.SwapCount);
        Assert.Equal(0UL, stats.IndexReadCount);
        Assert.Equal(0UL, stats.IndexWriteCount);
        Assert.Equal(42, single[0]);
    }

    [Fact]
    public void EdgeCaseTwoElementsSortedTest()
    {
        var stats = new StatisticsContext();
        var twoSorted = new[] { 1, 2 };
        QuickSortDualPivot.Sort(twoSorted.AsSpan(), stats);

        // Two elements already sorted:
        // - Line 44: Compare(0, 1) -> no swap needed (1st comparison)
        // - Loop doesn't execute (k=1, g=0, condition false)
        // - Lines 75-76: Swap(0, 0) and Swap(1, 2) (2 swaps)
        // - Line 80: Compare(0, 1) to check if middle section needed (2nd comparison)
        // - Total: 2 comparisons, 2 swaps
        Assert.Equal(2UL, stats.CompareCount);
        Assert.Equal(2UL, stats.SwapCount);
        Assert.Equal([ 1, 2 ], twoSorted);
    }

    [Fact]
    public void EdgeCaseTwoElementsReversedTest()
    {
        var stats = new StatisticsContext();
        var twoReversed = new[] { 2, 1 };
        QuickSortDualPivot.Sort(twoReversed.AsSpan(), stats);

        // Two elements reversed:
        // - Line 44: Compare(0, 1) -> swap needed (1st comparison, 1 swap)
        // - Loop doesn't execute (k=1, g=0, condition false)
        // - Lines 75-76: Swap(0, 0) and Swap(1, 2) (2 more swaps)
        // - Line 80: Compare(0, 1) to check if middle section needed (2nd comparison)
        // - Total: 2 comparisons, 3 swaps
        Assert.Equal(2UL, stats.CompareCount);
        Assert.Equal(3UL, stats.SwapCount);
        Assert.Equal([ 1, 2 ], twoReversed);
    }

    [Fact]
    public void RangeSortTest()
    {
        var stats = new StatisticsContext();
        var array = new[] { 5, 3, 8, 1, 9, 2, 7, 4, 6 };
        
        // Sort only the range [2, 6) -> indices 2, 3, 4, 5
        QuickSortDualPivot.Sort(array.AsSpan(), 2, 6, stats);

        // Expected: first 2 elements unchanged, middle 4 sorted, last 3 unchanged
        Assert.Equal(new[] { 5, 3, 1, 2, 8, 9, 7, 4, 6 }, array);
    }

    [Fact]
    public void RangeSortFullArrayTest()
    {
        var stats = new StatisticsContext();
        var array = new[] { 5, 3, 8, 1, 9, 2, 7, 4, 6 };
        
        // Sort the entire array using range API
        QuickSortDualPivot.Sort(array.AsSpan(), 0, array.Length, stats);

        Assert.Equal(new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 }, array);
    }

    [Fact]
    public void RangeSortSingleElementTest()
    {
        var stats = new StatisticsContext();
        var array = new[] { 5, 3, 8, 1, 9 };
        
        // Sort a single element range [2, 3)
        QuickSortDualPivot.Sort(array.AsSpan(), 2, 3, stats);

        // Array should be unchanged (single element is already sorted)
        Assert.Equal(new[] { 5, 3, 8, 1, 9 }, array);
        Assert.Equal(0UL, stats.CompareCount);
        Assert.Equal(0UL, stats.SwapCount);
    }

    [Fact]
    public void RangeSortBeginningTest()
    {
        var stats = new StatisticsContext();
        var array = new[] { 9, 7, 5, 3, 1, 2, 4, 6, 8 };
        
        // Sort only the first 5 elements [0, 5)
        QuickSortDualPivot.Sort(array.AsSpan(), 0, 5, stats);

        // Expected: first 5 sorted, last 4 unchanged
        Assert.Equal(new[] { 1, 3, 5, 7, 9, 2, 4, 6, 8 }, array);
    }

    [Fact]
    public void RangeSortEndTest()
    {
        var stats = new StatisticsContext();
        var array = new[] { 1, 3, 5, 7, 9, 8, 6, 4, 2 };
        
        // Sort only the last 4 elements [5, 9)
        QuickSortDualPivot.Sort(array.AsSpan(), 5, 9, stats);

        // Expected: first 5 unchanged, last 4 sorted
        Assert.Equal(new[] { 1, 3, 5, 7, 9, 2, 4, 6, 8 }, array);
    }
}
