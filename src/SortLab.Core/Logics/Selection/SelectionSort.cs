using System;

namespace SortLab.Core.Logics;

/*
Ref span ...

| Method        | Number | Mean         | Error         | StdDev       | Min          | Max          | Allocated |
|-------------- |------- |-------------:|--------------:|-------------:|-------------:|-------------:|----------:|
| SelectionSort | 100    |     17.60 us |     11.393 us |     0.624 us |     16.90 us |     18.10 us |     736 B |
| SelectionSort | 1000   |    271.30 us |     18.515 us |     1.015 us |    270.20 us |    272.20 us |     736 B |
| SelectionSort | 10000  | 19,828.60 us |  4,618.653 us |   253.164 us | 19,536.60 us | 19,986.60 us |     736 B |
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
    public override SortType SortType => SortType.Selection;

    public override T[] Sort(T[] array)
    {
        Statistics.Reset(array.Length, SortType, nameof(SelectionSort<T>));
        var span = array.AsSpan();

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
        return array;
    }
}
