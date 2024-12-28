﻿namespace SortLab.Core.Sortings;

public abstract class SortBase<T> : ISort<T> where T : IComparable<T>
{
    public IStatistics Statistics => statistics;
    private IStatistics statistics = new SortStatistics();

    public virtual SortType SortType => SortType.None;

    public abstract T[] Sort(T[] array);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected int Compare(T x, T y)
    {
#if DEBUG
        Statistics.AddCompareCount();
#endif
        return x.CompareTo(y);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected void Swap(ref T a, ref T b)
    {
#if DEBUG
        Statistics.AddSwapCount();
#endif
        (a, b) = (b, a);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected ref T Index(ref Span<T> span, int pos)
    {
#if DEBUG
        Statistics.AddIndexCount();
#endif
        return ref span[pos];
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected int UnsignedRightShift(int number, int bits)
    {
        return (int)((uint)number >> bits);
    }
}
