using SortLab.Core.Algorithms;
using SortLab.Core.Contexts;

namespace SortLab.Tests;

public class DoubleSelectionSortTests
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
        DoubleSelectionSort.Sort(array.AsSpan(), stats);

        Assert.Equal((ulong)inputSample.Samples.Length, (ulong)array.Length);
    }

    [Fact]
    public void RangeSortTest()
    {
        var stats = new StatisticsContext();
        var array = new[] { 5, 3, 8, 1, 9, 2, 7, 4, 6 };

        // Sort only the range [2, 6) -> indices 2, 3, 4, 5
        DoubleSelectionSort.Sort(array.AsSpan(), 2, 6, stats);

        // Expected: first 2 elements unchanged, middle 4 sorted, last 3 unchanged
        Assert.Equal(new[] { 5, 3, 1, 2, 8, 9, 7, 4, 6 }, array);
    }

    [Fact]
    public void RangeSortFullArrayTest()
    {
        var stats = new StatisticsContext();
        var array = new[] { 5, 3, 8, 1, 9, 2, 7, 4, 6 };

        // Sort the entire array using range API
        DoubleSelectionSort.Sort(array.AsSpan(), 0, array.Length, stats);

        Assert.Equal(new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 }, array);
    }

    [Fact]
    public void RangeSortSingleElementTest()
    {
        var stats = new StatisticsContext();
        var array = new[] { 5, 3, 8, 1, 9 };

        // Sort a single element range [2, 3)
        DoubleSelectionSort.Sort(array.AsSpan(), 2, 3, stats);

        // Array should be unchanged (single element is already sorted)
        Assert.Equal(new[] { 5, 3, 8, 1, 9 }, array);
    }

    [Fact]
    public void RangeSortBeginningTest()
    {
        var stats = new StatisticsContext();
        var array = new[] { 9, 7, 5, 3, 1, 2, 4, 6, 8 };

        // Sort only the first 5 elements [0, 5)
        DoubleSelectionSort.Sort(array.AsSpan(), 0, 5, stats);

        // Expected: first 5 sorted, last 4 unchanged
        Assert.Equal(new[] { 1, 3, 5, 7, 9, 2, 4, 6, 8 }, array);
    }

    [Fact]
    public void RangeSortEndTest()
    {
        var stats = new StatisticsContext();
        var array = new[] { 1, 3, 5, 7, 9, 8, 6, 4, 2 };

        // Sort only the last 4 elements [5, 9)
        DoubleSelectionSort.Sort(array.AsSpan(), 5, 9, stats);

        // Expected: first 5 unchanged, last 4 sorted
        Assert.Equal(new[] { 1, 3, 5, 7, 9, 2, 4, 6, 8 }, array);
    }

    [Fact]
    public void EdgeCaseMinMaxAtBoundariesTest()
    {
        var stats = new StatisticsContext();
        var array = new[] { 9, 2, 3, 4, 1 }; // max at left, min at right

        DoubleSelectionSort.Sort(array.AsSpan(), stats);

        Assert.Equal(new[] { 1, 2, 3, 4, 9 }, array);
    }

    [Fact]
    public void EdgeCaseMaxAtLeftTest()
    {
        var stats = new StatisticsContext();
        var array = new[] { 5, 1, 2, 3, 4 }; // max at left

        DoubleSelectionSort.Sort(array.AsSpan(), stats);

        Assert.Equal(new[] { 1, 2, 3, 4, 5 }, array);
    }

    [Fact]
    public void EdgeCaseMinAtRightTest()
    {
        var stats = new StatisticsContext();
        var array = new[] { 2, 3, 4, 5, 1 }; // min at right

        DoubleSelectionSort.Sort(array.AsSpan(), stats);

        Assert.Equal(new[] { 1, 2, 3, 4, 5 }, array);
    }

    [Fact]
    public void EdgeCaseAllEqualElementsTest()
    {
        var stats = new StatisticsContext();
        var array = new[] { 5, 5, 5, 5, 5 };

        DoubleSelectionSort.Sort(array.AsSpan(), stats);

        Assert.Equal(new[] { 5, 5, 5, 5, 5 }, array);
    }

    [Fact]
    public void EdgeCaseTwoElementsTest()
    {
        var stats = new StatisticsContext();
        var array = new[] { 2, 1 };

        DoubleSelectionSort.Sort(array.AsSpan(), stats);

        Assert.Equal(new[] { 1, 2 }, array);
    }

    [Fact]
    public void EdgeCaseAlreadySortedTest()
    {
        var stats = new StatisticsContext();
        var array = new[] { 1, 2, 3, 4, 5 };

        DoubleSelectionSort.Sort(array.AsSpan(), stats);

        Assert.Equal(new[] { 1, 2, 3, 4, 5 }, array);
    }

#if DEBUG

    [Theory]
    [ClassData(typeof(MockSortedData))]
    public void StatisticsSortedTest(IInputSample<int> inputSample)
    {
        var stats = new StatisticsContext();
        var array = inputSample.Samples.ToArray();
        DoubleSelectionSort.Sort(array.AsSpan(), stats);

        Assert.Equal((ulong)inputSample.Samples.Length, (ulong)array.Length);
        Assert.NotEqual(0UL, stats.IndexReadCount);
        Assert.Equal(0UL, stats.IndexWriteCount);
        Assert.NotEqual(0UL, stats.CompareCount);
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
        DoubleSelectionSort.Sort(sorted.AsSpan(), stats);

        // Double Selection Sort processes from both ends
        // In each iteration i:
        //   - Range is [left..right] where left=i, right=n-1-i
        //   - Elements compared: (right - left) [from left+1 to right]
        //   - Comparisons: 2 * (right - left) [each element compared with both min and max]
        //
        // Example for n=10:
        // i=0: left=0, right=9, compares=2*9=18
        // i=1: left=1, right=8, compares=2*7=14
        // i=2: left=2, right=7, compares=2*5=10
        // i=3: left=3, right=6, compares=2*3=6
        // i=4: left=4, right=5, compares=2*1=2
        // Total = 50

        ulong expectedCompares = 0;
        int iterations = n / 2;
        for (int i = 0; i < iterations; i++)
        {
            int left = i;
            int right = n - 1 - i;
            if (left >= right) break;
            int elementsToCompare = right - left; // from left+1 to right inclusive
            expectedCompares += (ulong)(elementsToCompare * 2);
        }

        // For sorted data, min is always at left and max is always at right
        // So no swaps are needed
        var expectedSwaps = 0UL;
        var expectedWrites = 0UL;

        // Each comparison reads 2 elements
        var minIndexReads = expectedCompares;

        Assert.Equal(expectedCompares, stats.CompareCount);
        Assert.Equal(expectedSwaps, stats.SwapCount);
        Assert.Equal(expectedWrites, stats.IndexWriteCount);
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
        DoubleSelectionSort.Sort(reversed.AsSpan(), stats);

        // Double Selection Sort for reversed data [n-1, n-2, ..., 1, 0]:
        // Comparisons are the same as sorted case
        ulong expectedCompares = 0;
        int iterations = n / 2;
        for (int i = 0; i < iterations; i++)
        {
            int left = i;
            int right = n - 1 - i;
            if (left >= right) break;
            int elementsToCompare = right - left;
            expectedCompares += (ulong)(elementsToCompare * 2);
        }

        // For reversed data, in each iteration:
        // - min is at position right (rightmost element in current range)
        // - max is at position left (leftmost element in current range)
        // This triggers the case where max==left, leading to:
        //   1. swap(left, right) - moves max to right, and what was at right (min) to left
        //   2. After the swap, min index needs adjustment, but since min was at right
        //      and is now at left, we check if min != left, which is false, so no more swaps
        // Actually, looking at the code: when max==left, it swaps(left, right), then
        // adjusts min index: if min was at right, it's now at left.
        // Then it checks "if (min != left)" which is false, so no second swap.
        // Result: only 1 swap per iteration.
        // Number of swaps = floor(n/2)
        var expectedSwaps = (ulong)(n / 2);
        var expectedWrites = (ulong)n; // Each swap writes 2 elements

        var minIndexReads = expectedCompares;

        Assert.Equal(expectedCompares, stats.CompareCount);
        Assert.Equal(expectedSwaps, stats.SwapCount);
        Assert.Equal(expectedWrites, stats.IndexWriteCount);
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
        DoubleSelectionSort.Sort(random.AsSpan(), stats);

        // Double Selection Sort comparison count
        ulong expectedCompares = 0;
        int iterations = n / 2;
        for (int i = 0; i < iterations; i++)
        {
            int left = i;
            int right = n - 1 - i;
            if (left >= right) break;
            int elementsToCompare = right - left;
            expectedCompares += (ulong)(elementsToCompare * 2);
        }

        // For random data, swap count varies
        // Minimum: 0 swaps (already sorted)
        // Maximum: n swaps (worst case, up to 2 swaps per iteration)
        var minSwaps = 0UL;
        var maxSwaps = (ulong)n;

        var minIndexReads = expectedCompares;

        Assert.Equal(expectedCompares, stats.CompareCount);
        Assert.InRange(stats.SwapCount, minSwaps, maxSwaps);
        Assert.True(stats.IndexReadCount >= minIndexReads,
            $"IndexReadCount ({stats.IndexReadCount}) should be >= {minIndexReads}");
    }

#endif

}
