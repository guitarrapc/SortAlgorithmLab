using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SortAlgorithm.Logics
{
    /// <summary>
    /// 2^8のバケット対応基数ソート。基数の数だけバケットを用意して、基数の桁ごとにバケットソートを行うことで、バケットソートにあった値の範囲制限をゆるくしたもの。
    /// これにより、桁ごとにバケットを用意すれば良くなり対象が楽になる。
    /// </summary>
    /// <remarks>
    /// stable : yes
    /// inplace : no
    /// Compare : 2kn
    /// Swap : 0
    /// Order : O(kn)
    /// </remarks>
    /// <typeparam name="T"></typeparam>
    public class RadixLSD4Sort<T> : SortBase<T> where T : IComparable<T>
    {
        public override SortType SortType => SortType.Distributed;

        public int[] Sort(int[] array)
        {
            base.Statics.Reset(array.Length, SortType, nameof(RadixLSD4Sort<T>));
            if (array.Min() >= 0)
            {
                return SortImplPositive(array);
            }
            else
            {
                return SortImplNegative(array);
            }
        }

        private int[] SortImplPositive(int[] array)
        {
            var bucket = new List<int>[256];
            var digit = 4;

            for (int d = 0, logR = 0; d < digit; ++d, logR += 8)
            {
                // make bucket for possibly assigned number of int
                for (var i = 0; i < array.Length; i++)
                {
                    base.Statics.AddIndexAccess();
                    // pick 256 radix d's digit number
                    var key = (array[i] >> logR) & 255;
                    if (bucket[key] == null) bucket[key] = new List<int>();
                    bucket[key].Add(array[i]);
                }

                // put array int to each bucket.
                for (int j = 0, i = 0; j < bucket.Length; ++j)
                {
                    if (bucket[j] != null)
                    {
                        foreach (var item in bucket[j])
                        {
                            base.Statics.AddIndexAccess();
                            array[i++] = item;
                        }
                    }
                    else
                    {
                        base.Statics.AddIndexAccess();
                    }
                }

                for (var j = 0; j < bucket.Length; ++j)
                {
                    base.Statics.AddIndexAccess();
                    bucket[j] = null;
                }
            }

            return array;
        }

        private int[] SortImplNegative(int[] array)
        {
            var positiveBucket = new List<int>[256];
            var negativeBucket = new List<int>[256];
            var digit = 4;

            for (int d = 0, logR = 0; d < digit; ++d, logR += 8)
            {
                var offset = array.Length;
                // make bucket for possibly assigned number of int
                for (var i = 0; i < array.Length; i++)
                {
                    base.Statics.AddCompareCount();
                    if (array[i].CompareTo(0) >= 0)
                    {
                        base.Statics.AddIndexAccess();
                        // pick 256 radix d's digit number
                        var key = (array[i] >> logR) & 255;
                        if (positiveBucket[key] == null) positiveBucket[key] = new List<int>();
                        positiveBucket[key].Add(array[i]);
                        offset--;
                    }
                    else
                    {
                        base.Statics.AddIndexAccess();
                        // pick 256 radix d's digit number
                        var key = (array[i] >> logR) & 255;
                        if (negativeBucket[key] == null) negativeBucket[key] = new List<int>();
                        negativeBucket[key].Add(array[i]);
                    }
                }

                // put array int to each bucket.
                // negative bucket
                for (int j = 0, i = 0; j < negativeBucket.Length; ++j)
                {
                    if (negativeBucket[j] != null)
                    {
                        foreach (var item in negativeBucket[j])
                        {
                            base.Statics.AddIndexAccess();
                            array[i++] = item;
                        }
                    }
                    else
                    {
                        base.Statics.AddIndexAccess();
                    }
                }
                // positive bucket
                for (int j = 0, i = offset; j < positiveBucket.Length; ++j)
                {
                    if (positiveBucket[j] != null)
                    {
                        foreach (var item in positiveBucket[j])
                        {
                            base.Statics.AddIndexAccess();
                            array[i++] = item;
                        }
                    }
                    else
                    {
                        base.Statics.AddIndexAccess();
                    }
                }

                for (var j = 0; j < positiveBucket.Length; ++j)
                {
                    base.Statics.AddIndexAccess();
                    positiveBucket[j] = null;
                    negativeBucket[j] = null;
                }
            }

            return array;
        }

        private int GetValue(int key, int i)
        {
            return (UnsignedRightShift(key, (8 * i))) & 255;
        }
    }
}
