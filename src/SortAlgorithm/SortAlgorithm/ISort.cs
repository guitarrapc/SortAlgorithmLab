using System;

namespace SortAlgorithm
{
    public interface ISort<T> where T : IComparable<T>
    {
        SortType SortType { get; }

        IStatics Statics { get; }
        T[] Sort(T[] array);
    }
}
