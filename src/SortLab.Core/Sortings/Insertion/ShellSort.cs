namespace SortLab.Core.Sortings;

/*

Ref span ...

| Method           | Number | Mean          | Error        | StdDev      | Median        | Min           | Max           | Allocated |
|----------------- |------- |--------------:|-------------:|------------:|--------------:|--------------:|--------------:|----------:|
| ShellSort        | 100    |      9.767 us |    10.374 us |   0.5686 us |      9.600 us |      9.300 us |     10.400 us |     448 B |
| ShellSort        | 1000   |     42.933 us |     4.591 us |   0.2517 us |     42.900 us |     42.700 us |     43.200 us |     736 B |
| ShellSort        | 10000  |    502.667 us |   142.001 us |   7.7835 us |    503.500 us |    494.500 us |    510.000 us |     736 B |

*/

/// <summary>
/// hi+1 = 3hi + 1となるhで配列を分割し、分割された細かい配列ごとに挿入ソート<see cref="InsertionSort{T}"/>を行う(A)。次のhを/3で求めて、Aを繰り返しh=1まで行う。hごとにでソート済みとなっているため、最後の1は通常の挿入ソートと同じだが、挿入ソートが持つソート済み配列で高速に動作する性質から高速な並び替えが可能になる。選択ソートを使っているので不安定ソート。<see cref="BubbleSort{T}"/>に同様の概念を適用したのが<see cref="CombSort{T}"/>である。
/// </summary>
/// <remarks>
/// stable : no
/// inplace : yes
/// Compare : O(nlogn) * O(n) (O(n^0.25)～O(n^0.5) * O(n) = O(n^1.25))
/// Swap :
/// Order : O(n^1.25) (Better case : O(n)) (Worst case : O(nlog^2n))
/// </remarks>
/// <typeparam name="T"></typeparam>
public class ShellSort<T> : SortBase<T> where T : IComparable<T>
{
    public override SortType SortType => SortType.Insertion;
    protected override string Name => nameof(ShellSort<T>);

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

    private void SortCore(Span<T> span, int first, int last)
    {
        var length = last - first;
        // Only 1 or 0 elements
        if (length < 2)
            return;

        // Knuth gap sequence `h(i + 1) = 3h(i) + 1`: 1, 4, 13, 40, 121, 364, 1093, ...
        var h = 0;
        while (h < length / 9) // 目安で h を大きくする
        {
            h = h * 3 + 1;
        }

        // make gap h smaller from large, try next h with / 3....
        for (; h > 0; h /= 3)
        {
            //h.Dump(array.Length.ToString());
            // Same as InsertSort (1 will be h)
            for (var i = first + h; i < last; i++)
            {
                // As like InsertSort, compare and swap
                for (int j = i; j >= h && Compare(Index(ref span, j - h), Index(ref span, j)) > 0; j -= h)
                {
                    Swap(ref Index(ref span, j), ref Index(ref span, j - h));
                }
            }
        }
    }
}
