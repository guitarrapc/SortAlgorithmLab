using System;
using System.Collections.Generic;
using System.Text;

namespace SortAlgorithm.Logics
{
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
    /// sortKind : GnomeSort, ArraySize : 100, IndexAccessCount : 5008, CompareCount : 5008, SwapCount : 2456
    /// </remarks>
    /// <typeparam name="T"></typeparam>
    public class GnomeSort<T> : SortBase<T> where T : IComparable<T>
    {
        public override T[] Sort(T[] array)
        {
            base.sortStatics.Reset(array.Length);
            for (var i = 1; i < array.Length;)
            {
                base.sortStatics.AddIndexAccess();
                base.sortStatics.AddCompareCount();
                if (array[i - 1].CompareTo(array[i]) <= 0)
                {
                    i++;
                }
                else
                {
                    base.sortStatics.AddSwapCount();
                    Swap(ref array[i - 1], ref array[i]);
                    i -= 1;
                    if (i == 0) i = 1;
                }

            }
            return array;
        }
    }

    /// <remarks>
    /// sortKind : GnomeSort2, ArraySize : 100, IndexAccessCount : 2552, CompareCount : 2552, SwapCount : 2456
    /// </remarks>
    /// <typeparam name="T"></typeparam>

    public class GnomeSort2<T> : SortBase<T> where T : IComparable<T>
    {
        public override T[] Sort(T[] array)
        {
            base.sortStatics.Reset(array.Length);
            for (var i = 0; i < array.Length - 1; i++)
            {
                base.sortStatics.AddIndexAccess();
                base.sortStatics.AddCompareCount();
                if (array[i].CompareTo(array[i + 1]) > 0)
                {
                    base.sortStatics.AddSwapCount();
                    Swap(ref array[i], ref array[i + 1]);
                    for (var j = i; j > 0; j--)
                    {
                        base.sortStatics.AddIndexAccess();
                        base.sortStatics.AddCompareCount();

                        if (array[j - 1].CompareTo(array[j]) <= 0) break;
                        base.sortStatics.AddSwapCount();
                        Swap(ref array[j - 1], ref array[j]);
                    }
                }
            }
            return array;
        }
    }

    /// <remarks>
    /// sortKind : GnomeSort3, ArraySize : 100, IndexAccessCount : 5012, CompareCount : 5012, SwapCount : 2456
    /// </remarks>
    /// <typeparam name="T"></typeparam>
    public class GnomeSort3<T> : SortBase<T> where T : IComparable<T>
    {
        public override T[] Sort(T[] array)
        {
            var i = 0;
            base.sortStatics.Reset(array.Length);
            while (i < array.Length)
            {
                base.sortStatics.AddIndexAccess();
                base.sortStatics.AddCompareCount();
                if (i == 0 || array[i - 1].CompareTo(array[i]) <= 0)
                {
                    i++;
                }
                else
                {
                    base.sortStatics.AddSwapCount();
                    Swap(ref array[i], ref array[i - 1]);
                    --i;
                }
            }
            return array;
        }
    }
}
