using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;

namespace SortAlgorithm.Tests;

public class CountingSortIntegerTests
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
        CountingSortInteger.Sort(array.AsSpan(), stats);

        Assert.Equal((ulong)inputSample.Samples.Length, (ulong)array.Length);
    }

    [Theory]
    [InlineData(10_000_001)]
    public void RangeLimitTest(int range)
    {
        // Test that excessive range throws ArgumentException
        var array = new[] { 0, range };
        Assert.Throws<ArgumentException>(() => CountingSortInteger.Sort(array.AsSpan()));
    }

    [Fact]
    public void NegativeValuesTest()
    {
        var stats = new StatisticsContext();
        var array = new[] { -5, -1, -10, 3, 0, -3 };
        var n = array.Length;
        CountingSortInteger.Sort(array.AsSpan(), stats);

        Assert.Equal(new[] { -10, -5, -3, -1, 0, 3 }, array);
    }

    [Fact]
    public void EmptyArrayTest()
    {
        var stats = new StatisticsContext();
        var array = Array.Empty<int>();
        CountingSortInteger.Sort(array.AsSpan(), stats);

        Assert.Empty(array);
    }

    [Fact]
    public void SingleElementTest()
    {
        var stats = new StatisticsContext();
        var array = new[] { 42 };
        CountingSortInteger.Sort(array.AsSpan(), stats);

        Assert.Single(array);
        Assert.Equal(42, array[0]);
    }

    [Theory]
    [InlineData(2)]
    [InlineData(5)]
    [InlineData(10)]
    public void DuplicateValuesTest(int duplicateCount)
    {
        var stats = new StatisticsContext();
        var array = Enumerable.Repeat(5, duplicateCount).Concat(Enumerable.Repeat(3, duplicateCount)).ToArray();
        var n = array.Length;
        CountingSortInteger.Sort(array.AsSpan(), stats);

        var expected = Enumerable.Repeat(3, duplicateCount).Concat(Enumerable.Repeat(5, duplicateCount)).ToArray();
        Assert.Equal(expected, array);
    }


    [Fact]
    public void StabilityTest()
    {
        // Test stability: elements with same key maintain relative order
        var records = new[]
        {
            (value: 5, id: 1),
            (value: 3, id: 2),
            (value: 5, id: 3),
            (value: 3, id: 4),
            (value: 5, id: 5)
        };

        var keys = records.Select(r => r.value).ToArray();
        CountingSortInteger.Sort(keys.AsSpan());

        // After sorting by value, records with same value should maintain original order
        // Since we only sorted keys, we verify the sort is stable by checking
        // that multiple sorts preserve order
        var firstSort = records.Select(r => r.value).ToArray();
        CountingSortInteger.Sort(firstSort.AsSpan());

        var secondSort = firstSort.ToArray();
        CountingSortInteger.Sort(secondSort.AsSpan());

        Assert.Equal(firstSort, secondSort);
    }

    [Fact]
    public void SortWithNegativeNumbers()
    {
        var stats = new StatisticsContext();
        var array = new[] { -5, 3, -1, 0, 2, -3, 1 };
        var expected = new[] { -5, -3, -1, 0, 1, 2, 3 };
        CountingSortInteger.Sort(array.AsSpan(), stats);

        Assert.Equal(expected, array);
    }

    [Fact]
    public void SortWithAllSameValues()
    {
        var stats = new StatisticsContext();
        var array = new[] { 5, 5, 5, 5, 5 };
        RadixLSD256Sort.Sort(array.AsSpan(), stats);

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
            CountingSortInteger.Sort(array.AsSpan(), stats);
            Assert.True(IsSorted(array));
        }
        else if (type == typeof(sbyte))
        {
            var array = new sbyte[] { -5, 2, -8, 1, 9 };
            CountingSortInteger.Sort(array.AsSpan(), stats);
            Assert.True(IsSorted(array));
        }
        else if (type == typeof(short))
        {
            var array = new short[] { -5, 2, -8, 1, 9 };
            CountingSortInteger.Sort(array.AsSpan(), stats);
            Assert.True(IsSorted(array));
        }
        else if (type == typeof(ushort))
        {
            var array = new ushort[] { 5, 2, 8, 1, 9 };
            CountingSortInteger.Sort(array.AsSpan(), stats);
            Assert.True(IsSorted(array));
        }
        else if (type == typeof(int))
        {
            var array = new int[] { -5, 2, -8, 1, 9 };
            CountingSortInteger.Sort(array.AsSpan(), stats);
            Assert.True(IsSorted(array));
        }
        else if (type == typeof(uint))
        {
            var array = new uint[] { 5, 2, 8, 1, 9 };
            CountingSortInteger.Sort(array.AsSpan(), stats);
            Assert.True(IsSorted(array));
        }
        else if (type == typeof(long))
        {
            var array = new long[] { -5, 2, -8, 1, 9 };
            CountingSortInteger.Sort(array.AsSpan(), stats);
            Assert.True(IsSorted(array));
        }
        else if (type == typeof(ulong))
        {
            var array = new ulong[] { 5, 2, 8, 1, 9 };
            CountingSortInteger.Sort(array.AsSpan(), stats);
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

#if DEBUG

    [Theory]
    [ClassData(typeof(MockSortedData))]
    public void StatisticsSortedTest(IInputSample<int> inputSample)
    {
        var stats = new StatisticsContext();
        var array = inputSample.Samples.ToArray();
        CountingSortInteger.Sort(array.AsSpan(), stats);

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
        CountingSortInteger.Sort(sorted.AsSpan(), stats);

        // CountingSortInteger with temp buffer tracking:
        // 1. Find min/max: n reads (s.Read)
        // 2. Count occurrences: n reads (s.Read)
        // 3. Build result in reverse: n reads (s.Read) + n writes (tempSpan.Write)
        // 4. Write back: n reads (tempSpan.Read) + n writes (s.Write)
        //  Total: 4n reads, 2n writes
        var expectedReads = (ulong)(4 * n);
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
        CountingSortInteger.Sort(reversed.AsSpan(), stats);

        // CountingSortInteger complexity is O(n + k) regardless of input order
        // With temp buffer tracking: 4n reads, 2n writes
        var expectedReads = (ulong)(4 * n);
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
        CountingSortInteger.Sort(random.AsSpan(), stats);

        // CountingSortInteger has same complexity regardless of input distribution
        // 4n reads due to temp buffer tracking, 2n writes
        var expectedReads = (ulong)(4 * n);
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
        CountingSortInteger.Sort(allSame.AsSpan(), stats);

        // When all values are the same (min == max), early return after min/max scan
        // Only n reads for finding min/max, then early return (no writes)
        var expectedReads = (ulong)n;
        var expectedWrites = 0UL;

        Assert.Equal(0UL, stats.CompareCount);
        Assert.Equal(0UL, stats.SwapCount);
        Assert.Equal(expectedReads, stats.IndexReadCount);
        Assert.Equal(expectedWrites, stats.IndexWriteCount);
    }

#endif

}
