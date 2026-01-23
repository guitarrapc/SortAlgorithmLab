using SortLab.Core.Contexts;

namespace SortLab.Core.Algorithms;

/*
Array ...

| Method   | Number | Mean        | Error       | StdDev    | Min         | Max         | Allocated |
|--------- |------- |------------:|------------:|----------:|------------:|------------:|----------:|
| HeapSort | 100    |    16.60 us |    10.16 us |  0.557 us |    16.10 us |    17.20 us |     448 B |
| HeapSort | 1000   |   226.70 us |    42.08 us |  2.307 us |   224.50 us |   229.10 us |     736 B |
| HeapSort | 10000  | 3,028.70 us | 1,625.10 us | 89.077 us | 2,960.20 us | 3,129.40 us |     736 B |

Span ...

| Method   | Number | Mean        | Error      | StdDev    | Min         | Max         | Allocated |
|--------- |------- |------------:|-----------:|----------:|------------:|------------:|----------:|
| HeapSort | 100    |    17.20 us |   5.473 us |  0.300 us |    16.90 us |    17.50 us |     448 B |
| HeapSort | 1000   |   254.60 us |  59.145 us |  3.242 us |   251.70 us |   258.10 us |     736 B |
| HeapSort | 10000  | 3,375.50 us | 330.347 us | 18.107 us | 3,357.70 us | 3,393.90 us |     736 B |

Ref span (Recursive heapify) ...

| Method        | Number | Mean          | Error          | StdDev        | Median        | Min          | Max           | Allocated |
|-------------- |------- |--------------:|---------------:|--------------:|--------------:|-------------:|--------------:|----------:|
| HeapSort      | 100    |      13.57 us |       4.213 us |      0.231 us |      13.70 us |     13.30 us |      13.70 us |     736 B |
| HeapSort      | 1000   |     195.50 us |      11.393 us |      0.624 us |     195.30 us |    195.00 us |     196.20 us |     736 B |
| HeapSort      | 10000  |   2,571.93 us |      83.948 us |      4.601 us |   2,571.80 us |  2,567.40 us |   2,576.60 us |     448 B |

Ref span ...

| Method        | Number | Mean          | Error          | StdDev        | Median        | Min          | Max           | Allocated |
|-------------- |------- |--------------:|---------------:|--------------:|--------------:|-------------:|--------------:|----------:|
| HeapSort      | 100    |      14.13 us |       3.798 us |      0.208 us |      14.20 us |     13.90 us |      14.30 us |     736 B |
| HeapSort      | 1000   |     252.43 us |      18.989 us |      1.041 us |     252.10 us |    251.60 us |     253.60 us |     736 B |
| HeapSort      | 10000  |     649.33 us |     843.733 us |     46.248 us |     653.60 us |    601.10 us |     693.30 us |     736 B |

Span ...

| Method        | Number | Mean          | Error          | StdDev       | Median       | Min          | Max           | Allocated |
|-------------- |------- |--------------:|---------------:|-------------:|-------------:|-------------:|--------------:|----------:|
| HeapSort      | 100    |      15.40 us |       6.320 us |     0.346 us |     15.60 us |     15.00 us |      15.60 us |     736 B |
| HeapSort      | 1000   |     279.03 us |      28.400 us |     1.557 us |    279.20 us |    277.40 us |     280.50 us |     736 B |
| HeapSort      | 10000  |     545.52 us |     109.346 us |     5.994 us |    543.05 us |    541.15 us |     552.35 us |     736 B |

*/

/// <summary>
/// 配列から、常に最大の要素をルートにもつヒープ（二分ヒープ）を作成します（この時点で不安定）。
/// その後、ルート要素をソート済み配列の末尾に移動し、ヒープの末端をルートに持ってきて再度ヒープ構造を維持します。これを繰り返すことで、ヒープの最大値が常にルートに保たれ、ソート済み配列に追加されることで自然とソートが行われます。
/// <br/>
/// Builds a heap (binary heap) from the array where the root always contains the maximum element (which is inherently unstable).
/// Then, the root element is moved to the end of the sorted array, the last element is moved to the root, and the heap structure is re-established. Repeating this process ensures that the maximum value in the heap is always at the root, allowing elements to be naturally sorted as they are moved to the sorted array.
/// </summary>
/// <remarks>
/// <para><strong>Theoretical Conditions for Correct Heapsort:</strong></para>
/// <list type="number">
/// <item><description><strong>Heap Property Maintenance:</strong> For a max-heap, every parent node must be greater than or equal to its children.
/// For array index i, left child is at 2i+1 and right child is at 2i+2. This implementation correctly maintains this property through the iterative heapify operation.</description></item>
/// <item><description><strong>Build Heap Phase:</strong> The initial heap construction starts from the last non-leaf node (n/2 - 1) and heapifies downward to index 0.
/// This bottom-up approach runs in O(n) time, which is more efficient than the naive O(n log n) top-down construction.</description></item>
/// <item><description><strong>Extract Max Phase:</strong> Repeatedly swap the root (maximum element) with the last element, reduce heap size, and re-heapify.
/// This phase performs n-1 extractions, each requiring O(log n) heapify operations, totaling O(n log n).</description></item>
/// <item><description><strong>Heapify Operation:</strong> Uses an iterative (non-recursive) sift-down approach to restore heap property.
/// Compares parent with both children, swaps with the larger child if needed, and continues down the tree until heap property is satisfied.</description></item>
/// </list>
/// <para><strong>Performance Characteristics:</strong></para>
/// <list type="bullet">
/// <item><description>Family      : Heap / Selection</description></item>
/// <item><description>Stable      : No (swapping elements by index breaks relative order)</description></item>
/// <item><description>In-place    : Yes (O(1) auxiliary space)</description></item>
/// <item><description>Best case   : Ω(n log n) - Even for sorted input, heap construction and extraction are required</description></item>
/// <item><description>Average case: Θ(n log n) - Build heap O(n) + n-1 extractions with O(log n) heapify each</description></item>
/// <item><description>Worst case  : O(n log n) - Guaranteed upper bound regardless of input distribution</description></item>
/// <item><description>Comparisons : ~2n log n - Approximately 2 comparisons per heapify (left and right child checks)</description></item>
/// <item><description>Swaps       : ~n log n - One swap per level during heapify, averaged across all operations</description></item>
/// <item><description>Cache       : Poor locality - Heap structure causes frequent cache misses due to non-sequential access</description></item>
/// </list>
/// <para><strong>Why "Heap / Selection" Family?:</strong></para>
/// <para>
/// HeapSort belongs to the Selection sort family. Like Selection Sort, it repeatedly 
/// selects the maximum element and places it at the end of the sorted portion. 
/// The key difference is the selection mechanism:
/// </para>
/// <list type="bullet">
/// <item><description>Selection Sort: Linear search O(n) to find maximum</description></item>
/// <item><description>Heap Sort: Heap structure O(log n) to extract maximum</description></item>
/// </list>
/// <para>
/// Thus, HeapSort is essentially an optimized Selection Sort using a heap data structure,
/// improving time complexity from O(n²) to O(n log n).
/// </para>
/// <para><strong>Implementation Notes:</strong></para>
/// <list type="bullet">
/// <item><description>Uses iterative heapify (loop) instead of recursive for better performance and stack safety</description></item>
/// <item><description>Builds max-heap for ascending sort (min-heap would produce descending order)</description></item>
/// <item><description>Comparison-based algorithm: requires O(n log n) comparisons in all cases</description></item>
/// <item><description>Despite O(n log n) guarantee, often slower than Quicksort in practice due to poor cache performance</description></item>
/// </list>
/// </remarks>

public static class HeapSort
{
    // Buffer identifiers for visualization
    private const int BUFFER_MAIN = 0;       // Main input array

    /// <summary>
    /// Sorts the elements in the specified span in ascending order using the default comparer.
    /// </summary>
    /// <typeparam name="T">The type of elements in the span. Must implement <see cref="IComparable{T}"/>.</typeparam>
    /// <param name="span">The span of elements to sort in place.</param>
    public static void Sort<T>(Span<T> span) where T : IComparable<T>
    {
        Sort(span, 0, span.Length, NullContext.Default);
    }

    /// <summary>
    /// Sorts the elements in the specified span using the provided sort context.
    /// </summary>
    /// <typeparam name="T">The type of elements in the span. Must implement <see cref="IComparable{T}"/>.</typeparam>
    /// <param name="span">The span of elements to sort. The elements within this span will be reordered in place.</param>
    /// <param name="context">The sort context that defines the sorting strategy or options to use during the operation. Cannot be null.</param>
    public static void Sort<T>(Span<T> span, ISortContext context) where T : IComparable<T>
    {
        Sort(span, 0, span.Length, context);
    }

    /// <summary>
    /// Sorts the subrange [first..last) using the provided sort context.
    /// </summary>
    /// <typeparam name="T">The type of elements in the span. Must implement <see cref="IComparable{T}"/>.</typeparam>
    /// <param name="span">The span containing the elements to sort.</param>
    /// <param name="first">The zero-based index of the first element in the range to sort.</param>
    /// <param name="last">The exclusive upper bound of the range to sort (one past the last element).</param>
    /// <param name="context">The sort context to use during the sorting operation for tracking statistics and visualization.</param>
    public static void Sort<T>(Span<T> span, int first, int last, ISortContext context) where T : IComparable<T>
    {
        ArgumentOutOfRangeException.ThrowIfNegative(first);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(last, span.Length);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(first, last);

        if (last - first <= 1) return;

        var s = new SortSpan<T>(span, context, BUFFER_MAIN);
        SortCore(s, first, last);
    }

    /// <summary>
    /// Sorts the subrange [first..last) using the provided sort context.
    /// This overload accepts a SortSpan directly for use by other algorithms that already have a SortSpan instance.
    /// </summary>
    /// <typeparam name="T">The type of elements in the span. Must implement <see cref="IComparable{T}"/>.</typeparam>
    /// <param name="s">The SortSpan wrapping the span to sort.</param>
    /// <param name="first">The inclusive start index of the range to sort.</param>
    /// <param name="last">The exclusive end index of the range to sort.</param>
    internal static void SortCore<T>(SortSpan<T> s, int first, int last) where T : IComparable<T>
    {
        var n = last - first;

        // Build heap
        for (var i = first + n / 2 - 1; i >= first; i--)
        {
            Heapify(s, i, n, first);
        }

        // Extract elements from heap
        for (var i = last - 1; i > first; i--)
        {
            // Move current root to end
            s.Swap(first, i);

            // Re-heapify the reduced heap
            Heapify(s, first, i - first, first);
        }
    }

    /// <summary>
    /// Restores the heap property for a subtree rooted at the specified index using iterative sift-down.
    /// </summary>
    /// <typeparam name="T">The type of elements in the span. Must implement <see cref="IComparable{T}"/>.</typeparam>
    /// <param name="s">The SortSpan containing the elements and context for tracking operations.</param>
    /// <param name="root">The index of the root node of the subtree to heapify.</param>
    /// <param name="size">The size of the heap (number of elements to consider).</param>
    /// <param name="offset">The starting index offset for the heap within the span.</param>
    /// <remarks>
    /// This method implements the sift-down operation to maintain the max-heap property.
    /// It iteratively compares the parent node with its left and right children, swapping with the larger child if needed,
    /// and continues down the tree until the heap property is satisfied or a leaf node is reached.
    /// <para>Time Complexity: O(log n) - Worst case traverses from root to leaf (height of the tree).</para>
    /// <para>Space Complexity: O(1) - Uses iteration instead of recursion.</para>
    /// </remarks>
    private static void Heapify<T>(SortSpan<T> s, int root, int size, int offset) where T : IComparable<T>
    {
        while (true)
        {
            var largest = root;
            var left = 2 * (root - offset) + 1 + offset;
            var right = 2 * (root - offset) + 2 + offset;

            // If left child is larger than root
            if (left < offset + size && s.Compare(left, largest) > 0)
            {
                largest = left;
            }

            // If right child is larger than largest so far
            if (right < offset + size && s.Compare(right, largest) > 0)
            {
                largest = right;
            }

            // If largest is not root, swap and heapify the affected sub-tree
            if (largest != root)
            {
                s.Swap(root, largest);
                root = largest;
            }
            else
            {
                break;
            }
        }
    }
}
