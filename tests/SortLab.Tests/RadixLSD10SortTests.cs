using SortLab.Core.Algorithms;
using SortLab.Core.Contexts;

namespace SortLab.Tests;

public class RadixLSD10SortTests
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
        RadixLSD10Sort.Sort(array.AsSpan());
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
        RadixLSD10Sort.Sort(array.AsSpan(), stats);

        Assert.Equal((ulong)inputSample.Samples.Length, (ulong)array.Length);
        Assert.NotEqual(0UL, stats.IndexReadCount);
        Assert.NotEqual(0UL, stats.IndexWriteCount);
        Assert.Equal(0UL, stats.CompareCount); // Radix sort is non-comparison based
        Assert.Equal(0UL, stats.SwapCount);
    }

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
    [ClassData(typeof(MockRandomData))]
    public void StatisticsResetTest(IInputSample<int> inputSample)
    {
        var stats = new StatisticsContext();
        var array = inputSample.Samples.ToArray();
        RadixLSD10Sort.Sort(array.AsSpan(), stats);

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
        RadixLSD10Sort.Sort(sorted.AsSpan(), stats);

        // LSD Radix Sort on sorted non-negative data:
        // 1. Check for negatives: n reads (no negatives, so check all elements)
        // 2. SortCorePositive:
        //    - Find max to determine digits: n reads
        //    - For each digit d (from 0 to digitCount-1):
        //      - Count phase: n reads
        //      - Distribute phase: n reads
        //      - Copy back phase: n writes
        // 
        // For n elements with max value (n-1):
        // - max = n-1, so digitCount = ⌈log₁₀(n)⌉ (e.g., n=100 → max=99 → 2 digits)
        // 
        // Total reads = n (check negatives) + n (find max) + digitCount × 2n (count + distribute)
        // Total writes = digitCount × n (copy back)
        var max = n - 1;
        var digitCount = GetDigitCount(max);

        var expectedReads = (ulong)(n + n + digitCount * 2 * n); // Check negatives + find max + (count + distribute) per digit
        var expectedWrites = (ulong)(digitCount * n); // Copy back per digit

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

        // LSD Radix Sort on reversed non-negative data:
        // Same as sorted - performance is data-independent O(d × n)
        // Plus the check for negatives: n reads
        var max = n - 1;
        var digitCount = GetDigitCount(max);

        var expectedReads = (ulong)(n + n + digitCount * 2 * n); // Check negatives + find max + (count + distribute)
        var expectedWrites = (ulong)(digitCount * n);

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

        // LSD Radix Sort on random non-negative data:
        // Same complexity as sorted/reversed - O(d × n)
        // Plus the check for negatives: n reads (worst case, all checked)
        var max = n - 1;
        var digitCount = GetDigitCount(max);

        var expectedReads = (ulong)(n + n + digitCount * 2 * n); // Check negatives + find max + (count + distribute)
        var expectedWrites = (ulong)(digitCount * n);

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

        // With negative numbers, the processing flow is:
        // 1. Check for negatives: 1 read (early exit on first negative element)
        // 2. Separate into negative/non-negative: n reads (SortCoreNegative reads all via SortSpan)
        // 3. Sort negative subset (by absolute value) - NOW COUNTED via SortSpan
        // 4. Sort non-negative subset - NOW COUNTED via SortSpan
        // 5. Merge: n writes (writes back to original SortSpan)
        //
        // For input [-n/2, ..., -1, 0, 1, ..., n/2-1]:
        // - Negative count = n/2 (elements: -n/2, -n/2+1, ..., -1)
        // - Non-negative count = n/2 (elements: 0, 1, ..., n/2-1)
        // 
        // Negative subset: max abs value = n/2 → digitCount_neg
        // Non-negative subset: max value = n/2-1 → digitCount_pos
        var negativeCount = n / 2;
        var nonNegativeCount = n - negativeCount;
        var maxNegativeAbs = n / 2;
        var maxNonNegative = n / 2 - 1;
        var digitCountNeg = GetDigitCount(maxNegativeAbs);
        var digitCountPos = GetDigitCount(maxNonNegative);

        // Calculate expected reads and writes:
        // Reads:
        //   1. Check for negatives: 1 (early exit)
        //   2. Separate: n
        //   3. Sort negatives: negativeCount (find max) + digitCountNeg × 2 × negativeCount
        //   4. Sort non-negatives: nonNegativeCount (find max) + digitCountPos × 2 × nonNegativeCount
        // Writes:
        //   1. Sort negatives: digitCountNeg × negativeCount
        //   2. Sort non-negatives: digitCountPos × nonNegativeCount
        //   3. Merge: n
        var expectedReads = (ulong)(
            1 // Check for negatives (early exit)
            + n // Separate
            + negativeCount + digitCountNeg * 2 * negativeCount // Sort negatives
            + nonNegativeCount + digitCountPos * 2 * nonNegativeCount // Sort non-negatives
        );
        
        var expectedWrites = (ulong)(
            digitCountNeg * negativeCount // Sort negatives
            + digitCountPos * nonNegativeCount // Sort non-negatives
            + n // Merge
        );

        Assert.Equal(expectedReads, stats.IndexReadCount);
        Assert.Equal(expectedWrites, stats.IndexWriteCount);
        Assert.Equal(0UL, stats.CompareCount); // Still non-comparison
        Assert.Equal(0UL, stats.SwapCount);
        
        // Verify result is sorted
        Assert.Equal(mixed.OrderBy(x => x), mixed);
    }

    /// <summary>
    /// Helper to calculate digit count for theoretical tests
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
}
