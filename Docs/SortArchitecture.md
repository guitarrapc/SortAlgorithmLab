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

## 提案設計

### 案1: Generic Struct Context（推奨）

構造体Contextをジェネリクスで受け取り、JITインライン化で最高パフォーマンスを実現。

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
// Context 実装：No-op（統計不要時）
// ===========================================
public readonly struct NullContext : ISortContext
{
    // JITが空メソッドを完全に削除 → ゼロオーバーヘッド
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void OnCompare(int i, int j, int result) { }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void OnSwap(int i, int j) { }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void OnIndexAccess(int index) { }
}

// ===========================================
// Context 実装：統計収集
// ===========================================
public struct StatisticsContext : ISortContext
{
    public ulong CompareCount;
    public ulong SwapCount;
    public ulong IndexAccessCount;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void OnCompare(int i, int j, int result) => CompareCount++;
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void OnSwap(int i, int j) => SwapCount++;
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void OnIndexAccess(int index) => IndexAccessCount++;
}

// ===========================================
// Context 実装：ビジュアライズ
// ===========================================
public class VisualizationContext : ISortContext
{
    private readonly Action<int, int, int>? _onCompareCallback;
    private readonly Action<int, int>? _onSwapCallback;
    private readonly Action<int>? _onAccessCallback;
    
    public VisualizationContext(
        Action<int, int, int>? onCompare = null,
        Action<int, int>? onSwap = null,
        Action<int>? onAccess = null)
    {
        _onCompareCallback = onCompare;
        _onSwapCallback = onSwap;
        _onAccessCallback = onAccess;
    }
    
    public void OnCompare(int i, int j, int result) 
        => _onCompareCallback?.Invoke(i, j, result);
    
    public void OnSwap(int i, int j) 
        => _onSwapCallback?.Invoke(i, j);
    
    public void OnIndexAccess(int index) 
        => _onAccessCallback?.Invoke(index);
}

// ===========================================
// ソートアルゴリズム：静的メソッド
// ===========================================
public static class BubbleSort
{
    // シンプルAPI：統計なし
    public static void Sort<T>(Span<T> span) where T : IComparable<T>
    {
        var context = new NullContext();
        Sort(span, ref context);
    }

    // 統計/描画対応API：Contextを受け取る
    public static void Sort<T, TContext>(Span<T> span, ref TContext context)
        where T : IComparable<T>
        where TContext : ISortContext
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

**使用例：**

```csharp
// 1. シンプルな使用（統計なし、最高パフォーマンス）
BubbleSort.Sort<int>(array);

// 2. 統計収集あり
var stats = new StatisticsContext();
BubbleSort.Sort(array.AsSpan(), ref stats);
Console.WriteLine($"Comparisons: {stats.CompareCount}, Swaps: {stats.SwapCount}");

// 3. ビジュアライズ（描画）
var viz = new VisualizationContext(
    onCompare: (i, j, result) => HighlightCompare(i, j),
    onSwap: (i, j) => AnimateSwap(i, j),
    onAccess: (index) => HighlightAccess(index)
);
BubbleSort.Sort(array.AsSpan(), ref viz);

// 4. ビジュアライズ（将来）
var viz = new VisualizationContext(onSwap: (i, j) => Render(i, j));
BubbleSort.Sort(array.AsSpan(), ref viz);

// 4. 並行実行（各スレッドが独自のContextを持つ）
Parallel.ForEach(arrays, array =>
{
    var localStats = new StatisticsContext();
    BubbleSort.Sort(array.AsSpan(), ref localStats);
    // localStatsは各スレッドで独立
});
```

**パフォーマンス特性：**

| Context | 仮想呼び出し | インライン化 | 実行時オーバーヘッド |
|---------|------------|------------|-------------------|
| `NullContext` (struct) | なし | ◎ 完全インライン化 | **ゼロ**（引数も最適化で除去） |
| `StatisticsContext` (struct) | なし | ◎ 完全インライン化 | 最小（カウントのみ、引数は無視） |
| `VisualizationContext` (class) | あり | △ 部分的 | あり（コールバック + 引数渡し） |

**Note：** 位置情報（インデックス）を引数で渡すことで、統計収集では無視し、ビジュアライズでは活用できます。構造体Contextの場合、JITが未使用引数を最適化で除去するため、パフォーマンスへの影響は最小限です。

---

### 案2: インターフェース直接渡し（シンプル版）

ジェネリクスを使わず、インターフェースで受け取る。

```csharp
public static class BubbleSort
{
    public static void Sort<T>(Span<T> span) where T : IComparable<T>
        => Sort(span, null);

    public static void Sort<T>(Span<T> span, ISortContext? context) where T : IComparable<T>
    {
        for (var i = 0; i < span.Length; i++)
        {
            for (var j = span.Length - 1; j > i; j--)
            {
                context?.OnIndexAccess(j);
                context?.OnIndexAccess(j - 1);
                
                var cmp = span[j].CompareTo(span[j - 1]);
                context?.OnCompare(j, j - 1, cmp);
                
                if (cmp < 0)
                {
                    context?.OnSwap(j, j - 1);
                    (span[j], span[j - 1]) = (span[j - 1], span[j]);
                }
            }
        }
    }
}
```

**メリット：**
- ✅ シンプルなAPI
- ✅ `null` 渡しで統計なし

**デメリット：**
- ⚠️ `null` チェックのオーバーヘッド（JITが最適化する可能性はあるが保証なし）
- ⚠️ 仮想呼び出しが残る

---

### 案3: 戻り値で統計を返す（純粋関数型）

統計が必要な場合は戻り値で返す。Context不要。

**注意：** この案はビジュアライズに対応できないため、統計のみの用途に限定されます。

```csharp
public readonly record struct SortResult(
    ulong CompareCount,
    ulong SwapCount,
    ulong IndexAccessCount);

public static class BubbleSort
{
    // 統計なし
    public static void Sort<T>(Span<T> span) where T : IComparable<T>
    {
        // ソートロジックのみ
    }

    // 統計あり：結果を戻り値で返す
    public static SortResult SortWithStatistics<T>(Span<T> span) where T : IComparable<T>
    {
        ulong compareCount = 0, swapCount = 0, indexAccessCount = 0;
        
        // ソートロジック + カウント
        
        return new SortResult(compareCount, swapCount, indexAccessCount);
    }
}
```

**メリット：**
- ✅ 完全にステートレス
- ✅ APIが最もシンプル

**デメリット：**
- ⚠️ コード重複（統計あり/なしで2つの実装が必要）
- ⚠️ ビジュアライズ対応不可

---

## 比較表

| 観点 | 案1: Generic Struct | 案2: Interface | 案3: 戻り値 |
|------|-------------------|----------------|------------|
| **パフォーマンス（統計なし）** | ◎ ゼロオーバーヘッド | ○ nullチェック分 | ◎ ゼロ |
| **パフォーマンス（統計あり）** | ◎ インライン化 | ○ 仮想呼び出し | ◎ ローカル変数 |
| **ソート関数のステートレス性** | ◎ 完全 | ◎ 完全 | ◎ 完全 |
| **ビジュアライズ対応** | ◎ 可能 | ◎ 可能 | ✗ 不可 |
| **API の簡潔さ** | ○ ジェネリクスが増える | ◎ シンプル | ◎ 最もシンプル |
| **コード重複** | ◎ なし（1実装） | ◎ なし | △ 2実装必要 |
| **型安全性** | ◎ コンパイル時チェック | ○ 実行時 | ◎ |

---

## 推奨案

**案1: Generic Struct Context** を推奨します。

**理由：**
1. **ソート関数が完全にステートレス** - 静的メソッドで純粋関数
2. **状態はContextに逃がせる** - 統計・描画の状態はContext側が管理
3. **パフォーマンス最高** - `NullContext` 使用時はゼロオーバーヘッド
4. **将来のビジュアライズ対応が可能** - Contextを拡張するだけ
5. **コード重複なし** - 1つの実装で統計あり/なし両対応

---

## 補足：クラスContextの対応

ビジュアライズでクラスContextを使いたい場合、`ref` 渡しの制約を回避するオーバーロードを追加：

```csharp
public static class BubbleSort
{
    // 構造体Context用（ref渡し）
    public static void Sort<T, TContext>(Span<T> span, ref TContext context)
        where T : IComparable<T>
        where TContext : ISortContext
    { /* ... */ }

    // クラスContext用（参照渡し不要）
    public static void Sort<T>(Span<T> span, ISortContext context)
        where T : IComparable<T>
    {
        // 内部でラップして呼び出し
        var wrapper = new ContextWrapper(context);
        Sort(span, ref wrapper);
    }
}

// または、クラス用の別オーバーロードを用意
```

---

## ファイル構成案

```
src/SortLab.Core/
├── Contexts/
│   ├── ISortContext.cs           # インターフェース
│   ├── NullContext.cs            # No-op（struct）
│   ├── StatisticsContext.cs      # 統計収集（struct）
│   └── VisualizationContext.cs   # 描画用（class）
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
    public static int Compare<T, TContext>(Span<T> span, int i, int j, ref TContext context)
        where T : IComparable<T>
        where TContext : ISortContext
    {
        var result = span[i].CompareTo(span[j]);
        context.OnCompare(i, j, result);
        return result;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Swap<T, TContext>(Span<T> span, int i, int j, ref TContext context)
        where TContext : ISortContext
    {
        context.OnSwap(i, j);
        (span[i], span[j]) = (span[j], span[i]);
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref T Index<T, TContext>(Span<T> span, int index, ref TContext context)
        where TContext : ISortContext
    {
        context.OnIndexAccess(index);
        return ref span[index];
    }
}

// 使用例
if (SortHelper.Compare(span, j, j - 1, ref context) < 0)
{
    SortHelper.Swap(span, j, j - 1, ref context);
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

## 決定事項

1. **採用する設計** - 案1（Generic Struct Context）/ 案2 / 案3
2. **ビジュアライズ対応** - 必要 / 不要（将来対応）
3. **クラスContext対応** - オーバーロード追加 / 不要
4. **ヘルパーメソッド** - 静的クラス / 各アルゴリズム内で重複

---

## 次のステップ

1. 設計案の選択
2. `BubbleSort` での実装
3. ベンチマークで現行版との比較
4. 他アルゴリズムへの展開
