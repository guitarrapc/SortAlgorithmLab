using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;

namespace SortAlgorithm.Tests;

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
        BitonicSort.Sort(sorted.AsSpan(), stats);

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
        BitonicSort.Sort(reversed.AsSpan(), stats);

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
        BitonicSort.Sort(random.AsSpan(), stats);

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

        // n must be a power of 2 for BitonicSort
        int k = 0;
        int temp = n;
        while (temp > 1)
        {
            temp >>= 1;
            k++;
        }

        // Formula: n * k * (k+1) / 4
        return (ulong)(n * k * (k + 1) / 4);
    }

#endif
}
