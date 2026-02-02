using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;

namespace SortAlgorithm.Tests;

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
        StableQuickSort.Sort(array.AsSpan(), stats);

        Assert.Equal((ulong)inputSample.Samples.Length, (ulong)array.Length);
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
    public void SortedArrayTest()
    {
        var stats = new StatisticsContext();
        var sorted = Enumerable.Range(1, 100).ToArray();
        StableQuickSort.Sort(sorted.AsSpan(), stats);

        Assert.Equal(Enumerable.Range(1, 100).ToArray(), sorted);
    }

    [Fact]
    public void ReverseSortedArrayTest()
    {
        var stats = new StatisticsContext();
        var reversed = Enumerable.Range(1, 100).Reverse().ToArray();
        StableQuickSort.Sort(reversed.AsSpan(), stats);

        Assert.Equal(Enumerable.Range(1, 100).ToArray(), reversed);
    }

    [Fact]
    public void AllEqualElementsTest()
    {
        var stats = new StatisticsContext();
        var allEqual = Enumerable.Repeat(42, 100).ToArray();
        StableQuickSort.Sort(allEqual.AsSpan(), stats);

        Assert.Equal(Enumerable.Repeat(42, 100).ToArray(), allEqual);
    }

    [Fact]
    public void ManyDuplicatesTest()
    {
        var stats = new StatisticsContext();
        var duplicates = new[] { 1, 2, 1, 3, 2, 1, 4, 3, 2, 1, 5, 4, 3, 2, 1 };
        StableQuickSort.Sort(duplicates.AsSpan(), stats);

        Assert.Equal(new[] { 1, 1, 1, 1, 1, 2, 2, 2, 2, 3, 3, 3, 4, 4, 5 }, duplicates);
    }

    [Fact]
    public void LargeArrayTest()
    {
        var stats = new StatisticsContext();
        var random = new Random(42);
        var large = Enumerable.Range(0, 10000).OrderBy(_ => random.Next()).ToArray();
        var expected = large.OrderBy(x => x).ToArray();

        StableQuickSort.Sort(large.AsSpan(), stats);

        Assert.Equal(expected, large);
    }

    [Fact]
    public void NearlySortedArrayTest()
    {
        var stats = new StatisticsContext();
        var nearlySorted = Enumerable.Range(1, 100).ToArray();
        // Swap a few elements to make it nearly sorted
        (nearlySorted[10], nearlySorted[20]) = (nearlySorted[20], nearlySorted[10]);
        (nearlySorted[50], nearlySorted[60]) = (nearlySorted[60], nearlySorted[50]);

        StableQuickSort.Sort(nearlySorted.AsSpan(), stats);

        Assert.Equal(Enumerable.Range(1, 100).ToArray(), nearlySorted);
    }

    [Fact]
    public void SmallArrayInsertionSortThresholdTest()
    {
        var stats = new StatisticsContext();
        var small = new[] { 5, 2, 8, 1, 9, 3, 7, 4, 6, 10, 15, 12, 18, 11, 19, 13, 17, 14, 16, 20 };
        StableQuickSort.Sort(small.AsSpan(), stats);

        Assert.Equal(Enumerable.Range(1, 20).ToArray(), small);
    }

    [Fact]
    public void StringSortTest()
    {
        var stats = new StatisticsContext();
        var strings = new[] { "zebra", "apple", "mango", "banana", "cherry" };
        StableQuickSort.Sort(strings.AsSpan(), stats);

        Assert.Equal(new[] { "apple", "banana", "cherry", "mango", "zebra" }, strings);
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
        // Expected behavior with SortSpan-based temporary buffer:
        //
        // MedianOf3Value (all elements equal):
        //   - s.Compare(lowIdx, midIdx): 2 reads (low, mid)
        //   - low == mid, so else branch
        //   - s.Compare(midIdx, highIdx): 2 reads (mid, high)
        //   - mid == high, so else branch
        //   - s.Read(midIdx): 1 read
        //   Total: 5 reads, 2 compares
        //
        // StablePartition Phase 1 (count):
        //   - for loop: n iterations
        //   - s.Read(i): n reads
        //   - element.CompareTo(pivot): not tracked by SortSpan
        //   Total: n reads, 0 compares
        //
        // StablePartition Phase 2 (distribute to temp):
        //   - for loop: n iterations
        //   - s.Read(i): n reads (store to element)
        //   - s.Compare(i, pivot): n reads + n compares (all equal, so false)
        //   - else if s.Compare(i, pivot) == 0: n reads + n compares (all true)
        //   - tempSortSpan.Write(equalIdx++, element): n writes to temp buffer
        //   Total: 3n reads (n + n + n), 2n compares, n writes to temp
        //
        // StablePartition Phase 3 (copy back):
        //   - for loop: n iterations
        //   - tempSortSpan.Read(i): n reads from temp buffer
        //   - s.Write(left + i, ...): n writes to main buffer
        //   Total: n reads from temp, n writes to main
        //
        // Grand Total:
        //   - Reads: 5 (median) + n (phase1) + 3n (phase2) + n (phase3) = 5 + 5n
        //   - Writes: n (phase2 to temp) + n (phase3 to main) = 2n
        //   - Compares: 2 (median) + 2n (phase2) = 2 + 2n
        //   - Swaps: 0

        // Comparisons: 2 (median-of-3) + 2n (partition)
        var expectedCompares = (ulong)(2 + 2 * n);
        Assert.Equal(expectedCompares, stats.CompareCount);

        // StableQuickSort doesn't use Swap
        Assert.Equal(0UL, stats.SwapCount);

        // Verify the array is still correct (all values unchanged)
        Assert.All(sameValues, val => Assert.Equal(42, val));

        // IndexReads: 5 (median) + n (phase1) + 3n (phase2) + n (phase3) = 5 + 5n
        var expectedIndexReads = (ulong)(5 + 5 * n);
        Assert.Equal(expectedIndexReads, stats.IndexReadCount);

        // IndexWrites: n (phase2 to temp) + n (phase3 to main) = 2n
        var expectedIndexWrites = (ulong)(2 * n);
        Assert.Equal(expectedIndexWrites, stats.IndexWriteCount);
    }

#endif

}
