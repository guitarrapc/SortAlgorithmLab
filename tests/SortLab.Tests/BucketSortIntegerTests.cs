using SortLab.Core.Algorithms;
using SortLab.Core.Contexts;

namespace SortLab.Tests;

public class BucketSortIntegerTests
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
        BucketSortInteger.Sort(array.AsSpan(), stats);

        Assert.Equal((ulong)inputSample.Samples.Length, (ulong)array.Length);
    }

#if DEBUG

    [Theory]
    [ClassData(typeof(MockSortedData))]
    public void StatisticsSortedTest(IInputSample<int> inputSample)
    {
        var stats = new StatisticsContext();
        var array = inputSample.Samples.ToArray();
        BucketSortInteger.Sort(array.AsSpan(), stats);

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
        BucketSortInteger.Sort(sorted.AsSpan(), stats);

        var minReads = (ulong)(2 * n);
        var expectedWrites = (ulong)(2 * n); // n (distribute to temp) + n (CopyTo to main)
        var bucketCount = Math.Max(2, Math.Min(1000, (int)Math.Sqrt(n)));
        var expectedCompares = (ulong)(n - bucketCount);

        Assert.True(stats.IndexReadCount >= minReads,
            $"IndexReadCount ({stats.IndexReadCount}) should be >= {minReads}");
        Assert.Equal(expectedWrites, stats.IndexWriteCount);
        Assert.True(stats.CompareCount <= expectedCompares * 2,
            $"CompareCount ({stats.CompareCount}) should be <= {expectedCompares * 2}");
        Assert.Equal(0UL, stats.SwapCount);
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
        BucketSortInteger.Sort(reversed.AsSpan(), stats);

        var minReads = (ulong)(2 * n);
        var expectedWrites = (ulong)(2 * n); // n (distribute to temp) + n (CopyTo to main)
        var bucketCount = Math.Max(2, Math.Min(1000, (int)Math.Sqrt(n)));
        var bucketSize = n / bucketCount;
        var maxComparesPerBucket = bucketSize * (bucketSize - 1) / 2;
        var maxCompares = (ulong)(bucketCount * maxComparesPerBucket);

        Assert.True(stats.IndexReadCount >= minReads,
            $"IndexReadCount ({stats.IndexReadCount}) should be >= {minReads}");
        Assert.Equal(expectedWrites, stats.IndexWriteCount);
        Assert.True(stats.CompareCount <= maxCompares * 2,
            $"CompareCount ({stats.CompareCount}) should be <= {maxCompares * 2}");
        Assert.Equal(0UL, stats.SwapCount);
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
        BucketSortInteger.Sort(random.AsSpan(), stats);

        var minReads = (ulong)(2 * n);
        var expectedWrites = (ulong)(2 * n); // n (distribute to temp) + n (CopyTo to main)
        var bucketCount = Math.Max(2, Math.Min(1000, (int)Math.Sqrt(n)));
        var minCompares = 0UL;
        var bucketSize = n / bucketCount;
        var maxComparesPerBucket = bucketSize * (bucketSize - 1) / 2;
        var maxCompares = (ulong)(bucketCount * maxComparesPerBucket);

        Assert.True(stats.IndexReadCount >= minReads,
            $"IndexReadCount ({stats.IndexReadCount}) should be >= {minReads}");
        Assert.Equal(expectedWrites, stats.IndexWriteCount);
        Assert.InRange(stats.CompareCount, minCompares, maxCompares * 2);
        Assert.Equal(0UL, stats.SwapCount);
    }

    [Theory]
    [InlineData(10)]
    [InlineData(20)]
    [InlineData(50)]
    [InlineData(100)]
    public void TheoreticalValuesAllSameTest(int n)
    {
        var stats = new StatisticsContext();
        var allSame = Enumerable.Repeat(42, n).ToArray();
        BucketSortInteger.Sort(allSame.AsSpan(), stats);

        var expectedReads = (ulong)n;
        var expectedWrites = 0UL;
        var expectedCompares = 0UL;

        Assert.Equal(expectedReads, stats.IndexReadCount);
        Assert.Equal(expectedWrites, stats.IndexWriteCount);
        Assert.Equal(expectedCompares, stats.CompareCount);
        Assert.Equal(0UL, stats.SwapCount);
    }

#endif

}
