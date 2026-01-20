namespace SortLab.Core.Sortings;

/*

Ref span ...

| Method        | Number | Mean          | Error          | StdDev       | Median        | Min          | Max           | Allocated |
|-------------- |------- |--------------:|---------------:|-------------:|--------------:|-------------:|--------------:|----------:|
| PancakeSort   | 100    |      82.23 us |      91.261 us |     5.002 us |      81.40 us |     77.70 us |      87.60 us |     736 B |
| PancakeSort   | 1000   |   5,258.63 us |     518.649 us |    28.429 us |   5,246.60 us |  5,238.20 us |   5,291.10 us |     736 B |
| PancakeSort   | 10000  |  32,987.80 us |   4,770.804 us |   261.504 us |  32,853.00 us | 32,821.20 us |  33,289.20 us |     736 B |

PancakeSort ...

| Method        | Number | Mean          | Error          | StdDev       | Median       | Min          | Max           | Allocated |
|-------------- |------- |--------------:|---------------:|-------------:|-------------:|-------------:|--------------:|----------:|
| PancakeSort   | 100    |      85.48 us |      88.803 us |     4.868 us |     84.55 us |     81.15 us |      90.75 us |     736 B |
| PancakeSort   | 1000   |   5,593.57 us |     377.536 us |    20.694 us |  5,585.90 us |  5,577.80 us |   5,617.00 us |     736 B |
| PancakeSort   | 10000  |  33,698.07 us |   1,645.026 us |    90.169 us | 33,649.70 us | 33,642.40 us |  33,802.10 us |     736 B |

*/

/// <summary>
/// 未ソート部分から最大の要素を見つけ、それを先頭にフリップ（逆転）し、次に先頭から未ソート部分の最後の要素までフリップして配置します。このプロセスを繰り返すことで、配列をソートします。フリップ数がカウントに相当し、フリップ回数が少ない場合は高速ですが、多くなると遅くなります。
/// <br/>
/// Finds the maximum element in the unsorted portion, flips (reverses) the array to bring it to the front, and then flips the array up to the last unsorted element to place the maximum element in its correct position. This process is repeated until the array is sorted. The number of flips corresponds to the count, and fewer flips result in faster performance, while more flips can slow down the process.
/// </summary>
/// <remarks>
/// stable  : no
/// inplace : yes
/// Compare : O(n^2)  (Performs approximately n(n-1)/2 comparisons)  
/// Swap    : O(n^2)  (Performs up to 2n swap in the worst case)  
/// Index   : O(n^2)  (Each element is accessed O(n) times during the sort) 
/// Order   : O(n^2)  (Average and worst-case time complexity)  
/// </remarks>
/// <typeparam name="T"></typeparam>
public class PancakeSort<T> : SortBase<T> where T : IComparable<T>
{
    public override SortMethod SortType => SortMethod.Selection;
    protected override string Name => nameof(PancakeSort<T>);

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

    /// <summary>
    /// Sort a portion of the array from index first to last-1.
    /// </summary>
    /// <param name="array"></param>
    /// <param name="first"></param>
    /// <param name="last"></param>
    internal void Sort(T[] array, int first, int last)
    {
        SortCore(array.AsSpan(), first, last);
    }

    /// <summary>
    /// Sort a portion of the span from index first to last-1.
    /// </summary>
    /// <param name="span"></param>
    /// <param name="first"></param>
    /// <param name="last"></param>
    internal void Sort(Span<T> span, int first, int last)
    {
        SortCore(span, first, last);
    }

    private void SortCore(Span<T> span, int first, int last)
    {
        if (first < 0 || last > span.Length || first >= last)
            throw new ArgumentOutOfRangeException(nameof(first), "Invalid range for sorting.");

        for (var currentSize = last; currentSize > first; currentSize--)
        {
            var maxIndex = MaxIndex(span, first, currentSize);

            // Max element is already at the end
            if (maxIndex == currentSize - 1)
                continue;

            // Move the maximum element to the front, then flip to the right position
            Flip(span, first, maxIndex);
            Flip(span, first, currentSize - 1);
        }
    }

    /// <summary>
    /// Finds the index of the maximum element within the first n elements of the span.
    /// </summary>
    /// <param name="span"></param>
    /// <param name="last"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int MaxIndex(Span<T> span, int first, int last)
    {
        var maxIndex = first;
        for (int i = first + 1; i < last; i++)
        {
            if (Compare(Index(span, i), Index(span, maxIndex)) > 0)
            {
                maxIndex = i;
            }
        }
        return maxIndex;
    }

    /// <summary>
    /// Reverses the elements in the span from index 0 to index i.
    /// </summary>
    /// <param name="span"></param>
    /// <param name="end"></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void Flip(Span<T> span, int start, int end)
    {
        while (start < end)
        {
            Swap(ref Index(span, start), ref Index(span, end));
            start++;
            end--;
        }
    }
}
