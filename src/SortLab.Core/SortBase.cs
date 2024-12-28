﻿using System;
using System.Runtime.CompilerServices;

namespace SortLab.Core;

public abstract class SortBase<T> : ISort<T> where T : IComparable<T>
{
    public IStatistics Statistics => statistics;
    protected IStatistics statistics = new SortStatistics();

    public virtual SortType SortType => SortType.None;

    public abstract T[] Sort(T[] array);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected int Compare(T x, T y)
    {
        Statistics.AddCompareCount();
        return x.CompareTo(y);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected void Swap(ref T a, ref T b)
    {
        Statistics.AddSwapCount();
        (a, b) = (b, a);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected int UnsignedRightShift(int number, int bits)
    {
        return (int)((uint)number >> bits);
    }
}
