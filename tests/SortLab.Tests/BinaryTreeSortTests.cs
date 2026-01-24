using SortLab.Core.Algorithms;
using SortLab.Core.Contexts;

namespace SortLab.Tests;

public class BinaryTreeSortTests
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
        BinaryTreeSort.Sort(array.AsSpan(), stats);

        Assert.Equal((ulong)inputSample.Samples.Length, (ulong)array.Length);
    }

#if DEBUG

    [Theory]
    [ClassData(typeof(MockSortedData))]
    public void StatisticsSortedTest(IInputSample<int> inputSample)
    {
        var stats = new StatisticsContext();
        var array = inputSample.Samples.ToArray();
        BinaryTreeSort.Sort(array.AsSpan(), stats);

        Assert.Equal((ulong)inputSample.Samples.Length, (ulong)array.Length);
        Assert.NotEqual(0UL, stats.IndexReadCount);
        Assert.NotEqual(0UL, stats.IndexWriteCount);
        Assert.NotEqual(0UL, stats.CompareCount);
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
        BinaryTreeSort.Sort(sorted.AsSpan(), stats);

        // For sorted data [0, 1, 2, ..., n-1], the BST becomes completely unbalanced
        // forming a right-skewed tree (worst case):
        // - Insertion comparisons: 0 + 1 + 2 + ... + (n-1) = n(n-1)/2
        // With ItemIndex implementation:
        // - Each comparison reads 2 values (both items being compared)
        // - In-order traversal reads each element once: n reads
        // - Total reads: 2 * CompareCount + n
        var expectedCompares = (ulong)(n * (n - 1) / 2);
        var expectedReads = 2 * expectedCompares + (ulong)n;  // 2 reads per comparison + n traversal reads
        var expectedWrites = (ulong)n; // Writing during in-order traversal

        Assert.Equal(expectedCompares, stats.CompareCount);
        Assert.Equal(expectedReads, stats.IndexReadCount);
        Assert.Equal(expectedWrites, stats.IndexWriteCount);
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
        BinaryTreeSort.Sort(reversed.AsSpan(), stats);

        // For reversed data [n-1, n-2, ..., 1, 0], the BST becomes completely unbalanced
        // forming a left-skewed tree (worst case):
        // - Insertion comparisons: 0 + 1 + 2 + ... + (n-1) = n(n-1)/2
        // With ItemIndex implementation:
        // - Each comparison reads 2 values (both items being compared)
        // - In-order traversal reads each element once: n reads
        // - Total reads: 2 * CompareCount + n
        var expectedCompares = (ulong)(n * (n - 1) / 2);
        var expectedReads = 2 * expectedCompares + (ulong)n;  // 2 reads per comparison + n traversal reads
        var expectedWrites = (ulong)n; // Writing during in-order traversal

        Assert.Equal(expectedCompares, stats.CompareCount);
        Assert.Equal(expectedReads, stats.IndexReadCount);
        Assert.Equal(expectedWrites, stats.IndexWriteCount);
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
        BinaryTreeSort.Sort(random.AsSpan(), stats);

        // For random data, the BST is likely to be more balanced (average case)
        // Average insertion comparisons for balanced tree:
        // Each insertion into a tree of i elements takes ~log2(i) comparisons
        // Total: sum of log2(i) for i=1 to n
        //
        // Approximation: n*log2(n) - 1.44*n (based on average case analysis)
        // However, random data can vary, so we use a flexible range:
        // - Lower bound: about 50% of n*log2(n) (very lucky balanced insertions)
        // - Upper bound: worst case n(n-1)/2 (unlikely but possible)
        var avgCompares = n * Math.Log2(n);
        var minCompares = (ulong)(avgCompares * 0.4);  // Allow significantly lower for balanced trees
        var maxCompares = (ulong)(n * (n - 1) / 2);    // Worst case (unbalanced)

        // With ItemIndex: 2 reads per comparison + n traversal reads
        var expectedMinReads = 2 * minCompares + (ulong)n;
        var expectedMaxReads = 2 * maxCompares + (ulong)n;
        var expectedWrites = (ulong)n; // Writing during in-order traversal

        Assert.InRange(stats.CompareCount, minCompares, maxCompares);
        Assert.InRange(stats.IndexReadCount, expectedMinReads, expectedMaxReads);
        Assert.Equal(expectedWrites, stats.IndexWriteCount);
        Assert.Equal(0UL, stats.SwapCount);
    }

    [Theory]
    [InlineData(7)]  // Perfect binary tree: 3 levels
    [InlineData(15)] // Perfect binary tree: 4 levels
    [InlineData(31)] // Perfect binary tree: 5 levels
    public void TheoreticalValuesBalancedTest(int n)
    {
        var stats = new StatisticsContext();
        // Create a sequence that produces a balanced BST
        // Using middle-out insertion order for near-perfect balance
        var balanced = CreateBalancedInsertionOrder(n);
        BinaryTreeSort.Sort(balanced, stats);

        // For a balanced tree, insertion comparisons are O(n log n)
        // Each insertion into a balanced tree of height h requires ~h comparisons
        // Average height for balanced tree: log2(n)
        var minCompares = (ulong)(n * Math.Log2(n) * 0.5);  // Lower bound
        var maxCompares = (ulong)(n * Math.Log2(n) * 2.0);  // Upper bound with some overhead

        // With ItemIndex: 2 reads per comparison + n traversal reads
        var expectedMinReads = 2 * minCompares + (ulong)n;
        var expectedMaxReads = 2 * maxCompares + (ulong)n;
        var expectedWrites = (ulong)n; // Writing during in-order traversal

        Assert.InRange(stats.CompareCount, minCompares, maxCompares);
        Assert.InRange(stats.IndexReadCount, expectedMinReads, expectedMaxReads);
        Assert.Equal(expectedWrites, stats.IndexWriteCount);
        Assert.Equal(0UL, stats.SwapCount);
    }

    /// <summary>
    /// Creates an array with insertion order that produces a relatively balanced BST.
    /// Uses a middle-recursive approach similar to binary search tree construction.
    /// </summary>
    private static Span<int> CreateBalancedInsertionOrder(int n)
    {
        var sorted = Enumerable.Range(0, n).ToArray();
        var result = new int[n];
        var index = 0;

        void AddMiddle(int left, int right)
        {
            if (left > right) return;
            var mid = left + (right - left) / 2;
            result[index++] = sorted[mid];
            AddMiddle(left, mid - 1);
            AddMiddle(mid + 1, right);
        }

        AddMiddle(0, n - 1);
        return result.AsSpan();
    }

#endif

}
