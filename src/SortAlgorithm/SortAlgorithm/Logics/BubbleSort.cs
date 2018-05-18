using System;
using System.Collections.Generic;
using System.Text;

namespace SortAlgorithm.Logics
{
    /// <summary>
    /// 配列の末尾から、n番目の要素をn-1番目の要素と比較して交換を続ける。末尾から1ずつおろしていくことで、毎ループで配列の頭には確定した若い要素が必ず入る。ICompatibleの性質から、n > n-1 = -1 となり、< 0 で元順序を保証しているので、安定ソートとなる。
    /// </summary>
    /// <remarks>
    /// sortKind : BubbleSort, ArraySize : 100, IndexAccessCount : 4950, CompareCount : 2170, SwapCount : 2170
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
                    if (array[j].CompareTo(array[j - 1]) < 0)
                    {
                        base.sortStatics.AddCompareCount();
                        base.sortStatics.AddSwapCount();
                        Swap(ref array[j], ref array[j - 1]);
                    }
                }
            }
            return array;
        }
    }
}
