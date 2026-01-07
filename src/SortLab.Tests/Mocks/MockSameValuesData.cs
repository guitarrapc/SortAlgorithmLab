using System.Collections;

namespace SortLab.Tests;

public class MockSameValuesData : IEnumerable<object[]>
{
    private List<object[]> testData = new List<object[]>();

    public MockSameValuesData()
    {
        var randomeSample = Enumerable.Range(0, 100).Sample(10).ToArray();
        var randomeSample2 = Enumerable.Range(0, 1000).Sample(10).ToArray();
        var randomeSample3 = Enumerable.Range(0, 10000).Sample(10).ToArray();
        testData.Add([new InputSample<int>() { InputType = InputType.SameValues, Samples = randomeSample.SelectMany(x => Enumerable.Repeat(x, 10)).Sample(100).ToArray() }]);
        testData.Add([new InputSample<int>() { InputType = InputType.SameValues, Samples = randomeSample2.SelectMany(x => Enumerable.Repeat(x, 100)).Sample(1000).ToArray() }]);
        testData.Add([new InputSample<int>() { InputType = InputType.SameValues, Samples = randomeSample3.SelectMany(x => Enumerable.Repeat(x, 1000)).Sample(10000).ToArray() }]);
    }

    public IEnumerator<object[]> GetEnumerator() => testData.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
