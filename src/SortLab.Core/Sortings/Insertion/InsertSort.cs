namespace SortLab.Core.Sortings;

/*

Ref span ...

| Method           | Number | Mean          | Error        | StdDev      | Median        | Min           | Max           | Allocated |
|----------------- |------- |--------------:|-------------:|------------:|--------------:|--------------:|--------------:|----------:|
| InsertSort       | 100    |    143.700 us | 3,371.833 us | 184.8215 us |     48.700 us |     25.700 us |    356.700 us |     736 B |
| InsertSort       | 1000   |    227.000 us |    60.727 us |   3.3287 us |    225.600 us |    224.600 us |    230.800 us |     736 B |
| InsertSort       | 10000  | 19,341.567 us |   917.448 us |  50.2884 us | 19,330.700 us | 19,297.600 us | 19,396.400 us |     448 B |
 
*/
/// <summary>
/// ソート済みとなる先頭に並ぶ配列がある。ソート済み配列の末尾から未ソートの配列の後ろに進み、各要素と比較して値が小さい限りソート済み配列と入れ替える。(つまり新しい要素の値が小さい限り前にいく)。ICompatibleの性質から、n > n-1 = -1 となり、> 0 で元順序を保証しているので安定ソート。
/// ソート済み配列には早いが、Reverse配列には遅い
/// </summary>
/// <remarks>
/// stable : yes
/// inplace : yes
/// Compare : n(n-1) / 2
/// Swap : n^2/2
/// Order : O(n^2) (Better case : O(n)) (Worst case : O(n^2))
/// </remarks>
/// <typeparam name="T"></typeparam>
public class InsertSort<T> : SortBase<T> where T : IComparable<T>
{
    public override SortType SortType => SortType.Insertion;
    protected override string Name => nameof(InsertSort<T>);

    public override T[] Sort(T[] array)
    {
        Statistics.Reset(array.Length, SortType, Name);
        SortCore(array.AsSpan());
        return array;
    }

    public T[] Sort(T[] array, int first, int last)
    {
        Statistics.Reset(array.Length, SortType, Name);
        SortCore(array.AsSpan(), first, last);
        return array;
    }

    private void SortCore(Span<T> span)
    {
        for (var i = 1; i < span.Length; i++)
        {
            var tmp = span[i];
            for (var j = i; j >= 1 && Compare(Index(ref span, j - 1), Index(ref span, j)) > 0; --j)
            {
                //array.Dump($"{j - 1} : {array[j - 1]}, {j} : {array[j]}, {array[j - 1].CompareTo(array[j]) > 0}");
                if (Compare(Index(ref span, j - 1), Index(ref span, j)) > 0)
                {
                    Swap(ref Index(ref span, j), ref Index(ref span, j - 1));
                }
            }
        }
    }

    private void SortCore(Span<T> span, int first, int last)
    {
        for (var i = first + 1; i < last; i++)
        {
            for (var j = i; j > first && Compare(Index(ref span, j - 1), Index(ref span, j)) > 0; --j)
            {
                Statistics.AddIndexCount();
                Swap(ref Index(ref span, j), ref Index(ref span, j - 1));
            }
        }
    }
}
