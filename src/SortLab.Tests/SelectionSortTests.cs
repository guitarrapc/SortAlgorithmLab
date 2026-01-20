namespace SortLab.Tests;

public class SelectionSortTests
{
    private ISort<int> sort;
    private string algorithm;
    private SortMethod method;

    public SelectionSortTests()
    {
        sort = new SelectionSort<int>();
        algorithm = nameof(SelectionSort<int>);
        method = SortMethod.Selection;
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
        Assert.NotEqual((ulong)0, sort.Statistics.IndexAccessCount);
        Assert.NotEqual((ulong)0, sort.Statistics.CompareCount);
        Assert.NotEqual((ulong)0, sort.Statistics.SwapCount);
    }

    [Theory]
    [ClassData(typeof(MockSortedData))]
    public void StatisticsSortedTest(IInputSample<int> inputSample)
    {
        sort.Sort(inputSample.Samples);
        Assert.Equal(algorithm, sort.Statistics.Algorithm);
        Assert.Equal(inputSample.Samples.Length, sort.Statistics.ArraySize);
        Assert.NotEqual((ulong)0, sort.Statistics.IndexAccessCount);
        Assert.NotEqual((ulong)0, sort.Statistics.CompareCount);
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

        // 理論値: ソート済みの場合
        // 比較回数: n(n-1)/2 (常に全ての要素を比較)
        // 交換回数: 0 (交換不要)
        var expectedCompares = (ulong)(n * (n - 1) / 2);

        Assert.Equal(expectedCompares, sort.Statistics.CompareCount);
        Assert.Equal(0UL, sort.Statistics.SwapCount);
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

        // 理論値: 逆順の場合
        // 比較回数: n(n-1)/2 (常に同じ)
        // 交換回数: 最大 n/2 (各ペアの中央で交換)
        var expectedCompares = (ulong)(n * (n - 1) / 2);

        Assert.Equal(expectedCompares, sort.Statistics.CompareCount);
        Assert.True(sort.Statistics.SwapCount <= (ulong)(n / 2 + 1)); // 理論上の上限
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
        Assert.Equal((ulong)0, sort.Statistics.IndexAccessCount);
        Assert.Equal((ulong)0, sort.Statistics.CompareCount);
        Assert.Equal((ulong)0, sort.Statistics.SwapCount);
    }

}
