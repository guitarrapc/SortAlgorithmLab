using SortLab.Core.Algorithms;
using SortLab.Core.Contexts;

namespace SortLab.Tests;

public class InsertionSortTests
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
        InsertionSort.Sort(array.AsSpan());
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
        InsertionSort.Sort(array.AsSpan(), stats);

        Assert.Equal((ulong)inputSample.Samples.Length, (ulong)array.Length);
        Assert.NotEqual(0UL, stats.IndexReadCount);
        Assert.NotEqual(0UL, stats.IndexWriteCount);
        Assert.NotEqual(0UL, stats.CompareCount);
        Assert.Equal(0UL, stats.SwapCount); // Insertion sort uses shifts, not swaps
    }

    [Theory]
    [ClassData(typeof(MockSortedData))]
    public void StatisticsSortedTest(IInputSample<int> inputSample)
    {
        var stats = new StatisticsContext();
        var array = inputSample.Samples.ToArray();
        InsertionSort.Sort(array.AsSpan(), stats);

        Assert.Equal((ulong)inputSample.Samples.Length, (ulong)array.Length);
        Assert.NotEqual(0UL, stats.IndexReadCount);
        Assert.Equal(0UL, stats.IndexWriteCount); // Already sorted, no writes needed
        Assert.Equal((ulong)(inputSample.Samples.Length - 1), stats.CompareCount);
        Assert.Equal(0UL, stats.SwapCount);
    }

    [Theory]
    [ClassData(typeof(MockRandomData))]
    public void StatisticsResetTest(IInputSample<int> inputSample)
    {
        var stats = new StatisticsContext();
        var array = inputSample.Samples.ToArray();
        InsertionSort.Sort(array.AsSpan(), stats);

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
        InsertionSort.Sort(sorted.AsSpan(), stats);

        // Insertion Sort on sorted data: best case O(n)
        // - For each position i (from 1 to n-1), we compare once with the previous element
        // - Since the current element is >= the previous element, no shifting occurs
        // - Total comparisons: n-1
        // - Total writes: 0 (already sorted)
        var expectedCompares = (ulong)(n - 1);
        var expectedWrites = 0UL;

        // Each comparison reads 2 elements (j and tmp)
        var minIndexReads = expectedCompares * 2;

        Assert.Equal(expectedCompares, stats.CompareCount);
        Assert.Equal(expectedWrites, stats.IndexWriteCount);
        Assert.True(stats.IndexReadCount >= minIndexReads,
            $"IndexReadCount ({stats.IndexReadCount}) should be >= {minIndexReads}");
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
        InsertionSort.Sort(reversed.AsSpan(), stats);

        // Insertion Sort on reversed data: worst case O(n^2)
        // - Position 1: 1 comparison, 1 shift
        // - Position 2: 2 comparisons, 2 shifts
        // - ...
        // - Position n-1: (n-1) comparisons, (n-1) shifts
        // - Total comparisons: 1 + 2 + ... + (n-1) = n(n-1)/2
        // - Total shifts: same as comparisons = n(n-1)/2
        // - Each shift writes 1 element, plus final write for tmp = shift + 1 write per position
        // - Total writes: For each position i (1 to n-1):
        //   - i shifts (each shift is 1 write: s.Write(j+1, s.Read(j)))
        //   - 1 final write for tmp
        //   - Total: sum from i=1 to n-1 of (i+1) = sum(i) + (n-1) = n(n-1)/2 + (n-1) = (n-1)(n+2)/2
        var expectedCompares = (ulong)(n * (n - 1) / 2);
        var expectedWrites = (ulong)((n - 1) * (n + 2) / 2);

        // Each comparison reads 2 elements
        var minIndexReads = expectedCompares * 2;

        Assert.Equal(expectedCompares, stats.CompareCount);
        Assert.Equal(expectedWrites, stats.IndexWriteCount);
        Assert.True(stats.IndexReadCount >= minIndexReads,
            $"IndexReadCount ({stats.IndexReadCount}) should be >= {minIndexReads}");
        Assert.Equal(0UL, stats.SwapCount); // Insertion sort uses shifts, not swaps
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
        InsertionSort.Sort(random.AsSpan(), stats);

        // Insertion Sort on random data: average case O(n^2)
        // - Average comparisons: approximately n^2/4
        // - Comparisons range from best case (n-1) to worst case (n(n-1)/2)
        var minCompares = (ulong)(n - 1);
        var maxCompares = (ulong)(n * (n - 1) / 2);

        // Writes vary based on how many elements need to be shifted
        var minWrites = 0UL; // Best case (already sorted by chance)
        var maxWrites = (ulong)((n - 1) * (n + 2) / 2); // Worst case (reversed)

        Assert.InRange(stats.CompareCount, minCompares, maxCompares);
        Assert.InRange(stats.IndexWriteCount, minWrites, maxWrites);
        Assert.Equal(0UL, stats.SwapCount); // Insertion sort uses shifts, not swaps
    }

    [Fact]
    public void StabilityTest()
    {
        // Test stability: equal elements should maintain relative order
        // Gnome Sort uses > (strict inequality) to ensure stability
        var stats = new StatisticsContext();

        // Create items with same value but different original indices
        var items = new[]
        {
            new StabilityTestItem(1, 0),
            new StabilityTestItem(2, 1),
            new StabilityTestItem(1, 2),
            new StabilityTestItem(3, 3),
            new StabilityTestItem(1, 4),
            new StabilityTestItem(2, 5),
        };

        var array = items.ToArray();
        InsertionSort.Sort(array.AsSpan(), stats);

        // Verify sorting correctness - values should be in ascending order
        Assert.Equal([1, 1, 1, 2, 2, 3], array.Select(x => x.Value).ToArray());

        // Verify stability: for each group of equal values, original order is preserved
        var value1Indices = array.Where(x => x.Value == 1).Select(x => x.OriginalIndex).ToArray();
        var value2Indices = array.Where(x => x.Value == 2).Select(x => x.OriginalIndex).ToArray();
        var value3Indices = array.Where(x => x.Value == 3).Select(x => x.OriginalIndex).ToArray();

        // Value 1 appeared at original indices 0, 2, 4 - should remain in this order
        Assert.Equal([0, 2, 4], value1Indices);

        // Value 2 appeared at original indices 1, 5 - should remain in this order
        Assert.Equal([1, 5], value2Indices);

        // Value 3 appeared at original index 3
        Assert.Equal([3], value3Indices);
    }

    [Fact]
    public void StabilityTestWithComplex()
    {
        // Test stability with more complex scenario - multiple equal values
        var stats = new StatisticsContext();

        var items = new[]
        {
            new StabilityTestItemWithId(5, "A"),
            new StabilityTestItemWithId(2, "B"),
            new StabilityTestItemWithId(5, "C"),
            new StabilityTestItemWithId(2, "D"),
            new StabilityTestItemWithId(8, "E"),
            new StabilityTestItemWithId(2, "F"),
            new StabilityTestItemWithId(5, "G"),
        };

        var array = items.ToArray();
        InsertionSort.Sort(array.AsSpan(), stats);

        // Expected: [2:B, 2:D, 2:F, 5:A, 5:C, 5:G, 8:E]
        // Keys are sorted, and elements with the same key maintain original order

        Assert.Equal(2, array[0].Key);
        Assert.Equal("B", array[0].Id);

        Assert.Equal(2, array[1].Key);
        Assert.Equal("D", array[1].Id);

        Assert.Equal(2, array[2].Key);
        Assert.Equal("F", array[2].Id);

        Assert.Equal(5, array[3].Key);
        Assert.Equal("A", array[3].Id);

        Assert.Equal(5, array[4].Key);
        Assert.Equal("C", array[4].Id);

        Assert.Equal(5, array[5].Key);
        Assert.Equal("G", array[5].Id);

        Assert.Equal(8, array[6].Key);
        Assert.Equal("E", array[6].Id);
    }

    [Fact]
    public void StabilityTestWithAllEqual()
    {
        // Edge case: all elements have the same value
        // They should remain in original order
        var stats = new StatisticsContext();

        var items = new[]
        {
            new StabilityTestItem(1, 0),
            new StabilityTestItem(1, 1),
            new StabilityTestItem(1, 2),
            new StabilityTestItem(1, 3),
            new StabilityTestItem(1, 4),
        };

        var array = items.ToArray();
        InsertionSort.Sort(array.AsSpan(), stats);

        // All values are 1
        Assert.All(array, item => Assert.Equal(1, item.Value));

        // Original order should be preserved: 0, 1, 2, 3, 4
        Assert.Equal([0, 1, 2, 3, 4], array.Select(x => x.OriginalIndex).ToArray());

        // For sorted data with all equal elements, no swaps should occur
        Assert.Equal(0UL, stats.SwapCount);
        Assert.Equal(0UL, stats.IndexWriteCount);
    }
}

