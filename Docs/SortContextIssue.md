# Sort Visualization Context Issue - Root Cause Analysis

## 問題の概要

デバッグビルドで可視化を実行すると、InsertionSort、MergeSort、HeapSort、QuickSortなど、すべてのアルゴリズムで棒グラフが正しくソートされない（または全く動かない）問題が発生している。

## 根本原因の分析

### 1. **致命的な設計上の矛盾**

#### 問題点A: `SortSpan.Swap()` の多重操作記録

`SortSpan.cs` (157-164行目):
```csharp
public void Swap(int i, int j)
{
#if DEBUG
    var a = Read(i);      // ← OnIndexRead(i) が呼ばれる
    var b = Read(j);      // ← OnIndexRead(j) が呼ばれる
    
    _context.OnSwap(i, j, _bufferId);  // ← OnSwap が呼ばれる
    
    Write(i, b);          // ← OnIndexWrite(i) が呼ばれる
    Write(j, a);          // ← OnIndexWrite(j) が呼ばれる
#else
    (_span[i], _span[j]) = (_span[j], _span[i]);
#endif
}
```

**1回の`Swap()`呼び出しで5つの操作が記録される:**
1. `IndexRead(i)`
2. `IndexRead(j)`
3. `Swap(i, j)`
4. `IndexWrite(i)`
5. `IndexWrite(j)`

#### 問題点B: `PlaybackService` は `Swap` しか配列に適用していない

`PlaybackService.cs` (225-234行目):
```csharp
case OperationType.Swap:
    State.SwapIndices.Add(operation.Index1);
    State.SwapIndices.Add(operation.Index2);
    if (applyToArray)
    {
        var arr = GetArray(operation.BufferId1);
        (arr[operation.Index1], arr[operation.Index2]) = (arr[operation.Index2], arr[operation.Index1]);
    }
    if (updateStats) State.SwapCount++;
    break;

case OperationType.IndexWrite:
    State.WriteIndices.Add(operation.Index1);
    // ← 配列への書き込みが実装されていない！
    if (updateStats) State.IndexWriteCount++;
    break;
```

**`IndexWrite`操作では配列が更新されない！**

### 2. **操作の意味的矛盾**

#### BubbleSort/GnomeSortが動作する理由

これらのアルゴリズムは`Swap()`のみを使用:
```csharp
// BubbleSort
if (s.Compare(j, j + 1) > 0)
{
    s.Swap(j, j + 1);  // ← Swap操作だけで完結
}
```

→ `Swap`操作が配列に適用されるため、正しく動作

#### InsertionSortが動作しない理由

InsertionSortはシフト操作を使用:
```csharp
// InsertionSort
var tmp = s.Read(i);              // IndexRead
while (j >= first && s.Compare(j, tmp) > 0)
{
    s.Write(j + 1, s.Read(j));    // IndexRead + IndexWrite
    j--;
}
s.Write(j + 1, tmp);              // IndexWrite
```

→ `IndexWrite`操作が配列に適用されないため、配列が変化しない

#### MergeSort/HeapSort/QuickSortが動作しない理由

これらは`Swap()`を使用するが、**Swap内部のIndexRead/IndexWriteが記録される**:

**記録される操作シーケンス:**
```
Operation #1: IndexRead(i)    ← Swap内部
Operation #2: IndexRead(j)    ← Swap内部
Operation #3: Swap(i, j)      ← これだけ適用される
Operation #4: IndexWrite(i)   ← 適用されない
Operation #5: IndexWrite(j)   ← 適用されない
```

**再生時の動作:**
1. Op #1 (IndexRead): 何もしない
2. Op #2 (IndexRead): 何もしない
3. Op #3 (Swap): 配列を入れ替える ✓
4. Op #4 (IndexWrite): 何もしない ← **問題！同じ位置に再度書き込むべきだが適用されない**
5. Op #5 (IndexWrite): 何もしない ← **問題！同じ位置に再度書き込むべきだが適用されない**

**結果:** Swap後のIndexWriteで「すでにスワップされた値」を再度書き込むべきだが、それが実行されないため、配列の状態が不整合になる

### 3. **`#if DEBUG` による二重の問題**

#### 問題点C: DEBUGビルドとRELEASEビルドで動作が異なる

`SortSpan.cs`のすべての操作が`#if DEBUG`で条件分岐:
- **DEBUGビルド**: コンテキスト呼び出し + 多重操作記録
- **RELEASEビルド**: コンテキスト呼び出しなし + 直接操作

**これは可視化にとって致命的:**
- 可視化用の操作記録はDEBUGビルドでしか実行されない
- DEBUGビルドでは不要な操作（Swap内のRead/Write）まで記録される
- RELEASEビルドでは何も記録されない

### 4. **`SortExecutor` の配列管理の問題**

`SortExecutor.cs` (84行目):
```csharp
algorithm.SortAction(workArray.AsSpan(0, sourceArray.Length).ToArray(), context);
```

**問題:**
- `AsSpan().ToArray()` で新しい配列を作成している
- この配列は`workArray`とは別のインスタンス
- ソート実行中に`workArray`は使われず、新しい配列が使われる
- `workArray`をArrayPoolに返却しているが、実際にソートされた配列は失われる

**結果:** ソート実行中の配列の状態を追跡できない

## 問題の影響範囲

| アルゴリズム | 動作状況 | 理由 |
|------------|---------|------|
| BubbleSort | ✓ 正常 | Swapのみ使用、IndexWriteを使わない |
| GnomeSort | ✓ 正常 | Swapのみ使用、IndexWriteを使わない |
| InsertionSort | ✗ 不動 | IndexWriteを使用するが適用されない |
| InsertionSortUnOptimized | ✗ 不動 | Swapを使用するが、Swap内のIndexWriteが干渉 |
| SelectionSort | ✗ 部分的 | Swapを使用するが、Swap内のIndexWriteが干渉 |
| MergeSort | ✗ 不動 | Swapを使用するが、Swap内のIndexWriteが干渉 |
| HeapSort | ✗ 不動 | Swapを使用するが、Swap内のIndexWriteが干渉 |
| QuickSort | ✗ 不動 | Swapを使用するが、Swap内のIndexWriteが干渉 |
| ShellSort | ✗ 不動 | Swapを使用するが、Swap内のIndexWriteが干渉 |

## 根本的な設計上の誤り

### 誤り1: 操作の粒度が一貫していない

- **Swap**: 単一の論理操作として扱うべきだが、内部で4つの操作（Read×2 + Write×2）を記録
- **Compare**: 単一の論理操作として扱われている（正しい）
- **IndexRead/IndexWrite**: 単一の操作として扱われているが、値の情報がない

### 誤り2: 操作記録と操作適用の不一致

- **記録側（SortSpan）**: Read/Write/Swap/Compareをすべて記録
- **適用側（PlaybackService）**: Swapしか配列に適用しない

この不一致により、記録された操作と実際の配列の状態が乖離する。

### 誤り3: 値の追跡メカニズムの欠如

- `IndexWrite`操作には「何を書き込むか」の情報がない
- `IndexRead`操作で読み取った値を追跡する仕組みがない
- スタックベースのアプローチを試みたが、Swap内の多重操作により破綻

## 解決策の検討

### オプション1: Swapの記録を単純化（推奨）

**変更点:**
- `SortSpan.Swap()` では `OnSwap` **のみ**を呼び出す
- `Read(i)`, `Read(j)`, `Write(i, b)`, `Write(j, a)` のコンテキスト呼び出しを削除
- Swap内部の操作は記録しない

**利点:**
- 操作の粒度が一貫する
- BubbleSort/GnomeSortは引き続き動作
- Swap内のRead/Writeが記録されないため、干渉がなくなる

**欠点:**
- Swap内のRead/Writeの統計が取れない（可視化には不要）

### オプション2: IndexWriteに値を追加（複雑）

**変更点:**
- `ISortContext.OnIndexWrite` に値パラメータを追加
- ジェネリック型の問題を解決（object型、またはint専用インターフェース）
- `SortOperation` に `Value` フィールドを追加
- `PlaybackService` で値を適用

**利点:**
- すべての操作で配列の状態を正確に再現可能

**欠点:**
- ジェネリック型とボクシングの問題
- 実装が複雑化
- Swapの多重記録問題は残る

### オプション3: 可視化専用の操作記録モード（大規模変更）

**変更点:**
- `#if DEBUG` ではなく、`#if VISUALIZATION` など専用の条件を使用
- 可視化モードでは、Swapを単一操作として記録
- 通常モードでは、詳細な統計のためにすべての操作を記録

**利点:**
- 用途に応じた最適な記録が可能

**欠点:**
- ビルド設定の複雑化
- 2つのモードのメンテナンスが必要

### オプション4: 操作の後処理で統合（中間案）

**変更点:**
- すべての操作を記録（現状維持）
- `SortExecutor` で後処理を実行
- 連続する `IndexRead(i) → IndexRead(j) → Swap(i,j) → IndexWrite(i) → IndexWrite(j)` のパターンを検出
- これらを単一の `Swap(i,j)` 操作に統合
- 孤立した `IndexWrite` には値を推定して追加

**利点:**
- `SortSpan` の変更が不要
- 既存の統計機能を維持

**欠点:**
- パターンマッチングが複雑
- すべてのパターンをカバーできない可能性

## 推奨される解決策

**オプション1（Swapの単純化）+ IndexWriteの値追跡**

### フェーズ1: Swapの単純化（即座に実装可能）

1. `SortSpan.Swap()` を修正:
```csharp
public void Swap(int i, int j)
{
#if DEBUG
    _context.OnSwap(i, j, _bufferId);
#endif
    (_span[i], _span[j]) = (_span[j], _span[i]);
}
```

2. `PlaybackService.ApplyOperation` はそのまま（Swapは既に実装済み）

**結果:** BubbleSort、GnomeSort、および他のSwap系アルゴリズムが正常に動作

### フェーズ2: IndexWriteの値追跡（InsertionSort対応）

1. `SortSpan.Write()` を修正して、直前の`Read()`の値を記憶
2. `SortExecutor` でRead→Writeのペアを検出して値を記録
3. `PlaybackService` でIndexWriteに値を適用

**結果:** InsertionSortなどのシフト系アルゴリズムも正常に動作

## まとめ

**現在の可視化が動作しない根本原因:**

1. ✗ `SortSpan.Swap()` が1回の呼び出しで5つの操作を記録（Read×2 + Swap + Write×2）
2. ✗ `PlaybackService` が `IndexWrite` を配列に適用していない
3. ✗ Swap内部のIndexWriteが記録されるが適用されず、配列の状態が破綻
4. ✗ IndexWriteに値の情報がなく、配列を更新できない
5. ✗ `SortExecutor` が配列を複製して渡すため、状態追跡が困難

**最も重要な修正:**
- `SortSpan.Swap()` でRead/Writeのコンテキスト呼び出しを削除
- `PlaybackService` でIndexWriteを配列に適用（値の追跡が必要）

この2点を修正すれば、すべてのソートアルゴリズムで可視化が正常に動作するはずです。
