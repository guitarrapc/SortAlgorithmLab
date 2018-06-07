using System;

namespace SortAlgorithm
{
    public interface ISort<T> where T : IComparable<T>
    {
        SortStatics Statics { get; }
        T[] Sort(T[] array);
    }
}
