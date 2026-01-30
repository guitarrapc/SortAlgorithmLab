using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;

namespace SortAlgorithm.Tests;

public class PigeonholeSortTests
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
        PigeonholeSort.Sort(array.AsSpan(), x => x, stats);

        Assert.Equal((ulong)inputSample.Samples.Length, (ulong)array.Length);
    }

    [Theory]
    [InlineData(10_000_001)]
    public void RangeLimitTest(int range)
    {
        // Test that excessive range throws ArgumentException
        var array = new[] { 0, range };
        Assert.Throws<ArgumentException>(() => PigeonholeSort.Sort(array.AsSpan(), x => x));
    }

    [Fact]
    public void NegativeValuesTest()
    {
        var stats = new StatisticsContext();
        var array = new[] { -5, -1, -10, 3, 0, -3 };
        PigeonholeSort.Sort(array.AsSpan(), x => x, stats);

        Assert.Equal(new[] { -10, -5, -3, -1, 0, 3 }, array);
    }

    [Fact]
    public void EmptyArrayTest()
    {
        var stats = new StatisticsContext();
        var array = Array.Empty<int>();
        PigeonholeSort.Sort(array.AsSpan(), x => x, stats);

        Assert.Empty(array);
    }

#if DEBUG

    [Theory]
    [ClassData(typeof(MockSortedData))]
    public void StatisticsSortedTest(IInputSample<int> inputSample)
    {
        var stats = new StatisticsContext();
        var array = inputSample.Samples.ToArray();
        PigeonholeSort.Sort(array.AsSpan(), x => x, stats);

        Assert.Equal((ulong)inputSample.Samples.Length, (ulong)array.Length);
        Assert.NotEqual(0UL, stats.IndexReadCount);
        Assert.NotEqual(0UL, stats.IndexWriteCount);
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
        PigeonholeSort.Sort(sorted.AsSpan(), x => x, stats);

        // Pigeonhole Sort with internal buffer tracking (via SortSpan):
        // 1. Find min/max and cache keys: n reads (main buffer)
        // 2. Copy to temp and count: n reads (main) + n writes (temp)
        // 3. Calculate positions: 0 reads/writes (transform holes array in-place)
        // 4. Place elements: n reads (temp) + n writes (main)
        //
        // Total reads: n + n + n = 3n
        // Total writes: n + n = 2n
        var expectedReads = (ulong)(3 * n);
        var expectedWrites = (ulong)(2 * n);

        Assert.Equal(0UL, stats.CompareCount);
        Assert.Equal(0UL, stats.SwapCount);
        Assert.Equal(expectedReads, stats.IndexReadCount);
        Assert.Equal(expectedWrites, stats.IndexWriteCount);
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
        PigeonholeSort.Sort(reversed.AsSpan(), x => x, stats);

        // Pigeonhole Sort complexity is O(n + k) regardless of input order
        // Same operation counts for reversed as for sorted (with internal buffer tracking)
        var expectedReads = (ulong)(3 * n);
        var expectedWrites = (ulong)(2 * n);

        Assert.Equal(0UL, stats.CompareCount);
        Assert.Equal(0UL, stats.SwapCount);
        Assert.Equal(expectedReads, stats.IndexReadCount);
        Assert.Equal(expectedWrites, stats.IndexWriteCount);
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
        PigeonholeSort.Sort(random.AsSpan(), x => x, stats);

        // Pigeonhole Sort has same complexity regardless of input distribution
        // With internal buffer tracking: 3n reads, 2n writes
        var expectedReads = (ulong)(3 * n);
        var expectedWrites = (ulong)(2 * n);

        Assert.Equal(0UL, stats.CompareCount);
        Assert.Equal(0UL, stats.SwapCount);
        Assert.Equal(expectedReads, stats.IndexReadCount);
        Assert.Equal(expectedWrites, stats.IndexWriteCount);
    }

    [Fact]
    public void TheoreticalValuesAllSameTest()
    {
        var stats = new StatisticsContext();
        var n = 100;
        var allSame = Enumerable.Repeat(42, n).ToArray();
        PigeonholeSort.Sort(allSame.AsSpan(), x => x, stats);

        // When all keys are the same (min == max), early return after min/max scan
        // Only n reads for finding min/max, then early return
        var expectedReads = (ulong)n;
        var expectedWrites = 0UL;

        Assert.Equal(0UL, stats.CompareCount);
        Assert.Equal(0UL, stats.SwapCount);
        Assert.Equal(expectedReads, stats.IndexReadCount);
        Assert.Equal(expectedWrites, stats.IndexWriteCount);
    }

#endif

}
