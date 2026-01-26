using SortAlgorithm.Contexts;

namespace SortAlgorithm.Algorithms;

/// <summary>
/// 配列を自然なランに分割し、それらを適応的にマージする高度な安定ソートアルゴリズムです。
/// 部分的にソートされたデータに対して優れた性能を発揮し、最悪でもO(n log n)を保証します。
/// <br/>
/// An adaptive, stable sorting algorithm that identifies natural runs in the data and merges them intelligently.
/// Excels on partially sorted data while guaranteeing O(n log n) worst-case performance.
/// </summary>
/// <remarks>
/// <para><strong>Theoretical Conditions for Correct TimSort:</strong></para>
/// <list type="number">
/// <para><strong>Performance Characteristics:</strong></para>
/// <list type="bullet">
/// <item><description>Family      : Hybrid (Merge + Insertion)</description></item>
/// <item><description>Stable      : Yes (equal elements maintain relative order through careful merge logic)</description></item>
/// <item><description>In-place    : No (requires O(n/2) temporary space worst-case for merging)</description></item>
/// <item><description>Best case   : O(n) - Already sorted or reverse sorted data (single pass with n-1 comparisons)</description></item>
/// <item><description>Average case: O(n log n) - Balanced run detection and merge tree</description></item>
/// <item><description>Worst case  : O(n log n) - Guaranteed by minrun calculation and merge invariants</description></item>
/// <item><description>Comparisons : O(n log n) worst-case, often much less on partially sorted data</description></item>
/// <item><description>Writes      : O(n log n) worst-case, minimal on sorted data due to run detection</description></item>
/// <item><description>Space       : O(n/2) worst-case for merge buffer (uses ArrayPool for efficiency)</description></item>
/// </list>
/// <para><strong>Adaptive Behavior:</strong></para>
/// <list type="bullet">
/// <item><description>Sorted data: O(n) - Detected as single ascending run</description></item>
/// <item><description>Reverse sorted: O(n) - Detected as single descending run, then reversed</description></item>
/// <item><description>Many runs: Galloping mode efficiently handles repeated patterns</description></item>
/// <item><description>Random data: Falls back to efficient merge sort with optimized merging</description></item>
/// </list>
/// <para><strong>Why TimSort is Superior for Real-World Data:</strong></para>
/// <list type="bullet">
/// <item><description>Exploits existing order: Natural runs are identified and preserved</description></item>
/// <item><description>Stable sorting: Critical for multi-key sorts and maintaining data integrity</description></item>
/// <item><description>Predictable performance: O(n log n) guarantee prevents worst-case scenarios</description></item>
/// <item><description>Memory efficient: Only allocates temp buffer when needed, uses ArrayPool</description></item>
/// </list>
/// <para><strong>Reference:</strong></para>
/// <para>Wiki: https://en.wikipedia.org/wiki/Timsort</para>
/// <para>Original description: https://github.com/python/cpython/blob/v3.4.10/Objects/listsort.txt</para>
/// <para>YouTube: https://www.youtube.com/watch?v=exbuZQpWkQ0 (Efficient Algorithms COMP526 (Fall 2023))</para>
/// </remarks>
public static class TimSort
{
    // Buffer identifiers for visualization
    private const int BUFFER_MAIN = 0;       // Main input array
    private const int BUFFER_TEMP = 1;       // Temporary merge buffer

    // TimSort constants
    private const int MIN_MERGE = 32;        // Minimum sized sequence to merge
    private const int MIN_GALLOP = 7;        // Threshold for entering galloping mode

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

        var n = last - first;
        if (n <= 1) return;

        // For very small arrays, use binary insertion sort directly
        if (n < MIN_MERGE)
        {
            BinaryInsertSort.Sort(span, first, last, context);
            return;
        }

        SortCore(span, first, last, context);
    }

    /// <summary>
    /// Core TimSort implementation.
    /// </summary>
    private static void SortCore<T>(Span<T> span, int first, int last, ISortContext context) where T : IComparable<T>
    {
        var n = last - first;
        var minRun = ComputeMinRun(n);
        var s = new SortSpan<T>(span, context, BUFFER_MAIN);

        // Stack to track runs (start position and length)
        Span<int> runBase = stackalloc int[85]; // 85 is enough for 2^64 elements
        Span<int> runLen = stackalloc int[85];
        var stackSize = 0;

        var i = first;
        while (i < last)
        {
            // Find next run (either ascending or strictly descending)
            var runEnd = i + 1;
            if (runEnd < last)
            {
                // Check if descending
                if (s.Compare(i, runEnd) > 0)
                {
                    // Strictly descending run
                    while (runEnd < last && s.Compare(runEnd - 1, runEnd) > 0)
                    {
                        runEnd++;
                    }
                    // Reverse the descending run to make it ascending
                    Reverse(s, i, runEnd - 1);
                }
                else
                {
                    // Ascending run (allowing equals for stability)
                    while (runEnd < last && s.Compare(runEnd - 1, runEnd) <= 0)
                    {
                        runEnd++;
                    }
                }
            }

            var runLength = runEnd - i;

            // If run is too small, extend it to minRun using binary insertion sort
            if (runLength < minRun)
            {
                var force = Math.Min(minRun, last - i);
                BinaryInsertSort.SortCore(s, i, i + force, i + runLength);
                runEnd = i + force;
                runLength = force;
            }

            // Push run onto stack
            runBase[stackSize] = i;
            runLen[stackSize] = runLength;
            stackSize++;

            // Merge runs to maintain invariants
            MergeCollapse(span, runBase, runLen, ref stackSize, context);

            i = runEnd;
        }

        // Force merge all remaining runs
        MergeForceCollapse(span, runBase, runLen, ref stackSize, context);
    }

    /// <summary>
    /// Computes the minimum run length for the given array size.
    /// </summary>
    private static int ComputeMinRun(int n)
    {
        var r = 0;
        while (n >= MIN_MERGE)
        {
            r |= n & 1;
            n >>= 1;
        }
        return n + r;
    }

    /// <summary>
    /// Reverses the elements in the range [lo..hi].
    /// </summary>
    private static void Reverse<T>(SortSpan<T> s, int lo, int hi) where T : IComparable<T>
    {
        while (lo < hi)
        {
            s.Swap(lo, hi);
            lo++;
            hi--;
        }
    }

    /// <summary>
    /// Maintains the run stack invariants by merging runs when necessary.
    /// </summary>
    private static void MergeCollapse<T>(Span<T> span, Span<int> runBase, Span<int> runLen, ref int stackSize, ISortContext context) where T : IComparable<T>
    {
        while (stackSize > 1)
        {
            var n = stackSize - 2;

            // Check invariants:
            // 1. runLen[i-1] > runLen[i] + runLen[i+1]
            // 2. runLen[i] > runLen[i+1]
            if (n > 0 && runLen[n - 1] <= runLen[n] + runLen[n + 1])
            {
                // Merge the smaller of the two runs with X
                if (runLen[n - 1] < runLen[n + 1])
                {
                    n--;
                }
                MergeAt(span, runBase, runLen, ref stackSize, n, context);
            }
            else if (runLen[n] <= runLen[n + 1])
            {
                MergeAt(span, runBase, runLen, ref stackSize, n, context);
            }
            else
            {
                break;
            }
        }
    }

    /// <summary>
    /// Merges all runs on the stack until only one remains.
    /// </summary>
    private static void MergeForceCollapse<T>(Span<T> span, Span<int> runBase, Span<int> runLen, ref int stackSize, ISortContext context) where T : IComparable<T>
    {
        while (stackSize > 1)
        {
            var n = stackSize - 2;
            if (n > 0 && runLen[n - 1] < runLen[n + 1])
            {
                n--;
            }
            MergeAt(span, runBase, runLen, ref stackSize, n, context);
        }
    }

    /// <summary>
    /// Merges the run at stack position i with the run at position i+1.
    /// </summary>
    private static void MergeAt<T>(Span<T> span, Span<int> runBase, Span<int> runLen, ref int stackSize, int i, ISortContext context) where T : IComparable<T>
    {
        var base1 = runBase[i];
        var len1 = runLen[i];
        var base2 = runBase[i + 1];
        var len2 = runLen[i + 1];

        // Merge runs
        MergeRuns(span, base1, len1, base2, len2, context);

        // Update stack
        runLen[i] = len1 + len2;
        if (i == stackSize - 3)
        {
            runBase[i + 1] = runBase[i + 2];
            runLen[i + 1] = runLen[i + 2];
        }
        stackSize--;
    }

    /// <summary>
    /// Merges two adjacent runs.
    /// </summary>
    private static void MergeRuns<T>(Span<T> span, int base1, int len1, int base2, int len2, ISortContext context) where T : IComparable<T>
    {
        var s = new SortSpan<T>(span, context, BUFFER_MAIN);

        // Copy first run to temp array
        var tmp = new T[len1];
        var t = new SortSpan<T>(tmp, context, BUFFER_TEMP);
        s.CopyTo(base1, t, 0, len1);

        var i = 0;              // Index in temp (first run)
        var j = base2;          // Index in span (second run)
        var k = base1;          // Destination index

        // Merge
        while (i < len1 && j < base2 + len2)
        {
            var leftVal = t.Read(i);
            var rightVal = s.Read(j);
            
            // Stability: use <= to take from left when equal
            if (s.Compare(leftVal, rightVal) <= 0)
            {
                s.Write(k++, leftVal);
                i++;
            }
            else
            {
                s.Write(k++, rightVal);
                j++;
            }
        }

        // Copy remaining elements from temp
        while (i < len1)
        {
            s.Write(k++, t.Read(i++));
        }
        // Elements from second run are already in place
    }
}
