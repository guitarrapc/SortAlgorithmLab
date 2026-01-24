using SortLab.Core.Contexts;

namespace SortLab.Core.Algorithms;

/// <summary>
/// 配列の先頭、中央、末尾から中央値を求めてピボットとし、このピボットを基準に配列を左右に分割する分割統治法のソートアルゴリズムです。
/// Hoare partition schemeを使用し、Median-of-3法でピボットを選択することで安定した性能を実現します。
/// <br/>
/// A divide-and-conquer sorting algorithm using Hoare partition scheme with median-of-3 pivot selection for reliable performance.
/// </summary>
/// <remarks>
/// <para><strong>Theoretical Conditions for Correct QuickSort with Median-of-3 Pivot Selection:</strong></para>
/// <list type="number">
/// <item><description><strong>Median-of-3 Pivot Selection:</strong> The pivot value is selected as the median of three sampled elements: 
/// array[left], array[mid], and array[right], where mid = left + (right - left) / 2.
/// This selection method is computed using 2-3 comparisons and ensures better pivot quality than random selection.
/// The median-of-3 strategy reduces the probability of worst-case partitioning from O(1/n) (random pivot) to O(1/n³),
/// and handles sorted, reverse-sorted, and nearly-sorted arrays efficiently (lines 124-125, 170-194).</description></item>
/// <item><description><strong>Hoare Partition Scheme:</strong> The array is partitioned into two regions using bidirectional scanning:
/// <list type="bullet">
/// <item><description>Initialize pointers: l = left, r = right (line 128-129)</description></item>
/// <item><description>Left scan: advance l rightward while array[l] &lt; pivot, with boundary check l &lt; right to prevent overflow (line 134-137)</description></item>
/// <item><description>Right scan: advance r leftward while array[r] &gt; pivot, with boundary check r &gt; left to prevent underflow (line 140-143)</description></item>
/// <item><description>Swap and advance: if l ≤ r, swap array[l] with array[r], then increment l and decrement r (line 146-151)</description></item>
/// <item><description>Termination: loop exits when l &gt; r, ensuring proper partitioning (line 131)</description></item>
/// </list>
/// Boundary checks (l &lt; right and r &gt; left) prevent out-of-bounds access when all elements are smaller/larger than pivot.
/// The condition l ≤ r (not l &lt; r) ensures elements equal to pivot are swapped, preventing infinite loops on duplicate-heavy arrays.</description></item>
/// <item><description><strong>Partition Invariant:</strong> Upon completion of the partitioning phase (when l &gt; r):
/// <list type="bullet">
/// <item><description>All elements in range [left, r] satisfy: element ≤ pivot</description></item>
/// <item><description>All elements in range [l, right] satisfy: element ≥ pivot</description></item>
/// <item><description>Partition boundaries satisfy: left - 1 ≤ r &lt; l ≤ right + 1</description></item>
/// <item><description>The gap between r and l (r &lt; l) may contain elements equal to pivot that have been properly partitioned</description></item>
/// </list>
/// This invariant guarantees that after partitioning, the array is divided into two well-defined regions for recursive sorting.</description></item>
/// <item><description><strong>Recursive Subdivision:</strong> The algorithm recursively sorts two independent subranges (lines 156-163):
/// <list type="bullet">
/// <item><description>Left subrange: [left, r] is sorted only if left &lt; r (contains 2+ elements)</description></item>
/// <item><description>Right subrange: [l, right] is sorted only if l &lt; right (contains 2+ elements)</description></item>
/// </list>
/// Base case: when right ≤ left, the range contains ≤ 1 element and is trivially sorted (line 121).
/// The conditional checks (left &lt; r and l &lt; right) prevent unnecessary recursion on empty or single-element ranges,
/// improving efficiency and preventing stack overflow on edge cases.</description></item>
/// <item><description><strong>Termination Guarantee:</strong> The algorithm terminates for all inputs because:
/// <list type="bullet">
/// <item><description>Progress property: After each partition, r &lt; l, so both subranges [left, r] and [l, right] are strictly smaller than [left, right]</description></item>
/// <item><description>Minimum progress: Even when all elements equal the pivot, at least one element is excluded from each recursive call (the swapped elements at l and r)</description></item>
/// <item><description>Base case reached: The recursion depth is bounded, and each recursive call eventually reaches the base case (right ≤ left)</description></item>
/// <item><description>Expected recursion depth: O(log n) with median-of-3 pivot selection</description></item>
/// <item><description>Worst-case recursion depth: O(n) when partitioning is maximally unbalanced (extremely rare with median-of-3)</description></item>
/// </list>
/// The Hoare partition scheme guarantees that partitioning makes progress even on arrays with many duplicate elements.</description></item>
/// </list>
/// <para><strong>Performance Characteristics:</strong></para>
/// <list type="bullet">
/// <item><description>Family      : Partitioning (Divide and Conquer)</description></item>
/// <item><description>Partition   : Hoare partition scheme (bidirectional scan)</description></item>
/// <item><description>Stable      : No (partitioning does not preserve relative order of equal elements)</description></item>
/// <item><description>In-place    : Yes (O(log n) auxiliary space for recursion stack, O(1) for partitioning)</description></item>
/// <item><description>Best case   : Θ(n log n) - Occurs when pivot consistently divides array into balanced partitions</description></item>
/// <item><description>Average case: Θ(n log n) - Expected ~1.386n log₂ n comparisons with Hoare partition</description></item>
/// <item><description>Worst case  : O(n²) - Occurs when partitioning is maximally unbalanced (probability ~1/n³ with median-of-3)</description></item>
/// <item><description>Comparisons : ~1.386n log₂ n (average) - Hoare partition uses ~2n ln n comparisons, fewer than Lomuto's ~3n ln n</description></item>
/// <item><description>Swaps       : ~0.33n log₂ n (average) - Hoare partition performs ~3× fewer swaps than Lomuto partition</description></item>
/// </list>
/// <para><strong>Median-of-3 Pivot Selection Benefits:</strong></para>
/// <list type="bullet">
/// <item><description>Worst-case probability reduction: From O(1/n) with random pivot to O(1/n³) with median-of-3</description></item>
/// <item><description>Improved pivot quality: Median-of-3 tends to select pivots closer to the true median of the array</description></item>
/// <item><description>Minimal overhead: Requires only 2-3 additional comparisons per partitioning step</description></item>
/// <item><description>Sorted input handling: Efficiently handles sorted, reverse-sorted, and nearly-sorted arrays without degrading to O(n²)</description></item>
/// <item><description>Cache efficiency: Samples elements from beginning, middle, and end, improving spatial locality</description></item>
/// </list>
/// <para><strong>Hoare Partition Scheme Advantages:</strong></para>
/// <list type="bullet">
/// <item><description>Fewer comparisons: ~1.386n log n vs ~2n log n for Lomuto partition (31% reduction)</description></item>
/// <item><description>Fewer swaps: Approximately 3× fewer swaps than Lomuto partition scheme</description></item>
/// <item><description>Better duplicate handling: Elements equal to pivot are distributed between partitions, preventing degenerate partitioning</description></item>
/// <item><description>Balanced partitions: Tends to produce more balanced partitions than Lomuto on arrays with duplicates</description></item>
/// </list>
/// <para><strong>Comparison with Other QuickSort Variants:</strong></para>
/// <list type="bullet">
/// <item><description>vs. Random Pivot: Median-of-3 provides more consistent performance with minimal overhead</description></item>
/// <item><description>vs. Lomuto Partition: Hoare partition (used here) performs ~3× fewer swaps and better handles duplicates</description></item>
/// <item><description>vs. Dual-Pivot QuickSort: Simpler implementation, but dual-pivot can be ~5-10% faster on modern CPUs</description></item>
/// </list>
/// </remarks>
public static class QuickSortMedian3
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
        SortCore(s, first, last - 1, context);
    }

    /// <summary>
    /// Sorts the subrange [first..last) using the provided sort context.
    /// This overload accepts a SortSpan directly for use by other algorithms that already have a SortSpan instance.
    /// </summary>
    /// <typeparam name="T">The type of elements in the span. Must implement <see cref="IComparable{T}"/>.</typeparam>
    /// <param name="s">The SortSpan wrapping the span to sort.</param>
    /// <param name="left">The inclusive start index of the range to sort.</param>
    /// <param name="right">The exclusive end index of the range to sort.</param>
    internal static void SortCore<T>(SortSpan<T> s, int left, int right, ISortContext context) where T : IComparable<T>
    { 
        if (left >= right) return;

        // Phase 1. Select pivot using median-of-3 strategy
        var mid = left + (right - left) / 2;
        var pivot = MedianOf3Value(s, left, mid, right);

        // Phase 2. Partition array using Hoare partition scheme
        var l = left;
        var r = right;

        while (l <= r)
        {
            // Move l forward while elements are less than pivot
            while (l < right && s.Read(l).CompareTo(pivot) < 0)
            {
                l++;
            }

            // Move r backward while elements are greater than pivot
            while (r > left && s.Read(r).CompareTo(pivot) > 0)
            {
                r--;
            }

            // If pointers haven't crossed, swap and advance both
            if (l <= r)
            {
                s.Swap(l, r);
                l++;
                r--;
            }
        }

        // Phase 3. Recursively sort left and right partitions
        // After partitioning: r is the last index of left partition, l is the first index of right partition
        if (left < r)
        {
            SortCore(s, left, r, context);
        }
        if (l < right)
        {
            SortCore(s, l, right, context);
        }
    }

    /// <summary>
    /// Returns the median value among three elements at specified indices.
    /// This method performs exactly 2-3 comparisons to determine the median value.
    /// </summary>
    private static T MedianOf3Value<T>(SortSpan<T> s, int lowIdx, int midIdx, int highIdx) where T : IComparable<T>
    {
        // Use SortSpan.Compare to track statistics
        var cmpLowMid = s.Compare(lowIdx, midIdx);
        
        if (cmpLowMid > 0) // low > mid
        {
            var cmpMidHigh = s.Compare(midIdx, highIdx);
            if (cmpMidHigh > 0) // low > mid > high
            {
                return s.Read(midIdx); // mid is median
            }
            else // low > mid, mid <= high
            {
                var cmpLowHigh = s.Compare(lowIdx, highIdx);
                return cmpLowHigh > 0 ? s.Read(highIdx) : s.Read(lowIdx);
            }
        }
        else // low <= mid
        {
            var cmpMidHigh = s.Compare(midIdx, highIdx);
            if (cmpMidHigh > 0) // low <= mid, mid > high
            {
                var cmpLowHigh = s.Compare(lowIdx, highIdx);
                return cmpLowHigh > 0 ? s.Read(lowIdx) : s.Read(highIdx);
            }
            else // low <= mid <= high
            {
                return s.Read(midIdx); // mid is median
            }
        }
    }
}
