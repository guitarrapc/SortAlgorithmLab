using System;

namespace SortLab.Core.Logics;

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
    public override SortType SortType => SortType.Selection;

    public override T[] Sort(T[] array)
    {
        Statistics.Reset(array.Length, SortType, nameof(SmoothSort<T>));
        return SortCore(array);
    }

    private T[] SortCore(T[] array)
    {
        var span = array.AsSpan();

        int q = 1, r = 0, p = 1, b = 1, c = 1;
        int r1 = 0, b1 = 0, c1 = 0;

        while (q < span.Length)
        {
            r1 = r;
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
                }
                ++p;
            }

            ++q;
            ++r;
        }

        r1 = r;
        Trinkle(span, ref p, ref b1, ref b, ref c1, ref c, ref r1);

        while (q > 1)
        {
            --q;
            if (b == 1)
            {
                --r;
                --p;
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
                    r = r + c;
                    SemiTrinkle(span, ref p, ref b1, ref b, ref c1, ref c, ref r1, ref r);
                    Down(ref b, ref c);
                    p = (p << 1) + 1;
                }
            }
        }

        return array;
    }

    private void Shift(Span<T> span, ref int r1, ref int b1, ref int c1)
    {
        var r0 = r1;
        var t = span[r0];

        while (b1 >= 3)
        {
            var r2 = r1 - b1 + c1;
            Statistics.AddIndexCount();
            if (Compare(span[r1 - 1], span[r2]) > 0)
            {
                r2 = r1 - 1;
                Down(ref b1, ref c1);
            }

            if (Compare(span[r2], t) <= 0)
            {
                b1 = 1;
            }
            else
            {
                span[r1] = span[r2];
                r1 = r2;
                Down(ref b1, ref c1);
            }
        }

        if (r1 - r0 != 0)
        {
            span[r1] = t;
        }
    }

    private void Trinkle(Span<T> span, ref int p, ref int b1, ref int b, ref int c1, ref int c, ref int r1)
    {
        int p1, r0 = 0;
        p1 = p;
        b1 = b;
        c1 = c;
        r0 = r1;
        var t = span[r0];

        while (p1 > 0)
        {
            while ((p1 & 1) == 0)
            {
                p1 >>= 1;
                Up(ref b1, ref c1);
            }

            var r3 = r1 - b1;

            Statistics.AddIndexCount();
            if ((p1 == 1) || Compare(span[r3], t) <= 0)
            {
                p1 = 0;
            }
            else
            {
                p1--;
                if (b1 == 1)
                {
                    span[r1] = span[r3];
                    r1 = r3;
                }
                else
                {
                    if (b1 >= 3)
                    {
                        var r2 = r1 - b1 + c1;
                        if (Compare(span[r1 - 1], span[r2]) > 0)
                        {
                            r2 = r1 - 1;
                            Down(ref b1, ref c1);
                            p1 <<= 1;
                        }

                        if (Compare(span[r2], span[r3]) <= 0)
                        {
                            span[r1] = span[r3];
                            r1 = r3;
                        }
                        else
                        {
                            span[r1] = span[r2];
                            r1 = r2;
                            Down(ref b1, ref c1);
                            p1 = 0;
                        }
                    }
                }
            }
        }

        if (r1 - r0 != 0)
        {
            span[r1] = t;
        }

        Shift(span, ref r1, ref b1, ref c1);
    }

    private void SemiTrinkle(Span<T> span, ref int p, ref int b1, ref int b, ref int c1, ref int c, ref int r1, ref int r)
    {
        r1 = r - c;
        if (Compare(span[r1], span[r]) > 0)
        {
            Swap(ref span[r], ref span[r1]);
            Trinkle(span, ref p, ref b1, ref b, ref c1, ref c, ref r1);
        }
    }

    private void Up(ref int a, ref int b)
    {
        Statistics.AddSwapCount();
        var temp = a;
        a += b + 1;
        b = temp;
    }

    private void Down(ref int a, ref int b)
    {
        Statistics.AddSwapCount();
        var temp = b;
        b = a - b - 1;
        a = temp;
    }
}
