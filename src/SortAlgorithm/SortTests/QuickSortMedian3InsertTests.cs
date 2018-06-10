using SortAlgorithm;
using SortAlgorithm.Logics;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace SortTests
{
    public class QuickSortMedian3InsertTests
    {
        private ISort<int> sort;
        private string algorithm;

        public QuickSortMedian3InsertTests()
        {
            sort = new QuickSortMedian3Insert<int>();
            algorithm = nameof(QuickSortMedian3Insert<int>);
        }

        [Fact]
        public void SortTypeTest()
        {
            sort.SortType.Is(SortType.Partition);
        }

        [Theory]
        [ClassData(typeof(MockRandomData))]
        public void RandomInputTypeTest(IInputSample<int> inputSample)
        {
            inputSample.InputType.Is(InputType.Random);
        }

        [Theory]
        [ClassData(typeof(MockNegativePositiveRandomData))]
        public void MixRandomInputTypeTest(IInputSample<int> inputSample)
        {
            inputSample.InputType.Is(InputType.MixRandom);
        }

        [Theory]
        [ClassData(typeof(MockNegativeRandomData))]
        public void NegativeRandomInputTypeTest(IInputSample<int> inputSample)
        {
            inputSample.InputType.Is(InputType.NegativeRandom);
        }

        [Theory]
        [ClassData(typeof(MockReversedData))]
        public void ReverseInputTypeTest(IInputSample<int> inputSample)
        {
            inputSample.InputType.Is(InputType.Reversed);
        }

        [Theory]
        [ClassData(typeof(MockMountainData))]
        public void MountainInputTypeTest(IInputSample<int> inputSample)
        {
            inputSample.InputType.Is(InputType.Mountain);
        }

        [Theory]
        [ClassData(typeof(MockNearlySortedData))]
        public void NearlySortedInputTypeTest(IInputSample<int> inputSample)
        {
            inputSample.InputType.Is(InputType.NearlySorted);
        }

        [Theory]
        [ClassData(typeof(MockSortedData))]
        public void SortedInputTypeTest(IInputSample<int> inputSample)
        {
            inputSample.InputType.Is(InputType.Sorted);
        }

        [Theory]
        [ClassData(typeof(MockSameValuesData))]
        public void SameValuesInputTypeTest(IInputSample<int> inputSample)
        {
            inputSample.InputType.Is(InputType.SameValues);
        }

        [Theory]
        [ClassData(typeof(MockRandomData))]
        [ClassData(typeof(MockNegativePositiveRandomData))]
        [ClassData(typeof(MockNegativeRandomData))]
        [ClassData(typeof(MockReversedData))]
        [ClassData(typeof(MockMountainData))]
        [ClassData(typeof(MockNearlySortedData))]
        [ClassData(typeof(MockSortedData))]
        [ClassData(typeof(MockSameValuesData))]
        public void SortResultOrderTest(IInputSample<int> inputSample)
        {
            sort.Sort(inputSample.Samples).Is(inputSample.Samples.OrderBy(x => x));
        }

        [Theory]
        [ClassData(typeof(MockRandomData))]
        [ClassData(typeof(MockNegativePositiveRandomData))]
        [ClassData(typeof(MockNegativeRandomData))]
        [ClassData(typeof(MockReversedData))]
        [ClassData(typeof(MockMountainData))]
        [ClassData(typeof(MockNearlySortedData))]
        [ClassData(typeof(MockSameValuesData))]
        public void StatisticsTest(IInputSample<int> inputSample)
        {
            sort.Sort(inputSample.Samples);
            sort.Statistics.Algorithm.Is(algorithm);
            sort.Statistics.ArraySize.Is(inputSample.Samples.Length);
            sort.Statistics.IndexAccessCount.IsNot<ulong>(0);
            sort.Statistics.CompareCount.IsNot<ulong>(0);
            sort.Statistics.SwapCount.IsNot<ulong>(0);
        }

        [Theory]
        [ClassData(typeof(MockSortedData))]
        public void StatisticsNoSwapCountTest(IInputSample<int> inputSample)
        {
            sort.Sort(inputSample.Samples);
            sort.Statistics.Algorithm.Is(algorithm);
            sort.Statistics.ArraySize.Is(inputSample.Samples.Length);
            sort.Statistics.IndexAccessCount.IsNot<ulong>(0);
            sort.Statistics.CompareCount.IsNot<ulong>(0);
            sort.Statistics.SwapCount.IsNot<ulong>(0);
        }

        [Theory]
        [ClassData(typeof(MockRandomData))]
        [ClassData(typeof(MockNegativePositiveRandomData))]
        [ClassData(typeof(MockNegativeRandomData))]
        [ClassData(typeof(MockReversedData))]
        [ClassData(typeof(MockMountainData))]
        [ClassData(typeof(MockNearlySortedData))]
        [ClassData(typeof(MockSortedData))]
        [ClassData(typeof(MockSameValuesData))]
        public void StatisticsResetTest(IInputSample<int> inputSample)
        {
            sort.Sort(inputSample.Samples);
            sort.Statistics.Reset();
            sort.Statistics.IndexAccessCount.Is<ulong>(0);
            sort.Statistics.CompareCount.Is<ulong>(0);
            sort.Statistics.SwapCount.Is<ulong>(0);
        }
    }
}