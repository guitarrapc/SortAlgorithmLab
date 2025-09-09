namespace SortLab.Core.Sortings;

/// <summary>
/// Contains Bug on HeapSort. Not yet working.
/// </summary>
/// <remarks>
/// stable : yes
/// inplace : no (n)
/// Compare : n log n
/// Swap : n log n
/// Order : O(n log n) (Worst case : O(n log n))
/// </remarks>
/// <typeparam name="T"></typeparam>
// refer : https://github.com/WaqasAhmed16B116SE/TimSort/blob/master/Program.cs
public class TimSort<T> : SortBase<T> where T : IComparable<T>
{
    public override SortMethod SortType => SortMethod.Hybrid;
    protected override string Name => nameof(TimSort<T>);

    private BinaryInsertSort<T> insertSort = new BinaryInsertSort<T>();

    private static readonly int defaultMinMerge = 32;
    private static readonly int defaultMinGallop = 7;
    private static readonly int defaultTmpStorageLength = 256;
    private int minGallop = defaultMinGallop;
    private int tmpStorageLength = defaultTmpStorageLength;
    private double[] powerOfThen = new[] { 1e0, 1e1, 1e2, 1e3, 1e4, 1e5, 1e6, 1e7, 1e8, 1e9 };

    private int[] runStart = [];
    private int[] runLength = [];
    private int stackSize = 0;

    private T[] array = [];
    private T[] tmp = [];

    private void Initialize(T[] array)
    {
        this.array = array;
        this.minGallop = defaultMinGallop;
        tmpStorageLength = defaultTmpStorageLength;
        if (array.Length < 2 * defaultTmpStorageLength)
        {
            this.tmpStorageLength = array.Length >> 1;
        }
        this.tmp = new T[this.tmpStorageLength];
        var stackLength = array.Length < 120 ? 5
            : array.Length < 1542 ? 10
            : array.Length < 119151 ? 19
            : 40;
        this.runStart = new int[stackLength];
        this.runLength = new int[stackLength];
    }

    public override T[] Sort(T[] array)
    {
        Statistics.Reset(array.Length, SortType, Name);

        // run
        Initialize(array);
        var result = SortImpl(array, 0, array.Length);
        Statistics.AddCompareCount(insertSort.Statistics.CompareCount);
        Statistics.AddIndexCount(insertSort.Statistics.IndexAccessCount);
        Statistics.AddSwapCount(insertSort.Statistics.SwapCount);
        return result;
    }

    private T[] SortImpl(T[] array, int low, int high)
    {
        var remaining = high - low;
        if (remaining < 2) return array;

        var runLength = 0;
        // small array will use binaryinsertsort
        if (remaining < defaultMinGallop)
        {
            runLength = Ascending(array, low, high);
            insertSort.Sort(array, low, high, low + runLength);
            return array;
        }

        var minRun = MinRunLength(remaining);

        do
        {
            runLength = Ascending(array, low, high);

            // small array will use binaryinsertsort
            if (runLength < minRun)
            {
                var force = remaining > minRun ? minRun : remaining;
                insertSort.Sort(array, low, low + force, low + runLength);
                runLength = force;
            }

            // push new run and merge
            PushRun(low, runLength);
            MergeRun();

            // next run
            remaining -= runLength;
            low += runLength;
        }
        while (remaining != 0);

        // force merge remaining
        ForceMergeRun(array);

        return array;
    }

    private void PushRun(int runStart, int runLength)
    {
        this.runStart[this.stackSize] = runStart;
        this.runLength[this.stackSize] = runLength;
        this.stackSize++;
    }

    private void MergeRun()
    {
        while (this.stackSize > 1)
        {
            var n = this.stackSize - 2;
            if ((n >= 1 && this.runLength[n - 1] <= this.runLength[n] + this.runLength[n + 1]) ||
                (n >= 2 && this.runLength[n - 2] <= this.runLength[n] + this.runLength[n - 1]))
            {
                if (this.runLength[n - 1] < this.runLength[n + 1])
                {
                    n--;
                }
            }
            else if (this.runLength[n] > this.runLength[n + 1])
            {
                break;
            }
            MergeAt(array, n);
        }
    }

    private void ForceMergeRun(T[] array)
    {
        while (this.stackSize > 1)
        {
            var n = this.stackSize - 2;
            if (this.runLength[n - 1] < this.runLength[n + 1])
            {
                n--;
            }
            MergeAt(array, n);
        }
    }

    private void MergeAt(T[] array, int i)
    {
        var start1 = this.runStart[i];
        var length1 = this.runLength[i];
        var start2 = this.runStart[i + 1];
        var length2 = this.runLength[i + 1];

        this.runLength[i] = length1 + length2;

        if (i == this.stackSize - 3)
        {
            this.runStart[i + 1] = this.runStart[i + 2];
            this.runLength[i + 1] = this.runLength[i + 2];
        }

        this.stackSize--;

        var k = GallopRight(array[start2], array, start1, length1, 0);
        start1 += k;
        length1 -= k;
        if (length1 == 0) return;

        length2 = GallopLeft(array[start1 + length1 - 1], array, start2, length2, length2 - 1);
        if (length2 == 0) return;

        if (length1 <= length2)
        {
            MergeLow(array, start1, length1, start2, length2);
        }
        else
        {
            MergeHigh(array, start1, length1, start2, length2);
        }
    }

    private void MergeLow(T[] array, int start1, int length1, int start2, int length2)
    {
        var tmp = this.tmp;

        for (var i = 0; i < length1; i++)
        {
            tmp[i] = array[start1 + i];
        }

        var cursor1 = 0;
        var cursor2 = start2;
        var dest = start1;

        array[dest++] = array[cursor2++];

        if (--length2 == 0)
        {
            for (var i = 0; i < length1; i++)
            {
                array[dest + i] = tmp[cursor1 + i];
            }
            return;
        }

        if (length1 == 1)
        {
            for (var i = 0; i < length2; i++)
            {
                array[dest + i] = array[cursor2 + i];
            }
            array[dest + length2] = tmp[cursor1];
            return;
        }

        var minGallop = this.minGallop;

        while (true)
        {
            var count1 = 0;
            var count2 = 0;
            var exit = false;

            do
            {
                if (Compare(array[cursor2], tmp[cursor1]) < 0)
                {
                    array[dest++] = array[cursor2++];
                    count2++;
                    count1 = 0;

                    if (--length2 == 0)
                    {
                        exit = true;
                        break;
                    }
                }
                else
                {
                    array[dest++] = tmp[cursor1++];
                    count1++;
                    count2 = 0;
                    if (--length1 == 1)
                    {
                        exit = true;
                        break;
                    }
                }
            } while ((count1 | count2) < minGallop);

            if (exit) break;

            do
            {
                count1 = GallopRight(array[cursor2], tmp, cursor1, length1, 0);

                if (count1 != 0)
                {
                    for (var i = 0; i < count1; i++)
                    {
                        array[dest + i] = tmp[cursor1 + i];
                    }

                    dest += count1;
                    cursor1 += count1;
                    length1 -= count1;
                    if (length1 <= 1)
                    {
                        exit = true;
                        break;
                    }
                }

                array[dest++] = array[cursor2++];

                if (--length2 == 0)
                {
                    exit = true;
                    break;
                }

                count2 = GallopLeft(tmp[cursor1], array, cursor2, length2, 0);

                if (count2 != 0)
                {
                    for (var i = 0; i < count2; i++)
                    {
                        array[dest + i] = array[cursor2 + i];
                    }

                    dest += count2;
                    cursor2 += count2;
                    length2 -= count2;

                    if (length2 == 0)
                    {
                        exit = true;
                        break;
                    }
                }
                array[dest++] = tmp[cursor1++];

                if (--length1 == 1)
                {
                    exit = true;
                    break;
                }

                minGallop--;
            } while (count1 >= defaultMinGallop || count2 >= defaultMinGallop);

            if (exit) break;

            if (minGallop < 0)
            {
                minGallop = 0;
            }

            minGallop += 2;
        }

        this.minGallop = minGallop;
        if (minGallop < 1)
        {
            this.minGallop = 1;
        }

        if (length1 == 1)
        {
            for (var i = 0; i < length2; i++)
            {
                array[dest + i] = array[cursor2 + i];
            }
            array[dest + length2] = tmp[cursor1];
        }
        else if (length1 == 0)
        {
            throw new ArgumentException($"{nameof(MergeLow)} precondition not respected.");
        }
        else
        {
            for (var i = 0; i < length1; i++)
            {
                array[dest + i] = tmp[cursor1 + i];
            }
        }
    }

    private void MergeHigh(T[] array, int start1, int length1, int start2, int length2)
    {
        var tmp = this.tmp;

        for (var i = 0; i < length2; i++)
        {
            tmp[i] = array[start2 + i];
        }

        var cursor1 = start1 + length1 - 1;
        var cursor2 = length2 - 1;
        var dest = start2 + length2 - 1;
        var customCursor = 0;
        var customDest = 0;

        array[dest--] = array[cursor1--];

        if (--length1 == 0)
        {
            customCursor = dest - length2 - 1;
            for (var i = 0; i < length2; i++)
            {
                array[customCursor + i] = tmp[i];
            }
            return;
        }

        if (length2 == 1)
        {
            dest -= length1;
            cursor1 -= length1;
            customDest = dest + 1;
            customCursor = cursor1 + 1;

            for (var i = length1 - 1; i >= 0; i--)
            {
                array[customDest + i] = array[customDest + i];
            }
            array[dest] = tmp[cursor2];
            return;
        }

        var minGallop = this.minGallop;

        while (true)
        {
            var count1 = 0;
            var count2 = 0;
            var exit = false;

            do
            {
                if (Compare(tmp[cursor2], array[cursor1]) < 0)
                {
                    array[dest--] = array[cursor1--];
                    count1++;
                    count2 = 0;

                    if (--length1 == 0)
                    {
                        exit = true;
                        break;
                    }
                }
                else
                {
                    array[dest--] = tmp[cursor2--];
                    count2++;
                    count1 = 0;
                    if (--length2 == 1)
                    {
                        exit = true;
                        break;
                    }
                }
            } while ((count1 | count2) < minGallop);


            if (exit) break;

            do
            {
                count1 = length1 - GallopRight(array[cursor2], array, start1, length1, length1 - 1);

                if (count1 != 0)
                {
                    dest -= count1;
                    cursor1 -= count1;
                    length1 -= count1;
                    customDest = dest + 1;
                    customCursor = cursor1 + 1;

                    for (var i = count1 - 1; i >= 0; i--)
                    {
                        array[customDest + i] = tmp[customCursor + i];
                    }

                    if (length1 == 0)
                    {
                        exit = true;
                        break;
                    }
                }

                array[dest--] = array[cursor2--];

                if (--length2 == 1)
                {
                    exit = true;
                    break;
                }

                count2 = length2 - GallopLeft(array[cursor1], tmp, 0, length2, length2 - 1);

                if (count2 != 0)
                {
                    dest -= count2;
                    cursor2 -= count2;
                    length2 -= count2;
                    customDest = dest + 1;
                    customCursor = cursor2 + 1;

                    for (var i = 0; i < count2; i++)
                    {
                        array[customDest + i] = array[customCursor + i];
                    }

                    if (length2 <= 1)
                    {
                        exit = true;
                        break;
                    }
                }
                array[dest--] = tmp[cursor1--];

                if (--length1 == 0)
                {
                    exit = true;
                    break;
                }

                minGallop--;
            } while (count1 >= defaultMinGallop || count2 >= defaultMinGallop);


            if (exit) break;

            if (minGallop < 0)
            {
                minGallop = 0;
            }

            minGallop += 2;
        }

        this.minGallop = minGallop;
        if (minGallop < 1)
        {
            this.minGallop = 1;
        }

        if (length2 == 1)
        {
            dest -= length1;
            cursor1 -= length1;
            customDest = dest + 1;
            customCursor = cursor1 + 1;

            for (var i = length1 - 1; i >= 0; i--)
            {
                array[customDest + i] = array[customCursor + i];
            }
            array[dest] = tmp[cursor2];
        }
        else if (length2 == 0)
        {
            throw new ArgumentException($"{nameof(MergeHigh)} precondition not respected.");
        }
        else
        {
            customCursor = dest - length2 - 1;
            for (var i = 0; i < length2; i++)
            {
                array[customCursor + i] = tmp[i];
            }
        }
    }

    private int Ascending(T[] array, int low, int high)
    {
        var runHigh = low + 1;
        if (runHigh == high) return 1;

        if (Compare(array[runHigh++], array[low]) < 0)
        {
            // decending
            while (runHigh < high && Compare(array[runHigh], array[runHigh - 1]) < 0)
            {
                runHigh++;
            }
            Reverse(array, low, runHigh);
        }
        else
        {
            // ascending
            while (runHigh < high && Compare(array[runHigh], array[runHigh - 1]) >= 0)
            {
                runHigh++;
            }
        }

        return runHigh - low;
    }

    private void Reverse(T[] array, int low, int high)
    {
        high--;

        while (low < high)
        {
            var t = array[low];
            array[low++] = array[high];
            array[high--] = t;
        }
    }

    private int GallopRight(T value, T[] array, int start, int length, int hint)
    {
        var lastOffset = 0;
        var maxOffset = 0;
        var offset = 1;

        if (Compare(value, array[start + hint]) < 0)
        {
            maxOffset = hint + 1;

            while (offset < maxOffset && Compare(value, array[start + hint - offset]) < 0)
            {
                lastOffset = offset;
                offset = (offset << 1) + 1;

                if (offset <= 0)
                {
                    offset = maxOffset;
                }
            }

            if (offset > maxOffset)
            {
                offset = maxOffset;
            }

            // make offset relative from start
            var temp = lastOffset;
            lastOffset = hint - offset;
            offset = hint - temp;
        }
        else
        {
            maxOffset = length - hint;
            while (offset < maxOffset && Compare(value, array[start + hint + offset]) >= 0)
            {
                lastOffset = offset;
                offset = (offset << 1) + 1;

                if (offset <= 0)
                {
                    offset = maxOffset;
                }
            }

            if (offset > maxOffset)
            {
                offset = maxOffset;
            }

            // make offset relative from start
            lastOffset += hint;
            offset = +hint;
        }

        // current guarantee : array[start+lastOffset] < value <= array[start+offset]
        // BinarySearch value from array[start + lastOffset - 1] < value <= array[start + offset]
        //BinarySearch(ref array, value, start + lastOffset - 1, start + offset);
        lastOffset++;
        while (lastOffset < offset)
        {
            var m = lastOffset + (UnsignedRightShift((offset - lastOffset), 1));
            if (Compare(value, array[start + m]) < 0)
            {
                offset = m;
            }
            else
            {
                lastOffset = m + 1;
            }
        }
        return offset;
    }

    private int GallopLeft(T value, T[] array, int start, int length, int hint)
    {
        var lastOffset = 0;
        var maxOffset = 0;
        var offset = 1;

        if (Compare(value, array[start + hint]) > 0)
        {
            maxOffset = length - hint;

            while (offset < maxOffset && Compare(value, array[start + hint + offset]) > 0)
            {
                lastOffset = offset;
                offset = (offset << 1) + 1;

                if (offset <= 0)
                {
                    offset = maxOffset;
                }
            }

            if (offset > maxOffset)
            {
                offset = maxOffset;
            }

            // make offset relative from start
            lastOffset += hint;
            offset += hint;
        }
        else
        {
            maxOffset = hint + 1;
            while (offset < maxOffset && Compare(value, array[start + hint - offset]) <= 0)
            {
                lastOffset = offset;
                offset = (offset << 1) + 1;

                if (offset <= 0)
                {
                    offset = maxOffset;
                }
            }

            if (offset > maxOffset)
            {
                offset = maxOffset;
            }

            // make offset relative from start
            var tmp = lastOffset;
            lastOffset = hint - offset;
            offset = hint - tmp;
        }

        // current guarantee : array[start+lastOffset] < value <= array[start+offset]
        // BinarySearch value from array[start + lastOffset - 1] < value <= array[start + offset]
        //BinarySearch(ref array, value, start + lastOffset - 1, start + offset);
        lastOffset++;
        while (lastOffset < offset)
        {
            var m = lastOffset + (UnsignedRightShift((offset - lastOffset), 1));
            if (Compare(value, array[start + m]) > 0)
            {
                lastOffset = m + 1;
            }
            else
            {
                offset = m;
            }
        }
        return offset;
    }

    private int MinRunLength(int n)
    {
        var r = 0;
        while (n >= defaultMinMerge)
        {
            r |= (n & 1);
            n >>= 1;
        }
        return n + r;
    }
}
