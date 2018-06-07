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
            base.Statics.Reset(array.Length);
            return Sort(array, 0, array.Length - 1);
        }

        private T[] Sort(T[] array, int left, int right)
        {
            if (left >= right) return array;

            // fase 1. decide pivot
            var pivot = Median(array[left], array[(left + (right - left)) / 2], array[right]);
            var l = left;
            var r = right;

            while (l <= r)
            {
                while (l < right && array[l].CompareTo(pivot) < 0)
                {
                    base.Statics.AddIndexAccess();
                    base.Statics.AddCompareCount();
                    l++;
                }

                while (r > left && array[r].CompareTo(pivot) > 0)
                {
                    base.Statics.AddIndexAccess();
                    base.Statics.AddCompareCount();
                    r--;
                }

                if (l > r) break;
                Swap(ref array[l], ref array[r]);
                l++;
                r--;
            }

            // fase 2. Sort Left and Right
            Sort(array, left, l - 1);
            Sort(array, l, right);
            return array;
        }

        // less efficient compatison
        //    private T Median(T low, T mid, T high)
        //    {
        //        base.Statics.AddCompareCount();
        //        if (low.CompareTo(mid) > 0)
        //        {
        //            base.Statics.AddCompareCount();
        //            Swap(ref low, ref mid);
        //        }
        //        base.Statics.AddCompareCount();
        //        if (low.CompareTo(high) > 0)
        //        {
        //            base.Statics.AddCompareCount();
        //            Swap(ref low, ref high);
        //        }
        //        base.Statics.AddCompareCount();
        //        if (mid.CompareTo(high) > 0)
        //        {
        //            base.Statics.AddCompareCount();
        //            Swap(ref mid, ref high);
        //        }
        //        return mid;
        //    }

        // much more efficient comparison
        private T Median(T low, T mid, T high)
        {
            base.Statics.AddCompareCount();
            if (low.CompareTo(mid) > 0)
            {
                base.Statics.AddCompareCount();
                if (mid.CompareTo(high) > 0)
                {
                    return mid;
                }
                else
                {
                    base.Statics.AddCompareCount();
                    return low.CompareTo(high) > 0 ? high : low;
                }
            }
            else
            {
                base.Statics.AddCompareCount();
                if (mid.CompareTo(high) > 0)
                {
                    base.Statics.AddCompareCount();
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
