using System;
using System.Collections.Generic;
using System.Text;

namespace SortAlgorithm
{
    public class SortBase<T> : ISort<T> where T : IComparable<T>
    {
        public IStatics Statics => statics;
        protected IStatics statics = new SortStatics();

        public virtual SortType SortType => SortType.None;

        public virtual T[] Sort(T[] array)
        {
            throw new NotImplementedException();
        }

        protected void Swap(ref T a, ref T b)
        {
            Statics.AddSwapCount();
            var tmp = a;
            a = b;
            b = tmp;
        }

        protected int UnsignedRightShift(int number, int bits)
        {
            return (int)((uint)number >> bits);
        }
    }
}
