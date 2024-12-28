using System;
using System.Collections;
using System.Collections.Generic;

namespace SortTests;

public class MockReversedData : IEnumerable<object[]>
{
    private List<object[]> testData = new List<object[]>();

    public MockReversedData()
    {
        testData.Add(new object[] { new InputSample<int>() { InputType = InputType.Reversed, Samples = Enumerable.Range(0, 100).Reverse().ToArray() } });
        testData.Add(new object[] { new InputSample<int>() { InputType = InputType.Reversed, Samples = Enumerable.Range(0, 1000).Reverse().ToArray() } });
        testData.Add(new object[] { new InputSample<int>() { InputType = InputType.Reversed, Samples = Enumerable.Range(0, 10000).Reverse().ToArray() } });
    }

    public IEnumerator<object[]> GetEnumerator() => testData.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
