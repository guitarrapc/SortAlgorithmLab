using SortLab.Core.Algorithms;
using SortLab.Core.Contexts;

namespace SortLab.Tests;

public class BinaryInsertSortTests
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
        BinaryInsertSort.Sort(array.AsSpan());
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
        BinaryInsertSort.Sort(array.AsSpan(), stats);

        Assert.Equal((ulong)inputSample.Samples.Length, (ulong)array.Length);
        Assert.NotEqual(0UL, stats.IndexReadCount);
        Assert.NotEqual(0UL, stats.IndexWriteCount);
        Assert.NotEqual(0UL, stats.CompareCount);
        Assert.Equal(0UL, stats.SwapCount); // BinaryInsertSort uses shift, not swap
    }

    [Theory]
    [ClassData(typeof(MockSortedData))]
    public void StatisticsSortedTest(IInputSample<int> inputSample)
    {
        var stats = new StatisticsContext();
        var array = inputSample.Samples.ToArray();
        BinaryInsertSort.Sort(array.AsSpan(), stats);

        Assert.Equal((ulong)inputSample.Samples.Length, (ulong)array.Length);
        Assert.NotEqual(0UL, stats.IndexReadCount);
        Assert.Equal(0UL, stats.IndexWriteCount); // No writes needed for sorted data
        Assert.NotEqual(0UL, stats.CompareCount);
        Assert.Equal(0UL, stats.SwapCount);
    }

    [Theory]
    [ClassData(typeof(MockRandomData))]
    public void StatisticsResetTest(IInputSample<int> inputSample)
    {
        var stats = new StatisticsContext();
        var array = inputSample.Samples.ToArray();
        BinaryInsertSort.Sort(array.AsSpan(), stats);

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
        BinaryInsertSort.Sort(sorted.AsSpan(), stats);

        // Binary Insertion Sort performs binary search for each element from index 1 to n-1
        // For sorted data, binary search for element at position i searches in range [0..i)
        // The number of comparisons is at most ceiling(log2(i+1)) per search
        // 
        // The actual count depends on the binary search implementation and can vary
        // based on the comparison results. We use a wide tolerance to account for this.
        var expectedCompares = CalculateBinaryInsertSortComparisons(n);

        // Binary search can vary significantly based on data, allow ±50% tolerance
        var minCompares = expectedCompares / 2;
        var maxCompares = (expectedCompares * 3) / 2;

        // Sorted data: no shifts needed (all elements are already in correct positions)
        var expectedWrites = 0UL;

        // IndexReadCount: At minimum, each comparison reads 1 element during binary search
        var minIndexReads = minCompares;

        Assert.InRange(stats.CompareCount, minCompares, maxCompares);
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
        BinaryInsertSort.Sort(reversed.AsSpan(), stats);

        // Binary Insertion Sort comparisons: at most ceiling(log2(i+1)) per search
        // For reversed data, binary search may take more comparisons than sorted data
        var expectedCompares = CalculateBinaryInsertSortComparisons(n);

        // Reversed data can cause more comparisons, allow wider range (50% to 200%)
        var minCompares = expectedCompares / 2;
        var maxCompares = expectedCompares * 2;

        // Reversed data: worst case for shifts
        // Element at position i needs to be shifted to position 0, requiring i shifts
        // For each element at position i (from 1 to n-1):
        // - Shift i elements to the right (i writes)
        // - Write the current element at position 0 (1 write)
        // Total: sum from i=1 to n-1 of (i+1) = n(n+1)/2 - 1
        var minWrites = (ulong)((n * (n + 1)) / 2 - 2);
        var maxWrites = (ulong)((n * (n + 1)) / 2 + 1);

        // IndexReadCount: Each comparison reads 1 element during binary search
        // Plus reads during shifting: each shift reads the element being moved
        // Total shift reads = n(n-1)/2
        var minShiftReads = (ulong)(n * (n - 1) / 2);
        var minIndexReads = minCompares + minShiftReads;

        Assert.InRange(stats.CompareCount, minCompares, maxCompares);
        Assert.InRange(stats.IndexWriteCount, minWrites, maxWrites);
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
        BinaryInsertSort.Sort(random.AsSpan(), stats);

        // Binary Insertion Sort comparisons: at most ceiling(log2(i+1)) per search
        // Random data can vary widely, allow very wide range
        var expectedCompares = CalculateBinaryInsertSortComparisons(n);
        var minCompares = expectedCompares / 2;
        var maxCompares = expectedCompares * 2;

        // Random data: varies significantly based on arrangement
        // Best case: nearly sorted (minimal shifts, close to 0 writes)
        // Worst case: reverse sorted (maximum shifts, n(n+1)/2 writes)
        var minWrites = 0UL;
        var maxWrites = (ulong)((n * (n + 1)) / 2 + 1);

        Assert.InRange(stats.CompareCount, minCompares, maxCompares);
        Assert.InRange(stats.IndexWriteCount, minWrites, maxWrites);
        Assert.True(stats.IndexReadCount >= minCompares / 2,
            $"IndexReadCount ({stats.IndexReadCount}) should be >= {minCompares / 2}");
    }

    /// <summary>
    /// Calculate theoretical number of comparisons for Binary Insertion Sort
    /// Uses the maximum number of comparisons per binary search (worst case)
    /// For a range of size n, binary search takes at most ceiling(log2(n+1)) comparisons
    /// </summary>
    private ulong CalculateBinaryInsertSortComparisons(int n)
    {
        ulong totalCompares = 0;
        for (int i = 1; i < n; i++)
        {
            // Binary search in range [0..i) can take up to ceiling(log2(i+1)) comparisons
            // This is the worst-case number of iterations for the while loop
            if (i == 1)
                totalCompares += 1;
            else
                totalCompares += (ulong)Math.Ceiling(Math.Log2(i + 1));
        }
        return totalCompares;
    }

    [Theory]
    [ClassData(typeof(MockStabilityData))]
    public void StabilityTest(StabilityTestItem[] items)
    {
        // Test stability: equal elements should maintain relative order
        var stats = new StatisticsContext();

        BinaryInsertSort.Sort(items.AsSpan(), stats);

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

        BinaryInsertSort.Sort(items.AsSpan(), stats);

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

        BinaryInsertSort.Sort(items.AsSpan(), stats);

        // All values are 1
        Assert.All(items, item => Assert.Equal(1, item.Value));

        // Original order should be preserved: 0, 1, 2, 3, 4
        Assert.Equal(MockStabilityAllEqualsData.Sorted, items.Select(x => x.OriginalIndex).ToArray());

        // For sorted data with all equal elements, no swaps should occur
        Assert.Equal(0UL, stats.SwapCount);
        Assert.Equal(0UL, stats.IndexWriteCount);
    }
}
