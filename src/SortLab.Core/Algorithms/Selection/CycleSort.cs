using SortLab.Core.Contexts;

namespace SortLab.Core.Algorithms;

/*

Ref span ...

| Method        | Number | Mean          | Error          | StdDev        | Median        | Min          | Max           | Allocated |
|-------------- |------- |--------------:|---------------:|--------------:|--------------:|-------------:|--------------:|----------:|
| CycleSort     | 100    |     105.30 us |      90.780 us |      4.976 us |     104.30 us |    100.90 us |     110.70 us |     448 B |
| CycleSort     | 1000   |   8,549.93 us |     872.692 us |     47.835 us |   8,559.90 us |  8,497.90 us |   8,592.00 us |      64 B |
| CycleSort     | 10000  | 107,614.00 us | 189,812.814 us | 10,404.281 us | 113,471.50 us | 95,601.40 us | 113,769.10 us |     448 B |

Span ...

| Method        | Number | Mean          | Error          | StdDev       | Median       | Min          | Max           | Allocated |
|-------------- |------- |--------------:|---------------:|-------------:|-------------:|-------------:|--------------:|----------:|
| CycleSort     | 100    |     112.00 us |       6.320 us |     0.346 us |    112.20 us |    111.60 us |     112.20 us |     736 B |
| CycleSort     | 1000   |   9,055.67 us |   1,466.106 us |    80.362 us |  9,078.00 us |  8,966.50 us |   9,122.50 us |     448 B |
| CycleSort     | 10000  | 101,010.50 us | 174,936.094 us | 9,588.838 us | 96,730.30 us | 94,307.20 us | 111,994.00 us |     736 B |

*/

/// <summary>
/// 現在の要素より後ろに、より小さな要素がないか確認しその個数から現在の要素を差し込むインデックスを算出して入れ替え。これを繰り返す。交換回数は常に少ないが、比較回数が莫大になる。
/// <br/>
/// Checks for elements smaller than the current element further in the array, calculates the insertion index based on the number of such elements, and swaps. This process is repeated. The number of swaps is always minimal, but the number of comparisons becomes enormous.
/// </summary>
/// <remarks>
/// stable  : no 
/// inplace : yes
/// Compare : O(n^2)     (Always performs approximately n(n-1)/2 comparisons)
/// Swap    : O(n)       (Minimizes swaps, theoretically optimal)
/// Index   : O(n^2)     (Each element may be accessed multiple times during comparisons)  
/// Order   : O(n^2)
///         * average   : O(n^2)  
///         * best case : O(n^2)     (comparisons are always needed)
///         * worst case: O(n^2) 
/// </remarks>
public static class CycleSort
{
    /// <summary>
    /// Sorts the specified span using Cycle Sort algorithm.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="span"></param>
    public static void Sort<T>(Span<T> span) where T : IComparable<T>
    {
        Sort(span, NullContext.Default);
    }

    /// <summary>
    /// Sorts the specified span using Cycle Sort algorithm.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="span"></param>
    /// <param name="context"></param>
    public static void Sort<T>(Span<T> span, ISortContext context) where T : IComparable<T>
    {
        if (span.Length <= 1) return;

        var s = new SortSpan<T>(span, context);

        for (var cycleStart = 0; cycleStart < span.Length - 1; cycleStart++)
        {
            var item = s.Read(cycleStart);
            var pos = FindPosition(ref s, item, cycleStart);

            // If the item is already in the correct position, skip
            if (pos == cycleStart) continue;

            // Skip duplicates
            pos = SkipDuplicates(ref s, item, pos);

            // Put the item at its correct position
            var temp = s.Read(pos);
            s.Write(pos, item);
            item = temp;

            // Rotate the rest of the cycle
            while (pos != cycleStart)
            {
                pos = FindPosition(ref s, item, cycleStart);
                pos = SkipDuplicates(ref s, item, pos);

                temp = s.Read(pos);
                s.Write(pos, item);
                item = temp;
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int FindPosition<T>(ref SortSpan<T> s, T value, int start) where T : IComparable<T>
    {
        var pos = start;
        for (var i = start + 1; i < s.Length; i++)
        {
            if (s.Compare(i, value) < 0)
            {
                pos++;
            }
        }
        return pos;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int SkipDuplicates<T>(ref SortSpan<T> s, T value, int pos) where T : IComparable<T>
    {
        while (pos < s.Length && s.Compare(value, pos) == 0)
        {
            pos++;
        }
        return pos;
    }
}
