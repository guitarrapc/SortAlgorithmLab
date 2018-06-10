using System;
using System.Collections.Generic;
using System.Text;

namespace SortAlgorithm.Logics
{
    /// <summary>
    /// QuickSort + InsertSortによる Quick Searchでだいたいソート済みになった時に最速を目指す。Median9バージョン
    /// </summary>
    /// <remarks>
    /// stable : no
    /// inplace : yes
    /// Compare :
    /// Swap :
    /// Order : O(n log n) (Worst case : O(n nlog n))
    /// </remarks>
    /// <typeparam name="T"></typeparam>
    public class QuickSortMedian9Insert<T> : SortBase<T> where T : IComparable<T>
    {
        public override SortType SortType => SortType.Partition;

        // ref : https://github.com/nlfiedler/burstsort4j/blob/master/src/org/burstsort4j/Introsort.java
        private const int InsertThreshold = 16;
        private InsertSort<T> insertSort = new InsertSort<T>();

        public override T[] Sort(T[] array)
        {
            base.Statistics.Reset(array.Length, SortType, nameof(QuickSortMedian9Insert<T>));
            var result = SortImpl(array, 0, array.Length - 1);
            base.Statistics.AddCompareCount(insertSort.Statistics.CompareCount);
            base.Statistics.AddIndexAccess(insertSort.Statistics.IndexAccessCount);
            base.Statistics.AddSwapCount(insertSort.Statistics.SwapCount);
            return result;
        }

        private T[] SortImpl(T[] array, int left, int right)
        {
            if (left >= right) return array;

            // switch to insert sort
            if (right - left < InsertThreshold)
            {
                return insertSort.Sort(array, left, right + 1);
            }

            // fase 1. decide pivot
            base.Statistics.AddIndexAccess();
            var pivot = Median9(array, left, right);
            var l = left;
            var r = right;

            while (l <= r)
            {
                while (l < right && array[l].CompareTo(pivot) < 0)
                {
                    base.Statistics.AddIndexAccess();
                    base.Statistics.AddCompareCount();
                    l++;
                }

                while (r > left && array[r].CompareTo(pivot) > 0)
                {
                    base.Statistics.AddIndexAccess();
                    base.Statistics.AddCompareCount();
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

        private T Median3(T low, T mid, T high)
        {
            base.Statistics.AddCompareCount();
            if (low.CompareTo(mid) > 0)
            {
                base.Statistics.AddCompareCount();
                if (mid.CompareTo(high) > 0)
                {
                    return mid;
                }
                else
                {
                    base.Statistics.AddCompareCount();
                    return low.CompareTo(high) > 0 ? high : low;
                }
            }
            else
            {
                base.Statistics.AddCompareCount();
                if (mid.CompareTo(high) > 0)
                {
                    base.Statistics.AddCompareCount();
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
