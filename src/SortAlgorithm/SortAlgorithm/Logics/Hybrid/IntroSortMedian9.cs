using System;
using System.Collections.Generic;
using System.Text;

namespace SortAlgorithm.Logics
{
    /// <summary>
    /// Contains Bug on HeapSort.
    /// 配列から、常に最大の要素をルートにもつ2分木構造(ヒープ)を作る(この時点で不安定)。あとは、ルート要素をソート済み配列の末尾に詰めて、ヒープの末端をルートに持ってきて再度ヒープ構造を作る。これを繰り返すことでヒープの最大値は常にルート要素になり、これをソート済み配列につめていくことで自然とソートができる。
    /// Median3 は山データでエッジケース問題があるため、Median9が望ましい
    /// </summary>
    /// <remarks>
    /// stable : no
    /// inplace : yes
    /// Compare : n log n
    /// Swap : n log n
    /// Order : O(n log n) (Worst case : O(n log n))
    /// </remarks>
    /// <typeparam name="T"></typeparam>
    public class IntroSortMedian9<T> : SortBase<T> where T : IComparable<T>
    {
        public override SortType SortType => SortType.Hybrid;

        // ref : https://www.cs.waikato.ac.nz/~bernhard/317/source/IntroSort.java
        private const int IntroThreshold = 16;
        private HeapSort<T> heapSort = new HeapSort<T>();
        private InsertSort<T> insertSort = new InsertSort<T>();

        public override T[] Sort(T[] array)
        {
            base.Statics.Reset(array.Length, SortType, nameof(IntroSortMedian9<T>));
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
                var partition = Partition(array, left, right, Median9(array, left, right));
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

        private static int FloorLog(int length)
        {
            return (int)(Math.Floor(Math.Log(length) / Math.Log(2)));
        }
    }
}
