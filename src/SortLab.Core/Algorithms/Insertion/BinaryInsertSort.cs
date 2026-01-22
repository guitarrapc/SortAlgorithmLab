using SortLab.Core.Contexts;
using System.Diagnostics;

namespace SortLab.Core.Algorithms;

/*
Ref span ...

| Method           | Number | Mean       | Error       | StdDev    | Median      | Min         | Max        | Allocated |
|----------------- |------- |-----------:|------------:|----------:|------------:|------------:|-----------:|----------:|
| BinaryInsertSort | 100    |   112.1 us | 2,820.79 us | 154.62 us |    31.50 us |    14.50 us |   290.4 us |     736 B |
| BinaryInsertSort | 1000   |   112.4 us |    55.38 us |   3.04 us |   112.90 us |   109.10 us |   115.1 us |     736 B |
| BinaryInsertSort | 10000  | 6,902.2 us |   897.73 us |  49.21 us | 6,929.30 us | 6,845.40 us | 6,931.9 us |     736 B |

Ref span (start) ...

| Method           | Number | Mean       | Error       | StdDev    | Median      | Min         | Max        | Allocated |
|----------------- |------- |-----------:|------------:|----------:|------------:|------------:|-----------:|----------:|
| BinaryInsertSort | 100    |   122.6 us | 3,383.21 us | 185.45 us |    15.70 us |    15.30 us |   336.7 us |     448 B |
| BinaryInsertSort | 1000   |   121.1 us |    55.55 us |   3.04 us |   121.70 us |   117.80 us |   123.8 us |     448 B |
| BinaryInsertSort | 10000  | 8,673.2 us | 4,841.61 us | 265.38 us | 8,720.50 us | 8,387.30 us | 8,911.7 us |     736 B |

Span ...

| Method                 | Number | Mean          | Error        | StdDev      | Median        | Min           | Max          | Allocated |
|----------------------- |------- |--------------:|-------------:|------------:|--------------:|--------------:|-------------:|----------:|
| BinaryInsertSort       | 100    |    135.333 us | 3,753.441 us | 205.7388 us |     16.600 us |     16.500 us |    372.90 us |     736 B |
| BinaryInsertSort       | 1000   |    122.133 us |    39.312 us |   2.1548 us |    122.300 us |    119.900 us |    124.20 us |     736 B |
| BinaryInsertSort       | 10000  |  6,903.433 us |   742.971 us |  40.7247 us |  6,898.500 us |  6,865.400 us |  6,946.40 us |     736 B |

*/

/// <summary>
/// 通常のリニアサーチと異なり、挿入位置を二分探索して決定するため比較範囲、回数を改善できる。
/// ソート済み配列では高速に動作しますが、逆順の配列では遅くなります。
/// <br/>
/// Unlike the traditional linear search, it uses binary search to determine the insertion position, thereby improving the comparison range and the number of comparisons. Stable sort.
/// Performs efficiently on already sorted arrays but is slow on reverse-sorted arrays.
/// </summary>
/// <remarks>
/// family  : insertion
/// stable  : yes
/// inplace : yes
/// Compare : O(n log n) (Binary search for each insertion) 
/// Swap    : O(n^2)     (Each insertion may require shifting elements) 
/// Index   : O(n^2)     (Each element may be accessed multiple times during shifts)
/// Order   : O(n^2)     (Comparisons are O(n log n), but shifts dominate at O(n^2))
///         * average   : O(n^2)  
///         * best case : O(n)    (nearly sorted)  
///         * worst case: O(n^2)  (reverse sorted)
/// </remarks>
/// <typeparam name="T"></typeparam>
public static class BinaryInsertSort
{
    public static void Sort<T>(Span<T> span) where T : IComparable<T>
    {
        Sort(span, 0, span.Length, NullContext.Default);
    }

    public static void Sort<T>(Span<T> span, ISortContext context) where T : IComparable<T>
    {
        Sort(span, 0, span.Length, context);
    }

    /// <summary>
    /// Sort the subrange [first..last).
    /// </summary>
    /// <param name="span"></param>
    /// <param name="first"></param>
    /// <param name="last"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    internal static void Sort<T>(Span<T> span, int first, int last, ISortContext context) where T : IComparable<T>
    {
        Sort(span, first, last, first, context);
    }

    /// <summary>
    /// binary insertion sort with shift
    /// </summary>
    /// <param name="span"></param>
    /// <param name="first"></param>
    /// <param name="last"></param>
    /// <param name="start"></param>
    /// <param name="context"></param>
    internal static void Sort<T>(Span<T> span, int first, int last, int start, ISortContext context) where T : IComparable<T>
    {
        Debug.Assert(first >= 0 && last <= span.Length && first < last, "Invalid range for sorting.");

        if (span.Length <= 1) return;

        var s = new SortSpan<T>(span, context);

        // The following commented-out code shows an example of using the built-in
        // Array.BinarySearch and Array.Copy for inserting an element, but it is
        // not used here since we rely on a manual approach below.
        //
        // for (var i = first + 1; i < last; i++)
        // {
        //     var tmp = span[i];
        //     var j = Array.BinarySearch(array, 0, i, tmp);
        //     Array.Copy(array, j, array, j+1, i - j);
        //     Swap(ref array[j], ref tmp);
        // }

        // If 'start' equals 'first', move it forward to begin insertion from the next element
        if (start == first)
            start++;

        for (var i = start; i < last; i++)
        {
            var tmp = s.Read(i);

            // Find the insertion position using a custom binary search
            var pos = BinarySearch(s, tmp, first, i);

            // Only perform shift and write if the element needs to move
            if (pos != i)
            {
                // shift [pos.. i-1] -> [pos+1.. i]
                for (var j = i - 1; j >= pos; j--)
                {
                    s.Write(j + 1, s.Read(j));
                }

                // Finally, write 'tmp' into the 'pos' position
                s.Write(pos, tmp);
            }
        }
    }

    /// <summary>
    /// Performs a binary search over [first..index) to find the insertion point for 'tmp'.
    /// If elements are equal, we move to the right (<=0), ensuring a stable sort.
    /// </summary>
    private static int BinarySearch<T>(SortSpan<T> s, T tmp, int first, int index) where T : IComparable<T>
    {
        var left = first;
        var right = index;
        while (left < right)
        {
            var mid = (left + right) / 2;
            if (s.Compare(mid, tmp) <= 0)
            {
                left = mid + 1;
            }
            else
            {
                right = mid;
            }
        }
        return left;
    }
}
