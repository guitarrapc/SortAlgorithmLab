using System;

namespace SortLab.Core.Logics;

/// <summary>
/// 配列から、常に最大の要素をルートにもつ2分木構造(BinaryTree : ヒープ)を作る(この時点で不安定)。あとは、ルート要素をソート済み配列の末尾に詰めて、ヒープの末端をルートに持ってきて再度ヒープ構造を作る。これを繰り返すことでヒープの最大値は常にルート要素になり、これをソート済み配列につめていくことで自然とソートができる。
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
        var n = array.Length;

        // Build heap
        for (var i = n / 2 - 1; i >= 0; i--)
        {
            DownHeap(array, i, n);
        }

        // Extract elements from heap
        for (var i = n - 1; i > 0; i--)
        {
            // Move current root to end
            Swap(ref array[0], ref array[i]);
            // Re-heapify the reduced heap
            DownHeap(array, 0, i);
        }
        return array;
    }

    /// <summary>
    /// 指定した範囲の配列をヒープソートする
    /// </summary>
    /// <param name="array"></param>
    /// <param name="low"></param>
    /// <param name="high"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public T[] Sort(T[] array, int low, int high)
    {
        if (low < 0 || high > array.Length || low >= high)
        {
            throw new ArgumentOutOfRangeException(nameof(low), "Invalid range for sorting.");
        }

        Statistics.Reset(array.Length, SortType, nameof(HeapSort<T>));

        var n = high - low;

        // Build heap
        for (var i = n / 2 - 1; i >= 0; i--)
        {
            DownHeap(array, i, n, low);
        }

        // Extract elements from heap
        for (var i = n - 1; i > 0; i--)
        {
            // Move current root to end
            Swap(ref array[low], ref array[low + i]);
            // Re-heapify the reduced heap
            DownHeap(array, 0, i, low);
        }

        return array;
    }

    private void DownHeap(T[] array, int root, int size)
    {
        var largest = root;  // Initialize largest as root
        var left = 2 * root + 1;  // Left child
        var right = 2 * root + 2;  // Right child

        // If left child is larger than root
        if (left < size && Compare(array[left], array[largest]) > 0)
        {
            largest = left;
        }

        // If right child is larger than largest so far
        if (right < size && Compare(array[right], array[largest]) > 0)
        {
            largest = right;
        }

        // If largest is not root
        if (largest != root)
        {
            Swap(ref array[root], ref array[largest]);
            // Recursively heapify the affected sub-tree
            DownHeap(array, largest, size);
        }
    }

    private void DownHeap(T[] array, int root, int size, int low)
    {
        var largest = root;
        var left = 2 * root + 1;
        var right = 2 * root + 2;

        // Adjust indices by adding "low"
        if (left < size && Compare(array[low + left], array[low + largest]) > 0)
        {
            largest = left;
        }

        if (right < size && Compare(array[low + right], array[low + largest]) > 0)
        {
            largest = right;
        }

        if (largest != root)
        {
            Swap(ref array[low + root], ref array[low + largest]);
            DownHeap(array, largest, size, low);
        }
    }
}
