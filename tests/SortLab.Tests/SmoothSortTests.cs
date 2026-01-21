using SortLab.Core.Algorithms;
using SortLab.Core.Contexts;

namespace SortLab.Tests;

public class SmoothSortTests
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
        SmoothSort.Sort(array.AsSpan());
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
        SmoothSort.Sort(array.AsSpan(), stats);

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
        SmoothSort.Sort(array.AsSpan(), stats);

        Assert.Equal((ulong)inputSample.Samples.Length, (ulong)array.Length);
        Assert.NotEqual(0UL, stats.IndexReadCount);
        Assert.Equal(0UL, stats.IndexWriteCount); // Sorted data requires no writes
        Assert.NotEqual(0UL, stats.CompareCount);
        Assert.Equal(0UL, stats.SwapCount); // Sorted data requires no swaps
    }

    [Theory]
    [ClassData(typeof(MockRandomData))]
    public void StatisticsResetTest(IInputSample<int> inputSample)
    {
        var stats = new StatisticsContext();
        var array = inputSample.Samples.ToArray();
        SmoothSort.Sort(array.AsSpan(), stats);

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
        SmoothSort.Sort(sorted.AsSpan(), stats);

        // Smooth Sort achieves O(n) for sorted data
        // It should perform comparisons proportional to n (not n log n)
        // Best case: approximately n comparisons
        // Each comparison involves reading elements
        var minCompares = (ulong)n;
        var maxCompares = (ulong)(n * Math.Log(n, 2) * 2); // Allow some overhead

        Assert.InRange(stats.CompareCount, minCompares, maxCompares);
        Assert.True(stats.IndexReadCount >= stats.CompareCount,
            $"IndexReadCount ({stats.IndexReadCount}) should be >= CompareCount ({stats.CompareCount})");
        
        // Sorted data should have minimal operations
        Assert.True(stats.CompareCount < (ulong)(n * n / 2),
            $"Sorted data should be O(n), not O(n²). CompareCount: {stats.CompareCount}, n²/2: {n * n / 2}");
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
        SmoothSort.Sort(reversed.AsSpan(), stats);

        // Smooth Sort has O(n log n) worst case
        // For reversed data, it should perform more comparisons than sorted
        var minCompares = (ulong)(n * Math.Log(n, 2));
        var maxCompares = (ulong)(n * Math.Log(n, 2) * 4); // Allow overhead for Leonardo heap operations

        Assert.InRange(stats.CompareCount, minCompares, maxCompares);
        Assert.NotEqual(0UL, stats.SwapCount);
        Assert.True(stats.IndexReadCount >= stats.CompareCount,
            $"IndexReadCount ({stats.IndexReadCount}) should be >= CompareCount ({stats.CompareCount})");
        Assert.True(stats.IndexWriteCount >= stats.SwapCount * 2,
            $"IndexWriteCount ({stats.IndexWriteCount}) should be >= SwapCount * 2 ({stats.SwapCount * 2})");
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
        SmoothSort.Sort(random.AsSpan(), stats);

        // Smooth Sort has O(n log n) average case
        // Random data should fall between best (O(n)) and worst (O(n log n)) cases
        var minCompares = (ulong)n; // Best case
        var maxCompares = (ulong)(n * Math.Log(n, 2) * 4); // Worst case with overhead

        Assert.InRange(stats.CompareCount, minCompares, maxCompares);
        Assert.True(stats.IndexReadCount >= stats.CompareCount,
            $"IndexReadCount ({stats.IndexReadCount}) should be >= CompareCount ({stats.CompareCount})");
    }
}
