using System.Buffers;
using SortLab.Core.Contexts;

namespace SortLab.Core.Algorithms;

/// <summary>
/// 配列を再帰的に半分に分割し、それぞれをソートした後、ソート済みの部分配列をマージして全体をソートする分割統治アルゴリズムです。
/// 安定ソートであり、最悪・平均・最良のすべてのケースでO(n log n)の性能を保証します。
/// <br/>
/// Recursively divides the array in half, sorts each part, then merges the sorted subarrays to produce a fully sorted result.
/// This divide-and-conquer algorithm is stable and guarantees O(n log n) performance in all cases (worst, average, best).
/// </summary>
/// <remarks>
/// <para><strong>Theoretical Conditions for Correct Merge Sort:</strong></para>
/// <list type="number">
/// <item><description><strong>Divide Step (Binary Partitioning):</strong> The array must be divided into two roughly equal halves at each recursion level.
/// The midpoint is calculated as mid = (left + right) / 2, ensuring balanced subdivision.
/// This guarantees a recursion depth of ⌈log₂(n)⌉.</description></item>
/// <item><description><strong>Base Case (Termination Condition):</strong> Recursion must terminate when a subarray has size ≤ 1.
/// An array of size 0 or 1 is trivially sorted and requires no further processing.</description></item>
/// <item><description><strong>Conquer Step (Recursive Sorting):</strong> Each half must be sorted independently via recursive calls.
/// The left subarray [left..mid] and right subarray [mid+1..right] are sorted before merging.</description></item>
/// <item><description><strong>Merge Step (Sorted Subarray Combination):</strong> Two sorted subarrays must be merged into a single sorted array.
/// This requires O(n) auxiliary space to temporarily hold one half during the merge operation.
/// The merge compares elements from both halves and writes them in ascending order.</description></item>
/// <item><description><strong>Stability Preservation (Equal Element Ordering):</strong> When merging, elements from the left subarray must be taken first
/// when both sides have equal values (using &lt;= comparison). This preserves the relative order of equal elements, ensuring stability.</description></item>
/// <item><description><strong>Comparison Count:</strong> At each recursion level, merging n elements requires at most n-1 comparisons.
/// With ⌈log₂(n)⌉ levels, total comparisons: n⌈log₂(n)⌉ - 2^⌈log₂(n)⌉ + 1 (worst case).
/// Best case (sorted data): approximately n⌈log₂(n)⌉ / 2.</description></item>
/// </list>
/// <para><strong>Performance Characteristics:</strong></para>
/// <list type="bullet">
/// <item><description>Family      : Merge</description></item>
/// <item><description>Stable      : Yes (equal elements maintain relative order via &lt;= comparison during merge)</description></item>
/// <item><description>In-place    : No (requires O(n) auxiliary space for merging)</description></item>
/// <item><description>Best case   : O(n log n) - Even sorted data requires ⌈log₂(n)⌉ levels of merging</description></item>
/// <item><description>Average case: O(n log n) - Balanced recursion tree with n work per level</description></item>
/// <item><description>Worst case  : O(n log n) - Guaranteed balanced partitioning regardless of input</description></item>
/// <item><description>Comparisons : O(n log n) - At most n⌈log₂(n)⌉ - 2^⌈log₂(n)⌉ + 1 comparisons</description></item>
/// <item><description>Writes      : O(n log n) - Each level writes all n elements, ⌈log₂(n)⌉ levels total</description></item>
/// <item><description>Space       : O(n) - Auxiliary buffer of size n/2 for merging (this implementation uses ArrayPool for efficiency)</description></item>
/// </list>
/// <para><strong>Advantages of Merge Sort:</strong></para>
/// <list type="bullet">
/// <item><description>Predictable performance - O(n log n) guaranteed in all cases</description></item>
/// <item><description>Stable - Preserves relative order of equal elements</description></item>
/// <item><description>Parallelizable - Independent recursive branches can be processed concurrently</description></item>
/// <item><description>External sorting - Well-suited for sorting data that doesn't fit in memory</description></item>
/// </list>
/// <para><strong>Use Cases:</strong></para>
/// <list type="bullet">
/// <item><description>When stability is required (e.g., sorting records by multiple keys)</description></item>
/// <item><description>When predictable O(n log n) performance is essential</description></item>
/// <item><description>Linked list sorting (can be done in-place with O(1) space)</description></item>
/// <item><description>External sorting of large datasets</description></item>
/// </list>
/// </remarks>
public static class MergeSort
{
    // Buffer identifiers for visualization
    private const int BUFFER_MAIN = 0;       // Main input array
    private const int BUFFER_MERGE = 1;      // Merge buffer (auxiliary space)
    
    /// <summary>
    /// Sorts the elements in the specified span in ascending order using the default comparer.
    /// </summary>
    /// <typeparam name="T">The type of elements in the span. Must implement <see cref="IComparable{T}"/>.</typeparam>
    /// <param name="span">The span of elements to sort in place.</param>
    public static void Sort<T>(Span<T> span) where T : IComparable<T>
    {
        Sort(span, NullContext.Default);
    }

    /// <summary>
    /// Sorts the elements in the specified span using the provided sort context.
    /// </summary>
    /// <typeparam name="T">The type of elements in the span. Must implement <see cref="IComparable{T}"/>.</typeparam>
    /// <param name="span">The span of elements to sort. The elements within this span will be reordered in place.</param>
    /// <param name="context">The sort context that defines the sorting strategy or options to use during the operation. Cannot be null.</param>
    public static void Sort<T>(Span<T> span, ISortContext context) where T : IComparable<T>
    {
        if (span.Length <= 1) return;

        // Rent buffer from ArrayPool for O(n) auxiliary space (instead of O(n log n) stack allocations)
        var buffer = ArrayPool<T>.Shared.Rent(span.Length);
        try
        {
            SortCore(span, buffer.AsSpan(0, span.Length), context);
        }
        finally
        {
            ArrayPool<T>.Shared.Return(buffer);
        }
    }

    /// <summary>
    /// Core recursive merge sort implementation.
    /// </summary>
    /// <param name="span">The span to sort</param>
    /// <param name="buffer">Auxiliary buffer for merging (same size as span)</param>
    /// <param name="context">Sort context for statistics tracking</param>
    private static void SortCore<T>(Span<T> span, Span<T> buffer, ISortContext context) where T : IComparable<T>
    {
        if (span.Length <= 1) return; // Base case: array of size 0 or 1 is sorted

        var mid = span.Length / 2;
        var left = span.Slice(0, mid);
        var right = span.Slice(mid);

        // Conquer: Recursively sort left and right halves
        SortCore(left, buffer.Slice(0, mid), context);
        SortCore(right, buffer.Slice(mid), context);

        // Optimization: Skip merge if already sorted (left[last] <= right[first])
        // This dramatically improves performance on nearly-sorted data
        var s = new SortSpan<T>(span, context, BUFFER_MAIN);
        if (s.Compare(mid - 1, mid) <= 0)
        {
            return; // Already sorted, no merge needed
        }

        // Merge: Combine two sorted halves
        Merge(span, left, right, buffer, context);
    }

    /// <summary>
    /// Merges two sorted subarrays (left and right) into span using buffer as auxiliary space.
    /// </summary>
    private static void Merge<T>(Span<T> span, Span<T> left, Span<T> right, Span<T> buffer, ISortContext context) where T : IComparable<T>
    {
        var s = new SortSpan<T>(span, context, BUFFER_MAIN);
        var b = new SortSpan<T>(buffer.Slice(0, left.Length), context, BUFFER_MERGE);

        // Copy left partition to buffer to avoid overwriting during merge
        for (var i = 0; i < left.Length; i++)
        {
            b.Write(i, s.Read(i));
        }

        var l = 0;           // Index in buffer (left partition copy)
        var r = left.Length; // Index in span (right partition, starts after left)
        var k = 0;           // Index in result (span)

        // Merge: compare elements from buffer (left) and right partition
        // Optimization: Read both values once and reuse them to minimize indirect access
        while (l < left.Length && r < span.Length)
        {
            var leftValue = b.Read(l);
            var rightValue = s.Read(r);
            
            // Stability: use <= to take from left when equal
            // Use SortSpan.Compare(T, T) to track statistics while avoiding redundant Read
            if (s.Compare(leftValue, rightValue) <= 0)
            {
                s.Write(k, leftValue);
                l++;
            }
            else
            {
                s.Write(k, rightValue);
                r++;
            }
            k++;
        }

        // Copy remaining elements from buffer (left partition) if any
        while (l < left.Length)
        {
            s.Write(k, b.Read(l));
            l++;
            k++;
        }

        // Right partition elements are already in place, no need to copy
    }
}
