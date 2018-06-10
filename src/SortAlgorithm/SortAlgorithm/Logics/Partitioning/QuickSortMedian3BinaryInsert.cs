using System;
using System.Collections.Generic;
using System.Text;

namespace SortAlgorithm.Logics
{
    /// <summary>
    /// QuickSort + BinaryInsertSortによる Quick Searchでだいたいソート済みになった時に最速を目指すが、InsertSortの方がわずかに効率が良くBinarySearchのコストが目立つ
    /// </summary>
    /// <remarks>
    /// stable : no
    /// inplace : yes
    /// Compare :
    /// Swap :
    /// Order : O(n log n) (Worst case : O(n nlog n))
    /// </remarks>
    /// <typeparam name="T"></typeparam>
    public class QuickSortMedian3BinaryInsert<T> : SortBase<T> where T : IComparable<T>
    {
        public override SortType SortType => SortType.Partition;

        // ref : https://github.com/nlfiedler/burstsort4j/blob/master/src/org/burstsort4j/Introsort.java
        private const int InsertThreshold = 16;
        private BinaryInsertSort<T> insertSort = new BinaryInsertSort<T>();

        public override T[] Sort(T[] array)
        {
            base.Statistics.Reset(array.Length, SortType, nameof(QuickSortMedian3BinaryInsert<T>));
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
            var pivot = Median3(array[left], array[(left + (right - left)) / 2], array[right]);
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
    }
}
