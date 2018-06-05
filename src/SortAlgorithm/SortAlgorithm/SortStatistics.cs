using System;
using System.Collections.Generic;
using System.Text;

namespace SortAlgorithm
{
    public class SortStatics
    {
        public int ArraySize { get; set; }
        public int IndexAccessCount { get; set; }
        public int CompareCount { get; set; }
        public int SwapCount { get; set; }
        public bool IsSorted { get; set; }

        public void Reset()
        {
            IndexAccessCount = 0;
            CompareCount = 0;
            SwapCount = 0;
            IsSorted = false;
        }

        public void Reset(int arraySize)
        {
            ArraySize = arraySize;
            IndexAccessCount = 0;
            CompareCount = 0;
            SwapCount = 0;
            IsSorted = false;
        }

        public void AddIndexAccess()
        {
            IndexAccessCount++;
        }
        public void AddIndexAccess(int count)
        {
            IndexAccessCount += count;
        }
        public void AddCompareCount()
        {
            CompareCount++;
        }
        public void AddCompareCount(int count)
        {
            CompareCount += count;
        }
        public void AddSwapCount()
        {
            SwapCount++;
        }
        public void AddSwapCount(int count)
        {
            SwapCount += count;
        }
    }
}
