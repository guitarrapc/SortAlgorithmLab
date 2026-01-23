using SortLab.Core.Contexts;

namespace SortLab.Core.Algorithms;

/// <summary>
/// 2つのピボットによる分割統治法と、小規模配列での挿入ソートへの切り替えを組み合わせたハイブリッドソートアルゴリズムです。
/// Dual-Pivot QuickSortの高速な平均性能と、小規模配列における挿入ソートの低オーバーヘッド特性を組み合わせることで、実用的な性能を実現します。
/// <br/>
/// A hybrid sorting algorithm that combines dual-pivot partitioning with insertion sort for small subarrays.
/// Achieves practical performance by combining the fast average-case of Dual-Pivot QuickSort with the low overhead of insertion sort on small arrays.
/// </summary>
/// <remarks>
/// <para><strong>Theoretical Conditions for Correct Dual-Pivot QuickSort with Insertion Sort Hybrid:</strong></para>
/// <list type="number">
/// <item><description><strong>Threshold-Based Algorithm Selection:</strong> For subarrays of size ≤ InsertThreshold (16 elements), the algorithm switches to insertion sort (lines 52-56).
/// This is theoretically sound because:
/// <list type="bullet">
/// <item><description>Insertion sort has O(n²) worst-case but low constant factors and good cache locality</description></item>
/// <item><description>For small n (≤16), the overhead of partitioning and recursion exceeds the benefit</description></item>
/// <item><description>The threshold value is empirically determined (typical values: 10-47, Java uses 27-47 depending on data patterns)</description></item>
/// </list>
/// </description></item>
/// <item><description><strong>Pivot Selection and Ordering:</strong> Two pivots (p1, p2) are selected from the leftmost and rightmost positions.
/// The pivots must satisfy p1 ≤ p2 after initial comparison (line 59-62 ensures this invariant).</description></item>
/// <item><description><strong>Three-Way Partitioning:</strong> The array is partitioned into three regions:
/// <list type="bullet">
/// <item><description>Left region: elements &lt; p1 (indices [left, l-1])</description></item>
/// <item><description>Middle region: elements where p1 ≤ element ≤ p2 (indices [l+1, g-1])</description></item>
/// <item><description>Right region: elements &gt; p2 (indices [g+1, right])</description></item>
/// </list>
/// The partitioning loop (lines 65-82) maintains these invariants by examining each element k and placing it in the appropriate region.
/// </description></item>
/// <item><description><strong>Pivot Placement:</strong> After partitioning, pivots are moved to their final positions (lines 84-87):
/// - p1 is swapped to position l (boundary of left region)
/// - p2 is swapped to position g (boundary of right region)
/// This ensures pivots are correctly positioned between their respective regions.</description></item>
/// <item><description><strong>Recursive Division with Optimization:</strong> The algorithm recursively sorts three regions (lines 89-95):
/// - Left region: [left, l-1]
/// - Middle region: [l+1, g-1] - Only sorted if pivots are distinct (l &lt; g) AND there are elements between them (l+1 &lt; g-1, i.e., l+1 &lt; g)
/// - Right region: [g+1, right]
/// The middle region optimization (line 92) is theoretically correct because:
/// <list type="bullet">
/// <item><description>If l ≥ g, the pivots collided or crossed, meaning all elements equal the pivot value</description></item>
/// <item><description>If l = g-1, there are no elements between the pivots (they are adjacent)</description></item>
/// <item><description>Only when l &lt; g (equivalent to l+1 ≤ g) do we have elements that need sorting</description></item>
/// </list>
/// </description></item>
/// <item><description><strong>Termination:</strong> The algorithm terminates because:
/// - Base case 1: right ≤ left (≤1 element, trivially sorted)
/// - Base case 2: Small subarray (≤InsertThreshold) is delegated to insertion sort, which terminates in O(n²)
/// - Recursive case: Each partition strictly reduces subarray size (at least 2 elements become pivots)
/// - Maximum recursion depth: O(log₃ n) expected, O(n) worst case</description></item>
/// </list>
/// <para><strong>Performance Characteristics:</strong></para>
/// <list type="bullet">
/// <item><description>Family      : Hybrid (Partitioning + Insertion)</description></item>
/// <item><description>Stable      : No (neither dual-pivot partitioning nor insertion sort in this context preserves stability)</description></item>
/// <item><description>In-place    : Yes (O(log n) auxiliary space for recursion stack)</description></item>
/// <item><description>Best case   : Θ(n log n) - Balanced partitions with early cutoff to insertion sort</description></item>
/// <item><description>Average case: Θ(n log n) - Expected ~1.9n ln n comparisons from dual-pivot, plus insertion sort overhead on small subarrays</description></item>
/// <item><description>Worst case  : O(n²) - Highly unbalanced partitions or worst-case insertion sort behavior</description></item>
/// <item><description>Comparisons : ~1.9n ln n (average) for partitioning phase, plus O(InsertThreshold²) per small subarray</description></item>
/// <item><description>Swaps       : ~0.6n ln n (average) for partitioning phase, plus O(InsertThreshold²) per small subarray</description></item>
/// </list>
/// <para><strong>Differences from Java's DualPivotQuicksort:</strong></para>
/// <list type="bullet">
/// <item><description>Java uses more sophisticated pivot selection (sorting 5 sample elements to choose 2nd and 4th as pivots)</description></item>
/// <item><description>Java uses different thresholds (27 for quicksort, 47 for mergesort fallback, 286 for counting sort)</description></item>
/// <item><description>Java includes additional optimizations for partially sorted data and duplicate elements</description></item>
/// <item><description>This implementation is a simplified educational version focusing on core dual-pivot mechanics</description></item>
/// </list>
/// </remarks>
public static class QuickSortDualPivotInsertion
{
    private const int InsertThreshold = 16;

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
    /// </summary>
    /// <typeparam name="T">The type of elements in the span. Must implement <see cref="IComparable{T}"/>.</typeparam>
    /// <param name="span">The span containing elements to sort.</param>
    /// <param name="first">The inclusive start index of the range to sort.</param>
    /// <param name="last">The exclusive end index of the range to sort.</param>
    /// <param name="context">The sort context for tracking statistics and observations.</param>
    public static void Sort<T>(Span<T> span, int first, int last, ISortContext context) where T : IComparable<T>
    {
        ArgumentOutOfRangeException.ThrowIfNegative(first);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(last, span.Length);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(first, last);

        if (last - first <= 1) return;

        var s = new SortSpan<T>(span, context, BUFFER_MAIN);
        SortCore(s, first, last - 1);
    }

    /// <summary>
    /// Sorts the subrange [first..last) using the provided sort context.
    /// This overload accepts a SortSpan directly for use by other algorithms that already have a SortSpan instance.
    /// </summary>
    /// <typeparam name="T">The type of elements in the span. Must implement <see cref="IComparable{T}"/>.</typeparam>
    /// <param name="s">The SortSpan wrapping the span to sort.</param>
    /// <param name="left">The inclusive start index of the range to sort.</param>
    /// <param name="right">The exclusive end index of the range to sort.</param>
    internal static void SortCore<T>(SortSpan<T> s, int left, int right) where T : IComparable<T>
    {
        if (right <= left) return;

        // switch to insertion sort for small subarrays
        if (right - left + 1 <= InsertThreshold)
        {
            InsertionSort.SortCore(s, left, right + 1);
            return;
        }

        // phase 0. Make sure left item is lower than right item
        if (s.Compare(left, right) > 0)
        {
            s.Swap(left, right);
        }

        // phase 1. Partition into three regions using dual pivots
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

        // phase 2. Recursively sort left, middle, and right regions
        SortCore(s, left, l - 1);
        // Check if middle region needs sorting (pivots are distinct AND elements exist)
        if (s.Compare(l, g) < 0 && l + 1 < g)
        {
            SortCore(s, l + 1, g - 1);
        }
        SortCore(s, g + 1, right);
    }
}
