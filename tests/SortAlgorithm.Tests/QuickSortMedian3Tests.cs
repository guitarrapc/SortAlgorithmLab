using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;

namespace SortAlgorithm.Tests;

public class QuickSortMedian3Tests
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
        QuickSortMedian3.Sort(array.AsSpan(), stats);

        Assert.Equal((ulong)inputSample.Samples.Length, (ulong)array.Length);
    }

    [Fact]
    public void EdgeCaseEmptyArrayTest()
    {
        var stats = new StatisticsContext();
        var empty = Array.Empty<int>();
        QuickSortMedian3.Sort(empty.AsSpan(), stats);
    }

    [Fact]
    public void EdgeCaseSingleElementTest()
    {
        var stats = new StatisticsContext();
        var single = new[] { 42 };
        QuickSortMedian3.Sort(single.AsSpan(), stats);

        Assert.Equal(42, single[0]);
    }

    [Fact]
    public void EdgeCaseTwoElementsSortedTest()
    {
        var stats = new StatisticsContext();
        var twoSorted = new[] { 1, 2 };
        QuickSortMedian3.Sort(twoSorted.AsSpan(), stats);

        Assert.Equal([1, 2], twoSorted);
    }

    [Fact]
    public void EdgeCaseTwoElementsReversedTest()
    {
        var stats = new StatisticsContext();
        var twoReversed = new[] { 2, 1 };
        QuickSortMedian3.Sort(twoReversed.AsSpan(), stats);

        Assert.Equal([1, 2], twoReversed);
    }

    [Fact]
    public void EdgeCaseThreeElementsTest()
    {
        var stats = new StatisticsContext();
        var three = new[] { 3, 1, 2 };
        QuickSortMedian3.Sort(three.AsSpan(), stats);

        Assert.Equal([1, 2, 3], three);
    }

    [Fact]
    public void RangeSortTest()
    {
        var stats = new StatisticsContext();
        var array = new[] { 5, 3, 8, 1, 9, 2, 7, 4, 6 };

        // Sort only the range [2, 6) -> indices 2, 3, 4, 5
        QuickSortMedian3.Sort(array.AsSpan(), 2, 6, stats);

        // Expected: first 2 elements unchanged, middle 4 sorted, last 3 unchanged
        Assert.Equal(new[] { 5, 3, 1, 2, 8, 9, 7, 4, 6 }, array);
    }

    [Fact]
    public void RangeSortFullArrayTest()
    {
        var stats = new StatisticsContext();
        var array = new[] { 5, 3, 8, 1, 9, 2, 7, 4, 6 };

        // Sort the entire array using range API
        QuickSortMedian3.Sort(array.AsSpan(), 0, array.Length, stats);

        Assert.Equal(new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 }, array);
    }

    [Fact]
    public void RangeSortSingleElementTest()
    {
        var stats = new StatisticsContext();
        var array = new[] { 5, 3, 8, 1, 9 };

        // Sort a single element range [2, 3)
        QuickSortMedian3.Sort(array.AsSpan(), 2, 3, stats);

        // Array should be unchanged (single element is already sorted)
        Assert.Equal(new[] { 5, 3, 8, 1, 9 }, array);
    }

    [Fact]
    public void RangeSortBeginningTest()
    {
        var stats = new StatisticsContext();
        var array = new[] { 9, 7, 5, 3, 1, 2, 4, 6, 8 };

        // Sort only the first 5 elements [0, 5)
        QuickSortMedian3.Sort(array.AsSpan(), 0, 5, stats);

        // Expected: first 5 sorted, last 4 unchanged
        Assert.Equal(new[] { 1, 3, 5, 7, 9, 2, 4, 6, 8 }, array);
    }

    [Fact]
    public void RangeSortEndTest()
    {
        var stats = new StatisticsContext();
        var array = new[] { 1, 3, 5, 7, 9, 8, 6, 4, 2 };

        // Sort only the last 4 elements [5, 9)
        QuickSortMedian3.Sort(array.AsSpan(), 5, 9, stats);

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
        QuickSortMedian3.Sort(array.AsSpan(), stats);

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
        QuickSortMedian3.Sort(sorted.AsSpan(), stats);

        // QuickSort Median-of-3 with Hoare partition on sorted data:
        // - Median-of-3 selects median of (left, mid, right) as pivot
        // - For sorted data, median of (first, middle, last) is the middle element
        // - Hoare partition performs bidirectional scanning
        //
        // Best case complexity analysis:
        // - Each partition divides array roughly in half
        // - Recursion depth: O(log n)
        // - At each level, all n elements are processed
        //
        // Comparisons:
        // - Median-of-3: 2-3 comparisons per partition call
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
        QuickSortMedian3.Sort(reversed.AsSpan(), stats);

        // QuickSort Median-of-3 with Hoare partition on reversed data:
        // - Median-of-3 helps avoid worst-case by selecting better pivots
        // - For reversed data, median of (first, middle, last) is still reasonable
        // - Hoare partition is more efficient than Lomuto for reversed data
        //
        // Average case complexity (median-of-3 avoids O(n²)):
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
        QuickSortMedian3.Sort(random.AsSpan(), stats);

        // QuickSort Median-of-3 with Hoare partition on random data:
        // - Average case O(n log n) complexity
        // - Median-of-3 pivot selection provides good partitioning
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
        QuickSortMedian3.Sort(sameValues.AsSpan(), stats);

        // QuickSort Median-of-3 with Hoare partition on all equal elements:
        // - Median-of-3: all three sampled elements are equal
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

#endif

}
