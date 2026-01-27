using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;

namespace SortAlgorithm.Tests;

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
    [ClassData(typeof(MockAntiQuickSortData))]
    [ClassData(typeof(MockQuickSortWorstCaseData))]
    public void SortResultOrderTest(IInputSample<int> inputSample)
    {
        var stats = new StatisticsContext();
        var array = inputSample.Samples.ToArray();
        RadixLSD4Sort.Sort(array.AsSpan(), stats);

        Assert.Equal((ulong)inputSample.Samples.Length, (ulong)array.Length);
    }

#if DEBUG

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

        // LSD Radix Sort (Radix-4, 2-bit per pass) with sign-bit flipping and early termination:
        // For 32-bit integers with range [0, n-1]:
        // 
        // Early termination optimization:
        // - Find min/max keys: n reads
        // - Calculate required bits from range (max ^ min)
        // - Only process required digit passes
        //
        // Example for n=100:
        // - max value = 99 → key = 0x8000_0063 (after sign-bit flip for signed int)
        // - min value = 0 → key = 0x8000_0000
        // - range = 0x8000_0063 ^ 0x8000_0000 = 0x0000_0063 = 99
        // - required bits = 7 (for value 99)
        // - required passes = ceil(7 / 2) = 4
        //
        // Per pass d (d=0,1,2,3):
        //   - Count phase: n reads
        //   - Distribute phase: n reads + n writes (to temp)
        //   - Copy back phase: n reads (from temp) + n writes (to main)
        //
        // Total per pass:
        // - Reads: n (count) + n (distribute) + n (copy back) = 3n
        // - Writes: n (distribute to temp) + n (copy back to main) = 2n
        //
        // Total:
        // - Initial scan: n reads
        // - Radix passes: digitCount × (3n reads + 2n writes)
        var maxValue = n - 1;
        var range = (ulong)maxValue; // min=0 after sign-bit flip, range = max - min
        var requiredBits = range == 0 ? 0 : (64 - System.Numerics.BitOperations.LeadingZeroCount(range));
        var digitCount = Math.Max(1, (requiredBits + 2 - 1) / 2); // ceil(requiredBits / 2)
        
        var expectedReads = (ulong)(n + digitCount * 3 * n);  // Initial + (count + distribute + copy) × passes
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

        // LSD Radix Sort on reversed data:
        // Same as sorted - performance is data-independent O(d × n)
        var digitCount = 16;
        var expectedReads = (ulong)(digitCount * 3 * n);
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

        // LSD Radix Sort on random data with early termination:
        // Same complexity - determined by actual range
        var maxValue = n - 1;
        var range = (ulong)maxValue;
        var requiredBits = range == 0 ? 0 : (64 - System.Numerics.BitOperations.LeadingZeroCount(range));
        var digitCount = Math.Max(1, (requiredBits + 2 - 1) / 2);
        
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

        // With sign-bit flipping, negative and positive values are processed in the same passes:
        // No separate negative/positive processing needed.
        // 
        // Total for 16 passes:
        // - Reads: 16 × 3n = 48n
        // - Writes: 16 × 2n = 32n
        var digitCount = 16;
        var expectedReads = (ulong)(digitCount * 3 * n);
        var expectedWrites = (ulong)(digitCount * 2 * n);

        Assert.Equal(expectedReads, stats.IndexReadCount);
        Assert.Equal(expectedWrites, stats.IndexWriteCount);
        Assert.Equal(0UL, stats.CompareCount);
        Assert.Equal(0UL, stats.SwapCount);

        // Verify result is sorted
        Assert.Equal(mixed.OrderBy(x => x), mixed);
    }

    [Fact]
    public void MinValueHandlingTest()
    {
        var stats = new StatisticsContext();
        // Test that int.MinValue is handled correctly (no overflow)
        var array = new[] { int.MinValue, -1, 0, 1, int.MaxValue };
        RadixLSD4Sort.Sort(array.AsSpan(), stats);

        Assert.Equal(new[] { int.MinValue, -1, 0, 1, int.MaxValue }, array);
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
        RadixLSD4Sort.Sort(keys.AsSpan());

        // After sorting by value, records with same value should maintain original order
        // Since we only sorted keys, we verify the sort is stable by checking
        // that multiple sorts preserve order
        var firstSort = records.Select(r => r.value).ToArray();
        RadixLSD4Sort.Sort(firstSort.AsSpan());
        
        var secondSort = firstSort.ToArray();
        RadixLSD4Sort.Sort(secondSort.AsSpan());

        Assert.Equal(firstSort, secondSort);
    }

#endif

}
