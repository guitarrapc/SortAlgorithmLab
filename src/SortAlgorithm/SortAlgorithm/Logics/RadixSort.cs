using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SortAlgorithm.Logics
{
    /// <summary>
    /// 基数の数だけバケットを用意して、基数の桁ごとにバケットソートを行うことで、バケットソートにあった値の範囲制限をゆるくしたもの。
    /// これにより、桁ごとにバケットを用意すれば良くなり対象が楽になる。
    /// </summary>
    /// <remarks>
    /// stable : yes
    /// inplace : no
    /// Compare : 2kn
    /// Swap : 0
    /// Order : O(kn)
    /// sortKind : Radix4Sort, ArraySize : 100, IndexAccessCount : 2848, CompareCount : 400, SwapCount : 0
    /// </remarks>
    /// <typeparam name="T"></typeparam>

    public class Radix4Sort<T> : SortBase<T> where T : IComparable<T>
    {
        public int[] Sort(int[] array)
        {
            base.sortStatics.Reset(array.Length);
            var bucket = new List<int>[256];
            var digit = 4;

            for (int d = 0, logR = 0; d < digit; ++d, logR += 8)
            {
                // make bucket for possibly assigned number of int
                for (var i = 0; i < array.Length; i++)
                {
                    base.sortStatics.AddIndexAccess();
                    base.sortStatics.AddCompareCount();
                    // pick 256 radix d's digit number
                    var key = (array[i] >> logR) & 255;
                    if (bucket[key] == null) bucket[key] = new List<int>();
                    bucket[key].Add(array[i]);
                }

                // put array int to each bucket.
                for (int j = 0, i = 0; j < bucket.Length; ++j)
                {
                    sortStatics.AddIndexAccess();
                    if (bucket[j] != null)
                    {
                        foreach (var item in bucket[j])
                        {
                            sortStatics.AddIndexAccess();
                            array[i++] = item;
                        }
                    }
                }

                for (var j = 0; j < bucket.Length; ++j)
                {
                    sortStatics.AddIndexAccess();
                    bucket[j] = null;
                }
            }

            return array;
        }
    }

    /// <summary>
    /// </summary>
    /// <remarks>
    /// sortKind : Radix10Sort, ArraySize : 100, IndexAccessCount : 440, CompareCount : 200, SwapCount : 0
    /// </remarks>
    /// <typeparam name="T"></typeparam>
    public class Radix10Sort<T> : SortBase<T> where T : IComparable<T>
    {
        public int[] Sort(int[] array)
        {
            base.sortStatics.Reset(array.Length);
            var digit = 1 + (int)array.Max(x => Math.Log10(x));

            var bucket = new List<int>[10];

            for (int d = 0, r = 1; d < digit; ++d, r *= 10)
            {
                // make bucket for possibly assigned number of int
                for (var i = 0; i < array.Length; i++)
                {
                    base.sortStatics.AddIndexAccess();
                    base.sortStatics.AddCompareCount();
                    var key = (array[i] / r) % 10;
                    if (bucket[key] == null) bucket[key] = new List<int>();
                    bucket[key].Add(array[i]);
                }

                // put array int to each bucket.
                for (int j = 0, i = 0; j < bucket.Length; ++j)
                {
                    sortStatics.AddIndexAccess();
                    if (bucket[j] != null)
                    {
                        foreach (var item in bucket[j])
                        {
                            sortStatics.AddIndexAccess();
                            array[i++] = item;
                        }
                    }
                }

                for (var j = 0; j < bucket.Length; ++j)
                {
                    sortStatics.AddIndexAccess();
                    bucket[j] = null;
                }
            }

            return array;
        }
    }
}
