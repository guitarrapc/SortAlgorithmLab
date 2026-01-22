using SortLab.Core.Contexts;

namespace SortLab.Core.Algorithms;

/// <summary>
/// ノームソートアルゴリズム、位置を記憶する最適化版。
/// 前回の位置を記憶することで、リストの先頭に戻る際の無駄な移動を削減します。この最適化により、挿入ソートと同程度の計算量を実現します。
/// ノームソートは「庭師のソート (Stupid sort)」とも呼ばれ、庭師が鉢植えを一つずつ並べ替えるように動作します。隣接要素を逐次的にスワップし、一歩ずつ後退しながら正しい位置を見つけるというシンプルなアルゴリズムです。
/// ノームソートは隣接要素の逐次的なスワップが本質です。これをシフト操作に変えるとInsertionSortになってしまいます。
/// <br/>
/// Gnome sort algorithm - optimized version with position memory.
/// By remembering the previous position, it reduces unnecessary movements when returning to the front.　This optimization achieves computational complexity comparable to insertion sort.
/// Gnome sort, also known as "Stupid sort," works like a garden gnome arranging flower pots one by one.　It is a simple algorithm that swaps adjacent elements sequentially and moves backward step by step to find the correct position.
/// The essence of gnome sort is sequential swapping of adjacent elements.　If you change this to shift operations, it becomes insertion sort.
/// </summary>
/// <remarks>
/// family  : insertion
/// stable  : yes
/// inplace : yes
/// Compare : O(n^2)  (Best case: O(n) when already sorted)
/// Swap    : O(n^2)  (Best case: 0 when already sorted)
/// Index   : O(n^2)
/// Order   : O(n^2)  (Best case: O(n))
/// </remarks>
public static class GnomeSort
{
    public static void Sort<T>(Span<T> span) where T : IComparable<T>
    {
        Sort(span, NullContext.Default);
    }

    public static void Sort<T>(Span<T> span, ISortContext context) where T : IComparable<T>
    {
        if (span.Length <= 1) return;

        var s = new SortSpan<T>(span, context);

        for (var i = 0; i < s.Length; i++)
        {
            while (i > 0 && s.Compare(i - 1, i) > 0)
            {
                s.Swap(i - 1, i);
                i--;
            }
        }
    }
}

/// <summary>
/// ノームソートアルゴリズム - 単純なwhileループ実装。
/// 最も基本的なノームソートの実装で、位置記憶などの最適化は行いません。
/// 実装は非常にシンプルですが、パフォーマンスは劣ります。
/// ノームソートは「庭師のソート (Stupid sort)」とも呼ばれ、庭師が鉢植えを一つずつ並べ替えるように動作します。隣接要素を逐次的にスワップし、一歩ずつ後退しながら正しい位置を見つけるというシンプルなアルゴリズムです。
/// ノームソートは隣接要素の逐次的なスワップが本質です。これをシフト操作に変えるとInsertionSortになってしまいます。
/// <br/>
/// Gnome sort algorithm - simple while loop implementation.
/// The most basic gnome sort implementation without optimizations like position memory.
/// Very simple implementation but with inferior performance.
/// Gnome sort, also known as "Stupid sort," works like a garden gnome arranging flower pots one by one.　It is a simple algorithm that swaps adjacent elements sequentially and moves backward step by step to find the correct position.
/// The essence of gnome sort is sequential swapping of adjacent elements.　If you change this to shift operations, it becomes insertion sort.
/// </summary>
/// <remarks>
/// family  : insertion
/// stable  : yes
/// inplace : yes
/// Compare : O(n^2)  (Best case: O(n) when already sorted)
/// Swap    : O(n^2)  (Best case: 0 when already sorted)
/// Index   : O(n^2)
/// Order   : O(n^2)  (Best case: O(n))
/// </remarks>
public static class GnomeSortNonOptimized
{
    public static void Sort<T>(Span<T> span) where T : IComparable<T>
    {
        Sort(span, NullContext.Default);
    }

    public static void Sort<T>(Span<T> span, ISortContext context) where T : IComparable<T>
    {
        if (span.Length <= 1) return;

        var s = new SortSpan<T>(span, context);

        var i = 0;
        while (i < s.Length)
        {
            if (i == 0 || s.Compare(i - 1, i) <= 0)
            {
                i++;
            }
            else
            {
                s.Swap(i, i - 1);
                --i;
            }
        }
    }
}
