using System;

namespace SortTests;

public class CountingSortTests
{
    private ISort<int> sort;
    private Func<int[], int[]> func;
    private string algorithm;
    private SortType sortType;

    public CountingSortTests()
    {
        var sort = new CountingSort<int>();
        func = array => sort.Sort(array);
        this.sort = sort;
        algorithm = nameof(CountingSort<int>);
        sortType = SortType.Distributed;
    }

    [Fact]
    public void SortTypeTest()
    {
        sort.SortType.Should().Be(sortType);
    }

    [Theory]
    [ClassData(typeof(MockRandomData))]
    public void RandomInputTypeTest(IInputSample<int> inputSample)
    {
        inputSample.InputType.Should().Be(InputType.Random);
    }

    [Theory]
    [ClassData(typeof(MockNegativePositiveRandomData))]
    public void MixRandomInputTypeTest(IInputSample<int> inputSample)
    {
        inputSample.InputType.Should().Be(InputType.MixRandom);
    }

    [Theory]
    [ClassData(typeof(MockNegativeRandomData))]
    public void NegativeRandomInputTypeTest(IInputSample<int> inputSample)
    {
        inputSample.InputType.Should().Be(InputType.NegativeRandom);
    }

    [Theory]
    [ClassData(typeof(MockReversedData))]
    public void ReverseInputTypeTest(IInputSample<int> inputSample)
    {
        inputSample.InputType.Should().Be(InputType.Reversed);
    }

    [Theory]
    [ClassData(typeof(MockMountainData))]
    public void MountainInputTypeTest(IInputSample<int> inputSample)
    {
        inputSample.InputType.Should().Be(InputType.Mountain);
    }

    [Theory]
    [ClassData(typeof(MockNearlySortedData))]
    public void NearlySortedInputTypeTest(IInputSample<int> inputSample)
    {
        inputSample.InputType.Should().Be(InputType.NearlySorted);
    }

    [Theory]
    [ClassData(typeof(MockSortedData))]
    public void SortedInputTypeTest(IInputSample<int> inputSample)
    {
        inputSample.InputType.Should().Be(InputType.Sorted);
    }

    [Theory]
    [ClassData(typeof(MockSameValuesData))]
    public void SameValuesInputTypeTest(IInputSample<int> inputSample)
    {
        inputSample.InputType.Should().Be(InputType.SameValues);
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
        func(inputSample.Samples).Should().BeEquivalentTo(inputSample.Samples.OrderBy(x => x));
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
        func(inputSample.Samples);
        sort.Statistics.Algorithm.Should().Be(algorithm);
        sort.Statistics.ArraySize.Should().Be(inputSample.Samples.Length);
        sort.Statistics.IndexAccessCount.Should().NotBe(0);
        sort.Statistics.CompareCount.Should().Be(0);
        sort.Statistics.SwapCount.Should().Be(0);
    }

    [Theory]
    [ClassData(typeof(MockSortedData))]
    public void StatisticsNoSwapCountTest(IInputSample<int> inputSample)
    {
        func(inputSample.Samples);
        sort.Statistics.Algorithm.Should().Be(algorithm);
        sort.Statistics.ArraySize.Should().Be(inputSample.Samples.Length);
        sort.Statistics.IndexAccessCount.Should().NotBe(0);
        sort.Statistics.CompareCount.Should().Be(0);
        sort.Statistics.SwapCount.Should().Be(0);
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
        func(inputSample.Samples);
        sort.Statistics.Reset();
        sort.Statistics.IndexAccessCount.Should().Be(0);
        sort.Statistics.CompareCount.Should().Be(0);
        sort.Statistics.SwapCount.Should().Be(0);
    }
}