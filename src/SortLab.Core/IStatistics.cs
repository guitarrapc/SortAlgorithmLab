﻿using System.Threading;

namespace SortLab.Core;

public class SortStatistics : IStatistics
{
    public SortMethod SortType { get; set; }
    public string Algorithm { get; set; }
    public int ArraySize { get; set; }
    public ulong IndexAccessCount => _indexAccessCount;
    public ulong CompareCount => _compareCount;
    public ulong SwapCount => _swapCount;
    public bool IsSorted { get; set; }

    private ulong _indexAccessCount;
    private ulong _compareCount;
    private ulong _swapCount;

    public void Reset()
    {
        SortType = SortMethod.None;
        Algorithm = "";
        ArraySize = 0;
        IsSorted = false;
        _indexAccessCount = 0;
        _compareCount = 0;
        _swapCount = 0;
    }

    public void Reset(int arraySize, SortMethod sortType, string algorithm)
    {
        ArraySize = arraySize;
        SortType = sortType;
        Algorithm = algorithm;
        IsSorted = false;
        _indexAccessCount = 0;
        _compareCount = 0;
        _swapCount = 0;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AddIndexCount() => Interlocked.Increment(ref _indexAccessCount);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AddIndexCount(ulong count) => Interlocked.Add(ref _indexAccessCount, count);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AddCompareCount() => Interlocked.Increment(ref _compareCount);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AddCompareCount(ulong count) => Interlocked.Add(ref _compareCount, count);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AddSwapCount() => Interlocked.Increment(ref _swapCount);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AddSwapCount(ulong count) => Interlocked.Add(ref _swapCount, count);
}

public interface IStatistics
{
    SortMethod SortType { get; set; }
    string Algorithm { get; set; }
    int ArraySize { get; set; }
    ulong IndexAccessCount { get; }
    ulong CompareCount { get; }
    ulong SwapCount { get; }
    bool IsSorted { get; set; }

    void Reset();

    void Reset(int arraySize, SortMethod sortType, string algorithm);

    void AddIndexCount();
    void AddIndexCount(ulong count);
    void AddCompareCount();
    void AddCompareCount(ulong count);
    void AddSwapCount();
    void AddSwapCount(ulong count);
}