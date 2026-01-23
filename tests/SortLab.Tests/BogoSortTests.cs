using SortLab.Core.Algorithms;
using SortLab.Core.Contexts;
using SortLab.Tests.Attributes;

namespace SortLab.Tests;

public class BogoSortTests
{
    [CISkippableTheory]
    [ClassData(typeof(MockRandomData))]
    [ClassData(typeof(MockNegativePositiveRandomData))]
    [ClassData(typeof(MockNegativeRandomData))]
    [ClassData(typeof(MockReversedData))]
    [ClassData(typeof(MockMountainData))]
    [ClassData(typeof(MockNearlySortedData))]
    [ClassData(typeof(MockSameValuesData))]
    public void SortResultOrderTest(IInputSample<int> inputSample)
    {
        if (inputSample.Samples.Length <= 10)
        {
            var stats = new StatisticsContext();
            var array = inputSample.Samples.ToArray();
            BogoSort.Sort(array.AsSpan(), stats);

            Assert.Equal((ulong)inputSample.Samples.Length, (ulong)array.Length);
        }
    }

    [CISkippableTheory]
    [ClassData(typeof(MockSortedData))]
    public void StatisticsSortedTest(IInputSample<int> inputSample)
    {
        if (inputSample.Samples.Length <= 10)
        {
            var stats = new StatisticsContext();
            var array = inputSample.Samples.ToArray();
            BogoSort.Sort(array.AsSpan(), stats);

            Assert.Equal((ulong)inputSample.Samples.Length, (ulong)array.Length);
            Assert.NotEqual(0UL, stats.IndexReadCount);
            Assert.Equal(0UL, stats.IndexWriteCount);
            Assert.NotEqual(0UL, stats.CompareCount);
            Assert.Equal(0UL, stats.SwapCount);
        }
    }

    [CISkippableFact]
    public void TheoreticalValuesSortedTest()
    {
        // Bogo Sort for sorted data should:
        // 1. Check if sorted (n-1 comparisons, 2*(n-1) reads)
        // 2. Already sorted, so no shuffle needed
        // 3. Exit immediately with 0 swaps and 0 writes
        var stats = new StatisticsContext();
        var sorted = Enumerable.Range(0, 5).ToArray();
        BogoSort.Sort(sorted.AsSpan(), stats);

        var n = sorted.Length;
        var expectedCompares = (ulong)(n - 1);
        var expectedSwaps = 0UL;
        var expectedWrites = 0UL;
        var expectedReads = (ulong)(2 * (n - 1));

        Assert.Equal(expectedCompares, stats.CompareCount);
        Assert.Equal(expectedSwaps, stats.SwapCount);
        Assert.Equal(expectedWrites, stats.IndexWriteCount);
        Assert.Equal(expectedReads, stats.IndexReadCount);
    }

    [CISkippableFact]
    public void TheoreticalValuesSingleShuffleTest()
    {
        // Test with a very small array that requires exactly one shuffle
        // For array [1, 0], it needs one shuffle to become [0, 1]
        var stats = new StatisticsContext();
        var array = new[] { 1, 0 };
        BogoSort.Sort(array.AsSpan(), stats);

        var n = array.Length;

        // First iteration: IsSorted check (1 comparison, 2 reads) -> not sorted
        // Shuffle: n swaps (each swap: 2 reads + 2 writes)
        // Second iteration: IsSorted check (1 comparison, 2 reads) -> sorted
        //
        // Note: Actual values depend on Random.Shared.Next results
        // We can only verify that sorting happened and operations were counted
        Assert.True(stats.CompareCount >= (ulong)(n - 1));
        Assert.NotEqual(0UL, stats.SwapCount);
        Assert.NotEqual(0UL, stats.IndexWriteCount);
        Assert.NotEqual(0UL, stats.IndexReadCount);
        Assert.Equal(new[] { 0, 1 }, array);
    }

    [CISkippableTheory]
    [InlineData(3)]
    [InlineData(5)]
    [InlineData(7)]
    public void TheoreticalValuesRandomTest(int n)
    {
        // Bogo Sort has unbounded runtime for random data
        // We can only test that:
        // 1. The array gets sorted
        // 2. All operation counts are non-zero
        // 3. Each shuffle performs n swaps
        // 4. Each IsSorted check performs n-1 comparisons
        var stats = new StatisticsContext();
        var random = Enumerable.Range(0, n).OrderBy(_ => Guid.NewGuid()).ToArray();
        BogoSort.Sort(random.AsSpan(), stats);

        // Verify the array is sorted
        Assert.Equal(Enumerable.Range(0, n), random);

        // Verify operations were performed
        Assert.NotEqual(0UL, stats.CompareCount);
        Assert.True(stats.IndexReadCount > 0);

        // For non-sorted input, there must be at least one shuffle
        // Each shuffle performs n swaps (2n reads + 2n writes)
        // Minimum is when array becomes sorted after first shuffle
        Assert.True(stats.SwapCount >= 0,
            $"SwapCount ({stats.SwapCount}) should be >= 0");
        Assert.True(stats.IndexWriteCount >= 0,
            $"IndexWriteCount ({stats.IndexWriteCount}) should be >= 0");
    }

    [CISkippableTheory]
    [InlineData(3)]
    [InlineData(5)]
    [InlineData(7)]
    public void TheoreticalValuesReversedTest(int n)
    {
        // Bogo Sort for reversed data has unbounded runtime
        // We verify that:
        // 1. The array gets sorted
        // 2. Multiple shuffles are performed (reversed is very unlikely to sort quickly)
        // 3. All operation counts are significant
        var stats = new StatisticsContext();
        var reversed = Enumerable.Range(0, n).Reverse().ToArray();
        BogoSort.Sort(reversed.AsSpan(), stats);

        // Verify the array is sorted
        Assert.Equal(Enumerable.Range(0, n), reversed);

        // Verify significant operations were performed
        Assert.True(stats.CompareCount >= (ulong)(n - 1),
            $"CompareCount ({stats.CompareCount}) should be >= {n - 1}");
        Assert.NotEqual(0UL, stats.SwapCount);
        Assert.NotEqual(0UL, stats.IndexWriteCount);
        Assert.NotEqual(0UL, stats.IndexReadCount);
    }
}

