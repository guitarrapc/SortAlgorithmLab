using System;

namespace SortLab.Core.Logics;

/// <summary>
/// 0..n で最大の値を示すインデックスmaxを探し、0とmaxでフリップしてから0と整列していない最後の要素でフリップ、これを繰り返してソートするまで実行。フリップ数がカウントに相当し、枚数が少ない時は早いが多くなると遅い。
/// </summary>
/// <remarks>
/// stable : no
/// inplace : log n
/// Compare :
/// Swap :
/// Order : n (Best case: n )
/// </remarks>
/// <typeparam name="T"></typeparam>
public class PancakeSort<T> : SortBase<T> where T : IComparable<T>
{
    public override SortType SortType => SortType.Other;

    public override T[] Sort(T[] array)
    {
        Statistics.Reset(array.Length, SortType, nameof(PancakeSort<T>));
        return SortImpl(array);
    }

    private T[] SortImpl(T[] array)
    {
        for (var currentSize = array.Length; currentSize > 1; currentSize--)
        {
            var maxIndex = MaxIndex(array, currentSize);
            if (maxIndex != currentSize - 1)
            {
                Flip(array, maxIndex);
                Flip(array, currentSize - 1);
            }
        }
        return array;
    }


    private int MaxIndex(T[] array, int n)
    {
        var max = 0;
        for (int i = 0; i < n; i++)
        {
            Statistics.AddIndexCount();
            if (Compare(array[i], array[max]) > 0)
            {
                max = i;
            }
        }
        return max;
    }

    private void Flip(T[] array, int i)
    {
        var start = 0;
        while (start < i)
        {
            Swap(ref array[start], ref array[i]);
            start++;
            i--;
        }
    }
}
