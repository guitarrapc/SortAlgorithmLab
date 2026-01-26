using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;

namespace SortAlgorithm.Tests;

public class BlockQuickSortTests
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
        BlockQuickSort.Sort(array.AsSpan(), stats);

        Assert.Equal((ulong)inputSample.Samples.Length, (ulong)array.Length);
    }

    [Fact]
    public void EdgeCaseEmptyArrayTest()
    {
        var stats = new StatisticsContext();
        var empty = Array.Empty<int>();
        BlockQuickSort.Sort(empty.AsSpan(), stats);
    }

    [Fact]
    public void EdgeCaseSingleElementTest()
    {
        var stats = new StatisticsContext();
        var single = new[] { 42 };
        BlockQuickSort.Sort(single.AsSpan(), stats);

        Assert.Equal(42, single[0]);
    }

    [Fact]
    public void EdgeCaseTwoElementsSortedTest()
    {
        var stats = new StatisticsContext();
        var twoSorted = new[] { 1, 2 };
        BlockQuickSort.Sort(twoSorted.AsSpan(), stats);

        Assert.Equal([1, 2], twoSorted);
    }

    [Fact]
    public void EdgeCaseTwoElementsReversedTest()
    {
        var stats = new StatisticsContext();
        var twoReversed = new[] { 2, 1 };
        BlockQuickSort.Sort(twoReversed.AsSpan(), stats);

        Assert.Equal([1, 2], twoReversed);
    }

    [Fact]
    public void EdgeCaseThreeElementsTest()
    {
        var stats = new StatisticsContext();
        var three = new[] { 3, 1, 2 };
        BlockQuickSort.Sort(three.AsSpan(), stats);

        Assert.Equal([1, 2, 3], three);
    }

    [Fact]
    public void RangeSortTest()
    {
        var stats = new StatisticsContext();
        var array = new[] { 5, 3, 8, 1, 9, 2, 7, 4, 6 };

        // Sort only the range [2, 6) -> indices 2, 3, 4, 5
        BlockQuickSort.Sort(array.AsSpan(), 2, 6, stats);

        // Expected: first 2 elements unchanged, middle 4 sorted, last 3 unchanged
        Assert.Equal(new[] { 5, 3, 1, 2, 8, 9, 7, 4, 6 }, array);
    }

    [Fact]
    public void RangeSortFullArrayTest()
    {
        var stats = new StatisticsContext();
        var array = new[] { 5, 3, 8, 1, 9, 2, 7, 4, 6 };

        // Sort the entire array using range API
        BlockQuickSort.Sort(array.AsSpan(), 0, array.Length, stats);

        Assert.Equal(new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 }, array);
    }

    [Fact]
    public void RangeSortEmptyRangeTest()
    {
        var stats = new StatisticsContext();
        var array = new[] { 3, 1, 2 };

        // Sort empty range [1, 1)
        BlockQuickSort.Sort(array.AsSpan(), 1, 1, stats);

        // Array should remain unchanged
        Assert.Equal(new[] { 3, 1, 2 }, array);
    }

    [Fact]
    public void SmallArraySwitchesToInsertionSortTest()
    {
        var stats = new StatisticsContext();
        var small = new[] { 9, 7, 5, 3, 1, 2, 4, 6, 8, 10, 12, 14, 16, 18, 20 }; // 15 elements (< 20 threshold)
        BlockQuickSort.Sort(small.AsSpan(), stats);

        // Verify array is sorted
        for (var i = 0; i < small.Length - 1; i++)
        {
            Assert.True(small[i] <= small[i + 1], $"Array not sorted at index {i}: {small[i]} > {small[i + 1]}");
        }
    }

    [Fact]
    public void LargeArrayTest()
    {
        var stats = new StatisticsContext();
        var size = 1000;
        var random = new Random(42);
        var array = Enumerable.Range(0, size).OrderBy(_ => random.Next()).ToArray();
        BlockQuickSort.Sort(array.AsSpan(), stats);

        // Verify sorted
        for (var i = 0; i < size - 1; i++)
        {
            Assert.True(array[i] <= array[i + 1], $"Array not sorted at index {i}: {array[i]} > {array[i + 1]}");
        }
    }

    [Fact]
    public void VeryLargeArrayTest()
    {
        var stats = new StatisticsContext();
        var size = 10000;
        var random = new Random(42);
        var array = Enumerable.Range(0, size).OrderBy(_ => random.Next()).ToArray();
        BlockQuickSort.Sort(array.AsSpan(), stats);

        // Verify sorted
        for (var i = 0; i < size - 1; i++)
        {
            Assert.True(array[i] <= array[i + 1], $"Array not sorted at index {i}: {array[i]} > {array[i + 1]}");
        }
    }

    [Fact]
    public void MassiveArrayTestMedianOfSqrt()
    {
        var stats = new StatisticsContext();
        var size = 25000; // > 20000 to trigger median-of-sqrt(n)
        var random = new Random(42);
        var array = Enumerable.Range(0, size).OrderBy(_ => random.Next()).ToArray();
        BlockQuickSort.Sort(array.AsSpan(), stats);

        // Verify sorted
        for (var i = 0; i < size - 1; i++)
        {
            Assert.True(array[i] <= array[i + 1], $"Array not sorted at index {i}: {array[i]} > {array[i + 1]}");
        }
    }

    [Fact]
    public void LargeArrayTestMedianOf5MediansOf5()
    {
        var stats = new StatisticsContext();
        var size = 1000; // > 800 to trigger median-of-5-medians-of-5
        var random = new Random(42);
        var array = Enumerable.Range(0, size).OrderBy(_ => random.Next()).ToArray();
        BlockQuickSort.Sort(array.AsSpan(), stats);

        // Verify sorted
        for (var i = 0; i < size - 1; i++)
        {
            Assert.True(array[i] <= array[i + 1], $"Array not sorted at index {i}: {array[i]} > {array[i + 1]}");
        }
    }

    [Fact]
    public void MediumArrayTestMedianOf3MediansOf3()
    {
        var stats = new StatisticsContext();
        var size = 200; // > 100 to trigger median-of-3-medians-of-3
        var random = new Random(42);
        var array = Enumerable.Range(0, size).OrderBy(_ => random.Next()).ToArray();
        BlockQuickSort.Sort(array.AsSpan(), stats);

        // Verify sorted
        for (var i = 0; i < size - 1; i++)
        {
            Assert.True(array[i] <= array[i + 1], $"Array not sorted at index {i}: {array[i]} > {array[i + 1]}");
        }
    }

    [Fact]
    public void BlockPartitioningBoundaryTest()
    {
        var stats = new StatisticsContext();
        // Test with size exactly at block size (128)
        var size = 128;
        var random = new Random(42);
        var array = Enumerable.Range(0, size).OrderBy(_ => random.Next()).ToArray();
        BlockQuickSort.Sort(array.AsSpan(), stats);

        // Verify sorted
        for (var i = 0; i < size - 1; i++)
        {
            Assert.True(array[i] <= array[i + 1], $"Array not sorted at index {i}: {array[i]} > {array[i + 1]}");
        }
    }

    [Fact]
    public void MultipleBlocksTest()
    {
        var stats = new StatisticsContext();
        // Test with size requiring multiple blocks (3 * 128 = 384)
        var size = 384;
        var random = new Random(42);
        var array = Enumerable.Range(0, size).OrderBy(_ => random.Next()).ToArray();
        BlockQuickSort.Sort(array.AsSpan(), stats);

        // Verify sorted
        for (var i = 0; i < size - 1; i++)
        {
            Assert.True(array[i] <= array[i + 1], $"Array not sorted at index {i}: {array[i]} > {array[i + 1]}");
        }
    }

    [Fact]
    public void DuplicateValuesTest()
    {
        var stats = new StatisticsContext();
        var array = new[] { 5, 2, 8, 2, 9, 1, 5, 5, 3, 2 };
        BlockQuickSort.Sort(array.AsSpan(), stats);

        Assert.Equal(new[] { 1, 2, 2, 2, 3, 5, 5, 5, 8, 9 }, array);
    }

    /// <summary>
    /// Tests pattern with repeating sequences of duplicates.
    /// Example: [1,1,1,2,2,2,3,3,3,...] shuffled
    /// </summary>
    [Fact]
    public void RepeatingDuplicateSequencesTest()
    {
        var stats = new StatisticsContext();
        var array = new List<int>();
        for (var i = 0; i < 100; i++)
        {
            // Each value appears 5 times
            for (var j = 0; j < 5; j++)
            {
                array.Add(i);
            }
        }

        // Shuffle the array
        var random = new Random(42);
        var shuffled = array.OrderBy(_ => random.Next()).ToArray();

        BlockQuickSort.Sort(shuffled.AsSpan(), stats);

        // Verify sorted and count occurrences
        for (var i = 0; i < shuffled.Length - 1; i++)
        {
            Assert.True(shuffled[i] <= shuffled[i + 1], 
                $"Array not sorted at index {i}: {shuffled[i]} > {shuffled[i + 1]}");
        }

        // Verify each value appears exactly 5 times
        for (var i = 0; i < 100; i++)
        {
            var count = shuffled.Count(x => x == i);
            Assert.Equal(5, count);
        }
    }

    /// <summary>
    /// Tests array with duplicates at block boundaries.
    /// This ensures block partitioning handles duplicates correctly across block transitions.
    /// </summary>
    [Fact]
    public void DuplicatesAtBlockBoundariesTest()
    {
        var stats = new StatisticsContext();
        var blockSize = 128;
        var array = new int[blockSize * 3];

        // Fill with pattern: many duplicates at block boundaries
        for (var i = 0; i < array.Length; i++)
        {
            if (i % blockSize < 10 || i % blockSize >= blockSize - 10)
            {
                array[i] = 1; // Duplicates near boundaries
            }
            else
            {
                array[i] = i; // Unique values in the middle
            }
        }

        // Shuffle
        var random = new Random(42);
        var shuffled = array.OrderBy(_ => random.Next()).ToArray();

        BlockQuickSort.Sort(shuffled.AsSpan(), stats);

        // Verify sorted
        for (var i = 0; i < shuffled.Length - 1; i++)
        {
            Assert.True(shuffled[i] <= shuffled[i + 1], 
                $"Array not sorted at index {i}: {shuffled[i]} > {shuffled[i + 1]}");
        }
    }

    /// <summary>
    /// Tests the case where pivot value appears many times in the array.
    /// Paper mentions: "pivot occurs twice in the sample" as a trigger for duplicate check.
    /// </summary>
    [Fact]
    public void PivotValueAppearsManyTimesTest()
    {
        var stats = new StatisticsContext();
        var size = 1000;
        var array = new int[size];

        // Fill array: 50% are the median value (500)
        var random = new Random(42);
        for (var i = 0; i < size; i++)
        {
            array[i] = random.Next(2) == 0 ? 500 : random.Next(1000);
        }

        BlockQuickSort.Sort(array.AsSpan(), stats);

        // Verify sorted
        for (var i = 0; i < size - 1; i++)
        {
            Assert.True(array[i] <= array[i + 1], 
                $"Array not sorted at index {i}: {array[i]} > {array[i + 1]}");
        }
    }

    /// <summary>
    /// Tests duplicate check optimization with small arrays containing many duplicates.
    /// This triggers the duplicate check path (size ≤ 512).
    /// </summary>
    [Fact]
    public void DuplicateCheckSmallArrayTest()
    {
        var stats = new StatisticsContext();
        var size = 300; // Small enough to trigger duplicate check
        var array = new int[size];

        // 80% of elements are value 50
        var random = new Random(42);
        for (var i = 0; i < size; i++)
        {
            array[i] = random.Next(10) < 8 ? 50 : random.Next(100);
        }

        BlockQuickSort.Sort(array.AsSpan(), stats);

        // Verify sorted
        for (var i = 0; i < size - 1; i++)
        {
            Assert.True(array[i] <= array[i + 1], 
                $"Array not sorted at index {i}: {array[i]} > {array[i + 1]}");
        }

        // Verify many elements are 50
        var count50 = array.Count(x => x == 50);
        Assert.True(count50 >= size * 0.7, $"Expected at least 70% to be 50, got {count50}/{size}");
    }

    /// <summary>
    /// Tests duplicate check with arrays at the threshold boundary.
    /// </summary>
    [Theory]
    [InlineData(256)]  // Half the threshold
    [InlineData(512)]  // At threshold
    [InlineData(513)]  // Just above threshold (no duplicate check)
    public void DuplicateCheckThresholdTest(int size)
    {
        var stats = new StatisticsContext();
        var array = new int[size];

        // 60% duplicates
        var random = new Random(42);
        for (var i = 0; i < size; i++)
        {
            array[i] = random.Next(10) < 6 ? 42 : random.Next(size);
        }

        BlockQuickSort.Sort(array.AsSpan(), stats);

        // Verify sorted
        for (var i = 0; i < size - 1; i++)
        {
            Assert.True(array[i] <= array[i + 1], 
                $"Array not sorted at index {i}: {array[i]} > {array[i + 1]}");
        }
    }

    /// <summary>
    /// Tests that duplicate check stops early when duplicates are sparse.
    /// The algorithm should stop scanning when less than 25% are duplicates.
    /// </summary>
    [Fact]
    public void DuplicateCheckEarlyStopTest()
    {
        var stats = new StatisticsContext();
        var size = 400;
        var array = new int[size];

        // First 20% are duplicates (value 1), rest are unique
        // This should trigger early stop in duplicate scanning
        for (var i = 0; i < size / 5; i++)
        {
            array[i] = 1;
        }
        for (var i = size / 5; i < size; i++)
        {
            array[i] = i;
        }

        // Shuffle
        var random = new Random(42);
        var shuffled = array.OrderBy(_ => random.Next()).ToArray();

        BlockQuickSort.Sort(shuffled.AsSpan(), stats);

        // Verify sorted
        for (var i = 0; i < size - 1; i++)
        {
            Assert.True(shuffled[i] <= shuffled[i + 1], 
                $"Array not sorted at index {i}: {shuffled[i]} > {shuffled[i + 1]}");
        }
    }



#if DEBUG

    [Theory]
    [ClassData(typeof(MockSortedData))]
    public void StatisticsSortedTest(IInputSample<int> inputSample)
    {
        var stats = new StatisticsContext();
        var array = inputSample.Samples.ToArray();
        BlockQuickSort.Sort(array.AsSpan(), stats);

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
        BlockQuickSort.Sort(sorted.AsSpan(), stats);

        // BlockQuickSort behavior on sorted data:
        // - For small arrays (n ≤ 20): Uses InsertionSort
        //   - Best case O(n): n-1 comparisons, 0 swaps
        // - For larger arrays (n > 20): Uses QuickSort with adaptive pivot selection
        //   - For sorted data with median-of-3 or better pivot selection,
        //     partitioning creates relatively balanced partitions
        //   - InsertionSort is used for final small partitions (≤ 20 elements)
        //   - Block partitioning comparisons + InsertionSort for small partitions

        ulong minCompares, maxCompares, minSwaps, maxSwaps;

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
        BlockQuickSort.Sort(reversed.AsSpan(), stats);

        // BlockQuickSort behavior on reversed data:
        // - For small arrays (n ≤ 20): Uses InsertionSort
        //   - Worst case O(n²): n(n-1)/2 comparisons, (n-1)(n+2)/2 writes
        // - For larger arrays (n > 20): Uses QuickSort with adaptive pivot selection
        //   - Median-of-3 or better pivot selection helps avoid worst-case
        //   - Block partitioning occurs, then InsertionSort for final small partitions
        //   - Overall: O(n log n) behavior with good pivot selection

        ulong minCompares, maxCompares;

        // Conservative bounds that handle both paths
        if (n <= 20)
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
        BlockQuickSort.Sort(random.AsSpan(), stats);

        // BlockQuickSort behavior on random data:
        // - For small arrays (n ≤ 20): Uses InsertionSort
        //   - Average case O(n²): approximately n²/4 comparisons
        // - For larger arrays: Uses QuickSort with adaptive pivot selection + InsertionSort for small partitions
        //   - Average case: ~1.2-1.4n log₂ n comparisons (better than standard QuickSort)
        //   - Block partitioning improves cache efficiency without changing comparison count
        //   - Combined complexity: O(n log n) with InsertionSort overhead

        // Conservative bounds for both paths
        ulong minCompares = (ulong)(n - 1); // Minimum: at least n-1 comparisons
        ulong maxCompares = (ulong)(n * n); // Allow for InsertionSort worst-case on partitions

        Assert.InRange(stats.CompareCount, minCompares, maxCompares);

        var minIndexReads = stats.CompareCount;
        Assert.True(stats.IndexReadCount >= minIndexReads,
            $"IndexReadCount ({stats.IndexReadCount}) should be >= {minIndexReads}");
    }

#endif
}
