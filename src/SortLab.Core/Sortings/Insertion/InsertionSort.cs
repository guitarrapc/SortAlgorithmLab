namespace SortLab.Core.Sortings;

/*

Ref span ...

| Method           | Number | Mean          | Error        | StdDev      | Median        | Min           | Max           | Allocated |
|----------------- |------- |--------------:|-------------:|------------:|--------------:|--------------:|--------------:|----------:|
| InsertionSort    | 100    |     99.600 us | 2,663.800 us | 146.0119 us |     15.300 us |     15.300 us |    268.200 us |     736 B |
| InsertionSort    | 1000   |    148.700 us |   100.936 us |   5.5326 us |    147.600 us |    143.800 us |    154.700 us |     736 B |
| InsertionSort    | 10000  | 12,273.300 us | 3,278.699 us | 179.7166 us | 12,242.300 us | 12,111.100 us | 12,466.500 us |     448 B |

Span ...

| Method                 | Number | Mean          | Error        | StdDev      | Median        | Min           | Max          | Allocated |
|----------------------- |------- |--------------:|-------------:|------------:|--------------:|--------------:|-------------:|----------:|
| InsertionSort          | 100    |    101.700 us | 2,690.663 us | 147.4843 us |     16.800 us |     16.300 us |    272.00 us |     736 B |
| InsertionSort          | 1000   |    148.733 us |   249.388 us |  13.6698 us |    141.500 us |    140.200 us |    164.50 us |     448 B |
| InsertionSort          | 10000  | 12,031.800 us |   758.226 us |  41.5609 us | 12,010.400 us | 12,005.300 us | 12,079.70 us |     736 B |

*/

/// <summary>
/// ソート済みとなる先頭に並ぶ部分を維持します。ソート済み部分の末尾から配列の末尾に向かって進み、各要素と比較し、値が小さい限りソート済み部分と交換します。(つまり、新しい要素の値が小さい限り前方に移動します。) 
/// <see cref="IComparable"/> の性質により、x.CompareTo(y) > 0 の場合は元の順序が保持されるため、安定ソートです。
/// すでにソートされた配列では高速に動作しますが、逆順の配列では遅くなります。  
/// <br/>
/// Maintains a sorted portion of the array at the beginning. Progresses from the end of the sorted portion towards the end of the array, Compares each element and swaps it with the sorted portion as long as it is smaller. (In other words, new elements are moved forward as long as their value is smaller.)
/// Due to the properties of <see cref="IComparable"/>, where x.CompareTo(y) > 0 ensures the original order is preserved, this is a stable sort.
/// Performs well on already sorted arrays but is slow on reverse-sorted arrays.
/// </summary>
/// <remarks>
/// stable  : yes
/// inplace : yes
/// Compare : O(n^2), strictly n(n-1) / 2
/// Swap    : 0
/// Index   : O(n^2) (Each element may be accessed multiple times during swaps)
/// Order   : O(n^2)
///         * average:                   O(n^2) 
///         * best case (nearly sorted): O(n)
///         * worst case can approach  : O(n^2)
/// </remarks>
/// <typeparam name="T"></typeparam>
public class InsertionSort<T> : SortBase<T> where T : IComparable<T>
{
    public override SortMethod SortType => SortMethod.Insertion;
    protected override string Name => nameof(InsertionSort<T>);

    public override void Sort(T[] array)
    {
        Statistics.Reset(array.Length, SortType, Name);
        SortCore(array.AsSpan(), 0, array.Length);
    }

    public override void Sort(Span<T> span)
    {
        Statistics.Reset(span.Length, SortType, Name);
        SortCore(span, 0, span.Length);
    }

    internal void Sort(Span<T> span, int first, int last)
    {
        SortCore(span, first, last);
    }

    /// <summary>
    /// Optimized insertion sort
    /// </summary>
    /// <param name="span"></param>
    /// <param name="first"></param>
    /// <param name="last"></param>
    private void SortCore(Span<T> span, int first, int last)
    {
        if (last - first <= 1)
            return;

        for (var i = first + 1; i < last; i++)
        {
            // Temporarily store the value to be inserted
            var tmp = Index(span, i);

            // Shift the elements larger than tmp to the right
            var j = i - 1;
            while (j >= first && Compare(Index(span, j), tmp) > 0)
            {
                Index(span, j + 1) = Index(span, j);
                j--;
            }

            // Insert tmp into the correct position
            Index(span, j + 1) = tmp;
        }
    }
}

/// <summary>
/// ソート済みとなる先頭に並ぶ部分を維持します。ソート済み部分の末尾から配列の末尾に向かって進み、各要素と比較し、値が小さい限りソート済み部分と交換します。(つまり、新しい要素の値が小さい限り前方に移動します。) 
/// <see cref="IComparable"/> の性質により、x.CompareTo(y) > 0 の場合は元の順序が保持されるため、安定ソートです。
/// すでにソートされた配列では高速に動作しますが、逆順の配列では遅くなります。  
/// <br/>
/// Maintains a sorted portion of the array at the beginning. Progresses from the end of the sorted portion towards the end of the array, Compares each element and swaps it with the sorted portion as long as it is smaller. (In other words, new elements are moved forward as long as their value is smaller.)
/// Due to the properties of <see cref="IComparable"/>, where x.CompareTo(y) > 0 ensures the original order is preserved, this is a stable sort.
/// Performs well on already sorted arrays but is slow on reverse-sorted arrays.
/// </summary>
/// <remarks>
/// stable  : yes
/// inplace : yes
/// Compare : O(n^2), strictly n(n-1) / 2
/// Swap    : O(n^2), strictly n^2/2
/// Index   : O(n^2) (Each element may be accessed multiple times during swaps)
/// Order   : O(n^2)
///         * average:                   O(n^2) 
///         * best case (nearly sorted): O(n)
///         * worst case can approach  : O(n^2)
/// </remarks>
/// <typeparam name="T"></typeparam>
public class InsertionNonOptimizedSort<T> : SortBase<T> where T : IComparable<T>
{
    public override SortMethod SortType => SortMethod.Insertion;
    protected override string Name => nameof(InsertionSort<T>);

    public override void Sort(T[] array)
    {
        Statistics.Reset(array.Length, SortType, Name);
        SortCore(array.AsSpan(), 0, array.Length);
    }

    public override void Sort(Span<T> span)
    {
        Statistics.Reset(span.Length, SortType, Name);
        SortCore(span, 0, span.Length);
    }

    internal void Sort(Span<T> span, int first, int last)
    {
        SortCore(span, first, last);
    }

    /// <summary>
    /// Insertion sort using swaps (non-optimized)
    /// </summary>
    /// <param name="span"></param>
    /// <param name="first"></param>
    /// <param name="last"></param>
    private void SortCore(Span<T> span, int first, int last)
    {
        if (last - first <= 1)
            return;

        for (var i = first + 1; i < last; i++)
        {
            // Move the element at position i backward until it's in the correct position
            var j = i;
            while (j > first && Compare(Index(span, j - 1), Index(span, j)) > 0)
            {
                Swap(ref Index(span, j - 1), ref Index(span, j));
                j--;
            }
        }
    }
}
