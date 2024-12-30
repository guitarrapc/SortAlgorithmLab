namespace SortLab.Core.Sortings;

/// <summary>
/// Merge Sortの類似で、ソート済みの場合に高速に処理がなされる不安定な外部ソート。特に80%以上がソート済みだと高速。10k以上の長大な配列のソートの場合は、QuickSortよりも2-5倍高速になることがある。
/// 以下の条件下で高速
/// * Less than 20-30% of the elements out of order AND these are randomly distributed in the data (not clumped).
/// * You have a lot of data (10k elements or more, and definitively when you get into the millions).
/// </summary>
/// <remarks>
/// stable : no
/// inplace : no (O((K))
/// Compare : O(N + K⋅log(K))
/// Swap : n log2 n
/// Order : O(n log n) (Best case : O(n), Worst case : O(n log n))
/// </remarks>
/// <typeparam name="T"></typeparam>
// ref : https://github.com/emilk/drop-merge-sort
// ref : https://github.com/Jooraz/js-drop-merge-sort
public class DropMergeSort<T> : SortBase<T> where T : IComparable<T>
{
    // refer: https://github.com/JamesQuintero/ShiftSort
    public override SortMethod Method => SortMethod.Merging;
    protected override string Name => nameof(DropMergeSort<T>);

    private QuickSortMedian9WithBinaryInsert<T> quickSort = new QuickSortMedian9WithBinaryInsert<T>();
    private QuickSortMedian9WithBinaryInsert<T> quickSort2 = new QuickSortMedian9WithBinaryInsert<T>();

    /// This speeds up well-ordered input by quite a lot.
    const bool DoubleComparisons = true;
    /// The algorithms uses recency=8 which means it can handle no more than 8 outliers in a row.
    /// This number was chosen by experimentation, and could perhaps be adjusted dynamically for increased performance.
    /// * Low RECENCY = faster when there is low disorder (a lot of order).
    /// * High RECENCY = more resilient against long stretches of noise.
    /// If RECENCY is too small we are more dependent on nice data/luck.
    const int Recency = 8;
    /// Back-track several elements at once. This is helpful when there are big clumps out-of-order.
    const bool FastBackTracking = true;
    /// Break early if we notice that the input is not ordered enough.
    const bool EarlyOut = true;
    /// Test for early-out when we have processed len / EARLY_OUT_TEST_AT elements.
    const int EarlyOutTestAt = 4;
    /// If more than this percentage of elements have been dropped, we abort.
    const double EarlyOutDisorderFraction = 0.6;

    public override T[] Sort(T[] array)
    {
        Statistics.Reset(array.Length, Method, Name);
        var a = SortImpl(array);
        Statistics.AddIndexCount(quickSort.Statistics.IndexAccessCount);
        Statistics.AddCompareCount(quickSort.Statistics.CompareCount);
        Statistics.AddSwapCount(quickSort.Statistics.SwapCount);
        Statistics.AddIndexCount(quickSort2.Statistics.IndexAccessCount);
        Statistics.AddCompareCount(quickSort2.Statistics.CompareCount);
        Statistics.AddSwapCount(quickSort2.Statistics.SwapCount);
        return a;
    }

    private T[] SortImpl(T[] array)
    {
        var droppedInRow = 0;
        var write = 0;
        var read = 0;
        var dropped = new T[array.Length];
        var droppedIndex = 0;

        while (read < array.Length)
        {
            Statistics.AddIndexCount();

            // fallback to QuickSort
            if (EarlyOut
                && read == array.Length / EarlyOutTestAt
                && dropped.Length > (read * EarlyOutDisorderFraction))
            {
                for (var i = 0; i < droppedIndex; i++)
                {
                    array[write + i] = dropped[i];
                }
                return quickSort.Sort(array);
            }

            if (write == 0 || Compare(array[read], array[write - 1]) >= 0)
            {
                // The element is order - keep it:
                array[write] = array[read];
                read++;
                write++;
                droppedInRow = 0;
            }
            else
            {
                // The next element is smaller than the last stored one.
                // The question is - should we drop the new element, or was accepting the previous element a mistake?

                /*
                    Check this situation:
                    0 1 2 3 9 5 6 7  (the 9 is a one-off)
                            | |
                            | read
                            write - 1
                    Checking this improves performance because we catch common problems earlier (without back-tracking).
                */

                if (DoubleComparisons
                    && droppedInRow == 0
                    && 2 <= write
                    && Compare(array[read], array[write - 2]) >= 0)
                {
                    Statistics.AddSwapCount();
                    // Quick undo: drop previously accepted element, and overwrite with new one:
                    dropped[droppedIndex++] = array[write - 1];
                    //dropped.push(prev);
                    array[write - 1] = array[read];
                    read++;
                    continue;
                }

                if (droppedInRow < Recency)
                {
                    // Drop it
                    dropped[droppedIndex++] = array[read];
                    // dropped.push(slice[read]);
                    read++;
                    droppedInRow++;
                }
                else
                {
                    /*
                    We accepted something num_dropped_in_row elements back that made us drop all RECENCY subsequent items.
                    Accepting that element was obviously a mistake - so let's undo it!

                    Example problem (RECENCY = 3):    0 1 12 3 4 5 6
                        0 1 12 is accepted. 3, 4, 5 will be rejected because they are larger than the last kept item (12).
                        When we get to 5 we reach num_dropped_in_row == RECENCY.
                        This will trigger an undo where we drop the 12.
                        When we again go to 3, we will keep it because it is larger than the last kept item (1).

                    Example worst-case (RECENCY = 3):   ...100 101 102 103 104 1 2 3 4 5 ....
                        100-104 is accepted. When we get to 3 we reach num_dropped_in_row == RECENCY.
                        We drop 104 and reset the read by RECENCY. We restart, and then we drop again.
                        This can lead us to backtracking RECENCY number of elements
                        as many times as the leading non-decreasing subsequence is long.
                    */

                    // Undo dropping the last num_dropped_in_row elements:
                    var truncToLength = dropped.Length - droppedInRow;
                    //dropped = dropped.GetRange(0, droppedInRow);
                    droppedIndex -= droppedInRow;
                    read -= droppedInRow;

                    var backTracked = 1;
                    write--;

                    if (FastBackTracking)
                    {
                        // Back-track until we can accept at least one of the recently dropped elements:
                        var maxOfDropped = array.AsSpan(read, read + droppedInRow + 1).ToArray().Max();
                        // while (1 <= write && max_of_dropped < array[write - 1]) {
                        while (1 <= write && Compare(maxOfDropped, array[write - 1]) < 0)
                        {
                            backTracked++;
                            write--;
                        }
                    }

                    // Optimized for C# to not change size of array
                    // Drop the back-tracked elements:
                    for (var i = 0; i < backTracked; i++)
                    {
                        dropped[droppedIndex++] = array[write + i];
                    }
                    droppedInRow = 0;
                }
            }
        }

        // Optimized for C# to not change size of array.
        // Drop the back-tracked elements:
        dropped = dropped.AsSpan(0, droppedIndex).ToArray();
        dropped = quickSort2.Sort(dropped);

        var back = array.Length;
        while (dropped.Length > 0)
        {
            // C# alternate Pop implementation.
            var lastDropped = dropped[dropped.Length - 1];
            dropped = dropped.AsSpan(0, dropped.Length - 1).ToArray();
            // let last_dropped = dropped.pop();

            while (0 < write && Compare(lastDropped, array[write - 1]) < 0)
            {
                Statistics.AddSwapCount();
                array[back - 1] = array[write - 1];
                back--;
                write--;
            }
            array[back - 1] = lastDropped;
            back--;
        }

        return array;
    }
}
