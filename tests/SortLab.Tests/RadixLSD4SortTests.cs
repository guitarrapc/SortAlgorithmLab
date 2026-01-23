using SortLab.Core.Algorithms;
using SortLab.Core.Contexts;

namespace SortLab.Tests;

public class RadixLSD4SortTests
{
    [Theory]
    [ClassData(typeof(MockRandomData))]
    [ClassData(typeof(MockNegativePositiveRandomData))]
    [ClassData(typeof(MockNegativeRandomData))]
    [ClassData(typeof(MockReversedData))]
    [ClassData(typeof(MockMountainData))]
    [ClassData(typeof(MockNearlySortedData))]
    [ClassData(typeof(MockSameValuesData))]
    public void SortResultOrderTest(IInputSample<int> inputSample)
    {
        var stats = new StatisticsContext();
        var array = inputSample.Samples.ToArray();
        RadixLSD4Sort.Sort(array.AsSpan(), stats);

        Assert.Equal((ulong)inputSample.Samples.Length, (ulong)array.Length);
    }

    [Theory]
    [ClassData(typeof(MockSortedData))]
    public void StatisticsSortedTest(IInputSample<int> inputSample)
    {
        var stats = new StatisticsContext();
        var array = inputSample.Samples.ToArray();
        RadixLSD4Sort.Sort(array.AsSpan(), stats);

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
        RadixLSD4Sort.Sort(sorted.AsSpan(), stats);

        // LSD Radix Sort (Base-256) on sorted non-negative data:
        // For 32-bit integers: digitCount = 4 (4 bytes)
        // 1. Check for negatives: n reads (no negatives found, all elements checked)
        // 2. SortCorePositive (4 passes):
        //    Per pass d (d=0,1,2,3):
        //      - Count phase: n reads
        //      - Distribute phase: n reads + n writes (to temp)
        //      - Copy back phase: n reads (from temp) + n writes (to main)
        //
        // Total per pass: n + n + n + n + n = 5n operations (but we track reads/writes separately)
        // - Reads: n (count) + n (distribute read from main) + n (copy back read from temp) = 3n per pass
        // - Writes: n (distribute write to temp) + n (copy back write to main) = 2n per pass
        //
        // Total for 4 passes:
        // - Reads: n (check negatives) + 4 × 3n = n + 12n = 13n
        // - Writes: 4 × 2n = 8n
        var digitCount = 4; // 32-bit int = 4 bytes
        var expectedReads = (ulong)(n + digitCount * 3 * n);  // Check + (count + distribute + copy) × passes
        var expectedWrites = (ulong)(digitCount * 2 * n);     // (temp write + main write) × passes

        Assert.Equal(expectedReads, stats.IndexReadCount);
        Assert.Equal(expectedWrites, stats.IndexWriteCount);
        Assert.Equal(0UL, stats.CompareCount);
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
        RadixLSD4Sort.Sort(reversed.AsSpan(), stats);

        // LSD Radix Sort on reversed non-negative data:
        // Same as sorted - performance is data-independent O(d × n)
        var digitCount = 4;
        var expectedReads = (ulong)(n + digitCount * 3 * n);
        var expectedWrites = (ulong)(digitCount * 2 * n);

        Assert.Equal(expectedReads, stats.IndexReadCount);
        Assert.Equal(expectedWrites, stats.IndexWriteCount);
        Assert.Equal(0UL, stats.CompareCount);
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
        RadixLSD4Sort.Sort(random.AsSpan(), stats);

        // LSD Radix Sort on random non-negative data:
        // Same complexity as sorted/reversed - O(d × n)
        var digitCount = 4;
        var expectedReads = (ulong)(n + digitCount * 3 * n);
        var expectedWrites = (ulong)(digitCount * 2 * n);

        Assert.Equal(expectedReads, stats.IndexReadCount);
        Assert.Equal(expectedWrites, stats.IndexWriteCount);
        Assert.Equal(0UL, stats.CompareCount);
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
        // Mix of negative and non-negative: [-n/2, ..., -1, 0, 1, ..., n/2-1]
        var mixed = Enumerable.Range(-n / 2, n).ToArray();
        RadixLSD4Sort.Sort(mixed.AsSpan(), stats);

        // With negative numbers:
        // 1. Check for negatives: 1 read (early exit on first negative)
        // 2. Separate: n reads (from main) + n writes (to neg/nonneg buffers via SortSpan)
        // 3. Sort negative subset (digitCount passes)
        // 4. Sort non-negative subset (digitCount passes)
        // 5. Merge: n reads (from neg/nonneg buffers) + n writes (to main)
        //
        // For input [-n/2, ..., -1, 0, 1, ..., n/2-1]:
        // - Negative count = n/2
        // - Non-negative count = n/2
        var negativeCount = n / 2;
        var nonNegativeCount = n - negativeCount;
        var digitCount = 4;

        // Reads:
        //   1. Check: 1
        //   2. Separate: n (read from main)
        //   3. Sort negatives: digitCount × 3 × negativeCount
        //   4. Sort non-negatives: digitCount × 3 × nonNegativeCount
        //   5. Merge: n (read from buffers)
        // Writes:
        //   1. Separate: n (write to neg/nonneg buffers)
        //   2. Sort negatives: digitCount × 2 × negativeCount
        //   3. Sort non-negatives: digitCount × 2 × nonNegativeCount
        //   4. Merge: n (write to main)
        var expectedReads = (ulong)(
            1 // Check for negatives (early exit)
            + n // Separate (read from main)
            + digitCount * 3 * negativeCount // Sort negatives
            + digitCount * 3 * nonNegativeCount // Sort non-negatives
            + n // Merge (read from buffers)
        );
        
        var expectedWrites = (ulong)(
            n // Separate (write to buffers)
            + digitCount * 2 * negativeCount // Sort negatives
            + digitCount * 2 * nonNegativeCount // Sort non-negatives
            + n // Merge (write to main)
        );

        Assert.Equal(expectedReads, stats.IndexReadCount);
        Assert.Equal(expectedWrites, stats.IndexWriteCount);
        Assert.Equal(0UL, stats.CompareCount);
        Assert.Equal(0UL, stats.SwapCount);
        
        // Verify result is sorted
        Assert.Equal(mixed.OrderBy(x => x), mixed);
    }
}
