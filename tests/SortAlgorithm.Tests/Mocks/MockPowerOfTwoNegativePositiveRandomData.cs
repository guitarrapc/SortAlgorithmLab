using SortAlgorithm;
using System.Collections;

namespace SortAlgorithm.Tests;

public class MockPowerOfTwoNegativePositiveRandomData : IEnumerable<object[]>
{
    private List<object[]> testData = new List<object[]>();

    public MockPowerOfTwoNegativePositiveRandomData()
    {
        testData.Add([new InputSample<int>() { InputType = InputType.Random, Samples = Enumerable.Range(-8, 16).Sample(16).ToArray() }]);
        testData.Add([new InputSample<int>() { InputType = InputType.Random, Samples = Enumerable.Range(-32, 64).Sample(64).ToArray() }]);
        testData.Add([new InputSample<int>() { InputType = InputType.Random, Samples = Enumerable.Range(-128, 256).Sample(256).ToArray() }]);
        testData.Add([new InputSample<int>() { InputType = InputType.Random, Samples = Enumerable.Range(-256, 512).Sample(512).ToArray() }]);
        testData.Add([new InputSample<int>() { InputType = InputType.Random, Samples = Enumerable.Range(-512, 1024).Sample(1024).ToArray() }]);
        testData.Add([new InputSample<int>() { InputType = InputType.Random, Samples = Enumerable.Range(-1024, 2048).Sample(2048).ToArray() }]);
    }

    public IEnumerator<object[]> GetEnumerator() => testData.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
