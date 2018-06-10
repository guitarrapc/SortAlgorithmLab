using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SortAlgorithm.Logics
{
    /// <summary>
    /// 必要となるバケットをはじめに用意して、配列の各要素をバケットに詰める。最後にバケットの中身を結合すればソートが済んでいる。比較を一切しない安定な外部ソート。
    /// 問題は、桁の数だけバケットの数が膨大になること
    /// </summary>
    /// <remarks>
    /// stable : yes
    /// inplace : no
    /// Compare : 2n
    /// Swap : 0
    /// Order : O(n) (Worst case : O(n^2))
    /// </remarks>
    /// <typeparam name="T"></typeparam>

    public class BucketSortT<T>
    {
        public SortType SortType => SortType.Distributed;

        public IStatics Statics => statics;
        protected IStatics statics = new SortStatics();

        public T[] Sort(T[] array, Func<T, int> getKey)
        {
            Statics.Reset(array.Length, SortType, nameof(BucketSortT<T>));
            var size = array.Select(x => getKey(x)).Max() + 1;

            // 0 position
            var offset = 0;
            var min = array.Select(x => getKey(x)).Min();

            // incase lower than 0
            if (min < 0)
            {
                offset = Math.Abs(min);
                size = array.Select(x => getKey(x)).Max() - min + 1;
            }

            var bucket = new List<T>[size];
            var keys = array.Select(x => getKey(x)).ToArray();

            foreach (var item in array)
            {
                statics.AddIndexAccess();
                statics.AddCompareCount();
                var key = getKey(item) + offset;
                if (bucket[key] == null)
                {
                    bucket[key] = new List<T>();
                }
                bucket[key].Add(item);
            }

            for (int j = 0, i = 0; j < bucket.Length; ++j)
            {
                if (bucket[j] != null)
                {
                    foreach (var item in bucket[j])
                    {
                        statics.AddIndexAccess();
                        array[i++] = item;
                    }
                }
                else
                {
                    statics.AddIndexAccess();
                }
            }

            return array;
        }
    }

    /// <summary>
    /// 整数専用のバケットソート
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <typeparam name="T"></typeparam>
    public class BucketSort<T> : SortBase<T> where T : IComparable<T>
    {
        public override SortType SortType => SortType.Distributed;

        public int[] Sort(int[] array)
        {
            base.Statics.Reset(array.Length, SortType, nameof(BucketSort<T>));
            var size = array.Max();

            // 0 position
            var offset = 0;
            var min = array.Min();

            // incase lower than 0
            if (min < 0)
            {
                offset = Math.Abs(min);
                size = array.Max() - min;
            }

            // make bucket for possibly assigned number of int
            var bucket = new int[size + 1];
            for (var i = 0; i < array.Length; i++)
            {
                base.Statics.AddIndexAccess();
                base.Statics.AddCompareCount();
                bucket[array[i] + offset]++;
            }

            // put array int to each bucket.
            for (int j = 0, i = 0; j < bucket.Length; ++j)
            {
                for (var k = bucket[j]; k != 0; --k, ++i)
                {
                    base.Statics.AddIndexAccess();
                    array[i] = j - offset;
                }
            }

            return array;
        }
    }
}
