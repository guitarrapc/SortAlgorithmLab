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

        // BucketSort on sorted data (with internal buffer tracking via SortSpan):
        //
        // Operations breakdown:
        // 1. Find min/max: n reads (main buffer)
        // 2. Distribution: n reads (main buffer)
        // 3. InsertionSort per bucket (via SortSpan on bucket buffers):
        //    - For sorted buckets: Read for key, Read for comparison
        //    - Each element except first: 2 reads (key read + 1 comparison read)
        //    - Total across all buckets: ~2n reads
        // 4. Write back: n writes (main buffer) + n reads from temp (NOT via SortSpan)
        //
        // Actual observations:
        // n=10:  34 reads (3.4n), 36 writes
        // n=20:  72 reads (3.6n), 36 writes  
        // n=50:  186 reads (3.72n)
        // n=100: 380 reads (3.8n)
        //
        // Writes include: InsertionSort shifts + write back

        var bucketCount = Math.Max(2, Math.Min(1000, (int)Math.Sqrt(n)));
        
        // IndexReadCount: 2n (find/distribute) + ~1.4n to 1.8n (insertion sort)
        var expectedReadsMin = (ulong)(3.3 * n);
        var expectedReadsMax = (ulong)(4.0 * n);

        // IndexWriteCount: includes writes during insertion sort
        // For sorted data, minimal shifts but key writes occur
        // Observed: ~1.8n for small n, approaching ~2n for larger n
        var expectedWritesMin = (ulong)n;
        var expectedWritesMax = (ulong)(2.5 * n);

        // CompareCount: n - bucketCount (one per element except first in each bucket)
        // But with SortSpan Compare(), each comparison involves reads
        // Observed: slightly higher due to additional comparison checks
        var expectedComparesMin = (ulong)(n - bucketCount);
        var expectedComparesMax = (ulong)(n);

        Assert.InRange(stats.IndexReadCount, expectedReadsMin, expectedReadsMax);
        Assert.InRange(stats.IndexWriteCount, expectedWritesMin, expectedWritesMax);
        Assert.InRange(stats.CompareCount, expectedComparesMin, expectedComparesMax);
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

        // BucketSort on reversed data (with internal buffer tracking):
        // - Distribution is same as sorted (independent of order)
        // - Within each bucket, elements are reversed
        // - InsertionSort worst case: many shifts
        //
        // Actual observations:
        // n=10:  writes=30 (3n)
        // n=20:  writes=76 (3.8n)
        // n=50:  writes=262 (5.24n)
        // n=100: writes=640 (6.4n)
        
        var bucketCount = Math.Max(2, Math.Min(1000, (int)Math.Sqrt(n)));
        
        // IndexReadCount: base operations + heavy insertion sort reads
        var minReads = (ulong)(2 * n);
        var maxReads = (ulong)(15 * n); // Allow for worst case

        // IndexWriteCount: many shifts during insertion sort
        var minWrites = (ulong)n;
        var maxWrites = (ulong)(8 * n);

        // CompareCount: worst case for insertion sort
        var bucketSize = n / bucketCount;
        var maxComparesPerBucket = bucketSize * (bucketSize - 1) / 2;
        var maxCompares = (ulong)(bucketCount * maxComparesPerBucket * 2);

        Assert.InRange(stats.IndexReadCount, minReads, maxReads);
        Assert.InRange(stats.IndexWriteCount, minWrites, maxWrites);
        Assert.InRange(stats.CompareCount, 0UL, maxCompares);
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

        // BucketSort on random data (with internal buffer tracking):
        // - For Enumerable.Range shuffled, keys are still 0..n-1 (uniform distribution)
        // - InsertionSort per bucket performs more operations on random data
        //
        // Actual observations:
        // n=10:  reads vary significantly, writes ~12-30
        // n=20:  reads ~70-130, writes ~36-70  
        // n=50:  reads ~180-350, writes ~100-200
        // n=100: reads ~400-900, writes ~200-900

        var bucketCount = Math.Max(2, Math.Min(1000, (int)Math.Sqrt(n)));
        
        // IndexReadCount: highly variable based on randomness
        // Base: 2n (find/distribute) + variable insertion sort reads
        var expectedReadsMin = (ulong)(2 * n);
        var expectedReadsMax = (ulong)(10 * n);

        // IndexWriteCount: includes insertion sort shifts
        // Random data requires more shifts than sorted data
        var expectedWritesMin = (ulong)(0.5 * n);
        var expectedWritesMax = (ulong)(10 * n);

        // CompareCount: varies based on bucket distribution
        var bucketSize = n / bucketCount;
        var maxComparesPerBucket = bucketSize * (bucketSize - 1) / 2;
        var maxCompares = (ulong)(bucketCount * maxComparesPerBucket * 2);

        Assert.InRange(stats.IndexReadCount, expectedReadsMin, expectedReadsMax);
        Assert.InRange(stats.IndexWriteCount, expectedWritesMin, expectedWritesMax);
        Assert.InRange(stats.CompareCount, 0UL, maxCompares);
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
