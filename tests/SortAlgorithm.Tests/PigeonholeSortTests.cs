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
    [ClassData(typeof(MockAllIdenticalData))]
    [ClassData(typeof(MockTwoDistinctValuesData))]
    [ClassData(typeof(MockHalfZeroHalfOneData))]
    [ClassData(typeof(MockManyDuplicatesSqrtRangeData))]
    [ClassData(typeof(MockHighlySkewedData))]
    public void SortResultOrderTest(IInputSample<int> inputSample)
    {
        var stats = new StatisticsContext();
        var array = inputSample.Samples.ToArray();
        var originalCounts = array.GroupBy(x => x).ToDictionary(g => g.Key, g => g.Count());

        PigeonholeSort.Sort(array.AsSpan(), x => x, stats);

        // Check is sorted
        for (int i = 0; i < array.Length - 1; i++)
            Assert.True(array[i] <= array[i + 1]);

        // Check element counts match
        var sortedCounts = array.GroupBy(x => x).ToDictionary(g => g.Key, g => g.Count());
        Assert.Equal(originalCounts, sortedCounts);
    }

    [Theory]
    [ClassData(typeof(MockStabilityData))]
    public void StabilityTest(StabilityTestItem[] items)
    {
        // Test stability: equal elements should maintain relative order
        var stats = new StatisticsContext();

        PigeonholeSort.Sort(items.AsSpan(), x => x.Value, stats);

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

        PigeonholeSort.Sort(items.AsSpan(), x => x.Key, stats);

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

        PigeonholeSort.Sort(items.AsSpan(), x => x.Value, stats);

        // All values are 1
        Assert.All(items, item => Assert.Equal(1, item.Value));

        // Original order should be preserved: 0, 1, 2, 3, 4
        Assert.Equal(MockStabilityAllEqualsData.Sorted, items.Select(x => x.OriginalIndex).ToArray());
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
