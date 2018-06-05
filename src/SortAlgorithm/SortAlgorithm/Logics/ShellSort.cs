using System;
using System.Collections.Generic;
using System.Text;

namespace SortAlgorithm.Logics
{
    /// <summary>
    /// hi+1 = 3hi + 1となるhで配列を分割し、分割された細かい配列ごとに挿入ソート<see cref="InsertSort{T}"/>を行う(A)。次のhを/3で求めて、Aを繰り返しh=1まで行う。hごとにでソート済みとなっているため、最後の1は通常の挿入ソートと同じだが、挿入ソートが持つソート済み配列で高速に動作する性質から高速な並び替えが可能になる。選択ソートを使っているので不安定ソート。<see cref="BubbleSort{T}"/>に同様の概念を適用したのが<see cref="CombSort{T}"/>である。
    /// </summary>
    /// <remarks>
    /// stable : no
    /// inplace : yes
    /// Compare : O(nlogn) * O(n) (O(n^0.25)～O(n^0.5) * O(n) = O(n^1.25))
    /// Swap : 
    /// Order : O(n^1.25) (Better case : O(n)) (Worst case : O(nlog^2n))
    /// sortKind : ShellSort, ArraySize : 100, IndexAccessCount : 2433, CompareCount : 2433, SwapCount : 172
    /// </remarks>
    /// <typeparam name="T"></typeparam>
    public class ShellSort<T> : SortBase<T> where T : IComparable<T>
    {
        public override T[] Sort(T[] array)
        {
            base.sortStatics.Reset(array.Length);

            // calculate h
            // most efficient h will be : h(i + 1) = 3h(i) + 1 = O(nlog^2n) = 1,4,13,40,121,364,1093
            var h = 0;
            for (h = 1; h < array.Length / 9; h = h * 3 + 1) ;

            // try next h with / 3....
            for (; h > 0; h /= 3)
            {
                //h.Dump(array.Length.ToString());
                // Same as InsertSort (1 will be h, and > 0 will be >= h)
                //h.Dump(array.Length.ToString());
                // Same as InsertSort (1 will be h)
                for (var i = h; i < array.Length; i++)
                {
                    base.sortStatics.AddCompareCount();
                    for (int j = i; j >= h && array[j - h].CompareTo(array[j]) > 0; j -= h)
                    {
                        base.sortStatics.AddIndexAccess();
                        base.sortStatics.AddSwapCount();
                        Swap(ref array[j], ref array[j - h]);
                    }
                }
            }

            return array;
        }
    }
}
