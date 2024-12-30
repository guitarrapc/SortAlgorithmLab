﻿namespace SortLab.Core.Sortings;

/*
Ref span ...

| Method           | Number | Mean       | Error       | StdDev    | Median      | Min         | Max        | Allocated |
|----------------- |------- |-----------:|------------:|----------:|------------:|------------:|-----------:|----------:|
| BinaryInsertSort | 100    |   112.1 us | 2,820.79 us | 154.62 us |    31.50 us |    14.50 us |   290.4 us |     736 B |
| BinaryInsertSort | 1000   |   112.4 us |    55.38 us |   3.04 us |   112.90 us |   109.10 us |   115.1 us |     736 B |
| BinaryInsertSort | 10000  | 6,902.2 us |   897.73 us |  49.21 us | 6,929.30 us | 6,845.40 us | 6,931.9 us |     736 B |

Ref span (start) ...

| Method           | Number | Mean       | Error       | StdDev    | Median      | Min         | Max        | Allocated |
|----------------- |------- |-----------:|------------:|----------:|------------:|------------:|-----------:|----------:|
| BinaryInsertSort | 100    |   122.6 us | 3,383.21 us | 185.45 us |    15.70 us |    15.30 us |   336.7 us |     448 B |
| BinaryInsertSort | 1000   |   121.1 us |    55.55 us |   3.04 us |   121.70 us |   117.80 us |   123.8 us |     448 B |
| BinaryInsertSort | 10000  | 8,673.2 us | 4,841.61 us | 265.38 us | 8,720.50 us | 8,387.30 us | 8,911.7 us |     736 B |

*/

/// <summary>
/// 通常のリニアサーチと異なり、挿入位置を二分探索して決定するため比較範囲、回数を改善できる。
/// ソート済み配列では高速に動作しますが、逆順の配列では遅くなります。
/// <br/>
/// Unlike the traditional linear search, it uses binary search to determine the insertion position, thereby improving the comparison range and the number of comparisons. Stable sort.
/// Performs efficiently on already sorted arrays but is slow on reverse-sorted arrays.
/// </summary>
/// <remarks>
/// stable  : yes
/// inplace : yes
/// Compare : O(n log n) on average (Each insertion performs a binary search) 
/// Swap    : O(n^2/2) (Each insertion may require shifting elements) 
/// Index   : O(n^2) (Each element may be accessed multiple times during shifts)
/// Order   : O(n log n)
///         * average:                   O(n log n) for comparisons and O(n^2) for shifts  
///         * best case (nearly sorted): O(n)  
///         * worst case can approach:   O(n^2) 
/// </remarks>
/// <typeparam name="T"></typeparam>
public class BinaryInsertSort<T> : SortBase<T> where T : IComparable<T>
{
    public override SortMethod Method => SortMethod.Insertion;
    protected override string Name => nameof(BinaryInsertSort<T>);

    public override T[] Sort(T[] array)
    {
        Statistics.Reset(array.Length, Method, Name);
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
        Statistics.Reset(array.Length, Method, Name);
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
        Statistics.Reset(array.Length, Method, Name);
        SortCore(array.AsSpan(), first, last, start);
        return array;
    }

    /// <summary>
    /// binary insertion sort with swap
    /// </summary>
    /// <param name="span"></param>
    /// <param name="first"></param>
    /// <param name="last"></param>
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
            var pos = BinarySearch(ref span, tmp, i);

            // Rotate 'tmp' into the correct place by swapping from 'left' up to 'i'.
            // This ensures elements between [left..i) move one position right
            // and 'tmp' takes the 'left' position in a stable manner.
            for (var j = pos; j <= i; j++)
            {
                Swap(ref Index(ref span, j), ref tmp);
            }
        }
    }

    /// <summary>
    /// binary insertion sort with shift
    /// </summary>
    /// <param name="span"></param>
    /// <param name="first"></param>
    /// <param name="last"></param>
    /// <param name="start"></param>
    private void SortCore(Span<T> span, int first, int last, int start)
    {
        // If 'start' equals 'first', move it forward to begin insertion from the next element
        if (start == first)
        {
            start++;
        }

        for (var i = start; i < last; i++)
        {
            var tmp = Index(ref span, i);

            // Find the insertion position using a custom binary search
            var pos = BinarySearch(ref span, tmp, i);

            // shift [pod.. start-1] -> [pod+1.. start]
            int length = i - pos;
            if (length > 0)
            {
                for (var j = i - 1; j >= pos; j--)
                {
                    Index(ref span, j + 1) = Index(ref span, j);
                }
            }

            // Finally, swap 'tmp' into the 'pos' position
            Index(ref span, pos) = tmp;
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
