using System;
using System.Runtime.CompilerServices;

namespace SortLab.Core.Logics;

/*

non span...

| Method    | Number | Mean        | Error     | StdDev   | Min         | Max         | Allocated |
|---------- |------- |------------:|----------:|---------:|------------:|------------:|----------:|
| CycleSort | 100    |    17.10 us |  2.222 us | 0.122 us |    16.98 us |    17.22 us |         - |
| CycleSort | 1000   | 1,706.31 us | 63.847 us | 3.500 us | 1,704.28 us | 1,710.35 us |       1 B |

| Method    | Number | Mean        | Error      | StdDev    | Min         | Max         | Allocated |
|---------- |------- |------------:|-----------:|----------:|------------:|------------:|----------:|
| CycleSort | 100    |    17.12 us |   0.965 us |  0.053 us |    17.09 us |    17.18 us |         - |
| CycleSort | 1000   | 1,722.36 us | 189.327 us | 10.378 us | 1,713.07 us | 1,733.56 us |       1 B |
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
            Swap(ref span[pos], ref tmp);
            Statistics.AddIndexAccess();

            // Complete the cycle
            while (pos != start)
            {
                pos = FindPosition(span, tmp, start);
                pos = SkipDuplicates(span, tmp, pos);
                Swap(ref span[pos], ref tmp);
                Statistics.AddIndexAccess();
            }
        }
        return array;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int FindPosition(ReadOnlySpan<T> span, T value, int start)
    {
        var pos = start;
        for (var i = start + 1; i < span.Length; i++)
        {
            Statistics.AddIndexAccess();
            if (Compare(span[i], value) < 0)
            {
                pos++;
            }
        }
        return pos;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int SkipDuplicates(ReadOnlySpan<T> span, T value, int pos)
    {
        while (Compare(value, span[pos]) == 0)
        {
            Statistics.AddIndexAccess();
            pos++;
        }
        return pos;
    }
}
