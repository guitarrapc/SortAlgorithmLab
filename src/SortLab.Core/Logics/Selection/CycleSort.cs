using System;
using System.Runtime.CompilerServices;

namespace SortLab.Core.Logics;

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

        for (var start = 0; start <= array.Length - 2; start++)
        {
            // Compare value
            var tmp = array[start];
            Statistics.AddIndexAccess();

            // Find position to swap, start base point to find lower element on right side.
            var pos = FindPosition(array, tmp, start);

            // If no swap needed, continue to the next cycle
            if (pos == start) continue;

            // Ignore duplicate
            pos = SkipDuplicates(array, tmp, pos);

            // Perform the initial swap
            Swap(ref array[pos], ref tmp);
            Statistics.AddIndexAccess();

            // Complete the cycle
            while (pos != start)
            {
                pos = FindPosition(array, tmp, start);
                pos = SkipDuplicates(array, tmp, pos);
                Swap(ref array[pos], ref tmp);
                Statistics.AddIndexAccess();
            }
        }
        return array;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int FindPosition(T[] array, T value, int start)
    {
        var pos = start;
        for (var i = start + 1; i < array.Length; i++)
        {
            Statistics.AddIndexAccess();
            if (Compare(array[i], value) < 0)
            {
                pos++;
            }
        }
        return pos;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int SkipDuplicates(T[] array, T value, int pos)
    {
        while (Compare(value, array[pos]) == 0)
        {
            Statistics.AddIndexAccess();
            pos++;
        }
        return pos;
    }
}
