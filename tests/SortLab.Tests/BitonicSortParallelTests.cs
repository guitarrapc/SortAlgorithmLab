using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;

namespace SortLab.Tests;

public class BitonicSortParallelTests
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
        BitonicSortParallel.Sort(array, stats);

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

        Assert.Throws<ArgumentException>(() => BitonicSortParallel.Sort(array, stats));
    }

    [Fact]
    public void ThrowsOnNullArray()
    {
        var stats = new StatisticsContext();
        int[]? array = null;

        Assert.Throws<ArgumentNullException>(() => BitonicSortParallel.Sort(array!, stats));
    }

    [Fact]
    public void ThrowsOnNullContext()
    {
        var array = new int[] { 1, 2, 3, 4 };

        Assert.Throws<ArgumentNullException>(() => BitonicSortParallel.Sort(array, null!));
    }

    [Fact]
    public void EmptyArray()
    {
        var stats = new StatisticsContext();
        var array = Array.Empty<int>();
        BitonicSortParallel.Sort(array, stats);
        Assert.Empty(array);
    }

    [Fact]
    public void SingleElement()
    {
        var stats = new StatisticsContext();
        var array = new int[] { 42 };
        BitonicSortParallel.Sort(array, stats);
        Assert.Single(array);
        Assert.Equal(42, array[0]);
    }

    [Fact]
    public void TwoElements()
    {
        var stats = new StatisticsContext();
        var array = new int[] { 2, 1 };
        BitonicSortParallel.Sort(array, stats);
        Assert.Equal(2, array.Length);
        Assert.Equal(1, array[0]);
        Assert.Equal(2, array[1]);
    }

    [Fact]
    public void FourElements()
    {
        var stats = new StatisticsContext();
        var array = new int[] { 3, 1, 4, 2 };
        BitonicSortParallel.Sort(array, stats);
        Assert.Equal(new int[] { 1, 2, 3, 4 }, array);
    }

    [Fact]
    public void EightElements()
    {
        var stats = new StatisticsContext();
        var array = new int[] { 5, 2, 8, 1, 9, 3, 7, 4 };
        BitonicSortParallel.Sort(array, stats);
        Assert.Equal(new int[] { 1, 2, 3, 4, 5, 7, 8, 9 }, array);
    }

    [Fact]
    public void SixteenElementsAllSame()
    {
        var stats = new StatisticsContext();
        var array = Enumerable.Repeat(42, 16).ToArray();
        BitonicSortParallel.Sort(array, stats);
        Assert.All(array, x => Assert.Equal(42, x));
    }

    [Fact]
    public void LargeArrayParallelization()
    {
        // Test with array size >= PARALLEL_THRESHOLD (1024)
        var stats = new StatisticsContext();
        var array = Enumerable.Range(0, 2048).Reverse().ToArray();
        BitonicSortParallel.Sort(array, stats);
        
        Assert.Equal(Enumerable.Range(0, 2048).ToArray(), array);
    }

    [Fact]
    public void VeryLargeArray()
    {
        // Test with a very large power-of-2 array to ensure parallelization works
        var stats = new StatisticsContext();
        var random = new Random(42);
        var array = Enumerable.Range(0, 4096).OrderBy(_ => random.Next()).ToArray();
        var expected = array.OrderBy(x => x).ToArray();
        
        BitonicSortParallel.Sort(array, stats);
        
        Assert.Equal(expected, array);
    }

#if DEBUG

    [Theory]
    [ClassData(typeof(MockPowerOfTwoSortedData))]
    public void StatisticsSortedTest(IInputSample<int> inputSample)
    {
        var stats = new StatisticsContext();
        var array = inputSample.Samples.ToArray();
        BitonicSortParallel.Sort(array, stats);

        Assert.Equal((ulong)inputSample.Samples.Length, (ulong)array.Length);
        Assert.True(stats.CompareCount > 0);
        Assert.True(stats.IndexReadCount > 0);
    }

    [Theory]
    [InlineData(2)]
    [InlineData(4)]
    [InlineData(8)]
    [InlineData(16)]
    [InlineData(32)]
    [InlineData(64)]
    [InlineData(128)]
    public void TheoreticalValuesSortedTest(int n)
    {
        var stats = new StatisticsContext();
        var sorted = Enumerable.Range(0, n).ToArray();
        BitonicSortParallel.Sort(sorted, stats);

        // Bitonic sort performs the same number of comparisons regardless of input order
        var expectedCompares = CalculateBitonicComparisons(n);
        
        // For sorted data, swaps may still occur due to data-oblivious nature
        var expectedReads = expectedCompares * 2 + stats.SwapCount * 2;
        var expectedWrites = stats.SwapCount * 2;

        Assert.Equal(expectedCompares, stats.CompareCount);
        Assert.True(stats.SwapCount >= 0);
        Assert.Equal(expectedWrites, stats.IndexWriteCount);
        Assert.Equal(expectedReads, stats.IndexReadCount);
    }

    [Theory]
    [InlineData(2)]
    [InlineData(4)]
    [InlineData(8)]
    [InlineData(16)]
    [InlineData(32)]
    [InlineData(64)]
    [InlineData(128)]
    public void TheoreticalValuesReversedTest(int n)
    {
        var stats = new StatisticsContext();
        var reversed = Enumerable.Range(0, n).Reverse().ToArray();
        BitonicSortParallel.Sort(reversed, stats);

        var expectedCompares = CalculateBitonicComparisons(n);
        var expectedReads = expectedCompares * 2 + stats.SwapCount * 2;
        var expectedWrites = stats.SwapCount * 2;

        Assert.Equal(expectedCompares, stats.CompareCount);
        Assert.True(stats.SwapCount > 0, "Reversed array should require swaps");
        Assert.Equal(expectedWrites, stats.IndexWriteCount);
        Assert.Equal(expectedReads, stats.IndexReadCount);
    }

    [Theory]
    [InlineData(2)]
    [InlineData(4)]
    [InlineData(8)]
    [InlineData(16)]
    [InlineData(32)]
    [InlineData(64)]
    [InlineData(128)]
    public void TheoreticalValuesRandomTest(int n)
    {
        var stats = new StatisticsContext();
        var random = Enumerable.Range(0, n).OrderBy(_ => Guid.NewGuid()).ToArray();
        BitonicSortParallel.Sort(random, stats);

        var expectedCompares = CalculateBitonicComparisons(n);
        var expectedReads = expectedCompares * 2 + stats.SwapCount * 2;
        var expectedWrites = stats.SwapCount * 2;

        Assert.Equal(expectedCompares, stats.CompareCount);
        Assert.True(stats.SwapCount >= 0);
        Assert.Equal(expectedWrites, stats.IndexWriteCount);
        Assert.Equal(expectedReads, stats.IndexReadCount);
    }

    private static ulong CalculateBitonicComparisons(int n)
    {
        if (n <= 1) return 0;
        
        int k = 0;
        int temp = n;
        while (temp > 1)
        {
            temp >>= 1;
            k++;
        }
        
        return (ulong)(n * k * (k + 1) / 4);
    }

#endif
}
