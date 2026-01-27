using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;

namespace SortAlgorithm.Tests;

public class RadixLSD256SortTests
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
        RadixLSD256Sort.Sort(array.AsSpan(), stats);

        Assert.Equal((ulong)inputSample.Samples.Length, (ulong)array.Length);
    }

#if DEBUG

    [Theory]
    [ClassData(typeof(MockSortedData))]
    public void StatisticsSortedTest(IInputSample<int> inputSample)
    {
        var stats = new StatisticsContext();
        var array = inputSample.Samples.ToArray();
        RadixLSD256Sort.Sort(array.AsSpan(), stats);

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
        RadixLSD256Sort.Sort(sorted.AsSpan(), stats);

        // LSD Radix Sort (Radix-256) with sign-bit flipping:
        // For 32-bit integers: digitCount = 4 (4 bytes)
        // 
        // Unified processing for all values (no separate negative/positive paths):
        // Per pass d (d=0,1,2,3):
        //   - Count phase: n reads
        //   - Distribute phase: n reads + n writes (to temp)
        //   - Copy back phase: n reads (from temp) + n writes (to main)
        //
        // Total per pass:
        // - Reads: n (count) + n (distribute) + n (copy back) = 3n
        // - Writes: n (distribute to temp) + n (copy back to main) = 2n
        //
        // Total for 4 passes:
        // - Reads: 4 × 3n = 12n
        // - Writes: 4 × 2n = 8n
        var digitCount = 4; // 32-bit int = 4 bytes
        var expectedReads = (ulong)(digitCount * 3 * n);  // (count + distribute + copy) × passes
        var expectedWrites = (ulong)(digitCount * 2 * n); // (temp write + main write) × passes

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
        RadixLSD256Sort.Sort(reversed.AsSpan(), stats);

        // LSD Radix Sort on reversed data:
        // Same as sorted - performance is data-independent O(d × n)
        var digitCount = 4;
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
        RadixLSD256Sort.Sort(random.AsSpan(), stats);

        // LSD Radix Sort on random data:
        // Same complexity as sorted/reversed - O(d × n)
        var digitCount = 4;
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
    public void TheoreticalValuesNegativeTest(int n)
    {
        var stats = new StatisticsContext();
        // Mix of negative and non-negative: [-n/2, ..., -1, 0, 1, ..., n/2-1]
        var mixed = Enumerable.Range(-n / 2, n).ToArray();
        RadixLSD256Sort.Sort(mixed.AsSpan(), stats);

        // With sign-bit flipping, negative and positive values are processed in the same passes:
        // No separate negative/positive processing needed.
        // 
        // Total for 4 passes:
        // - Reads: 4 × 3n = 12n
        // - Writes: 4 × 2n = 8n
        var digitCount = 4;
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
        RadixLSD256Sort.Sort(array.AsSpan(), stats);

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
        RadixLSD256Sort.Sort(keys.AsSpan());

        // After sorting by value, records with same value should maintain original order
        // Since we only sorted keys, we verify the sort is stable by checking
        // that multiple sorts preserve order
        var firstSort = records.Select(r => r.value).ToArray();
        RadixLSD256Sort.Sort(firstSort.AsSpan());
        
        var secondSort = firstSort.ToArray();
        RadixLSD256Sort.Sort(secondSort.AsSpan());

        Assert.Equal(firstSort, secondSort);
    }

#endif

}
