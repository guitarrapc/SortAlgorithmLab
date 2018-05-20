using System;
using System.Collections.Generic;
using System.Text;

namespace SortAlgorithm.Logics
{
    /// <summary>
    /// 配列の末尾から、n番目の要素をn-1番目の要素と比較して交換を続ける。末尾から1ずつおろしていくことで、毎ループで配列の頭には確定した若い要素が必ず入る。ICompatibleの性質から、n > n-1 = -1 となり、< 0 で元順序を保証しているので、安定ソートとなる。
    /// 単純だが低速
    /// </summary>
    /// <remarks>
    /// stable : yes
    /// inplace : yes
    /// Compare : n(n-1) / 2
    /// Swap : O(n^2) (Average n(n-1)/4)
    /// sortKind : BubbleSort, ArraySize : 100, IndexAccessCount : 4950, CompareCount : 4950, SwapCount : 2278
    /// </remarks>
    /// <typeparam name="T"></typeparam>
    public class BubbleSort<T> : SortBase<T> where T : IComparable<T>
    {
        public override T[] Sort(T[] array)
        {
            base.sortStatics.Reset(array.Length);
            for (var i = 0; i < array.Length; i++)
            {
                for (var j = array.Length - 1; j > i; j--)
                {
                    base.sortStatics.AddIndexAccess();
                    base.sortStatics.AddCompareCount();
                    //array.Dump($"{j} : {array[j]}, {j - 1} : {array[j - 1]}, {array[j - 1].CompareTo(array[j]) < 0}");
                    if (array[j].CompareTo(array[j - 1]) < 0)
                    {
                        base.sortStatics.AddSwapCount();
                        Swap(ref array[j], ref array[j - 1]);
                    }
                }
            }
            return array;
        }
    }
}
