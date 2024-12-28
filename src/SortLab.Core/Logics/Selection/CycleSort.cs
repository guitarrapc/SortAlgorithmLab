using System;
using System.Runtime.CompilerServices;

namespace SortLab.Core.Logics;

/*
Ref span ...

| Method    | Number | Mean         | Error        | StdDev    | Min          | Max          | Allocated |
|---------- |------- |-------------:|-------------:|----------:|-------------:|-------------:|----------:|
| CycleSort | 100    |     151.3 us |     73.80 us |   4.05 us |     148.2 us |     155.9 us |     736 B |
| CycleSort | 1000   |  12,790.0 us |  1,195.65 us |  65.54 us |  12,720.5 us |  12,850.7 us |     448 B |
| CycleSort | 10000  | 218,218.1 us | 17,871.98 us | 979.62 us | 217,152.3 us | 219,079.2 us |     736 B |
*/

/// <summary>
/// 現在の要素より後ろに、より小さな要素がないか確認しその個数から現在の要素を差し込むインデックスを算出して入れ替え。これを繰り返す。交換回数は常に少ないが、比較回数が莫大になる。
/// </summary>
/// <remarks>
/// stable : no
/// inplace : yes
/// Compare : O(n^2)
/// Swap : O(n^2)
/// </remarks>
/// <typeparam name="T"></typeparam>
public class CycleSort<T> : SortBase<T> where T : IComparable<T>
{
    public override SortType SortType => SortType.Selection;

    public override T[] Sort(T[] array)
    {
        Statistics.Reset(array.Length, SortType, nameof(CycleSort<T>));
        var span = array.AsSpan();

        for (var start = 0; start <= span.Length - 2; start++)
        {
            // Compare value
            var tmp = span[start];
            Statistics.AddIndexAccess();

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
        return array;
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
