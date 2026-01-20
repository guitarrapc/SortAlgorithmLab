# Sort Architecture 再設計案

## 設計原則

### 核心的な考え方

1. **ソート関数は純粋関数** - 静的メソッドとしてステートレスに実装
2. **状態はContextに逃がす** - 統計・描画などの状態を持つものは外部から注入されるContextが担当
3. **標準ライブラリとの一貫性** - `Array.Sort()`, `MemoryExtensions.Sort()` と同様のAPI設計

```
┌──────────────────────────────────────────────────────────────────┐
│                        呼び出し側                                 │
│  ┌──────────────┐                        ┌───────────────────┐  │
│  │ Span<T>      │                        │ Context           │  │
│  │ (入力データ)  │                        │ (統計/描画の状態) │  │
│  └──────┬───────┘                        └─────────┬─────────┘  │
│         │                                          │            │
│         │              注入（オプション）            │            │
│         ▼                    ▼                     ▼            │
│  ┌─────────────────────────────────────────────────────────┐    │
│  │  BubbleSort.Sort<T>(span)                               │    │
│  │  BubbleSort.Sort<T, TContext>(span, ref context)        │    │
│  │  ──────────────────────────────────────────────────     │    │
│  │  • 静的メソッド（インスタンス不要）                       │    │
│  │  • 内部状態なし（純粋関数）                              │    │
│  │  • Context経由で統計/描画をフック                        │    │
│  └─────────────────────────────────────────────────────────┘    │
│         │                                          │            │
│         ▼                                          ▼            │
│  ┌──────────────┐                        ┌───────────────────┐  │
│  │ Span<T>      │                        │ Context           │  │
│  │ (ソート済み)  │                        │ (統計結果を保持)  │  │
│  └──────────────┘                        └───────────────────┘  │
└──────────────────────────────────────────────────────────────────┘
```

### 責務の分離

| コンポーネント | 責務 | 状態 |
|--------------|------|------|
| ソート関数 | データの並べ替え | **なし（ステートレス）** |
| Context | 統計収集・描画コールバック | **あり（ミュータブル）** |
| 呼び出し側 | Contextの生成・管理 | Contextを所有 |

---

## 現状の課題

```csharp
// 現在の実装：インスタンスベース、状態が内部に密結合
var sort = new BubbleSort<int>();  // インスタンス化が必要
sort.Sort(array);
var stats = sort.Statistics;       // 状態がソートクラス内部に存在
```

**問題点：**
1. インスタンス化が必要（静的メソッドであるべき）
2. 統計情報がソートクラス内部に密結合
3. 拡張（ビジュアライズ等）が困難

---

## 提案設計：Class-based Context Pattern

classベースのContextパターンを採用。シンプルなAPIと拡張性を両立。

### 設計方針

- **Context は class** - `ref` 渡し不要でAPIがシンプル
- **NullContext.Default** - シングルトンで何もしない（パフォーマンス優先）
- **CompositeContext** - 複数のContextを組み合わせ可能（統計+描画など）

```csharp
// ===========================================
// Context インターフェース
// ===========================================
public interface ISortContext
{
    /// <summary>比較が行われた</summary>
    /// <param name="i">比較元のインデックス</param>
    /// <param name="j">比較先のインデックス</param>
    /// <param name="result">比較結果（負:i<j, 0:i==j, 正:i>j）</param>
    void OnCompare(int i, int j, int result);
    
    /// <summary>スワップが行われた</summary>
    /// <param name="i">スワップ元のインデックス</param>
    /// <param name="j">スワップ先のインデックス</param>
    void OnSwap(int i, int j);
    
    /// <summary>インデックスアクセスが行われた</summary>
    /// <param name="index">アクセスされたインデックス</param>
    void OnIndexAccess(int index);
}

// ===========================================
// Context 実装：No-op（シングルトン）
// ===========================================
public sealed class NullContext : ISortContext
{
    public static readonly NullContext Default = new();
    
    private NullContext() { }  // シングルトン
    
    public void OnCompare(int i, int j, int result) { }
    public void OnSwap(int i, int j) { }
    public void OnIndexAccess(int index) { }
}

// ===========================================
// Context 実装：統計収集
// ===========================================
public sealed class StatisticsContext : ISortContext
{
    public ulong CompareCount { get; private set; }
    public ulong SwapCount { get; private set; }
    public ulong IndexAccessCount { get; private set; }

    public void OnCompare(int i, int j, int result) => CompareCount++;
    public void OnSwap(int i, int j) => SwapCount++;
    public void OnIndexAccess(int index) => IndexAccessCount++;
    
    public void Reset()
    {
        CompareCount = 0;
        SwapCount = 0;
        IndexAccessCount = 0;
    }
}

// ===========================================
// Context 実装：ビジュアライズ
// ===========================================
public sealed class VisualizationContext : ISortContext
{
    private readonly Action<int, int, int>? _onCompare;
    private readonly Action<int, int>? _onSwap;
    private readonly Action<int>? _onAccess;
    
    public VisualizationContext(
        Action<int, int, int>? onCompare = null,
        Action<int, int>? onSwap = null,
        Action<int>? onAccess = null)
    {
        _onCompare = onCompare;
        _onSwap = onSwap;
        _onAccess = onAccess;
    }
    
    public void OnCompare(int i, int j, int result) => _onCompare?.Invoke(i, j, result);
    public void OnSwap(int i, int j) => _onSwap?.Invoke(i, j);
    public void OnIndexAccess(int index) => _onAccess?.Invoke(index);
}

// ===========================================
// Context 実装：複合（統計+描画など）
// ===========================================
public sealed class CompositeContext : ISortContext
{
    private readonly ISortContext[] _contexts;
    
    public CompositeContext(params ISortContext[] contexts)
    {
        _contexts = contexts;
    }
    
    public void OnCompare(int i, int j, int result)
    {
        foreach (var ctx in _contexts)
            ctx.OnCompare(i, j, result);
    }
    
    public void OnSwap(int i, int j)
    {
        foreach (var ctx in _contexts)
            ctx.OnSwap(i, j);
    }
    
    public void OnIndexAccess(int index)
    {
        foreach (var ctx in _contexts)
            ctx.OnIndexAccess(index);
    }
}

// ===========================================
// ソートアルゴリズム：静的メソッド
// ===========================================
public static class BubbleSort
{
    // シンプルAPI：Contextなし（NullContext使用）
    public static void Sort<T>(Span<T> span) where T : IComparable<T>
        => Sort(span, NullContext.Default);

    // Context対応API
    public static void Sort<T>(Span<T> span, ISortContext context) where T : IComparable<T>
    {
        for (var i = 0; i < span.Length; i++)
        {
            for (var j = span.Length - 1; j > i; j--)
            {
                context.OnIndexAccess(j);
                context.OnIndexAccess(j - 1);
                
                var cmp = span[j].CompareTo(span[j - 1]);
                context.OnCompare(j, j - 1, cmp);
                
                if (cmp < 0)
                {
                    context.OnSwap(j, j - 1);
                    (span[j], span[j - 1]) = (span[j - 1], span[j]);
                }
            }
        }
    }
}
```

### 使用例

```csharp
// 1. シンプルな使用（統計なし）
BubbleSort.Sort<int>(array);

// 2. 統計収集
var stats = new StatisticsContext();
BubbleSort.Sort(array.AsSpan(), stats);
Console.WriteLine($"Comparisons: {stats.CompareCount}, Swaps: {stats.SwapCount}");

// 3. ビジュアライズ（描画）
var viz = new VisualizationContext(
    onCompare: (i, j, result) => HighlightCompare(i, j),
    onSwap: (i, j) => AnimateSwap(i, j)
);
BubbleSort.Sort(array.AsSpan(), viz);

// 4. 統計 + 描画を同時に（CompositeContext）
var stats = new StatisticsContext();
var viz = new VisualizationContext(onSwap: (i, j) => Render(i, j));
var composite = new CompositeContext(stats, viz);
BubbleSort.Sort(array.AsSpan(), composite);
Console.WriteLine($"Swaps: {stats.SwapCount}");  // 統計も取れる

// 5. 並行実行（各スレッドが独自のContextを持つ）
Parallel.ForEach(arrays, array =>
{
    var localStats = new StatisticsContext();
    BubbleSort.Sort(array.AsSpan(), localStats);
    // localStatsは各スレッドで独立
});
```

### パフォーマンス特性

| Context | 仮想呼び出し | 実行時オーバーヘッド | 備考 |
|---------|------------|-------------------|------|
| `NullContext.Default` | あり | 最小 | 空メソッドのみ、シングルトン |
| `StatisticsContext` | あり | 小 | カウントインクリメントのみ |
| `VisualizationContext` | あり | 中 | コールバック実行 |
| `CompositeContext` | あり | 中〜大 | 複数Context呼び出し |

**Note：**
- 仮想呼び出し（vtable lookup）のコストは存在するが、ソートの比較・スワップ回数に比べれば微小
- パフォーマンスクリティカルな場合は `NullContext.Default` で統計なしにする
- JITのdevirtualization最適化により、特定条件下ではさらに最適化される可能性あり

---

## ファイル構成案

```
src/SortLab.Core/
├── Contexts/
│   ├── ISortContext.cs           # インターフェース
│   ├── NullContext.cs            # No-op（シングルトン class）
│   ├── StatisticsContext.cs      # 統計収集（class）
│   ├── VisualizationContext.cs   # 描画用（class）
│   └── CompositeContext.cs       # 複合（class）
├── Algorithms/
│   ├── Exchange/
│   │   ├── BubbleSort.cs         # static class
│   │   └── ...
│   ├── Partition/
│   │   ├── QuickSort.cs
│   │   └── ...
│   └── ...
└── SortMethod.cs
```

---

## 懸念事項と解決策

### 1. ヘルパーメソッド（Compare, Swap, Index）の共有

**現状：** `SortBase<T>` の protected メソッドとして提供

**解決策：**

```csharp
// 静的ヘルパークラス
internal static class SortHelper
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Compare<T>(Span<T> span, int i, int j, ISortContext context)
        where T : IComparable<T>
    {
        var result = span[i].CompareTo(span[j]);
        context.OnCompare(i, j, result);
        return result;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Swap<T>(Span<T> span, int i, int j, ISortContext context)
    {
        context.OnSwap(i, j);
        (span[i], span[j]) = (span[j], span[i]);
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref T Index<T>(Span<T> span, int index, ISortContext context)
    {
        context.OnIndexAccess(index);
        return ref span[index];
    }
}

// 使用例
if (SortHelper.Compare(span, j, j - 1, context) < 0)
{
    SortHelper.Swap(span, j, j - 1, context);
}
```

### 2. アルゴリズム情報（名前、SortMethod）

**解決策：** 属性でメタデータを付与

```csharp
[SortAlgorithm(SortMethod.Exchange, "BubbleSort")]
public static class BubbleSort
{
    // ...
}
```

### 3. 既存テストとの互換性

**解決策：** 旧APIをラッパーとして残す（非推奨マーク付き）

```csharp
[Obsolete("Use static BubbleSort.Sort() instead")]
public class BubbleSortLegacy<T> : ISort<T> where T : IComparable<T>
{
    public void Sort(Span<T> span) => BubbleSort.Sort(span);
}
```

---

## 採用設計まとめ

| 項目 | 決定 |
|------|------|
| **Context の型** | class（シングルトン + 通常インスタンス） |
| **NullContext** | `NullContext.Default` シングルトン |
| **複合Context** | `CompositeContext` で複数組み合わせ可能 |
| **ソートAPI** | `Sort<T>(Span<T>)` と `Sort<T>(Span<T>, ISortContext)` |
| **ヘルパーメソッド** | `SortHelper` 静的クラス |
| **ビジュアライズ対応** | 対応（`VisualizationContext` で位置情報取得可能） |

---

## 次のステップ

1. `ISortContext` インターフェースと各Context実装を作成
2. `SortHelper` 静的クラスを作成
3. `BubbleSort` を新設計で実装
4. ベンチマークで現行版との比較
5. 他アルゴリズムへの展開
6. 旧API（`SortBase<T>`, `ISort<T>`）の非推奨化・削除
