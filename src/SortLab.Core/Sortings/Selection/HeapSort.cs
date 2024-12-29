namespace SortLab.Core.Sortings;

/*
Array ...

| Method   | Number | Mean        | Error       | StdDev    | Min         | Max         | Allocated |
|--------- |------- |------------:|------------:|----------:|------------:|------------:|----------:|
| HeapSort | 100    |    16.60 us |    10.16 us |  0.557 us |    16.10 us |    17.20 us |     448 B |
| HeapSort | 1000   |   226.70 us |    42.08 us |  2.307 us |   224.50 us |   229.10 us |     736 B |
| HeapSort | 10000  | 3,028.70 us | 1,625.10 us | 89.077 us | 2,960.20 us | 3,129.40 us |     736 B |

Span ...

| Method   | Number | Mean        | Error      | StdDev    | Min         | Max         | Allocated |
|--------- |------- |------------:|-----------:|----------:|------------:|------------:|----------:|
| HeapSort | 100    |    17.20 us |   5.473 us |  0.300 us |    16.90 us |    17.50 us |     448 B |
| HeapSort | 1000   |   254.60 us |  59.145 us |  3.242 us |   251.70 us |   258.10 us |     736 B |
| HeapSort | 10000  | 3,375.50 us | 330.347 us | 18.107 us | 3,357.70 us | 3,393.90 us |     736 B |

Ref span ...

| Method        | Number | Mean          | Error          | StdDev        | Median        | Min          | Max           | Allocated |
|-------------- |------- |--------------:|---------------:|--------------:|--------------:|-------------:|--------------:|----------:|
| HeapSort      | 100    |      12.90 us |       7.952 us |      0.436 us |      12.70 us |     12.60 us |      13.40 us |     736 B |
| HeapSort      | 1000   |     183.47 us |       9.362 us |      0.513 us |     183.60 us |    182.90 us |     183.90 us |     736 B |
| HeapSort      | 10000  |   2,434.78 us |     343.769 us |     18.843 us |   2,430.65 us |  2,418.35 us |   2,455.35 us |     736 B |

*/

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
    protected override string Name => nameof(HeapSort<T>);

    public override T[] Sort(T[] array)
    {
        Statistics.Reset(array.Length, SortType, Name);
        var span = array.AsSpan();
        SortCore(span);

        return array;
    }

    public void Sort(Span<T> span)
    {
        Statistics.Reset(span.Length, SortType, Name);
        SortCore(span);
    }

    private void SortCore(Span<T> span)
    {
        var n = span.Length;

        // Build heap
        for (var i = n / 2 - 1; i >= 0; i--)
        {
            DownHeap(span, i, n);
        }

        // Extract elements from heap
        for (var i = n - 1; i > 0; i--)
        {
            // Move current root to end
            Swap(ref Index(ref span, 0), ref Index(ref span, i));
            // Re-heapify the reduced heap
            DownHeap(span, 0, i);
        }
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

        Statistics.Reset(array.Length, SortType, Name);

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

    private void DownHeap(Span<T> span, int root, int size)
    {
        var largest = root;  // Initialize largest as root
        var left = 2 * root + 1;  // Left child
        var right = 2 * root + 2;  // Right child

        // If left child is larger than root
        if (left < size && Compare(Index(ref span, left), Index(ref span, largest)) > 0)
        {
            largest = left;
        }

        // If right child is larger than largest so far
        if (right < size && Compare(Index(ref span, right), Index(ref span, largest)) > 0)
        {
            largest = right;
        }

        // If largest is not root
        if (largest != root)
        {
            Swap(ref Index(ref span, root), ref Index(ref span, largest));
            // Recursively heapify the affected sub-tree
            DownHeap(span, largest, size);
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
