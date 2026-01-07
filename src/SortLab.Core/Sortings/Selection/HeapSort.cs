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

Ref span (Recursive heapify) ...

| Method        | Number | Mean          | Error          | StdDev        | Median        | Min          | Max           | Allocated |
|-------------- |------- |--------------:|---------------:|--------------:|--------------:|-------------:|--------------:|----------:|
| HeapSort      | 100    |      13.57 us |       4.213 us |      0.231 us |      13.70 us |     13.30 us |      13.70 us |     736 B |
| HeapSort      | 1000   |     195.50 us |      11.393 us |      0.624 us |     195.30 us |    195.00 us |     196.20 us |     736 B |
| HeapSort      | 10000  |   2,571.93 us |      83.948 us |      4.601 us |   2,571.80 us |  2,567.40 us |   2,576.60 us |     448 B |

Ref span ...

| Method        | Number | Mean          | Error          | StdDev        | Median        | Min          | Max           | Allocated |
|-------------- |------- |--------------:|---------------:|--------------:|--------------:|-------------:|--------------:|----------:|
| HeapSort      | 100    |      14.13 us |       3.798 us |      0.208 us |      14.20 us |     13.90 us |      14.30 us |     736 B |
| HeapSort      | 1000   |     252.43 us |      18.989 us |      1.041 us |     252.10 us |    251.60 us |     253.60 us |     736 B |
| HeapSort      | 10000  |     649.33 us |     843.733 us |     46.248 us |     653.60 us |    601.10 us |     693.30 us |     736 B |

Span ...

| Method        | Number | Mean          | Error          | StdDev       | Median       | Min          | Max           | Allocated |
|-------------- |------- |--------------:|---------------:|-------------:|-------------:|-------------:|--------------:|----------:|
| HeapSort      | 100    |      15.40 us |       6.320 us |     0.346 us |     15.60 us |     15.00 us |      15.60 us |     736 B |
| HeapSort      | 1000   |     279.03 us |      28.400 us |     1.557 us |    279.20 us |    277.40 us |     280.50 us |     736 B |
| HeapSort      | 10000  |     545.52 us |     109.346 us |     5.994 us |    543.05 us |    541.15 us |     552.35 us |     736 B |

*/

/// <summary>
/// 配列から、常に最大の要素をルートにもつヒープ（二分ヒープ）を作成します（この時点で不安定）。
/// その後、ルート要素をソート済み配列の末尾に移動し、ヒープの末端をルートに持ってきて再度ヒープ構造を維持します。これを繰り返すことで、ヒープの最大値が常にルートに保たれ、ソート済み配列に追加されることで自然とソートが行われます。
/// <br/>
/// Builds a heap (binary heap) from the array where the root always contains the maximum element (which is inherently unstable).
/// Then, the root element is moved to the end of the sorted array, the last element is moved to the root, and the heap structure is re-established. Repeating this process ensures that the maximum value in the heap is always at the root, allowing elements to be naturally sorted as they are moved to the sorted array.
/// </summary>
/// <remarks>
/// stable : no
/// inplace : yes
/// Compare : O(n log n)  
/// Swap    : O(n log n)  
/// Index   : O(n log n) (Each element is accessed O(log n) times during heap operations)  
/// Order   : O(n log n) (best, average, and worst cases)
/// </remarks>
/// <typeparam name="T"></typeparam>

public class HeapSort<T> : SortBase<T> where T : IComparable<T>
{
    public override SortMethod SortType => SortMethod.Selection;
    protected override string Name => nameof(HeapSort<T>);

    public override void Sort(T[] array)
    {
        SortCore(array.AsSpan(), 0, array.Length);
    }

    public override void Sort(Span<T> span)
    {
        SortCore(span, 0, span.Length);
    }

    /// <summary>
    /// Sort a portion of the array from index first to last-1.
    /// </summary>
    /// <param name="array"></param>
    /// <param name="first"></param>
    /// <param name="last"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public void Sort(T[] array, int first, int last)
    {
        SortCore(array.AsSpan(), first, last);
    }

    /// <summary>
    /// Sort a portion of the span from index first to last-1.
    /// </summary>
    /// <param name="span"></param>
    /// <param name="first"></param>
    /// <param name="last"></param>
    public void Sort(Span<T> span, int first, int last)
    {
        SortCore(span, first, last);
    }

    private void SortCore(Span<T> span, int first, int last)
    {
        Statistics.Reset(span.Length, SortType, Name);

        if (first < 0 || last > span.Length || first >= last)
            throw new ArgumentOutOfRangeException(nameof(first), "Invalid range for sorting.");

        var n = last - first;

        // Build heap
        for (var i = first + n / 2 - 1; i >= first; i--)
        {
            Heapify(span, i, n, first);
        }

        // Extract elements from heap
        for (var i = last - 1; i > first; i--)
        {
            // Move current root to end
            Swap(ref Index(span, first), ref Index(span, i));

            // Re-heapify the reduced heap
            Heapify(span, first, i - first, first);
        }
    }

    /// <summary>
    /// To heapify a subtree rooted with node i which is an index in span[]. n is size of heap.
    /// </summary>
    /// <param name="span"></param>
    /// <param name="root"></param>
    /// <param name="size"></param>
    /// <param name="offset"></param>
    private void Heapify(Span<T> span, int root, int size, int offset)
    {
        while (true)
        {
            var largest = root;
            var left = 2 * (root - offset) + 1 + offset;
            var right = 2 * (root - offset) + 2 + offset;

            // If left child is larger than root
            if (left < offset + size && Compare(Index(span, left), Index(span, largest)) > 0)
            {
                largest = left;
            }

            // If right child is larger than largest so far
            if (right < offset + size && Compare(Index(span, right), Index(span, largest)) > 0)
            {
                largest = right;
            }

            // If largest is not root, swap and heapify the affected sub-tree
            if (largest != root)
            {
                Swap(ref Index(span, root), ref Index(span, largest));
                root = largest;
            }
            else
            {
                break;
            }
        }
    }
}
