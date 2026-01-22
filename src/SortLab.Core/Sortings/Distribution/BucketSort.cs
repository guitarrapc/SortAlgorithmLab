using System.Runtime.CompilerServices;

namespace SortLab.Core.Sortings;

/// <summary>
/// 値の範囲に応じたバケットを用意し、各要素をキーに基づいてバケットに配置します。
/// 最後にバケットの中身を結合すればソートが完了します。
/// 比較演算を行わない安定なソートアルゴリズムです。
/// </summary>
/// <remarks>
/// stable  : yes
/// inplace : no (n + k where k = range of keys)
/// Compare : 0        (No comparison operations, only key extraction)
/// Swap    : 0
/// Order   : O(n + k) where k is the range of keys (Worst case: O(n^2) if range is too large)
/// Note    : 値の範囲が大きいとメモリ使用量が膨大になります。
/// </remarks>
/// <typeparam name="T">ソート対象の要素型</typeparam>
public class BucketSort<T>(Func<T, int> keySelector) : SortBase<T> where T : IComparable<T>
{
    private const int MaxBucketSize = 10_000_000; // Maximum allowed bucket array size

    public override SortMethod SortType => SortMethod.Distributed;
    protected override string Name => nameof(BucketSort<T>);

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
        if (span.Length <= 1) return;

        // Cache keys and find min/max in single pass
        var keys = new int[span.Length];
        var min = int.MaxValue;
        var max = int.MinValue;

        for (var i = 0; i < span.Length; i++)
        {
            var key = keySelector(Index(span, i));
            keys[i] = key;
            if (key < min) min = key;
            if (key > max) max = key;
        }

        // Check for overflow and validate range
        long range = (long)max - (long)min + 1;
        if (range > int.MaxValue)
        {
            throw new ArgumentException(
                $"Key range is too large for BucketSort: {range}. Maximum supported range is {int.MaxValue}.");
        }
        if (range > MaxBucketSize)
        {
            throw new ArgumentException(
                $"Key range ({range}) exceeds maximum bucket size ({MaxBucketSize}). Consider using QuickSort or another comparison-based sort.");
        }

        var offset = -min; // Offset to normalize keys to 0-based index
        var size = (int)range;

        // Count elements per bucket
        var bucketCounts = new int[size];
        for (var i = 0; i < span.Length; i++)
        {
            bucketCounts[keys[i] + offset]++;
        }

        // Create buckets with pre-allocated capacity
        var bucket = new List<T>[size];
        for (var i = 0; i < size; i++)
        {
            if (bucketCounts[i] > 0)
            {
                bucket[i] = new List<T>(bucketCounts[i]);
            }
        }

        // Fill buckets using cached keys
        for (var i = 0; i < span.Length; i++)
        {
            var key = keys[i] + offset;
            bucket[key].Add(Index(span, i));
        }

        // Write back to span
        for (int j = 0, i = 0; j < bucket.Length; j++)
        {
            if (bucket[j] != null)
            {
                foreach (var item in bucket[j])
                {
                    Index(span, i++) = item;
                }
            }
        }
    }
}

/// <summary>
/// 整数専用のバケットソート。
/// 値の範囲を複数のバケットに分割し、各バケット内をソートしてから結合します。
/// 値の分布が均等な場合に高速に動作します。
/// </summary>
/// <remarks>
/// stable : yes (バケット内のソートが安定な場合)
/// inplace : no (バケット用のメモリが必要)
/// Compare : O(n log(n/k)) where k is the number of buckets
/// Swap : バケット内のソート次第
/// Order : O(n + k) on average, O(n^2) worst case (when all elements go to one bucket)
/// Note: バケット数は自動的に調整されますが、最適なバケット数は入力データに依存します。
/// </remarks>
public class BucketSort : SortBase<int>
{
    private const int MaxBucketCount = 1000; // Maximum number of buckets
    private const int MinBucketCount = 2;    // Minimum number of buckets

    public override SortMethod SortType => SortMethod.Distributed;
    protected override string Name => nameof(BucketSort);

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

        // Find min and max
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

        // Determine bucket count based on input size and range
        long range = (long)max - (long)min + 1;
        
        // Calculate optimal bucket count (sqrt(n) is a common heuristic)
        var bucketCount = Math.Max(MinBucketCount, Math.Min(MaxBucketCount, (int)Math.Sqrt(span.Length)));
        
        // Adjust bucket count if range is smaller
        if (range < bucketCount)
        {
            bucketCount = (int)range;
        }

        // Calculate bucket size (range divided by bucket count)
        var bucketSize = Math.Max(1, (range + bucketCount - 1) / bucketCount);

        // Create buckets
        var buckets = new List<int>[bucketCount];
        for (var i = 0; i < bucketCount; i++)
        {
            buckets[i] = new List<int>();
        }

        // Distribute elements into buckets
        for (var i = 0; i < span.Length; i++)
        {
            var value = Index(span, i);
            var bucketIndex = (int)((value - min) / bucketSize);
            
            // Handle edge case where value == max
            if (bucketIndex >= bucketCount)
            {
                bucketIndex = bucketCount - 1;
            }
            
            buckets[bucketIndex].Add(value);
        }

        // Sort each bucket using insertion sort (stable and efficient for small arrays)
        foreach (var bucket in buckets)
        {
            if (bucket.Count > 1)
            {
                InsertionSortBucket(bucket);
            }
        }

        // Concatenate buckets back to span
        for (int j = 0, i = 0; j < bucketCount; j++)
        {
            foreach (var value in buckets[j])
            {
                Index(span, i++) = value;
            }
        }
    }

    /// <summary>
    /// Insertion sort for bucket contents (stable sort)
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void InsertionSortBucket(List<int> bucket)
    {
        for (var i = 1; i < bucket.Count; i++)
        {
            var key = bucket[i];
            var j = i - 1;

            while (j >= 0 && Compare(bucket[j], key) > 0)
            {
                bucket[j + 1] = bucket[j];
                j--;
            }
            bucket[j + 1] = key;
        }
    }
}
