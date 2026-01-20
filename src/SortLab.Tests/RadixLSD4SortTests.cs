namespace SortLab.Tests;

public class RadixLSD4SortTests
{
    private ISort<int> sort;
    private string algorithm;
    private SortMethod method;

    public RadixLSD4SortTests()
    {
        sort = new RadixLSD4Sort<int>();
        algorithm = nameof(RadixLSD4Sort<int>);
        method = SortMethod.Distributed;
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
        var array = inputSample.Samples.ToArray();
        sort.Sort(array);
        Assert.Equal(inputSample.Samples.OrderBy(x => x), array);
    }

    [Theory]
    [ClassData(typeof(MockRandomData))]
    [ClassData(typeof(MockReversedData))]
    [ClassData(typeof(MockMountainData))]
    [ClassData(typeof(MockNearlySortedData))]
    [ClassData(typeof(MockSameValuesData))]
    public void StatisticsTest(IInputSample<int> inputSample)
    {
        var array = inputSample.Samples.ToArray();
        sort.Sort(array);
        Assert.Equal(algorithm, sort.Statistics.Algorithm);
        Assert.Equal(inputSample.Samples.Length, sort.Statistics.ArraySize);
        Assert.NotEqual((ulong)0, sort.Statistics.IndexAccessCount);
        Assert.NotEqual((ulong)0, sort.Statistics.CompareCount);
        Assert.Equal((ulong)0, sort.Statistics.SwapCount);
    }

    [Theory]
    [ClassData(typeof(MockNegativePositiveRandomData))]
    [ClassData(typeof(MockNegativeRandomData))]
    public void StatisticsNegativeTest(IInputSample<int> inputSample)
    {
        var array = inputSample.Samples.ToArray();
        sort.Sort(array);
        Assert.Equal(algorithm, sort.Statistics.Algorithm);
        Assert.Equal(inputSample.Samples.Length, sort.Statistics.ArraySize);
        Assert.NotEqual((ulong)0, sort.Statistics.IndexAccessCount);
        Assert.NotEqual((ulong)0, sort.Statistics.CompareCount);
        Assert.Equal((ulong)0, sort.Statistics.SwapCount);
    }

    [Theory]
    [ClassData(typeof(MockSortedData))]
    public void StatisticsSortedTest(IInputSample<int> inputSample)
    {
        var array = inputSample.Samples.ToArray();
        sort.Sort(array);
        Assert.Equal(algorithm, sort.Statistics.Algorithm);
        Assert.Equal(inputSample.Samples.Length, sort.Statistics.ArraySize);
        Assert.NotEqual((ulong)0, sort.Statistics.IndexAccessCount);
        Assert.NotEqual((ulong)0, sort.Statistics.CompareCount);
        Assert.Equal((ulong)0, sort.Statistics.SwapCount);
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
        var array = inputSample.Samples.ToArray();
        sort.Sort(array);
        sort.Statistics.Reset();
        Assert.Equal((ulong)0, sort.Statistics.IndexAccessCount);
        Assert.Equal((ulong)0, sort.Statistics.CompareCount);
        Assert.Equal((ulong)0, sort.Statistics.SwapCount);
    }

    [Theory]
    [InlineData(10)]
    [InlineData(20)]
    [InlineData(50)]
    [InlineData(100)]
    public void TheoreticalValuesSortedTest(int n)
    {
        var sorted = Enumerable.Range(0, n).ToArray();
        sort.Sort(sorted);

        // Sorted data should produce predictable statistics
        Assert.NotEqual(0UL, sort.Statistics.CompareCount);
    }

    [Theory]
    [InlineData(10)]
    [InlineData(20)]
    [InlineData(50)]
    [InlineData(100)]
    public void TheoreticalValuesReversedTest(int n)
    {
        var reversed = Enumerable.Range(0, n).Reverse().ToArray();
        sort.Sort(reversed);

        // Reversed data should require sorting operations
        Assert.NotEqual(0UL, sort.Statistics.CompareCount);
    }
}
