namespace SortLab.Core.Sortings;

/// <summary>
/// 値の分布状況を数え上げることを利用してインデックスを導きソートします。
/// 各値の出現回数をカウントし、累積和を計算して正しい位置に配置する安定なソートアルゴリズムです。
/// 値の範囲が狭い場合に非常に高速ですが、範囲が広いとメモリを大量に消費します。
/// </summary>
/// <remarks>
/// stable  : yes
/// inplace : no (n + k where k = range of values)
/// Compare : 0        (No comparison operations)
/// Swap    : 0
/// Order   : O(n + k) where k is the range of values
/// Note    : 値の範囲が大きいとメモリ使用量が膨大になります。
/// </remarks>
public class CountingSort : SortBase<int>
{
    private const int MaxCountArraySize = 10_000_000; // Maximum allowed count array size

    public override SortMethod SortType => SortMethod.Distributed;
    protected override string Name => nameof(CountingSort);

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

        // Find min and max to determine range
        var min = int.MaxValue;
        var max = int.MinValue;

        for (var i = 0; i < span.Length; i++)
        {
            var value = Index(span, i);
            if (value < min) min = value;
            if (value > max) max = value;
        }

        // If all elements are the same, no need to sort
        if (min == max) return;

        // Check for overflow and validate range
        long range = (long)max - (long)min + 1;
        if (range > int.MaxValue)
            throw new ArgumentException($"Value range is too large for CountingSort: {range}. Maximum supported range is {int.MaxValue}.");
        if (range > MaxCountArraySize)
            throw new ArgumentException($"Value range ({range}) exceeds maximum count array size ({MaxCountArraySize}). Consider using QuickSort or another comparison-based sort.");

        var offset = -min; // Offset to normalize values to 0-based index
        var size = (int)range;

        // Create count array
        var countArray = new int[size];

        // Count occurrences of each value
        for (var i = 0; i < span.Length; i++)
        {
            countArray[Index(span, i) + offset]++;
        }

        // Calculate cumulative counts (for stable sort)
        for (var i = 1; i < countArray.Length; i++)
        {
            countArray[i] += countArray[i - 1];
        }

        // Build result using Span for proper index tracking
        var resultSpan = new int[span.Length].AsSpan();
        
        // Build result array in reverse order to maintain stability
        for (var i = span.Length - 1; i >= 0; i--)
        {
            var value = Index(span, i);
            var index = value + offset;
            Index(resultSpan, countArray[index] - 1) = value;
            countArray[index]--;
        }

        // Copy back to span
        for (var i = 0; i < span.Length; i++)
        {
            Index(span, i) = Index(resultSpan, i);
        }
    }
}
