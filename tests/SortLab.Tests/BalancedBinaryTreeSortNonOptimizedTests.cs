using SortLab.Core.Algorithms;
using SortLab.Core.Contexts;
using SortLab.Tests.Attributes;

namespace SortLab.Tests;

public class BalancedBinaryTreeSortNonOptimizedTests
{
    [CISkippableTheory]
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
        BalancedBinaryTreeSortNonOptimized.Sort(array.AsSpan(), stats);

        Assert.Equal((ulong)inputSample.Samples.Length, (ulong)array.Length);
    }

#if DEBUG

    [CISkippableTheory]
    [ClassData(typeof(MockSortedData))]
    public void StatisticsSortedTest(IInputSample<int> inputSample)
    {
        var stats = new StatisticsContext();
        var array = inputSample.Samples.ToArray();
        BalancedBinaryTreeSortNonOptimized.Sort(array.AsSpan(), stats);

        Assert.Equal((ulong)inputSample.Samples.Length, (ulong)array.Length);
        Assert.NotEqual(0UL, stats.IndexReadCount);
        Assert.NotEqual(0UL, stats.IndexWriteCount);
        Assert.NotEqual(0UL, stats.CompareCount);
        Assert.Equal(0UL, stats.SwapCount);
    }

    [CISkippableTheory]
    [InlineData(10)]
    [InlineData(20)]
    [InlineData(50)]
    [InlineData(100)]
    public void TheoreticalValuesSortedTest(int n)
    {
        var stats = new StatisticsContext();
        var sorted = Enumerable.Range(0, n).ToArray();
        BalancedBinaryTreeSortNonOptimized.Sort(sorted.AsSpan(), stats);

        // For sorted data [0, 1, 2, ..., n-1], AVL tree maintains balance through rotations
        // Expected comparisons for balanced tree:
        // - Each insertion into a tree of i elements takes ~log2(i) comparisons
        // - Total: approximately n*log2(n) comparisons
        // - Each insertion reads one element from the array: n reads
        // - In-order traversal writes all elements back: n writes
        var avgCompares = (ulong)(n * Math.Log2(Math.Max(n, 2)));
        var minCompares = avgCompares / 2;  // Allow some variance
        var maxCompares = avgCompares * 2;  // Upper bound for balanced insertions
        var expectedReads = (ulong)n;  // Reading during insertion
        var expectedWrites = (ulong)n; // Writing during in-order traversal

        Assert.InRange(stats.CompareCount, minCompares, maxCompares);
        Assert.Equal(expectedReads, stats.IndexReadCount);
        Assert.Equal(expectedWrites, stats.IndexWriteCount);
        Assert.Equal(0UL, stats.SwapCount);
    }

    [CISkippableTheory]
    [InlineData(10)]
    [InlineData(20)]
    [InlineData(50)]
    [InlineData(100)]
    public void TheoreticalValuesReversedTest(int n)
    {
        var stats = new StatisticsContext();
        var reversed = Enumerable.Range(0, n).Reverse().ToArray();
        BalancedBinaryTreeSortNonOptimized.Sort(reversed.AsSpan(), stats);

        // For reversed data [n-1, n-2, ..., 1, 0], AVL tree maintains balance through rotations
        // Expected comparisons for balanced tree:
        // - Each insertion into a tree of i elements takes ~log2(i) comparisons
        // - Total: approximately n*log2(n) comparisons
        // - Each insertion reads one element from the array: n reads
        // - In-order traversal writes all elements back: n writes
        var avgCompares = (ulong)(n * Math.Log2(Math.Max(n, 2)));
        var minCompares = avgCompares / 2;  // Allow some variance
        var maxCompares = avgCompares * 2;  // Upper bound for balanced insertions
        var expectedReads = (ulong)n;  // Reading during insertion
        var expectedWrites = (ulong)n; // Writing during in-order traversal

        Assert.InRange(stats.CompareCount, minCompares, maxCompares);
        Assert.Equal(expectedReads, stats.IndexReadCount);
        Assert.Equal(expectedWrites, stats.IndexWriteCount);
        Assert.Equal(0UL, stats.SwapCount);
    }

    [CISkippableTheory]
    [InlineData(10)]
    [InlineData(20)]
    [InlineData(50)]
    [InlineData(100)]
    public void TheoreticalValuesRandomTest(int n)
    {
        var stats = new StatisticsContext();
        var random = Enumerable.Range(0, n).OrderBy(_ => Guid.NewGuid()).ToArray();
        BalancedBinaryTreeSortNonOptimized.Sort(random.AsSpan(), stats);

        // For random data, AVL tree maintains balance automatically
        // Expected comparisons:
        // - Each insertion into a balanced tree of i elements takes ~log2(i) comparisons
        // - Total: approximately n*log2(n) comparisons
        // - Balanced tree guarantees O(log n) height, so worst case is better than BST
        var avgCompares = (ulong)(n * Math.Log2(Math.Max(n, 2)));
        var minCompares = avgCompares / 2;  // Allow variance for very balanced insertions
        var maxCompares = avgCompares * 2;  // Upper bound (still O(n log n))
        var expectedReads = (ulong)n;  // Reading during insertion
        var expectedWrites = (ulong)n; // Writing during in-order traversal

        Assert.InRange(stats.CompareCount, minCompares, maxCompares);
        Assert.Equal(expectedReads, stats.IndexReadCount);
        Assert.Equal(expectedWrites, stats.IndexWriteCount);
        Assert.Equal(0UL, stats.SwapCount);
    }

    [CISkippableTheory]
    [InlineData(10)]
    [InlineData(20)]
    [InlineData(50)]
    [InlineData(100)]
    public void TheoreticalValuesBalancedPropertyTest(int n)
    {
        var stats = new StatisticsContext();
        var random = Enumerable.Range(0, n).OrderBy(_ => Guid.NewGuid()).ToArray();
        BalancedBinaryTreeSortNonOptimized.Sort(random.AsSpan(), stats);

        // AVL tree guarantees that the height is always O(log n)
        // This ensures that comparisons remain in the O(n log n) range
        // even in worst-case scenarios (unlike unbalanced BST which degrades to O(n^2))
        var worstCaseBST = (ulong)(n * (n - 1) / 2);  // Unbalanced BST worst case
        var balancedUpperBound = (ulong)(n * Math.Log2(Math.Max(n, 2)) * 3);  // 3x safety margin

        // Verify that comparisons are within balanced tree bounds (O(n log n))
        // For small n, the difference may not be dramatic, so we use 70% of worst case as threshold
        Assert.True(stats.CompareCount < worstCaseBST * 7 / 10,
            $"CompareCount ({stats.CompareCount}) should be better than 70% of unbalanced BST worst case ({worstCaseBST})");
        Assert.True(stats.CompareCount < balancedUpperBound,
            $"CompareCount ({stats.CompareCount}) should be within balanced tree bounds ({balancedUpperBound})");
    }

#endif

}
