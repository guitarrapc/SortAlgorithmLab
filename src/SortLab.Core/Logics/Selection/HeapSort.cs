using System;

namespace SortLab.Core.Logics;

/// <summary>
/// 配列から、常に最大の要素をルートにもつ2分木構造(BinaruTree : ヒープ)を作る(この時点で不安定)。あとは、ルート要素をソート済み配列の末尾に詰めて、ヒープの末端をルートに持ってきて再度ヒープ構造を作る。これを繰り返すことでヒープの最大値は常にルート要素になり、これをソート済み配列につめていくことで自然とソートができる。
/// </summary>
/// <remarks>
/// stable : no
/// inplace : yes
/// Compare : n log2 n
/// Swap : n log2 2n
/// Order : O(n log n) (best case : n log n (n if all keys are distinct)) (Worst case : O(n log n))
/// </remarks>
/// <typeparam name="T"></typeparam>

public class HeapSort<T> : SortBase<T> where T : IComparable<T>
{
    public override SortType SortType => SortType.Selection;

    public override T[] Sort(T[] array)
    {
        Statistics.Reset(array.Length, SortType, nameof(HeapSort<T>));

        var i = 0;
        // create heap node
        while (i < array.Length)
        {
            UpHeap(array, i++);
        }
        // pick root and re-heap.
        while (--i > 0)
        {
            // move Max Heap to sorted array
            Swap(ref array[0], ref array[i]);
            // re-heap
            DownHeap(array, i - 1);
        }
        return array;
    }

    public T[] Sort(T[] array, int low, int high)
    {
        Statistics.Reset(array.Length, SortType, nameof(HeapSort<T>));

        var n = high - low;
        // create heap node
        for (var i = n / 2; i >= 1; i--)
        {
            DownHeap(array, i, n, low);
        }
        // pick root and re-heap.
        for (var i = n; i > 1; i--)
        {
            // move Max Heap to sorted array
            Swap(ref array[0], ref array[i]);
            // re-heap
            DownHeap(array, 1, i - 1, low);
        }
        return array;
    }

    // compare and move it to upward if larger or equal.
    private void UpHeap(T[] array, int current)
    {
        while (current != 0)
        {
            Statistics.AddIndexAccess();
            var parent = (current - 1) / 2;

            if (Compare(array[current], array[parent]) > 0)
            {
                Swap(ref array[current], ref array[parent]);
                current = parent;
            }
            else
            {
                break;
            }
        }
    }

    // pick root item to sorted array. then pick last heap item to root then start compare with heap node.
    private void DownHeap(T[] array, int current)
    {
        if (current == 0) return;
        var parent = 0;
        while (true)
        {
            Statistics.AddIndexAccess();
            var child = 2 * parent + 1;
            if (child > current) break;
            if (child < current && Compare(array[child], array[child + 1]) < 0)
            {
                child++;
            }
            if (Compare(array[parent], array[child]) < 0)
            {
                Swap(ref array[parent], ref array[child]);
                parent = child;
            }
            else
            {
                break;
            }
        }
    }

    private void DownHeap(T[] array, int i, int n, int low)
    {
        var d = array[low + i - 1];
        int child;
        while (i <= n / 2)
        {
            child = 2 * i;
            Statistics.AddIndexAccess();
            if (child < n && Compare(array[low + child - 1], array[low + child]) < 0)
            {
                child++;
            }
            if (Compare(d, array[low + child - 1]) >= 0) break;
            array[low + i - 1] = array[low + child - 1];
            i = child;
        }
        array[low + i - 1] = d;
    }
}
