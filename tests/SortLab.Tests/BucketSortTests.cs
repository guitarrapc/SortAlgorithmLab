using SortLab.Core.Algorithms;
using SortLab.Core.Contexts;
using BucketSort = SortLab.Core.Algorithms.BucketSort;

namespace SortLab.Tests;

public class BucketSortTests
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
        BucketSort.Sort(array.AsSpan(), x => x);
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
        BucketSort.Sort(array.AsSpan(), x => x, stats);

        Assert.Equal((ulong)inputSample.Samples.Length, (ulong)array.Length);
        Assert.NotEqual(0UL, stats.IndexReadCount);
        Assert.NotEqual(0UL, stats.IndexWriteCount);
        // BucketSort comparisons occur only within buckets (insertion sort)
        // For well-distributed data, buckets are small, so comparisons are minimal
        // CompareCount can be 0 if all buckets have size 1
        Assert.Equal(0UL, stats.SwapCount); // BucketSort uses writes, not swaps
    }

    [Theory]
    [ClassData(typeof(MockSortedData))]
    public void StatisticsSortedTest(IInputSample<int> inputSample)
    {
        var stats = new StatisticsContext();
        var array = inputSample.Samples.ToArray();
        BucketSort.Sort(array.AsSpan(), x => x, stats);

        Assert.Equal((ulong)inputSample.Samples.Length, (ulong)array.Length);
        Assert.NotEqual(0UL, stats.IndexReadCount);
        Assert.NotEqual(0UL, stats.IndexWriteCount);
        // Sorted data still needs to be distributed and written back
        Assert.Equal(0UL, stats.SwapCount);
    }

    [Theory]
    [ClassData(typeof(MockRandomData))]
    public void StatisticsResetTest(IInputSample<int> inputSample)
    {
        var stats = new StatisticsContext();
        var array = inputSample.Samples.ToArray();
        BucketSort.Sort(array.AsSpan(), x => x, stats);

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
        BucketSort.Sort(sorted.AsSpan(), x => x, stats);

        // BucketSort on sorted data with uniform distribution:
        // - First pass: Read n elements to find min/max and distribute
        // - Each bucket (if well-distributed) has ~n/k elements where k = sqrt(n)
        // - Insertion sort within each bucket: best case O(m) where m = bucket size
        // - For sorted data, each bucket is already sorted
        // - Second pass: Write n elements back

        // IndexReadCount:
        // - First pass (min/max + distribution): 2n reads (find min/max + distribute)
        // - Per-bucket insertion sort: n reads (reading elements for sorting)
        // - Total: ~3n reads
        var minReads = (ulong)(2 * n);

        // IndexWriteCount:
        // - Distribution: n writes to temp array (via s.Read, not s.Write)
        // - Write back: n writes to original span
        // - Total: n writes (only write back counts)
        var expectedWrites = (ulong)n;

        // CompareCount:
        // - For sorted uniform data, each bucket should be sorted
        // - Insertion sort comparisons: (bucket_size - 1) per bucket
        // - With k buckets of size n/k each: k * (n/k - 1) = n - k comparisons
        // - For sorted data with uniform distribution, this is minimal
        var bucketCount = Math.Max(2, Math.Min(1000, (int)Math.Sqrt(n)));
        var expectedCompares = (ulong)(n - bucketCount); // Best case for sorted buckets

        Assert.True(stats.IndexReadCount >= minReads,
            $"IndexReadCount ({stats.IndexReadCount}) should be >= {minReads}");
        Assert.Equal(expectedWrites, stats.IndexWriteCount);
        // For sorted data, comparisons should be minimal
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
        BucketSort.Sort(reversed.AsSpan(), x => x, stats);

        // BucketSort on reversed data:
        // - Distribution is the same as sorted case (independent of order)
        // - However, within each bucket, elements may be reversed
        // - Insertion sort worst case: O(m²) per bucket where m = bucket size
        
        // IndexReadCount: similar to sorted case
        var minReads = (ulong)(2 * n);

        // IndexWriteCount: same as sorted case (only write back)
        var expectedWrites = (ulong)n;

        // CompareCount:
        // - With k buckets of size ~n/k each
        // - Worst case insertion sort per bucket: m(m-1)/2 comparisons
        // - Total: sum over all buckets = sum(m(m-1)/2) where sum(m) = n
        // - For k buckets of size n/k: k * ((n/k)*(n/k-1)/2) ≈ n²/(2k) - n/2
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
        BucketSort.Sort(random.AsSpan(), x => x, stats);

        // BucketSort on random data with uniform distribution:
        // - Average case O(n + k) where k = sqrt(n)
        
        // IndexReadCount: ~3n (min/max + distribute + sort)
        var minReads = (ulong)(2 * n);

        // IndexWriteCount: n (distribute + write back)
        var expectedWrites = (ulong)n;

        // CompareCount varies based on distribution within buckets
        // For random data with good distribution, comparisons are moderate
        // However, if data is uniformly distributed (like Enumerable.Range),
        // each bucket might have only 1 element, resulting in 0 comparisons
        var bucketCount = Math.Max(2, Math.Min(1000, (int)Math.Sqrt(n)));
        var minCompares = 0UL; // Best case (all buckets size 1 or sorted)
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
        BucketSort.Sort(allSame.AsSpan(), x => x, stats);

        // All elements are the same:
        // - min == max, early return after first pass
        // - No distribution or sorting needed
        
        // IndexReadCount: only for finding min/max
        var expectedReads = (ulong)n;

        // IndexWriteCount: 0 (early return)
        var expectedWrites = 0UL;

        // CompareCount: 0 (no sorting needed)
        var expectedCompares = 0UL;

        Assert.Equal(expectedReads, stats.IndexReadCount);
        Assert.Equal(expectedWrites, stats.IndexWriteCount);
        Assert.Equal(expectedCompares, stats.CompareCount);
        Assert.Equal(0UL, stats.SwapCount);
    }

    [Theory]
    [ClassData(typeof(MockStabilityData))]
    public void StabilityTest(StabilityTestItem[] items)
    {
        // Test stability: equal elements should maintain relative order
        var stats = new StatisticsContext();

        BucketSort.Sort(items.AsSpan(), x => x.Value, stats);

        // Verify sorting correctness - values should be in ascending order
        Assert.Equal(MockStabilityData.Sorted, items.Select(x => x.Value).ToArray());

        // Verify stability: for each group of equal values, original order is preserved
        var value1Indices = items.Where(x => x.Value == 1).Select(x => x.OriginalIndex).ToArray();
        var value2Indices = items.Where(x => x.Value == 2).Select(x => x.OriginalIndex).ToArray();
        var value3Indices = items.Where(x => x.Value == 3).Select(x => x.OriginalIndex).ToArray();

        // Value 1 appeared at original indices 0, 2, 4 - should remain in this order
        Assert.Equal(MockStabilityData.Sorted1, value1Indices);

        // Value 2 appeared at original indices 1, 5 - should remain in this order
        Assert.Equal(MockStabilityData.Sorted2, value2Indices);

        // Value 3 appeared at original index 3
        Assert.Equal(MockStabilityData.Sorted3, value3Indices);
    }

    [Theory]
    [ClassData(typeof(MockStabilityWithIdData))]
    public void StabilityTestWithComplex(StabilityTestItemWithId[] items)
    {
        // Test stability with more complex scenario - multiple equal values
        var stats = new StatisticsContext();

        BucketSort.Sort(items.AsSpan(), x => x.Key, stats);

        // Expected: [2:B, 2:D, 2:F, 5:A, 5:C, 5:G, 8:E]
        // Keys are sorted, and elements with the same key maintain original order

        for (var i = 0; i < items.Length; i++)
        {
            Assert.Equal(MockStabilityWithIdData.Sorted[i].Key, items[i].Key);
            Assert.Equal(MockStabilityWithIdData.Sorted[i].Id, items[i].Id);
        }
    }

    [Theory]
    [ClassData(typeof(MockStabilityAllEqualsData))]
    public void StabilityTestWithAllEqual(StabilityTestItem[] items)
    {
        // Edge case: all elements have the same value
        // They should remain in original order
        var stats = new StatisticsContext();

        BucketSort.Sort(items.AsSpan(), x => x.Value, stats);

        // All values are 1
        Assert.All(items, item => Assert.Equal(1, item.Value));

        // Original order should be preserved: 0, 1, 2, 3, 4
        Assert.Equal(MockStabilityAllEqualsData.Sorted, items.Select(x => x.OriginalIndex).ToArray());

        // For data with all equal keys, BucketSort should early-return after finding min==max
        // No writes or comparisons should occur
        Assert.Equal(0UL, stats.IndexWriteCount);
        Assert.Equal(0UL, stats.CompareCount);
        Assert.Equal(0UL, stats.SwapCount);
    }
}
