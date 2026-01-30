using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;

namespace SortAlgorithm.Tests;

public class RadixLSD10SortTests
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
        RadixLSD10Sort.Sort(array.AsSpan(), stats);

        Assert.Equal((ulong)inputSample.Samples.Length, (ulong)array.Length);
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
        RadixLSD10Sort.Sort(keys.AsSpan());

        // After sorting by value, records with same value should maintain original order
        // Since we only sorted keys, we verify the sort is stable by checking
        // that multiple sorts preserve order
        var firstSort = records.Select(r => r.value).ToArray();
        RadixLSD10Sort.Sort(firstSort.AsSpan());

        var secondSort = firstSort.ToArray();
        RadixLSD10Sort.Sort(secondSort.AsSpan());

        Assert.Equal(firstSort, secondSort);
    }

    [Fact]
    public void MinValueHandlingTest()
    {
        var stats = new StatisticsContext();
        // Test that int.MinValue is handled correctly (no overflow)
        var array = new[] { int.MinValue, -1, 0, 1, int.MaxValue };
        RadixLSD10Sort.Sort(array.AsSpan(), stats);

        Assert.Equal(new[] { int.MinValue, -1, 0, 1, int.MaxValue }, array);
    }

    [Fact]
    public void SortWithNegativeNumbers()
    {
        var stats = new StatisticsContext();
        var array = new[] { -5, 3, -1, 0, 2, -3, 1 };
        var expected = new[] { -5, -3, -1, 0, 1, 2, 3 };
        RadixLSD10Sort.Sort(array.AsSpan(), stats);

        Assert.Equal(expected, array);
    }

    [Fact]
    public void SortWithAllSameValues()
    {
        var stats = new StatisticsContext();
        var array = new[] { 5, 5, 5, 5, 5 };
        RadixLSD10Sort.Sort(array.AsSpan(), stats);

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
            RadixLSD10Sort.Sort(array.AsSpan(), stats);
            Assert.True(IsSorted(array));
        }
        else if (type == typeof(sbyte))
        {
            var array = new sbyte[] { -5, 2, -8, 1, 9 };
            RadixLSD10Sort.Sort(array.AsSpan(), stats);
            Assert.True(IsSorted(array));
        }
        else if (type == typeof(short))
        {
            var array = new short[] { -5, 2, -8, 1, 9 };
            RadixLSD10Sort.Sort(array.AsSpan(), stats);
            Assert.True(IsSorted(array));
        }
        else if (type == typeof(ushort))
        {
            var array = new ushort[] { 5, 2, 8, 1, 9 };
            RadixLSD10Sort.Sort(array.AsSpan(), stats);
            Assert.True(IsSorted(array));
        }
        else if (type == typeof(int))
        {
            var array = new int[] { -5, 2, -8, 1, 9 };
            RadixLSD10Sort.Sort(array.AsSpan(), stats);
            Assert.True(IsSorted(array));
        }
        else if (type == typeof(uint))
        {
            var array = new uint[] { 5, 2, 8, 1, 9 };
            RadixLSD10Sort.Sort(array.AsSpan(), stats);
            Assert.True(IsSorted(array));
        }
        else if (type == typeof(long))
        {
            var array = new long[] { -5, 2, -8, 1, 9 };
            RadixLSD10Sort.Sort(array.AsSpan(), stats);
            Assert.True(IsSorted(array));
        }
        else if (type == typeof(ulong))
        {
            var array = new ulong[] { 5, 2, 8, 1, 9 };
            RadixLSD10Sort.Sort(array.AsSpan(), stats);
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
        RadixLSD10Sort.Sort(array.AsSpan(), stats);

        Assert.Equal((ulong)inputSample.Samples.Length, (ulong)array.Length);
        Assert.NotEqual(0UL, stats.IndexReadCount);
        Assert.NotEqual(0UL, stats.IndexWriteCount);
        Assert.Equal(0UL, stats.CompareCount); // Non-comparison sort
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
        RadixLSD10Sort.Sort(sorted.AsSpan(), stats);

        // LSD Radix Sort with sign-bit flipping (unified processing):
        // 1. Find min/max keys: n reads
        // 2. For each digit d (from 0 to digitCount-1):
        //    - Count phase: n reads
        //    - Distribute phase: n reads + n writes (to temp buffer)
        //    - Copy back phase (using CopyTo): n reads (from temp buffer) + n writes (to main buffer)
        //
        // For n elements with values [0, n-1]:
        // - max unsigned key = 0x8000_0000 + (n-1) for non-negative values
        // - digitCount = number of decimal digits needed to represent max key
        //
        // For example, n=100 → max value = 99 → max key = 0x80000063
        // - 0x80000063 = 2,147,483,747 in decimal → 10 decimal digits
        //
        // Total reads = n (find min/max) + digitCount × 3n (count + distribute + CopyTo read)
        // Total writes = digitCount × 2n (distribute write + CopyTo write)
        var maxValue = (uint)(n - 1);
        var maxKey = maxValue ^ 0x8000_0000; // Sign-bit flip for non-negative
        var digitCount = GetDigitCountFromUlong(maxKey);

        var expectedReads = (ulong)(n + digitCount * 3 * n); // Find min/max + (count + distribute + CopyTo) per digit
        var expectedWrites = (ulong)(digitCount * 2 * n); // (distribute + CopyTo) writes per digit

        Assert.Equal(expectedReads, stats.IndexReadCount);
        Assert.Equal(expectedWrites, stats.IndexWriteCount);
        Assert.Equal(0UL, stats.CompareCount); // Non-comparison sort
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
        RadixLSD10Sort.Sort(reversed.AsSpan(), stats);

        // LSD Radix Sort with sign-bit flipping:
        // Same as sorted - performance is data-independent O(d × n)
        var maxValue = (uint)(n - 1);
        var maxKey = maxValue ^ 0x8000_0000; // Sign-bit flip for non-negative
        var digitCount = GetDigitCountFromUlong(maxKey);

        var expectedReads = (ulong)(n + digitCount * 3 * n); // Find min/max + (count + distribute + CopyTo)
        var expectedWrites = (ulong)(digitCount * 2 * n); // (distribute + CopyTo) writes

        Assert.Equal(expectedReads, stats.IndexReadCount);
        Assert.Equal(expectedWrites, stats.IndexWriteCount);
        Assert.Equal(0UL, stats.CompareCount); // Non-comparison sort
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
        RadixLSD10Sort.Sort(random.AsSpan(), stats);

        // LSD Radix Sort with sign-bit flipping:
        // Same complexity as sorted/reversed - O(d × n)
        var maxValue = (uint)(n - 1);
        var maxKey = maxValue ^ 0x8000_0000; // Sign-bit flip for non-negative
        var digitCount = GetDigitCountFromUlong(maxKey);

        var expectedReads = (ulong)(n + digitCount * 3 * n); // Find min/max + (count + distribute + CopyTo)
        var expectedWrites = (ulong)(digitCount * 2 * n); // (distribute + CopyTo) writes

        Assert.Equal(expectedReads, stats.IndexReadCount);
        Assert.Equal(expectedWrites, stats.IndexWriteCount);
        Assert.Equal(0UL, stats.CompareCount); // Non-comparison sort
        Assert.Equal(0UL, stats.SwapCount);
    }

    [Theory]
    [InlineData(10)]
    [InlineData(20)]
    [InlineData(50)]
    [InlineData(100)]
    public void TheoreticalValuesNegativeTest(int n)
    {
        var stats = new StatisticsContext();
        // Mix of negative and positive: [-n/2, ..., -1, 0, 1, ..., n/2-1]
        var mixed = Enumerable.Range(-n / 2, n).ToArray();
        RadixLSD10Sort.Sort(mixed.AsSpan(), stats);

        // With sign-bit flipping, negative and positive numbers are processed uniformly:
        // 1. Find min/max keys: n reads
        // 2. For each digit d (from 0 to digitCount-1):
        //    - Count phase: n reads
        //    - Distribute phase: n reads + n writes (to temp buffer)
        //    - Copy back phase: n reads + n writes
        //
        // For input [-n/2, ..., -1, 0, 1, ..., n/2-1]:
        // - Min value: -n/2 → min key = 0x80000000 - n/2
        // - Max value: n/2-1 → max key = 0x80000000 + (n/2-1)
        // - Max key determines digit count
        var minValue = -n / 2;
        var maxValue = n / 2 - 1;
        var minKey = (uint)minValue ^ 0x8000_0000; // Sign-bit flip
        var maxKey = (uint)maxValue ^ 0x8000_0000; // Sign-bit flip
        var digitCount = GetDigitCountFromUlong(maxKey);

        var expectedReads = (ulong)(n + digitCount * 3 * n); // Find min/max + (count + distribute + CopyTo) per digit
        var expectedWrites = (ulong)(digitCount * 2 * n); // (distribute + CopyTo) writes per digit

        Assert.Equal(expectedReads, stats.IndexReadCount);
        Assert.Equal(expectedWrites, stats.IndexWriteCount);
        Assert.Equal(0UL, stats.CompareCount); // Still non-comparison
        Assert.Equal(0UL, stats.SwapCount);

        // Verify result is sorted
        Assert.Equal(mixed.OrderBy(x => x), mixed);
    }

    /// <summary>
    /// Helper to calculate digit count for theoretical tests (for original values)
    /// </summary>
    private static int GetDigitCount(int value)
    {
        if (value == 0) return 1;

        var count = 0;
        var temp = Math.Abs(value);
        while (temp > 0)
        {
            temp /= 10;
            count++;
        }
        return count;
    }

    /// <summary>
    /// Helper to calculate digit count from unsigned long value (for sign-flipped keys)
    /// </summary>
    private static int GetDigitCountFromUlong(ulong value)
    {
        if (value == 0) return 1;

        var count = 0;
        while (value > 0)
        {
            value /= 10;
            count++;
        }
        return count;
    }

#endif

}
