using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SortAlgorithm.Logics
{
    /// <summary>
    /// Merge Sortの類似ですが、Swap回数が極度に少ない安定な内部ソート。ベストケースでO(n)、平均でもマージソート同様にO(n log n)と高速に挙動する
    /// </summary>
    /// <remarks>
    /// stable : yes
    /// inplace : yes
    /// Compare : n log2 n
    /// Swap : n log2 n
    /// Order : O(n log n) (Best case : O(n), Worst case : O(n log n))
    /// </remarks>
    /// <typeparam name="T"></typeparam>

    public class ShiftSort<T> : SortBase<T> where T : IComparable<T>
    {
        // refer: https://github.com/JamesQuintero/ShiftSort
        public override SortType SortType => SortType.Merge;

        public override T[] Sort(T[] array)
        {
            base.Statics.Reset(array.Length, SortType, nameof(ShiftSort<T>));

            var zeroIndices = new int[(int)(array.Length / 2) + 2];
            zeroIndices[0] = array.Length;

            var endTracker = 1;

            // check 3items where decrease order like "array[x -2] < array[x - 1] < array[x]"
            for (var x = array.Length - 1; x >= 1; x--)
            {
                base.Statics.AddIndexAccess();
                base.Statics.AddCompareCount();
                if (array[x].CompareTo(array[x - 1]) < 0)
                {
                    base.Statics.AddIndexAccess();
                    base.Statics.AddCompareCount();
                    if (x > 1 && array[x - 1].CompareTo(array[x - 2]) < 0)
                    {
                        // change to increase order
                        Swap(ref array[x], ref array[x - 2]);

                        if (x != array.Length - 1)
                        {
                            base.Statics.AddIndexAccess();
                            base.Statics.AddCompareCount();
                            if (array[x + 1].CompareTo(array[x]) < 0)
                            {
                                base.Statics.AddIndexAccess();
                                zeroIndices[endTracker] = x + 1;
                                endTracker++;
                            }
                        }
                    }
                    else
                    {
                        zeroIndices[endTracker] = x;
                        endTracker++;
                    }

                    // skips
                    x--;
                }
            }

            //merges sorted lists specified by zero_indices
            Split(array, zeroIndices, 0, endTracker);
            return array;
        }

        private void Split(T[] array, int[] zeroIndices, int i, int j)
        {
            // if already 3 indices merge and end.
            // i == j -2
            if ((j - i) == 2)
            {
                base.Statics.AddIndexAccess();
                Merge(array, zeroIndices[j], zeroIndices[j - 1], zeroIndices[i]);
                return;
            }
            else if ((j - i) < 2)
            {
                // indice or less = not enough to merge. end split.
                return;
            }

            var j2 = i + (j - i) / 2;
            var i2 = j2 + 1;

            // split first half
            Split(array, zeroIndices, i, j2);
            // split second half
            Split(array, zeroIndices, i2, j); ;

            // merge first half
            Merge(array, zeroIndices[i2], zeroIndices[j2], zeroIndices[i]);
            // merge second half
            Merge(array, zeroIndices[j], zeroIndices[i2], zeroIndices[i]);
        }

        private void Merge(T[] array, int first, int second, int third)
        {
            if (second - first > third - second)
            {
                // 2nd list
                var tmp2nd = new T[third - second];
                var counter = 0;
                for (var y = second; y < third; y++)
                {
                    base.Statics.AddIndexAccess();
                    tmp2nd[counter] = array[y];
                    counter++;
                }

                // starts from 2nd list length
                var secondCounter = third - second;
                var left = second - 1;
                while (secondCounter > 0)
                {
                    base.Statics.AddIndexAccess();
                    base.Statics.AddCompareCount();
                    if (left >= first && array[left].CompareTo(tmp2nd[secondCounter - 1]) >= 0)
                    {
                        array[left + secondCounter] = array[left];
                        left--;
                    }
                    else
                    {
                        array[left + secondCounter] = tmp2nd[secondCounter - 1];
                        secondCounter--;
                    }
                }
            }
            else
            {
                // 1st list
                var tmp1st = new T[second - first];
                var counter = 0;
                for (var y = first; y < second; y++)
                {
                    base.Statics.AddIndexAccess();
                    tmp1st[counter] = array[y];
                    counter++;
                }

                // starts from 2nd list length
                var firstCounter = 0;
                var tmpLength = second - first;
                var right = second;
                while (firstCounter < tmp1st.Length)
                {
                    base.Statics.AddIndexAccess();
                    base.Statics.AddCompareCount();
                    if (right < third && array[right].CompareTo(tmp1st[firstCounter]) < 0)
                    {
                        array[right - tmpLength] = array[right];
                        right++;
                    }
                    else
                    {
                        array[right - tmpLength] = tmp1st[firstCounter];
                        firstCounter++;
                        tmpLength--;
                    }
                }
            }
        }
    }
}
