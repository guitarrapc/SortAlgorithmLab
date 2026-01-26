using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;

namespace SortLab.Tests;

public class BitonicSortTests
{
    [Theory]
    [ClassData(typeof(MockPowerOfTwoRandomData))]
    [ClassData(typeof(MockPowerOfTwoNegativePositiveRandomData))]
    [ClassData(typeof(MockPowerOfTwoReversedData))]
    [ClassData(typeof(MockPowerOfTwoNearlySortedData))]
    [ClassData(typeof(MockPowerOfTwoSameValuesData))]
    public void SortResultOrderTest(IInputSample<int> inputSample)
    {
        var stats = new StatisticsContext();
        var array = inputSample.Samples.ToArray();
        BitonicSort.Sort(array.AsSpan(), stats);

        Assert.Equal((ulong)inputSample.Samples.Length, (ulong)array.Length);
        
        // Verify sorted order
        for (int i = 0; i < array.Length - 1; i++)
        {
            Assert.True(array[i] <= array[i + 1], $"Array not sorted at index {i}: {array[i]} > {array[i + 1]}");
        }
    }

    [Fact]
    public void ThrowsOnNonPowerOfTwo()
    {
        var stats = new StatisticsContext();
        var array = new int[] { 3, 1, 4, 1, 5, 9, 2 }; // Length 7 is not power of 2

        Assert.Throws<ArgumentException>(() => BitonicSort.Sort(array.AsSpan(), stats));
    }

    [Fact]
    public void EmptyArray()
    {
        var stats = new StatisticsContext();
        var array = Array.Empty<int>();
        BitonicSort.Sort(array.AsSpan(), stats);
        Assert.Empty(array);
    }

    [Fact]
    public void SingleElement()
    {
        var stats = new StatisticsContext();
        var array = new int[] { 42 };
        BitonicSort.Sort(array.AsSpan(), stats);
        Assert.Single(array);
        Assert.Equal(42, array[0]);
    }

    [Fact]
    public void TwoElements()
    {
        var stats = new StatisticsContext();
        var array = new int[] { 2, 1 };
        BitonicSort.Sort(array.AsSpan(), stats);
        Assert.Equal(2, array.Length);
        Assert.Equal(1, array[0]);
        Assert.Equal(2, array[1]);
    }

    [Fact]
    public void FourElements()
    {
        var stats = new StatisticsContext();
        var array = new int[] { 3, 1, 4, 2 };
        BitonicSort.Sort(array.AsSpan(), stats);
        Assert.Equal(new int[] { 1, 2, 3, 4 }, array);
    }

    [Fact]
    public void EightElements()
    {
        var stats = new StatisticsContext();
        var array = new int[] { 5, 2, 8, 1, 9, 3, 7, 4 };
        BitonicSort.Sort(array.AsSpan(), stats);
        Assert.Equal(new int[] { 1, 2, 3, 4, 5, 7, 8, 9 }, array);
    }

    [Fact]
    public void SixteenElementsAllSame()
    {
        var stats = new StatisticsContext();
        var array = Enumerable.Repeat(42, 16).ToArray();
        BitonicSort.Sort(array.AsSpan(), stats);
        Assert.All(array, x => Assert.Equal(42, x));
    }

#if DEBUG

    [Theory]
    [ClassData(typeof(MockPowerOfTwoSortedData))]
    public void StatisticsSortedTest(IInputSample<int> inputSample)
    {
        var stats = new StatisticsContext();
        var array = inputSample.Samples.ToArray();
        BitonicSort.Sort(array.AsSpan(), stats);

        Assert.Equal((ulong)inputSample.Samples.Length, (ulong)array.Length);

        // Bitonic sort has O(n log^2 n) comparisons regardless of input
        Assert.True(stats.CompareCount > 0);
        Assert.True(stats.IndexReadCount > 0);
    }

    [Theory]
    [ClassData(typeof(MockPowerOfTwoRandomData))]
    public void StatisticsRandomTest(IInputSample<int> inputSample)
    {
        var stats = new StatisticsContext();
        var array = inputSample.Samples.ToArray();
        BitonicSort.Sort(array.AsSpan(), stats);

        Assert.Equal((ulong)inputSample.Samples.Length, (ulong)array.Length);

        // Bitonic sort should perform comparisons and swaps
        Assert.True(stats.CompareCount > 0);
        Assert.True(stats.SwapCount >= 0);
        Assert.True(stats.IndexReadCount > 0);
        Assert.True(stats.IndexWriteCount >= 0);
    }

    [Theory]
    [ClassData(typeof(MockPowerOfTwoReversedData))]
    public void StatisticsReversedTest(IInputSample<int> inputSample)
    {
        var stats = new StatisticsContext();
        var array = inputSample.Samples.ToArray();
        BitonicSort.Sort(array.AsSpan(), stats);

        Assert.Equal((ulong)inputSample.Samples.Length, (ulong)array.Length);

        // Bitonic sort worst case - many swaps needed
        Assert.True(stats.CompareCount > 0);
        Assert.True(stats.SwapCount > 0);
        Assert.True(stats.IndexReadCount > 0);
        Assert.True(stats.IndexWriteCount > 0);
    }

#endif
}
