using System.Collections;
using System.Collections.Generic;

namespace SortLab.Tests;

public class MockRandomData : IEnumerable<object[]>
{
    private List<object[]> testData = new List<object[]>();

    public MockRandomData()
    {
        testData.Add([new InputSample<int>() { InputType = InputType.Random, Samples = Enumerable.Range(0, 100).Sample(100).ToArray() }]);
        testData.Add([new InputSample<int>() { InputType = InputType.Random, Samples = Enumerable.Range(0, 1000).Sample(1000).ToArray() }]);
        testData.Add([new InputSample<int>() { InputType = InputType.Random, Samples = Enumerable.Range(0, 10000).Sample(10000).ToArray() }]);
    }

    public IEnumerator<object[]> GetEnumerator() => testData.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
