using SortAlgorithm.Contexts;
using SortAlgorithm.VisualizationWeb.Models;

namespace SortAlgorithm.VisualizationWeb.Services;

/// <summary>
/// ソート実行と操作記録を行うサービス
/// </summary>
public class SortExecutor
{
    /// <summary>
    /// ソートを実行し、すべての操作を記録する
    /// </summary>
    public List<SortOperation> ExecuteAndRecord(int[] array, object sortAlgorithm)
    {
        var operations = new List<SortOperation>();
        
        // VisualizationContextを使って操作を記録
        var context = new VisualizationContext(
            onCompare: (i, j, result, bufferIdI, bufferIdJ) =>
            {
                operations.Add(new SortOperation
                {
                    Type = OperationType.Compare,
                    Index1 = i,
                    Index2 = j,
                    BufferId1 = bufferIdI,
                    BufferId2 = bufferIdJ,
                    CompareResult = result
                });
            },
            onSwap: (i, j, bufferId) =>
            {
                operations.Add(new SortOperation
                {
                    Type = OperationType.Swap,
                    Index1 = i,
                    Index2 = j,
                    BufferId1 = bufferId
                });
            },
            onIndexRead: (index, bufferId) =>
            {
                operations.Add(new SortOperation
                {
                    Type = OperationType.IndexRead,
                    Index1 = index,
                    BufferId1 = bufferId
                });
            },
            onIndexWrite: (index, bufferId) =>
            {
                operations.Add(new SortOperation
                {
                    Type = OperationType.IndexWrite,
                    Index1 = index,
                    BufferId1 = bufferId
                });
            },
            onRangeCopy: (sourceIndex, destIndex, length, sourceBufferId, destBufferId) =>
            {
                operations.Add(new SortOperation
                {
                    Type = OperationType.RangeCopy,
                    Index1 = sourceIndex,
                    Index2 = destIndex,
                    Length = length,
                    BufferId1 = sourceBufferId,
                    BufferId2 = destBufferId
                });
            }
        );
        
        // リフレクションでSort(int[], ISortContext)メソッドを呼び出す
        try
        {
            var sortMethod = sortAlgorithm.GetType().GetMethod("Sort", [typeof(int[]), typeof(ISortContext)]);
            if (sortMethod != null)
            {
                sortMethod.Invoke(sortAlgorithm, [array, context]);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error invoking sort: {ex.Message}");
        }
        
        return operations;
    }
}
