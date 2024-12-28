using System;

namespace SortLab.Core.Logics;

/// <summary>
/// 開始と終わりから中央値を導いて、この3点を枢軸として配列を左右で分ける。左右で中央値よりも小さい(大きい)データに置き換えてデータを分割する(分割統治)。最後に左右それぞれをソートする(この時点で不安定)ことで計算量をソート済みに抑えることができる。不安定なソート。
/// </summary>
/// <remarks>
/// stable : no
/// inplace : no (log n)
/// Compare :
/// Swap :
/// Order : O(n log n) (Worst case : O(nlog^2n))
/// </remarks>
/// <typeparam name="T"></typeparam>
public class QuickSortMedian3<T> : SortBase<T> where T : IComparable<T>
{
    public override SortType SortType => SortType.Partition;

    public override T[] Sort(T[] array)
    {
        Statistics.Reset(array.Length, SortType, nameof(QuickSortMedian3<T>));
        return SortImpl(array, 0, array.Length - 1);
    }

    private T[] SortImpl(T[] array, int left, int right)
    {
        if (left >= right) return array;

        // fase 1. decide pivot
        var pivot = Median3(array[left], array[(left + (right - left)) / 2], array[right]);
        var l = left;
        var r = right;

        while (l <= r)
        {
            while (l < right && Compare(array[l], pivot) < 0)
            {
                Statistics.AddIndexCount();
                Statistics.AddCompareCount();
                l++;
            }

            while (r > left && Compare(array[r], pivot) > 0)
            {
                Statistics.AddIndexCount();
                Statistics.AddCompareCount();
                r--;
            }

            if (l > r) break;
            Swap(ref array[l], ref array[r]);
            l++;
            r--;
        }

        // fase 2. Sort Left and Right
        SortImpl(array, left, l - 1);
        SortImpl(array, l, right);
        return array;
    }

    // less efficient compatison
    //    private T Median3(T low, T mid, T high)
    //    {
    //        if (Compare(low, mid) > 0)
    //        {
    //            Swap(ref low, ref mid);
    //        }
    //        if (Compare(low, high) > 0)
    //        {
    //            Swap(ref low, ref high);
    //        }
    //        if (Compare(mid, high) > 0)
    //        {
    //            Swap(ref mid, ref high);
    //        }
    //        return mid;
    //    }

    // much more efficient comparison
    private T Median3(T low, T mid, T high)
    {
        if (Compare(low, mid) > 0)
        {
            if (Compare(mid, high) > 0)
            {
                return mid;
            }
            else
            {
                return Compare(low, high) > 0 ? high : low;
            }
        }
        else
        {
            if (Compare(mid, high) > 0)
            {
                return Compare(low, high) > 0 ? low : high;
            }
            else
            {
                return mid;
            }
        }
    }
}
