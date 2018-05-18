using System;
using System.Collections.Generic;
using System.Text;

namespace SortAlgorithm.Logics
{
    /// <summary>
    /// 配列にアクセスして、常に最小を末尾まで走査。最小を現在のインデックスであるn番目の要素（ソート済みの要素の末尾）と交換を続ける。ICompatibleの性質から、n > n-1 = -1 となり、> 0 で元順序を保証しているので、安定ソートとなる。
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SelectionSort<T> : SortBase<T> where T : IComparable<T>
    {
        public override T[] Sort(T[] array)
        {
            base.sortStatics.Reset(array.Length);
            for (var i = 0; i < array.Length; i++)
            {
                var min = i;
                for (var j = i + 1; j < array.Length; j++)
                {
                    base.sortStatics.AddIndexAccess();
                    if (array[min].CompareTo(array[j]) > 0)
                    {
                        base.sortStatics.AddCompareCount();
                        min = j;
                    }
                }
                base.sortStatics.AddSwapCount();
                Swap(ref array[min], ref array[i]);
            }
            return array;
        }
    }
}
