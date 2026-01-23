using SortLab.Core.Contexts;
using System.Diagnostics;

namespace SortLab.Core.Algorithms;

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

        // switch to insert sort for small subarrays
        if (right - left + 1 <= InsertThreshold)
        {
            InsertionSort.SortCore(s, left, right + 1);
            return;
        }

        // fase 0. Make sure left item is lower than right item
        if (s.Compare(left, right) > 0)
        {
            s.Swap(left, right);
        }

        // fase 1. decide pivot
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

        // fase 2. Sort Left, Mid and righ
        SortCore(s, left, l - 1);
        if (s.Compare(l, g) < 0)
        {
            SortCore(s, l + 1, g - 1);
        }
        SortCore(s, g + 1, right);
    }
}
