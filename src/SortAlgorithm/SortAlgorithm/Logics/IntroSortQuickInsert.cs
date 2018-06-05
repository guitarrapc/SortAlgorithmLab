using System;
using System.Collections.Generic;
using System.Text;

namespace SortAlgorithm.Logics
{
    /// <summary>
    /// QuickSort + InsertSortによる IntroSort(HeapSortが入っていないので微妙)。閾値以下の要素になった時にInsertSortに切り替わることでワーストケースをつぶす。
    /// </summary>
    /// <remarks>
    /// stable : no
    /// inplace : yes
    /// Compare : 
    /// Swap : 
    /// Order : O(n log n) (Worst case : O(n nlog n))
    /// sortKind : IntroSortQuickInsert, ArraySize : 100, IndexAccessCount : 384, CompareCount : 384, SwapCount : 129
    /// </remarks>
    /// <typeparam name="T"></typeparam>
    public class IntroSortQuickInsert<T> : SortBase<T> where T : IComparable<T>
    {
        // ref : https://github.com/nlfiedler/burstsort4j/blob/master/src/org/burstsort4j/Introsort.java
        private const int IntroThreshold = 16;
        private InsertSort<T> insertSort = new InsertSort<T>();

        public override T[] Sort(T[] array)
        {
            base.sortStatics.Reset(array.Length);
            var result = Sort(array, 0, array.Length - 1);
            base.SortStatics.AddCompareCount(insertSort.SortStatics.CompareCount);
            base.SortStatics.AddIndexAccess(insertSort.SortStatics.IndexAccessCount);
            base.SortStatics.AddSwapCount(insertSort.SortStatics.SwapCount);
            return result;
        }

        private T[] Sort(T[] array, int first, int last)
        {
            if (first >= last) return array;

            // switch to insert sort
            if (last - first < IntroThreshold)
            {
                return insertSort.Sort(array, first, last + 1);
            }

            // fase 1. decide pivot
            var pivot = Median(array[first], array[(first + (last - first)) / 2], array[last]);
            var l = first;
            var r = last;

            while (l <= r)
            {
                base.sortStatics.AddIndexAccess();
                base.sortStatics.AddCompareCount();
                while (l < last && array[l].CompareTo(pivot) < 0) l++;

                base.sortStatics.AddCompareCount();
                while (r > first && array[r].CompareTo(pivot) > 0) r--;

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
