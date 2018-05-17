using System;
using System.Collections.Generic;
using System.Text;

namespace SortAlgorithm
{
    public class SortBase<T> : ISort<T> where T : IComparable<T>
    {
        public SortStatics SortStatics => sortStatics;
        protected SortStatics sortStatics = null;

        public virtual T[] Sort(T[] array)
        {
            throw new NotImplementedException();
        }

        public static void Swap(ref T a, ref T b)
        {
            var tmp = a;
            a = b;
            b = tmp;
        }
    }
}
