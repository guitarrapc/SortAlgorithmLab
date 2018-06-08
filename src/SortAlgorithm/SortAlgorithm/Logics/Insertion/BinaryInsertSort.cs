using System;
using System.Collections.Generic;
using System.Text;

namespace SortAlgorithm.Logics
{
    /// <summary>
    /// 通常のリニアサーチと異なり、挿入一を2分探索して決定するため比較範囲、回数を改善できる。安定ソート
    /// ソート済み配列には早いが、Reverse配列には遅い
    /// </summary>
    /// <remarks>
    /// stable : yes
    /// inplace : yes
    /// Compare : log n
    /// Swap : n^2/2
    /// Order : O(n^2) (Better case : O(n)) (Worst case : O(n^2))
    /// ArraySize : 100, IsSorted : True, sortKind : BinaryInsertSort, IndexAccessCount : 3226, CompareCount : 537, SwapCount : 2590
    /// ArraySize : 1000, IsSorted : True, sortKind : BinaryInsertSort, IndexAccessCount : 264595, CompareCount : 8580, SwapCount : 255016
    /// ArraySize : 10000, IsSorted : True, sortKind : BinaryInsertSort, IndexAccessCount : 24902406, CompareCount : 118952, SwapCount : 24773455
    /// </remarks>
    /// <typeparam name="T"></typeparam>
    public class BinaryInsertSort<T> : SortBase<T> where T : IComparable<T>
    {
        public override SortType SortType => SortType.Insertion;

        public override T[] Sort(T[] array)
        {
            base.Statics.Reset(array.Length);
            for (var i = 1; i < array.Length; i++)
            {
                base.Statics.AddIndexAccess();
                var tmp = array[i];

                // C# Managed Code BinarySearch + Swap
                // var j = Array.BinarySearch(array, 0, i, tmp);
                // Array.Copy(array, j, array, j+1, i-j);
                // Swap(ref array[j], ref tmp);

                // Hand BinarySearch
                var left = BinarySearch(ref array, tmp, i);
                for (var j = left; j <= i; j++)
                {
                    base.Statics.AddIndexAccess();
                    Swap(ref array[j], ref tmp);
                }
            }
            return array;
        }

        public T[] Sort(T[] array, int first, int last)
        {
            base.Statics.Reset(array.Length);
            for (var i = first + 1; i < last; i++)
            {
                base.Statics.AddIndexAccess();
                var tmp = array[i];

                // Hand BinarySearch
                var left = BinarySearch(ref array, tmp, i);
                for (var j = left; j <= i; j++)
                {
                    base.Statics.AddIndexAccess();
                    Swap(ref array[j], ref tmp);
                }
            }
            return array;
        }

        private int BinarySearch(ref T[] array, T tmp, int index)
        {
            var left = 0;
            var right = index;
            while (left < right)
            {
                base.Statics.AddIndexAccess();
                base.Statics.AddCompareCount();
                var mid = (left + right) / 2;
                if (array[mid].CompareTo(tmp) <= 0)
                {
                    left = mid + 1;
                }
                else
                {
                    right = mid;
                }
            }
            return left;
        }
    }
}
