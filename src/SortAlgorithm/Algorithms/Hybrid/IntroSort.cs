using SortAlgorithm.Contexts;

namespace SortAlgorithm.Algorithms;

/// <summary>
/// QuickSort、HeapSort、InsertionSortを組み合わせたハイブリッドソートアルゴリズムです。
/// 通常はQuickSortを使用しますが、小さい配列ではInsertionSort、再帰深度が深くなりすぎた場合はHeapSortに切り替えることで、
/// QuickSortの最悪ケースO(n²)を回避し、常にO(n log n)を保証します。
/// <br/>
/// A hybrid sorting algorithm that combines QuickSort, HeapSort, and InsertionSort.
/// It primarily uses QuickSort, but switches to InsertionSort for small arrays and HeapSort when recursion depth becomes too deep,
/// avoiding QuickSort's worst-case O(n²) and guaranteeing O(n log n) in all cases.
/// </summary>
/// <remarks>
/// <para><strong>Theoretical Conditions for Correct Introsort:</strong></para>
/// <list type="number">
/// <item><description><strong>Adaptive Algorithm Selection:</strong> The algorithm must correctly choose between three sub-algorithms:
/// <list type="bullet">
/// <item><description>InsertionSort when partition size ≤ 16 (threshold can vary, typically 8-32)</description></item>
/// <item><description>HeapSort when recursion depth exceeds depthLimit = 2⌊log₂(n)⌋</description></item>
/// <item><description>QuickSort (median-of-three + Hoare partition) for all other cases</description></item>
/// </list>
/// This adaptive selection ensures O(n log n) worst-case while maintaining QuickSort's average-case performance.</description></item>
/// <item><description><strong>Depth Limit Calculation:</strong> The depth limit must be set to 2⌊log₂(n)⌋ where n is the partition size.
/// This value is derived from the expected depth of a balanced binary tree (⌊log₂(n)⌋) multiplied by 2 to allow for some imbalance.
/// When this limit is exceeded, it indicates pathological QuickSort behavior (e.g., adversarial input patterns),
/// triggering a switch to HeapSort which guarantees O(n log n) regardless of input.</description></item>
/// <item><description><strong>QuickSort Phase - Median-of-Three Pivot Selection:</strong> To avoid worst-case QuickSort behavior on sorted/reverse-sorted inputs,
/// the pivot is selected as the median of three quartile positions: q1 = left + n/4, mid = left + n/2, q3 = left + 3n/4.
/// This quartile-based sampling provides better pivot quality than simple left/mid/right sampling, especially for mountain-shaped or partially-sorted data.
/// The median-of-three reduces the probability of worst-case partitioning from O(1/n) (random pivot) to O(1/n³).</description></item>
/// <item><description><strong>QuickSort Phase - Hoare Partition Scheme:</strong> Partitioning uses bidirectional scanning:
/// <list type="bullet">
/// <item><description>Left pointer l advances while s[l] &lt; pivot (with boundary check l &lt; right)</description></item>
/// <item><description>Right pointer r retreats while s[r] &gt; pivot (with boundary check r &gt; left)</description></item>
/// <item><description>When both pointers stop and l ≤ r, swap s[l] ↔ s[r] and advance both pointers</description></item>
/// <item><description>Loop terminates when l &gt; r, ensuring partitioning invariant: s[left..r] ≤ pivot ≤ s[l..right]</description></item>
/// </list>
/// The condition l ≤ r (not l &lt; r) ensures elements equal to pivot are swapped, preventing infinite loops on duplicate-heavy arrays.
/// Boundary checks prevent out-of-bounds access when all elements are smaller/larger than pivot.</description></item>
/// <item><description><strong>Tail Recursion Optimization:</strong> After partitioning into [left, r] and [l, right],
/// the algorithm recursively processes only the smaller partition and loops on the larger partition.
/// This optimization limits the recursion stack depth to O(log n) instead of potentially O(n),
/// even in pathological cases before the depth limit triggers HeapSort.</description></item>
/// <item><description><strong>InsertionSort Threshold:</strong> For partitions of size ≤ 16, InsertionSort is used instead of QuickSort.
/// This threshold is empirically optimal:
/// <list type="bullet">
/// <item><description>InsertionSort has lower constant factors than QuickSort for small arrays</description></item>
/// <item><description>Reduces recursion overhead (16-element partition would require 4 more recursion levels)</description></item>
/// <item><description>Improves cache locality by processing small contiguous regions</description></item>
/// <item><description>Threshold of 16 is standard in many production implementations (C++ std::sort, .NET Array.Sort)</description></item>
/// </list>
/// This hybrid approach achieves better constant factors than pure QuickSort while maintaining O(n log n) worst-case.</description></item>
/// <item><description><strong>HeapSort Fallback Correctness:</strong> When depthLimit reaches 0, the current partition is sorted using HeapSort.
/// HeapSort guarantees O(n log n) time complexity regardless of input distribution, providing a safety net against adversarial inputs.
/// The depth limit ensures that HeapSort is invoked only when QuickSort exhibits pathological behavior,
/// preserving QuickSort's superior average-case performance for typical inputs.</description></item>
/// </list>
/// <para><strong>Performance Characteristics:</strong></para>
/// <list type="bullet">
/// <item><description>Family      : Hybrid (Partitioning + Heap + Insertion)</description></item>
/// <item><description>Stable      : No (QuickSort and HeapSort are unstable; element order is not preserved for equal values)</description></item>
/// <item><description>In-place    : Yes (O(log n) auxiliary space for recursion stack, no additional arrays allocated)</description></item>
/// <item><description>Best case   : Θ(n log n) - Occurs when QuickSort consistently creates balanced partitions and InsertionSort handles small subarrays efficiently</description></item>
/// <item><description>Average case: Θ(n log n) - Expected ~1.386n log₂ n comparisons from QuickSort with Hoare partition, reduced by InsertionSort optimization</description></item>
/// <item><description>Worst case  : O(n log n) - Guaranteed by HeapSort fallback when recursion depth exceeds 2⌊log₂(n)⌋</description></item>
/// <item><description>Comparisons : ~1.2-1.4n log₂ n (average) - Lower than pure QuickSort due to InsertionSort handling small partitions</description></item>
/// <item><description>Swaps       : ~0.33n log₂ n (average) - Hoare partition performs significantly fewer swaps than Lomuto partition</description></item>
/// </list>
/// <para><strong>Advantages of Introsort:</strong></para>
/// <list type="bullet">
/// <item><description>Worst-case guarantee: Always O(n log n), unlike pure QuickSort which degrades to O(n²)</description></item>
/// <item><description>Average-case efficiency: Matches QuickSort's performance on typical inputs (~1.4n log₂ n comparisons)</description></item>
/// <item><description>Cache-friendly: InsertionSort for small partitions improves spatial locality</description></item>
/// <item><description>Stack-safe: Tail recursion optimization + depth limit ensures O(log n) stack depth</description></item>
/// <item><description>Practical performance: Used in production libraries (C++ std::sort, .NET Array.Sort, Java Arrays.sort for primitives)</description></item>
/// <item><description>Robust pivot selection: Quartile-based median-of-three handles various data patterns (sorted, reverse, mountain, valley)</description></item>
/// </list>
/// <para><strong>Implementation Details:</strong></para>
/// <list type="bullet">
/// <item><description>Threshold value: 16 elements for switching to InsertionSort (empirically optimal, balances overhead vs. efficiency)</description></item>
/// <item><description>Depth limit: 2 × floor(log₂(n)) - allows some imbalance before triggering HeapSort</description></item>
/// <item><description>Pivot selection: Median-of-three using quartile positions (n/4, n/2, 3n/4) for better distribution sampling</description></item>
/// <item><description>Partition scheme: Hoare partition (bidirectional scan) for fewer swaps and better duplicate handling</description></item>
/// <item><description>Tail recursion: Always recurse on smaller partition, loop on larger to guarantee O(log n) stack depth</description></item>
/// </list>
/// <para><strong>Historical Context:</strong></para>
/// <para>
/// Introsort was invented by David Musser in 1997 as a solution to QuickSort's quadratic worst-case behavior.
/// The name "Introsort" is short for "Introspective Sort" - the algorithm introspects its own behavior (recursion depth)
/// and adapts by switching to HeapSort when needed. This hybrid approach combines the best characteristics of QuickSort
/// (fast average case), HeapSort (guaranteed O(n log n)), and InsertionSort (efficient for small arrays).
/// </para>
/// <para><strong>Why This Implementation is Theoretically Correct:</strong></para>
/// <list type="number">
/// <item><description>Partitioning correctness: Hoare partition maintains invariant s[left..r] ≤ pivot ≤ s[l..right] with proper boundary checks</description></item>
/// <item><description>Recursion correctness: Both partitions [left, r] and [l, right] are strictly smaller than [left, right] due to pointer advance after swap</description></item>
/// <item><description>Termination guarantee: Combination of depth limit (triggers HeapSort) and tail recursion (limits stack depth) ensures termination</description></item>
/// <item><description>Algorithm correctness: InsertionSort, HeapSort, and QuickSort are all proven correct sorting algorithms</description></item>
/// <item><description>Complexity guarantee: Depth limit of 2⌊log₂(n)⌋ ensures HeapSort fallback before stack overflow or quadratic behavior</description></item>
/// </list>
/// </remarks>
public static class IntroSort
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
    /// Sorts the subrange [left..right] using the provided sort context.
    /// This overload accepts a SortSpan directly for use by other algorithms that already have a SortSpan instance.
    /// </summary>
    /// <typeparam name="T">The type of elements in the span. Must implement <see cref="IComparable{T}"/>.</typeparam>
    /// <param name="s">The SortSpan wrapping the span to sort.</param>
    /// <param name="left">The inclusive start index of the range to sort.</param>
    /// <param name="right">The inclusive end index of the range to sort.</param>
    /// <param name="context">The sort context for tracking statistics and observations.</param>
    internal static void SortCore<T>(SortSpan<T> s, int left, int right, ISortContext context) where T : IComparable<T>
    {
        var depthLimit = 2 * FloorLog2(right - left + 1);
        IntroSortInternal(s, left, right, depthLimit, context);
    }

    /// <summary>
    /// Internal IntroSort implementation that switches between QuickSort, HeapSort, and InsertionSort based on size and depth.
    /// </summary>
    /// <typeparam name="T">The type of elements in the span. Must implement <see cref="IComparable{T}"/>.</typeparam>
    /// <param name="s">The SortSpan wrapping the span to sort.</param>
    /// <param name="left">The inclusive start index of the range to sort.</param>
    /// <param name="right">The inclusive end index of the range to sort.</param>
    /// <param name="depthLimit">The recursion depth limit before switching to HeapSort.</param>
    /// <param name="context">The sort context for tracking statistics and observations.</param>
    private static void IntroSortInternal<T>(SortSpan<T> s, int left, int right, int depthLimit, ISortContext context) where T : IComparable<T>
    {
        while (right > left)
        {
            var size = right - left + 1;

            // Small arrays: use InsertionSort
            if (size <= 16)
            {
                InsertionSort.SortCore(s, left, right + 1);
                return;
            }

            // Max depth reached: switch to HeapSort to guarantee O(n log n)
            if (depthLimit == 0)
            {
                HeapSort.SortCore(s, left, right + 1);
                return;
            }

            depthLimit--;

            // QuickSort with median-of-three pivot selection (Hoare partition)
            var length = right - left + 1;
            var q1 = left + length / 4;
            var mid = left + length / 2;
            var q3 = left + (length * 3) / 4;
            var pivot = MedianOf3Value(s, q1, mid, q3);

            // Hoare partition scheme
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

            // Tail recursion optimization: recurse on smaller partition, loop on larger
            var leftSize = r - left + 1;
            var rightSize = right - l + 1;

            if (leftSize < rightSize)
            {
                // Left partition is smaller: recurse on left, loop on right
                if (left < r)
                {
                    IntroSortInternal(s, left, r, depthLimit, context);
                }
                left = l; // Continue with right partition in next loop iteration
            }
            else
            {
                // Right partition is smaller (or equal): recurse on right, loop on left
                if (l < right)
                {
                    IntroSortInternal(s, l, right, depthLimit, context);
                }
                right = r; // Continue with left partition in next loop iteration
            }
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

    /// <summary>
    /// Computes the floor of the base-2 logarithm of a positive integer.
    /// </summary>
    /// <param name="n">The positive integer to compute the logarithm for.</param>
    /// <returns>The floor of log2(n).</returns>
    private static int FloorLog2(int n)
    {
        var result = 0;
        while (n > 1)
        {
            result++;
            n >>= 1;
        }
        return result;
    }
}
