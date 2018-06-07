using System;
using System.Collections.Generic;
using System.Text;

namespace SortAlgorithm
{
    public class SortBase<T> : ISort<T> where T : IComparable<T>
    {
        public SortStatics Statics => statics;
        protected SortStatics statics = new SortStatics();

        public virtual T[] Sort(T[] array)
        {
            throw new NotImplementedException();
        }

        public void Swap(ref T a, ref T b)
        {
            Statics.AddSwapCount();
            var tmp = a;
            a = b;
            b = tmp;
        }
    }
}
