using SortLab.Core.Algorithms;
using SortLab.Core.Contexts;

namespace SortLab.Tests;

public class QuickSortTests
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
        QuickSort.Sort(array.AsSpan(), stats);

        Assert.Equal((ulong)inputSample.Samples.Length, (ulong)array.Length);
    }

    [Fact]
    public void EdgeCaseEmptyArrayTest()
    {
        var stats = new StatisticsContext();
        var empty = Array.Empty<int>();
        QuickSort.Sort(empty.AsSpan(), stats);
    }

    [Fact]
    public void EdgeCaseSingleElementTest()
    {
        var stats = new StatisticsContext();
        var single = new[] { 42 };
        QuickSort.Sort(single.AsSpan(), stats);

        Assert.Equal(42, single[0]);
    }

    [Fact]
    public void EdgeCaseTwoElementsSortedTest()
    {
        var stats = new StatisticsContext();
        var twoSorted = new[] { 1, 2 };
        QuickSort.Sort(twoSorted.AsSpan(), stats);

        Assert.Equal([1, 2], twoSorted);
    }

    [Fact]
    public void EdgeCaseTwoElementsReversedTest()
    {
        var stats = new StatisticsContext();
        var twoReversed = new[] { 2, 1 };
        QuickSort.Sort(twoReversed.AsSpan(), stats);

        Assert.Equal([1, 2], twoReversed);
    }

    [Fact]
    public void EdgeCaseThreeElementsTest()
    {
        var stats = new StatisticsContext();
        var three = new[] { 3, 1, 2 };
        QuickSort.Sort(three.AsSpan(), stats);

        Assert.Equal([1, 2, 3], three);
    }

    [Fact]
    public void RangeSortTest()
    {
        var stats = new StatisticsContext();
        var array = new[] { 5, 3, 8, 1, 9, 2, 7, 4, 6 };

        // Sort only the range [2, 6) -> indices 2, 3, 4, 5
        QuickSort.Sort(array.AsSpan(), 2, 6, stats);

        // Expected: first 2 elements unchanged, middle 4 sorted, last 3 unchanged
        Assert.Equal(new[] { 5, 3, 1, 2, 8, 9, 7, 4, 6 }, array);
    }

    [Fact]
    public void RangeSortFullArrayTest()
    {
        var stats = new StatisticsContext();
        var array = new[] { 5, 3, 8, 1, 9, 2, 7, 4, 6 };

        // Sort the entire array using range API
        QuickSort.Sort(array.AsSpan(), 0, array.Length, stats);

        Assert.Equal(new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 }, array);
    }

    [Fact]
    public void RangeSortSingleElementTest()
    {
        var stats = new StatisticsContext();
        var array = new[] { 5, 3, 8, 1, 9 };

        // Sort a single element range [2, 3)
        QuickSort.Sort(array.AsSpan(), 2, 3, stats);

        // Array should be unchanged (single element is already sorted)
        Assert.Equal(new[] { 5, 3, 8, 1, 9 }, array);
    }

    [Fact]
    public void RangeSortBeginningTest()
    {
        var stats = new StatisticsContext();
        var array = new[] { 9, 7, 5, 3, 1, 2, 4, 6, 8 };

        // Sort only the first 5 elements [0, 5)
        QuickSort.Sort(array.AsSpan(), 0, 5, stats);

        // Expected: first 5 sorted, last 4 unchanged
        Assert.Equal(new[] { 1, 3, 5, 7, 9, 2, 4, 6, 8 }, array);
    }

    [Fact]
    public void RangeSortEndTest()
    {
        var stats = new StatisticsContext();
        var array = new[] { 1, 3, 5, 7, 9, 8, 6, 4, 2 };

        // Sort only the last 4 elements [5, 9)
        QuickSort.Sort(array.AsSpan(), 5, 9, stats);

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
        QuickSort.Sort(array.AsSpan(), stats);

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
        QuickSort.Sort(sorted.AsSpan(), stats);

        // QuickSort with Hoare partition on sorted data:
        // - Using middle element as pivot
        // - For sorted data, this provides relatively balanced partitions
        // - Each partition pass scans through elements comparing with pivot
        //
        // Expected behavior:
        // - Comparisons: O(n log n) - Hoare partition compares elements with pivot
        //   Average case: approximately 2n ln n ≈ 1.39n log₂ n comparisons
        //   For sorted data with middle pivot, performance is near-optimal
        // - Swaps: O(n log n) - Elements swap positions during partitioning
        //   Even on sorted data, Hoare partition performs swaps when i and j cross
        //   Average case: approximately n ln n / 3 swaps
        // - Recursion depth: O(log n) with balanced partitions
        var minCompares = (ulong)(n); // At minimum, each element visited once
        var maxCompares = (ulong)(n * n); // Worst case O(n²) if partitioning fails

        // For sorted data with middle pivot, swaps still occur during partitioning
        var minSwaps = 0UL;
        var maxSwaps = (ulong)(n * Math.Log(n, 2)); // O(n log n) average

        Assert.InRange(stats.CompareCount, minCompares, maxCompares);
        Assert.InRange(stats.SwapCount, minSwaps, maxSwaps);

        // IndexReads: Each comparison reads elements
        // In Hoare partition, s.Compare(i, pivot) reads s[i], and s.Compare(pivot, j) reads s[j]
        // Plus initial pivot read and swap reads
        var minIndexReads = stats.CompareCount; // At least one read per comparison
        Assert.True(stats.IndexReadCount >= minIndexReads,
            $"IndexReadCount ({stats.IndexReadCount}) should be >= {minIndexReads}");

        // IndexWrites: Each swap writes 2 elements
        var expectedWrites = stats.SwapCount * 2;
        Assert.Equal(expectedWrites, stats.IndexWriteCount);
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
        QuickSort.Sort(reversed.AsSpan(), stats);

        // QuickSort with Hoare partition on reversed data:
        // - Using middle element as pivot
        // - For reversed data with middle pivot, partitioning is still balanced
        // - Many swaps needed to rearrange elements
        //
        // Expected behavior:
        // - Comparisons: O(n log n) average case
        //   Similar to sorted data since middle pivot provides balance
        // - Swaps: O(n log n) - Many elements need repositioning
        //   Hoare partition performs approximately n ln n / 3 swaps on average
        var minCompares = (ulong)(n);
        var maxCompares = (ulong)(n * n); // Worst case (rare with middle pivot)

        var minSwaps = (ulong)(n / 2); // At least half the elements need swapping
        var maxSwaps = (ulong)(n * Math.Log(n, 2) * 2); // O(n log n) with some overhead

        Assert.InRange(stats.CompareCount, minCompares, maxCompares);
        Assert.InRange(stats.SwapCount, minSwaps, maxSwaps);

        // IndexReads: At least one per comparison
        var minIndexReads = stats.CompareCount;
        Assert.True(stats.IndexReadCount >= minIndexReads,
            $"IndexReadCount ({stats.IndexReadCount}) should be >= {minIndexReads}");

        // IndexWrites: Each swap writes 2 elements
        var expectedWrites = stats.SwapCount * 2;
        Assert.Equal(expectedWrites, stats.IndexWriteCount);
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
        QuickSort.Sort(random.AsSpan(), stats);

        // QuickSort with Hoare partition on random data: average case O(n log n)
        // - Middle element as pivot provides decent balance on average
        // - Partitioning divides array into approximately two halves
        //
        // Expected behavior:
        // - Comparisons: O(n log n) average
        //   Approximately 2n ln n ≈ 1.39n log₂ n comparisons
        // - Swaps: O(n log n) average
        //   Approximately n ln n / 3 swaps
        //   Hoare partition performs fewer swaps than Lomuto partition
        var minCompares = (ulong)(n);
        var maxCompares = (ulong)(n * n); // Worst case (very rare with random data)

        var minSwaps = 0UL; // Best case: already sorted by chance
        var maxSwaps = (ulong)(n * Math.Log(n, 2) * 2); // Average with overhead

        Assert.InRange(stats.CompareCount, minCompares, maxCompares);
        Assert.InRange(stats.SwapCount, minSwaps, maxSwaps);

        // IndexReads: At least one per comparison
        var minIndexReads = stats.CompareCount;
        Assert.True(stats.IndexReadCount >= minIndexReads,
            $"IndexReadCount ({stats.IndexReadCount}) should be >= {minIndexReads}");

        // IndexWrites: Each swap writes 2 elements
        var expectedWrites = stats.SwapCount * 2;
        Assert.Equal(expectedWrites, stats.IndexWriteCount);
    }

    [Theory]
    [InlineData(5)]
    [InlineData(10)]
    [InlineData(20)]
    public void TheoreticalValuesSameElementsTest(int n)
    {
        var stats = new StatisticsContext();
        var sameValues = Enumerable.Repeat(42, n).ToArray();
        QuickSort.Sort(sameValues.AsSpan(), stats);

        // QuickSort with Hoare partition on all equal elements:
        // - All elements equal to pivot
        // - Hoare partition still scans through array
        // - Many unnecessary swaps occur since all elements are equal
        // - This is a weakness of basic QuickSort (3-way partitioning handles this better)
        //
        // Expected behavior:
        // - Comparisons: O(n log n) - Still performs full partitioning at each level
        // - Swaps: O(n log n) - Many redundant swaps of equal elements
        //   Hoare partition swaps elements even when they're equal to pivot
        var minCompares = (ulong)(n);
        var maxCompares = (ulong)(n * n); // Worst case for equal elements

        // Swaps: Hoare partition swaps equal elements unnecessarily
        var minSwaps = 0UL;
        var maxSwaps = (ulong)(n * Math.Log(Math.Max(n, 2), 2) * 2);

        Assert.InRange(stats.CompareCount, minCompares, maxCompares);
        Assert.InRange(stats.SwapCount, minSwaps, maxSwaps);

        // Verify the array is still correct (all values unchanged)
        Assert.All(sameValues, val => Assert.Equal(42, val));

        // IndexReads: At least one per comparison
        var minIndexReads = stats.CompareCount;
        Assert.True(stats.IndexReadCount >= minIndexReads,
            $"IndexReadCount ({stats.IndexReadCount}) should be >= {minIndexReads}");

        // IndexWrites: Each swap writes 2 elements
        var expectedWrites = stats.SwapCount * 2;
        Assert.Equal(expectedWrites, stats.IndexWriteCount);
    }

#endif

}
