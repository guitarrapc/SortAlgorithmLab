using System;
using System.Collections.Generic;
using System.Text;

namespace SortAlgorithm.Logics
{
    /// <summary>
    /// ピボットを2つにすることで再帰の深さを浅くすることで、QuickSortを高速化する。
    /// </summary>
    /// <remarks>
    /// stable : no
    /// inplace : yes
    /// Compare :
    /// Swap :
    /// Order : O(n log n) (Worst case : O(nlog^2n))
    /// </remarks>
    /// <typeparam name="T"></typeparam>
    public class QuickDualPivotSort<T> : SortBase<T> where T : IComparable<T>
    {
        public override SortType SortType => SortType.Partition;

        public override T[] Sort(T[] array)
        {
            base.Statics.Reset(array.Length, SortType, nameof(QuickDualPivotSort<T>));
            return Sort(array, 0, array.Length - 1);
        }

        private T[] Sort(T[] array, int left, int right)
        {
            if (right <= left) return array;

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
