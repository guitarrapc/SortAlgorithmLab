using SortAlgorithm.Contexts;
using SortAlgorithm.VisualizationWeb.Models;
using System.Buffers;

namespace SortAlgorithm.VisualizationWeb.Services;

/// <summary>
/// ソート実行と操作記録を行うサービス
/// </summary>
public class SortExecutor
{
    /// <summary>
    /// ソートを実行し、すべての操作を記録する
    /// </summary>
    public List<SortOperation> ExecuteAndRecord(ReadOnlySpan<int> sourceArray, AlgorithmMetadata algorithm)
    {
        var operations = new List<SortOperation>();
        
        // ArrayPoolから配列をレンタル（ソート用の作業配列）
        var workArray = ArrayPool<int>.Shared.Rent(sourceArray.Length);
        
        try
        {
            // ソース配列をワーク配列にコピー
            sourceArray.CopyTo(workArray.AsSpan(0, sourceArray.Length));
            
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
            
            // デリゲートを直接呼び出し（リフレクション不要、AOT対応）
            algorithm.SortAction(workArray.AsSpan(0, sourceArray.Length).ToArray(), context);
        }
        finally
        {
            // ArrayPoolに配列を返却
            ArrayPool<int>.Shared.Return(workArray, clearArray: true);
        }
        
        return operations;
    }
}
