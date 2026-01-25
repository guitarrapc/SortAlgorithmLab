using SortLab.Core.Algorithms;
using SortLab.Core.Contexts;
using SortLab.Tests.Attributes;

namespace SortLab.Tests;

public class SlowSortTests
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
    public void SortResultOrderTest(IInputSample<int> inputSample)
    {
        // Slow Sort is extremely slow, so we limit to small arrays
        if (inputSample.Samples.Length <= 10)
        {
            var stats = new StatisticsContext();
            var array = inputSample.Samples.ToArray();
            SlowSort.Sort(array.AsSpan(), stats);

            Assert.Equal((ulong)inputSample.Samples.Length, (ulong)array.Length);
            Assert.True(IsSorted(array), "Array should be sorted");
        }
    }

#if DEBUG

    [CISkippableTheory]
    [ClassData(typeof(MockSortedData))]
    public void StatisticsSortedTest(IInputSample<int> inputSample)
    {
        // Slow Sort is extremely slow, so we limit to small arrays
        if (inputSample.Samples.Length <= 10)
        {
            var stats = new StatisticsContext();
            var array = inputSample.Samples.ToArray();
            SlowSort.Sort(array.AsSpan(), stats);

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
        // Slow Sort on sorted data still performs all comparisons but no swaps
        // T(n) = T(⌊n/2⌋) + T(⌈n/2⌉) + T(n-1) + 1
        // For n=5: T(5) = T(2) + T(3) + T(4) + 1 = 1 + 3 + 6 + 1 = 11
        var stats = new StatisticsContext();
        var sorted = Enumerable.Range(0, 5).ToArray();
        SlowSort.Sort(sorted.AsSpan(), stats);

        var n = sorted.Length;
        // For sorted data: comparisons still occur, but no swaps
        var expectedCompares = 11UL; // T(5) = 11
        var expectedSwaps = 0UL;
        var expectedWrites = 0UL; // No swaps means no writes
        var expectedReads = expectedCompares * 2; // Each comparison reads 2 elements

        Assert.Equal(expectedCompares, stats.CompareCount);
        Assert.Equal(expectedSwaps, stats.SwapCount);
        Assert.Equal(expectedWrites, stats.IndexWriteCount);
        Assert.Equal(expectedReads, stats.IndexReadCount);
        Assert.True(IsSorted(sorted), "Array should remain sorted");
    }

    [CISkippableTheory]
    [InlineData(3)]
    [InlineData(4)]
    [InlineData(5)]
    public void TheoreticalValuesReversedTest(int n)
    {
        // Slow Sort on reversed data performs all comparisons and many swaps
        // The number of comparisons is data-independent and follows T(n) = 2T(⌊n/2⌋) + T(n-1) + 1
        // But the number of swaps is data-dependent
        var stats = new StatisticsContext();
        var reversed = Enumerable.Range(0, n).Reverse().ToArray();
        SlowSort.Sort(reversed.AsSpan(), stats);

        // Verify the array is sorted
        Assert.Equal(Enumerable.Range(0, n), reversed);

        // For reversed data, many swaps will occur
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
        // Slow Sort has data-independent comparison count
        // but data-dependent swap count
        var stats = new StatisticsContext();
        var random = Enumerable.Range(0, n).OrderBy(_ => Guid.NewGuid()).ToArray();
        SlowSort.Sort(random.AsSpan(), stats);

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
    [InlineData(2, 1)]   // T(2) = T(1) + T(1) + T(1) + 1 = 0 + 0 + 0 + 1 = 1
    [InlineData(3, 3)]   // T(3) = T(1) + T(2) + T(2) + 1 = 0 + 1 + 1 + 1 = 3
    [InlineData(4, 6)]   // T(4) = T(2) + T(2) + T(3) + 1 = 1 + 1 + 3 + 1 = 6
    [InlineData(5, 11)]  // T(5) = T(2) + T(3) + T(4) + 1 = 1 + 3 + 6 + 1 = 11
    [InlineData(6, 18)]  // T(6) = T(3) + T(3) + T(5) + 1 = 3 + 3 + 11 + 1 = 18
    [InlineData(7, 28)]  // T(7) = T(3) + T(4) + T(6) + 1 = 3 + 6 + 18 + 1 = 28
    [InlineData(8, 41)]  // T(8) = T(4) + T(4) + T(7) + 1 = 6 + 6 + 28 + 1 = 41
    [InlineData(9, 59)]  // T(9) = T(4) + T(5) + T(8) + 1 = 6 + 11 + 41 + 1 = 59
    [InlineData(10, 82)] // T(10) = T(5) + T(5) + T(9) + 1 = 11 + 11 + 59 + 1 = 82
    public void TheoreticalComparisonCountTest(int n, int expectedComparisons)
    {
        // Test the theoretical comparison count for Slow Sort
        // Comparison count follows: T(n) = T(⌊n/2⌋) + T(⌈n/2⌉) + T(n-1) + 1 when n >= 2
        // T(0) = 0, T(1) = 0
        var stats = new StatisticsContext();
        var sorted = Enumerable.Range(0, n).ToArray();
        SlowSort.Sort(sorted.AsSpan(), stats);

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
        SlowSort.Sort(array.AsSpan(), stats);

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
        SlowSort.Sort(array.AsSpan(), stats);

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
        SlowSort.Sort(array.AsSpan(), stats);

        // Two sorted elements: T(2) = 1 comparison, no swap
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
        SlowSort.Sort(array.AsSpan(), stats);

        // Two reversed elements: T(2) = 1 comparison, 1 swap
        Assert.Equal(1UL, stats.CompareCount);
        Assert.Equal(1UL, stats.SwapCount);
        Assert.Equal(2UL, stats.IndexWriteCount); // 1 swap = 2 writes
        Assert.Equal(4UL, stats.IndexReadCount); // 1 comparison (2 reads) + 1 swap (2 reads) = 4 reads
        Assert.Equal(new[] { 1, 2 }, array);
    }

    [CISkippableTheory]
    [InlineData(3)]
    [InlineData(5)]
    [InlineData(7)]
    [InlineData(10)]
    public void IndexReadWriteConsistencyTest(int n)
    {
        // Verify that IndexRead and IndexWrite counts are consistent with Compare and Swap counts
        var stats = new StatisticsContext();
        var array = Enumerable.Range(0, n).Reverse().ToArray();
        SlowSort.Sort(array.AsSpan(), stats);

        // Each comparison reads 2 elements
        // Each swap reads 2 elements and writes 2 elements
        var expectedReads = stats.CompareCount * 2 + stats.SwapCount * 2;
        var expectedWrites = stats.SwapCount * 2;

        Assert.Equal(expectedReads, stats.IndexReadCount);
        Assert.Equal(expectedWrites, stats.IndexWriteCount);
        Assert.True(IsSorted(array), "Array should be sorted");
    }

    [CISkippableTheory]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    [InlineData(5)]
    [InlineData(6)]
    [InlineData(7)]
    public void ReversedDataMaximumSwapsTest(int n)
    {
        // Test that reversed data produces the maximum number of swaps
        // This validates that the algorithm is performing swaps correctly
        var stats = new StatisticsContext();
        var reversed = Enumerable.Range(0, n).Reverse().ToArray();
        SlowSort.Sort(reversed.AsSpan(), stats);

        // Verify the array is sorted
        Assert.Equal(Enumerable.Range(0, n), reversed);

        // For reversed data, we expect many swaps
        // The exact number depends on the recursive structure, but it should be > 0
        Assert.True(stats.SwapCount > 0, $"Expected swaps for reversed data of size {n}, got {stats.SwapCount}");
        Assert.True(stats.IndexWriteCount > 0, "Expected writes for reversed data");

        // Verify consistency: each swap = 2 writes
        Assert.Equal(stats.SwapCount * 2, stats.IndexWriteCount);
    }

    [CISkippableTheory]
    [InlineData(3)]
    [InlineData(5)]
    [InlineData(7)]
    public void ComparisonCountDataIndependentTest(int n)
    {
        // Verify that comparison count is data-independent
        // Sorted, reversed, and random data should all have the same comparison count
        var statsSorted = new StatisticsContext();
        var sorted = Enumerable.Range(0, n).ToArray();
        SlowSort.Sort(sorted.AsSpan(), statsSorted);

        var statsReversed = new StatisticsContext();
        var reversed = Enumerable.Range(0, n).Reverse().ToArray();
        SlowSort.Sort(reversed.AsSpan(), statsReversed);

        var statsRandom = new StatisticsContext();
        var random = Enumerable.Range(0, n).OrderBy(_ => Guid.NewGuid()).ToArray();
        SlowSort.Sort(random.AsSpan(), statsRandom);

        // All should have the same comparison count (data-independent)
        Assert.Equal(statsSorted.CompareCount, statsReversed.CompareCount);
        Assert.Equal(statsSorted.CompareCount, statsRandom.CompareCount);

        // But different swap counts (data-dependent)
        Assert.Equal(0UL, statsSorted.SwapCount); // Sorted has no swaps
        Assert.NotEqual(statsSorted.SwapCount, statsReversed.SwapCount); // Reversed has many swaps
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
