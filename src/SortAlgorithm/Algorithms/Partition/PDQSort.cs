using System.Runtime.CompilerServices;
using SortAlgorithm.Contexts;

namespace SortAlgorithm.Algorithms;

/// <summary>
/// Pattern-Defeating QuickSort (pdqsort) は、ランダム化quicksortの高速な平均ケースとheapsortの高速な最悪ケースを組み合わせ、
/// 特定のパターンを持つ入力に対して線形時間を実現する改良されたquicksort変種です。David MusserのIntrosortの拡張および改善版です。
/// <br/>
/// PDQSortは、従来のquicksortのパフォーマンスを低下させる、不良パーティション、ソート済みシーケンス、多数の等価要素などのパターンを検出して回避する様々な技術を使用します。
/// <br/>
/// Pattern-Defeating QuickSort (PDQSort) is an improved quicksort variant that combines the fast average
/// case of randomized quicksort with the fast worst case of heapsort, while achieving linear time on
/// inputs with certain patterns. It is an extension and improvement of David Musser's introsort.
/// <br/>
/// PDQSort uses various techniques to detect and defeat patterns that cause traditional quicksort to perform poorly,
/// including bad partitions, already sorted sequences, and inputs with many equal elements.
/// </summary>
/// <remarks>
/// <para><strong>Theoretical Conditions for Correct PDQSort:</strong></para>
/// <list type="number">
/// <item><description><strong>Adaptive Algorithm Selection:</strong> PDQSort must correctly choose between sub-algorithms:
/// <list type="bullet">
/// <item><description>InsertionSort when partition size &lt; 24 (InsertionSortThreshold)</description></item>
/// <item><description>HeapSort when bad partition count exceeds badAllowed = log₂(n)</description></item>
/// <item><description>PartialInsertionSort when already-partitioned pattern detected</description></item>
/// <item><description>QuickSort with adaptive pivot selection for all other cases</description></item>
/// </list>
/// This adaptive selection ensures O(n log n) worst-case while maintaining O(n) best-case for patterns.</description></item>
/// <item><description><strong>Bad Partition Detection and Limit:</strong> The bad partition limit must be set to log₂(n).
/// A partition is "bad" (highly unbalanced) when either side is &lt; n/8. When this limit is exceeded,
/// it indicates adversarial input patterns, triggering a switch to HeapSort which guarantees O(n log n).</description></item>
/// <item><description><strong>Pivot Selection - Ninther for Large Arrays:</strong> For arrays larger than 128 elements (NintherThreshold),
/// the pivot is selected as the median-of-medians (pseudomedian of 9 elements):
/// <list type="bullet">
/// <item><description>Three groups: {begin, mid, end-1}, {begin+1, mid-1, end-2}, {begin+2, mid+1, end-3}</description></item>
/// <item><description>Each group is sorted (Sort3), then the medians are sorted</description></item>
/// <item><description>The middle of the three medians becomes the pivot</description></item>
/// </list>
/// This ninther selection reduces the probability of worst-case partitioning to O(1/n⁹) from O(1/n³) (median-of-3).</description></item>
/// <item><description><strong>Partition Right Scheme:</strong> The primary partitioning scheme places equal elements in the right partition:
/// <list type="bullet">
/// <item><description>Bidirectional scan: left pointer advances while element &lt; pivot, right pointer retreats while element ≥ pivot</description></item>
/// <item><description>Boundary guards prevent out-of-bounds access when all elements are smaller/larger than pivot</description></item>
/// <item><description>Returns (pivotPos, alreadyPartitioned flag) for pattern detection</description></item>
/// <item><description>alreadyPartitioned flag is true when first ≥ last before any swaps, indicating pre-sorted partition</description></item>
/// </list>
/// This scheme enables detection of already-sorted sequences for O(n) behavior.</description></item>
/// <item><description><strong>Partition Left for Equal Elements:</strong> When pivot equals previous partition boundary (begin-1),
/// switch to PartitionLeft which places equal elements in the left partition:
/// <list type="bullet">
/// <item><description>This optimization handles inputs with many duplicate elements</description></item>
/// <item><description>Left partition becomes fully sorted (all equal to pivot), requiring no recursion</description></item>
/// <item><description>Achieves O(n) time for arrays with all equal elements</description></item>
/// </list>
/// This is triggered by comparing the pivot with *(begin-1), which is the largest element of the previous left partition.</description></item>
/// <item><description><strong>Partial Insertion Sort for Pattern Detection:</strong> When a partition appears already sorted (alreadyPartitioned flag),
/// attempt insertion sort with a limit of 8 element moves (PartialInsertionSortLimit):
/// <list type="bullet">
/// <item><description>If ≤ 8 elements are moved, the partition is nearly sorted → complete the insertion sort (return true)</description></item>
/// <item><description>If &gt; 8 elements are moved, the partition is not sorted → abort and use quicksort (return false)</description></item>
/// <item><description>Applied to both left and right partitions; if both succeed, entire range is sorted</description></item>
/// </list>
/// This achieves O(n) time for sorted and nearly-sorted inputs.</description></item>
/// <item><description><strong>Pattern-Defeating Shuffles:</strong> When a bad partition is detected, shuffle elements to break adversarial patterns:
/// <list type="bullet">
/// <item><description>Swap elements at positions: begin ↔ begin+n/4, pivotPos-1 ↔ pivotPos-n/4 (and similar for right partition)</description></item>
/// <item><description>For large partitions (&gt; 128), perform additional swaps at +1, +2 offsets</description></item>
/// <item><description>This randomization defeats carefully crafted adversarial inputs (e.g., anti-quicksort permutations)</description></item>
/// </list>
/// Without shuffling, adversarial inputs could force O(n²) behavior even with good pivot selection.</description></item>
/// <item><description><strong>Unguarded Insertion Sort Optimization:</strong> For non-leftmost partitions, use unguarded insertion sort:
/// <list type="bullet">
/// <item><description>*(begin-1) acts as a sentinel (guaranteed ≤ any element in [begin, end))</description></item>
/// <item><description>Eliminates boundary check (sift != begin) in the inner loop</description></item>
/// <item><description>Reduces comparisons and improves cache performance for small partitions</description></item>
/// </list>
/// This optimization is safe because quicksort's partitioning ensures *(begin-1) ≤ pivot ≤ all elements in right partition.</description></item>
/// <item><description><strong>Tail Recursion Elimination:</strong> After partitioning into [begin, pivotPos) and [pivotPos+1, end),
/// the algorithm recursively processes the left partition and loops on the right partition.
/// This limits the recursion stack depth to O(log n) instead of potentially O(n).</description></item>
/// </list>
/// <para><strong>Performance Characteristics:</strong></para>
/// <list type="bullet">
/// <item><description>Family      : Hybrid (Partition (base) + Heap + Insertion)</description></item>
/// <item><description>Stable      : No (partitioning and heapsort are unstable)</description></item>
/// <item><description>In-place    : Yes (O(log n) auxiliary space for recursion stack)</description></item>
/// <item><description>Best case   : O(n) - Sorted, reverse sorted, all equal elements (pattern detection + partial insertion sort)</description></item>
/// <item><description>Average case: Θ(n log n) - Expected ~1.2-1.4n log₂ n comparisons (better than basic quicksort due to optimizations)</description></item>
/// <item><description>Worst case  : O(n log n) - Guaranteed by HeapSort fallback when bad partition limit exceeded</description></item>
/// <item><description>Comparisons : ~1.2-1.4n log₂ n (average) - Ninther pivot selection and insertion sort reduce constant factors</description></item>
/// <item><description>Swaps       : ~0.33n log₂ n (average) - Partitioning performs fewer swaps than Lomuto scheme</description></item>
/// </list>
/// <para><strong>Advantages of PDQSort:</strong></para>
/// <list type="bullet">
/// <item><description>Pattern-aware: Detects and optimizes for sorted, reverse-sorted, and equal-element patterns (O(n) best case)</description></item>
/// <item><description>Worst-case guarantee: Always O(n log n), unlike pure QuickSort which degrades to O(n²)</description></item>
/// <item><description>No randomization needed: Deterministic algorithm (unlike randomized quicksort which requires RNG)</description></item>
/// <item><description>Cache-friendly: InsertionSort for small partitions + unguarded optimization improves locality</description></item>
/// <item><description>Robust pivot selection: Ninther (median-of-9) handles various data patterns better than median-of-3</description></item>
/// <item><description>Adversarial-resistant: Pattern-defeating shuffles prevent performance degradation on crafted inputs</description></item>
/// </list>
/// <para><strong>Implementation Details:</strong></para>
/// <list type="bullet">
/// <item><description>InsertionSortThreshold: 24 elements (empirically optimal, balances overhead vs. efficiency)</description></item>
/// <item><description>NintherThreshold: 128 elements (above this, use ninther; below this, use median-of-3)</description></item>
/// <item><description>PartialInsertionSortLimit: 8 element moves (threshold for detecting nearly-sorted partitions)</description></item>
/// <item><description>Bad partition limit: log₂(n) (allows some imbalance before triggering HeapSort)</description></item>
/// <item><description>Bad partition criterion: Either partition &lt; n/8 (highly unbalanced)</description></item>
/// </list>
/// <para><strong>Historical Context:</strong></para>
/// <para>
/// PDQSort was created by Orson Peters in 2015 as an improvement over Introsort (David Musser, 1997).
/// While Introsort combines QuickSort, HeapSort, and InsertionSort for O(n log n) guarantee,
/// PDQSort adds pattern detection and adaptive strategies for O(n) best case on common patterns.
/// The name "Pattern-Defeating" refers to its ability to detect and defeat adversarial input patterns
/// that cause traditional quicksort implementations to perform poorly.
/// </para>
/// <para><strong>Why This Implementation is Theoretically Correct:</strong></para>
/// <list type="number">
/// <item><description>Partitioning correctness: PartitionRight maintains invariant [begin..pivotPos-1] &lt; pivot ≤ [pivotPos+1..end-1]</description></item>
/// <item><description>Recursion correctness: Both partitions are strictly smaller than input (pivotPos excluded from both)</description></item>
/// <item><description>Termination guarantee: Combination of bad partition limit (triggers HeapSort) and tail recursion ensures termination</description></item>
/// <item><description>Pattern detection correctness: alreadyPartitioned flag and partial insertion sort correctly identify sorted patterns</description></item>
/// <item><description>Complexity guarantee: Bad partition limit of log₂(n) ensures HeapSort fallback before O(n²) behavior</description></item>
/// <item><description>Equal element handling: PartitionLeft optimization correctly handles duplicate-heavy inputs</description></item>
/// </list>
/// <para><strong>Reference:</strong></para>
/// <para>Paper: https://arxiv.org/abs/2106.05123</para>
/// <para>Other implementation: https://github.com/orlp/pdqsort</para>
/// </remarks>
public static class PDQSort
{
    // Constants
    private const int InsertionSortThreshold = 24;
    private const int NintherThreshold = 128;
    private const int PartialInsertionSortLimit = 8;

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
        var badAllowed = Log2(last - first);
        PDQSortLoop(s, first, last, badAllowed, true, context);
    }

    /// <summary>
    /// Main PDQSort loop with tail recursion elimination.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void PDQSortLoop<T>(SortSpan<T> s, int begin, int end, int badAllowed, bool leftmost, ISortContext context) where T : IComparable<T>
    {
        while (true)
        {
            var size = end - begin;

            // Use insertion sort for small arrays
            if (size < InsertionSortThreshold)
            {
                if (leftmost)
                {
                    InsertionSort.SortCore(s, begin, end);
                }
                else
                {
                    UnguardedInsertionSort(s, begin, end);
                }
                return;
            }

            // Choose pivot as median of 3 or pseudomedian of 9 (ninther)
            var s2 = size / 2;
            if (size > NintherThreshold)
            {
                // Ninther: median of medians for better pivot selection
                Sort3(s, begin, begin + s2, end - 1);
                Sort3(s, begin + 1, begin + (s2 - 1), end - 2);
                Sort3(s, begin + 2, begin + (s2 + 1), end - 3);
                Sort3(s, begin + (s2 - 1), begin + s2, begin + (s2 + 1));
                s.Swap(begin, begin + s2);
            }
            else
            {
                Sort3(s, begin + s2, begin, end - 1);
            }

            // If *(begin - 1) is the end of the right partition of a previous partition operation,
            // there is no element in [begin, end) that is smaller than *(begin - 1).
            // Then if our pivot compares equal to *(begin - 1) we change strategy.
            if (!leftmost && s.Compare(begin - 1, begin) >= 0)
            {
                begin = PartitionLeft(s, begin, end) + 1;
                continue;
            }

            // Partition and get results
            var (pivotPos, alreadyPartitioned) = PartitionRight(s, begin, end);

            // Check for a highly unbalanced partition
            var lSize = pivotPos - begin;
            var rSize = end - (pivotPos + 1);
            var highlyUnbalanced = lSize < size / 8 || rSize < size / 8;

            // If we got a highly unbalanced partition we shuffle elements to break many patterns
            if (highlyUnbalanced)
            {
                // If we had too many bad partitions, switch to heapsort to guarantee O(n log n)
                if (--badAllowed == 0)
                {
                    HeapSort.SortCore(s, begin, end);
                    return;
                }

                if (lSize >= InsertionSortThreshold)
                {
                    s.Swap(begin, begin + lSize / 4);
                    s.Swap(pivotPos - 1, pivotPos - lSize / 4);

                    if (lSize > NintherThreshold)
                    {
                        s.Swap(begin + 1, begin + (lSize / 4 + 1));
                        s.Swap(begin + 2, begin + (lSize / 4 + 2));
                        s.Swap(pivotPos - 2, pivotPos - (lSize / 4 + 1));
                        s.Swap(pivotPos - 3, pivotPos - (lSize / 4 + 2));
                    }
                }

                if (rSize >= InsertionSortThreshold)
                {
                    s.Swap(pivotPos + 1, pivotPos + (1 + rSize / 4));
                    s.Swap(end - 1, end - rSize / 4);

                    if (rSize > NintherThreshold)
                    {
                        s.Swap(pivotPos + 2, pivotPos + (2 + rSize / 4));
                        s.Swap(pivotPos + 3, pivotPos + (3 + rSize / 4));
                        s.Swap(end - 2, end - (1 + rSize / 4));
                        s.Swap(end - 3, end - (2 + rSize / 4));
                    }
                }
            }
            else
            {
                // If we were decently balanced and we tried to sort an already partitioned
                // sequence try to use insertion sort
                if (alreadyPartitioned &&
                    PartialInsertionSort(s, begin, pivotPos) &&
                    PartialInsertionSort(s, pivotPos + 1, end))
                {
                    return;
                }
            }

            // Sort the left partition first using recursion and do tail recursion elimination for
            // the right-hand partition
            PDQSortLoop(s, begin, pivotPos, badAllowed, leftmost, context);
            begin = pivotPos + 1;
            leftmost = false;
        }
    }

    /// <summary>
    /// Partitions [begin, end) around pivot *begin. Elements equal to the pivot are put in the right-hand partition.
    /// Returns the position of the pivot after partitioning and whether the passed sequence already was correctly partitioned.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static (int pivotPos, bool alreadyPartitioned) PartitionRight<T>(SortSpan<T> s, int begin, int end) where T : IComparable<T>
    {
        // Move pivot into local for speed
        var pivot = s.Read(begin);

        var first = begin;
        var last = end;

        // Find the first element greater than or equal to the pivot
        while (s.Compare(++first, pivot) < 0) { }

        // Find the first element strictly smaller than the pivot
        if (first - 1 == begin)
        {
            while (first < last && s.Compare(--last, pivot) >= 0) { }
        }
        else
        {
            while (s.Compare(--last, pivot) >= 0) { }
        }

        // If the first pair of elements that should be swapped to partition are the same element,
        // the passed in sequence already was correctly partitioned
        var alreadyPartitioned = first >= last;

        // Keep swapping pairs of elements that are on the wrong side of the pivot
        while (first < last)
        {
            s.Swap(first, last);
            while (s.Compare(++first, pivot) < 0) { }
            while (s.Compare(--last, pivot) >= 0) { }
        }

        // Put the pivot in the right place
        var pivotPos = first - 1;
        s.Write(begin, s.Read(pivotPos));
        s.Write(pivotPos, pivot);

        return (pivotPos, alreadyPartitioned);
    }

    /// <summary>
    /// Partitions [begin, end) around pivot *begin. Elements equal to the pivot are put to the left.
    /// Used when many equal elements are detected.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int PartitionLeft<T>(SortSpan<T> s, int begin, int end) where T : IComparable<T>
    {
        var pivot = s.Read(begin);
        var first = begin;
        var last = end;

        while (s.Compare(pivot, s.Read(--last)) < 0) { }

        if (last + 1 == end)
        {
            while (first < last && s.Compare(pivot, s.Read(++first)) >= 0) { }
        }
        else
        {
            while (s.Compare(pivot, s.Read(++first)) >= 0) { }
        }

        while (first < last)
        {
            s.Swap(first, last);
            while (s.Compare(pivot, s.Read(--last)) < 0) { }
            while (s.Compare(pivot, s.Read(++first)) >= 0) { }
        }

        var pivotPos = last;
        s.Write(begin, s.Read(pivotPos));
        s.Write(pivotPos, pivot);

        return pivotPos;
    }

    /// <summary>
    /// Attempts to use insertion sort on [begin, end). Will return false if more than
    /// PartialInsertionSortLimit elements were moved, and abort sorting.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool PartialInsertionSort<T>(SortSpan<T> s, int begin, int end) where T : IComparable<T>
    {
        if (begin == end) return true;

        var limit = 0;
        for (var cur = begin + 1; cur < end; cur++)
        {
            if (limit > PartialInsertionSortLimit) return false;

            var sift = cur;
            var siftValue = s.Read(cur);

            if (s.Compare(sift, sift - 1) < 0)
            {
                do
                {
                    s.Write(sift, s.Read(sift - 1));
                    sift--;
                }
                while (sift != begin && s.Compare(siftValue, s.Read(sift - 1)) < 0);

                s.Write(sift, siftValue);
                limit += cur - sift;
            }
        }

        return true;
    }

    /// <summary>
    /// Insertion sort assuming *(begin - 1) is a sentinel (smaller than or equal to any element in [begin, end)).
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void UnguardedInsertionSort<T>(SortSpan<T> s, int begin, int end) where T : IComparable<T>
    {
        if (begin == end) return;

        for (var cur = begin + 1; cur < end; cur++)
        {
            var sift = cur;
            var siftValue = s.Read(cur);

            if (s.Compare(sift, sift - 1) < 0)
            {
                do
                {
                    s.Write(sift, s.Read(sift - 1));
                    sift--;
                }
                while (s.Compare(siftValue, s.Read(sift - 1)) < 0);

                s.Write(sift, siftValue);
            }
        }
    }

    /// <summary>
    /// Sorts 3 elements at positions a, b, c.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void Sort3<T>(SortSpan<T> s, int a, int b, int c) where T : IComparable<T>
    {
        if (s.Compare(b, a) < 0) s.Swap(a, b);
        if (s.Compare(c, b) < 0) s.Swap(b, c);
        if (s.Compare(b, a) < 0) s.Swap(a, b);
    }

    /// <summary>
    /// Returns floor(log2(n)), assumes n > 0.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int Log2(int n)
    {
        var log = 0;
        while (n > 1)
        {
            n >>= 1;
            log++;
        }
        return log;
    }
}
