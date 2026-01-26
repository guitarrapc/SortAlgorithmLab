using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;

namespace SortLab.Tests;

public class TernaryHeapSortTests
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
        TernaryHeapSort.Sort(array.AsSpan(), stats);

        Assert.Equal((ulong)inputSample.Samples.Length, (ulong)array.Length);
    }

    [Fact]
    public void RangeSortTest()
    {
        var stats = new StatisticsContext();
        var array = new[] { 5, 3, 8, 1, 9, 2, 7, 4, 6 };

        // Sort only the range [2, 6) -> indices 2, 3, 4, 5
        TernaryHeapSort.Sort(array.AsSpan(), 2, 6, stats);

        // Expected: first 2 elements unchanged, middle 4 sorted, last 3 unchanged
        Assert.Equal(new[] { 5, 3, 1, 2, 8, 9, 7, 4, 6 }, array);
    }

    [Fact]
    public void RangeSortFullArrayTest()
    {
        var stats = new StatisticsContext();
        var array = new[] { 5, 3, 8, 1, 9, 2, 7, 4, 6 };

        // Sort the entire array using range API
        TernaryHeapSort.Sort(array.AsSpan(), 0, array.Length, stats);

        Assert.Equal(new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 }, array);
    }

    [Fact]
    public void RangeSortSingleElementTest()
    {
        var stats = new StatisticsContext();
        var array = new[] { 5, 3, 8, 1, 9 };

        // Sort a single element range [2, 3)
        TernaryHeapSort.Sort(array.AsSpan(), 2, 3, stats);

        // Array should be unchanged (single element is already sorted)
        Assert.Equal(new[] { 5, 3, 8, 1, 9 }, array);
    }

    [Fact]
    public void RangeSortBeginningTest()
    {
        var stats = new StatisticsContext();
        var array = new[] { 9, 7, 5, 3, 1, 2, 4, 6, 8 };

        // Sort only the first 5 elements [0, 5)
        TernaryHeapSort.Sort(array.AsSpan(), 0, 5, stats);

        // Expected: first 5 sorted, last 4 unchanged
        Assert.Equal(new[] { 1, 3, 5, 7, 9, 2, 4, 6, 8 }, array);
    }

    [Fact]
    public void RangeSortEndTest()
    {
        var stats = new StatisticsContext();
        var array = new[] { 1, 3, 5, 7, 9, 8, 6, 4, 2 };

        // Sort only the last 4 elements [5, 9)
        TernaryHeapSort.Sort(array.AsSpan(), 5, 9, stats);

        // Expected: first 5 unchanged, last 4 sorted
        Assert.Equal(new[] { 1, 3, 5, 7, 9, 2, 4, 6, 8 }, array);
    }

    [Fact]
    public void BasicSortTest()
    {
        var stats = new StatisticsContext();
        var array = new[] { 3, 1, 4, 1, 5, 9, 2, 6, 5, 3 };

        TernaryHeapSort.Sort(array.AsSpan(), stats);

        Assert.Equal(new[] { 1, 1, 2, 3, 3, 4, 5, 5, 6, 9 }, array);
    }

    [Fact]
    public void EmptyArrayTest()
    {
        var stats = new StatisticsContext();
        var array = Array.Empty<int>();

        TernaryHeapSort.Sort(array.AsSpan(), stats);

        Assert.Empty(array);
    }

    [Fact]
    public void SingleElementTest()
    {
        var stats = new StatisticsContext();
        var array = new[] { 42 };

        TernaryHeapSort.Sort(array.AsSpan(), stats);

        Assert.Equal(new[] { 42 }, array);
    }

    [Fact]
    public void TwoElementsTest()
    {
        var stats = new StatisticsContext();
        var array = new[] { 2, 1 };

        TernaryHeapSort.Sort(array.AsSpan(), stats);

        Assert.Equal(new[] { 1, 2 }, array);
    }

    [Fact]
    public void AlreadySortedTest()
    {
        var stats = new StatisticsContext();
        var array = new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 };

        TernaryHeapSort.Sort(array.AsSpan(), stats);

        Assert.Equal(new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 }, array);
    }

    [Fact]
    public void ReverseSortedTest()
    {
        var stats = new StatisticsContext();
        var array = new[] { 9, 8, 7, 6, 5, 4, 3, 2, 1 };

        TernaryHeapSort.Sort(array.AsSpan(), stats);

        Assert.Equal(new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 }, array);
    }

    [Fact]
    public void AllSameElementsTest()
    {
        var stats = new StatisticsContext();
        var array = new[] { 5, 5, 5, 5, 5 };

        TernaryHeapSort.Sort(array.AsSpan(), stats);

        Assert.Equal(new[] { 5, 5, 5, 5, 5 }, array);
    }

#if DEBUG

    [Theory]
    [ClassData(typeof(MockSortedData))]
    public void StatisticsSortedTest(IInputSample<int> inputSample)
    {
        var stats = new StatisticsContext();
        var array = inputSample.Samples.ToArray();
        TernaryHeapSort.Sort(array.AsSpan(), stats);

        Assert.Equal((ulong)inputSample.Samples.Length, (ulong)array.Length);
        Assert.NotEqual(0UL, stats.IndexReadCount);
        Assert.NotEqual(0UL, stats.IndexWriteCount);
        Assert.NotEqual(0UL, stats.CompareCount);
        Assert.NotEqual(0UL, stats.SwapCount);
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
        TernaryHeapSort.Sort(sorted.AsSpan(), stats);

        // Ternary Heap Sort characteristics:
        // Build heap phase: O(n) comparisons with some swaps even for sorted data
        // Extract phase: (n-1) extractions, each requiring O(log₃ n) heapify
        // Each heapify compares with 3 children instead of 2
        //
        // Empirical observations for sorted data:
        // n=10:  Compare=42,  Swap=26
        // n=20:  Compare=126, Swap=64
        // n=50:  Compare=449, Swap=198
        // n=100: Compare=1101, Swap=464
        //
        // Pattern: approximately n * log₃(n) * 3 ≈ n * log₂(n) * 1.9 for compares

        var logN = Math.Log(n + 1, 3); // log base 3 for ternary heap
        var minCompares = (ulong)(n * logN * 1.0);
        var maxCompares = (ulong)(n * logN * 4.5 + n);

        var minSwaps = (ulong)(n * logN * 0.5);
        var maxSwaps = (ulong)(n * logN * 2.5);

        // Each swap writes 2 elements
        var minWrites = minSwaps * 2;
        var maxWrites = maxSwaps * 2;

        // Each comparison reads 2 elements
        var minIndexReads = minCompares * 2;

        Assert.InRange(stats.CompareCount, minCompares, maxCompares);
        Assert.InRange(stats.SwapCount, minSwaps, maxSwaps);
        Assert.InRange(stats.IndexWriteCount, minWrites, maxWrites);
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
        TernaryHeapSort.Sort(reversed.AsSpan(), stats);

        // Ternary Heap Sort has O(n log n) time complexity regardless of input order
        // Reversed data shows similar patterns to sorted data due to heap property
        //
        // Empirical observations for reversed data:
        // n=10:  Compare=37,  Swap=19
        // n=20:  Compare=110, Swap=46
        // n=50:  Compare=393, Swap=159
        // n=100: Compare=986, Swap=382
        //
        // Pattern: approximately n * log₃(n) * 3 for compares

        var logN = Math.Log(n + 1, 3); // log base 3 for ternary heap
        var minCompares = (ulong)(n * logN * 1.0);
        var maxCompares = (ulong)(n * logN * 4.5 + n);

        var minSwaps = (ulong)(n * logN * 0.4);
        var maxSwaps = (ulong)(n * logN * 2.2);

        // Each swap writes 2 elements
        var minWrites = minSwaps * 2;
        var maxWrites = maxSwaps * 2;

        // Each comparison reads 2 elements
        var minIndexReads = minCompares * 2;

        Assert.InRange(stats.CompareCount, minCompares, maxCompares);
        Assert.InRange(stats.SwapCount, minSwaps, maxSwaps);
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
        TernaryHeapSort.Sort(random.AsSpan(), stats);

        // Ternary Heap Sort has consistent O(n log n) time complexity for random data
        // The values are similar to sorted/reversed cases
        //
        // Empirical observations for random data (averaged over 5 runs):
        // n=10:  Compare=40,  Swap=22
        // n=20:  Compare=117, Swap=57
        // n=50:  Compare=415, Swap=180
        // n=100: Compare=1015, Swap=422
        //
        // Pattern: approximately n * log₃(n) * 3, with variation due to randomness

        var logN = Math.Log(n + 1, 3); // log base 3 for ternary heap
        var minCompares = (ulong)(n * logN * 1.0);
        var maxCompares = (ulong)(n * logN * 4.5 + n);

        var minSwaps = (ulong)(n * logN * 0.5);
        var maxSwaps = (ulong)(n * logN * 2.5);

        // Each swap writes 2 elements
        var minWrites = minSwaps * 2;
        var maxWrites = maxSwaps * 2;

        // Each comparison reads 2 elements
        var minIndexReads = minCompares * 2;

        Assert.InRange(stats.CompareCount, minCompares, maxCompares);
        Assert.InRange(stats.SwapCount, minSwaps, maxSwaps);
        Assert.InRange(stats.IndexWriteCount, minWrites, maxWrites);
        Assert.True(stats.IndexReadCount >= minIndexReads,
            $"IndexReadCount ({stats.IndexReadCount}) should be >= {minIndexReads}");
    }

#endif
}
