using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;

namespace SortLab.Tests;

public class BitonicSortFillTests
{
    [Theory]
    [ClassData(typeof(MockRandomData))]
    [ClassData(typeof(MockNegativePositiveRandomData))]
    [ClassData(typeof(MockNegativeRandomData))]
    [ClassData(typeof(MockReversedData))]
    [ClassData(typeof(MockMountainData))]
    [ClassData(typeof(MockNearlySortedData))]
    [ClassData(typeof(MockSameValuesData))]
    public void SortResultOrderTest(IInputSample<int> inputSample)
    {
        var stats = new StatisticsContext();
        var array = inputSample.Samples.ToArray();
        BitonicSortFill.Sort(array.AsSpan(), stats);

        Assert.Equal((ulong)inputSample.Samples.Length, (ulong)array.Length);

        // Verify sorted order
        for (int i = 0; i < array.Length - 1; i++)
        {
            Assert.True(array[i] <= array[i + 1], $"Array not sorted at index {i}: {array[i]} > {array[i + 1]}");
        }
    }

    [Fact]
    public void NonPowerOfTwoSizes()
    {
        // Test various non-power-of-2 sizes
        int[] sizes = [3, 5, 7, 10, 15, 100, 127, 200, 1000];
        var random = new Random(42);

        foreach (var size in sizes)
        {
            var stats = new StatisticsContext();
            var array = Enumerable.Range(0, size).OrderBy(_ => random.Next()).ToArray();
            var expected = array.OrderBy(x => x).ToArray();

            BitonicSortFill.Sort(array.AsSpan(), stats);

            Assert.Equal(size, array.Length);
            Assert.Equal(expected, array);
        }
    }

    [Fact]
    public void EmptyArray()
    {
        var stats = new StatisticsContext();
        var array = Array.Empty<int>();
        BitonicSortFill.Sort(array.AsSpan(), stats);
        Assert.Empty(array);
    }

    [Fact]
    public void SingleElement()
    {
        var stats = new StatisticsContext();
        var array = new int[] { 42 };
        BitonicSortFill.Sort(array.AsSpan(), stats);
        Assert.Single(array);
        Assert.Equal(42, array[0]);
    }

    [Fact]
    public void TwoElements()
    {
        var stats = new StatisticsContext();
        var array = new int[] { 2, 1 };
        BitonicSortFill.Sort(array.AsSpan(), stats);
        Assert.Equal(2, array.Length);
        Assert.Equal(1, array[0]);
        Assert.Equal(2, array[1]);
    }

    [Fact]
    public void FourElements()
    {
        var stats = new StatisticsContext();
        var array = new int[] { 3, 1, 4, 2 };
        BitonicSortFill.Sort(array.AsSpan(), stats);
        Assert.Equal(new int[] { 1, 2, 3, 4 }, array);
    }

    [Fact]
    public void EightElements()
    {
        var stats = new StatisticsContext();
        var array = new int[] { 5, 2, 8, 1, 9, 3, 7, 4 };
        BitonicSortFill.Sort(array.AsSpan(), stats);
        Assert.Equal(new int[] { 1, 2, 3, 4, 5, 7, 8, 9 }, array);
    }

    [Fact]
    public void SixteenElementsAllSame()
    {
        var stats = new StatisticsContext();
        var array = Enumerable.Repeat(42, 16).ToArray();
        BitonicSortFill.Sort(array.AsSpan(), stats);
        Assert.All(array, x => Assert.Equal(42, x));
    }

    [Fact]
    public void HundredElements()
    {
        var stats = new StatisticsContext();
        var array = Enumerable.Range(0, 100).Reverse().ToArray();
        BitonicSortFill.Sort(array.AsSpan(), stats);
        Assert.Equal(Enumerable.Range(0, 100).ToArray(), array);
    }

#if DEBUG

    [Theory]
    [ClassData(typeof(MockPowerOfTwoSortedData))]
    public void StatisticsSortedTest(IInputSample<int> inputSample)
    {
        var stats = new StatisticsContext();
        var array = inputSample.Samples.ToArray();
        BitonicSortFill.Sort(array.AsSpan(), stats);

        Assert.Equal((ulong)inputSample.Samples.Length, (ulong)array.Length);

        // Bitonic sort has O(n log^2 n) comparisons regardless of input
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
        BitonicSortFill.Sort(sorted.AsSpan(), stats);

        // Bitonic sort performs the same number of comparisons regardless of input order
        // For n = 2^k, the number of comparisons is (k(k+1)/2) * n where k = log2(n)
        var expectedCompares = CalculateBitonicComparisons(n);
        
        // For sorted data, fewer swaps but not necessarily 0 due to data-oblivious nature
        // The bitonic network structure may still swap elements that are already in order
        // Reads: Each comparison reads 2 elements, each swap reads 2 elements
        var expectedReads = expectedCompares * 2 + stats.SwapCount * 2;
        var expectedWrites = stats.SwapCount * 2;

        Assert.Equal(expectedCompares, stats.CompareCount);
        // Note: BitonicSort may perform some swaps even on sorted data
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
        BitonicSortFill.Sort(reversed.AsSpan(), stats);

        // Bitonic sort performs the same number of comparisons regardless of input order
        var expectedCompares = CalculateBitonicComparisons(n);
        
        // For reversed data, many swaps are needed
        // Reads: Compare reads 2 elements, Swap also reads 2 elements before swapping
        // Total reads = (comparisons * 2) + (swaps * 2)
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
        BitonicSortFill.Sort(random.AsSpan(), stats);

        // Bitonic sort always performs the same number of comparisons regardless of input
        var expectedCompares = CalculateBitonicComparisons(n);
        
        // For random data, swap count varies based on disorder
        // Reads: Compare reads 2 elements, Swap also reads 2 elements
        // Total reads = (comparisons * 2) + (swaps * 2)
        var expectedReads = expectedCompares * 2 + stats.SwapCount * 2;
        var expectedWrites = stats.SwapCount * 2;

        Assert.Equal(expectedCompares, stats.CompareCount);
        Assert.True(stats.SwapCount >= 0);
        Assert.Equal(expectedWrites, stats.IndexWriteCount);
        Assert.Equal(expectedReads, stats.IndexReadCount);
    }

    /// <summary>
    /// Calculates the theoretical number of comparisons for Bitonic Sort.
    /// For n = 2^k, the formula is: n * k * (k+1) / 4, where k = log2(n)
    /// This comes from the recursive structure:
    /// - Total number of comparison stages: log n levels
    /// - At level i, we perform n/2 * i comparisons
    /// - Sum: n/2 * (1 + 2 + 3 + ... + log n) = n/2 * log n * (log n + 1) / 2 = n * log n * (log n + 1) / 4
    /// </summary>
    private static ulong CalculateBitonicComparisons(int n)
    {
        if (n <= 1) return 0;
        
        // Calculate next power of 2 if not already
        int paddedN = n;
        if ((n & (n - 1)) != 0)
        {
            paddedN = 1;
            while (paddedN < n) paddedN <<= 1;
        }
        
        int k = 0;
        int temp = paddedN;
        while (temp > 1)
        {
            temp >>= 1;
            k++;
        }
        
        // Formula: n * k * (k+1) / 4
        return (ulong)(paddedN * k * (k + 1) / 4);
    }

#endif
}
