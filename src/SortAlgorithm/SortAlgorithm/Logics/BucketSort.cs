using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SortAlgorithm.Logics
{
    /// <summary>
    /// 必要となるバケットをはじめに用意して、配列の各要素をバケットに詰める。最後にバケットの中身を結合すればソートが済んでいる。比較を一切しない安定な外部ソート。
    /// </summary>
    /// <remarks>
    /// stable : yes
    /// inplace : no
    /// Compare : 2n
    /// Swap : 0
    /// Order : O(n)
    /// sortKind : BucketSortT, ArraySize : 100, IndexAccessCount : 299, CompareCount : 100, SwapCount : 0
    /// </remarks>
    /// <typeparam name="T"></typeparam>

    public class BucketSortT<T>
    {
        public SortStatics SortStatics => sortStatics;
        protected SortStatics sortStatics = new SortStatics();

        public T[] Sort(T[] array, Func<T, int> getKey, int max)
        {
            sortStatics.Reset(array.Length);
            var bucket = new List<T>[max + 1];

            foreach (var item in array)
            {
                sortStatics.AddIndexAccess();
                sortStatics.AddCompareCount();
                var key = getKey(item);
                if (bucket[key] == null)
                {
                    bucket[key] = new List<T>();
                }
                bucket[key].Add(item);
            }

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

            return array;
        }
    }

    /// <summary>
    /// 整数専用のバケットソート
    /// </summary>
    /// <remarks>
    /// sortKind : BucketSort, ArraySize : 100, IndexAccessCount : 200, CompareCount : 100, SwapCount : 0
    /// </remarks>
    /// <typeparam name="T"></typeparam>
    public class BucketSort<T> : SortBase<T> where T : IComparable<T>
    {
        public int[] Sort(int[] array)
        {
            base.sortStatics.Reset(array.Length);
            var max = array.Max();

            // make bucket for possibly assigned number of int
            var bucket = new int[max + 1];
            for (var i = 0; i < array.Length; i++)
            {
                base.sortStatics.AddIndexAccess();
                base.sortStatics.AddCompareCount();
                bucket[array[i]]++;
            }

            // put array int to each bucket.
            for (int j = 0, i = 0; j < bucket.Length; ++j)
            {
                for (var k = bucket[j]; k != 0; --k, ++i)
                {
                    base.sortStatics.AddIndexAccess();
                    array[i] = j;
                }
            }

            return array;
        }
    }
}
