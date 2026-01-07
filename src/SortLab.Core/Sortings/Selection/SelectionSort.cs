namespace SortLab.Core.Sortings;

/*

Ref span ...

| Method        | Number | Mean          | Error          | StdDev        | Median       | Min          | Max           | Allocated |
|-------------- |------- |--------------:|---------------:|--------------:|-------------:|-------------:|--------------:|----------:|
| SelectionSort | 100    |      15.83 us |      12.147 us |      0.666 us |     16.00 us |     15.10 us |      16.40 us |     736 B |
| SelectionSort | 1000   |     260.53 us |      28.400 us |      1.557 us |    260.70 us |    258.90 us |     262.00 us |     736 B |
| SelectionSort | 10000  |  19,651.13 us |   1,740.622 us |     95.409 us | 19,596.70 us | 19,595.40 us |  19,761.30 us |     448 B |

Span ...

| Method        | Number | Mean          | Error          | StdDev       | Median       | Min          | Max           | Allocated |
|-------------- |------- |--------------:|---------------:|-------------:|-------------:|-------------:|--------------:|----------:|
| SelectionSort | 100    |      17.40 us |       3.649 us |     0.200 us |     17.40 us |     17.20 us |      17.60 us |     448 B |
| SelectionSort | 1000   |     234.67 us |      15.191 us |     0.833 us |    234.40 us |    234.00 us |     235.60 us |     448 B |
| SelectionSort | 10000  |  20,021.47 us |     846.496 us |    46.399 us | 20,012.20 us | 19,980.40 us |  20,071.80 us |     736 B |

 */

/// <summary>
/// 配列の各位置に対して、未ソート部分から最小の要素を見つけて現在の位置と交換することでソートを行います。値をインデックスベースで交換するため、不安定なソートアルゴリズムです。
/// <br/>
/// Iterates through each position in the array, finding the minimum element in the unsorted portion and swapping it with the current position. Swapping elements based on indices makes Selection Sort an unstable sorting algorithm.
/// </summary>
/// <remarks>
/// stable : no
/// inplace : yes
/// Compare : O(n^2)  (Performs approximately n(n-1)/2 comparisons) 
/// Swap : O(n)    (Performs n-1 swaps)
/// Order : O(n^2)  (Best, average, and worst-case time complexity)
/// </remarks>
/// <typeparam name="T"></typeparam>
public class SelectionSort<T> : SortBase<T> where T : IComparable<T>
{
    public override SortMethod SortType => SortMethod.Selection;
    protected override string Name => nameof(SelectionSort<T>);

    public override T[] Sort(T[] array)
    {
        Statistics.Reset(array.Length, SortType, Name);
        SortCore(array.AsSpan(), 0, array.Length);

        return array;
    }

    public T[] Sort(T[] array, int first, int last)
    {
        Statistics.Reset(array.Length, SortType, Name);
        SortCore(array.AsSpan(), first, last);

        return array;
    }

    /// <summary>
    /// Sort the subrange [first..last).
    /// </summary>
    /// <param name="span"></param>
    /// <param name="first"></param>
    /// <param name="last"></param>
    private void SortCore(Span<T> span, int first, int last)
    {
        if (first < 0 || last > span.Length || first >= last)
        {
            throw new ArgumentOutOfRangeException(nameof(first), "Invalid range for sorting.");
        }

        for (var i = first; i < last - 1; i++)
        {
            var min = i;

            // Find the index of the minimum element
            for (var j = i + 1; j < last; j++)
            {
                if (Compare(Index(span, j), Index(span, min)) < 0)
                {
                    min = j;
                }
            }

            // Swap the found minimum element with the first element of the unsorted part
            if (min != i)
            {
                Swap(ref Index(span, min), ref Index(span, i));
            }
        }
    }
}
