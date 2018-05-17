using System;
using System.Collections.Generic;
using System.Text;

namespace SortAlgorithm
{
    public class SortStatics
    {
        public int ArraySize { get; set; }
        public int IndexAccessCount { get; set; }
        public int SwapCount { get; set; }

        public SortStatics(int arraySize)
        {
            ArraySize = arraySize;
        }
        public void Reset()
        {
            IndexAccessCount = 0;
            SwapCount = 0;
        }

        public void AddIndexAccess()
        {
            IndexAccessCount++;
        }
        public void AddSwapCount()
        {
            SwapCount++;
        }
    }
}
