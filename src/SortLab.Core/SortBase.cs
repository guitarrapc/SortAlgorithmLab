using System;

namespace SortLab.Core;

public class SortBase<T> : ISort<T> where T : IComparable<T>
{
    public IStatistics Statistics => statistics;
    protected IStatistics statistics = new SortStatistics();

    public virtual SortType SortType => SortType.None;

    public virtual T[] Sort(T[] array)
    {
        throw new NotImplementedException();
    }

    protected void Swap(ref T a, ref T b)
    {
        Statistics.AddSwapCount();
        var tmp = a;
        a = b;
        b = tmp;
    }

    protected int UnsignedRightShift(int number, int bits)
    {
        return (int)((uint)number >> bits);
    }
}
