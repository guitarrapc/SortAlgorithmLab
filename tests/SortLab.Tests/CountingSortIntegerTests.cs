using SortLab.Core.Algorithms;
using SortLab.Core.Contexts;

namespace SortLab.Tests;

public class CountingSortIntegerTests
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
        CountingSortInteger.Sort(array.AsSpan());
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
        CountingSortInteger.Sort(array.AsSpan(), stats);

        Assert.Equal((ulong)inputSample.Samples.Length, (ulong)array.Length);
        Assert.NotEqual(0UL, stats.IndexReadCount);
        Assert.NotEqual(0UL, stats.IndexWriteCount);
        Assert.Equal(0UL, stats.CompareCount); // Counting sort doesn't compare
        Assert.Equal(0UL, stats.SwapCount);
    }

    [Theory]
    [ClassData(typeof(MockSortedData))]
    public void StatisticsSortedTest(IInputSample<int> inputSample)
    {
        var stats = new StatisticsContext();
        var array = inputSample.Samples.ToArray();
        CountingSortInteger.Sort(array.AsSpan(), stats);

        Assert.Equal((ulong)inputSample.Samples.Length, (ulong)array.Length);
        Assert.NotEqual(0UL, stats.IndexReadCount);
        Assert.NotEqual(0UL, stats.IndexWriteCount);
        Assert.Equal(0UL, stats.CompareCount);
        Assert.Equal(0UL, stats.SwapCount);
    }

    [Theory]
    [ClassData(typeof(MockRandomData))]
    public void StatisticsResetTest(IInputSample<int> inputSample)
    {
        var stats = new StatisticsContext();
        var array = inputSample.Samples.ToArray();
        CountingSortInteger.Sort(array.AsSpan(), stats);

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
        CountingSortInteger.Sort(sorted.AsSpan(), stats);

        // CountingSortInteger with temp buffer tracking:
        // 1. Find min/max: n reads (s.Read)
        // 2. Count occurrences: n reads (s.Read)
        // 3. Build result in reverse: n reads (s.Read) + n writes (tempSpan.Write)
        // 4. Write back: n reads (tempSpan.Read) + n writes (s.Write)
        //  Total: 4n reads, 2n writes
        var expectedReads = (ulong)(4 * n);
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
        CountingSortInteger.Sort(reversed.AsSpan(), stats);

        // CountingSortInteger complexity is O(n + k) regardless of input order
        // With temp buffer tracking: 4n reads, 2n writes
        var expectedReads = (ulong)(4 * n);
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
        CountingSortInteger.Sort(random.AsSpan(), stats);

        // CountingSortInteger has same complexity regardless of input distribution
        // 4n reads due to temp buffer tracking, 2n writes
        var expectedReads = (ulong)(4 * n);
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
        CountingSortInteger.Sort(allSame.AsSpan(), stats);

        // When all values are the same (min == max), early return after min/max scan
        // Only n reads for finding min/max, then early return (no writes)
        var expectedReads = (ulong)n;
        var expectedWrites = 0UL;

        Assert.Equal(0UL, stats.CompareCount);
        Assert.Equal(0UL, stats.SwapCount);
        Assert.Equal(expectedReads, stats.IndexReadCount);
        Assert.Equal(expectedWrites, stats.IndexWriteCount);
    }

    [Theory]
    [InlineData(10_000_001)]
    public void RangeLimitTest(int range)
    {
        // Test that excessive range throws ArgumentException
        var array = new[] { 0, range };
        Assert.Throws<ArgumentException>(() => CountingSortInteger.Sort(array.AsSpan()));
    }

    [Fact]
    public void NegativeValuesTest()
    {
        var stats = new StatisticsContext();
        var array = new[] { -5, -1, -10, 3, 0, -3 };
        var n = array.Length;
        CountingSortInteger.Sort(array.AsSpan(), stats);

        Assert.Equal(new[] { -10, -5, -3, -1, 0, 3 }, array);
        Assert.Equal((ulong)(4 * n), stats.IndexReadCount);
        Assert.Equal((ulong)(2 * n), stats.IndexWriteCount);
        Assert.Equal(0UL, stats.CompareCount);
    }

    [Fact]
    public void EmptyArrayTest()
    {
        var stats = new StatisticsContext();
        var array = Array.Empty<int>();
        CountingSortInteger.Sort(array.AsSpan(), stats);

        Assert.Empty(array);
        Assert.Equal(0UL, stats.IndexReadCount);
        Assert.Equal(0UL, stats.IndexWriteCount);
    }

    [Fact]
    public void SingleElementTest()
    {
        var stats = new StatisticsContext();
        var array = new[] { 42 };
        CountingSortInteger.Sort(array.AsSpan(), stats);

        Assert.Single(array);
        Assert.Equal(42, array[0]);
        Assert.Equal(0UL, stats.IndexReadCount);
        Assert.Equal(0UL, stats.IndexWriteCount);
    }

    [Theory]
    [InlineData(2)]
    [InlineData(5)]
    [InlineData(10)]
    public void DuplicateValuesTest(int duplicateCount)
    {
        var stats = new StatisticsContext();
        var array = Enumerable.Repeat(5, duplicateCount).Concat(Enumerable.Repeat(3, duplicateCount)).ToArray();
        var n = array.Length;
        CountingSortInteger.Sort(array.AsSpan(), stats);

        var expected = Enumerable.Repeat(3, duplicateCount).Concat(Enumerable.Repeat(5, duplicateCount)).ToArray();
        Assert.Equal(expected, array);
        Assert.Equal((ulong)(4 * n), stats.IndexReadCount);
        Assert.Equal((ulong)(2 * n), stats.IndexWriteCount);
    }
}
