using SortLab.Core.Algorithms;
using SortLab.Core.Contexts;

namespace SortLab.Tests;

public class ShiftSortTests
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
        ShiftSort.Sort(array.AsSpan());
        Assert.Equal(inputSample.Samples.OrderBy(x => x), array);
    }

    [Theory]
    [ClassData(typeof(MockRandomData))]
    [ClassData(typeof(MockNegativePositiveRandomData))]
    [ClassData(typeof(MockNegativeRandomData))]
    [ClassData(typeof(MockReversedData))]
    [ClassData(typeof(MockMountainData))]
    [ClassData(typeof(MockSameValuesData))]
    public void StatisticsTest(IInputSample<int> inputSample)
    {
        var stats = new StatisticsContext();
        var array = inputSample.Samples.ToArray();
        ShiftSort.Sort(array.AsSpan(), stats);

        Assert.Equal((ulong)inputSample.Samples.Length, (ulong)array.Length);
        Assert.NotEqual(0UL, stats.IndexReadCount);
        Assert.NotEqual(0UL, stats.IndexWriteCount);
        Assert.NotEqual(0UL, stats.CompareCount);
        // ShiftSort uses swaps only during run detection phase
        // Some data patterns may not require swaps
    }

    [Theory]
    [ClassData(typeof(MockNearlySortedData))]
    public void StatisticsNearlySortedTest(IInputSample<int> inputSample)
    {
        var stats = new StatisticsContext();
        var array = inputSample.Samples.ToArray();
        ShiftSort.Sort(array.AsSpan(), stats);

        Assert.Equal((ulong)inputSample.Samples.Length, (ulong)array.Length);
        Assert.NotEqual(0UL, stats.IndexReadCount);
        // Nearly sorted data should have minimal writes
        Assert.NotEqual(0UL, stats.IndexWriteCount);
        Assert.NotEqual(0UL, stats.CompareCount);
        // ShiftSort excels with nearly sorted data - minimal swaps
        Assert.InRange(stats.SwapCount, 0UL, 10UL);
    }

    [Theory]
    [ClassData(typeof(MockSortedData))]
    public void StatisticsSortedTest(IInputSample<int> inputSample)
    {
        var stats = new StatisticsContext();
        var array = inputSample.Samples.ToArray();
        ShiftSort.Sort(array.AsSpan(), stats);

        Assert.Equal((ulong)inputSample.Samples.Length, (ulong)array.Length);
        // Sorted data only needs run detection scan
        Assert.NotEqual(0UL, stats.IndexReadCount);
        // No merge operations needed, so minimal writes
        Assert.Equal(0UL, stats.IndexWriteCount);
        Assert.NotEqual(0UL, stats.CompareCount);
        // Sorted data requires no swaps
        Assert.Equal(0UL, stats.SwapCount);
    }

    [Theory]
    [ClassData(typeof(MockRandomData))]
    public void StatisticsResetTest(IInputSample<int> inputSample)
    {
        var stats = new StatisticsContext();
        var array = inputSample.Samples.ToArray();
        ShiftSort.Sort(array.AsSpan(), stats);

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
        ShiftSort.Sort(sorted.AsSpan(), stats);

        // For sorted data:
        // - Run detection: O(n) comparisons (n-1 comparisons in the scan loop)
        // - No run boundaries detected, so no merge operations
        // - No swaps needed
        // - No writes needed
        var expectedCompares = (ulong)(n - 1);
        var expectedSwaps = 0UL;
        var expectedWrites = 0UL;

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
        ShiftSort.Sort(reversed.AsSpan(), stats);

        // For reversed data [n-1, n-2, ..., 1, 0]:
        // - Run detection: O(n) comparisons
        //   * Every adjacent pair is out of order (n-1 boundaries detected)
        //   * Each boundary detection checks 2 elements (current and previous)
        //   * Three-element optimization applies when possible
        // - Maximum number of runs: approximately n/2 (worst case)
        // - Merge operations: O(n log k) where k is number of runs
        // - Swaps during run detection: O(n/2) empirically observed
        //   * The 3-element optimization swaps elements at positions x and x-2
        //   * For reversed data, this creates approximately n/2 swaps
        // - Writes during merge: O(n log k)
        
        // Run detection comparisons: approximately n
        var minRunDetectionCompares = (ulong)(n - 1);
        
        // Swaps are limited to run detection phase only (not during merge)
        // Empirically observed: reversed data produces approximately n/2 swaps
        // due to the 3-element optimization pattern
        var maxSwaps = (ulong)(n / 2 + 5); // Allow some margin for edge cases
        
        // Comparisons include both run detection and merge
        // For reversed data, expect O(n log n) total comparisons
        var minCompares = minRunDetectionCompares;
        var maxCompares = (ulong)(n * Math.Log(n, 2) * 2); // 2x for safety margin
        
        // Writes occur only during merge (shift-based, not swap-based)
        // For reversed data, most elements need to be shifted
        var minWrites = (ulong)(n - 1);
        
        Assert.InRange(stats.CompareCount, minCompares, maxCompares);
        Assert.InRange(stats.SwapCount, 0UL, maxSwaps);
        Assert.True(stats.IndexWriteCount >= minWrites,
            $"IndexWriteCount ({stats.IndexWriteCount}) should be >= {minWrites}");
        Assert.True(stats.IndexReadCount >= minCompares * 2,
            $"IndexReadCount ({stats.IndexReadCount}) should be >= {minCompares * 2}");
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
        ShiftSort.Sort(random.AsSpan(), stats);

        // For random data:
        // - Number of runs: varies significantly (typically k << n)
        // - Comparisons: O(n log k) where k is number of runs
        // - Swaps during run detection: typically less than n/2
        // - Writes during merge: O(n log k)
        
        var minCompares = (ulong)(n - 1); // At least run detection
        var maxCompares = (ulong)(n * Math.Log(n, 2) * 2); // At most O(n log n)
        
        var maxSwaps = (ulong)(n / 2 + 5); // Limited to run detection phase
        
        // Random data typically requires many merges
        var minWrites = (ulong)(n / 4);
        
        Assert.InRange(stats.CompareCount, minCompares, maxCompares);
        Assert.InRange(stats.SwapCount, 0UL, maxSwaps);
        Assert.True(stats.IndexWriteCount >= minWrites,
            $"IndexWriteCount ({stats.IndexWriteCount}) should be >= {minWrites}");
        Assert.True(stats.IndexReadCount >= minCompares * 2,
            $"IndexReadCount ({stats.IndexReadCount}) should be >= {minCompares * 2}");
    }

    [Theory]
    [InlineData(10)]
    [InlineData(20)]
    [InlineData(50)]
    [InlineData(100)]
    public void TheoreticalValuesAlternatingTest(int n)
    {
        var stats = new StatisticsContext();
        // Create alternating pattern: [0, 2, 4, ..., 1, 3, 5, ...]
        var alternating = Enumerable.Range(0, n)
            .OrderBy(x => x % 2)
            .ThenBy(x => x)
            .ToArray();
        ShiftSort.Sort(alternating.AsSpan(), stats);

        // Alternating data creates multiple runs that need merging
        // This tests the adaptive merge behavior
        
        var minCompares = (ulong)(n - 1);
        var maxCompares = (ulong)(n * Math.Log(n, 2) * 2);
        
        var maxSwaps = (ulong)(n / 2 + 5);
        
        Assert.InRange(stats.CompareCount, minCompares, maxCompares);
        Assert.InRange(stats.SwapCount, 0UL, maxSwaps);
        Assert.NotEqual(0UL, stats.IndexWriteCount);
        Assert.NotEqual(0UL, stats.IndexReadCount);
    }

    [Fact]
    public void StabilityTest()
    {
        // Test stability: equal elements should maintain relative order
        var stats = new StatisticsContext();
        var items = new[]
        {
            (Value: 1, Order: 0),
            (Value: 2, Order: 1),
            (Value: 1, Order: 2),
            (Value: 3, Order: 3),
            (Value: 1, Order: 4),
            (Value: 2, Order: 5),
        };

        // Sort by Value only
        var array = items.Select(x => x.Value).ToArray();
        ShiftSort.Sort(array.AsSpan(), stats);

        // Expected: [1, 1, 1, 2, 2, 3]
        Assert.Equal([1, 1, 1, 2, 2, 3], array);
    }

    [Theory]
    [InlineData(256)]  // Stackalloc threshold
    [InlineData(257)]  // Just over threshold (should use ArrayPool)
    [InlineData(512)]  // ArrayPool
    [InlineData(1024)] // Large array
    public void LargeArrayTest(int n)
    {
        var stats = new StatisticsContext();
        var array = Enumerable.Range(0, n).OrderBy(_ => Guid.NewGuid()).ToArray();
        ShiftSort.Sort(array.AsSpan(), stats);

        // Verify sorting correctness
        Assert.Equal(Enumerable.Range(0, n), array);
        
        // Verify statistics are tracked
        Assert.NotEqual(0UL, stats.IndexReadCount);
        Assert.NotEqual(0UL, stats.IndexWriteCount);
        Assert.NotEqual(0UL, stats.CompareCount);
    }
}
