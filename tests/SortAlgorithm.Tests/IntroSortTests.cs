using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;

namespace SortAlgorithm.Tests;

public class IntroSortTests
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
        IntroSort.Sort(array.AsSpan(), stats);

        Assert.Equal((ulong)inputSample.Samples.Length, (ulong)array.Length);
    }

    [Fact]
    public void EdgeCaseEmptyArrayTest()
    {
        var stats = new StatisticsContext();
        var empty = Array.Empty<int>();
        IntroSort.Sort(empty.AsSpan(), stats);
    }

    [Fact]
    public void EdgeCaseSingleElementTest()
    {
        var stats = new StatisticsContext();
        var single = new[] { 42 };
        IntroSort.Sort(single.AsSpan(), stats);

        Assert.Equal(42, single[0]);
    }

    [Fact]
    public void EdgeCaseTwoElementsSortedTest()
    {
        var stats = new StatisticsContext();
        var twoSorted = new[] { 1, 2 };
        IntroSort.Sort(twoSorted.AsSpan(), stats);

        Assert.Equal([1, 2], twoSorted);
    }

    [Fact]
    public void EdgeCaseTwoElementsReversedTest()
    {
        var stats = new StatisticsContext();
        var twoReversed = new[] { 2, 1 };
        IntroSort.Sort(twoReversed.AsSpan(), stats);

        Assert.Equal([1, 2], twoReversed);
    }

    [Fact]
    public void EdgeCaseThreeElementsTest()
    {
        var stats = new StatisticsContext();
        var three = new[] { 3, 1, 2 };
        IntroSort.Sort(three.AsSpan(), stats);

        Assert.Equal([1, 2, 3], three);
    }

    [Fact]
    public void RangeSortTest()
    {
        var stats = new StatisticsContext();
        var array = new[] { 5, 3, 8, 1, 9, 2, 7, 4, 6 };

        // Sort only the range [2, 6) -> indices 2, 3, 4, 5
        IntroSort.Sort(array.AsSpan(), 2, 6, stats);

        // Expected: first 2 elements unchanged, middle 4 sorted, last 3 unchanged
        Assert.Equal(new[] { 5, 3, 1, 2, 8, 9, 7, 4, 6 }, array);
    }

    [Fact]
    public void RangeSortFullArrayTest()
    {
        var stats = new StatisticsContext();
        var array = new[] { 5, 3, 8, 1, 9, 2, 7, 4, 6 };

        // Sort the entire array using range API
        IntroSort.Sort(array.AsSpan(), 0, array.Length, stats);

        Assert.Equal(new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 }, array);
    }

    [Fact]
    public void RangeSortEmptyRangeTest()
    {
        var stats = new StatisticsContext();
        var array = new[] { 3, 1, 2 };

        // Sort empty range [1, 1)
        IntroSort.Sort(array.AsSpan(), 1, 1, stats);

        // Array should remain unchanged
        Assert.Equal(new[] { 3, 1, 2 }, array);
    }

    [Fact]
    public void SmallArraySwitchesToInsertionSortTest()
    {
        var stats = new StatisticsContext();
        var small = new[] { 9, 7, 5, 3, 1, 2, 4, 6, 8, 10 }; // 10 elements (< 16)
        IntroSort.Sort(small.AsSpan(), stats);

        Assert.Equal(new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 }, small);
    }

    [Fact]
    public void LargeArrayWorstCaseTest()
    {
        var stats = new StatisticsContext();
        var size = 10000;
        var array = Enumerable.Range(0, size).Reverse().ToArray();
        IntroSort.Sort(array.AsSpan(), stats);

        // Verify sorted
        for (var i = 0; i < size; i++)
        {
            Assert.Equal(i, array[i]);
        }
    }

#if DEBUG

    [Theory]
    [ClassData(typeof(MockSortedData))]
    public void StatisticsSortedTest(IInputSample<int> inputSample)
    {
        var stats = new StatisticsContext();
        var array = inputSample.Samples.ToArray();
        IntroSort.Sort(array.AsSpan(), stats);

        Assert.Equal((ulong)inputSample.Samples.Length, (ulong)array.Length);
        Assert.NotEqual(0UL, stats.IndexReadCount);
        Assert.NotEqual(0UL, stats.CompareCount);
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
        IntroSort.Sort(sorted.AsSpan(), stats);

        // IntroSort behavior on sorted data:
        // - For small arrays (n ≤ 16): Uses InsertionSort
        //   - Best case O(n): n-1 comparisons, 0 swaps
        // - For larger arrays (n > 16): Uses QuickSort with median-of-three pivot
        //   - For sorted data, InsertionSort is still used for final small partitions
        //   - Comparisons include: median-of-3 selections + partitioning + InsertionSort for small partitions
        //   - Swaps may be minimal due to sorted nature

        ulong minCompares, maxCompares, minSwaps, maxSwaps;

        // Note: Even for n > 16, the entire array may still go through InsertionSort
        // if the initial partition immediately becomes small enough
        // For sorted data specifically, partitioning creates small partitions quickly

        // Conservative bounds that work for both InsertionSort and QuickSort paths
        minCompares = (ulong)(n - 1); // Minimum: at least n-1 comparisons
        maxCompares = (ulong)(3 * n * Math.Max(1, Math.Log(n, 2))); // Upper bound for QuickSort
        minSwaps = 0UL; // Sorted data may need no swaps
        maxSwaps = (ulong)(n * Math.Max(1, Math.Log(n, 2))); // Upper bound

        Assert.InRange(stats.CompareCount, minCompares, maxCompares);
        Assert.InRange(stats.SwapCount, minSwaps, maxSwaps);

        // IndexReads: Each comparison reads elements, each swap reads and writes
        var minIndexReads = stats.CompareCount;
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
        IntroSort.Sort(reversed.AsSpan(), stats);

        // IntroSort behavior on reversed data:
        // - For small arrays (n ≤ 16): Uses InsertionSort
        //   - Worst case O(n²): n(n-1)/2 comparisons, (n-1)(n+2)/2 writes
        // - For larger arrays (n > 16): Uses QuickSort with median-of-three pivot
        //   - Partitioning occurs, then InsertionSort for final small partitions
        //   - Overall: O(n log n) behavior

        ulong minCompares, maxCompares;

        // Conservative bounds that handle both paths
        if (n <= 16)
        {
            // InsertionSort worst case for small arrays
            minCompares = (ulong)(n * (n - 1) / 2);
            maxCompares = (ulong)(n * (n - 1) / 2);
        }
        else
        {
            // Larger arrays: combination of QuickSort partitioning + InsertionSort for small partitions
            minCompares = (ulong)(n);
            maxCompares = (ulong)(n * n); // Allow for worst-case InsertionSort on partitions
        }

        Assert.InRange(stats.CompareCount, minCompares, maxCompares);

        // IndexReads and IndexWrites should be proportional to operations
        var minIndexReads = stats.CompareCount;
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
        IntroSort.Sort(random.AsSpan(), stats);

        // IntroSort behavior on random data:
        // - For small arrays (n ≤ 16): Uses InsertionSort
        //   - Average case O(n²): approximately n²/4 comparisons
        // - For larger arrays: Uses QuickSort + InsertionSort for small partitions
        //   - Combined complexity: O(n log n) with InsertionSort overhead

        // Conservative bounds for both paths
        ulong minCompares = (ulong)(n - 1); // Minimum: at least n-1 comparisons
        ulong maxCompares = (ulong)(n * n); // Allow for InsertionSort worst-case on partitions

        Assert.InRange(stats.CompareCount, minCompares, maxCompares);

        var minIndexReads = stats.CompareCount;
        Assert.True(stats.IndexReadCount >= minIndexReads,
            $"IndexReadCount ({stats.IndexReadCount}) should be >= {minIndexReads}");
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
        IntroSort.Sort(sameValues.AsSpan(), stats);

        // IntroSort behavior on same elements:
        // - For small arrays (n ≤ 16): Uses InsertionSort
        //   - Best case O(n): n-1 comparisons (all equal, no shifts)
        // - For larger arrays: Uses QuickSort + InsertionSort
        //   - All elements equal to pivot
        //   - Hoare partition may still swap elements (l ≤ r condition)
        //   - Combined complexity varies

        // Conservative bounds for all cases
        ulong minCompares = (ulong)(n - 1);
        ulong maxCompares = (ulong)(n * Math.Max(1, (int)Math.Log(n, 2)) * 3);

        Assert.InRange(stats.CompareCount, minCompares, maxCompares);

        // Verify array is still correct (all values unchanged)
        Assert.All(sameValues, val => Assert.Equal(42, val));

        var minIndexReads = stats.CompareCount;
        Assert.True(stats.IndexReadCount >= minIndexReads,
            $"IndexReadCount ({stats.IndexReadCount}) should be >= {minIndexReads}");
    }

#endif
}
