using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SortAlgorithm.Logics
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// sortKind : MergeSort, ArraySize : 100, IndexAccessCount : 517, CompareCount : 548, SwapCount : 610
    /// </remarks>
    /// <typeparam name="T"></typeparam>

    public class MergeSort<T> : SortBase<T> where T : IComparable<T>
    {
        public override T[] Sort(T[] array)
        {
            base.sortStatics.Reset(array.Length);
            var work = new T[(array.Length) / 2];
            Sort(array, 0, array.Length - 1, work);
            return array;
        }

        private T[] Sort(T[] array, int left, int right, T[] work)
        {
            var mid = (left + right) / 2;
            if (left == right) return array;
            base.sortStatics.AddIndexAccess();

            // left : merge + sort
            Sort(array, left, mid, work);
            // right : merge + sort
            Sort(array, mid + 1, right, work);
            // left + right: merge
            Merge(array, left, right, mid, work);
            return array;
        }

        // escape left to work, then merge right, and last for left.
        private T[] Merge(T[] array, int left, int right, int mid, T[] work)
        {
            T max = default(T);
            // if array[2] = x,y. set work[0] = x
            for (var i = left; i <= mid; i++)
            {
                base.sortStatics.AddIndexAccess();
                work[i - left] = array[i];

                // max assign
                if (i - left >= work.Length - 1)
                {
                    max = array.Max();
                    break;
                }
            }

            int l = left;
            int r = mid + 1;

            // merge array-left and work
            while (true)
            {
                var k = l + r - (mid + 1);
                // if left is sorted then merge done.
                if (l > mid) break;

                // if right is sorted, do left(work)
                if (r > right)
                {
                    while (l <= mid)
                    {
                        base.sortStatics.AddIndexAccess();
                        base.sortStatics.AddSwapCount();
                        k = l + r - (mid + 1);
                        array[k] = work[l - left];

                        // max assign on edge case
                        if (l - left >= work.Length - 1)
                        {
                            array[right] = max;
                            break;
                        }
                        l++;
                    }
                    break;
                }

                // sort
                base.sortStatics.AddCompareCount();
                if (work[l - left].CompareTo(array[r]) < 0)
                {
                    base.sortStatics.AddSwapCount();
                    array[k] = work[l - left];
                    l++;
                }
                else
                {
                    base.sortStatics.AddSwapCount();
                    array[k] = array[r];
                    r++;
                }
            }

            return array;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// sortKind : MergeSort2, ArraySize : 100, IndexAccessCount : 771, CompareCount : 543, SwapCount : 672
    /// </remarks>
    /// <typeparam name="T"></typeparam>
    public class MergeSort2<T> : SortBase<T> where T : IComparable<T>
    {
        public override T[] Sort(T[] array)
        {
            if (array.Length <= 1) return array;
            if (sortStatics.ArraySize <= array.Length)
            {
                base.sortStatics.Reset(array.Length);
            }

            base.sortStatics.AddIndexAccess();

            int mid = array.Length / 2;
            var left = array.Take(mid).ToArray();
            var right = array.Skip(mid).ToArray();
            left = Sort(left);
            right = Sort(right);
            var result = Merge(left, right);
            return result;
        }

        private T[] Merge(T[] left, T[] right)
        {
            var result = new T[left.Length + right.Length];
            var i = 0;
            var j = 0;
            var current = 0;
            while (i < left.Length || j < right.Length)
            {
                base.sortStatics.AddIndexAccess();
                if (i < left.Length && j < right.Length)
                {
                    base.sortStatics.AddCompareCount();
                    if (left[i].CompareTo(right[j]) <= 0)
                    {
                        base.sortStatics.AddSwapCount();
                        Swap(ref result[current], ref left[i++]);
                    }
                    else
                    {
                        base.sortStatics.AddSwapCount();
                        Swap(ref result[current], ref right[j++]);
                    }
                }
                else if (i < left.Length)
                {
                    base.sortStatics.AddSwapCount();
                    Swap(ref result[current], ref left[i++]);
                }
                else
                {
                    base.sortStatics.AddSwapCount();
                    Swap(ref result[current], ref right[j++]);
                }
                current++;
            }

            return result;
        }
    }
}
