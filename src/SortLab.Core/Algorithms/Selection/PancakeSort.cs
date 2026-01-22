using SortLab.Core.Contexts;
using System.Diagnostics;

namespace SortLab.Core.Algorithms;

/*

Ref span ...

| Method        | Number | Mean          | Error          | StdDev       | Median        | Min          | Max           | Allocated |
|-------------- |------- |--------------:|---------------:|-------------:|--------------:|-------------:|--------------:|----------:|
| PancakeSort   | 100    |      82.23 us |      91.261 us |     5.002 us |      81.40 us |     77.70 us |      87.60 us |     736 B |
| PancakeSort   | 1000   |   5,258.63 us |     518.649 us |    28.429 us |   5,246.60 us |  5,238.20 us |   5,291.10 us |     736 B |
| PancakeSort   | 10000  |  32,987.80 us |   4,770.804 us |   261.504 us |  32,853.00 us | 32,821.20 us |  33,289.20 us |     736 B |

PancakeSort ...

| Method        | Number | Mean          | Error          | StdDev       | Median       | Min          | Max           | Allocated |
|-------------- |------- |--------------:|---------------:|-------------:|-------------:|-------------:|--------------:|----------:|
| PancakeSort   | 100    |      85.48 us |      88.803 us |     4.868 us |     84.55 us |     81.15 us |      90.75 us |     736 B |
| PancakeSort   | 1000   |   5,593.57 us |     377.536 us |    20.694 us |  5,585.90 us |  5,577.80 us |   5,617.00 us |     736 B |
| PancakeSort   | 10000  |  33,698.07 us |   1,645.026 us |    90.169 us | 33,649.70 us | 33,642.40 us |  33,802.10 us |     736 B |

*/

/// <summary>
/// 未ソート部分から最大の要素を見つけ、それを先頭にフリップ（逆転）し、次に先頭から未ソート部分の最後の要素までフリップして配置します。
/// このプロセスを繰り返すことで、配列をソートします。
/// フリップ数がカウントに相当し、フリップ回数が少ない場合は高速ですが、多くなると遅くなります。
/// <br/>
/// Finds the maximum element in the unsorted portion, flips (reverses) the array to bring it to the front, and then flips the array up to the last unsorted element to place the maximum element in its correct position. This process is repeated until the array is sorted. The number of flips corresponds to the count, and fewer flips result in faster performance, while more flips can slow down the process.
/// </summary>
/// <remarks>
/// family  : selection
/// stable  : no
/// inplace : yes
/// Compare : O(n^2)     (Performs approximately n(n-1)/2 comparisons)  
/// Swap    : O(n^2)     (Each flip performs O(n) swaps, up to 2n flips total)  
/// Index   : O(n^2)     (Each element is accessed O(n) times during the sort) 
/// Order   : O(n^2)
///         * average   : O(n^2)
///         * best case : O(n^2)
///         * worst case: O(n^2)  
/// </remarks>
/// <typeparam name="T"></typeparam>
public static class PancakeSort
{
    public static void Sort<T>(Span<T> span) where T : IComparable<T>
    {
        Sort(span, 0, span.Length, NullContext.Default);
    }

    public static void Sort<T>(Span<T> span, ISortContext context) where T : IComparable<T>
    {
        Sort(span, 0, span.Length, context);
    }

    internal static void Sort<T>(Span<T> span, int first, int last, ISortContext context) where T : IComparable<T>
    {
        Debug.Assert(first >= 0 && last <= span.Length && first < last, "Invalid range for sorting.");

        if (span.Length <= 1) return;

        var s = new SortSpan<T>(span, context);

        for (var currentSize = last; currentSize > first; currentSize--)
        {
            var maxIndex = MaxIndex(s, first, currentSize);

            // Max element is already at the end
            if (maxIndex == currentSize - 1)
                continue;

            // Move the maximum element to the front, then flip to the right position
            Flip(s, first, maxIndex);
            Flip(s, first, currentSize - 1);
        }
    }

    /// <summary>
    /// Finds the index of the maximum element within the first n elements of the span.
    /// </summary>
    /// <param name="s"></param>
    /// <param name="first"></param>
    /// <param name="last"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int MaxIndex<T>(SortSpan<T> s, int first, int last) where T : IComparable<T>
    {
        var maxIndex = first;
        for (var i = first + 1; i < last; i++)
        {
            if (s.Compare(i, maxIndex) > 0)
            {
                maxIndex = i;
            }
        }
        return maxIndex;
    }

    /// <summary>
    /// Reverses the elements in the span from start to end.
    /// </summary>
    /// <param name="s"></param>
    /// <param name="start"></param>
    /// <param name="end"></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void Flip<T>(SortSpan<T> s, int start, int end) where T : IComparable<T>
    {
        while (start < end)
        {
            s.Swap(start, end);
            start++;
            end--;
        }
    }
}
