﻿using System;
using System.Collections.Generic;
using System.Text;

namespace SortAlgorithm.Logics
{
    /// <summary>
    /// 開始と終わりから中央値を導いて、この3点を枢軸として配列を左右で分ける。左右で中央値よりも小さい(大きい)データに置き換えてデータを分割する(分割統治)。最後に左右それぞれをソートする(この時点で不安定)ことで計算量をソート済みに抑えることができる。不安定なソート。
    /// </summary>
    /// <remarks>
    /// stable : no
    /// inplace : yes
    /// Compare : 
    /// Swap : 
    /// Order : O(n log n) (Worst case : O(nlog^2n))
    /// sortKind : QuickSort, ArraySize : 100, IndexAccessCount : 158, CompareCount : 158, SwapCount : 147
    /// </remarks>
    /// <typeparam name="T"></typeparam>
    public class QuickSort<T> : SortBase<T> where T : IComparable<T>
    {
        public override T[] Sort(T[] array)
        {
            base.sortStatics.Reset(array.Length);
            return Sort(array, 0, array.Length - 1);
        }

        private T[] Sort(T[] array, int first, int last)
        {
            if (first >= last) return array;

            // fase 1. decide pivot
            var pivot = Median(array[first], array[(first + (last - first)) / 2], array[last]);
            var l = first;
            var r = last;

            while (l <= r)
            {
                base.sortStatics.AddIndexAccess();
                base.sortStatics.AddCompareCount();
                while (l < last && array[l].CompareTo(pivot) < 0) l++;
                while (r > first && array[r].CompareTo(pivot) >= 0) r--;
                if (l > r) break;
                base.sortStatics.AddSwapCount();
                Swap(ref array[l], ref array[r]);
                l++;
                r--;
            }

            // fase 2. Sort Left and Right
            Sort(array, first, l - 1);
            Sort(array, l, last);
            return array;
        }

        //private T Median(T low, T mid, T high)
        //{
        //    if (low.CompareTo(mid) > 0) Swap(ref low, ref mid);
        //    if (low.CompareTo(high) > 0) Swap(ref low, ref high);
        //    if (mid.CompareTo(high) > 0) Swap(ref mid, ref high);
        //    return mid;
        //}

        private static T Median(T low, T mid, T high)
        {
            if (low.CompareTo(mid) > 0)
            {
                if (mid.CompareTo(high) > 0)
                {
                    return mid;
                }
                else
                {
                    return low.CompareTo(high) > 0 ? high : low;
                }
            }
            else
            {
                if (mid.CompareTo(high) > 0)
                {
                    return low.CompareTo(high) > 0 ? low : high;
                }
                else
                {
                    return mid;
                }
            }
        }
    }
}