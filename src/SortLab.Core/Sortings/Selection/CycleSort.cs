namespace SortLab.Core.Sortings;

/*

Ref span ...

| Method        | Number | Mean          | Error          | StdDev        | Median        | Min          | Max           | Allocated |
|-------------- |------- |--------------:|---------------:|--------------:|--------------:|-------------:|--------------:|----------:|
| CycleSort     | 100    |     105.30 us |      90.780 us |      4.976 us |     104.30 us |    100.90 us |     110.70 us |     448 B |
| CycleSort     | 1000   |   8,549.93 us |     872.692 us |     47.835 us |   8,559.90 us |  8,497.90 us |   8,592.00 us |      64 B |
| CycleSort     | 10000  | 107,614.00 us | 189,812.814 us | 10,404.281 us | 113,471.50 us | 95,601.40 us | 113,769.10 us |     448 B |

*/

/// <summary>
/// 現在の要素より後ろに、より小さな要素がないか確認しその個数から現在の要素を差し込むインデックスを算出して入れ替え。これを繰り返す。交換回数は常に少ないが、比較回数が莫大になる。
/// <br/>
/// Checks for elements smaller than the current element further in the array, calculates the insertion index based on the number of such elements, and swaps. This process is repeated. The number of swaps is always minimal, but the number of comparisons becomes enormous.
/// </summary>
/// <remarks>
/// stable  : no 
/// inplace : yes
/// Compare : O(n^2)
/// Swap    : O(n)
/// Index   : O(n^2) (Each element may be accessed multiple times during comparisons)  
/// Order   : O(n^2)  
///         * average:                   O(n^2)  
///         * best case (nearly sorted): O(n)  
///         * worst case can approach:   O(n^2) 
/// </remarks>
/// <typeparam name="T"></typeparam>
public class CycleSort<T> : SortBase<T> where T : IComparable<T>
{
    public override SortMethod SortType => SortMethod.Selection;
    protected override string Name => nameof(CycleSort<T>);

    public override T[] Sort(T[] array)
    {
        Statistics.Reset(array.Length, SortType, Name);
        var span = array.AsSpan();
        SortCore(span);

        return array;
    }

    public void Sort(Span<T> span)
    {
        Statistics.Reset(span.Length, SortType, Name);
        SortCore(span);
    }

    private void SortCore(Span<T> span)
    {
        for (var start = 0; start <= span.Length - 2; start++)
        {
            // Compare value
            var tmp = Index(ref span, start);

            // Find position to swap, start base point to find lower element on right side.
            var pos = FindPosition(span, tmp, start);

            // If no swap needed, continue to the next cycle
            if (pos == start) continue;

            // Ignore duplicate
            pos = SkipDuplicates(span, tmp, pos);

            // Perform the initial swap
            Swap(ref Index(ref span, pos), ref tmp);

            // Complete the cycle
            while (pos != start)
            {
                pos = FindPosition(span, tmp, start);
                pos = SkipDuplicates(span, tmp, pos);
                Swap(ref Index(ref span, pos), ref tmp);
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int FindPosition(Span<T> span, T value, int start)
    {
        var pos = start;
        for (var i = start + 1; i < span.Length; i++)
        {
            if (Compare(Index(ref span, i), value) < 0)
            {
                pos++;
            }
        }
        return pos;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int SkipDuplicates(Span<T> span, T value, int pos)
    {
        while (Compare(value, Index(ref span, pos)) == 0)
        {
            pos++;
        }
        return pos;
    }
}
