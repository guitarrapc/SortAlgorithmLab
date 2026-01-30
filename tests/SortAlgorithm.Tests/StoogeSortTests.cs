using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;
using SortAlgorithm.Tests.Attributes;

namespace SortAlgorithm.Tests;

public class StoogeSortTests
{
    [CISkippableTheory]
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
        // Stooge Sort is extremely slow, so we limit to small arrays
        if (inputSample.Samples.Length <= 10)
        {
            var stats = new StatisticsContext();
            var array = inputSample.Samples.ToArray();
            StoogeSort.Sort(array.AsSpan(), stats);

            Assert.Equal((ulong)inputSample.Samples.Length, (ulong)array.Length);
            Assert.True(IsSorted(array), "Array should be sorted");
        }
    }

#if DEBUG

    [CISkippableTheory]
    [ClassData(typeof(MockSortedData))]
    public void StatisticsSortedTest(IInputSample<int> inputSample)
    {
        // Stooge Sort is extremely slow, so we limit to small arrays
        if (inputSample.Samples.Length <= 10)
        {
            var stats = new StatisticsContext();
            var array = inputSample.Samples.ToArray();
            StoogeSort.Sort(array.AsSpan(), stats);

            Assert.Equal((ulong)inputSample.Samples.Length, (ulong)array.Length);
            Assert.NotEqual(0UL, stats.IndexReadCount);
            Assert.Equal(0UL, stats.IndexWriteCount); // Already sorted, no swaps needed
            Assert.NotEqual(0UL, stats.CompareCount);
            Assert.Equal(0UL, stats.SwapCount); // Already sorted
        }
    }

    [CISkippableFact]
    public void TheoreticalValuesSortedTest()
    {
        // Stooge Sort on sorted data still performs all comparisons but no swaps
        // T(n) = 3T(⌈2n/3⌉) + 1 (comparison)
        // For n=5: Expected structure:
        // - Initial comparison (0,4): 1 comparison, 0 swaps
        // - Recursive calls follow the same pattern
        var stats = new StatisticsContext();
        var sorted = Enumerable.Range(0, 5).ToArray();
        StoogeSort.Sort(sorted.AsSpan(), stats);

        var n = sorted.Length;
        // For sorted data: comparisons still occur, but no swaps
        var expectedSwaps = 0UL;
        var expectedWrites = 0UL; // No swaps means no writes

        // Number of comparisons follows the recurrence: T(n) = 3T(⌈2n/3⌉) + 1
        // For n=5, this evaluates to a specific value
        // We verify that comparisons occurred but no swaps/writes
        Assert.NotEqual(0UL, stats.CompareCount);
        Assert.Equal(expectedSwaps, stats.SwapCount);
        Assert.Equal(expectedWrites, stats.IndexWriteCount);
        Assert.NotEqual(0UL, stats.IndexReadCount); // Reads occur during comparisons
        Assert.True(IsSorted(sorted), "Array should remain sorted");
    }

    [CISkippableTheory]
    [InlineData(3)]
    [InlineData(4)]
    [InlineData(5)]
    public void TheoreticalValuesReversedTest(int n)
    {
        // Stooge Sort on reversed data performs maximum swaps
        // The algorithm always performs the same number of comparisons regardless of input,
        // but the number of swaps varies based on data arrangement
        var stats = new StatisticsContext();
        var reversed = Enumerable.Range(0, n).Reverse().ToArray();
        StoogeSort.Sort(reversed.AsSpan(), stats);

        // Verify the array is sorted
        Assert.Equal(Enumerable.Range(0, n), reversed);

        // For reversed data, many swaps will occur
        // Number of comparisons: O(n^2.71) - follows T(n) = 3T(⌈2n/3⌉) + 1
        Assert.NotEqual(0UL, stats.CompareCount);
        Assert.NotEqual(0UL, stats.SwapCount);
        Assert.NotEqual(0UL, stats.IndexWriteCount);
        Assert.NotEqual(0UL, stats.IndexReadCount);

        // Each swap involves 2 reads + 2 writes
        Assert.Equal(stats.SwapCount * 2, stats.IndexWriteCount);
        // IndexReadCount includes reads from comparisons (2 per compare) and swaps (2 per swap)
        var expectedReads = stats.CompareCount * 2 + stats.SwapCount * 2;
        Assert.Equal(expectedReads, stats.IndexReadCount);
    }

    [CISkippableTheory]
    [InlineData(3)]
    [InlineData(4)]
    [InlineData(5)]
    public void TheoreticalValuesRandomTest(int n)
    {
        // Stooge Sort has data-independent comparison count
        // but data-dependent swap count
        var stats = new StatisticsContext();
        var random = Enumerable.Range(0, n).OrderBy(_ => Guid.NewGuid()).ToArray();
        StoogeSort.Sort(random.AsSpan(), stats);

        // Verify the array is sorted
        Assert.Equal(Enumerable.Range(0, n), random);

        // Verify operations were performed
        Assert.NotEqual(0UL, stats.CompareCount);
        Assert.True(stats.IndexReadCount > 0);

        // For non-sorted input, there should be some swaps
        // (unless the random input happens to already be sorted, very unlikely)
        // We verify the relationship between swaps and writes
        if (stats.SwapCount > 0)
        {
            Assert.Equal(stats.SwapCount * 2, stats.IndexWriteCount);
        }

        // IndexReadCount = CompareCount * 2 + SwapCount * 2
        var expectedReads = stats.CompareCount * 2 + stats.SwapCount * 2;
        Assert.Equal(expectedReads, stats.IndexReadCount);
    }

    [CISkippableTheory]
    [InlineData(2, 1)]  // Base case: 2 elements
    [InlineData(3, 4)]  // T(3) = 3×T(2) + 1 = 3×1 + 1 = 4
    [InlineData(4, 13)] // T(4) = 3×T(3) + 1 = 3×4 + 1 = 13
    [InlineData(5, 40)] // T(5) = 3×T(4) + 1 = 3×13 + 1 = 40
    public void TheoreticalComparisonCountTest(int n, int expectedComparisons)
    {
        // Test the theoretical comparison count for Stooge Sort
        // Comparison count follows: T(n) = 3T(⌈2n/3⌉) + 1 when n >= 3
        // T(1) = 0, T(2) = 1
        var stats = new StatisticsContext();
        var sorted = Enumerable.Range(0, n).ToArray();
        StoogeSort.Sort(sorted.AsSpan(), stats);

        // For sorted data, all comparisons occur but no swaps
        Assert.Equal((ulong)expectedComparisons, stats.CompareCount);
        Assert.Equal(0UL, stats.SwapCount); // Sorted data has no swaps
        Assert.Equal(0UL, stats.IndexWriteCount); // No swaps means no writes

        // IndexReadCount = CompareCount * 2 (each comparison reads 2 elements)
        var expectedReads = (ulong)(expectedComparisons * 2);
        Assert.Equal(expectedReads, stats.IndexReadCount);
    }

    [CISkippableFact]
    public void EdgeCaseSingleElementTest()
    {
        var stats = new StatisticsContext();
        var array = new[] { 42 };
        StoogeSort.Sort(array.AsSpan(), stats);

        // Single element: no comparisons, no operations
        Assert.Equal(0UL, stats.CompareCount);
        Assert.Equal(0UL, stats.SwapCount);
        Assert.Equal(0UL, stats.IndexWriteCount);
        Assert.Equal(0UL, stats.IndexReadCount);
        Assert.Equal(new[] { 42 }, array);
    }

    [CISkippableFact]
    public void EdgeCaseEmptyTest()
    {
        var stats = new StatisticsContext();
        var array = Array.Empty<int>();
        StoogeSort.Sort(array.AsSpan(), stats);

        // Empty array: no operations
        Assert.Equal(0UL, stats.CompareCount);
        Assert.Equal(0UL, stats.SwapCount);
        Assert.Equal(0UL, stats.IndexWriteCount);
        Assert.Equal(0UL, stats.IndexReadCount);
    }

    [CISkippableFact]
    public void EdgeCaseTwoElementsSortedTest()
    {
        var stats = new StatisticsContext();
        var array = new[] { 1, 2 };
        StoogeSort.Sort(array.AsSpan(), stats);

        // Two sorted elements: 1 comparison, no swap
        Assert.Equal(1UL, stats.CompareCount);
        Assert.Equal(0UL, stats.SwapCount);
        Assert.Equal(0UL, stats.IndexWriteCount);
        Assert.Equal(2UL, stats.IndexReadCount); // 1 comparison = 2 reads
        Assert.Equal(new[] { 1, 2 }, array);
    }

    [CISkippableFact]
    public void EdgeCaseTwoElementsReversedTest()
    {
        var stats = new StatisticsContext();
        var array = new[] { 2, 1 };
        StoogeSort.Sort(array.AsSpan(), stats);

        // Two reversed elements: 1 comparison, 1 swap
        Assert.Equal(1UL, stats.CompareCount);
        Assert.Equal(1UL, stats.SwapCount);
        Assert.Equal(2UL, stats.IndexWriteCount); // 1 swap = 2 writes
        Assert.Equal(4UL, stats.IndexReadCount); // 1 comparison (2 reads) + 1 swap (2 reads) = 4 reads
        Assert.Equal(new[] { 1, 2 }, array);
    }

#endif

    private static bool IsSorted<T>(T[] array) where T : IComparable<T>
    {
        for (int i = 1; i < array.Length; i++)
        {
            if (array[i - 1].CompareTo(array[i]) > 0)
                return false;
        }
        return true;
    }
}
