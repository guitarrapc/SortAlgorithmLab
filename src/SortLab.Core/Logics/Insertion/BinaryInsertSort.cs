using System;

namespace SortLab.Core.Logics;

/// <summary>
/// 通常のリニアサーチと異なり、挿入一を2分探索して決定するため比較範囲、回数を改善できる。安定ソート
/// ソート済み配列には早いが、Reverse配列には遅い
/// </summary>
/// <remarks>
/// stable : yes
/// inplace : yes
/// Compare : log n
/// Swap : n^2/2
/// Order : O(n^2) (Better case : O(n)) (Worst case : O(n^2))
/// </remarks>
/// <typeparam name="T"></typeparam>
public class BinaryInsertSort<T> : SortBase<T> where T : IComparable<T>
{
    public override SortType SortType => SortType.Insertion;

    public override T[] Sort(T[] array)
    {
        base.Statistics.Reset(array.Length, SortType, nameof(BinaryInsertSort<T>));
        SortImpl(array, 0, array.Length);
        return array;
    }

    public T[] Sort(T[] array, int first, int last)
    {
        base.Statistics.Reset(array.Length, SortType, nameof(BinaryInsertSort<T>));
        SortImpl(array, first, last);
        return array;
    }

    public T[] Sort(T[] array, int first, int last, int start)
    {
        base.Statistics.Reset(array.Length, SortType, nameof(BinaryInsertSort<T>));
        SortImpl(array, first, last, start);
        return array;
    }

    private T[] SortImpl(T[] array, int first, int last)
    {
        // C# Managed Code BinarySearch + Swap
        // for (var i = first + 1; i < last; i++)
        // {
        //     var j = Array.BinarySearch(array, 0, i, tmp);
        //     Array.Copy(array, j, array, j+1, i-j);
        //     Swap(ref array[j], ref tmp);
        // }

        // Handmade BinarySearch + Swap
        for (var i = first + 1; i < last; i++)
        {
            base.Statistics.AddIndexAccess();
            var tmp = array[i];

            // BinarySearch
            var left = BinarySearch(ref array, tmp, i);

            // Stable Sort
            for (var j = left; j <= i; j++)
            {
                base.Statistics.AddIndexAccess();
                Swap(ref array[j], ref tmp);
            }
        }
        return array;
    }

    private T[] SortImpl(T[] array, int first, int last, int start)
    {
        if (start == first)
        {
            start++;
        }

        for (; start < last; start++)
        {
            base.Statistics.AddIndexAccess();
            var tmp = array[start];

            // BinarySearch
            var left = BinarySearch(ref array, tmp, start);

            // Stable Sort
            for (var n = start - left; n > 0; n--)
            {
                base.Statistics.AddIndexAccess();
                Swap(ref array[left + n], ref array[left + n - 1]);
            }

            base.Statistics.AddIndexAccess();
            Swap(ref array[left], ref tmp);
        }
        return array;
    }

    private int BinarySearch(ref T[] array, T tmp, int index)
    {
        var left = 0;
        var right = index;
        while (left < right)
        {
            base.Statistics.AddIndexAccess();
            base.Statistics.AddCompareCount();
            var mid = (left + right) / 2;
            if (array[mid].CompareTo(tmp) <= 0)
            {
                left = mid + 1;
            }
            else
            {
                right = mid;
            }
        }
        return left;
    }
}
