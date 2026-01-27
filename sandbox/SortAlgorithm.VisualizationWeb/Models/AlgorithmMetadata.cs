namespace SortAlgorithm.VisualizationWeb.Models;

/// <summary>
/// アルゴリズムのメタデータ
/// </summary>
public record AlgorithmMetadata
{
    /// <summary>アルゴリズムの表示名</summary>
    public required string Name { get; init; }
    
    /// <summary>アルゴリズムのカテゴリ</summary>
    public required string Category { get; init; }
    
    /// <summary>時間計算量（平均）</summary>
    public required string TimeComplexity { get; init; }
    
    /// <summary>最大要素数</summary>
    public required int MaxElements { get; init; }
    
    /// <summary>型の完全名（リフレクション用）</summary>
    public required string TypeName { get; init; }
    
    /// <summary>説明</summary>
    public string Description { get; init; } = string.Empty;
}
