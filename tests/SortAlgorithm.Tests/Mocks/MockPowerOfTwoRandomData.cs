using SortAlgorithm;
using System.Collections;

namespace SortAlgorithm.Tests;

public class MockPowerOfTwoRandomData : IEnumerable<object[]>
{
    private List<object[]> testData = new List<object[]>();

    public MockPowerOfTwoRandomData()
    {
        testData.Add([new InputSample<int>() { InputType = InputType.Random, Samples = Enumerable.Range(0, 16).Sample(16).ToArray() }]);
        testData.Add([new InputSample<int>() { InputType = InputType.Random, Samples = Enumerable.Range(0, 64).Sample(64).ToArray() }]);
        testData.Add([new InputSample<int>() { InputType = InputType.Random, Samples = Enumerable.Range(0, 256).Sample(256).ToArray() }]);
        testData.Add([new InputSample<int>() { InputType = InputType.Random, Samples = Enumerable.Range(0, 512).Sample(512).ToArray() }]);
        testData.Add([new InputSample<int>() { InputType = InputType.Random, Samples = Enumerable.Range(0, 1024).Sample(1024).ToArray() }]);
        testData.Add([new InputSample<int>() { InputType = InputType.Random, Samples = Enumerable.Range(0, 2048).Sample(2048).ToArray() }]);
    }

    public IEnumerator<object[]> GetEnumerator() => testData.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
