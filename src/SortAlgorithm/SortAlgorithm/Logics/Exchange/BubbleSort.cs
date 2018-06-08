using System;
using System.Collections.Generic;
using System.Text;

namespace SortAlgorithm.Logics
{
    /// <summary>
    /// 配列の末尾から、n番目の要素をn-1番目の要素と比較して交換を続ける。末尾から1ずつおろしていくことで、毎ループで配列の頭には確定した若い要素が必ず入る。ICompatibleの性質から、n > n-1 = -1 となり、< 0 で元順序を保証しているので安定ソート。
    /// 単純だが低速
    /// </summary>
    /// <remarks>
    /// stable : yes
    /// inplace : yes
    /// Compare : n(n-1) / 2
    /// Swap : Average n(n-1)/4
    /// Order : O(n^2)
    /// ArraySize : 100, IsSorted : True, sortKind : BubbleSort, IndexAccessCount : 4950, CompareCount : 4950, SwapCount : 2491
    /// ArraySize : 1000, IsSorted : True, sortKind : BubbleSort, IndexAccessCount : 499500, CompareCount : 499500, SwapCount : 254017
    /// ArraySize : 10000, IsSorted : True, sortKind : BubbleSort, IndexAccessCount : 49995000, CompareCount : 49995000, SwapCount : 24763456
    /// </remarks>
    /// <typeparam name="T"></typeparam>
    public class BubbleSort<T> : SortBase<T> where T : IComparable<T>
    {
        public override SortType SortType => SortType.Exchange;

        public override T[] Sort(T[] array)
        {
            base.Statics.Reset(array.Length, SortType, nameof(BubbleSort<T>));
            for (var i = 0; i < array.Length; i++)
            {
                for (var j = array.Length - 1; j > i; j--)
                {
                    base.Statics.AddIndexAccess();
                    base.Statics.AddCompareCount();
                    //array.Dump($"{j} : {array[j]}, {j - 1} : {array[j - 1]}, {array[j - 1].CompareTo(array[j]) > 0}");
                    if (array[j].CompareTo(array[j - 1]) < 0)
                    {
                        Swap(ref array[j], ref array[j - 1]);
                    }
                }
            }
            return array;
        }
    }
}
