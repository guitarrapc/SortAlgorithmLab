using System;

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
            // compare value
            Statistics.AddIndexAccess();
            var tmp = array[start];

            // Find position to swap.
            // start base point to find lower element on right side.
            var pos = start;
            for (var i = start + 1; i < array.Length; i++)
            {
                Statistics.AddIndexAccess();
                if (Compare(array[i], tmp) < 0)
                {
                    pos++;
                }
            }

            // fix position. not found lower element on the right, go next index.
            if (pos == start) continue;

            // ignore duplicate
            while (Compare(tmp, array[pos]) == 0)
            {
                pos++;
            }

            Swap(ref array[pos], ref tmp);

            // rest of the cycle.
            while (pos != start)
            {
                pos = start;
                for (var i = start + 1; i < array.Length; i++)
                {
                    Statistics.AddIndexAccess();
                    if (Compare(array[i], tmp) < 0)
                    {
                        pos++;
                    }
                }

                Statistics.AddIndexAccess();
                while (Compare(tmp, array[pos]) == 0)
                {
                    pos++;
                }

                Statistics.AddIndexAccess();
                if (Compare(tmp, array[pos]) != 0)
                {
                    Swap(ref array[pos], ref tmp);
                }
            }
        }
        return array;
    }
}
