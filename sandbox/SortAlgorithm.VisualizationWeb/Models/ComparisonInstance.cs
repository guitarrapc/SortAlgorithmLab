namespace SortAlgorithm.VisualizationWeb.Models;

/// <summary>
/// 比較対象の個別アルゴリズム情報
/// </summary>
public class ComparisonInstance
{
    public required string AlgorithmName { get; init; }
    public required VisualizationState State { get; init; }
    public required AlgorithmMetadata Metadata { get; init; }
}


