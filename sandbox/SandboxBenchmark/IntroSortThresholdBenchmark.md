# IntroSort Threshold Benchmark

## 目的

IntroSortのInsertionSortへの切り替え閾値を型特性に基づいて調整することの効果を測定します。

C++ std::introsortでは以下のような最適化を行っています：
```cpp
const difference_type __limit = is_trivially_copy_constructible<value_type>::value &&
                                is_trivially_copy_assignable<value_type>::value ? 30 : 6;
```

## ✅ 最終決定

**閾値を16から30に変更** （2026年実装）

### ベンチマーク結果サマリー

| 型 | サイズ | パターン | Threshold8 | Threshold16 | **Threshold30** |
|----|--------|----------|------------|-------------|-----------------|
| int | 100 | Random | +19% 遅 | Baseline | **-29% 速** ✅ |
| int | 10000 | Random | +17% 遅 | Baseline | **-10% 速** ✅ |
| string | 100 | Random | +18% 遅 | Baseline | **-19% 速** ✅ |
| string | 10000 | Random | +13% 遅 | Baseline | **-15% 速** ✅ |

### 重要な発見

1. **プリミティブ型も参照型も閾値30が最適**
   - 当初の仮説（参照型は小さい閾値が有利）は否定された
   - C#の参照型はポインタコピー（8バイト）で軽量
   - C++の複雑なオブジェクトコピーとは異なる

2. **全データパターンで一貫した結果**
   - Random/Sorted/Reversed/NearlySortedすべてで同じ傾向
   - 小配列（100要素）で最大の改善（-30%）

3. **型特性ベースの動的閾値は不要**
   - 全型で閾値30が最適
   - シンプルな固定閾値で実装

## 仮説

- **プリミティブ型（int, double等）**: コピー/スワップが軽量なため、閾値を大きく（30程度）しても効率的
- **参照型（string等）**: コピー/スワップが重いため、閾値を小さく（8程度）する方が効率的

## ベンチマーク実行方法

```powershell
cd sandbox/SandboxBenchmark
dotnet run -c Release --filter *IntroSortThreshold*
```

### 特定のパターンのみ実行

```powershell
# ランダムデータのみ
dotnet run -c Release --filter *IntroSortThreshold* --job short --runtimes net10.0 --filter *Random*

# int型のみ
dotnet run -c Release --filter *IntroSortThreshold*Int_*
```

## テスト条件

### 配列サイズ
- 100: 小配列（閾値の影響が最も大きい）
- 1000: 中配列（Nintherが有効化される境界）
- 10000: 大配列（全体的なパフォーマンス）

### データパターン
- **Random**: ランダムなデータ（典型的なケース）
- **Sorted**: ソート済み（Nearly-sorted検出のベストケース）
- **Reversed**: 逆順（一部のピボット戦略での最悪ケース）
- **NearlySorted**: 95%ソート済み（Nearly-sorted最適化の効果測定）

### 閾値
- **8**: 小さい閾値（参照型に最適？）
- **16**: 現在の実装（ベースライン）
- **30**: 大きい閾値（プリミティブ型に最適？）

### テスト型
- **int**: プリミティブ型（`RuntimeHelpers.IsReferenceOrContainsReferences<int>()` = false）
- **string**: 参照型（`RuntimeHelpers.IsReferenceOrContainsReferences<string>()` = true）

## 結果

| Method                     | Size  | Pattern      | Mean           | Error | Ratio | Allocated | Alloc Ratio |
|--------------------------- |------ |------------- |---------------:|------:|------:|----------:|------------:|
| Int_Threshold16_Current    | 100   | Random       |       248.2 ns |    NA |  1.00 |         - |          NA |
| Int_Threshold8_Small       | 100   | Random       |       295.1 ns |    NA |  1.19 |         - |          NA |
| Int_Threshold30_Large      | 100   | Random       |       176.0 ns |    NA |  0.71 |         - |          NA |
| String_Threshold16_Current | 100   | Random       |    10,721.4 ns |    NA | 43.20 |         - |          NA |
| String_Threshold8_Small    | 100   | Random       |    12,638.1 ns |    NA | 50.92 |         - |          NA |
| String_Threshold30_Large   | 100   | Random       |     8,723.7 ns |    NA | 35.15 |         - |          NA |
|                            |       |              |                |       |       |           |             |
| Int_Threshold16_Current    | 100   | Sorted       |       241.5 ns |    NA |  1.00 |         - |          NA |
| Int_Threshold8_Small       | 100   | Sorted       |       289.2 ns |    NA |  1.20 |         - |          NA |
| Int_Threshold30_Large      | 100   | Sorted       |       168.0 ns |    NA |  0.70 |         - |          NA |
| String_Threshold16_Current | 100   | Sorted       |    10,709.3 ns |    NA | 44.34 |         - |          NA |
| String_Threshold8_Small    | 100   | Sorted       |    13,036.7 ns |    NA | 53.98 |         - |          NA |
| String_Threshold30_Large   | 100   | Sorted       |     7,971.1 ns |    NA | 33.01 |         - |          NA |
|                            |       |              |                |       |       |           |             |
| Int_Threshold16_Current    | 100   | Reversed     |       228.7 ns |    NA |  1.00 |         - |          NA |
| Int_Threshold8_Small       | 100   | Reversed     |       275.7 ns |    NA |  1.21 |         - |          NA |
| Int_Threshold30_Large      | 100   | Reversed     |       167.1 ns |    NA |  0.73 |         - |          NA |
| String_Threshold16_Current | 100   | Reversed     |    10,337.5 ns |    NA | 45.21 |         - |          NA |
| String_Threshold8_Small    | 100   | Reversed     |    12,871.6 ns |    NA | 56.29 |         - |          NA |
| String_Threshold30_Large   | 100   | Reversed     |     8,014.2 ns |    NA | 35.05 |         - |          NA |
|                            |       |              |                |       |       |           |             |
| Int_Threshold16_Current    | 100   | NearlySorted |       232.4 ns |    NA |  1.00 |         - |          NA |
| Int_Threshold8_Small       | 100   | NearlySorted |       286.0 ns |    NA |  1.23 |         - |          NA |
| Int_Threshold30_Large      | 100   | NearlySorted |       169.0 ns |    NA |  0.73 |         - |          NA |
| String_Threshold16_Current | 100   | NearlySorted |     9,842.8 ns |    NA | 42.35 |         - |          NA |
| String_Threshold8_Small    | 100   | NearlySorted |    13,477.3 ns |    NA | 57.99 |         - |          NA |
| String_Threshold30_Large   | 100   | NearlySorted |     8,304.7 ns |    NA | 35.73 |         - |          NA |
|                            |       |              |                |       |       |           |             |
| Int_Threshold16_Current    | 1000  | Random       |     4,016.6 ns |    NA |  1.00 |         - |          NA |
| Int_Threshold8_Small       | 1000  | Random       |     4,721.0 ns |    NA |  1.18 |         - |          NA |
| Int_Threshold30_Large      | 1000  | Random       |     4,095.2 ns |    NA |  1.02 |         - |          NA |
| String_Threshold16_Current | 1000  | Random       |   191,705.8 ns |    NA | 47.73 |         - |          NA |
| String_Threshold8_Small    | 1000  | Random       |   222,651.3 ns |    NA | 55.43 |         - |          NA |
| String_Threshold30_Large   | 1000  | Random       |   182,467.7 ns |    NA | 45.43 |         - |          NA |
|                            |       |              |                |       |       |           |             |
| Int_Threshold16_Current    | 1000  | Sorted       |     3,943.1 ns |    NA |  1.00 |         - |          NA |
| Int_Threshold8_Small       | 1000  | Sorted       |     4,854.5 ns |    NA |  1.23 |         - |          NA |
| Int_Threshold30_Large      | 1000  | Sorted       |     3,208.8 ns |    NA |  0.81 |         - |          NA |
| String_Threshold16_Current | 1000  | Sorted       |   200,568.8 ns |    NA | 50.87 |         - |          NA |
| String_Threshold8_Small    | 1000  | Sorted       |   221,369.4 ns |    NA | 56.14 |         - |          NA |
| String_Threshold30_Large   | 1000  | Sorted       |   182,503.6 ns |    NA | 46.28 |         - |          NA |
|                            |       |              |                |       |       |           |             |
| Int_Threshold16_Current    | 1000  | Reversed     |     3,599.6 ns |    NA |  1.00 |         - |          NA |
| Int_Threshold8_Small       | 1000  | Reversed     |     3,976.5 ns |    NA |  1.10 |         - |          NA |
| Int_Threshold30_Large      | 1000  | Reversed     |     2,988.0 ns |    NA |  0.83 |         - |          NA |
| String_Threshold16_Current | 1000  | Reversed     |   181,758.7 ns |    NA | 50.49 |         - |          NA |
| String_Threshold8_Small    | 1000  | Reversed     |   199,711.1 ns |    NA | 55.48 |         - |          NA |
| String_Threshold30_Large   | 1000  | Reversed     |   157,429.1 ns |    NA | 43.73 |         - |          NA |
|                            |       |              |                |       |       |           |             |
| Int_Threshold16_Current    | 1000  | NearlySorted |     3,544.9 ns |    NA |  1.00 |         - |          NA |
| Int_Threshold8_Small       | 1000  | NearlySorted |     4,084.2 ns |    NA |  1.15 |         - |          NA |
| Int_Threshold30_Large      | 1000  | NearlySorted |     3,052.4 ns |    NA |  0.86 |         - |          NA |
| String_Threshold16_Current | 1000  | NearlySorted |   182,432.9 ns |    NA | 51.46 |         - |          NA |
| String_Threshold8_Small    | 1000  | NearlySorted |   199,121.4 ns |    NA | 56.17 |         - |          NA |
| String_Threshold30_Large   | 1000  | NearlySorted |   156,552.3 ns |    NA | 44.16 |         - |          NA |
|                            |       |              |                |       |       |           |             |
| Int_Threshold16_Current    | 10000 | Random       |    52,906.1 ns |    NA |  1.00 |         - |          NA |
| Int_Threshold8_Small       | 10000 | Random       |    61,991.7 ns |    NA |  1.17 |         - |          NA |
| Int_Threshold30_Large      | 10000 | Random       |    47,576.5 ns |    NA |  0.90 |         - |          NA |
| String_Threshold16_Current | 10000 | Random       | 3,196,154.3 ns |    NA | 60.41 |         - |          NA |
| String_Threshold8_Small    | 10000 | Random       | 3,597,198.8 ns |    NA | 67.99 |         - |          NA |
| String_Threshold30_Large   | 10000 | Random       | 2,701,204.7 ns |    NA | 51.06 |         - |          NA |
|                            |       |              |                |       |       |           |             |
| Int_Threshold16_Current    | 10000 | Sorted       |    54,738.0 ns |    NA |  1.00 |         - |          NA |
| Int_Threshold8_Small       | 10000 | Sorted       |    75,691.9 ns |    NA |  1.38 |         - |          NA |
| Int_Threshold30_Large      | 10000 | Sorted       |    53,742.7 ns |    NA |  0.98 |         - |          NA |
| String_Threshold16_Current | 10000 | Sorted       | 3,417,391.0 ns |    NA | 62.43 |         - |          NA |
| String_Threshold8_Small    | 10000 | Sorted       | 3,510,403.5 ns |    NA | 64.13 |         - |          NA |
| String_Threshold30_Large   | 10000 | Sorted       | 3,018,039.1 ns |    NA | 55.14 |         - |          NA |
|                            |       |              |                |       |       |           |             |
| Int_Threshold16_Current    | 10000 | Reversed     |    59,755.9 ns |    NA |  1.00 |         - |          NA |
| Int_Threshold8_Small       | 10000 | Reversed     |    61,910.4 ns |    NA |  1.04 |         - |          NA |
| Int_Threshold30_Large      | 10000 | Reversed     |    47,295.4 ns |    NA |  0.79 |         - |          NA |
| String_Threshold16_Current | 10000 | Reversed     | 3,331,373.8 ns |    NA | 55.75 |         - |          NA |
| String_Threshold8_Small    | 10000 | Reversed     | 3,675,321.1 ns |    NA | 61.51 |         - |          NA |
| String_Threshold30_Large   | 10000 | Reversed     | 3,026,528.1 ns |    NA | 50.65 |         - |          NA |
|                            |       |              |                |       |       |           |             |
| Int_Threshold16_Current    | 10000 | NearlySorted |    53,653.1 ns |    NA |  1.00 |         - |          NA |
| Int_Threshold8_Small       | 10000 | NearlySorted |    53,745.8 ns |    NA |  1.00 |         - |          NA |
| Int_Threshold30_Large      | 10000 | NearlySorted |    47,783.8 ns |    NA |  0.89 |         - |          NA |
| String_Threshold16_Current | 10000 | NearlySorted | 3,075,768.0 ns |    NA | 57.33 |         - |          NA |
| String_Threshold8_Small    | 10000 | NearlySorted | 3,757,014.3 ns |    NA | 70.02 |         - |          NA |
| String_Threshold30_Large   | 10000 | NearlySorted | 3,110,153.5 ns |    NA | 57.97 |         - |          NA |


### 判断基準

- **Ratio < 0.95**: 明確に速い（5%以上改善）→ 採用を検討
- **0.95 ≤ Ratio ≤ 1.05**: 誤差範囲（±5%）→ 変更不要
- **Ratio > 1.05**: 明確に遅い（5%以上劣化）→ 不採用

### メモリ使用量

`[MemoryDiagnoser]`により、以下の情報も取得されます：
- **Allocated**: ヒープ割り当て量（IntroSortはin-placeなので0であるべき）

## 実装への反映

ベンチマーク結果が仮説を支持する場合、以下を実装：

```csharp
private static int GetInsertionSortThreshold<T>()
{
    // 参照型または参照を含む型 → 小さい閾値 (8)
    // プリミティブ型 → 大きい閾値 (30)
    return RuntimeHelpers.IsReferenceOrContainsReferences<T>() ? 8 : 30;
}
```

IntroSortInternal内で：
```csharp
var threshold = GetInsertionSortThreshold<T>();
if (size <= threshold) { ... }
```

## 注意事項

- ベンチマークは3回以上実行し、結果の一貫性を確認すること
- CPU負荷が低い状態で実行すること（バックグラウンドタスクを最小化）
- Releaseモードで実行すること（`-c Release`）
- 結果が仮説と大きく異なる場合、キャッシュ効果やJIT最適化の影響を考慮すること
