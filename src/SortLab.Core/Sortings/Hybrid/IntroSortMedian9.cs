﻿namespace SortLab.Core.Sortings;

/// <summary>
/// Contains Bug on HeapSort.
/// 配列から、常に最大の要素をルートにもつ2分木構造(ヒープ)を作る(この時点で不安定)。あとは、ルート要素をソート済み配列の末尾に詰めて、ヒープの末端をルートに持ってきて再度ヒープ構造を作る。これを繰り返すことでヒープの最大値は常にルート要素になり、これをソート済み配列につめていくことで自然とソートができる。
/// Median3 は山データでエッジケース問題があるため、Median9が望ましい
/// </summary>
/// <remarks>
/// stable : no
/// inplace : no (log n)
/// Compare : n log n
/// Swap : n log n
/// Order : O(n log n) (Worst case : O(n log n))
/// </remarks>
/// <typeparam name="T"></typeparam>
public class IntroSortMedian9<T> : SortBase<T> where T : IComparable<T>
{
    public override SortMethod Method => SortMethod.Hybrid;
    protected override string Name => nameof(IntroSortMedian9<T>);

    // ref : https://www.cs.waikato.ac.nz/~bernhard/317/source/IntroSort.java
    private const int IntroThreshold = 16;
    private HeapSort<T> heapSort = new HeapSort<T>();
    private InsertionSort<T> insertSort = new InsertionSort<T>();

    public override T[] Sort(T[] array)
    {
        Statistics.Reset(array.Length, Method, Name);
        var result = Sort(array, 0, array.Length - 1, 2 * FloorLog(array.Length));
        Statistics.AddCompareCount(heapSort.Statistics.CompareCount);
        Statistics.AddIndexCount(heapSort.Statistics.IndexAccessCount);
        Statistics.AddSwapCount(heapSort.Statistics.SwapCount);
        Statistics.AddCompareCount(insertSort.Statistics.CompareCount);
        Statistics.AddIndexCount(insertSort.Statistics.IndexAccessCount);
        Statistics.AddSwapCount(insertSort.Statistics.SwapCount);
        return result;
    }

    private T[] Sort(T[] array, int left, int right, int depthLimit)
    {
        while (right - left > IntroThreshold)
        {
            if (depthLimit == 0)
            {
                heapSort.Sort(array, left, right);
                return array;
            }
            depthLimit--;
            Statistics.AddIndexCount();
            var partition = Partition(array, left, right, Median9(array, left, right));
            Sort(array, partition, right, depthLimit);
            right = partition;
        }
        return insertSort.Sort(array, left, right + 1);
    }

    private int Partition(T[] array, int left, int right, T pivot)
    {
        var l = left;
        var r = right;
        while (true)
        {
            while (Compare(array[l], pivot) < 0)
            {
                Statistics.AddIndexCount();
                l++;
            }
            r--;
            while (Compare(pivot, array[r]) < 0)
            {
                Statistics.AddIndexCount();
                r--;
            }

            if (!(l < r))
            {
                return l;
            }

            Swap(ref array[l], ref array[r]);
            l++;
        }
    }

    private T Median3(T low, T mid, T high)
    {
        if (Compare(low, mid) > 0)
        {
            if (Compare(mid, high) > 0)
            {
                return mid;
            }
            else
            {
                return Compare(low, high) > 0 ? high : low;
            }
        }
        else
        {
            if (Compare(mid, high) > 0)
            {
                return Compare(low, high) > 0 ? low : high;
            }
            else
            {
                return mid;
            }
        }
    }

    private T Median9(T[] array, int low, int high)
    {
        var m2 = (high - low) / 2;
        var m4 = m2 / 2;
        var m8 = m4 / 2;
        var a = array[low];
        var b = array[low + m8];
        var c = array[low + m4];
        var d = array[low + m2 - m8];
        var e = array[low + m2];
        var f = array[low + m2 + m8];
        var g = array[high - m4];
        var h = array[high - m8];
        var i = array[high];
        return Median3(Median3(a, b, c), Median3(d, e, f), Median3(g, h, i));
    }

    private static int FloorLog(int length)
    {
        return (int)(Math.Floor(Math.Log(length) / Math.Log(2)));
    }
}
