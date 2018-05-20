using System;
using System.Collections.Generic;
using System.Text;

namespace SortAlgorithm.Logics
{
    /// <summary>
    /// 開始と終わりから中央値を導いて、この3点を枢軸として配列を左右で分ける。左右で中央値よりも小さい(大きい)データに置き換えてデータを分割する(この時点で不安定)。最後に左右それぞれをソートすることで計算量をソート済みに抑えることができる。不安定なソート。
    /// </summary>
    /// <remarks>
    /// stable : no
    /// inplace : yes
    /// Compare : 
    /// Swap : 
    /// Order : O(n log n) (Worst case : O(nlog^2n))
    /// sortKind : QuickSort, ArraySize : 100, IndexAccessCount : 265, CompareCount : 265, SwapCount : 172
    /// </remarks>
    /// <typeparam name="T"></typeparam>
    public class QuickSort<T> : SortBase<T> where T : IComparable<T>
    {
        public override T[] Sort(T[] array)
        {
            base.sortStatics.Reset(array.Length);
            return Sort(array, 0, array.Length - 1);
        }

        public T[] Sort(T[] array, int first, int last)
        {
            if (first < last)
            {
                var pivot = Median(array[first], array[(first + (last - first)) / 2], array[last]);
                var l = first;
                var r = last;

                while (true)
                {
                    base.sortStatics.AddIndexAccess();
                    base.sortStatics.AddCompareCount();
                    while (l < last && array[l].CompareTo(pivot) < 0) l++;
                    while (r > first && array[r].CompareTo(pivot) > 0) r--;
                    if (l >= r) break;
                    base.sortStatics.AddSwapCount();
                    Swap(ref array[l], ref array[r]);
                    l++;
                    r--;
                }

                Sort(array, first, l - 1);
                Sort(array, r + 1, last);
            }
            return array;
        }

        private T Median(T a, T b, T c)
        {
            if (a.CompareTo(b) > 0) Swap(ref a, ref b);
            if (a.CompareTo(c) > 0) Swap(ref a, ref c);
            if (b.CompareTo(c) > 0) Swap(ref b, ref c);
            return b;
        }
    }
}
