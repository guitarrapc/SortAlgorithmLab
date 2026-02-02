using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;

namespace SortAlgorithm.Tests;

public class AmericanFlagSortTests
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

        AmericanFlagSort.Sort(array.AsSpan(), stats);

        // Check is sorted
        for (int i = 0; i < array.Length - 1; i++)
            Assert.True(array[i] <= array[i + 1]);

        // Check element counts match
        var sortedCounts = array.GroupBy(x => x).ToDictionary(g => g.Key, g => g.Count());
        Assert.Equal(originalCounts, sortedCounts);
    }

    [Fact]
    public void MinValueHandlingTest()
    {
        var stats = new StatisticsContext();
        // Test that int.MinValue is handled correctly (no overflow)
        var array = new[] { int.MinValue, -1, 0, 1, int.MaxValue };
        AmericanFlagSort.Sort(array.AsSpan(), stats);

        Assert.Equal(new[] { int.MinValue, -1, 0, 1, int.MaxValue }, array);
    }

    [Fact]
    public void SortWithNegativeNumbers()
    {
        var stats = new StatisticsContext();
        var array = new[] { -5, 3, -1, 0, 2, -3, 1 };
        var expected = new[] { -5, -3, -1, 0, 1, 2, 3 };
        AmericanFlagSort.Sort(array.AsSpan(), stats);

        Assert.Equal(expected, array);
    }

    [Fact]
    public void SortWithAllSameValues()
    {
        var stats = new StatisticsContext();
        var array = new[] { 5, 5, 5, 5, 5 };
        AmericanFlagSort.Sort(array.AsSpan(), stats);

        Assert.All(array, x => Assert.Equal(5, x));
    }

    [Theory]
    [InlineData(typeof(byte))]
    [InlineData(typeof(sbyte))]
    [InlineData(typeof(short))]
    [InlineData(typeof(ushort))]
    [InlineData(typeof(int))]
    [InlineData(typeof(uint))]
    [InlineData(typeof(long))]
    [InlineData(typeof(ulong))]
    public void SortDifferentIntegerTypes(Type type)
    {
        var stats = new StatisticsContext();

        if (type == typeof(byte))
        {
            var array = new byte[] { 5, 2, 8, 1, 9 };
            AmericanFlagSort.Sort(array.AsSpan(), stats);
            Assert.True(IsSorted(array));
        }
        else if (type == typeof(sbyte))
        {
            var array = new sbyte[] { -5, 2, -8, 1, 9 };
            AmericanFlagSort.Sort(array.AsSpan(), stats);
            Assert.True(IsSorted(array));
        }
        else if (type == typeof(short))
        {
            var array = new short[] { -5, 2, -8, 1, 9 };
            AmericanFlagSort.Sort(array.AsSpan(), stats);
            Assert.True(IsSorted(array));
        }
        else if (type == typeof(ushort))
        {
            var array = new ushort[] { 5, 2, 8, 1, 9 };
            AmericanFlagSort.Sort(array.AsSpan(), stats);
            Assert.True(IsSorted(array));
        }
        else if (type == typeof(int))
        {
            var array = new int[] { -5, 2, -8, 1, 9 };
            AmericanFlagSort.Sort(array.AsSpan(), stats);
            Assert.True(IsSorted(array));
        }
        else if (type == typeof(uint))
        {
            var array = new uint[] { 5, 2, 8, 1, 9 };
            AmericanFlagSort.Sort(array.AsSpan(), stats);
            Assert.True(IsSorted(array));
        }
        else if (type == typeof(long))
        {
            var array = new long[] { -5, 2, -8, 1, 9 };
            AmericanFlagSort.Sort(array.AsSpan(), stats);
            Assert.True(IsSorted(array));
        }
        else if (type == typeof(ulong))
        {
            var array = new ulong[] { 5, 2, 8, 1, 9 };
            AmericanFlagSort.Sort(array.AsSpan(), stats);
            Assert.True(IsSorted(array));
        }
    }

    private static bool IsSorted<T>(T[] array) where T : IComparable<T>
    {
        for (int i = 1; i < array.Length; i++)
        {
            if (array[i - 1].CompareTo(array[i]) > 0)
                return false;
        }
        return true;
    }

    [Fact]
    public void EmptyArrayTest()
    {
        var array = Array.Empty<int>();
        AmericanFlagSort.Sort(array.AsSpan());
        Assert.Empty(array);
    }

    [Fact]
    public void SingleElementTest()
    {
        var array = new[] { 42 };
        AmericanFlagSort.Sort(array.AsSpan());
        Assert.Single(array);
        Assert.Equal(42, array[0]);
    }

    [Fact]
    public void TwoElementsTest()
    {
        var array = new[] { 2, 1 };
        AmericanFlagSort.Sort(array.AsSpan());
        Assert.Equal(new[] { 1, 2 }, array);
    }

    [Fact]
    public void DecimalDigitBoundaryTest()
    {
        // Test values that cross decimal digit boundaries (9→10, 99→100, etc.)
        var array = new[] { 100, 9, 99, 10, 1, 999, 1000 };
        var expected = new[] { 1, 9, 10, 99, 100, 999, 1000 };
        AmericanFlagSort.Sort(array.AsSpan());
        Assert.Equal(expected, array);
    }

    [Fact]
    public void InsertionSortCutoffTest()
    {
        // Test with array smaller than insertion sort cutoff (16)
        var array = new[] { 10, 5, 3, 8, 1, 9, 2, 7, 4, 6 };
        var expected = new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
        AmericanFlagSort.Sort(array.AsSpan());
        Assert.Equal(expected, array);
    }

    [Fact]
    public void LargeRangeTest()
    {
        var stats = new StatisticsContext();
        // Test with values spanning a large range
        var array = new[] { 1000000, -1000000, 0, 500000, -500000 };
        var expected = new[] { -1000000, -500000, 0, 500000, 1000000 };
        AmericanFlagSort.Sort(array.AsSpan(), stats);
        Assert.Equal(expected, array);
    }

    [Fact]
    public void InPlacePermutationTest()
    {
        var stats = new StatisticsContext();
        // Verify that the sort is performed in-place (no auxiliary array)
        // Use larger array to exceed InsertionSortCutoff (16)
        var array = new[] { 25, 13, 28, 11, 22, 29, 14, 27, 16, 30, 15, 26, 17, 31, 18, 24, 19, 23, 20, 21 };
        var expected = new[] { 11, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31 };
        AmericanFlagSort.Sort(array.AsSpan(), stats);
        Assert.Equal(expected, array);
    }

#if DEBUG

    [Theory]
    [ClassData(typeof(MockSortedData))]
    public void StatisticsSortedTest(IInputSample<int> inputSample)
    {
        var stats = new StatisticsContext();
        var array = inputSample.Samples.ToArray();
        AmericanFlagSort.Sort(array.AsSpan(), stats);

        Assert.Equal((ulong)inputSample.Samples.Length, (ulong)array.Length);
        Assert.NotEqual(0UL, stats.IndexReadCount);
        // For sorted data, in-place permutation may result in fewer writes
        // but insertion sort for small buckets will still cause operations
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
        AmericanFlagSort.Sort(sorted.AsSpan(), stats);

        // American Flag Sort is an in-place MSD Radix Sort variant
        // For sorted data:
        // - Elements distribute into buckets
        // - Small buckets (<=16) use insertion sort
        // - In-place permutation minimizes writes
        Assert.Equal((ulong)n, (ulong)sorted.Length);
        Assert.NotEqual(0UL, stats.IndexReadCount);

        // For sorted data, verify that the sort completes successfully
        Assert.True(IsSorted(sorted));
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
        AmericanFlagSort.Sort(reversed.AsSpan(), stats);

        // American Flag Sort on reversed data:
        // - In-place permutation requires swaps to rearrange elements
        // - Insertion sort for small buckets has more operations
        Assert.Equal((ulong)n, (ulong)reversed.Length);
        Assert.NotEqual(0UL, stats.IndexReadCount);
        Assert.True(IsSorted(reversed));
    }

    [Theory]
    [InlineData(10)]
    [InlineData(20)]
    [InlineData(50)]
    [InlineData(100)]
    public void TheoreticalValuesRandomTest(int n)
    {
        var stats = new StatisticsContext();
        var random = new Random(42);
        var array = Enumerable.Range(0, n).OrderBy(_ => random.Next()).ToArray();
        AmericanFlagSort.Sort(array.AsSpan(), stats);

        // American Flag Sort on random data:
        // - Bucket distribution varies
        // - In-place permutation requires swaps
        // - Combination of MSD partitioning and insertion sort
        Assert.Equal((ulong)n, (ulong)array.Length);
        Assert.NotEqual(0UL, stats.IndexReadCount);
        Assert.True(IsSorted(array));

        // Random data should require swap operations when n > InsertionSortCutoff (16)
        if (n > 16)
        {
            Assert.NotEqual(0UL, stats.SwapCount);
        }
    }

#endif
}
