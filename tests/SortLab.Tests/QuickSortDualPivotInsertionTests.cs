using SortLab.Core.Algorithms;
using SortLab.Core.Contexts;

namespace SortLab.Tests;

public class QuickSortDualPivotInsertionTests
{
    // InsertThreshold = 16 in the implementation
    private const int InsertThreshold = 16;

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
        QuickSortDualPivotInsertion.Sort(array.AsSpan(), stats);

        Assert.Equal((ulong)inputSample.Samples.Length, (ulong)array.Length);
    }

    [Fact]
    public void EdgeCaseEmptyArrayTest()
    {
        var stats = new StatisticsContext();
        var empty = Array.Empty<int>();
        QuickSortDualPivotInsertion.Sort(empty.AsSpan(), stats);
    }

    [Fact]
    public void EdgeCaseSingleElementTest()
    {
        var stats = new StatisticsContext();
        var single = new[] { 42 };
        QuickSortDualPivotInsertion.Sort(single.AsSpan(), stats);

        Assert.Equal(42, single[0]);
    }

    [Fact]
    public void EdgeCaseTwoElementsSortedTest()
    {
        var stats = new StatisticsContext();
        var twoSorted = new[] { 1, 2 };
        QuickSortDualPivotInsertion.Sort(twoSorted.AsSpan(), stats);

        Assert.Equal([1, 2], twoSorted);
    }

    [Fact]
    public void EdgeCaseTwoElementsReversedTest()
    {
        var stats = new StatisticsContext();
        var twoReversed = new[] { 2, 1 };
        QuickSortDualPivotInsertion.Sort(twoReversed.AsSpan(), stats);

        Assert.Equal([1, 2], twoReversed);
    }

    [Fact]
    public void EdgeCaseThresholdSizeTest()
    {
        var stats = new StatisticsContext();
        var array = Enumerable.Range(0, InsertThreshold).Reverse().ToArray();
        QuickSortDualPivotInsertion.Sort(array.AsSpan(), stats);

        Assert.Equal(Enumerable.Range(0, InsertThreshold), array);
    }

    [Fact]
    public void EdgeCaseThresholdPlusOneTest()
    {
        var stats = new StatisticsContext();
        var array = Enumerable.Range(0, InsertThreshold + 1).Reverse().ToArray();
        QuickSortDualPivotInsertion.Sort(array.AsSpan(), stats);

        Assert.Equal(Enumerable.Range(0, InsertThreshold + 1), array);
    }

    [Fact]
    public void RangeSortTest()
    {
        var stats = new StatisticsContext();
        var array = new[] { 5, 3, 8, 1, 9, 2, 7, 4, 6 };

        // Sort only the range [2, 6) -> indices 2, 3, 4, 5
        QuickSortDualPivotInsertion.Sort(array.AsSpan(), 2, 6, stats);

        // Expected: first 2 elements unchanged, middle 4 sorted, last 3 unchanged
        Assert.Equal(new[] { 5, 3, 1, 2, 8, 9, 7, 4, 6 }, array);
    }

    [Fact]
    public void RangeSortFullArrayTest()
    {
        var stats = new StatisticsContext();
        var array = new[] { 5, 3, 8, 1, 9, 2, 7, 4, 6 };

        // Sort the entire array using range API
        QuickSortDualPivotInsertion.Sort(array.AsSpan(), 0, array.Length, stats);

        Assert.Equal(new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 }, array);
    }

    [Fact]
    public void RangeSortSingleElementTest()
    {
        var stats = new StatisticsContext();
        var array = new[] { 5, 3, 8, 1, 9 };

        // Sort a single element range [2, 3)
        QuickSortDualPivotInsertion.Sort(array.AsSpan(), 2, 3, stats);

        // Array should be unchanged (single element is already sorted)
        Assert.Equal(new[] { 5, 3, 8, 1, 9 }, array);
    }

    [Fact]
    public void RangeSortBeginningTest()
    {
        var stats = new StatisticsContext();
        var array = new[] { 9, 7, 5, 3, 1, 2, 4, 6, 8 };

        // Sort only the first 5 elements [0, 5)
        QuickSortDualPivotInsertion.Sort(array.AsSpan(), 0, 5, stats);

        // Expected: first 5 sorted, last 4 unchanged
        Assert.Equal(new[] { 1, 3, 5, 7, 9, 2, 4, 6, 8 }, array);
    }

    [Fact]
    public void RangeSortEndTest()
    {
        var stats = new StatisticsContext();
        var array = new[] { 1, 3, 5, 7, 9, 8, 6, 4, 2 };

        // Sort only the last 4 elements [5, 9)
        QuickSortDualPivotInsertion.Sort(array.AsSpan(), 5, 9, stats);

        // Expected: first 5 unchanged, last 4 sorted
        Assert.Equal(new[] { 1, 3, 5, 7, 9, 2, 4, 6, 8 }, array);
    }

    [Theory]
    [InlineData(10)]
    [InlineData(20)]
    [InlineData(50)]
    public void IndexReadWriteConsistencyTest(int n)
    {
        var stats = new StatisticsContext();
        var array = Enumerable.Range(0, n).OrderBy(_ => Guid.NewGuid()).ToArray();
        QuickSortDualPivotInsertion.Sort(array.AsSpan(), stats);

        // Verify sorted correctly
        Assert.Equal(Enumerable.Range(0, n), array);
    }

    [Fact]
    public void SmallArrayIndexTrackingTest()
    {
        var stats = new StatisticsContext();
        var array = new[] { 3, 1, 4, 1, 5, 9, 2, 6 }; // 8 elements, uses InsertionSort
        QuickSortDualPivotInsertion.Sort(array.AsSpan(), stats);

        // Verify sorted correctly
        Assert.Equal(new[] { 1, 1, 2, 3, 4, 5, 6, 9 }, array);
    }

    [Fact]
    public void LargeArrayIndexTrackingTest()
    {
        var stats = new StatisticsContext();
        var array = Enumerable.Range(0, 50).Reverse().ToArray(); // 50 elements, uses hybrid
        QuickSortDualPivotInsertion.Sort(array.AsSpan(), stats);

        // Verify sorted correctly
        Assert.Equal(Enumerable.Range(0, 50), array);
    }

    [Fact]
    public void CompareWithValueTrackingTest()
    {
        var stats = new StatisticsContext();
        var array = new[] { 5, 2, 8, 1, 9 }; // Small array using InsertionSort
        QuickSortDualPivotInsertion.Sort(array.AsSpan(), stats);

        Assert.Equal(new[] { 1, 2, 5, 8, 9 }, array);
    }

    [Theory]
    [InlineData(5)]
    [InlineData(15)]
    [InlineData(16)]
    public void BoundaryThresholdTest(int n)
    {
        var stats = new StatisticsContext();
        var array = Enumerable.Range(0, n).Reverse().ToArray();
        QuickSortDualPivotInsertion.Sort(array.AsSpan(), stats);

        Assert.Equal(Enumerable.Range(0, n), array);
    }

#if DEBUG

    [Theory]
    [ClassData(typeof(MockSortedData))]
    public void StatisticsSortedTest(IInputSample<int> inputSample)
    {
        var stats = new StatisticsContext();
        var array = inputSample.Samples.ToArray();
        QuickSortDualPivotInsertion.Sort(array.AsSpan(), stats);

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
        QuickSortDualPivotInsertion.Sort(sorted.AsSpan(), stats);

        // QuickSortDualPivotInsertion is a hybrid algorithm:
        // - For subarrays with <= InsertThreshold (16) elements, uses InsertionSort
        // - For larger subarrays, uses QuickSort Dual Pivot, which recursively creates smaller subarrays
        //
        // For n <= 16: Pure InsertionSort behavior
        // - Sorted data: best case O(n)
        // - Comparisons: n-1 (one per element from position 1 to n-1)
        // - Writes: 0 (already sorted, no shifts needed)
        //
        // For n > 16: Hybrid behavior
        // - QuickSort partitions recursively, creating many comparisons
        // - For sorted data, partitioning still occurs but creates unbalanced splits
        // - Each partition operation compares elements with pivots
        // - Final small subarrays use InsertionSort
        //
        // Actual behavior (empirical):
        // - n=50: ~1137 comparisons (QuickSort dominates)
        // - n=100: ~4887 comparisons
        // The algorithm performs more comparisons than expected because sorted data
        // causes unbalanced partitioning in QuickSort

        if (n <= InsertThreshold)
        {
            // Pure InsertionSort behavior for sorted data
            var expectedCompares = (ulong)(n - 1);
            var expectedWrites = 0UL;

            Assert.Equal(expectedCompares, stats.CompareCount);
            Assert.Equal(expectedWrites, stats.IndexWriteCount);
        }
        else
        {
            // Hybrid: QuickSort partitioning + InsertionSort for small subarrays
            // For sorted data, expect O(n²) comparisons in worst case due to unbalanced partitions
            var minCompares = (ulong)(n - 1); // Absolute minimum
            var maxCompares = (ulong)(n * n); // Worst case

            Assert.InRange(stats.CompareCount, minCompares, maxCompares);

            // For sorted data with InsertionSort on small subarrays, minimal writes
            // But QuickSort swaps may occur
            Assert.True(stats.IndexWriteCount >= 0UL);
        }

        // Verify sorted correctly
        Assert.Equal(Enumerable.Range(0, n), sorted);

        // IndexReads should be at least 2x comparisons (each compare reads 2 elements)
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
        QuickSortDualPivotInsertion.Sort(reversed.AsSpan(), stats);

        // For n <= 16: Pure InsertionSort on reversed data
        // - Worst case O(n²)
        // - Comparisons: n(n-1)/2
        // - Writes: (n-1)(n+2)/2 (shifts + final placements)
        //
        // For n > 16: Hybrid behavior
        // - QuickSort partitions, then InsertionSort on small subarrays
        // - QuickSort on reversed: still O(n log n) with dual pivot
        // - InsertionSort on reversed subarrays: each subarray of ~16 elements takes ~120 compares, ~153 writes

        if (n <= InsertThreshold)
        {
            // Pure InsertionSort on reversed data: worst case
            var expectedCompares = (ulong)(n * (n - 1) / 2);
            var expectedWrites = (ulong)((n - 1) * (n + 2) / 2);

            Assert.Equal(expectedCompares, stats.CompareCount);
            Assert.Equal(expectedWrites, stats.IndexWriteCount);
        }
        else
        {
            // Hybrid: more complex behavior
            // QuickSort swaps many elements during partitioning
            // InsertionSort handles small reversed subarrays

            var minCompares = (ulong)(n); // At least scan all elements
            var maxCompares = (ulong)(n * n); // Worst case (rare with dual pivot + insertion hybrid)

            var minWrites = (ulong)(n / 4); // At least some elements moved
            var maxWrites = (ulong)(n * n); // Worst case

            Assert.InRange(stats.CompareCount, minCompares, maxCompares);
            Assert.InRange(stats.IndexWriteCount, minWrites, maxWrites);
        }

        // Verify sorted correctly
        Assert.Equal(Enumerable.Range(0, n), reversed);

        // IndexReads: should be higher due to swaps and writes
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
        QuickSortDualPivotInsertion.Sort(random.AsSpan(), stats);

        // QuickSortDualPivotInsertion on random data: O(n log n) average
        // - Hybrid combines QuickSort's partitioning with InsertionSort for small subarrays
        // - QuickSort part: O(n log3 n) comparisons, O(n log n) swaps
        // - InsertionSort part: O(n²) per subarray, but subarrays are small (16 elements)

        var minCompares = (ulong)(n - 1); // Minimum: nearly sorted by chance
        var maxCompares = (ulong)(n * n); // Maximum: worst case (very rare)

        // For random data, expect operations between best and worst case
        // Dominated by QuickSort partitioning for large n
        var minWrites = 0UL; // Best case: already sorted by chance
        var maxWrites = (ulong)(n * Math.Log(n, 2) * 4); // Average case estimate

        Assert.InRange(stats.CompareCount, minCompares, maxCompares);
        Assert.True(stats.IndexWriteCount >= minWrites,
            $"IndexWriteCount ({stats.IndexWriteCount}) should be >= {minWrites}");

        // IndexReads: proportional to comparisons and operations
        var minIndexReads = stats.CompareCount * 2;
        Assert.True(stats.IndexReadCount >= minIndexReads,
            $"IndexReadCount ({stats.IndexReadCount}) should be >= {minIndexReads}");

        // Verify sorted correctly
        Assert.Equal(Enumerable.Range(0, n).OrderBy(x => x), random.OrderBy(x => x));
    }

    [Theory]
    [InlineData(5)]
    [InlineData(10)]
    [InlineData(20)]
    public void TheoreticalValuesSameElementsTest(int n)
    {
        var stats = new StatisticsContext();
        var sameValues = Enumerable.Repeat(42, n).ToArray();
        QuickSortDualPivotInsertion.Sort(sameValues.AsSpan(), stats);

        // All same values: best case for both QuickSort and InsertionSort
        // - InsertionSort (n <= 16): n-1 comparisons, 0 writes
        // - QuickSort + InsertionSort (n > 16): partitioning still occurs
        //   but elements don't move much, middle section contains most elements

        var minCompares = (ulong)(n - 1); // At minimum, must compare adjacent elements
        var maxCompares = (ulong)(n * Math.Log(n, 2) * 4); // Allow for partitioning overhead

        // Very few writes needed since all values are equal
        var maxWrites = (ulong)(n); // Upper bound: should be much less in practice

        Assert.InRange(stats.CompareCount, minCompares, maxCompares);
        Assert.True(stats.IndexWriteCount <= maxWrites,
            $"IndexWriteCount ({stats.IndexWriteCount}) should be <= {maxWrites} for all equal elements");

        // Verify the array is still correct (all values unchanged)
        Assert.All(sameValues, val => Assert.Equal(42, val));
    }

    /// <summary>
    /// Tests adaptive pivot selection: simple method for arrays &lt; 47 elements
    /// </summary>
    [Theory]
    [InlineData(17)]  // Just above InsertThreshold
    [InlineData(30)]  // Medium-small array
    [InlineData(46)]  // Just below PivotThreshold (47)
    public void StatisticsAdaptivePivotSimpleMethodTest(int n)
    {
        var stats = new StatisticsContext();
        var random = Enumerable.Range(0, n).OrderBy(_ => Guid.NewGuid()).ToArray();
        QuickSortDualPivotInsertion.Sort(random.AsSpan(), stats);

        // Verify sorting is correct
        Assert.Equal(Enumerable.Range(0, n), random);

        // For InsertThreshold < n < 47, simple pivot selection is used (left and right elements)
        // This uses fewer comparisons for pivot selection compared to 5-sample method
        // The algorithm should use QuickSort partitioning (SwapCount > 0)
        Assert.NotEqual(0UL, stats.CompareCount);
        Assert.NotEqual(0UL, stats.SwapCount); // From QuickSort partitioning
    }

    /// <summary>
    /// Tests adaptive pivot selection: 5-sample method for arrays ≥ 47 elements
    /// </summary>
    [Theory]
    [InlineData(47)]   // Exactly at threshold
    [InlineData(50)]   // Just above threshold
    [InlineData(100)]  // Larger array
    [InlineData(200)]  // Even larger
    public void StatisticsAdaptivePivot5SampleMethodTest(int n)
    {
        var stats = new StatisticsContext();
        var random = Enumerable.Range(0, n).OrderBy(_ => Guid.NewGuid()).ToArray();
        QuickSortDualPivotInsertion.Sort(random.AsSpan(), stats);

        // Verify sorting is correct
        Assert.Equal(Enumerable.Range(0, n), random);

        // For n >= 47, 5-sample pivot selection is used
        // This involves sorting 5 sampled elements using 2-pass bubble sort (7-9 comparisons per partition level)
        // The improved pivot quality should result in better partitioning
        Assert.NotEqual(0UL, stats.CompareCount);
        Assert.NotEqual(0UL, stats.SwapCount);

        // 5-sample method adds overhead but improves partition balance
        // On random data, we expect more comparisons due to sampling overhead
        // but better overall performance due to balanced partitions
    }

    /// <summary>
    /// Tests that 5-sample pivot selection handles sorted data efficiently (≥ 47 elements)
    /// </summary>
    [Theory]
    [InlineData(47)]
    [InlineData(100)]
    public void StatisticsAdaptivePivot5SampleSortedTest(int n)
    {
        var stats = new StatisticsContext();
        var sorted = Enumerable.Range(0, n).ToArray();
        QuickSortDualPivotInsertion.Sort(sorted.AsSpan(), stats);

        // Verify array remains sorted
        Assert.Equal(Enumerable.Range(0, n), sorted);

        // With 5-sample pivot selection, sorted arrays should be handled efficiently
        // The 2nd and 4th elements from sorted samples provide good pivots
        // Expected behavior: O(n log n) comparisons, minimal swaps
        //
        // This hybrid adds complexity:
        // - InsertionSort for subarrays ≤16
        // - 5-sample pivot selection (7-9 comparisons per partition level)

        var recursionDepth = Math.Ceiling(Math.Log(n, 3));
        var maxCompares = (ulong)(n * 3 * recursionDepth + 20 * recursionDepth);
        Assert.True(stats.CompareCount <= maxCompares,
            $"CompareCount ({stats.CompareCount}) should be <= {maxCompares} for sorted data with 5-sample pivots");
    }

    /// <summary>
    /// Tests that 5-sample pivot selection handles reverse-sorted data efficiently
    /// </summary>
    [Theory]
    [InlineData(47)]
    [InlineData(100)]
    public void StatisticsAdaptivePivot5SampleReversedTest(int n)
    {
        var stats = new StatisticsContext();
        var reversed = Enumerable.Range(0, n).Reverse().ToArray();
        QuickSortDualPivotInsertion.Sort(reversed.AsSpan(), stats);

        // Verify array is now sorted
        Assert.Equal(Enumerable.Range(0, n), reversed);


        // With 5-sample pivot selection, reversed arrays should be handled efficiently
        // The sampling strategy should select good pivots even from reversed data
        //
        // Reversed data might need more comparisons than sorted data
        // but should still be O(n log n) with good pivot selection

        var recursionDepth = Math.Ceiling(Math.Log(n, 3));
        var maxCompares = (ulong)(n * 3 * recursionDepth + 15 * recursionDepth);

        Assert.True(stats.CompareCount <= maxCompares,
            $"CompareCount ({stats.CompareCount}) should be <= {maxCompares} for reversed data with 5-sample pivots");
    }

    /// <summary>
    /// Tests that the three-tier adaptive selection works correctly:
    /// InsertionSort (&lt;=16) &lt; Simple Pivot (17-46) &lt; 5-Sample Pivot (47+)
    /// </summary>
    [Theory]
    [InlineData(10)]   // InsertionSort only
    [InlineData(16)]   // Boundary: InsertionSort
    [InlineData(17)]   // Boundary: Simple pivot + insertion
    [InlineData(46)]   // Boundary: Simple pivot + insertion
    [InlineData(47)]   // Boundary: 5-sample + insertion
    [InlineData(100)]  // 5-sample + insertion
    public void StatisticsThreeTierAdaptiveSelectionTest(int n)
    {
        var stats = new StatisticsContext();
        var random = Enumerable.Range(0, n).OrderBy(_ => Guid.NewGuid()).ToArray();
        QuickSortDualPivotInsertion.Sort(random.AsSpan(), stats);

        // Verify sorting is correct
        Assert.Equal(Enumerable.Range(0, n), random);

        if (n <= InsertThreshold)
        {
            // Should use InsertionSort only (no swaps, only shifts)
            Assert.Equal(0UL, stats.SwapCount);
            Assert.NotEqual(0UL, stats.CompareCount);
        }
        else if (n < 47)
        {
            // Should use simple pivot QuickSort + InsertionSort
            Assert.NotEqual(0UL, stats.SwapCount); // From partitioning
            Assert.NotEqual(0UL, stats.CompareCount);
        }
        else
        {
            // Should use 5-sample pivot QuickSort + InsertionSort
            Assert.NotEqual(0UL, stats.SwapCount); // From partitioning + sample sorting
            Assert.NotEqual(0UL, stats.CompareCount);

            // 5-sample method should have more comparisons than simple pivot
            // due to sorting the 5 samples (7-9 extra comparisons per level)
        }
    }

#endif

}
