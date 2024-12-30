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
    public override SortMethod Method => SortMethod.Distributed;
    protected override string Name => nameof(CountingSort<T>);

    public override int[] Sort(int[] array)
    {
        Statistics.Reset(array.Length, Method, Name);
        if (array.Min() >= 0)
        {
            return SortImplPositive(array);
        }
        else
        {
            return SortImplNegative(array);
        }
    }

    private int[] SortImplPositive(int[] array)
    {
        var min = 0;
        var max = 0;

        for (var i = 1; i < array.Length; i++)
        {
            Statistics.AddIndexCount();
            if (array[i] < min)
            {
                min = array[i];
            }
            else if (array[i] > max)
            {
                max = array[i];
            }
        }

        var resultArray = new int[array.Length];
        var countArray = new int[max - min + 1 + 1];

        // count up each number of element to countArray
        for (var i = 0; i < array.Length; i++)
        {
            Statistics.AddIndexCount();
            ++countArray[array[i]];
        }

        // change current index element counter by adding previous index counter.
        for (var i = 1; i < countArray.Length; i++)
        {
            Statistics.AddIndexCount();
            countArray[i] += countArray[i - 1];
        }

        // set countArrayed index element into resultArray, then decrement countArray.
        for (var i = 0; i < array.Length; i++)
        {
            Statistics.AddIndexCount();
            resultArray[countArray[array[i]] - 1] = array[i];
            --countArray[array[i]];
        }

        return resultArray;
    }
    private int[] SortImplNegative(int[] array)
    {
        var max = -1;
        for (var i = 0; i < array.Length; i++)
        {
            Statistics.AddIndexCount();
            if (Math.Abs(array[i]) > max)
            {
                Statistics.AddIndexCount();
                max = Math.Abs(array[i]);
            }
        }
        var stack = new int[max * 2 + 1];

        for (var i = 0; i < array.Length; i++)
        {
            Statistics.AddIndexCount();
            stack[array[i] + max]++;
        }

        var j = stack.Length - 1;
        var k = array.Length - 1;
        while (k >= 0)
        {
            Statistics.AddIndexCount();
            if (stack[j] > 0)
            {
                Statistics.AddIndexCount();
                stack[j]--;
                array[k] = j - max;
                k--;
            }
            else
            {
                j--;
            }
        }
        return array;
    }
}
