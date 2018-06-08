using System;
using System.Collections.Generic;
using System.Text;

namespace SortAlgorithm.Logics
{
    /// <summary>
    /// 配列にアクセスして、常に最小を末尾まで走査。最小を現在のインデックスであるn番目の要素（ソート済みの要素の末尾）と交換を続ける。値をインデックスベースで入れ替えてしまうため不安定ソート。
    /// 交換回数が一定して少ないので、比較が軽くて交換が重い場合に有効
    /// </summary>
    /// <remarks>
    /// stable : no
    /// inplace : yes
    /// Compare : n(n-1) / 2
    /// Swap : n-1
    /// Order : O(n^2)
    /// ArraySize : 100, IsSorted : True, sortKind : SelectionSort, IndexAccessCount : 4950, CompareCount : 4950, SwapCount : 100
    /// ArraySize : 1000, IsSorted : True, sortKind : SelectionSort, IndexAccessCount : 499500, CompareCount : 499500, SwapCount : 1000
    /// ArraySize : 10000, IsSorted : True, sortKind : SelectionSort, IndexAccessCount : 49995000, CompareCount : 49995000, SwapCount : 10000
    /// </remarks>
    /// <typeparam name="T"></typeparam>
    public class SelectionSort<T> : SortBase<T> where T : IComparable<T>
    {
    public override SortType SortType => SortType.Selection;

        public override T[] Sort(T[] array)
        {
            base.Statics.Reset(array.Length, SortType, nameof(SelectionSort<T>));
            for (var i = 0; i < array.Length; i++)
            {
                var min = i;
                for (var j = i + 1; j < array.Length; j++)
                {
                    base.Statics.AddIndexAccess();
                    base.Statics.AddCompareCount();
                    //array.Dump($"{min} : {array[min]}, {j} : {array[j]}, {array[min].CompareTo(array[j]) > 0}");
                    if (array[min].CompareTo(array[j]) > 0)
                    {
                        min = j;
                    }
                }
                Swap(ref array[min], ref array[i]);
            }
            return array;
        }
    }
}
