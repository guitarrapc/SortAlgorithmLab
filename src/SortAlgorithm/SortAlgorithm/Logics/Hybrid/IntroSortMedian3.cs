using System;
using System.Collections.Generic;
using System.Text;

namespace SortAlgorithm.Logics
{
    /// <summary>
    /// Contains Bug on HeapSort.
    /// QuickSort + HeapSort + InsertSort によるQuickSortの最悪ケースでのO(n^2) を回避する実装。一定の深度以下になった場合にHeapSortにスイッチすることで最悪ケースを防ぎ、ほぼソート済み状況ではInsertSortで最速を狙う。
    /// </summary>
    /// <remarks>
    /// stable : no
    /// inplace : yes
    /// Compare : n log n
    /// Swap : n log n
    /// Order : O(n log n) (Worst case : O(n log n))
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
            base.Statistics.Reset(array.Length, SortType, nameof(IntroSortMedian3<T>));
            var result = Sort(array, 0, array.Length - 1, 2 * FloorLog(array.Length));
            base.Statistics.AddCompareCount(heapSort.Statistics.CompareCount);
            base.Statistics.AddIndexAccess(heapSort.Statistics.IndexAccessCount);
            base.Statistics.AddSwapCount(heapSort.Statistics.SwapCount);
            base.Statistics.AddCompareCount(insertSort.Statistics.CompareCount);
            base.Statistics.AddIndexAccess(insertSort.Statistics.IndexAccessCount);
            base.Statistics.AddSwapCount(insertSort.Statistics.SwapCount);
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
                base.Statistics.AddIndexAccess();
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
                    base.Statistics.AddIndexAccess();
                    base.Statistics.AddCompareCount();
                    l++;
                }
                r--;
                while (pivot.CompareTo(array[r]) < 0)
                {
                    base.Statistics.AddIndexAccess();
                    base.Statistics.AddCompareCount();
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

        private static int FloorLog(int length)
        {
            return (int)(Math.Floor(Math.Log(length) / Math.Log(2)));
        }
    }
}
