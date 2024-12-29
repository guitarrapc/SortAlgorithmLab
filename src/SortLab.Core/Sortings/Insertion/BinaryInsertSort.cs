namespace SortLab.Core.Sortings;

/*
Ref span ...

| Method           | Number | Mean       | Error      | StdDev    | Median      | Min         | Max        | Allocated |
|----------------- |------- |-----------:|-----------:|----------:|------------:|------------:|-----------:|----------:|
| BinaryInsertSort | 100    |   119.4 us | 3,272.1 us | 179.35 us |    16.00 us |    15.70 us |   326.5 us |     736 B |
| BinaryInsertSort | 1000   |   118.0 us |   152.1 us |   8.33 us |   114.90 us |   111.60 us |   127.4 us |     448 B |
| BinaryInsertSort | 10000  | 7,034.7 us |   666.4 us |  36.53 us | 7,042.80 us | 6,994.80 us | 7,066.5 us |     736 B |

 */

/// <summary>
/// 通常のリニアサーチと異なり、挿入位置を2分探索して決定するため比較範囲、回数を改善できる。安定ソート
/// ソート済み配列には早いが、Reverse配列には遅い
/// </summary>
/// <remarks>
/// stable : yes
/// inplace : yes
/// Compare : log n
/// Swap : n^2/2
/// Order : O(n^2) (Better case : O(n)) (Worst case : O(n^2))
/// </remarks>
/// <typeparam name="T"></typeparam>
public class BinaryInsertSort<T> : SortBase<T> where T : IComparable<T>
{
    public override SortType SortType => SortType.Insertion;
    protected override string Name => nameof(BinaryInsertSort<T>);

    public override T[] Sort(T[] array)
    {
        Statistics.Reset(array.Length, SortType, Name);
        SortCore(array.AsSpan(), 0, array.Length);
        return array;
    }

    /// <summary>
    /// Sort the subrange [first..last).
    /// </summary>
    /// <param name="array"></param>
    /// <param name="first"></param>
    /// <param name="last"></param>
    /// <returns></returns>
    public T[] Sort(T[] array, int first, int last)
    {
        Statistics.Reset(array.Length, SortType, Name);
        SortCore(array.AsSpan(), first, last);
        return array;
    }

    /// <summary>
    /// Sort the subrange [first..last) and start insertion sort from 'start'.
    /// </summary>
    /// <param name="array"></param>
    /// <param name="first"></param>
    /// <param name="last"></param>
    /// <param name="start"></param>
    /// <returns></returns>
    public T[] Sort(T[] array, int first, int last, int start)
    {
        Statistics.Reset(array.Length, SortType, Name);
        SortCore(array.AsSpan(), first, last, start);
        return array;
    }

    private void SortCore(Span<T> span, int first, int last)
    {
        // The following commented-out code shows an example of using the built-in
        // Array.BinarySearch and Array.Copy for inserting an element, but it is
        // not used here since we rely on a manual approach below.
        //
        // for (var i = first + 1; i < last; i++)
        // {
        //     var tmp = span[i];
        //     var j = Array.BinarySearch(array, 0, i, tmp);
        //     Array.Copy(array, j, array, j+1, i - j);
        //     Swap(ref array[j], ref tmp);
        // }

        // Manual binary search and repeated swapping approach to insert 'tmp'
        // while maintaining stability (stable insertion sort).
        for (var i = first + 1; i < last; i++)
        {
            var tmp = Index(ref span, i);

            // Find the insertion position using a custom binary search
            var ois = BinarySearch(ref span, tmp, i);

            // Rotate 'tmp' into the correct place by swapping from 'left' up to 'i'.
            // This ensures elements between [left..i) move one position right
            // and 'tmp' takes the 'left' position in a stable manner.
            for (var j = ois; j <= i; j++)
            {
                Swap(ref Index(ref span, j), ref tmp);
            }
        }
    }

    private void SortCore(Span<T> span, int first, int last, int start)
    {
        // If 'start' equals 'first', move it forward to begin insertion from the next element
        if (start == first)
        {
            start++;
        }

        for (; start < last; start++)
        {
            var tmp = Index(ref span, start);

            // Find the insertion position using a custom binary search
            var pod = BinarySearch(ref span, tmp, start);

            // Perform multiple swaps to move 'tmp' to index 'left' in a stable fashion
            for (var n = start - pod; n > 0; n--)
            {
                Swap(ref Index(ref span, pod + n), ref Index(ref span, pod + n - 1));
            }

            // Finally, swap 'tmp' into the 'pos' position
            Swap(ref Index(ref span, pod), ref tmp);
        }
    }

    /// <summary>
    /// Performs a binary search over [0..index) to find the insertion point for 'tmp'.
    /// If elements are equal, we move to the right (<=0), ensuring a stable sort.
    /// </summary>
    private int BinarySearch(ref Span<T> span, T tmp, int index)
    {
        var left = 0;
        var right = index;
        while (left < right)
        {
            var mid = (left + right) / 2;
            if (Compare(Index(ref span, mid), tmp) <= 0)
            {
                left = mid + 1;
            }
            else
            {
                right = mid;
            }
        }
        return left;
    }
}
