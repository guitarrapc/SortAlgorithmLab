namespace SortLab.Core;

public interface ISort<T> where T : IComparable<T>
{
    SortMethod SortType { get; }

    IStatistics Statistics { get; }

    /// <summary>
    /// Executes the sorting algorithm on the provided array.
    /// </summary>
    /// <param name="array"></param>
    void Sort(T[] array);

    /// <summary>
    /// Executes the sorting algorithm on the provided span.
    /// </summary>
    /// <param name="span"></param>
    void Sort(Span<T> span);
}
