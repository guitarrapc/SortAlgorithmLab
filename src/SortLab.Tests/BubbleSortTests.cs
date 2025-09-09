﻿namespace SortLab.Tests;

public class BubbleSortTests
{
    private ISort<int> sort;
    private string algorithm;
    private SortMethod method;

    public BubbleSortTests()
    {
        sort = new BubbleSort<int>();
        algorithm = nameof(BubbleSort<int>);
        method = SortMethod.Exchange;
    }

    [Fact]
    public void SortMethodTest()
    {
        Assert.Equal(method, sort.SortType);
    }

    [Theory]
    [ClassData(typeof(MockRandomData))]
    public void RandomInputTypeTest(IInputSample<int> inputSample)
    {
        Assert.Equal(InputType.Random, inputSample.InputType);
    }

    [Theory]
    [ClassData(typeof(MockNegativePositiveRandomData))]
    public void MixRandomInputTypeTest(IInputSample<int> inputSample)
    {
        Assert.Equal(InputType.MixRandom, inputSample.InputType);
    }

    [Theory]
    [ClassData(typeof(MockNegativeRandomData))]
    public void NegativeRandomInputTypeTest(IInputSample<int> inputSample)
    {
        Assert.Equal(InputType.NegativeRandom, inputSample.InputType);
    }

    [Theory]
    [ClassData(typeof(MockReversedData))]
    public void ReverseInputTypeTest(IInputSample<int> inputSample)
    {
        Assert.Equal(InputType.Reversed, inputSample.InputType);
    }

    [Theory]
    [ClassData(typeof(MockMountainData))]
    public void MountainInputTypeTest(IInputSample<int> inputSample)
    {
        Assert.Equal(InputType.Mountain, inputSample.InputType);
    }

    [Theory]
    [ClassData(typeof(MockNearlySortedData))]
    public void NearlySortedInputTypeTest(IInputSample<int> inputSample)
    {
        Assert.Equal(InputType.NearlySorted, inputSample.InputType);
    }

    [Theory]
    [ClassData(typeof(MockSortedData))]
    public void SortedInputTypeTest(IInputSample<int> inputSample)
    {
        Assert.Equal(InputType.Sorted, inputSample.InputType);
    }

    [Theory]
    [ClassData(typeof(MockSameValuesData))]
    public void SameValuesInputTypeTest(IInputSample<int> inputSample)
    {
        Assert.Equal(InputType.SameValues, inputSample.InputType);
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
        var result = sort.Sort(inputSample.Samples);
        var expected = inputSample.Samples.OrderBy(x => x).ToArray();
        Assert.Equal(expected, result);
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
        Assert.Equal(algorithm, sort.Statistics.Algorithm);
        Assert.Equal(inputSample.Samples.Length, sort.Statistics.ArraySize);
        Assert.NotEqual(0UL, sort.Statistics.IndexAccessCount);
        Assert.NotEqual(0UL, sort.Statistics.CompareCount);
        Assert.NotEqual(0UL, sort.Statistics.SwapCount);
    }

    [Theory]
    [ClassData(typeof(MockSortedData))]
    public void StatisticsSortedTest(IInputSample<int> inputSample)
    {
        sort.Sort(inputSample.Samples);
        Assert.Equal(algorithm, sort.Statistics.Algorithm);
        Assert.Equal(inputSample.Samples.Length, sort.Statistics.ArraySize);
        Assert.NotEqual(0UL, sort.Statistics.IndexAccessCount);
        Assert.NotEqual(0UL, sort.Statistics.CompareCount);
        Assert.Equal(0UL, sort.Statistics.SwapCount);
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
        Assert.Equal(0UL, sort.Statistics.IndexAccessCount);
        Assert.Equal(0UL, sort.Statistics.CompareCount);
        Assert.Equal(0UL, sort.Statistics.SwapCount);
    }
}

