using System;
using System.Collections.Generic;
using System.Text;

namespace SortAlgorithm
{
    public class BubbleSort<T> : SortBase<T> where T : IComparable<T>
    {
        public override T[] Sort(T[] array)
        {
            base.sortStatics = new SortStatics(array.Length);
            for (var i = 0; i < array.Length; i++)
            {
                for (var j = array.Length - 1; j > i; j--)
                {
                    base.sortStatics.AddIndexAccess();
                    if (array[j].CompareTo(array[j - 1]) < 0)
                    {
                        base.sortStatics.AddSwapCount();
                        Swap(ref array[j], ref array[j - 1]);
                    }
                }
            }
            return array;
        }
    }
}
