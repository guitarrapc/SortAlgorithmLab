using SortLab.Core.Algorithms;
using SortLab.Core.Contexts;

namespace SortLab.Tests;

// Tests using Ciura2001 as it's considered one of the best gap sequences
public class ShellSortTests
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
        ShellSortCiura2001.Sort(array.AsSpan());
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
        ShellSortCiura2001.Sort(array.AsSpan(), stats);

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
        ShellSortCiura2001.Sort(array.AsSpan(), stats);

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
        ShellSortCiura2001.Sort(array.AsSpan(), stats);

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
        ShellSortCiura2001.Sort(sorted.AsSpan(), stats);

        // Shell Sort with sorted data:
        // - No swaps needed (all elements already in correct positions)
        // - Comparisons depend on gap sequence, but final h=1 pass requires at least n-1 comparisons
        // - Each comparison reads 2 elements
        var expectedSwaps = 0UL;
        var expectedWrites = 0UL; // No swaps = no writes
        var minCompares = (ulong)(n - 1); // Final h=1 pass minimum

        Assert.Equal(expectedSwaps, stats.SwapCount);
        Assert.Equal(expectedWrites, stats.IndexWriteCount);
        Assert.True(stats.CompareCount >= minCompares,
            $"CompareCount ({stats.CompareCount}) should be >= {minCompares}");
        
        // Each comparison reads 2 elements, each swap also reads 2 elements
        var expectedReads = stats.CompareCount * 2 + stats.SwapCount * 2;
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
        ShellSortCiura2001.Sort(reversed.AsSpan(), stats);

        // Shell Sort with reversed data (worst case):
        // - Gap sequence determines exact behavior
        // - With Knuth sequence (1, 4, 13, 40, ...):
        //   * Many swaps needed at each gap level
        //   * Comparisons: O(n^1.5) typically
        //   * Swaps: O(n^1.5) typically
        // - Each swap writes 2 elements
        var minSwaps = 1UL; // At least 1 swap is needed
        var maxSwaps = (ulong)(n * n); // Upper bound (pessimistic)
        var minCompares = (ulong)n; // At least n comparisons
        
        Assert.InRange(stats.SwapCount, minSwaps, maxSwaps);
        Assert.True(stats.CompareCount >= minCompares,
            $"CompareCount ({stats.CompareCount}) should be >= {minCompares}");
        
        // Each swap writes 2 elements
        var expectedWrites = stats.SwapCount * 2;
        Assert.Equal(expectedWrites, stats.IndexWriteCount);
        
        // Each comparison reads 2 elements, each swap also reads 2 elements
        var expectedReads = stats.CompareCount * 2 + stats.SwapCount * 2;
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
        ShellSortCiura2001.Sort(random.AsSpan(), stats);

        // Shell Sort with random data (average case):
        // - Gap sequence determines performance
        // - With good gap sequences (Knuth, Tokuda, Ciura):
        //   * Comparisons: O(n log n) to O(n^1.3)
        //   * Swaps: Similar to comparisons
        // - Performance is better than O(n^2) but depends on gap sequence
        var minSwaps = 0UL; // Could be sorted by chance
        var maxSwaps = (ulong)(n * n); // Upper bound
        var minCompares = (ulong)(n - 1); // At least n-1 comparisons in final pass
        
        Assert.InRange(stats.SwapCount, minSwaps, maxSwaps);
        Assert.True(stats.CompareCount >= minCompares,
            $"CompareCount ({stats.CompareCount}) should be >= {minCompares}");
        
        // Each swap writes 2 elements
        var expectedWrites = stats.SwapCount * 2;
        Assert.Equal(expectedWrites, stats.IndexWriteCount);
        
        // Each comparison reads 2 elements, each swap also reads 2 elements
        var expectedReads = stats.CompareCount * 2 + stats.SwapCount * 2;
        Assert.Equal(expectedReads, stats.IndexReadCount);
    }

    [Theory]
    [InlineData(10)]
    [InlineData(20)]
    [InlineData(50)]
    [InlineData(100)]
    public void VerifyConsistencyOfStatistics(int n)
    {
        var stats = new StatisticsContext();
        var random = Enumerable.Range(0, n).OrderBy(_ => Guid.NewGuid()).ToArray();
        ShellSortCiura2001.Sort(random.AsSpan(), stats);

        // Verify relationships between statistics:
        // 1. IndexWriteCount = SwapCount * 2 (each swap writes 2 elements)
        // 2. IndexReadCount = CompareCount * 2 + SwapCount * 2 (each comparison reads 2 elements, each swap also reads 2 elements)
        
        var expectedWrites = stats.SwapCount * 2;
        Assert.Equal(expectedWrites, stats.IndexWriteCount);
        
        // Each comparison reads 2 elements
        // Each swap also reads 2 elements before swapping
        // So total reads = CompareCount * 2 + SwapCount * 2
        var expectedReads = stats.CompareCount * 2 + stats.SwapCount * 2;
        Assert.Equal(expectedReads, stats.IndexReadCount);
    }

    [Theory]
    [InlineData(10)]
    [InlineData(20)]
    [InlineData(50)]
    [InlineData(100)]
    public void VerifyGapSequenceEfficiency(int n)
    {
        var stats = new StatisticsContext();
        var random = Enumerable.Range(0, n).OrderBy(_ => Guid.NewGuid()).ToArray();
        ShellSortCiura2001.Sort(random.AsSpan(), stats);

        // Shell Sort with good gap sequences should perform much better than O(n^2)
        // For random data with Knuth/Ciura sequence:
        // - Expected comparisons: O(n^1.3) to O(n^1.5)
        // - This test verifies we're significantly better than naive O(n^2)
        
        // Bubble sort would do n^2/2 comparisons in average case
        var bubbleSortWorstCase = (ulong)(n * n / 2);
        
        // Shell Sort should use significantly fewer comparisons
        // We expect at least 50% improvement for n >= 20
        var maxExpectedCompares = bubbleSortWorstCase / 2;
        
        // For small n, the overhead might not show improvement
        if (n >= 20)
        {
            Assert.True(stats.CompareCount < maxExpectedCompares,
                $"CompareCount ({stats.CompareCount}) should be < {maxExpectedCompares} for efficient gap sequence");
        }
    }

    [Theory]
    [InlineData(5)]
    [InlineData(10)]
    [InlineData(15)]
    public void VerifyStatisticsAccuracy(int n)
    {
        var stats = new StatisticsContext();
        var data = Enumerable.Range(0, n).OrderBy(_ => Guid.NewGuid()).ToArray();
        ShellSortCiura2001.Sort(data.AsSpan(), stats);

        // Verify that the array is actually sorted
        Assert.Equal(Enumerable.Range(0, n), data);

        // Verify statistics are non-zero for random data
        Assert.True(stats.CompareCount > 0, "CompareCount should be > 0");
        Assert.True(stats.IndexReadCount > 0, "IndexReadCount should be > 0");
        
        // For random data, we expect some swaps (unless by chance it's already sorted)
        // But we can't guarantee swaps, so we don't assert on SwapCount/IndexWriteCount
        
        // Verify consistency: each swap writes 2 elements
        Assert.Equal(stats.SwapCount * 2, stats.IndexWriteCount);
        
        // Verify that reads account for both comparisons and swaps
        // Each comparison reads 2 elements, each swap also reads 2 elements
        var expectedReads = stats.CompareCount * 2 + stats.SwapCount * 2;
        Assert.Equal(expectedReads, stats.IndexReadCount);
    }

    [Fact]
    public void EmptyArrayTest()
    {
        var stats = new StatisticsContext();
        var empty = Array.Empty<int>();
        ShellSortCiura2001.Sort(empty.AsSpan(), stats);

        Assert.Empty(empty);
        Assert.Equal(0UL, stats.CompareCount);
        Assert.Equal(0UL, stats.SwapCount);
        Assert.Equal(0UL, stats.IndexReadCount);
        Assert.Equal(0UL, stats.IndexWriteCount);
    }

    [Fact]
    public void SingleElementArrayTest()
    {
        var stats = new StatisticsContext();
        var single = new[] { 42 };
        ShellSortCiura2001.Sort(single.AsSpan(), stats);

        Assert.Equal(new[] { 42 }, single);
        Assert.Equal(0UL, stats.CompareCount);
        Assert.Equal(0UL, stats.SwapCount);
        Assert.Equal(0UL, stats.IndexReadCount);
        Assert.Equal(0UL, stats.IndexWriteCount);
    }

    [Fact]
    public void TwoElementsSortedTest()
    {
        var stats = new StatisticsContext();
        var two = new[] { 1, 2 };
        ShellSortCiura2001.Sort(two.AsSpan(), stats);

        Assert.Equal(new[] { 1, 2 }, two);
        Assert.Equal(1UL, stats.CompareCount); // Only 1 comparison needed
        Assert.Equal(0UL, stats.SwapCount); // No swap needed
        Assert.Equal(2UL, stats.IndexReadCount); // 1 comparison reads 2 elements
        Assert.Equal(0UL, stats.IndexWriteCount); // No writes
    }

    [Fact]
    public void TwoElementsReversedTest()
    {
        var stats = new StatisticsContext();
        var two = new[] { 2, 1 };
        ShellSortCiura2001.Sort(two.AsSpan(), stats);

        Assert.Equal(new[] { 1, 2 }, two);
        Assert.Equal(1UL, stats.CompareCount); // Only 1 comparison needed
        Assert.Equal(1UL, stats.SwapCount); // 1 swap needed
        // 1 comparison reads 2 elements + 1 swap reads 2 elements = 4 reads total
        Assert.Equal(4UL, stats.IndexReadCount);
        Assert.Equal(2UL, stats.IndexWriteCount); // 1 swap writes 2 elements
    }
}
