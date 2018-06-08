using System;
using System.Collections.Generic;
using System.Text;

namespace SortAlgorithm.Logics
{
    /// <summary>
    /// リストの先頭に戻る前に、前回の位置を覚えておくことで<see cref="GnomeSort1{T}"/>を最適化している。これにより、<see cref="InsertSort{T}"/>と同程度の計算量になる。
    /// </summary>
    /// <remarks>
    /// stable : yes
    /// inplace : yes
    /// Compare : 
    /// Swap : 
    /// Order : O(n^2)
    /// ArraySize : 100, IsSorted : True, sortKind : GnomeSort, IndexAccessCount : 2591, CompareCount : 2491, SwapCount : 2491
    /// ArraySize : 1000, IsSorted : True, sortKind : GnomeSort, IndexAccessCount : 255017, CompareCount : 254017, SwapCount : 254017
    /// ArraySize : 10000, IsSorted : True, sortKind : GnomeSort, IndexAccessCount : 24773456, CompareCount : 24763456, SwapCount : 24763456
    /// </remarks>
    /// <typeparam name="T"></typeparam>
    public class GnomeSort<T> : SortBase<T> where T : IComparable<T>
    {
        public override SortType SortType => SortType.Exchange;

        public override T[] Sort(T[] array)
        {
            base.Statics.Reset(array.Length);
            for (var i = 0; i < array.Length; i++)
            {
                base.Statics.AddIndexAccess();
                while (i > 0 && array[i - 1].CompareTo(array[i]) > 0)
                {
                    base.Statics.AddCompareCount();
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
    /// ArraySize : 100, IsSorted : True, sortKind : GnomeSort1, IndexAccessCount : 5077, CompareCount : 5077, SwapCount : 2491
    /// ArraySize : 1000, IsSorted : True, sortKind : GnomeSort1, IndexAccessCount : 509029, CompareCount : 509029, SwapCount : 254017
    /// ArraySize : 10000, IsSorted : True, sortKind : GnomeSort1, IndexAccessCount : 49536899, CompareCount : 49536899, SwapCount : 24763456
    /// </remarks>
    /// <typeparam name="T"></typeparam>
    public class GnomeSort1<T> : SortBase<T> where T : IComparable<T>
    {
        public override SortType SortType => SortType.Exchange;

        public override T[] Sort(T[] array)
        {
            base.Statics.Reset(array.Length);
            for (var i = 1; i < array.Length;)
            {
                base.Statics.AddIndexAccess();
                base.Statics.AddCompareCount();
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

    /// <remarks>
    /// ArraySize : 100, IsSorted : True, sortKind : GnomeSort2, IndexAccessCount : 2488, CompareCount : 2586, SwapCount : 2491
    /// ArraySize : 1000, IsSorted : True, sortKind : GnomeSort2, IndexAccessCount : 254018, CompareCount : 255012, SwapCount : 254017
    /// ArraySize : 10000, IsSorted : True, sortKind : GnomeSort2, IndexAccessCount : 24763454, CompareCount : 24773443, SwapCount : 24763456
    /// </remarks>
    /// <typeparam name="T"></typeparam>

    public class GnomeSort2<T> : SortBase<T> where T : IComparable<T>
    {
        public override SortType SortType => SortType.Exchange;

        public override T[] Sort(T[] array)
        {
            base.Statics.Reset(array.Length);
            for (var i = 0; i < array.Length - 1; i++)
            {
                base.Statics.AddCompareCount();
                if (array[i].CompareTo(array[i + 1]) > 0)
                {
                    Swap(ref array[i], ref array[i + 1]);
                    for (var j = i; j > 0; j--)
                    {
                        base.Statics.AddIndexAccess();
                        base.Statics.AddCompareCount();
                        if (array[j - 1].CompareTo(array[j]) <= 0) break;

                        Swap(ref array[j - 1], ref array[j]);
                    }
                }
                else
                {
                    base.Statics.AddIndexAccess();
                }
            }
            return array;
        }
    }

    /// <remarks>
    /// ArraySize : 100, IsSorted : True, sortKind : GnomeSort3, IndexAccessCount : 5082, CompareCount : 5082, SwapCount : 2491
    /// ArraySize : 1000, IsSorted : True, sortKind : GnomeSort3, IndexAccessCount : 509034, CompareCount : 509034, SwapCount : 254017
    /// ArraySize : 10000, IsSorted : True, sortKind : GnomeSort3, IndexAccessCount : 49536912, CompareCount : 49536912, SwapCount : 24763456
    /// </remarks>
    /// <typeparam name="T"></typeparam>
    public class GnomeSort3<T> : SortBase<T> where T : IComparable<T>
    {
        public override SortType SortType => SortType.Exchange;

        public override T[] Sort(T[] array)
        {
            var i = 0;
            base.Statics.Reset(array.Length);
            while (i < array.Length)
            {
                base.Statics.AddIndexAccess();
                base.Statics.AddCompareCount();
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
}
