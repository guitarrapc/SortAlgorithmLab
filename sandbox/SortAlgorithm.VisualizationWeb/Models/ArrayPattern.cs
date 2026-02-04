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
    /// 単一要素移動（ソート済みから1つの要素だけをランダム位置に移動）
    /// </summary>
    SingleElementMoved,

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

    /// <summary>
    /// 奇数インデックスのみシャッフル（偶数インデックスはソート済み）
    /// </summary>
    ShuffledOdds,

    /// <summary>
    /// 半分シャッフル（全体をシャッフル後、前半のみソート）
    /// </summary>
    ShuffledHalf,

    /// <summary>
    /// ダブルレイヤー（偶数インデックスを対称位置とスワップ）
    /// </summary>
    DoubleLayered,

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
    /// 真の最終基数パス（ビットマスクベース）
    /// </summary>
    RealFinalRadix,

    /// <summary>
    /// 再帰的最終基数パス
    /// </summary>
    RecursiveFinalRadix,

    /// <summary>
    /// 最終バイトニックパス（反転後にPipe Organ配置）
    /// </summary>
    FinalBitonicPass,

    /// <summary>
    /// ビット反転順序（FFT用）
    /// </summary>
    BitReversal,

    /// <summary>
    /// ブロックごとにランダムシャッフル
    /// </summary>
    BlockRandomly,

    /// <summary>
    /// ブロックごとに反転
    /// </summary>
    BlockReverse,

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
    /// 逆BST（レベル順 → 中順変換の逆操作）
    /// </summary>
    InvertedBst,

    /// <summary>
    /// 対数スロープ（2のべき乗ベースの配置）
    /// </summary>
    LogarithmicSlopes,

    /// <summary>
    /// 半分回転（前半と後半を入れ替え）
    /// </summary>
    HalfRotation,

    /// <summary>
    /// ヒープ化済み（max-heap構造）
    /// </summary>
    Heapified,

    /// <summary>
    /// スムースヒープ化済み（Smooth Sortのヒープ構造）
    /// </summary>
    SmoothHeapified,

    /// <summary>
    /// ポプラヒープ化済み（Poplar Heapソート用）
    /// </summary>
    PoplarHeapified,

    /// <summary>
    /// 三角ヒープ化済み（Triangular Heapソート用）
    /// </summary>
    TriangularHeapified,

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
    /// 二次曲線分布
    /// </summary>
    QuadraticDistribution,

    /// <summary>
    /// 平方根曲線分布
    /// </summary>
    SquareRootDistribution,

    /// <summary>
    /// 三次曲線分布（中心）
    /// </summary>
    CubicDistribution,

    /// <summary>
    /// 五次曲線分布（中心）
    /// </summary>
    QuinticDistribution,

    /// <summary>
    /// 立方根曲線分布
    /// </summary>
    CubeRootDistribution,

    /// <summary>
    /// 五乗根曲線分布
    /// </summary>
    FifthRootDistribution,

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

    /// <summary>
    /// ルーラー関数分布
    /// </summary>
    RulerDistribution,

    /// <summary>
    /// ブランマンジェ曲線分布
    /// </summary>
    BlancmangeDistribution,

    /// <summary>
    /// カントール関数分布
    /// </summary>
    CantorDistribution,

    /// <summary>
    /// 約数の和関数分布
    /// </summary>
    DivisorsDistribution,

    /// <summary>
    /// FSD分布（Fly Straight Dangit - OEIS A133058）
    /// </summary>
    FsdDistribution,

    /// <summary>
    /// 逆対数分布（減少ランダム）
    /// </summary>
    ReverseLogDistribution,

    /// <summary>
    /// モジュロ関数分布
    /// </summary>
    ModuloDistribution,

    /// <summary>
    /// オイラーのトーシェント関数分布
    /// </summary>
    TotientDistribution,

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
