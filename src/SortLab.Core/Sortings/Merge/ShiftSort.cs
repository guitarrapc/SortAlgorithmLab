namespace SortLab.Core.Sortings;

/// <summary>
/// Merge Sortの類似ですが、Swap回数が極度に少ない安定な内部ソート。ベストケースでO(n)、平均でもマージソート同様にO(n log n)と高速に挙動する
/// </summary>
/// <remarks>
/// stable : yes
/// inplace : no (n)
/// Compare : n log2 n
/// Swap : n log2 n
/// Order : O(n log n) (Best case : O(n), Worst case : O(n log n))
/// </remarks>
/// <typeparam name="T"></typeparam>

public class ShiftSort<T> : SortBase<T> where T : IComparable<T>
{
    // refer: https://github.com/JamesQuintero/ShiftSort
    public override SortMethod SortType => SortMethod.Merging;
    protected override string Name => nameof(ShiftSort<T>);

    public override void Sort(T[] array)
    {
        Statistics.Reset(array.Length, SortType, Name);
        SortCore(array.AsSpan());
    }

    public override void Sort(Span<T> span)
    {
        Statistics.Reset(span.Length, SortType, Name);
        SortCore(span);
    }

    private void SortCore(Span<T> span)
    {
        Span<int> zeroIndices = stackalloc int[(span.Length / 2) + 2];
        zeroIndices[0] = span.Length;

        var endTracker = 1;

        // check 3items where decrease order like "span[x -2] < span[x - 1] < span[x]"
        for (var x = span.Length - 1; x >= 1; x--)
        {
            if (Compare(Index(span, x), Index(span, x - 1)) < 0)
            {
                if (x > 1 && Compare(Index(span, x - 1), Index(span, x - 2)) < 0)
                {
                    // change to increase order
                    Swap(ref Index(span, x), ref Index(span, x - 2));

                    if (x != span.Length - 1)
                    {
                        if (Compare(Index(span, x + 1), Index(span, x)) < 0)
                        {
                            zeroIndices[endTracker] = x + 1;
                            endTracker++;
                        }
                    }
                }
                else
                {
                    zeroIndices[endTracker] = x;
                    endTracker++;
                }

                // skips
                x--;
            }
        }

        //merges sorted lists specified by zero_indices
        Split(span, zeroIndices, 0, endTracker);
    }

    private void Split(Span<T> span, Span<int> zeroIndices, int i, int j)
    {
        // if already 3 indices merge and end.
        // i == j -2
        if ((j - i) == 2)
        {
            Merge(span, zeroIndices[j], zeroIndices[j - 1], zeroIndices[i]);
            return;
        }
        else if ((j - i) < 2)
        {
            // indice or less = not enough to merge. end split.
            return;
        }

        var j2 = i + (j - i) / 2;
        var i2 = j2 + 1;

        // split first half
        Split(span, zeroIndices, i, j2);
        // split second half
        Split(span, zeroIndices, i2, j); ;

        // merge first half
        Merge(span, zeroIndices[i2], zeroIndices[j2], zeroIndices[i]);
        // merge second half
        Merge(span, zeroIndices[j], zeroIndices[i2], zeroIndices[i]);
    }

    private void Merge(Span<T> span, int first, int second, int third)
    {
        if (second - first > third - second)
        {
            // 2nd list
            var tmp2nd = new T[third - second];
            var counter = 0;
            for (var y = second; y < third; y++)
            {
                tmp2nd[counter] = Index(span, y);
                counter++;
            }

            // starts from 2nd list length
            var secondCounter = third - second;
            var left = second - 1;
            while (secondCounter > 0)
            {
                if (left >= first && Compare(Index(span, left), tmp2nd[secondCounter - 1]) >= 0)
                {
                    Index(span, left + secondCounter) = Index(span, left);
                    left--;
                }
                else
                {
                    Index(span, left + secondCounter) = tmp2nd[secondCounter - 1];
                    secondCounter--;
                }
            }
        }
        else
        {
            // 1st list
            var tmp1st = new T[second - first];
            var counter = 0;
            for (var y = first; y < second; y++)
            {
                tmp1st[counter] = Index(span, y);
                counter++;
            }

            // starts from 2nd list length
            var firstCounter = 0;
            var tmpLength = second - first;
            var right = second;
            while (firstCounter < tmp1st.Length)
            {
                if (right < third && Compare(Index(span, right), tmp1st[firstCounter]) < 0)
                {
                    Index(span, right - tmpLength) = Index(span, right);
                    right++;
                }
                else
                {
                    Index(span, right - tmpLength) = tmp1st[firstCounter];
                    firstCounter++;
                    tmpLength--;
                }
            }
        }
    }
}
