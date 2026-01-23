using SortLab.Core.Algorithms;
using SortLab.Core.Contexts;

namespace SortLab.Tests;

public class BubbleSortTests
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
        BubbleSort.Sort(array.AsSpan());
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
        BubbleSort.Sort(array.AsSpan(), stats);

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
        BubbleSort.Sort(array.AsSpan(), stats);

        Assert.Equal((ulong)inputSample.Samples.Length, (ulong)array.Length);
        Assert.NotEqual(0UL, stats.IndexReadCount);
        Assert.Equal(0UL, stats.IndexWriteCount);
        Assert.NotEqual(0UL, stats.CompareCount);
        Assert.Equal(0UL, stats.SwapCount);
    }

    [Theory]
    [ClassData(typeof(MockRandomData))]
    public void StatisticsResetTest(IInputSample<int> inputSample)
    {
        var stats = new StatisticsContext();
        var array = inputSample.Samples.ToArray();
        BubbleSort.Sort(array.AsSpan(), stats);

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
        BubbleSort.Sort(sorted.AsSpan(), stats);

        // Bubble Sort always performs n(n-1)/2 comparisons regardless of input order
        // For sorted data, no swaps are needed since all elements are already in order
        var expectedCompares = (ulong)(n * (n - 1) / 2);
        var expectedSwaps = 0UL;
        var expectedWrites = 0UL; // No swaps = no writes

        // Each comparison reads 2 elements (positions j and j-1)
        // Total reads = 2 * number of comparisons
        var expectedReads = expectedCompares * 2;

        Assert.Equal(expectedCompares, stats.CompareCount);
        Assert.Equal(expectedSwaps, stats.SwapCount);
        Assert.Equal(expectedWrites, stats.IndexWriteCount);
        Assert.Equal(expectedReads, stats.IndexReadCount);
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
        BubbleSort.Sort(reversed.AsSpan(), stats);

        // Bubble Sort worst case: reversed array
        // Comparisons: n(n-1)/2 (all adjacent pairs are compared)
        // Swaps: n(n-1)/2 (every comparison results in a swap)
        var expectedCompares = (ulong)(n * (n - 1) / 2);
        var expectedSwaps = (ulong)(n * (n - 1) / 2);

        // Each swap writes 2 elements (swap reads and writes both positions)
        var expectedWrites = expectedSwaps * 2;

        // Each comparison reads 2 elements + each swap reads 2 elements
        // Total reads = 2 * comparisons + 2 * swaps = 4 * n(n-1)/2
        var expectedReads = expectedCompares * 2 + expectedSwaps * 2;

        Assert.Equal(expectedCompares, stats.CompareCount);
        Assert.Equal(expectedSwaps, stats.SwapCount);
        Assert.Equal(expectedWrites, stats.IndexWriteCount);
        Assert.Equal(expectedReads, stats.IndexReadCount);
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
        BubbleSort.Sort(random.AsSpan(), stats);

        // Bubble Sort always performs n(n-1)/2 comparisons regardless of input
        // For random data, swap count varies based on the number of inversions
        // - Best case: 0 swaps (already sorted by chance)
        // - Average case: n(n-1)/4 swaps (approximately half of comparisons result in swaps)
        // - Worst case: n(n-1)/2 swaps (reversed)
        var expectedCompares = (ulong)(n * (n - 1) / 2);
        var minSwaps = 0UL;
        var maxSwaps = (ulong)(n * (n - 1) / 2);

        // Each comparison reads 2 elements
        var minReads = expectedCompares * 2;
        // Maximum reads occur when all comparisons result in swaps
        var maxReads = expectedCompares * 2 + maxSwaps * 2;

        Assert.Equal(expectedCompares, stats.CompareCount);
        Assert.InRange(stats.SwapCount, minSwaps, maxSwaps);
        Assert.InRange(stats.IndexReadCount, minReads, maxReads);

        // Writes = 2 * swaps (each swap writes 2 elements)
        var expectedMinWrites = minSwaps * 2;
        var expectedMaxWrites = maxSwaps * 2;
        Assert.InRange(stats.IndexWriteCount, expectedMinWrites, expectedMaxWrites);
    }

    [Fact]
    public void StabilityTest()
    {
        // Test stability: equal elements should maintain relative order
        // Bubble Sort swaps only when s.Compare(j, j-1) < 0, preserving equal elements' order
        var stats = new StatisticsContext();
        
        // Create items with same value but different original indices
        var items = new[]
        {
            new StabilityTestItem(1, 0),
            new StabilityTestItem(2, 1),
            new StabilityTestItem(1, 2),
            new StabilityTestItem(3, 3),
            new StabilityTestItem(1, 4),
            new StabilityTestItem(2, 5),
        };

        var array = items.ToArray();
        BubbleSort.Sort(array.AsSpan(), stats);

        // Verify sorting correctness - values should be in ascending order
        Assert.Equal([1, 1, 1, 2, 2, 3], array.Select(x => x.Value).ToArray());
        
        // Verify stability: for each group of equal values, original order is preserved
        var value1Indices = array.Where(x => x.Value == 1).Select(x => x.OriginalIndex).ToArray();
        var value2Indices = array.Where(x => x.Value == 2).Select(x => x.OriginalIndex).ToArray();
        var value3Indices = array.Where(x => x.Value == 3).Select(x => x.OriginalIndex).ToArray();
        
        // Value 1 appeared at original indices 0, 2, 4 - should remain in this order
        Assert.Equal([0, 2, 4], value1Indices);
        
        // Value 2 appeared at original indices 1, 5 - should remain in this order
        Assert.Equal([1, 5], value2Indices);
        
        // Value 3 appeared at original index 3
        Assert.Equal([3], value3Indices);
    }

    [Fact]
    public void StabilityTestWithComplex()
    {
        // Test stability with more complex scenario - multiple equal values
        var stats = new StatisticsContext();
        
        var items = new[]
        {
            new StabilityTestItemWithId(5, "A"),
            new StabilityTestItemWithId(2, "B"),
            new StabilityTestItemWithId(5, "C"),
            new StabilityTestItemWithId(2, "D"),
            new StabilityTestItemWithId(8, "E"),
            new StabilityTestItemWithId(2, "F"),
            new StabilityTestItemWithId(5, "G"),
        };

        var array = items.ToArray();
        BubbleSort.Sort(array.AsSpan(), stats);

        // Expected: [2:B, 2:D, 2:F, 5:A, 5:C, 5:G, 8:E]
        // Keys are sorted, and elements with the same key maintain original order
        
        Assert.Equal(2, array[0].Key);
        Assert.Equal("B", array[0].Id);
        
        Assert.Equal(2, array[1].Key);
        Assert.Equal("D", array[1].Id);
        
        Assert.Equal(2, array[2].Key);
        Assert.Equal("F", array[2].Id);
        
        Assert.Equal(5, array[3].Key);
        Assert.Equal("A", array[3].Id);
        
        Assert.Equal(5, array[4].Key);
        Assert.Equal("C", array[4].Id);
        
        Assert.Equal(5, array[5].Key);
        Assert.Equal("G", array[5].Id);
        
        Assert.Equal(8, array[6].Key);
        Assert.Equal("E", array[6].Id);
    }

    [Fact]
    public void StabilityTestWithAllEqual()
    {
        // Edge case: all elements have the same value
        // They should remain in original order
        var stats = new StatisticsContext();
        
        var items = new[]
        {
            new StabilityTestItem(1, 0),
            new StabilityTestItem(1, 1),
            new StabilityTestItem(1, 2),
            new StabilityTestItem(1, 3),
            new StabilityTestItem(1, 4),
        };

        var array = items.ToArray();
        BubbleSort.Sort(array.AsSpan(), stats);

        // All values are 1
        Assert.All(array, item => Assert.Equal(1, item.Value));
        
        // Original order should be preserved: 0, 1, 2, 3, 4
        Assert.Equal([0, 1, 2, 3, 4], array.Select(x => x.OriginalIndex).ToArray());
        
        // For data with all equal elements, no swaps should occur
        Assert.Equal(0UL, stats.SwapCount);
        Assert.Equal(0UL, stats.IndexWriteCount);
    }
}


