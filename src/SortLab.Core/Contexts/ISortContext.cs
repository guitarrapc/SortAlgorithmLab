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
    /// Handles the event when an item at the specified index is read.
    /// </summary>
    /// <param name="index">The zero-based index of the item that was read.</param>
    void OnIndexRead(int index);

    /// <summary>
    /// Handles a write operation at the specified index.
    /// </summary>
    /// <param name="index">The zero-based index at which the write operation occurs.</param>
    void OnIndexWrite(int index);
}
