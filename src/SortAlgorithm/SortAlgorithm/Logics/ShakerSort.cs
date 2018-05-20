using System;
using System.Collections.Generic;
using System.Text;

namespace SortAlgorithm.Logics
{
    /// <summary>
    /// 配列の先頭から、n番目の要素をn+1番目の要素と比較して交換を続ける。末尾から1ずつ増やしていくことで、毎ループで配列の末尾には確定した若い要素が必ず入る。次の配列の末尾から、n-1番目の要素と比較して交換を続ける。末尾から1ずつおろしていくことで、毎ループで配列の頭には確定した若い要素が必ず入る。ICompatibleの性質から、n > n-1 = -1 となり、< 0 で元順序を保証しているので、安定ソートとなる。
    /// 両方向のバブルソートで、ほとんど整列済みのデータに対してはバブルソートより高速になる。
    /// </summary>
    /// <remarks>
    /// stable : yes
    /// inplace : yes
    /// Compare : n(n-1) / 2
    /// Swap : O(n^2) (Average n(n-1)/4)
    /// sortKind : ShakerSort, ArraySize : 100, IndexAccessCount : 3376, CompareCount : 3376, SwapCount : 2519
    /// </remarks>
    /// <typeparam name="T"></typeparam>
    public class ShakerSort<T> : SortBase<T> where T : IComparable<T>
    {
        public override T[] Sort(T[] array)
        {
            base.sortStatics.Reset(array.Length);
            var min = 0;
            var max = array.Length - 1;

            // Is "while true" required? Isn't min != max enough?
            //while(true)
            while (min != max)
            {
                // 順方向スキャン
                var lastSwapIndex = min;
                for (var i = min; i < max; i++)
                {
                    base.sortStatics.AddIndexAccess();
                    base.sortStatics.AddCompareCount();
                    if (array[i].CompareTo(array[i + 1]) > 0)
                    {
                        //array.Dump($"min -> {i} : {array[i]}, {i + 1} : {array[i + 1]}");
                        base.sortStatics.AddSwapCount();
                        Swap(ref array[i], ref array[i + 1]);
                        lastSwapIndex = i;
                    }
                }

                max = lastSwapIndex;
                if (min == max) break;

                // 逆方向スキャン
                lastSwapIndex = max;
                for (var i = max; i > min; i--)
                {
                    base.sortStatics.AddIndexAccess();
                    base.sortStatics.AddCompareCount();
                    if (array[i].CompareTo(array[i - 1]) < 0)
                    {
                        //array.Dump($"max -> {i} : {array[i]}, {i + 1} : {array[i + 1]}");
                        base.sortStatics.AddSwapCount();
                        Swap(ref array[i], ref array[i - 1]);
                        lastSwapIndex = i;
                    }
                }
                min = lastSwapIndex;
                if (min == max) break;
            }

            return array;
        }
    }
}
