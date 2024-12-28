using System;

namespace SortLab.Core.Logics;

/// <summary>
/// Contains Bug on HeapSort.
/// QuickSort + HeapSort + InsertSort によるQuickSortの最悪ケースでのO(n^2) を回避する実装。一定の深度以下になった場合にHeapSortにスイッチすることで最悪ケースを防ぎ、ほぼソート済み状況ではInsertSortで最速を狙う。
/// </summary>
/// <remarks>
/// stable : no
/// inplace : no (log n)
/// Compare : n log n
/// Swap : n log n
/// Order : O(n log n) (Worst case : O(n log n))
/// </remarks>
/// <typeparam name="T"></typeparam>
public class IntroSortMedian3<T> : SortBase<T> where T : IComparable<T>
{
    public override SortType SortType => SortType.Hybrid;

    // ref : https://www.cs.waikato.ac.nz/~bernhard/317/source/IntroSort.java
    private const int IntroThreshold = 16;
    private HeapSort<T> heapSort = new HeapSort<T>();
    private InsertSort<T> insertSort = new InsertSort<T>();

    public override T[] Sort(T[] array)
    {
        Statistics.Reset(array.Length, SortType, nameof(IntroSortMedian3<T>));
        var result = Sort(array, 0, array.Length - 1, 2 * FloorLog(array.Length));
        Statistics.AddCompareCount(heapSort.Statistics.CompareCount);
        Statistics.AddIndexAccess(heapSort.Statistics.IndexAccessCount);
        Statistics.AddSwapCount(heapSort.Statistics.SwapCount);
        Statistics.AddCompareCount(insertSort.Statistics.CompareCount);
        Statistics.AddIndexAccess(insertSort.Statistics.IndexAccessCount);
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
            Statistics.AddIndexAccess();
            var partition = Partition(array, left, right, Median3(array[left], array[left + ((right - left) / 2) + 1], array[right - 1]));
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
                Statistics.AddIndexAccess();
                l++;
            }
            r--;
            while (Compare(pivot, array[r]) < 0)
            {
                Statistics.AddIndexAccess();
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

    private static int FloorLog(int length)
    {
        return (int)(Math.Floor(Math.Log(length) / Math.Log(2)));
    }
}
