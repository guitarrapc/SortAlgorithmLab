namespace SortLab.Core.Contexts;

public interface ISortContext
{
    /// <summary>
    /// Handles the result of comparing two elements at the specified indices.
    /// </summary>
    /// <param name="i">Index of the compare from</param>
    /// <param name="j">Index of the compare to</param>
    /// <param name="result">The result of the comparison. negative(-) if the first element is less than the second, zero(0) if they are equal, and positive(+) if the first is greater than the second.</param>
    void OnCompare(int i, int j, int result);

    /// <summary>
    /// Handles the swapping of two elements at the specified indices.
    /// </summary>
    /// <param name="i">Index of the swap from</param>
    /// <param name="j">Index of the swap to</param>
    void OnSwap(int i, int j);

    /// <summary>
    /// Handles access to an element at the specified index.
    /// </summary>
    /// <param name="index">Index of the access</param>
    void OnIndexAccess(int index);
}
