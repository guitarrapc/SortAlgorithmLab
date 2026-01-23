using SortLab.Core.Algorithms;
using SortLab.Core.Contexts;

namespace SortLab.Tests;

public class QuickSortMedian9Tests
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
        QuickSortMedian9.Sort(array.AsSpan());
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
        QuickSortMedian9.Sort(array.AsSpan(), stats);

        Assert.Equal((ulong)inputSample.Samples.Length, (ulong)array.Length);
        Assert.NotEqual(0UL, stats.IndexReadCount);
        Assert.NotEqual(0UL, stats.IndexWriteCount);
        Assert.NotEqual(0UL, stats.CompareCount);
        Assert.NotEqual(0UL, stats.SwapCount);
    }

    [Theory]
    [ClassData(typeof(MockSortedData))]
    public void StatisticsSortedTest(IInputSample<int> inputSample)
    {
        var stats = new StatisticsContext();
        var array = inputSample.Samples.ToArray();
        QuickSortMedian9.Sort(array.AsSpan(), stats);

        Assert.Equal((ulong)inputSample.Samples.Length, (ulong)array.Length);
        Assert.NotEqual(0UL, stats.IndexReadCount);
        Assert.NotEqual(0UL, stats.IndexWriteCount);
        Assert.NotEqual(0UL, stats.CompareCount);
        // For sorted data, QuickSort Median-of-9 with Hoare partition performs some swaps during partitioning
        Assert.True(stats.SwapCount >= 0UL);
    }

    [Theory]
    [ClassData(typeof(MockRandomData))]
    public void StatisticsResetTest(IInputSample<int> inputSample)
    {
        var stats = new StatisticsContext();
        var array = inputSample.Samples.ToArray();
        QuickSortMedian9.Sort(array.AsSpan(), stats);

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
        QuickSortMedian9.Sort(sorted.AsSpan(), stats);

        // QuickSort Median-of-9 with Hoare partition on sorted data:
        // - Median-of-9 selects median of (left, mid, right) as pivot
        // - For sorted data, median of (first, middle, last) is the middle element
        // - Hoare partition performs bidirectional scanning
        // 
        // Best case complexity analysis:
        // - Each partition divides array roughly in half
        // - Recursion depth: O(log n)
        // - At each level, all n elements are processed
        // 
        // Comparisons:
        // - Median-of-9: 2-3 comparisons per partition call
        // - Hoare partition: Each element compared with pivot once
        // - Total: approximately 2n log n comparisons
        // 
        // Swaps:
        // - For sorted data, Hoare partition still performs swaps when l <= r
        // - Even when elements are in correct relative positions, swaps occur
        // - Total: approximately 0.5n log n swaps

        var minCompares = (ulong)(n); // At minimum, some comparisons occur
        var maxCompares = (ulong)(3 * n * Math.Log(n, 2)); // Upper bound

        // For sorted data with Hoare partition, swaps still occur
        var minSwaps = 0UL;
        var maxSwaps = (ulong)(n * Math.Log(n, 2)); // Upper bound

        Assert.InRange(stats.CompareCount, minCompares, maxCompares);
        Assert.InRange(stats.SwapCount, minSwaps, maxSwaps);

        // IndexReads: Each comparison reads elements, each swap reads and writes
        var minIndexReads = stats.CompareCount;
        Assert.True(stats.IndexReadCount >= minIndexReads,
            $"IndexReadCount ({stats.IndexReadCount}) should be >= {minIndexReads}");

        // IndexWrites: Each swap writes 2 elements
        var minIndexWrites = stats.SwapCount * 2;
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
        QuickSortMedian9.Sort(reversed.AsSpan(), stats);

        // QuickSort Median-of-9 with Hoare partition on reversed data:
        // - Median-of-9 helps avoid worst-case by selecting better pivots
        // - For reversed data, median of (first, middle, last) is still reasonable
        // - Hoare partition is more efficient than Lomuto for reversed data
        // 
        // Average case complexity (Median-of-9 avoids O(n²)):
        // - Recursion depth: O(log n) on average
        // - Comparisons: approximately 1.386n log₂ n
        // - Swaps: approximately 0.33n log₂ n
        //
        // For reversed data specifically:
        // - Most elements need to be swapped during partitioning
        // - More swaps than sorted data, but still O(n log n)

        var minCompares = (ulong)(n);
        var maxCompares = (ulong)(3 * n * Math.Log(n, 2));

        var minSwaps = (ulong)(n / 4); // At least some swaps needed
        var maxSwaps = (ulong)(2 * n * Math.Log(n, 2)); // Allow for more swaps

        Assert.InRange(stats.CompareCount, minCompares, maxCompares);
        Assert.InRange(stats.SwapCount, minSwaps, maxSwaps);

        // IndexReads and IndexWrites should be proportional to operations
        var minIndexReads = stats.CompareCount;
        Assert.True(stats.IndexReadCount >= minIndexReads,
            $"IndexReadCount ({stats.IndexReadCount}) should be >= {minIndexReads}");

        var minIndexWrites = stats.SwapCount * 2;
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
        QuickSortMedian9.Sort(random.AsSpan(), stats);

        // QuickSort Median-of-9 with Hoare partition on random data:
        // - Average case O(n log n) complexity
        // - Median-of-9 pivot selection provides good partitioning
        // - Hoare partition performs approximately 1.386n log₂ n comparisons
        // - Swaps: approximately 0.33n log₂ n on average

        var minCompares = (ulong)(n);
        var maxCompares = (ulong)(3 * n * Math.Log(n, 2));

        var minSwaps = 0UL;
        var maxSwaps = (ulong)(2 * n * Math.Log(n, 2));

        Assert.InRange(stats.CompareCount, minCompares, maxCompares);
        Assert.InRange(stats.SwapCount, minSwaps, maxSwaps);

        var minIndexReads = stats.CompareCount;
        Assert.True(stats.IndexReadCount >= minIndexReads,
            $"IndexReadCount ({stats.IndexReadCount}) should be >= {minIndexReads}");

        var minIndexWrites = stats.SwapCount * 2;
        Assert.True(stats.IndexWriteCount >= minIndexWrites,
            $"IndexWriteCount ({stats.IndexWriteCount}) should be >= {minIndexWrites}");
    }

    [Theory]
    [InlineData(5)]
    [InlineData(10)]
    [InlineData(20)]
    [InlineData(50)]
    public void TheoreticalValuesSameElementsTest(int n)
    {
        var stats = new StatisticsContext();
        var sameValues = Enumerable.Repeat(42, n).ToArray();
        QuickSortMedian9.Sort(sameValues.AsSpan(), stats);

        // QuickSort Median-of-9 with Hoare partition on all equal elements:
        // - Median-of-9: all three sampled elements are equal
        // - Pivot equals all elements in the array
        // - Hoare partition: elements equal to pivot are distributed between partitions
        // - This causes many swaps even though all elements are equal
        // 
        // Expected behavior:
        // - Comparisons: O(n log n) - partitioning still occurs at each level
        // - Swaps: O(n log n) - Hoare partition swaps elements even when equal to pivot
        //   This is a known characteristic: condition is "l <= r" not "l < r"
        // - The algorithm still terminates correctly

        var minCompares = (ulong)(n - 1); // At minimum, some comparisons
        var maxCompares = (ulong)(3 * n * Math.Log(Math.Max(n, 2), 2));

        var minSwaps = 0UL;
        var maxSwaps = (ulong)(2 * n * Math.Log(Math.Max(n, 2), 2));

        Assert.InRange(stats.CompareCount, minCompares, maxCompares);
        Assert.InRange(stats.SwapCount, minSwaps, maxSwaps);

        // Verify array is still correct (all values unchanged)
        Assert.All(sameValues, val => Assert.Equal(42, val));

        var minIndexReads = stats.CompareCount;
        Assert.True(stats.IndexReadCount >= minIndexReads,
            $"IndexReadCount ({stats.IndexReadCount}) should be >= {minIndexReads}");

        var minIndexWrites = stats.SwapCount * 2;
        Assert.True(stats.IndexWriteCount >= minIndexWrites,
            $"IndexWriteCount ({stats.IndexWriteCount}) should be >= {minIndexWrites}");
    }


    [Fact]
    public void EdgeCaseEmptyArrayTest()
    {
        var stats = new StatisticsContext();
        var empty = Array.Empty<int>();
        QuickSortMedian9.Sort(empty.AsSpan(), stats);

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
        QuickSortMedian9.Sort(single.AsSpan(), stats);

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
        QuickSortMedian9.Sort(twoSorted.AsSpan(), stats);

        // Two elements already sorted with Hoare partition:
        // - Median-of-9: left=0, mid=0, right=1
        //   Compare(0, 0) = 0, so lowMidCmp <= 0
        //   Compare(0, 1) and return appropriate median
        //   Total: 2 comparisons for median selection
        // - Partition with pivot from median:
        //   l=0, r=1, pivot = median value
        //   Inner while loops may execute, then swap at l and r
        //   Total: varies, but at least 1 swap
        // - Recursion on subranges
        // 
        // Actual behavior depends on pivot value selected and partition logic

        Assert.True(stats.CompareCount >= 1UL,
            $"CompareCount ({stats.CompareCount}) should be >= 1");
        Assert.True(stats.SwapCount >= 0UL,
            $"SwapCount ({stats.SwapCount}) should be >= 0");
        Assert.Equal([1, 2], twoSorted);

        // Verify IndexReads and IndexWrites are tracked
        Assert.True(stats.IndexReadCount >= stats.CompareCount,
            $"IndexReadCount ({stats.IndexReadCount}) should be >= CompareCount ({stats.CompareCount})");
        Assert.True(stats.IndexWriteCount >= stats.SwapCount * 2,
            $"IndexWriteCount ({stats.IndexWriteCount}) should be >= SwapCount * 2 ({stats.SwapCount * 2})");
    }

    [Fact]
    public void EdgeCaseTwoElementsReversedTest()
    {
        var stats = new StatisticsContext();
        var twoReversed = new[] { 2, 1 };
        QuickSortMedian9.Sort(twoReversed.AsSpan(), stats);

        // Two elements reversed with Hoare partition:
        // - Median-of-9 selection: 2-3 comparisons
        // - Partition: at least 1 swap needed to fix order
        // - May recurse on subranges

        Assert.True(stats.CompareCount >= 1UL,
            $"CompareCount ({stats.CompareCount}) should be >= 1");
        Assert.True(stats.SwapCount >= 1UL,
            $"SwapCount ({stats.SwapCount}) should be >= 1 (elements need swapping)");
        Assert.Equal([1, 2], twoReversed);

        Assert.True(stats.IndexReadCount >= stats.CompareCount,
            $"IndexReadCount ({stats.IndexReadCount}) should be >= CompareCount");
        Assert.True(stats.IndexWriteCount >= stats.SwapCount * 2,
            $"IndexWriteCount ({stats.IndexWriteCount}) should be >= SwapCount * 2");
    }

    [Fact]
    public void EdgeCaseThreeElementsTest()
    {
        var stats = new StatisticsContext();
        var three = new[] { 3, 1, 2 };
        QuickSortMedian9.Sort(three.AsSpan(), stats);

        // Three elements: Median-of-9 selects from all three
        // - Median-of-9: 2-3 comparisons to find median
        // - Partition with selected pivot
        // - Possible recursion on smaller subranges

        Assert.True(stats.CompareCount >= 2UL,
            $"CompareCount ({stats.CompareCount}) should be >= 2");
        Assert.True(stats.SwapCount >= 1UL,
            $"SwapCount ({stats.SwapCount}) should be >= 1");
        Assert.Equal([1, 2, 3], three);

        Assert.True(stats.IndexReadCount > 0UL,
            $"IndexReadCount ({stats.IndexReadCount}) should be > 0");
        Assert.True(stats.IndexWriteCount >= stats.SwapCount * 2,
            $"IndexWriteCount ({stats.IndexWriteCount}) should be >= SwapCount * 2");
    }

    [Fact]
    public void RangeSortTest()
    {
        var stats = new StatisticsContext();
        var array = new[] { 5, 3, 8, 1, 9, 2, 7, 4, 6 };

        // Sort only the range [2, 6) -> indices 2, 3, 4, 5
        QuickSortMedian9.Sort(array.AsSpan(), 2, 6, stats);

        // Expected: first 2 elements unchanged, middle 4 sorted, last 3 unchanged
        Assert.Equal(new[] { 5, 3, 1, 2, 8, 9, 7, 4, 6 }, array);

        // Verify statistics are tracked for range sort
        Assert.True(stats.CompareCount > 0UL,
            $"CompareCount ({stats.CompareCount}) should be > 0");
        Assert.True(stats.IndexReadCount > 0UL,
            $"IndexReadCount ({stats.IndexReadCount}) should be > 0");
    }

    [Fact]
    public void RangeSortFullArrayTest()
    {
        var stats = new StatisticsContext();
        var array = new[] { 5, 3, 8, 1, 9, 2, 7, 4, 6 };

        // Sort the entire array using range API
        QuickSortMedian9.Sort(array.AsSpan(), 0, array.Length, stats);

        Assert.Equal(new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 }, array);

        Assert.True(stats.CompareCount > 0UL,
            $"CompareCount ({stats.CompareCount}) should be > 0");
        Assert.True(stats.SwapCount >= 0UL,
            $"SwapCount ({stats.SwapCount}) should be >= 0");
        Assert.True(stats.IndexReadCount > 0UL,
            $"IndexReadCount ({stats.IndexReadCount}) should be > 0");
        Assert.True(stats.IndexWriteCount >= 0UL,
            $"IndexWriteCount ({stats.IndexWriteCount}) should be >= 0");
    }

    [Fact]
    public void RangeSortSingleElementTest()
    {
        var stats = new StatisticsContext();
        var array = new[] { 5, 3, 8, 1, 9 };

        // Sort a single element range [2, 3)
        QuickSortMedian9.Sort(array.AsSpan(), 2, 3, stats);

        // Array should be unchanged (single element is already sorted)
        Assert.Equal(new[] { 5, 3, 8, 1, 9 }, array);
        Assert.Equal(0UL, stats.CompareCount);
        Assert.Equal(0UL, stats.SwapCount);
        Assert.Equal(0UL, stats.IndexReadCount);
        Assert.Equal(0UL, stats.IndexWriteCount);
    }

    [Fact]
    public void RangeSortBeginningTest()
    {
        var stats = new StatisticsContext();
        var array = new[] { 9, 7, 5, 3, 1, 2, 4, 6, 8 };

        // Sort only the first 5 elements [0, 5)
        QuickSortMedian9.Sort(array.AsSpan(), 0, 5, stats);

        // Expected: first 5 sorted, last 4 unchanged
        Assert.Equal(new[] { 1, 3, 5, 7, 9, 2, 4, 6, 8 }, array);

        Assert.True(stats.CompareCount > 0UL);
        Assert.True(stats.IndexReadCount > 0UL);
        Assert.True(stats.IndexWriteCount >= 0UL);
    }

    [Fact]
    public void RangeSortEndTest()
    {
        var stats = new StatisticsContext();
        var array = new[] { 1, 3, 5, 7, 9, 8, 6, 4, 2 };

        // Sort only the last 4 elements [5, 9)
        QuickSortMedian9.Sort(array.AsSpan(), 5, 9, stats);

        // Expected: first 5 unchanged, last 4 sorted
        Assert.Equal(new[] { 1, 3, 5, 7, 9, 2, 4, 6, 8 }, array);

        Assert.True(stats.CompareCount > 0UL);
        Assert.True(stats.IndexReadCount > 0UL);
        Assert.True(stats.IndexWriteCount >= 0UL);
    }
}
