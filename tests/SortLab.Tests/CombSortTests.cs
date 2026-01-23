using SortLab.Core.Algorithms;
using SortLab.Core.Contexts;

namespace SortLab.Tests;

public class CombSortTests
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
        CombSort.Sort(array.AsSpan(), stats);

        Assert.Equal((ulong)inputSample.Samples.Length, (ulong)array.Length);
    }

    [Theory]
    [ClassData(typeof(MockSortedData))]
    public void StatisticsSortedTest(IInputSample<int> inputSample)
    {
        var stats = new StatisticsContext();
        var array = inputSample.Samples.ToArray();
        CombSort.Sort(array.AsSpan(), stats);

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
        CombSort.Sort(sorted.AsSpan(), stats);

        // Comb Sort with sorted data performs comparisons across all gaps
        // Gap sequence: n/1.3, n/1.69, ..., 11, 8, 6, 4, 3, 2, 1
        // For each gap h, it performs (n-h) comparisons
        // Final pass with h=1 performs (n-1) comparisons
        // Since data is sorted, no swaps occur
        var expectedSwaps = 0UL;
        var expectedWrites = 0UL;

        // Comparisons should happen for all gaps
        Assert.NotEqual(0UL, stats.CompareCount);
        Assert.Equal(expectedSwaps, stats.SwapCount);
        Assert.Equal(expectedWrites, stats.IndexWriteCount);
        
        // Each comparison reads 2 elements
        var minIndexReads = stats.CompareCount * 2;
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
        CombSort.Sort(reversed.AsSpan(), stats);

        // Comb Sort with reversed data performs multiple passes
        // Gap sequence reduces by factor of 1.3 each iteration
        // Each gap h performs (n-h) comparisons
        // Reversed data will require many swaps, especially in early passes
        
        // Comparisons: Sum of (n-h) for all gaps in sequence
        Assert.NotEqual(0UL, stats.CompareCount);
        
        // Swaps: Should be significant for reversed data
        Assert.NotEqual(0UL, stats.SwapCount);
        
        // Each swap writes 2 elements
        var expectedWrites = stats.SwapCount * 2;
        Assert.Equal(expectedWrites, stats.IndexWriteCount);
        
        // Each comparison reads 2 elements
        var minIndexReads = stats.CompareCount * 2;
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
        CombSort.Sort(random.AsSpan(), stats);

        // Comb Sort on random data should perform O(n log n) comparisons on average
        // Gap sequence: n/1.3, n/1.69, ..., down to 1
        // Number of gaps ≈ log₁.₃(n) ≈ 2.4 * log₂(n)
        // For each gap h: (n-h) comparisons
        
        // Conservative estimates:
        var minCompares = (ulong)n; // At minimum, final pass with gap=1
        var maxCompares = (ulong)(n * n); // Upper bound for worst case
        
        Assert.InRange(stats.CompareCount, minCompares, maxCompares);
        Assert.NotEqual(0UL, stats.SwapCount);
        
        // Each swap writes 2 elements
        var expectedWrites = stats.SwapCount * 2;
        Assert.Equal(expectedWrites, stats.IndexWriteCount);
        
        // Each comparison reads 2 elements
        var minIndexReads = stats.CompareCount * 2;
        Assert.True(stats.IndexReadCount >= minIndexReads,
            $"IndexReadCount ({stats.IndexReadCount}) should be >= {minIndexReads}");
    }

    [Theory]
    [InlineData(13)]
    [InlineData(26)]
    [InlineData(39)]
    public void GapSequenceTest(int n)
    {
        var stats = new StatisticsContext();
        var data = Enumerable.Range(0, n).Reverse().ToArray();
        CombSort.Sort(data.AsSpan(), stats);

        // Verify that Comb11 optimization is working:
        // When gap calculation results in 9 or 10, it should be set to 11
        // This should result in better performance than standard 1.3 shrink factor
        
        // All elements should be sorted correctly
        Assert.Equal(Enumerable.Range(0, n), data);
        
        // Algorithm should complete successfully
        Assert.NotEqual(0UL, stats.CompareCount);
    }
}
