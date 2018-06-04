using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SortAlgorithm.Logics
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// sortKind : HeapSort, ArraySize : 100, IndexAccessCount : 714, CompareCount : 1063, SwapCount : 520
    /// </remarks>
    /// <typeparam name="T"></typeparam>

    public class HeapSort<T> : SortBase<T> where T : IComparable<T>
    {
        public override T[] Sort(T[] array)
        {
            base.sortStatics.Reset(array.Length);

            var i = 0;
            // create heap node
            while (i < array.Length)
            {
                UpHeap(array, i++);
            }
            // pick root and re-heap.
            while (--i > 0)
            {
                // move Max Heap to sorted array
                Swap(ref array[0], ref array[i]);
                // re-heap
                DownHeap(array, i - 1);
            }
            return array;
        }

        // compare and move it to upward if larger or equal.
        private void UpHeap(T[] array, int current)
        {
            while (current != 0)
            {
                base.sortStatics.AddIndexAccess();
                var parent = (current - 1) / 2;

                base.sortStatics.AddCompareCount();
                if (array[current].CompareTo(array[parent]) > 0)
                {
                    base.sortStatics.AddSwapCount();
                    Swap(ref array[current], ref array[parent]);
                    current = parent;
                }
                else
                {
                    break;
                }
            }
        }

        // pick root item to sorted array. then pick last heap item to root then start compare with heap node.
        private void DownHeap(T[] array, int current)
        {
            if (current == 0) return;
            var parent = 0;
            while (true)
            {
                base.sortStatics.AddIndexAccess();
                var child = 2 * parent + 1;
                if (child > current) break;
                base.sortStatics.AddCompareCount();
                if (child < current && array[child].CompareTo(array[child + 1]) < 0)
                {
                    child++;
                }
                base.sortStatics.AddCompareCount();
                if (array[parent].CompareTo(array[child]) < 0)
                {
                    base.sortStatics.AddSwapCount();
                    Swap(ref array[parent], ref array[child]);
                    parent = child;
                }
                else
                {
                    break;
                }
            }
        }
    }
}
