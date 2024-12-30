namespace SortLab.Core.Sortings;

/*
Ref span ...

| Method        | Number | Mean          | Error          | StdDev        | Median        | Min          | Max           | Allocated |
|-------------- |------- |--------------:|---------------:|--------------:|--------------:|-------------:|--------------:|----------:|
| SelectionSort | 100    |      16.27 us |      10.690 us |      0.586 us |      16.50 us |     15.60 us |      16.70 us |     736 B |
| SelectionSort | 1000   |     267.27 us |      46.952 us |      2.574 us |     268.60 us |    264.30 us |     268.90 us |     448 B |
| SelectionSort | 10000  |  20,349.98 us |   1,341.729 us |     73.545 us |  20,340.15 us | 20,281.85 us |  20,427.95 us |     736 B |

 */

/// <summary>
/// 配列にアクセスして、常に最小を末尾まで走査。最小を現在のインデックスであるn番目の要素（ソート済みの要素の末尾）と交換を続ける。値をインデックスベースで入れ替えてしまうため不安定ソート。
/// 交換回数が一定して少ないので、比較が軽くて交換が重い場合に有効
/// </summary>
/// <remarks>
/// stable : no
/// inplace : yes
/// Compare : n(n-1) / 2
/// Swap : n-1
/// Order : O(n^2)
/// </remarks>
/// <typeparam name="T"></typeparam>
public class SelectionSort<T> : SortBase<T> where T : IComparable<T>
{
    public override SortMethod Method => SortMethod.Selection;
    protected override string Name => nameof(SelectionSort<T>);

    public override T[] Sort(T[] array)
    {
        Statistics.Reset(array.Length, Method, Name);
        var span = array.AsSpan();
        SortCore(span);

        return array;
    }

    public void Sort(Span<T> span)
    {
        Statistics.Reset(span.Length, Method, nameof(SelectionSort<T>));
        SortCore(span);
    }

    private void SortCore(Span<T> span)
    {
        for (var i = 0; i < span.Length - 1; i++)
        {
            var min = i;

            // Find the index of the minimum element
            for (var j = i + 1; j < span.Length; j++)
            {
                if (Compare(Index(ref span, j), Index(ref span, min)) < 0)
                {
                    min = j;
                }
            }

            // Swap the found minimum element with the first element of the unsorted part
            if (min != i)
            {
                Swap(ref Index(ref span, min), ref Index(ref span, i));
            }
        }
    }
}
