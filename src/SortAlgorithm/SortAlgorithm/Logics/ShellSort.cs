using System;
using System.Collections.Generic;
using System.Text;

namespace SortAlgorithm.Logics
{
    /// <summary>
    /// hi+1 = 3hi + 1となるhで配列を分割し、分割された細かい配列ごとに挿入ソート<see cref="InsertSort{T}"/>を行う(A)。次のhを/3で求めて、Aを繰り返しh=1まで行う。hごとにでソート済みとなっているため、最後の1は通常の挿入ソートと同じだが、挿入ソートが持つソート済み配列で高速に動作する性質から高速な並び替えが可能になる。選択ソートを使っているので不安定ソート。
    /// </summary>
    /// <remarks>
    /// stable : no
    /// inplace : yes
    /// Compare : ((n-1)(n/2)) / 2
    /// Swap : O(n^2) (Average n(n-1)/4)
    /// sortKind : ShellSort, ArraySize : 100, IndexAccessCount : 2433, CompareCount : 2433, SwapCount : 172
    /// </remarks>
    /// <typeparam name="T"></typeparam>
    public class ShellSort<T> : SortBase<T> where T : IComparable<T>
    {
        public override T[] Sort(T[] array)
        {
            base.sortStatics.Reset(array.Length);

            // calculate h
            // most efficient h will be : h(i + 1) = 3h(i) + 1
            var h = 0;
            for (h = 1; h < array.Length / 9; h = h * 3 + 1) ;

            // try next h with / 3....
            for (; h > 0; h /= 3)
            {
                //h.Dump(array.Length.ToString());
                // Same as InsertSort (1 will be h, and > 0 will be >= h)
                for (var i = h; i < array.Length; i++)
                {
                    var tmp = array[i];
                    base.sortStatics.AddIndexAccess();
                    base.sortStatics.AddCompareCount();
                    if (array[i - h].CompareTo(tmp) > 0)
                    {
                        var j = i;
                        do
                        {
                            base.sortStatics.AddIndexAccess();
                            base.sortStatics.AddCompareCount();
                            //array.Dump($"[{tmp}] -> {j}, {j - h} : {array[j - h]}, {array[j - h].CompareTo(tmp) > 0}");
                            array[j] = array[j - h];
                            j--;
                        } while (j >= h && array[j - h].CompareTo(tmp) > 0);
                        base.sortStatics.AddSwapCount();
                        Swap(ref array[j], ref tmp);
                    }
                }
            }

            return array;
        }
    }
}
