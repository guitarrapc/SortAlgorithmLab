using System;
using System.Collections.Generic;
using System.Text;

namespace SortAlgorithm.Logics
{
    /// <summary>
    /// QuickSortのMedian-3 Killer対策に、Median9を採用する。ランダムデータでは若干おそくなるものの、山型データでは高速化、および最悪ケースの頻度は下がる
    /// </summary>
    /// <remarks>
    /// stable : no
    /// inplace : yes
    /// Compare : 
    /// Swap : 
    /// Order : O(n log n) (Worst case : O(nlog^2n))
    /// ArraySize : 100, IsSorted : True, sortKind : QuickSortMedian9BinaryInsert, IndexAccessCount : 293, CompareCount : 337, SwapCount : 109
    /// ArraySize : 1000, IsSorted : True, sortKind : QuickSortMedian9BinaryInsert, IndexAccessCount : 4371, CompareCount : 5127, SwapCount : 1630
    /// ArraySize : 10000, IsSorted : True, sortKind : QuickSortMedian9BinaryInsert, IndexAccessCount : 56984, CompareCount : 65687, SwapCount : 24351
    /// </remarks>
    /// <typeparam name="T"></typeparam>
    public class QuickSortMedian9<T> : SortBase<T> where T : IComparable<T>
    {
        public override SortType SortType => SortType.Exchange;

        public override T[] Sort(T[] array)
        {
            base.Statics.Reset(array.Length);
            return Sort(array, 0, array.Length - 1);
        }

        private T[] Sort(T[] array, int left, int right)
        {
            if (left >= right) return array;

            // fase 1. decide pivot
            var pivot = Median9(array, left, right);
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

        private T Median3(T low, T mid, T high)
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

        private T Median9(T[] array, int low, int high)
        {
            var m2 = (high - low) / 2;
            var m4 = m2 / 2;
            var m8 = m4 / 2;
            var a = array[low];
            var b = array[low + m8];
            var c = array[low + m4];
            var d = array[low + m2 - m8];
            var e = array[low + m2];
            var f = array[low + m2 + m8];
            var g = array[high - m4];
            var h = array[high - m8];
            var i = array[high];
            return Median3(Median3(a, b, c), Median3(d, e, f), Median3(g, h, i));
        }
    }
}
