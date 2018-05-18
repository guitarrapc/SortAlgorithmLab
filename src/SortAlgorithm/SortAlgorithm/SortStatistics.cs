using System;
using System.Collections.Generic;
using System.Text;

namespace SortAlgorithm
{
    public struct SortStatics
    {
        public int ArraySize { get; set; }
        public int IndexAccessCount { get; set; }
        public int CompareCount { get; set; }
        public int SwapCount { get; set; }

        public void Reset()
        {
            IndexAccessCount = 0;
            CompareCount = 0;
            SwapCount = 0;
        }

        public void Reset(int arraySize)
        {
            ArraySize = arraySize;
            IndexAccessCount = 0;
            CompareCount = 0;
            SwapCount = 0;
        }

        public void AddIndexAccess()
        {
            IndexAccessCount++;
        }
        public void AddCompareCount()
        {
            CompareCount++;
        }
        public void AddSwapCount()
        {
            SwapCount++;
        }
    }
}
