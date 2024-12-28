using System;

namespace SortLab.Core.Logics;

/// <summary>
/// ヒープソートの亜種。HeapSortのBinaryTree手法ではなく、Leonardo Sequenceで並べる。あとは、大きい数値をツリーから拾って順に並べる。レオナルド数列の特性から、順番に並んでいる要素をなるべく移動しないようにソー0とできるため、順番に並んでいるほど計算時間が少なくなる。(完全に並んでいればO(n))
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
    private static int q, r, p, b, c, r1, b1, c1;

    public override T[] Sort(T[] array)
    {
        Statistics.Reset(array.Length, SortType, nameof(SmoothSort<T>));
        return SortImpl(array);
    }

    private T[] SortImpl(T[] array)
    {
        q = 1;
        r = 0;
        p = 1;
        b = 1;
        c = 1;

        while (q < array.Length)
        {
            r1 = r;
            if ((p & 7) == 3)
            {
                b1 = b;
                c1 = c;
                Shift(array);
                p = (p + 1) >> 2;
                Up(ref b, ref c);
                Up(ref b, ref c);
            }
            else if ((p & 3) == 1)
            {
                if (q + c < array.Length)
                {
                    b1 = b;
                    c1 = c;
                    Shift(array);
                }
                else
                {
                    Trinkle(array);
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
        Trinkle(array);

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
                        SemiTrinkle(array);
                    }

                    Down(ref b, ref c);
                    p = (p << 1) + 1;
                    r = r + c;
                    SemiTrinkle(array);
                    Down(ref b, ref c);
                    p = (p << 1) + 1;
                }
            }
        }

        return array;
    }

    private void Shift(T[] array)
    {
        var r0 = r1;
        var t = array[r0];

        while (b1 >= 3)
        {
            var r2 = r1 - b1 + c1;
            Statistics.AddIndexCount();
            if (Compare(array[r1 - 1], array[r2]) > 0)
            {
                r2 = r1 - 1;
                Down(ref b1, ref c1);
            }

            if (Compare(array[r2], t) <= 0)
            {
                b1 = 1;
            }
            else
            {
                array[r1] = array[r2];
                r1 = r2;
                Down(ref b1, ref c1);
            }
        }

        if (r1 - r0 != 0)
        {
            array[r1] = t;
        }
    }

    private void Trinkle(T[] array)
    {
        int p1, r0 = 0;
        p1 = p;
        b1 = b;
        c1 = c;
        r0 = r1;
        var t = array[r0];

        while (p1 > 0)
        {
            while ((p1 & 1) == 0)
            {
                p1 >>= 1;
                Up(ref b1, ref c1);
            }

            var r3 = r1 - b1;

            Statistics.AddIndexCount();
            if ((p1 == 1) || Compare(array[r3], t) <= 0)
            {
                p1 = 0;
            }
            else
            {
                p1--;
                if (b1 == 1)
                {
                    array[r1] = array[r3];
                    r1 = r3;
                }
                else
                {
                    if (b1 >= 3)
                    {
                        var r2 = r1 - b1 + c1;
                        if (Compare(array[r1 - 1], array[r2]) > 0)
                        {
                            r2 = r1 - 1;
                            Down(ref b1, ref c1);
                            p1 <<= 1;
                        }

                        if (Compare(array[r2], array[r3]) <= 0)
                        {
                            array[r1] = array[r3];
                            r1 = r3;
                        }
                        else
                        {
                            array[r1] = array[r2];
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
            array[r1] = t;
        }

        Shift(array);
    }

    private void SemiTrinkle(T[] array)
    {
        r1 = r - c;
        if (Compare(array[r1], array[r]) > 0)
        {
            Swap(ref array[r], ref array[r1]);
            Trinkle(array);
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
