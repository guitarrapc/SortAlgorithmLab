namespace SortLab.Core.Sortings;

/// <summary>
/// 値の分布状況を数え上げることを利用してインデックスを導きソートする。バケットソート同様に、とりえる値の範囲を知っていないといけないことと、カウントと結果用の配列分メモリを食う。
/// </summary>
/// <remarks>
/// stable : yes
/// inplace : no (n + r)
/// Compare : 0
/// Swap : 0
/// Order : O(n + k) (k = helper array = counting + result)
/// </remarks>
/// <typeparam name="T"></typeparam>

public class CountingSort<T> : SortBase<int> where T : IComparable<T>
{
    public override SortMethod SortType => SortMethod.Distributed;
    protected override string Name => nameof(CountingSort<T>);

    public override void Sort(int[] array)
    {
        Statistics.Reset(array.Length, SortType, Name);
        SortCore(array.AsSpan());
    }

    public override void Sort(Span<int> span)
    {
        Statistics.Reset(span.Length, SortType, Name);
        SortCore(span);
    }

    private void SortCore(Span<int> span)
    {
        if (span.Length <= 1) return;

        // Find min to determine if we need negative handling
        var hasNegative = false;
        for (var i = 0; i < span.Length; i++)
        {
            if (Index(span, i) < 0)
            {
                hasNegative = true;
                break;
            }
        }

        if (hasNegative)
        {
            SortCoreNegative(span);
        }
        else
        {
            SortCorePositive(span);
        }
    }

    private void SortCorePositive(Span<int> span)
    {
        var min = int.MaxValue;
        var max = int.MinValue;

        for (var i = 0; i < span.Length; i++)
        {
            var value = Index(span, i);
            if (value < min) min = value;
            if (value > max) max = value;
        }

        var resultArray = new int[span.Length];
        var countArray = new int[max - min + 2];

        // Count up each number of element to countArray
        for (var i = 0; i < span.Length; i++)
        {
            countArray[Index(span, i) - min]++;
        }

        // Change current index element counter by adding previous index counter
        for (var i = 1; i < countArray.Length; i++)
        {
            countArray[i] += countArray[i - 1];
        }

        // Set countArrayed index element into resultArray, then decrement countArray
        for (var i = span.Length - 1; i >= 0; i--)
        {
            var value = Index(span, i);
            resultArray[countArray[value - min] - 1] = value;
            countArray[value - min]--;
        }

        // Copy back to span
        for (var i = 0; i < span.Length; i++)
        {
            Index(span, i) = resultArray[i];
        }
    }

    private void SortCoreNegative(Span<int> span)
    {
        var max = -1;
        for (var i = 0; i < span.Length; i++)
        {
            var absValue = Math.Abs(Index(span, i));
            if (absValue > max)
            {
                max = absValue;
            }
        }

        var stack = new int[max * 2 + 1];

        for (var i = 0; i < span.Length; i++)
        {
            stack[Index(span, i) + max]++;
        }

        var j = stack.Length - 1;
        var k = span.Length - 1;
        while (k >= 0)
        {
            if (stack[j] > 0)
            {
                stack[j]--;
                Index(span, k) = j - max;
                k--;
            }
            else
            {
                j--;
            }
        }
    }
}
