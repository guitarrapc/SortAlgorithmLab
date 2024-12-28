using System;

namespace SortLab.Core.Logics;

/// <summary>
/// 配列左半分で最大の数をさがし、配列右半分で最大の数を探して両者を比較し左半分が大きければ交換。残りを配列の要素数-1で繰り返しソート済みまで行く。
/// </summary>
/// <remarks>
/// stable : no
/// inplace : no (n)
/// Compare : ....
/// Swap : ....
/// Order : .....
/// </remarks>
/// <typeparam name="T"></typeparam>
public class SlowSort<T> : SortBase<T> where T : IComparable<T>
{
    public override SortType SortType => SortType.Exchange;

    public override T[] Sort(T[] array)
    {
        Statistics.Reset(array.Length, SortType, nameof(SlowSort<T>));
        return SortImpl(array, 0, array.Length - 1);
    }

    private T[] SortImpl(T[] array, int start, int end)
    {
        if (start >= end) return array;

        var m = (start + end) / 2;
        SortImpl(array, start, m);
        SortImpl(array, m + 1, end);

        Statistics.AddIndexCount();
        if (Compare(array[end], array[m]) < 0)
        {
            Swap(ref array[end], ref array[m]);
        }
        SortImpl(array, start, end - 1);
        return array;
    }
}
