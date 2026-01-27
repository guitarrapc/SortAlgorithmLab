# 可視化の要件分析 - ゴールから逆算する設計

## アプローチ

棒グラフの描画（ゴール）から逆算して、必要な情報とインターフェース設計を決定する。

## 1. 棒グラフの描画に必要な操作と情報

### 1.1 配列の状態を変更する操作

可視化では、**初期状態の配列**から**各操作を順次適用**して配列の状態を再現する必要がある。

#### A. Swap（要素の交換）

**やりたいこと:**
```csharp
// 配列[i]と配列[j]を交換
(array[i], array[j]) = (array[j], array[i]);
```

**必要な情報:**
- インデックスi
- インデックスj
- バッファーID

**値の情報は必要か？**
→ **不要**。配列を持っているので、インデックスがあれば交換できる。

**現在のISortContext:**
```csharp
void OnSwap(int i, int j, int bufferId);  // ✓ OK
```

#### B. IndexWrite（単一要素の書き込み / シフト操作）

**やりたいこと:**
```csharp
// InsertionSortのシフト操作
array[j + 1] = array[j];  // 右にシフト
array[j + 1] = tmp;       // 保存した値を書き込み
```

**必要な情報:**
- インデックス
- **書き込む値**
- バッファーID

**値の情報は必要か？**
→ **必須**。何を書き込むか分からないと配列を更新できない。

**現在のISortContext:**
```csharp
void OnIndexWrite(int index, int bufferId);  // ✗ 値が欠けている
```

**問題:** 値の情報がないため、`array[index] = ???` が実行できない。

#### C. RangeCopy（範囲コピー）

**やりたいこと:**
```csharp
// MergeSortで一時配列からメイン配列へコピー
sourceArray[sourceIndex..(sourceIndex + length)]
    .CopyTo(destArray[destIndex..]);
```

**必要な情報:**
- ソースインデックス、長さ
- デスティネーションインデックス
- ソースバッファーID、デスティネーションバッファーID

**値の情報は必要か？**
→ **不要**。両方のバッファーを持っているので、範囲情報があればコピーできる。

**現在のISortContext:**
```csharp
void OnRangeCopy(int sourceIndex, int destinationIndex, int length, 
                 int sourceBufferId, int destinationBufferId);  // ✓ OK
```

### 1.2 ハイライト表示のための操作

配列は変更しないが、視覚的なフィードバックのために記録する操作。

#### D. Compare（比較）

**やりたいこと:**
- 比較中の2つのバーをハイライト
- 統計情報の更新

**必要な情報:**
- インデックスi、j
- 比較結果（統計用）
- バッファーID

**現在のISortContext:**
```csharp
void OnCompare(int i, int j, int result, int bufferIdI, int bufferIdJ);  // ✓ OK
```

#### E. IndexRead（読み取り）

**やりたいこと:**
- 読み取り中のバーをハイライト
- 統計情報の更新

**必要な情報:**
- インデックス
- バッファーID

**現在のISortContext:**
```csharp
void OnIndexRead(int index, int bufferId);  // ✓ OK
```

## 2. 現在の実装の問題点（逆算視点）

### 問題1: IndexWriteに値がない

**必要な処理:**
```csharp
// PlaybackServiceでの再生時
case OperationType.IndexWrite:
    array[operation.Index] = operation.Value;  // ← Valueが必要
```

**現状:**
```csharp
case OperationType.IndexWrite:
    State.WriteIndices.Add(operation.Index1);
    // 配列への書き込みができない！
```

**結論:** `OnIndexWrite` に値パラメータが必須。

### 問題2: Swapが多重操作を記録

**現在のSortSpan.Swap():**
```csharp
var a = Read(i);      // OnIndexRead(i) が記録される
var b = Read(j);      // OnIndexRead(j) が記録される
OnSwap(i, j);         // OnSwap が記録される
Write(i, b);          // OnIndexWrite(i) が記録される
Write(j, a);          // OnIndexWrite(j) が記録される
```

**記録される操作:**
1. IndexRead(i)
2. IndexRead(j)
3. Swap(i, j)
4. IndexWrite(i)
5. IndexWrite(j)

**再生時の処理:**
```csharp
1. IndexRead(i)    → 何もしない
2. IndexRead(j)    → 何もしない
3. Swap(i, j)      → array[i]とarray[j]を交換  ✓
4. IndexWrite(i)   → array[i] = ??? （値がない）
5. IndexWrite(j)   → array[j] = ??? （値がない）
```

**問題:**
- 操作3でSwapが完了しているのに、操作4と5で再度書き込もうとしている
- しかし値がないので書き込めず、かつ書き込む必要もない
- **Swapは単一の論理操作として扱うべき**

**必要な処理:**
```csharp
// 棒グラフの描画に必要なのは、これだけ
case OperationType.Swap:
    (array[i], array[j]) = (array[j], array[i]);
```

**結論:** `SortSpan.Swap()` は `OnSwap` だけを呼び、Read/Writeを呼ばない。

## 3. 理想的なインターフェース設計（逆算結果）

### 3.1 配列変更操作

棒グラフの描画（配列の状態再現）に必要な情報：

```csharp
public interface ISortContext
{
    // Swap: インデックスのペアで十分（値は不要）
    void OnSwap(int i, int j, int bufferId);
    
    // IndexWrite: 値が必須
    void OnIndexWrite(int index, int bufferId, T value);  // ← Tが問題
    
    // RangeCopy: 範囲情報で十分（値は不要）
    void OnRangeCopy(int sourceIndex, int destinationIndex, int length, 
                     int sourceBufferId, int destinationBufferId);
}
```

### 3.2 ジェネリック型の問題と解決策

#### 問題: `OnIndexWrite(int index, int bufferId, T value)`

`ISortContext` は非ジェネリックインターフェース。
`SortSpan<T>` はジェネリック型。

**解決策オプション:**

#### オプションA: ISortContextをジェネリック化

```csharp
public interface ISortContext<T> where T : IComparable<T>
{
    void OnIndexWrite(int index, int bufferId, T value);
}

// 使用側
var context = new VisualizationContext<int>(...);
```

**利点:**
- 型安全
- ボクシング不要

**欠点:**
- すべてのコンテキスト実装がジェネリック化される
- 既存のコードへの影響が大きい

#### オプションB: object型で値を渡す

```csharp
public interface ISortContext
{
    void OnIndexWrite(int index, int bufferId, object value);
}

// SortSpan側
public void Write(int i, T value)
{
    _context.OnIndexWrite(i, _bufferId, value);  // ボクシング発生
    _span[i] = value;
}
```

**利点:**
- インターフェース変更が最小限
- 既存コードへの影響が少ない

**欠点:**
- ボクシング/アンボクシングのコスト
- 型安全性の喪失

#### オプションC: 可視化専用インターフェース

```csharp
// 既存の統計用インターフェース
public interface ISortContext
{
    void OnIndexWrite(int index, int bufferId);
}

// 可視化専用インターフェース
public interface IVisualizationContext : ISortContext
{
    void OnIndexWriteWithValue(int index, int bufferId, object value);
}

// SortSpan側
public void Write(int i, T value)
{
    if (_context is IVisualizationContext visContext)
    {
        visContext.OnIndexWriteWithValue(i, _bufferId, value);
    }
    else
    {
        _context.OnIndexWrite(i, _bufferId);
    }
    _span[i] = value;
}
```

**利点:**
- 統計用と可視化用を分離
- 既存の統計機能は影響を受けない

**欠点:**
- 実装が複雑化
- 型チェックのオーバーヘッド

#### オプションD: 可視化は `int` 専用と割り切る

```csharp
// 統計用の汎用インターフェース
public interface ISortContext
{
    void OnIndexWrite(int index, int bufferId);
}

// 可視化専用インターフェース（int専用）
public interface IIntVisualizationContext : ISortContext
{
    void OnIndexWrite(int index, int bufferId, int value);
}

// SortSpan側
public void Write(int i, T value)
{
    if (_context is IIntVisualizationContext intVisContext && value is int intValue)
    {
        intVisContext.OnIndexWrite(i, _bufferId, intValue);
    }
    else
    {
        _context.OnIndexWrite(i, _bufferId);
    }
    _span[i] = value;
}
```

**利点:**
- 可視化用途に特化
- ボクシング不要（int型のまま）
- 統計機能は影響を受けない

**欠点:**
- int以外の型の可視化はできない（現時点では不要）
- メソッドシグネチャの重複

## 4. 推奨される実装方針（逆算結果）

### フェーズ1: Swapの単純化（即座に実装可能）

**目的:** BubbleSort、GnomeSort、およびSwap系アルゴリズムを正常動作させる

**変更:**
```csharp
// SortSpan.cs
public void Swap(int i, int j)
{
#if DEBUG
    _context.OnSwap(i, j, _bufferId);  // これだけ
#endif
    (_span[i], _span[j]) = (_span[j], _span[i]);
}
```

**理由:**
- Swap操作に値の情報は不要
- Read/Writeを記録すると多重操作になり混乱する
- 棒グラフの描画には `(array[i], array[j]) = (array[j], array[i])` だけで十分

**効果:**
- BubbleSort、GnomeSort、SelectionSort、HeapSort、QuickSortが正常に動作
- 操作の粒度が一貫する

### フェーズ2: IndexWriteへの値追加（InsertionSort対応）

**目的:** InsertionSortなどのシフト系アルゴリズムを正常動作させる

**オプション2B: object型（シンプル）を推奨**

**変更1: ISortContext**
```csharp
public interface ISortContext
{
    void OnIndexWrite(int index, int bufferId, object? value = null);
}
```

**変更2: SortSpan.Write()**
```csharp
public void Write(int i, T value)
{
#if DEBUG
    _context.OnIndexWrite(i, _bufferId, value);  // 値を渡す
#endif
    _span[i] = value;
}
```

**変更3: VisualizationContext**
```csharp
public VisualizationContext(
    Action<int, int, object?>? onIndexWrite = null,
    ...)
```

**変更4: SortOperation**
```csharp
public record SortOperation
{
    public int? Value { get; init; }  // nullable int
}
```

**変更5: PlaybackService**
```csharp
case OperationType.IndexWrite:
    if (applyToArray && operation.Value.HasValue)
    {
        var arr = GetArray(operation.BufferId1);
        arr[operation.Index1] = operation.Value.Value;
    }
```

**理由:**
- ボクシングは可視化の用途では許容可能（パフォーマンスクリティカルではない）
- 実装がシンプル
- 既存コードへの影響が少ない

**効果:**
- InsertionSortが正常に動作
- すべてのソートアルゴリズムで棒グラフが正しく描画される

## 5. まとめ: 逆算から得られた設計方針

### 棒グラフの描画に必要な情報（ゴール）

| 操作 | 必要な情報 | 値は必要か？ | 現状 |
|------|-----------|------------|------|
| Swap | インデックスi, j, バッファーID | **不要** | ✓ OK |
| IndexWrite | インデックス, バッファーID, **値** | **必須** | ✗ 値がない |
| RangeCopy | 範囲情報, バッファーID | **不要** | ✓ OK |
| Compare | インデックスi, j, 結果, バッファーID | 不要 | ✓ OK |
| IndexRead | インデックス, バッファーID | 不要 | ✓ OK |

### 実装の優先順位

1. **最優先: Swapの単純化**
   - `SortSpan.Swap()` で `OnSwap` のみを呼ぶ
   - Read/Writeの呼び出しを削除
   - → 大半のアルゴリズムが動作

2. **次点: IndexWriteに値を追加**
   - `OnIndexWrite(int index, int bufferId, object? value)`
   - `SortOperation` に `Value` フィールド追加
   - `PlaybackService` で値を適用
   - → InsertionSortも動作

### 設計原則（逆算から学んだこと）

1. **操作の粒度を統一**: Swapは単一操作、内部のRead/Writeは記録しない
2. **必要十分な情報**: 配列の状態を再現するために必要な情報だけを渡す
3. **値の追跡**: IndexWriteには書き込む値が必須
4. **シンプル優先**: ボクシングは許容、複雑な型システムは避ける

この分析により、**ゴールから逆算した明確な実装方針**が得られました。
