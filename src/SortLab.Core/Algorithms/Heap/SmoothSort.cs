using SortLab.Core.Contexts;
using System.Diagnostics;

namespace SortLab.Core.Algorithms;

/*
 *
Ref span ...

| Method        | Number | Mean          | Error          | StdDev        | Median        | Min          | Max           | Allocated |
|-------------- |------- |--------------:|---------------:|--------------:|--------------:|-------------:|--------------:|----------:|
| SmoothSort    | 100    |      22.57 us |     103.626 us |      5.680 us |      19.80 us |     18.80 us |      29.10 us |     448 B |
| SmoothSort    | 1000   |     921.87 us |  19,097.679 us |  1,046.808 us |     323.00 us |    312.00 us |   2,130.60 us |     736 B |
| SmoothSort    | 10000  |   3,582.73 us |     290.198 us |     15.907 us |   3,573.70 us |  3,573.40 us |   3,601.10 us |     448 B |

Span ...

| Method        | Number | Mean          | Error          | StdDev       | Median       | Min          | Max           | Allocated |
|-------------- |------- |--------------:|---------------:|-------------:|-------------:|-------------:|--------------:|----------:|
| SmoothSort    | 100    |      19.47 us |       4.591 us |     0.252 us |     19.50 us |     19.20 us |      19.70 us |     736 B |
| SmoothSort    | 1000   |     925.30 us |  18,632.536 us | 1,021.312 us |    339.80 us |    331.50 us |   2,104.60 us |     736 B |
| SmoothSort    | 10000  |   3,857.30 us |     352.104 us |    19.300 us |  3,860.50 us |  3,836.60 us |   3,874.80 us |     736 B |

 */

/// <summary>
/// ヒープソートの亜種。HeapSortのBinaryTree手法ではなく、Leonardo Sequenceで並べる。あとは、大きい数値をツリーから拾って順に並べる。レオナルド数列の特性から、順番に並んでいる要素をなるべく移動しないようにソートできるため、順番に並んでいるほど計算時間が少なくなる。(完全に並んでいればO(n))
/// </summary>
/// <remarks>
/// stable  : no
/// inplace : yes
/// Compare : O(n log n)
/// Swap    : O(n log n)
/// Order   : O(n log n)
///         * average   : O(n log n)
///         * best case : O(n)       (when data is already sorted)
///         * worst case: O(n log n)
/// </remarks>
/// <typeparam name="T"></typeparam>

public static class SmoothSort
{
    // refer : https://www.slideshare.net/habib_786/smooth-sort
    public static void Sort<T>(Span<T> span) where T : IComparable<T>
    {
        Sort(span, NullContext.Default);
    }

    public static void Sort<T>(Span<T> span, ISortContext context) where T : IComparable<T>
    {
        if (span.Length <= 1) return;

        var s = new SortSpan<T>(span, context);

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
                Shift(s, ref r1, ref b1, ref c1);

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
                    Shift(s, ref r1, ref b1, ref c1);
                }
                else
                {
                    Trinkle(s, ref p, ref b1, ref b, ref c1, ref c, ref r1);
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
        Trinkle(s, ref p, ref b1, ref b, ref c1, ref c, ref r1);

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
                        SemiTrinkle(s, ref p, ref b1, ref b, ref c1, ref c, ref r1, ref r);
                    }

                    Down(ref b, ref c);
                    p = (p << 1) + 1;

                    // update r and re-SemiTrinkle
                    r = r + c;
                    SemiTrinkle(s, ref p, ref b1, ref b, ref c1, ref c, ref r1, ref r);
                    Down(ref b, ref c);
                    p = (p << 1) + 1;

                    Debug.Assert(p != 0, "p should not be zero");
                }
            }

            Debug.Assert(p != 0, "p should not be zero");
        }
    }

    /// <summary>
    /// Shift the element to the right position
    /// </summary>
    /// <param name="s"></param>
    /// <param name="r1"></param>
    /// <param name="b1"></param>
    /// <param name="c1"></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void Shift<T>(SortSpan<T> s, ref int r1, ref int b1, ref int c1) where T : IComparable<T>
    {
        var r0 = r1;
        var t = s.Read(r0);

        while (b1 >= 3)
        {
            var r2 = r1 - b1 + c1;
            if (s.Compare(r1 - 1, r2) > 0)
            {
                r2 = r1 - 1;
                Down(ref b1, ref c1);
            }

            if (s.Compare(r2, t) <= 0)
            {
                b1 = 1;
            }
            else
            {
                s.Write(r1, s.Read(r2));
                r1 = r2;
                Down(ref b1, ref c1);
            }
        }

        if (r1 - r0 != 0)
        {
            s.Write(r1, t);
        }
    }

    /// <summary>
    /// Construct or adjust the partial heap (reconstruct the main heap)
    /// </summary>
    /// <param name="s"></param>
    /// <param name="p"></param>
    /// <param name="b1"></param>
    /// <param name="b"></param>
    /// <param name="c1"></param>
    /// <param name="c"></param>
    /// <param name="r1"></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void Trinkle<T>(SortSpan<T> s, ref int p, ref int b1, ref int b, ref int c1, ref int c, ref int r1) where T : IComparable<T>
    {
        int p1 = p, r0 = r1;
        b1 = b;
        c1 = c;

        var t = s.Read(r0);

        while (p1 > 0)
        {
            while ((p1 & 1) == 0)
            {
                p1 >>= 1;
                Up(ref b1, ref c1);
            }

            var r3 = r1 - b1;

            if ((p1 == 1) || s.Compare(r3, t) <= 0)
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
                    s.Write(r1, s.Read(r3));
                    r1 = r3;
                }
                else
                {
                    if (b1 >= 3)
                    {
                        var r2 = r1 - b1 + c1;
                        if (s.Compare(r1 - 1, r2) > 0)
                        {
                            r2 = r1 - 1;
                            Down(ref b1, ref c1);
                            p1 <<= 1;
                        }

                        // Judge swap or not
                        if (s.Compare(r2, r3) <= 0)
                        {
                            s.Write(r1, s.Read(r3));
                            r1 = r3;
                        }
                        else
                        {
                            s.Write(r1, s.Read(r2));
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
            s.Write(r1, t);
        }

        // final adjustment
        Shift(s, ref r1, ref b1, ref c1);
    }

    /// <summary>
    /// Trinkle part of the heap
    /// </summary>
    /// <param name="s"></param>
    /// <param name="p"></param>
    /// <param name="b1"></param>
    /// <param name="b"></param>
    /// <param name="c1"></param>
    /// <param name="c"></param>
    /// <param name="r1"></param>
    /// <param name="r"></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void SemiTrinkle<T>(SortSpan<T> s, ref int p, ref int b1, ref int b, ref int c1, ref int c, ref int r1, ref int r) where T : IComparable<T>
    {
        r1 = r - c;
        if (s.Compare(r1, r) > 0)
        {
            s.Swap(r, r1);
            Trinkle(s, ref p, ref b1, ref b, ref c1, ref c, ref r1);
        }
    }

    /// <summary>
    /// Upward Leonardo heap step
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void Up(ref int a, ref int b)
    {
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
    private static void Down(ref int a, ref int b)
    {
        var temp = b;
        b = a - b - 1;
        a = temp;
    }
}
