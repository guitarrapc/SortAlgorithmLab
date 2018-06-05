using System;
using System.Collections.Generic;
using System.Text;

namespace SortAlgorithm.Logics
{
    /// <summary>
    /// ソート済みとなる先頭に並ぶ配列がある。ソート済み配列の末尾から未ソートの配列の後ろに進み、各要素と比較して値が小さい限りソート済み配列と入れ替える。(つまり新しい要素の値が小さい限り前にいく)。ICompatibleの性質から、n > n-1 = -1 となり、> 0 で元順序を保証しているので安定ソート。
    /// ソート済み配列には早いが、Reverse配列には遅い
    /// </summary>
    /// <remarks>
    /// stable : yes
    /// inplace : yes
    /// Compare : n(n-1) / 2
    /// Swap : n^2/2
    /// Order : O(n^2) (Better case : O(n)) (Worst case : O(n^2))
    /// sortKind : InsertSort, ArraySize : 100, IndexAccessCount : 2529, CompareCount : 2529, SwapCount : 94
    /// </remarks>
    /// <typeparam name="T"></typeparam>
    public class InsertSort<T> : SortBase<T> where T : IComparable<T>
    {
        public override T[] Sort(T[] array)
        {
            base.sortStatics.Reset(array.Length);
            for (var i = 1; i < array.Length; i++)
            {
                var tmp = array[i];
                for (var j = i; j >= 1 && array[j - 1].CompareTo(array[j]) > 0; --j)
                {
                    base.sortStatics.AddIndexAccess();
                    base.sortStatics.AddCompareCount();
                    //array.Dump($"{j - 1} : {array[j - 1]}, {j} : {array[j]}, {array[j - 1].CompareTo(array[j]) > 0}");
                    if (array[j - 1].CompareTo(array[j]) > 0)
                    {
                        base.sortStatics.AddSwapCount();
                        Swap(ref array[j], ref array[j - 1]);
                    }
                }
            }
            return array;
        }

        public T[] Sort(T[] array, int first, int last)
        {
            base.sortStatics.Reset(array.Length);
            for (var i = first + 1; i < last; i++)
            {
                base.sortStatics.AddCompareCount();
                for (var j = i; j > first && array[j - 1].CompareTo(array[j]) > 0; --j)
                {
                    base.sortStatics.AddIndexAccess();
                    base.sortStatics.AddSwapCount();
                    Swap(ref array[j], ref array[j - 1]);
                }
            }
            return array;
        }
    }
}
