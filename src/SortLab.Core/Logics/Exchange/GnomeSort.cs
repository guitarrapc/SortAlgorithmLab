﻿using System;

namespace SortLab.Core.Logics;

/// <summary>
/// リストの先頭に戻る前に、前回の位置を覚えておくことで<see cref="GnomeSort1{T}"/>を最適化している。これにより、<see cref="InsertSort{T}"/>と同程度の計算量になる。
/// </summary>
/// <remarks>
/// stable : yes
/// inplace : yes
/// Compare :
/// Swap :
/// Order : O(n^2)
/// </remarks>
/// <typeparam name="T"></typeparam>
public class GnomeSort<T> : SortBase<T> where T : IComparable<T>
{
    public override SortType SortType => SortType.Exchange;

    public override T[] Sort(T[] array)
    {
        base.Statistics.Reset(array.Length, SortType, nameof(GnomeSort<T>));
        for (var i = 0; i < array.Length; i++)
        {
            base.Statistics.AddIndexAccess();
            while (i > 0 && array[i - 1].CompareTo(array[i]) > 0)
            {
                base.Statistics.AddCompareCount();
                Swap(ref array[i - 1], ref array[i]);
                i--;
            }
        }
        return array;
    }
}

/// <summary>
/// 目の前の要素が現在の要素より小さければ前に、大きければ入れ替えて後ろに進む。一番後ろに行ったときは、前に進む。<see cref="InsertSort{T}"/>に似ているが、挿入ではなく交換となっている。常に直前とのみ比較するので、ソート中に末尾要素が追加されも問題ない。
/// BubbleSortよりは高速でInsertSort程度
/// </summary>
/// <remarks>
/// stable : yes
/// inplace : yes
/// Compare :
/// Swap :
/// Order : O(n^2)
/// </remarks>
/// <typeparam name="T"></typeparam>
public class GnomeSort1<T> : SortBase<T> where T : IComparable<T>
{
    public override SortType SortType => SortType.Exchange;

    public override T[] Sort(T[] array)
    {
        base.Statistics.Reset(array.Length, SortType, nameof(GnomeSort1<T>));
        for (var i = 1; i < array.Length;)
        {
            base.Statistics.AddIndexAccess();
            base.Statistics.AddCompareCount();
            if (array[i - 1].CompareTo(array[i]) <= 0)
            {
                i++;
            }
            else
            {
                Swap(ref array[i - 1], ref array[i]);
                i -= 1;
                if (i == 0) i = 1;
            }

        }
        return array;
    }
}

/// <summary>
/// <see cref="GnomeSort{T}"/> に似ているが最適化がされていない
/// </summary>
/// <typeparam name="T"></typeparam>
public class GnomeSort2<T> : SortBase<T> where T : IComparable<T>
{
    public override SortType SortType => SortType.Exchange;

    public override T[] Sort(T[] array)
    {
        base.Statistics.Reset(array.Length, SortType, nameof(GnomeSort2<T>));
        for (var i = 0; i < array.Length - 1; i++)
        {
            base.Statistics.AddCompareCount();
            if (array[i].CompareTo(array[i + 1]) > 0)
            {
                Swap(ref array[i], ref array[i + 1]);
                for (var j = i; j > 0; j--)
                {
                    base.Statistics.AddIndexAccess();
                    base.Statistics.AddCompareCount();
                    if (array[j - 1].CompareTo(array[j]) <= 0) break;

                    Swap(ref array[j - 1], ref array[j]);
                }
            }
            else
            {
                base.Statistics.AddIndexAccess();
            }
        }
        return array;
    }
}

/// <summary>
/// 実装はシンプルだが遅い
/// </summary>
/// <typeparam name="T"></typeparam>
public class GnomeSort3<T> : SortBase<T> where T : IComparable<T>
{
    public override SortType SortType => SortType.Exchange;

    public override T[] Sort(T[] array)
    {
        base.Statistics.Reset(array.Length, SortType, nameof(GnomeSort3<T>));

        var i = 0;
        while (i < array.Length)
        {
            base.Statistics.AddIndexAccess();
            base.Statistics.AddCompareCount();
            if (i == 0 || array[i - 1].CompareTo(array[i]) <= 0)
            {
                i++;
            }
            else
            {
                Swap(ref array[i], ref array[i - 1]);
                --i;
            }
        }
        return array;
    }
}
