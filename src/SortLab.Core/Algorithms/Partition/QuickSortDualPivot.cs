using SortLab.Core.Contexts;
using System.Diagnostics;

namespace SortLab.Core.Algorithms;

/// <summary>
/// 2つのピボットを使用して配列を3つの領域に分割する分割統治法のソートアルゴリズムです。
/// 単一ピボットのQuickSortと比較して、より均等な分割により再帰の深さを浅くし、キャッシュ効率を高めることで高速化を実現します。
/// Java 7以降の標準ソートアルゴリズム（DualPivotQuicksort）として採用されています。
/// <br/>
/// A divide-and-conquer sorting algorithm that uses two pivots to partition the array into three regions.
/// Compared to single-pivot QuickSort, it achieves faster performance through more balanced partitioning, reducing recursion depth and improving cache efficiency.
/// Adopted as the standard sorting algorithm in Java 7 and later (DualPivotQuicksort).
/// </summary>
/// <remarks>
/// <para><strong>Theoretical Conditions for Correct Dual-Pivot QuickSort:</strong></para>
/// <list type="number">
/// <item><description><strong>Pivot Selection and Ordering:</strong> Two pivots (p1, p2) are selected from the array, typically from the leftmost and rightmost positions.
/// The pivots must satisfy p1 ≤ p2 after initial comparison (line 43-46 ensures this invariant).</description></item>
/// <item><description><strong>Three-Way Partitioning:</strong> The array is partitioned into three regions:
/// <list type="bullet">
/// <item><description>Left region: elements &lt; p1 (indices [left, l-1])</description></item>
/// <item><description>Middle region: elements where p1 ≤ element ≤ p2 (indices [l+1, g-1])</description></item>
/// <item><description>Right region: elements &gt; p2 (indices [g+1, right])</description></item>
/// </list>
/// The partitioning loop (lines 53-70) maintains these invariants:
/// - Elements in [left+1, l-1] are &lt; p1
/// - Elements in [l, k-1] are in [p1, p2]
/// - Elements in [g+1, right-1] are &gt; p2
/// - Element at index k is currently being examined
/// </description></item>
/// <item><description><strong>Pivot Placement:</strong> After partitioning, pivots are moved to their final positions (lines 74-75):
/// - p1 is swapped with the element at position l (boundary of left region)
/// - p2 is swapped with the element at position g (boundary of right region)
/// This ensures pivots are correctly positioned between their respective regions.</description></item>
/// <item><description><strong>Recursive Division:</strong> The algorithm recursively sorts three independent regions (lines 78-83):
/// - Left region: [left, l-1]
/// - Middle region: [l+1, g-1] (only if p1 &lt; p2, i.e., pivots are distinct)
/// - Right region: [g+1, right]
/// Base case: when right ≤ left, the region has ≤ 1 element and is trivially sorted.</description></item>
/// <item><description><strong>Termination:</strong> The algorithm terminates because:
/// - Each recursive call operates on a strictly smaller subarray (at least 2 elements are pivots)
/// - The base case (right ≤ left) is eventually reached for all subarrays
/// - Maximum recursion depth: O(log₃ n) on average, O(n) in worst case</description></item>
/// </list>
/// <para><strong>Performance Characteristics:</strong></para>
/// <list type="bullet">
/// <item><description>Family      : Partitioning (Divide and Conquer)</description></item>
/// <item><description>Stable      : No (partitioning does not preserve relative order of equal elements)</description></item>
/// <item><description>In-place    : Yes (O(log n) auxiliary space for recursion stack)</description></item>
/// <item><description>Best case   : Θ(n log₃ n) - Balanced partitions (each region ≈ n/3)</description></item>
/// <item><description>Average case: Θ(n log₃ n) - Expected number of comparisons: 1.9n ln n ≈ 1.37n log₂ n (vs 2n ln n for single-pivot)</description></item>
/// <item><description>Worst case  : O(n²) - Occurs when partitioning is highly unbalanced (rare with dual pivots)</description></item>
/// <item><description>Comparisons : 1.9n ln n (average) - Each element compared with both pivots during partitioning</description></item>
/// <item><description>Swaps       : 0.6n ln n (average) - Fewer swaps than single-pivot due to better partitioning</description></item>
/// </list>
/// <para><strong>Advantages over Single-Pivot QuickSort:</strong></para>
/// <list type="bullet">
/// <item><description>More balanced partitions: log₃ n vs log₂ n recursion depth (≈37% reduction)</description></item>
/// <item><description>Fewer comparisons on average: 1.9n ln n vs 2n ln n (≈5% reduction)</description></item>
/// <item><description>Better cache locality: three regions fit better in CPU cache than two</description></item>
/// <item><description>Lower probability of worst-case behavior: dual pivots provide better sampling</description></item>
/// </list>
/// </remarks>
public static class QuickSortDualPivot
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
    /// <param name="context">The sort context that tracks statistics and provides sorting operations. Cannot be null.</param>
    public static void Sort<T>(Span<T> span, ISortContext context) where T : IComparable<T>
    {
        Sort(span, 0, span.Length, context);
    }

    /// <summary>
    /// Sorts the subrange [first..last) using the provided sort context.
    /// This overload is used internally for range-based sorting (e.g., by hybrid sort algorithms).
    /// </summary>
    /// <typeparam name="T">The type of elements in the span. Must implement <see cref="IComparable{T}"/>.</typeparam>
    /// <param name="span">The span containing elements to sort.</param>
    /// <param name="first">The inclusive start index of the range to sort.</param>
    /// <param name="last">The exclusive end index of the range to sort.</param>
    /// <param name="context">The sort context for tracking statistics and observations.</param>
    internal static void Sort<T>(Span<T> span, int first, int last, ISortContext context) where T : IComparable<T>
    {
        Debug.Assert(first >= 0 && last <= span.Length && first <= last, "Invalid range for sorting.");

        if (last - first <= 1) return;
        
        var s = new SortSpan<T>(span, context, BUFFER_MAIN);
        SortCore(s, first, last - 1);
    }

    private static void SortCore<T>(SortSpan<T> s, int left, int right) where T : IComparable<T>
    {
        if (right <= left) return;

        // Phase 0. Make sure left item is lower than right item
        if (s.Compare(left, right) > 0)
        {
            s.Swap(left, right);
        }

        // Phase 1. Partition array into three regions using dual pivots
        var l = left + 1;
        var k = l;
        var g = right - 1;

        while (k <= g)
        {
            if (s.Compare(k, left) < 0)
            {
                s.Swap(k, l);
                k++;
                l++;
            }
            else if (s.Compare(right, k) < 0)
            {
                s.Swap(k, g);
                g--;
            }
            else
            {
                k++;
            }
        }

        l--;
        g++;
        s.Swap(left, l);
        s.Swap(right, g);

        // Phase 2. Sort left, middle, and right regions recursively
        SortCore(s, left, l - 1);
        if (s.Compare(left, right) < 0)
        {
            SortCore(s, l + 1, g - 1);
        }
        SortCore(s, g + 1, right);
    }
}
