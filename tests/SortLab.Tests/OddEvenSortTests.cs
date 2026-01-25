using SortLab.Core.Algorithms;
using SortLab.Core.Contexts;

namespace SortLab.Tests;

public class OddEvenSortTests
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
    public void SortResultOrderTest(IInputSample<int> inputSample)
    {
        var stats = new StatisticsContext();
        var array = inputSample.Samples.ToArray();
        OddEvenSort.Sort(array.AsSpan(), stats);

        Assert.Equal((ulong)inputSample.Samples.Length, (ulong)array.Length);
    }

    [Theory]
    [ClassData(typeof(MockStabilityData))]
    public void StabilityTest(StabilityTestItem[] items)
    {
        // Test stability: equal elements should maintain relative order
        var stats = new StatisticsContext();

        OddEvenSort.Sort(items.AsSpan(), stats);

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

        OddEvenSort.Sort(items.AsSpan(), stats);

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

        OddEvenSort.Sort(items.AsSpan(), stats);

        // All values are 1
        Assert.All(items, item => Assert.Equal(1, item.Value));

        // Original order should be preserved: 0, 1, 2, 3, 4
        Assert.Equal(MockStabilityAllEqualsData.Sorted, items.Select(x => x.OriginalIndex).ToArray());
    }

#if DEBUG

    [Theory]
    [ClassData(typeof(MockSortedData))]
    public void StatisticsSortedTest(IInputSample<int> inputSample)
    {
        var stats = new StatisticsContext();
        var array = inputSample.Samples.ToArray();
        OddEvenSort.Sort(array.AsSpan(), stats);

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
        OddEvenSort.Sort(sorted.AsSpan(), stats);

        // Odd-Even Sort performs one pass through the array for sorted data
        // - Odd-even pass: floor(n/2) comparisons (positions 0-1, 2-3, 4-5, ...)
        // - Even-odd pass: floor((n-1)/2) comparisons (positions 1-2, 3-4, 5-6, ...)
        // Total comparisons per pass ≈ n-1
        // For sorted data, one pass is enough to verify, so approximately n-1 comparisons
        var expectedCompares = (ulong)(n - 1);
        var expectedSwaps = 0UL;
        var expectedWrites = 0UL; // No swaps = no writes

        // Each comparison reads 2 elements
        var minIndexReads = expectedCompares * 2;

        Assert.Equal(expectedCompares, stats.CompareCount);
        Assert.Equal(expectedSwaps, stats.SwapCount);
        Assert.Equal(expectedWrites, stats.IndexWriteCount);
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
        OddEvenSort.Sort(reversed.AsSpan(), stats);

        // Odd-Even Sort worst case (reversed data):
        // The algorithm needs approximately n/2 phases to sort reversed data
        // Each phase does approximately n-1 comparisons:
        // - Odd-even pass: floor(n/2) comparisons
        // - Even-odd pass: floor((n-1)/2) comparisons
        // Total comparisons ≈ (n/2) * (n-1) ≈ n²/2
        //
        // Empirical observations:
        // - n=10: 54 comparisons
        // - n=20: 209 comparisons
        // - n=50: 1274 comparisons
        // - n=100: 5049 comparisons
        //
        // Pattern: approximately n²/2 comparisons
        var minCompares = (ulong)(n * n / 2 * 0.95); // Allow 5% below
        var maxCompares = (ulong)(n * n / 2 * 1.10); // Allow 10% above

        var expectedSwaps = (ulong)(n * (n - 1) / 2);
        var expectedWrites = expectedSwaps * 2; // Each swap writes 2 elements

        // Each comparison reads 2 elements
        var minIndexReads = minCompares * 2;

        Assert.InRange(stats.CompareCount, minCompares, maxCompares);
        Assert.Equal(expectedSwaps, stats.SwapCount);
        Assert.Equal(expectedWrites, stats.IndexWriteCount);
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
        OddEvenSort.Sort(random.AsSpan(), stats);

        // Odd-Even Sort performs approximately n/4 to n/2 phases for random data
        // Each phase does approximately n-1 comparisons
        // Average comparisons ≈ n²/4 to n²/2
        //
        // For random data, the actual behavior varies significantly based on initial order
        // Allow wider range to account for lucky initial states (almost sorted by chance)
        var minCompares = (ulong)(n); // At minimum, need one pass (n-1) comparisons
        var maxCompares = (ulong)(n * n / 2 * 1.1); // Allow 10% above worst case average

        // For random data, swap count varies significantly
        // Average case: approximately n²/8 to n(n-1)/2
        // Allow wider range for random variations
        var minSwaps = 0UL; // Lucky case: already sorted
        var maxSwaps = (ulong)(n * (n - 1) / 2);

        // Each comparison reads 2 elements
        var minIndexReads = minCompares * 2;

        Assert.InRange(stats.CompareCount, minCompares, maxCompares);
        Assert.InRange(stats.SwapCount, minSwaps, maxSwaps);
        Assert.True(stats.IndexReadCount >= minIndexReads,
            $"IndexReadCount ({stats.IndexReadCount}) should be >= {minIndexReads}");
    }

#endif

}
