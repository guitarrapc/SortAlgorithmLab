using System;
using System.Collections.Generic;
using System.Text;

namespace SortAlgorithm.Logics
{
    /// <summary>
    /// Dual-Pivot QuickSort に InsertSortを組み合わせて最速を狙う
    /// </summary>
    /// <remarks>
    /// stable : no
    /// inplace : yes
    /// Compare :
    /// Swap :
    /// Order : O(n log n) (Worst case : O(nlog^2n))
    /// ArraySize : 100, IsSorted : True, sortKind : QuickDualPivotSortInsert, IndexAccessCount : 296, CompareCount : 361, SwapCount : 222
    /// ArraySize : 1000, IsSorted : True, sortKind : QuickDualPivotSortInsert, IndexAccessCount : 4816, CompareCount : 6313, SwapCount : 3240
    /// ArraySize : 10000, IsSorted : True, sortKind : QuickDualPivotSortInsert, IndexAccessCount : 77049, CompareCount : 105503, SwapCount : 50877
    /// </remarks>
    /// <typeparam name="T"></typeparam>
    public class QuickDualPivotSortInsert<T> : SortBase<T> where T : IComparable<T>
    {
        public override SortType SortType => SortType.Exchange;

        private const int InsertThreshold = 16;
        private InsertSort<T> insertSort = new InsertSort<T>();

        public override T[] Sort(T[] array)
        {
            base.Statics.Reset(array.Length, SortType, nameof(QuickDualPivotSortInsert<T>));
            return Sort(array, 0, array.Length - 1);
        }

        private T[] Sort(T[] array, int left, int right)
        {
            if (right <= left) return array;

            // switch to insert sort
            if (right - left < InsertThreshold)
            {
                return insertSort.Sort(array, left, right + 1);
            }

            base.Statics.AddCompareCount();
            // fase 0. Make sure left item is lower than right item
            if (array[left].CompareTo(array[right]) > 0)
            {
                Swap(ref array[left], ref array[right]);
            }

            // fase 1. decide pivot
            var l = left + 1;
            var k = l;
            var g = right - 1;

            while (k <= g)
            {
                base.Statics.AddIndexAccess();
                base.Statics.AddCompareCount();
                if (array[k].CompareTo(array[left]) < 0)
                {
                    Swap(ref array[k], ref array[l]);
                    k++;
                    l++;
                }
                else if (array[right].CompareTo(array[k]) < 0)
                {
                    base.Statics.AddCompareCount();
                    Swap(ref array[k], ref array[g]);
                    g--;
                }
                else
                {
                    k++;
                }
            }

            l--;
            g++;
            Swap(ref array[left], ref array[l]);
            Swap(ref array[right], ref array[g]);

            // fase 2. Sort Left, Mid and righ
            Sort(array, left, l - 1);
            if (array[left].CompareTo(array[right]) < 0)
            {
                Sort(array, l + 1, g - 1);
            }
            Sort(array, g + 1, right);
            return array;
        }
    }
}
