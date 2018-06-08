using System;
using System.Collections.Generic;
using System.Text;

namespace SortAlgorithm.Logics
{
    /// <summary>
    /// QuickSort + HeapSort + InsertSort によるQuickSortの最悪ケースでのO(n^2) を回避する実装。一定の深度以下になった場合にHeapSortにスイッチすることで最悪ケースを防ぎ、ほぼソート済み状況ではInsertSortで最速を狙う。
    /// </summary>
    /// <remarks>
    /// stable : no
    /// inplace : yes
    /// Compare : n log n
    /// Swap : n log n
    /// Order : O(n log n) (Worst case : O(n log n))
    /// ArraySize : 100, IsSorted : True, sortKind : IntroSortMedian3, IndexAccessCount : 236, CompareCount : 216, SwapCount : 123
    /// ArraySize : 1000, IsSorted : True, sortKind : IntroSortMedian3, IndexAccessCount : 4895, CompareCount : 5062, SwapCount : 1561
    /// ArraySize : 10000, IsSorted : True, sortKind : IntroSortMedian3, IndexAccessCount : 74461, CompareCount : 76123, SwapCount : 23871
    /// </remarks>
    /// <typeparam name="T"></typeparam>
    public class IntroSortMedian3<T> : SortBase<T> where T : IComparable<T>
    {
        public override SortType SortType => SortType.Hybrid;

        // ref : https://www.cs.waikato.ac.nz/~bernhard/317/source/IntroSort.java
        private const int IntroThreshold = 16;
        private HeapSort<T> heapSort = new HeapSort<T>();
        private InsertSort<T> insertSort = new InsertSort<T>();

        public override T[] Sort(T[] array)
        {
            base.Statics.Reset(array.Length, SortType, nameof(IntroSortMedian3<T>));
            var result = Sort(array, 0, array.Length - 1, 2 * FloorLog(array.Length));
            base.Statics.AddCompareCount(heapSort.Statics.CompareCount);
            base.Statics.AddIndexAccess(heapSort.Statics.IndexAccessCount);
            base.Statics.AddSwapCount(heapSort.Statics.SwapCount);
            base.Statics.AddCompareCount(insertSort.Statics.CompareCount);
            base.Statics.AddIndexAccess(insertSort.Statics.IndexAccessCount);
            base.Statics.AddSwapCount(insertSort.Statics.SwapCount);
            return result;
        }

        private T[] Sort(T[] array, int left, int right, int depthLimit)
        {
            while (right - left > IntroThreshold)
            {
                if (depthLimit == 0)
                {
                    heapSort.Sort(array, left, right);
                    return array;
                }
                depthLimit--;
                base.Statics.AddIndexAccess();
                var partition = Partition(array, left, right, Median3(array[left], array[left + ((right - left) / 2) + 1], array[right - 1]));
                Sort(array, partition, right, depthLimit);
                right = partition;
            }
            return insertSort.Sort(array, left, right + 1);
        }

        private int Partition(T[] array, int left, int right, T pivot)
        {
            var l = left;
            var r = right;
            while (true)
            {
                while (array[l].CompareTo(pivot) < 0)
                {
                    base.Statics.AddIndexAccess();
                    base.Statics.AddCompareCount();
                    l++;
                }
                r--;
                while (pivot.CompareTo(array[r]) < 0)
                {
                    base.Statics.AddIndexAccess();
                    base.Statics.AddCompareCount();
                    r--;
                }

                if (!(l < r))
                {
                    return l;
                }

                Swap(ref array[l], ref array[r]);
                l++;
            }
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

        private static int FloorLog(int length)
        {
            return (int)(Math.Floor(Math.Log(length) / Math.Log(2)));
        }
    }
}
