namespace SortLab.Core.Sortings;

/*
Ref span ...

| Method           | Number | Mean       | Error       | StdDev    | Median      | Min         | Max        | Allocated |
|----------------- |------- |-----------:|------------:|----------:|------------:|------------:|-----------:|----------:|
| BinaryInsertSort | 100    |   103.8 us | 2,796.00 us | 153.26 us |    15.70 us |    15.00 us |   280.8 us |     736 B |
| BinaryInsertSort | 1000   |   111.8 us |    11.15 us |   0.61 us |   111.70 us |   111.30 us |   112.5 us |     736 B |
| BinaryInsertSort | 10000  | 6,861.7 us |   359.62 us |  19.71 us | 6,862.50 us | 6,841.60 us | 6,881.0 us |     448 B |

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

    public T[] Sort(T[] array, int first, int last)
    {
        Statistics.Reset(array.Length, SortType, Name);
        SortCore(array.AsSpan(), first, last);
        return array;
    }

    public T[] Sort(T[] array, int first, int last, int start)
    {
        Statistics.Reset(array.Length, SortType, Name);
        SortCore(array.AsSpan(), first, last, start);
        return array;
    }

    private void SortCore(Span<T> span, int first, int last)
    {
        // C# Managed Code BinarySearch + Swap
        // for (var i = first + 1; i < last; i++)
        // {
        //     var j = Array.BinarySearch(array, 0, i, tmp);
        //     Array.Copy(array, j, array, j+1, i-j);
        //     Swap(ref array[j], ref tmp);
        // }

        // Handmade BinarySearch + Swap
        for (var i = first + 1; i < last; i++)
        {
            var tmp = Index(ref span, i);

            // BinarySearch
            var left = BinarySearch(ref span, tmp, i);

            // Stable Sort
            for (var j = left; j <= i; j++)
            {
                Swap(ref Index(ref span, j), ref tmp);
            }
        }
    }

    private void SortCore(Span<T> span, int first, int last, int start)
    {
        if (start == first)
        {
            start++;
        }

        for (; start < last; start++)
        {
            var tmp = Index(ref span, start);

            // BinarySearch
            var left = BinarySearch(ref span, tmp, start);

            // Stable Sort
            for (var n = start - left; n > 0; n--)
            {
                Statistics.AddIndexCount();
                Swap(ref Index(ref span, left + n), ref Index(ref span, left + n - 1));
            }

            Swap(ref Index(ref span, left), ref tmp);
        }
    }

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
