namespace SortLab.Core.Sortings;

/*
Ref span ...

| Method        | Number | Mean          | Error          | StdDev        | Median        | Min          | Max           | Allocated |
|-------------- |------- |--------------:|---------------:|--------------:|--------------:|-------------:|--------------:|----------:|
| SmoothSort    | 100    |      22.57 us |     103.626 us |      5.680 us |      19.80 us |     18.80 us |      29.10 us |     448 B |
| SmoothSort    | 1000   |     921.87 us |  19,097.679 us |  1,046.808 us |     323.00 us |    312.00 us |   2,130.60 us |     736 B |
| SmoothSort    | 10000  |   3,582.73 us |     290.198 us |     15.907 us |   3,573.70 us |  3,573.40 us |   3,601.10 us |     448 B |

 */

/// <summary>
/// ヒープソートの亜種。HeapSortのBinaryTree手法ではなく、Leonardo Sequenceで並べる。あとは、大きい数値をツリーから拾って順に並べる。レオナルド数列の特性から、順番に並んでいる要素をなるべく移動しないようにソートできるため、順番に並んでいるほど計算時間が少なくなる。(完全に並んでいればO(n))
/// </summary>
/// <remarks>
/// stable : no
/// inplace : yes
/// Compare : n log n
/// Swap : n log n
/// Order : O(n log n) (best case : n) (Worst case : O(n log n))
/// </remarks>
/// <typeparam name="T"></typeparam>

public class SmoothSort<T> : SortBase<T> where T : IComparable<T>
{
    // refer : https://www.slideshare.net/habib_786/smooth-sort
    public override SortMethod SortType => SortMethod.Selection;
    protected override string Name => nameof(SmoothSort<T>);

    public override T[] Sort(T[] array)
    {
        Statistics.Reset(array.Length, SortType, Name);
        SortCore(array.AsSpan());

        return array;
    }

    public void Sort(Span<T> span)
    {
        Statistics.Reset(span.Length, SortType, nameof(SmoothSort<T>));
        SortCore(span);
    }

    private void SortCore(Span<T> span)
    {
        int q = 1, r = 0, p = 1, b = 1, c = 1;
        int r1 = 0, b1 = 0, c1 = 0;

        // Build heap fase
        while (q < span.Length)
        {
            r1 = r;
            //Debug.WriteLine($"[SmoothSort - Build] Start q={q}, r={r}, p={p}, b={b}, c={c}, r1={r1}");

            if ((p & 7) == 3)
            {
                b1 = b;
                c1 = c;
                Shift(span, ref r1, ref b1, ref c1);

                p = (p + 1) >> 2;

                Up(ref b, ref c);
                Up(ref b, ref c);
            }
            else if ((p & 3) == 1)
            {
                if (q + c < span.Length)
                {
                    b1 = b;
                    c1 = c;
                    Shift(span, ref r1, ref b1, ref c1);
                }
                else
                {
                    Trinkle(span, ref p, ref b1, ref b, ref c1, ref c, ref r1);
                }

                Down(ref b, ref c);
                p <<= 1;

                while (b > 1)
                {
                    Down(ref b, ref c);
                    p <<= 1;

                    ArgumentOutOfRangeException.ThrowIfZero(p);
                }
                ++p;

                ArgumentOutOfRangeException.ThrowIfZero(p);
            }

            ++q;
            ++r;
        }

        // Sort fase
        r1 = r;
        //Debug.WriteLine($"[SmoothSort - Sort] Start q={q}, r={r}, p={p}, b={b}, c={c}, r1={r1}");
        Trinkle(span, ref p, ref b1, ref b, ref c1, ref c, ref r1);

        while (q > 1)
        {
            //Debug.WriteLine($"[SmoothSort - Sort] Loop q={q}, r={r}, p={p}, b={b}, c={c}, r1={r1}");
            --q;
            if (b == 1)
            {
                --r;
                --p;

                // shift while p is power of 2
                while ((p & 1) == 0)
                {
                    p >>= 1;
                    Up(ref b, ref c);
                }
            }
            else
            {
                if (b >= 3)
                {
                    --p;
                    r = r - b + c;

                    if (p > 0)
                    {
                        SemiTrinkle(span, ref p, ref b1, ref b, ref c1, ref c, ref r1, ref r);
                    }

                    Down(ref b, ref c);
                    p = (p << 1) + 1;

                    // update r and re-SemiTrinkle
                    r = r + c;
                    SemiTrinkle(span, ref p, ref b1, ref b, ref c1, ref c, ref r1, ref r);
                    Down(ref b, ref c);
                    p = (p << 1) + 1;

                    ArgumentOutOfRangeException.ThrowIfZero(p);
                }
            }

            ArgumentOutOfRangeException.ThrowIfZero(p);
        }
    }

    /// <summary>
    /// Shift the element to the right position
    /// </summary>
    /// <param name="span"></param>
    /// <param name="r1"></param>
    /// <param name="b1"></param>
    /// <param name="c1"></param>
    private void Shift(Span<T> span, ref int r1, ref int b1, ref int c1)
    {
        var r0 = r1;
        var t = Index(ref span, r0);

        while (b1 >= 3)
        {
            var r2 = r1 - b1 + c1;
            if (Compare(Index(ref span, r1 - 1), Index(ref span, r2)) > 0)
            {
                r2 = r1 - 1;
                Down(ref b1, ref c1);
            }

            if (Compare(Index(ref span, r2), t) <= 0)
            {
                b1 = 1;
            }
            else
            {
                span[r1] = Index(ref span, r2);
                r1 = r2;
                Down(ref b1, ref c1);
            }
        }

        if (r1 - r0 != 0)
        {
            span[r1] = t;
        }
    }

    /// <summary>
    /// Construct or adjust the partial heap (reconstruct the main heap)
    /// </summary>
    /// <param name="span"></param>
    /// <param name="p"></param>
    /// <param name="b1"></param>
    /// <param name="b"></param>
    /// <param name="c1"></param>
    /// <param name="c"></param>
    /// <param name="r1"></param>
    private void Trinkle(Span<T> span, ref int p, ref int b1, ref int b, ref int c1, ref int c, ref int r1)
    {
        int p1 = p, r0 = r1;
        b1 = b;
        c1 = c;

        var t = Index(ref span, r0);

        while (p1 > 0)
        {
            while ((p1 & 1) == 0)
            {
                p1 >>= 1;
                Up(ref b1, ref c1);
            }

            var r3 = r1 - b1;

            if ((p1 == 1) || Compare(Index(ref span, r3), t) <= 0)
            {
                // don't need to reconstruct the heap
                p1 = 0;
            }
            else
            {
                p1--;
                if (b1 == 1)
                {
                    // 1st step heap, just move the element
                    span[r1] = Index(ref span, r3);
                    r1 = r3;
                }
                else
                {
                    if (b1 >= 3)
                    {
                        var r2 = r1 - b1 + c1;
                        if (Compare(Index(ref span, r1 - 1), Index(ref span, r2)) > 0)
                        {
                            r2 = r1 - 1;
                            Down(ref b1, ref c1);
                            p1 <<= 1;
                        }

                        // Judge swap or not
                        if (Compare(Index(ref span, r2), Index(ref span, r3)) <= 0)
                        {
                            span[r1] = Index(ref span, r3);
                            r1 = r3;
                        }
                        else
                        {
                            span[r1] = Index(ref span, r2);
                            r1 = r2;
                            Down(ref b1, ref c1);
                            p1 = 0;
                        }
                    }
                }
            }
        }

        // fix if position is changed from origin
        if (r1 - r0 != 0)
        {
            span[r1] = t;
        }

        // final adjustment
        Shift(span, ref r1, ref b1, ref c1);
    }

    /// <summary>
    /// Trinkle part of the heap
    /// </summary>
    /// <param name="span"></param>
    /// <param name="p"></param>
    /// <param name="b1"></param>
    /// <param name="b"></param>
    /// <param name="c1"></param>
    /// <param name="c"></param>
    /// <param name="r1"></param>
    /// <param name="r"></param>
    private void SemiTrinkle(Span<T> span, ref int p, ref int b1, ref int b, ref int c1, ref int c, ref int r1, ref int r)
    {
        r1 = r - c;
        if (Compare(Index(ref span, r1), Index(ref span, r)) > 0)
        {
            Swap(ref Index(ref span, r), ref Index(ref span, r1));
            Trinkle(span, ref p, ref b1, ref b, ref c1, ref c, ref r1);
        }
    }

    /// <summary>
    /// Upward Leonardo heap step
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void Up(ref int a, ref int b)
    {
#if DEBUG
        Statistics.AddSwapCount();
#endif
        var temp = a;
        a += b + 1;
        b = temp;
    }

    /// <summary>
    /// Downward Leonardo heap step
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void Down(ref int a, ref int b)
    {
#if DEBUG
        Statistics.AddSwapCount();
#endif
        var temp = b;
        b = a - b - 1;
        a = temp;
    }
}
