namespace SortAlgorithm.VisualizationWeb.Models;

/// <summary>
/// 配列生成パターンの種類
/// </summary>
public enum ArrayPattern
{
    // Basic Patterns
    
    /// <summary>
    /// ランダム配列
    /// </summary>
    Random,

    /// <summary>
    /// ソート済み配列（昇順）
    /// </summary>
    Sorted,

    /// <summary>
    /// 逆順配列（降順）
    /// </summary>
    Reversed,

    // Nearly Sorted Patterns
    
    /// <summary>
    /// ほぼソート済み配列（5%のペアをランダムスワップ）
    /// </summary>
    AlmostSorted,

    /// <summary>
    /// ほぼソート済み配列（要素の10%をランダムに入れ替え）
    /// </summary>
    NearlySorted,

    /// <summary>
    /// スクランブル末尾（約14%の要素を末尾に抽出してシャッフル）
    /// </summary>
    ScrambledTail,

    /// <summary>
    /// スクランブル先頭（約14%の要素を先頭に抽出してシャッフル）
    /// </summary>
    ScrambledHead,

    /// <summary>
    /// ノイズ入り（小ブロックごとにシャッフルされた配列）
    /// </summary>
    Noisy,

    // Merge Patterns
    
    /// <summary>
    /// 最終マージ状態（偶数・奇数インデックスが別々にソート済み）
    /// </summary>
    FinalMerge,

    /// <summary>
    /// シャッフル後最終マージ（全体をシャッフル後、前半と後半を別々にソート）
    /// </summary>
    ShuffledFinalMerge,

    /// <summary>
    /// ソートギア状（4-wayインターリーブでソート済み）
    /// </summary>
    Sawtooth,

    // Partitioned Patterns
    
    /// <summary>
    /// パーティション済み（ソート後、前半と後半を別々にシャッフル）
    /// </summary>
    Partitioned,

    /// <summary>
    /// 半分ソート済み（前半のみソート済み、後半はランダム）
    /// </summary>
    HalfSorted,

    /// <summary>
    /// 半分反転（後半が逆順）
    /// </summary>
    HalfReversed,

    // Shape Patterns
    
    /// <summary>
    /// パイプオルガン型（偶数要素が前半、奇数要素が後半逆順）
    /// </summary>
    PipeOrgan,

    /// <summary>
    /// 山型配列（中央が最大値、両端が小さい値）
    /// </summary>
    MountainShape,

    /// <summary>
    /// 谷型配列（中央が最小値、両端が大きい値）
    /// </summary>
    ValleyShape,

    // Radix/Interleaved Patterns
    
    /// <summary>
    /// 最終基数パス（偶数・奇数要素が交互配置）
    /// </summary>
    FinalRadix,

    /// <summary>
    /// インターレース（最小値を先頭、残りを両端から交互配置）
    /// </summary>
    Interlaced,

    /// <summary>
    /// ジグザグパターン（交互に上下する）
    /// </summary>
    Zigzag,

    // Tree/Heap Patterns
    
    /// <summary>
    /// 二分探索木中順走査（ランダム挿入からの中順走査結果）
    /// </summary>
    BstTraversal,

    /// <summary>
    /// ヒープ化済み（max-heap構造）
    /// </summary>
    Heapified,

    // Duplicate Patterns
    
    /// <summary>
    /// 少数ユニーク値（3種類の値: 25%, 50%, 75%位置）
    /// </summary>
    FewUnique,

    /// <summary>
    /// 重複多数（ユニーク値は配列サイズの20%程度）
    /// </summary>
    ManyDuplicates,

    /// <summary>
    /// 全要素同一（全て同じ値）
    /// </summary>
    AllEqual,

    // Distribution Patterns
    
    /// <summary>
    /// 正弦波分布
    /// </summary>
    SineWave,

    /// <summary>
    /// 余弦波分布
    /// </summary>
    CosineWave,

    /// <summary>
    /// ベル曲線分布（正規分布）
    /// </summary>
    BellCurve,

    /// <summary>
    /// パーリンノイズ曲線
    /// </summary>
    PerlinNoiseCurve,

    // Advanced/Fractal Patterns
    
    /// <summary>
    /// サークルソート初回パス（シャッフル後にサークルソート1パス）
    /// </summary>
    CirclePass,

    /// <summary>
    /// ペアワイズ最終パス（隣接ペアがソート済み）
    /// </summary>
    PairwisePass,

    /// <summary>
    /// 再帰的反転（反転を再帰的に適用）
    /// </summary>
    RecursiveReversal,

    /// <summary>
    /// グレイコードフラクタル
    /// </summary>
    GrayCodeFractal,

    /// <summary>
    /// シェルピンスキー三角形
    /// </summary>
    SierpinskiTriangle,

    /// <summary>
    /// 三角数配列
    /// </summary>
    Triangular,

    // Adversarial Patterns (Worst-case for specific algorithms)
    
    /// <summary>
    /// QuickSort最悪ケース（median-of-3 pivot用）
    /// </summary>
    QuickSortAdversary,

    /// <summary>
    /// PDQソート最悪ケース（Pattern-defeating QuickSort用）
    /// </summary>
    PdqSortAdversary,

    /// <summary>
    /// Grailソート最悪ケース
    /// </summary>
    GrailSortAdversary,

    /// <summary>
    /// ShuffleMerge最悪ケース
    /// </summary>
    ShuffleMergeAdversary,
}
