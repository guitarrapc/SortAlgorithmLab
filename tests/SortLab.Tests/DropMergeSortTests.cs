using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;

namespace SortLab.Tests;

public class DropMergeSortTests
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
        DropMergeSort.Sort(array.AsSpan(), stats);

        Assert.Equal((ulong)inputSample.Samples.Length, (ulong)array.Length);

        // Verify the array is sorted
        for (var i = 0; i < array.Length - 1; i++)
        {
            Assert.True(array[i] <= array[i + 1], $"Array not sorted at index {i}: {array[i]} > {array[i + 1]}");
        }
    }

#if DEBUG

    [Theory]
    [ClassData(typeof(MockSortedData))]
    public void StatisticsSortedTest(IInputSample<int> inputSample)
    {
        var stats = new StatisticsContext();
        var array = inputSample.Samples.ToArray();
        DropMergeSort.Sort(array.AsSpan(), stats);

        Assert.Equal((ulong)inputSample.Samples.Length, (ulong)array.Length);
        Assert.NotEqual(0UL, stats.IndexReadCount);
        Assert.Equal(0UL, stats.IndexWriteCount); // Already sorted, no writes needed (optimized away)
        Assert.NotEqual(0UL, stats.CompareCount);
    }

    [Theory]
    [ClassData(typeof(MockRandomData))]
    public void StatisticsRandomTest(IInputSample<int> inputSample)
    {
        var stats = new StatisticsContext();
        var array = inputSample.Samples.ToArray();
        DropMergeSort.Sort(array.AsSpan(), stats);

        Assert.Equal((ulong)inputSample.Samples.Length, (ulong)array.Length);
        Assert.NotEqual(0UL, stats.IndexReadCount);
        Assert.NotEqual(0UL, stats.IndexWriteCount);
        Assert.NotEqual(0UL, stats.CompareCount);
    }

    [Theory]
    [ClassData(typeof(MockReversedData))]
    public void StatisticsReversedTest(IInputSample<int> inputSample)
    {
        var stats = new StatisticsContext();
        var array = inputSample.Samples.ToArray();
        DropMergeSort.Sort(array.AsSpan(), stats);

        Assert.Equal((ulong)inputSample.Samples.Length, (ulong)array.Length);
        Assert.NotEqual(0UL, stats.IndexReadCount);
        Assert.NotEqual(0UL, stats.IndexWriteCount);
        Assert.NotEqual(0UL, stats.CompareCount);
    }

    [Theory]
    [ClassData(typeof(MockNearlySortedData))]
    public void StatisticsNearlySortedTest(IInputSample<int> inputSample)
    {
        var stats = new StatisticsContext();
        var array = inputSample.Samples.ToArray();
        DropMergeSort.Sort(array.AsSpan(), stats);

        Assert.Equal((ulong)inputSample.Samples.Length, (ulong)array.Length);
        Assert.NotEqual(0UL, stats.IndexReadCount);
        Assert.NotEqual(0UL, stats.IndexWriteCount);
        Assert.NotEqual(0UL, stats.CompareCount);
    }

#endif

    [Fact]
    public void EmptyArrayTest()
    {
        var stats = new StatisticsContext();
        var array = Array.Empty<int>();
        DropMergeSort.Sort(array.AsSpan(), stats);

        Assert.Empty(array);
    }

    [Fact]
    public void SingleElementTest()
    {
        var stats = new StatisticsContext();
        var array = new[] { 42 };
        DropMergeSort.Sort(array.AsSpan(), stats);

        Assert.Single(array);
        Assert.Equal(42, array[0]);
    }

    [Fact]
    public void TwoElementsSortedTest()
    {
        var stats = new StatisticsContext();
        var array = new[] { 1, 2 };
        DropMergeSort.Sort(array.AsSpan(), stats);

        Assert.Equal(2, array.Length);
        Assert.Equal(1, array[0]);
        Assert.Equal(2, array[1]);
    }

    [Fact]
    public void TwoElementsReversedTest()
    {
        var stats = new StatisticsContext();
        var array = new[] { 2, 1 };
        DropMergeSort.Sort(array.AsSpan(), stats);

        Assert.Equal(2, array.Length);
        Assert.Equal(1, array[0]);
        Assert.Equal(2, array[1]);
    }

    [Fact]
    public void AlreadySortedTest()
    {
        var stats = new StatisticsContext();
        var array = new[] { 1, 2, 3, 4, 5 };
        DropMergeSort.Sort(array.AsSpan(), stats);

        Assert.Equal(5, array.Length);
        Assert.Equal(new[] { 1, 2, 3, 4, 5 }, array);
    }

    [Fact]
    public void ReverseSortedTest()
    {
        var stats = new StatisticsContext();
        var array = new[] { 5, 4, 3, 2, 1 };
        DropMergeSort.Sort(array.AsSpan(), stats);

        Assert.Equal(5, array.Length);
        Assert.Equal(new[] { 1, 2, 3, 4, 5 }, array);
    }

    [Fact]
    public void SingleOutlierTest()
    {
        // Test the "quick undo" optimization path
        var stats = new StatisticsContext();
        var array = new[] { 0, 1, 2, 3, 9, 5, 6, 7 };
        DropMergeSort.Sort(array.AsSpan(), stats);

        Assert.Equal(8, array.Length);
        Assert.Equal(new[] { 0, 1, 2, 3, 5, 6, 7, 9 }, array);
    }

    [Fact]
    public void NearlySortedWithFewOutliersTest()
    {
        var stats = new StatisticsContext();
        var array = new[] { 1, 2, 15, 3, 4, 5, 20, 6, 7, 8, 9, 10 };
        DropMergeSort.Sort(array.AsSpan(), stats);

        Assert.Equal(12, array.Length);
        Assert.Equal(new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 15, 20 }, array);
    }
}
