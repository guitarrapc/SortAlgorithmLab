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
    public override SortMethod SortType => SortMethod.Merging;
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

    public override void Sort(T[] array)
    {
        Statistics.Reset(array.Length, SortType, Name);
        SortCore(array.AsSpan());
        Statistics.AddIndexCount(quickSort.Statistics.IndexAccessCount);
        Statistics.AddCompareCount(quickSort.Statistics.CompareCount);
        Statistics.AddSwapCount(quickSort.Statistics.SwapCount);
        Statistics.AddIndexCount(quickSort2.Statistics.IndexAccessCount);
        Statistics.AddCompareCount(quickSort2.Statistics.CompareCount);
        Statistics.AddSwapCount(quickSort2.Statistics.SwapCount);
    }

    public override void Sort(Span<T> span)
    {
        Statistics.Reset(span.Length, SortType, Name);
        SortCore(span);
        Statistics.AddIndexCount(quickSort.Statistics.IndexAccessCount);
        Statistics.AddCompareCount(quickSort.Statistics.CompareCount);
        Statistics.AddSwapCount(quickSort.Statistics.SwapCount);
        Statistics.AddIndexCount(quickSort2.Statistics.IndexAccessCount);
        Statistics.AddCompareCount(quickSort2.Statistics.CompareCount);
        Statistics.AddSwapCount(quickSort2.Statistics.SwapCount);
    }

    private void SortCore(Span<T> span)
    {
        var droppedInRow = 0;
        var write = 0;
        var read = 0;
        var dropped = new T[span.Length];
        var droppedIndex = 0;

        while (read < span.Length)
        {
            // fallback to QuickSort
            if (EarlyOut
                && read == span.Length / EarlyOutTestAt
                && dropped.Length > (read * EarlyOutDisorderFraction))
            {
                for (var i = 0; i < droppedIndex; i++)
                {
                    Index(span, write + i) = dropped[i];
                }
                var tempArray = span.ToArray();
                quickSort.Sort(tempArray);
                tempArray.AsSpan().CopyTo(span);
                return;
            }

            if (write == 0 || Compare(Index(span, read), Index(span, write - 1)) >= 0)
            {
                // The element is order - keep it:
                Index(span, write) = Index(span, read);
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
                    && Compare(Index(span, read), Index(span, write - 2)) >= 0)
                {
                    // Quick undo: drop previously accepted element, and overwrite with new one:
                    dropped[droppedIndex++] = Index(span, write - 1);
                    Index(span, write - 1) = Index(span, read);
                    read++;
                    continue;
                }

                if (droppedInRow < Recency)
                {
                    // Drop it
                    dropped[droppedIndex++] = Index(span, read);
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
                    droppedIndex -= droppedInRow;
                    read -= droppedInRow;

                    var backTracked = 1;
                    write--;

                    if (FastBackTracking)
                    {
                        // Back-track until we can accept at least one of the recently dropped elements:
                        var maxOfDropped = span.Slice(read, droppedInRow + 1).ToArray().Max()!;
                        while (1 <= write && Compare(maxOfDropped, Index(span, write - 1)) < 0)
                        {
                            backTracked++;
                            write--;
                        }
                    }

                    // Drop the back-tracked elements:
                    for (var i = 0; i < backTracked; i++)
                    {
                        dropped[droppedIndex++] = Index(span, write + i);
                    }
                    droppedInRow = 0;
                }
            }
        }

        // Sort dropped elements
        var droppedArray = dropped.AsSpan(0, droppedIndex).ToArray();
        quickSort2.Sort(droppedArray);

        var back = span.Length;
        var droppedCount = droppedIndex;
        while (droppedCount > 0)
        {
            var lastDropped = droppedArray[droppedCount - 1];
            droppedCount--;

            while (0 < write && Compare(lastDropped, Index(span, write - 1)) < 0)
            {
                Index(span, back - 1) = Index(span, write - 1);
                back--;
                write--;
            }
            Index(span, back - 1) = lastDropped;
            back--;
        }
    }
}
