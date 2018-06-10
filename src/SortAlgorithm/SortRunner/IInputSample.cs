using System;
using System.Collections.Generic;
using System.Text;

namespace SortRunner
{
    public enum InputType
    {
        Random,
        Reversed,
        Mountain,
        NearlySorted,
        Sorted,
        SameValues,

        MixRandom,
        NegativeRandom,
        DictionaryRamdom,
    }

    public interface IInputSample<T> where T : IComparable
    {
        InputType InputType { get; set; }
        T[] Samples { get; set; }
        KeyValuePair<T, string>[] DictionarySamples { get; set; }
    }

    public class InputSample<T> : IInputSample<T> where T : IComparable
    {
        public InputType InputType { get; set; }
        public T[] Samples { get; set; }
        public KeyValuePair<T, string>[] DictionarySamples { get; set; }
    }
}
