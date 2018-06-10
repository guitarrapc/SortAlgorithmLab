# SortAlgorithm

This repository shows implementation for the Major Sort Algorithm.
Aim not to use LINQ or similar ease to use, but memory unefficient technique.

Suppose to work on following platform.

Language | Platform | Remarks
---- | ---- | ----
C# | .NETStandard 2.0 | Code can be use on NET 3.5 or higher.
Go | ?? | not yet.
PowerShell | PowerShell Core | Not yet.
Pythom | 3.x | Not yet. 
Swift | 4.x | not yet.

## TODO

* [ ] Benchmark (preparing)
* [x] Data
* [x] Chart

## Result

### Better algorithm for Numerics

Algorithm | Stable | Space | Order | Remarks
---- | ---- | ---- | ---- | ----
Counting Sort | O | n + r | n + r | Need numeric key for T use.
Radix4 Sort (LSD) | O | n + 2^d | n*k/d | Need numeric key for T use. (Radix10 use mod.)

### Better always nice performance for general purpose

Algorithm | Stable | Space | Order | Remarks
---- | ---- | ---- | ---- | ----
QuickSort Median9(with BinaryInsertSort) | X | log n | n log n | May better on "Reversed, Mountain and NearlySorted" cases. But InsertSort shows a bit better performance on random.
QuickSort Median9(with InsertSort) | X | log n | n log n | Better than pure QuickSort Median 3 version and DualPivot QuickSort.
QuickSort DualPivot(InsertSort) | X | log 2n | n log n | Better than Median 3. Slightly unefficient on "Reversed, Mountain and NearlySorted" cases.
IntroSort | X | log n | n log n | MergeSort has some bug on this implementation. Need fix.
ShiftSort | O | n | n log n | Better than MergeSort and is Stable.
TimSort | O | n | n logg n | Not implemented yet.

### Description Chart

![](images/all.png)
![](images/by_performance/better_onlogn.png)

## Data

### Random

#### Size : 100

InputType | ArraySize | IsSorted | SortType | Algorithm | IndexAccessCount | CompareCount | SwapCount
---- | ---- | ---- | ---- | ---- | ---- | ---- | ----
Random | 100 | True | Exchange | BubbleSort | 4950 | 4950 | 2255
Random | 100 | True | Exchange | OddEvenSort | 4950 | 4950 | 2255
Random | 100 | True | Exchange | CocktailShakerSort | 3274 | 3274 | 2255
Random | 100 | True | Exchange | CocktailShakerSort2 | 3725 | 3725 | 2255
Random | 100 | True | Exchange | CombSort | 1195 | 1195 | 240
Random | 100 | True | Exchange | CycleSort | 14372 | 14410 | 97
Random | 100 | True | Exchange | GnomeSort | 2355 | 2255 | 2255
Random | 100 | True | Exchange | GnomeSort1 | 4604 | 4604 | 2255
Random | 100 | True | Exchange | GnomeSort2 | 2256 | 2349 | 2255
Random | 100 | True | Exchange | GnomeSort3 | 4610 | 4610 | 2255
Random | 100 | True | Selection | SelectionSort | 4950 | 4950 | 100
Random | 100 | True | Selection | HeapSort | 721 | 1061 | 629
Random | 100 | True | Insertion | InsertSort | 2255 | 2255 | 2255
Random | 100 | True | Insertion | BinaryInsertSort | 2983 | 530 | 2354
Random | 100 | True | Insertion | ShellSort | 473 | 282 | 473
Random | 100 | True | Insertion | BinaryTreeSort | 100 | 540 | 0
Random | 100 | True | Partition | QuickSortMedian3 | 384 | 674 | 204
Random | 100 | True | Partition | QuickSortMedian9 | 287 | 1175 | 219
Random | 100 | True | Partition | QuickDualPivotSort | 381 | 478 | 307
Random | 100 | True | Partition | QuickSortMedian3Insert | 283 | 262 | 128
Random | 100 | True | Partition | QuickSortMedian9Insert | 246 | 264 | 161
Random | 100 | True | Partition | QuickDualPivotSortInsert | 254 | 294 | 171
Random | 100 | True | Partition | QuickSortMedian3BinaryInsert | 401 | 338 | 142
Random | 100 | True | Partition | QuickSortMedian9BinaryInsert | 374 | 347 | 176
Random | 100 | True | Partition | QuickDualPivotSortBinaryInsert | 254 | 294 | 171
Random | 100 | True | Merge | MergeSort | 536 | 540 | 621
Random | 100 | True | Merge | MergeSort2 | 771 | 543 | 672
Random | 100 | True | Merge | ShiftSort | 836 | 626 | 9
Random | 100 | True | Distributed | BucketSort | 200 | 100 | 0
Random | 100 | True | Distributed | RadixLSD10Sort | 420 | 200 | 0
Random | 100 | True | Distributed | RadixLSD4Sort | 2781 | 0 | 0
Random | 100 | True | Distributed | CountingSort | 399 | 0 | 0
Random | 100 | True | Hybrid | IntroSortMedian9 | 199 | 235 | 127
DictionaryRamdom | 100 | True | Distributed | BucketSortT | 236 | 100 | 0

#### Size : 1000

InputType | ArraySize | IsSorted | SortType | Algorithm | IndexAccessCount | CompareCount | SwapCount
---- | ---- | ---- | ---- | ---- | ---- | ---- | ----
Random | 1000 | True | Exchange | BubbleSort | 499500 | 499500 | 247850
Random | 1000 | True | Exchange | OddEvenSort | 479520 | 479520 | 247850
Random | 1000 | True | Exchange | CocktailShakerSort | 332246 | 332246 | 247850
Random | 1000 | True | Exchange | CocktailShakerSort2 | 380672 | 380672 | 247850
Random | 1000 | True | Exchange | CombSort | 21704 | 21704 | 3913
Random | 1000 | True | Exchange | CycleSort | 1485853 | 1486328 | 996
Random | 1000 | True | Exchange | GnomeSort | 248850 | 247850 | 247850
Random | 1000 | True | Exchange | GnomeSort1 | 496695 | 496695 | 247850
Random | 1000 | True | Exchange | GnomeSort2 | 247857 | 248845 | 247850
Random | 1000 | True | Exchange | GnomeSort3 | 496700 | 496700 | 247850
Random | 1000 | True | Selection | SelectionSort | 499500 | 499500 | 1000
Random | 1000 | True | Selection | HeapSort | 10583 | 17240 | 9595
Random | 1000 | True | Insertion | InsertSort | 247850 | 247850 | 247850
Random | 1000 | True | Insertion | BinaryInsertSort | 258453 | 8605 | 248849
Random | 1000 | True | Insertion | ShellSort | 9082 | 4821 | 9082
Random | 1000 | True | Insertion | BinaryTreeSort | 1000 | 11149 | 0
Random | 1000 | True | Partition | QuickSortMedian3 | 6973 | 9962 | 2775
Random | 1000 | True | Partition | QuickSortMedian9 | 5023 | 13981 | 2971
Random | 1000 | True | Partition | QuickDualPivotSort | 6118 | 8474 | 5064
Random | 1000 | True | Partition | QuickSortMedian3Insert | 5460 | 5642 | 1625
Random | 1000 | True | Partition | QuickSortMedian9Insert | 3822 | 4673 | 1676
Random | 1000 | True | Partition | QuickDualPivotSortInsert | 5045 | 6749 | 3621
Random | 1000 | True | Partition | QuickSortMedian3BinaryInsert | 5636 | 5773 | 1640
Random | 1000 | True | Partition | QuickSortMedian9BinaryInsert | 3964 | 4779 | 1688
Random | 1000 | True | Partition | QuickDualPivotSortBinaryInsert | 5045 | 6749 | 3621
Random | 1000 | True | Merge | MergeSort | 6687 | 8679 | 9323
Random | 1000 | True | Merge | MergeSort2 | 10975 | 8697 | 9976
Random | 1000 | True | Merge | ShiftSort | 14851 | 10915 | 181
Random | 1000 | True | Distributed | BucketSort | 2000 | 1000 | 0
Random | 1000 | True | Distributed | RadixLSD10Sort | 6030 | 3000 | 0
Random | 1000 | True | Distributed | RadixLSD4Sort | 9789 | 0 | 0
Random | 1000 | True | Distributed | CountingSort | 3996 | 0 | 0
Random | 1000 | True | Hybrid | IntroSortMedian9 | 3835 | 4667 | 1644
DictionaryRamdom | 1000 | True | Distributed | BucketSortT | 2358 | 1000 | 0

#### Size : 10000

InputType | ArraySize | IsSorted | SortType | Algorithm | IndexAccessCount | CompareCount | SwapCount
---- | ---- | ---- | ---- | ---- | ---- | ---- | ----
Random | 10000 | True | Exchange | BubbleSort | 49995000 | 49995000 | 24957552
Random | 10000 | True | Exchange | OddEvenSort | 49785021 | 49785021 | 24957552
Random | 10000 | True | Exchange | CocktailShakerSort | 33342109 | 33342109 | 24957552
Random | 10000 | True | Exchange | CocktailShakerSort2 | 37795670 | 37795670 | 24957552
Random | 10000 | True | Exchange | CombSort | 306727 | 306727 | 59071
Random | 10000 | True | Exchange | CycleSort | 149790599 | 149795582 | 9998
Random | 10000 | True | Exchange | GnomeSort | 24967552 | 24957552 | 24957552
Random | 10000 | True | Exchange | GnomeSort1 | 49925091 | 49925091 | 24957552
Random | 10000 | True | Exchange | GnomeSort2 | 24957547 | 24967539 | 24957552
Random | 10000 | True | Exchange | GnomeSort3 | 49925104 | 49925104 | 24957552
Random | 10000 | True | Selection | SelectionSort | 49995000 | 49995000 | 10000
Random | 10000 | True | Selection | HeapSort | 139515 | 239326 | 129523
Random | 10000 | True | Insertion | InsertSort | 24957552 | 24957552 | 24957552
Random | 10000 | True | Insertion | BinaryInsertSort | 25096505 | 118955 | 24967551
Random | 10000 | True | Insertion | ShellSort | 151329 | 75084 | 151329
Random | 10000 | True | Insertion | BinaryTreeSort | 10000 | 161181 | 0
Random | 10000 | True | Partition | QuickSortMedian3 | 101223 | 131209 | 35908
Random | 10000 | True | Partition | QuickSortMedian9 | 72373 | 162190 | 37585
Random | 10000 | True | Partition | QuickDualPivotSort | 91750 | 124881 | 66244
Random | 10000 | True | Partition | QuickSortMedian3Insert | 85705 | 88081 | 23901
Random | 10000 | True | Partition | QuickSortMedian9Insert | 59814 | 68680 | 24175
Random | 10000 | True | Partition | QuickDualPivotSortInsert | 81749 | 108735 | 52866
Random | 10000 | True | Partition | QuickSortMedian3BinaryInsert | 85917 | 88251 | 23915
Random | 10000 | True | Partition | QuickSortMedian9BinaryInsert | 59994 | 68824 | 24187
Random | 10000 | True | Partition | QuickDualPivotSortBinaryInsert | 81749 | 108735 | 52866
Random | 10000 | True | Merge | MergeSort | 86735 | 120430 | 128158
Random | 10000 | True | Merge | MergeSort2 | 143615 | 120494 | 133616
Random | 10000 | True | Merge | ShiftSort | 211619 | 157240 | 1712
Random | 10000 | True | Distributed | BucketSort | 20000 | 10000 | 0
Random | 10000 | True | Distributed | RadixLSD10Sort | 80040 | 40000 | 0
Random | 10000 | True | Distributed | RadixLSD4Sort | 81750 | 0 | 0
Random | 10000 | True | Distributed | CountingSort | 39999 | 0 | 0
Random | 10000 | True | Hybrid | IntroSortMedian9 | 60070 | 68887 | 24280
DictionaryRamdom | 10000 | True | Distributed | BucketSortT | 23671 | 10000 | 0

### MixRandom

##### Size : 100

InputType | ArraySize | IsSorted | SortType | Algorithm | IndexAccessCount | CompareCount | SwapCount
---- | ---- | ---- | ---- | ---- | ---- | ---- | ----
MixRandom | 100 | True | Exchange | BubbleSort | 4950 | 4950 | 2583
MixRandom | 100 | True | Exchange | OddEvenSort | 4455 | 4455 | 2583
MixRandom | 100 | True | Exchange | CocktailShakerSort | 3469 | 3469 | 2583
MixRandom | 100 | True | Exchange | CocktailShakerSort2 | 4247 | 4247 | 2583
MixRandom | 100 | True | Exchange | CombSort | 1195 | 1195 | 229
MixRandom | 100 | True | Exchange | CycleSort | 14038 | 14071 | 99
MixRandom | 100 | True | Exchange | GnomeSort | 2683 | 2583 | 2583
MixRandom | 100 | True | Exchange | GnomeSort1 | 5259 | 5259 | 2583
MixRandom | 100 | True | Exchange | GnomeSort2 | 2582 | 2676 | 2583
MixRandom | 100 | True | Exchange | GnomeSort3 | 5266 | 5266 | 2583
MixRandom | 100 | True | Selection | SelectionSort | 4950 | 4950 | 100
MixRandom | 100 | True | Selection | HeapSort | 706 | 1054 | 613
MixRandom | 100 | True | Insertion | InsertSort | 2583 | 2583 | 2583
MixRandom | 100 | True | Insertion | BinaryInsertSort | 3316 | 535 | 2682
MixRandom | 100 | True | Insertion | ShellSort | 486 | 282 | 486
MixRandom | 100 | True | Insertion | BinaryTreeSort | 100 | 607 | 0
MixRandom | 100 | True | Partition | QuickSortMedian3 | 364 | 657 | 213
MixRandom | 100 | True | Partition | QuickSortMedian9 | 292 | 1180 | 223
MixRandom | 100 | True | Partition | QuickDualPivotSort | 331 | 467 | 306
MixRandom | 100 | True | Partition | QuickSortMedian3Insert | 230 | 254 | 90
MixRandom | 100 | True | Partition | QuickSortMedian9Insert | 185 | 284 | 101
MixRandom | 100 | True | Partition | QuickDualPivotSortInsert | 219 | 300 | 172
MixRandom | 100 | True | Partition | QuickSortMedian3BinaryInsert | 263 | 275 | 94
MixRandom | 100 | True | Partition | QuickSortMedian9BinaryInsert | 229 | 313 | 106
MixRandom | 100 | True | Partition | QuickDualPivotSortBinaryInsert | 219 | 300 | 172
MixRandom | 100 | True | Merge | MergeSort | 543 | 531 | 619
MixRandom | 100 | True | Merge | MergeSort2 | 771 | 537 | 672
MixRandom | 100 | True | Merge | ShiftSort | 893 | 673 | 16
MixRandom | 100 | True | Distributed | BucketSort | 200 | 100 | 0
MixRandom | 100 | True | Distributed | RadixLSD10Sort | 551 | 0 | 0
MixRandom | 100 | True | Distributed | RadixLSD4Sort | 3802 | 400 | 0
MixRandom | 100 | True | Distributed | CountingSort | 509 | 0 | 0
MixRandom | 100 | True | Hybrid | IntroSortMedian9 | 190 | 252 | 108

#### Size : 1000

InputType | ArraySize | IsSorted | SortType | Algorithm | IndexAccessCount | CompareCount | SwapCount
---- | ---- | ---- | ---- | ---- | ---- | ---- | ----
MixRandom | 1000 | True | Exchange | BubbleSort | 499500 | 499500 | 252066
MixRandom | 1000 | True | Exchange | OddEvenSort | 496503 | 496503 | 252066
MixRandom | 1000 | True | Exchange | CocktailShakerSort | 336567 | 336567 | 252066
MixRandom | 1000 | True | Exchange | CocktailShakerSort2 | 384540 | 384540 | 252066
MixRandom | 1000 | True | Exchange | CombSort | 20705 | 20705 | 4117
MixRandom | 1000 | True | Exchange | CycleSort | 1488934 | 1489430 | 1000
MixRandom | 1000 | True | Exchange | GnomeSort | 253066 | 252066 | 252066
MixRandom | 1000 | True | Exchange | GnomeSort1 | 505127 | 505127 | 252066
MixRandom | 1000 | True | Exchange | GnomeSort2 | 252071 | 253061 | 252066
MixRandom | 1000 | True | Exchange | GnomeSort3 | 505132 | 505132 | 252066
MixRandom | 1000 | True | Selection | SelectionSort | 499500 | 499500 | 1000
MixRandom | 1000 | True | Selection | HeapSort | 10533 | 17203 | 9543
MixRandom | 1000 | True | Insertion | InsertSort | 252066 | 252066 | 252066
MixRandom | 1000 | True | Insertion | BinaryInsertSort | 262645 | 8581 | 253065
MixRandom | 1000 | True | Insertion | ShellSort | 9127 | 4821 | 9127
MixRandom | 1000 | True | Insertion | BinaryTreeSort | 1000 | 11301 | 0
MixRandom | 1000 | True | Partition | QuickSortMedian3 | 7113 | 10102 | 2825
MixRandom | 1000 | True | Partition | QuickSortMedian9 | 5316 | 14288 | 2940
MixRandom | 1000 | True | Partition | QuickDualPivotSort | 6229 | 9306 | 5067
MixRandom | 1000 | True | Partition | QuickSortMedian3Insert | 5612 | 5817 | 1666
MixRandom | 1000 | True | Partition | QuickSortMedian9Insert | 4122 | 5005 | 1618
MixRandom | 1000 | True | Partition | QuickDualPivotSortInsert | 5281 | 7701 | 3714
MixRandom | 1000 | True | Partition | QuickSortMedian3BinaryInsert | 5755 | 5924 | 1678
MixRandom | 1000 | True | Partition | QuickSortMedian9BinaryInsert | 4204 | 5066 | 1625
MixRandom | 1000 | True | Partition | QuickDualPivotSortBinaryInsert | 5281 | 7701 | 3714
MixRandom | 1000 | True | Merge | MergeSort | 6679 | 8719 | 9355
MixRandom | 1000 | True | Merge | MergeSort2 | 10975 | 8719 | 9976
MixRandom | 1000 | True | Merge | ShiftSort | 15044 | 11135 | 188
MixRandom | 1000 | True | Distributed | BucketSort | 2000 | 1000 | 0
MixRandom | 1000 | True | Distributed | RadixLSD10Sort | 7073 | 0 | 0
MixRandom | 1000 | True | Distributed | RadixLSD4Sort | 10622 | 4000 | 0
MixRandom | 1000 | True | Distributed | CountingSort | 5006 | 0 | 0
MixRandom | 1000 | True | Hybrid | IntroSortMedian9 | 3709 | 4623 | 1668

#### Size : 10000

InputType | ArraySize | IsSorted | SortType | Algorithm | IndexAccessCount | CompareCount | SwapCount
---- | ---- | ---- | ---- | ---- | ---- | ---- | ----
MixRandom | 10000 | True | Exchange | BubbleSort | 49995000 | 49995000 | 25024702
MixRandom | 10000 | True | Exchange | OddEvenSort | 49615038 | 49615038 | 25024702
MixRandom | 10000 | True | Exchange | CocktailShakerSort | 33290810 | 33290810 | 25024702
MixRandom | 10000 | True | Exchange | CocktailShakerSort2 | 37746225 | 37746225 | 25024702
MixRandom | 10000 | True | Exchange | CombSort | 306727 | 306727 | 58808
MixRandom | 10000 | True | Exchange | CycleSort | 149841994 | 149846943 | 9997
MixRandom | 10000 | True | Exchange | GnomeSort | 25034702 | 25024702 | 25024702
MixRandom | 10000 | True | Exchange | GnomeSort1 | 50059395 | 50059395 | 25024702
MixRandom | 10000 | True | Exchange | GnomeSort2 | 25024699 | 25034693 | 25024702
MixRandom | 10000 | True | Exchange | GnomeSort3 | 50059404 | 50059404 | 25024702
MixRandom | 10000 | True | Selection | SelectionSort | 49995000 | 49995000 | 10000
MixRandom | 10000 | True | Selection | HeapSort | 139616 | 239517 | 129623
MixRandom | 10000 | True | Insertion | InsertSort | 25024702 | 25024702 | 25024702
MixRandom | 10000 | True | Insertion | BinaryInsertSort | 25163683 | 118983 | 25034701
MixRandom | 10000 | True | Insertion | ShellSort | 168262 | 75084 | 168262
MixRandom | 10000 | True | Insertion | BinaryTreeSort | 10000 | 148883 | 0
MixRandom | 10000 | True | Partition | QuickSortMedian3 | 103129 | 133116 | 35484
MixRandom | 10000 | True | Partition | QuickSortMedian9 | 71207 | 161131 | 37687
MixRandom | 10000 | True | Partition | QuickDualPivotSort | 89157 | 125888 | 69452
MixRandom | 10000 | True | Partition | QuickSortMedian3Insert | 87606 | 90043 | 23508
MixRandom | 10000 | True | Partition | QuickSortMedian9Insert | 58823 | 67693 | 24248
MixRandom | 10000 | True | Partition | QuickDualPivotSortInsert | 78890 | 109427 | 55848
MixRandom | 10000 | True | Partition | QuickSortMedian3BinaryInsert | 87621 | 90055 | 23509
MixRandom | 10000 | True | Partition | QuickSortMedian9BinaryInsert | 58973 | 67813 | 24258
MixRandom | 10000 | True | Partition | QuickDualPivotSortBinaryInsert | 78890 | 109427 | 55848
MixRandom | 10000 | True | Merge | MergeSort | 86603 | 120508 | 128104
MixRandom | 10000 | True | Merge | MergeSort2 | 143615 | 120489 | 133616
MixRandom | 10000 | True | Merge | ShiftSort | 210166 | 155621 | 1791
MixRandom | 10000 | True | Distributed | BucketSort | 20000 | 10000 | 0
MixRandom | 10000 | True | Distributed | RadixLSD10Sort | 90093 | 0 | 0
MixRandom | 10000 | True | Distributed | RadixLSD4Sort | 82516 | 40000 | 0
MixRandom | 10000 | True | Distributed | CountingSort | 50008 | 0 | 0
MixRandom | 10000 | True | Hybrid | IntroSortMedian9 | 62559 | 71469 | 24055

### NegativeRandom

#### Size : 100

InputType | ArraySize | IsSorted | SortType | Algorithm | IndexAccessCount | CompareCount | SwapCount
---- | ---- | ---- | ---- | ---- | ---- | ---- | ----
NegativeRandom | 100 | True | Exchange | BubbleSort | 4950 | 4950 | 2667
NegativeRandom | 100 | True | Exchange | OddEvenSort | 4752 | 4752 | 2667
NegativeRandom | 100 | True | Exchange | CocktailShakerSort | 3667 | 3667 | 2667
NegativeRandom | 100 | True | Exchange | CocktailShakerSort2 | 4170 | 4170 | 2667
NegativeRandom | 100 | True | Exchange | CombSort | 1294 | 1294 | 267
NegativeRandom | 100 | True | Exchange | CycleSort | 14502 | 14539 | 98
NegativeRandom | 100 | True | Exchange | GnomeSort | 2767 | 2667 | 2667
NegativeRandom | 100 | True | Exchange | GnomeSort1 | 5430 | 5430 | 2667
NegativeRandom | 100 | True | Exchange | GnomeSort2 | 2667 | 2763 | 2667
NegativeRandom | 100 | True | Exchange | GnomeSort3 | 5434 | 5434 | 2667
NegativeRandom | 100 | True | Selection | SelectionSort | 4950 | 4950 | 100
NegativeRandom | 100 | True | Selection | HeapSort | 702 | 1059 | 606
NegativeRandom | 100 | True | Insertion | InsertSort | 2667 | 2667 | 2667
NegativeRandom | 100 | True | Insertion | BinaryInsertSort | 3402 | 537 | 2766
NegativeRandom | 100 | True | Insertion | ShellSort | 453 | 282 | 453
NegativeRandom | 100 | True | Insertion | BinaryTreeSort | 100 | 649 | 0
NegativeRandom | 100 | True | Partition | QuickSortMedian3 | 427 | 719 | 202
NegativeRandom | 100 | True | Partition | QuickSortMedian9 | 321 | 1200 | 214
NegativeRandom | 100 | True | Partition | QuickDualPivotSort | 318 | 474 | 289
NegativeRandom | 100 | True | Partition | QuickSortMedian3Insert | 290 | 283 | 128
NegativeRandom | 100 | True | Partition | QuickSortMedian9Insert | 227 | 274 | 147
NegativeRandom | 100 | True | Partition | QuickDualPivotSortInsert | 238 | 337 | 166
NegativeRandom | 100 | True | Partition | QuickSortMedian3BinaryInsert | 399 | 353 | 141
NegativeRandom | 100 | True | Partition | QuickSortMedian9BinaryInsert | 336 | 344 | 160
NegativeRandom | 100 | True | Partition | QuickDualPivotSortBinaryInsert | 238 | 337 | 166
NegativeRandom | 100 | True | Merge | MergeSort | 550 | 530 | 625
NegativeRandom | 100 | True | Merge | MergeSort2 | 771 | 536 | 672
NegativeRandom | 100 | True | Merge | ShiftSort | 886 | 653 | 15
NegativeRandom | 100 | True | Distributed | BucketSort | 200 | 100 | 0
NegativeRandom | 100 | True | Distributed | RadixLSD10Sort | 798 | 0 | 0
NegativeRandom | 100 | True | Distributed | RadixLSD4Sort | 3803 | 400 | 0
NegativeRandom | 100 | True | Distributed | CountingSort | 604 | 0 | 0
NegativeRandom | 100 | True | Hybrid | IntroSortMedian9 | 234 | 277 | 125

#### Size : 1000

InputType | ArraySize | IsSorted | SortType | Algorithm | IndexAccessCount | CompareCount | SwapCount
---- | ---- | ---- | ---- | ---- | ---- | ---- | ----
NegativeRandom | 1000 | True | Exchange | BubbleSort | 499500 | 499500 | 250372
NegativeRandom | 1000 | True | Exchange | OddEvenSort | 486513 | 486513 | 250372
NegativeRandom | 1000 | True | Exchange | CocktailShakerSort | 334929 | 334929 | 250372
NegativeRandom | 1000 | True | Exchange | CocktailShakerSort2 | 380672 | 380672 | 250372
NegativeRandom | 1000 | True | Exchange | CombSort | 20705 | 20705 | 4073
NegativeRandom | 1000 | True | Exchange | CycleSort | 1485706 | 1486151 | 999
NegativeRandom | 1000 | True | Exchange | GnomeSort | 251372 | 250372 | 250372
NegativeRandom | 1000 | True | Exchange | GnomeSort1 | 501740 | 501740 | 250372
NegativeRandom | 1000 | True | Exchange | GnomeSort2 | 250374 | 251368 | 250372
NegativeRandom | 1000 | True | Exchange | GnomeSort3 | 501744 | 501744 | 250372
NegativeRandom | 1000 | True | Selection | SelectionSort | 499500 | 499500 | 1000
NegativeRandom | 1000 | True | Selection | HeapSort | 10586 | 17236 | 9593
NegativeRandom | 1000 | True | Insertion | InsertSort | 250372 | 250372 | 250372
NegativeRandom | 1000 | True | Insertion | BinaryInsertSort | 260966 | 8596 | 251371
NegativeRandom | 1000 | True | Insertion | ShellSort | 9415 | 4821 | 9415
NegativeRandom | 1000 | True | Insertion | BinaryTreeSort | 1000 | 11728 | 0
NegativeRandom | 1000 | True | Partition | QuickSortMedian3 | 6955 | 9941 | 2835
NegativeRandom | 1000 | True | Partition | QuickSortMedian9 | 5079 | 14083 | 2960
NegativeRandom | 1000 | True | Partition | QuickDualPivotSort | 6465 | 8632 | 4526
NegativeRandom | 1000 | True | Partition | QuickSortMedian3Insert | 5408 | 5634 | 1636
NegativeRandom | 1000 | True | Partition | QuickSortMedian9Insert | 3932 | 4822 | 1692
NegativeRandom | 1000 | True | Partition | QuickDualPivotSortInsert | 5499 | 7067 | 3189
NegativeRandom | 1000 | True | Partition | QuickSortMedian3BinaryInsert | 5525 | 5721 | 1646
NegativeRandom | 1000 | True | Partition | QuickSortMedian9BinaryInsert | 4085 | 4936 | 1705
NegativeRandom | 1000 | True | Partition | QuickDualPivotSortBinaryInsert | 5499 | 7067 | 3189
NegativeRandom | 1000 | True | Merge | MergeSort | 6697 | 8697 | 9351
NegativeRandom | 1000 | True | Merge | MergeSort2 | 10975 | 8702 | 9976
NegativeRandom | 1000 | True | Merge | ShiftSort | 14818 | 10866 | 165
NegativeRandom | 1000 | True | Distributed | BucketSort | 2000 | 1000 | 0
NegativeRandom | 1000 | True | Distributed | RadixLSD10Sort | 9128 | 0 | 0
NegativeRandom | 1000 | True | Distributed | RadixLSD4Sort | 10814 | 4000 | 0
NegativeRandom | 1000 | True | Distributed | CountingSort | 6004 | 0 | 0
NegativeRandom | 1000 | True | Hybrid | IntroSortMedian9 | 3924 | 4772 | 1624

#### Size : 10000

InputType | ArraySize | IsSorted | SortType | Algorithm | IndexAccessCount | CompareCount | SwapCount
---- | ---- | ---- | ---- | ---- | ---- | ---- | ----
NegativeRandom | 10000 | True | Exchange | BubbleSort | 49995000 | 49995000 | 24902687
NegativeRandom | 10000 | True | Exchange | OddEvenSort | 49855014 | 49855014 | 24902687
NegativeRandom | 10000 | True | Exchange | CocktailShakerSort | 33194918 | 33194918 | 24902687
NegativeRandom | 10000 | True | Exchange | CocktailShakerSort2 | 37397310 | 37397310 | 24902687
NegativeRandom | 10000 | True | Exchange | CombSort | 306727 | 306727 | 58759
NegativeRandom | 10000 | True | Exchange | CycleSort | 149880288 | 149885389 | 9999
NegativeRandom | 10000 | True | Exchange | GnomeSort | 24912687 | 24902687 | 24902687
NegativeRandom | 10000 | True | Exchange | GnomeSort1 | 49815364 | 49815364 | 24902687
NegativeRandom | 10000 | True | Exchange | GnomeSort2 | 24902688 | 24912677 | 24902687
NegativeRandom | 10000 | True | Exchange | GnomeSort3 | 49815374 | 49815374 | 24902687
NegativeRandom | 10000 | True | Selection | SelectionSort | 49995000 | 49995000 | 10000
NegativeRandom | 10000 | True | Selection | HeapSort | 139559 | 239358 | 129568
NegativeRandom | 10000 | True | Insertion | InsertSort | 24902687 | 24902687 | 24902687
NegativeRandom | 10000 | True | Insertion | BinaryInsertSort | 25041717 | 119032 | 24912686
NegativeRandom | 10000 | True | Insertion | ShellSort | 157531 | 75084 | 157531
NegativeRandom | 10000 | True | Insertion | BinaryTreeSort | 10000 | 157546 | 0
NegativeRandom | 10000 | True | Partition | QuickSortMedian3 | 108499 | 138485 | 35637
NegativeRandom | 10000 | True | Partition | QuickSortMedian9 | 70256 | 160034 | 37721
NegativeRandom | 10000 | True | Partition | QuickDualPivotSort | 90883 | 121942 | 66198
NegativeRandom | 10000 | True | Partition | QuickSortMedian3Insert | 93006 | 95362 | 23495
NegativeRandom | 10000 | True | Partition | QuickSortMedian9Insert | 58083 | 66977 | 24312
NegativeRandom | 10000 | True | Partition | QuickDualPivotSortInsert | 80713 | 105797 | 52771
NegativeRandom | 10000 | True | Partition | QuickSortMedian3BinaryInsert | 93111 | 95446 | 23502
NegativeRandom | 10000 | True | Partition | QuickSortMedian9BinaryInsert | 58294 | 67146 | 24326
NegativeRandom | 10000 | True | Partition | QuickDualPivotSortBinaryInsert | 80713 | 105797 | 52771
NegativeRandom | 10000 | True | Merge | MergeSort | 86652 | 120446 | 128091
NegativeRandom | 10000 | True | Merge | MergeSort2 | 143615 | 120456 | 133616
NegativeRandom | 10000 | True | Merge | ShiftSort | 207366 | 152913 | 1674
NegativeRandom | 10000 | True | Distributed | BucketSort | 20000 | 10000 | 0
NegativeRandom | 10000 | True | Distributed | RadixLSD10Sort | 110158 | 0 | 0
NegativeRandom | 10000 | True | Distributed | RadixLSD4Sort | 82774 | 40000 | 0
NegativeRandom | 10000 | True | Distributed | CountingSort | 60010 | 0 | 0
NegativeRandom | 10000 | True | Hybrid | IntroSortMedian9 | 59767 | 68571 | 24194

### Reversed

#### Size : 100

InputType | ArraySize | IsSorted | SortType | Algorithm | IndexAccessCount | CompareCount | SwapCount
---- | ---- | ---- | ---- | ---- | ---- | ---- | ----
Reversed | 100 | True | Exchange | BubbleSort | 4950 | 4950 | 4950
Reversed | 100 | True | Exchange | OddEvenSort | 5049 | 5049 | 4950
Reversed | 100 | True | Exchange | CocktailShakerSort | 4950 | 4950 | 4950
Reversed | 100 | True | Exchange | CocktailShakerSort2 | 4950 | 4950 | 4950
Reversed | 100 | True | Exchange | CombSort | 1195 | 1195 | 116
Reversed | 100 | True | Exchange | CycleSort | 8774 | 8725 | 100
Reversed | 100 | True | Exchange | GnomeSort | 5050 | 4950 | 4950
Reversed | 100 | True | Exchange | GnomeSort1 | 9900 | 9900 | 4950
Reversed | 100 | True | Exchange | GnomeSort2 | 4851 | 4950 | 4950
Reversed | 100 | True | Exchange | GnomeSort3 | 10000 | 10000 | 4950
Reversed | 100 | True | Selection | SelectionSort | 4950 | 4950 | 100
Reversed | 100 | True | Selection | HeapSort | 614 | 955 | 516
Reversed | 100 | True | Insertion | InsertSort | 4950 | 4950 | 4950
Reversed | 100 | True | Insertion | BinaryInsertSort | 5721 | 573 | 5049
Reversed | 100 | True | Insertion | ShellSort | 474 | 282 | 474
Reversed | 100 | True | Insertion | BinaryTreeSort | 100 | 4950 | 0
Reversed | 100 | True | Partition | QuickSortMedian3 | 1705 | 1995 | 148
Reversed | 100 | True | Partition | QuickSortMedian9 | 474 | 1266 | 148
Reversed | 100 | True | Partition | QuickDualPivotSort | 2450 | 2500 | 150
Reversed | 100 | True | Partition | QuickSortMedian3Insert | 1404 | 1508 | 95
Reversed | 100 | True | Partition | QuickSortMedian9Insert | 201 | 261 | 56
Reversed | 100 | True | Partition | QuickDualPivotSortInsert | 2394 | 2436 | 126
Reversed | 100 | True | Partition | QuickSortMedian3BinaryInsert | 1524 | 1583 | 110
Reversed | 100 | True | Partition | QuickSortMedian9BinaryInsert | 289 | 316 | 67
Reversed | 100 | True | Partition | QuickDualPivotSortBinaryInsert | 2394 | 2436 | 126
Reversed | 100 | True | Merge | MergeSort | 811 | 316 | 672
Reversed | 100 | True | Merge | MergeSort2 | 771 | 356 | 672
Reversed | 100 | True | Merge | ShiftSort | 1114 | 820 | 49
Reversed | 100 | True | Distributed | BucketSort | 200 | 100 | 0
Reversed | 100 | True | Distributed | RadixLSD10Sort | 420 | 200 | 0
Reversed | 100 | True | Distributed | RadixLSD4Sort | 2745 | 0 | 0
Reversed | 100 | True | Distributed | CountingSort | 398 | 0 | 0
Reversed | 100 | True | Hybrid | IntroSortMedian9 | 211 | 266 | 61

#### Size : 1000

InputType | ArraySize | IsSorted | SortType | Algorithm | IndexAccessCount | CompareCount | SwapCount
---- | ---- | ---- | ---- | ---- | ---- | ---- | ----
Reversed | 1000 | True | Exchange | BubbleSort | 499500 | 499500 | 499500
Reversed | 1000 | True | Exchange | OddEvenSort | 500499 | 500499 | 499500
Reversed | 1000 | True | Exchange | CocktailShakerSort | 499500 | 499500 | 499500
Reversed | 1000 | True | Exchange | CocktailShakerSort2 | 499500 | 499500 | 499500
Reversed | 1000 | True | Exchange | CombSort | 20705 | 20705 | 1536
Reversed | 1000 | True | Exchange | CycleSort | 875249 | 874750 | 1000
Reversed | 1000 | True | Exchange | GnomeSort | 500500 | 499500 | 499500
Reversed | 1000 | True | Exchange | GnomeSort1 | 999000 | 999000 | 499500
Reversed | 1000 | True | Exchange | GnomeSort2 | 498501 | 499500 | 499500
Reversed | 1000 | True | Exchange | GnomeSort3 | 1000000 | 1000000 | 499500
Reversed | 1000 | True | Selection | SelectionSort | 499500 | 499500 | 1000
Reversed | 1000 | True | Selection | HeapSort | 9314 | 15981 | 8316
Reversed | 1000 | True | Insertion | InsertSort | 499500 | 499500 | 499500
Reversed | 1000 | True | Insertion | BinaryInsertSort | 510475 | 8977 | 500499
Reversed | 1000 | True | Insertion | ShellSort | 5990 | 4821 | 5990
Reversed | 1000 | True | Insertion | BinaryTreeSort | 1000 | 499500 | 0
Reversed | 1000 | True | Partition | QuickSortMedian3 | 167127 | 170114 | 1498
Reversed | 1000 | True | Partition | QuickSortMedian9 | 7978 | 15970 | 1498
Reversed | 1000 | True | Partition | QuickDualPivotSort | 249500 | 250000 | 1500
Reversed | 1000 | True | Partition | QuickSortMedian3Insert | 167240 | 169037 | 1393
Reversed | 1000 | True | Partition | QuickSortMedian9Insert | 5001 | 5456 | 562
Reversed | 1000 | True | Partition | QuickDualPivotSortInsert | 249444 | 249936 | 1476
Reversed | 1000 | True | Partition | QuickSortMedian3BinaryInsert | 167405 | 169157 | 1408
Reversed | 1000 | True | Partition | QuickSortMedian9BinaryInsert | 5155 | 5568 | 576
Reversed | 1000 | True | Partition | QuickDualPivotSortBinaryInsert | 249444 | 249936 | 1476
Reversed | 1000 | True | Merge | MergeSort | 11087 | 4932 | 9976
Reversed | 1000 | True | Merge | MergeSort2 | 10975 | 5044 | 9976
Reversed | 1000 | True | Merge | ShiftSort | 17484 | 13022 | 499
Reversed | 1000 | True | Distributed | BucketSort | 2000 | 1000 | 0
Reversed | 1000 | True | Distributed | RadixLSD10Sort | 6030 | 3000 | 0
Reversed | 1000 | True | Distributed | RadixLSD4Sort | 9786 | 0 | 0
Reversed | 1000 | True | Distributed | CountingSort | 3998 | 0 | 0
Reversed | 1000 | True | Hybrid | IntroSortMedian9 | 5011 | 5514 | 514

#### Size : 10000

InputType | ArraySize | IsSorted | SortType | Algorithm | IndexAccessCount | CompareCount | SwapCount
---- | ---- | ---- | ---- | ---- | ---- | ---- | ----
Reversed | 10000 | True | Exchange | BubbleSort | 49995000 | 49995000 | 49995000
Reversed | 10000 | True | Exchange | OddEvenSort | 50004999 | 50004999 | 49995000
Reversed | 10000 | True | Exchange | CocktailShakerSort | 49995000 | 49995000 | 49995000
Reversed | 10000 | True | Exchange | CocktailShakerSort2 | 49995000 | 49995000 | 49995000
Reversed | 10000 | True | Exchange | CombSort | 296728 | 296728 | 20094
Reversed | 10000 | True | Exchange | CycleSort | 87502499 | 87497500 | 10000
Reversed | 10000 | True | Exchange | GnomeSort | 50005000 | 49995000 | 49995000
Reversed | 10000 | True | Exchange | GnomeSort1 | 99990000 | 99990000 | 49995000
Reversed | 10000 | True | Exchange | GnomeSort2 | 49985001 | 49995000 | 49995000
Reversed | 10000 | True | Exchange | GnomeSort3 | 100000000 | 100000000 | 49995000
Reversed | 10000 | True | Selection | SelectionSort | 49995000 | 49995000 | 10000
Reversed | 10000 | True | Selection | HeapSort | 126694 | 226719 | 116696
Reversed | 10000 | True | Insertion | InsertSort | 49995000 | 49995000 | 49995000
Reversed | 10000 | True | Insertion | BinaryInsertSort | 50138615 | 123617 | 50004999
Reversed | 10000 | True | Insertion | ShellSort | 55972 | 75084 | 55972
Reversed | 10000 | True | Insertion | BinaryTreeSort | 10000 | 49995000 | 0
Reversed | 10000 | True | Partition | QuickSortMedian3 | 16671462 | 16701445 | 14998
Reversed | 10000 | True | Partition | QuickSortMedian9 | 113618 | 193610 | 14998
Reversed | 10000 | True | Partition | QuickDualPivotSort | 24995000 | 25000000 | 15000
Reversed | 10000 | True | Partition | QuickSortMedian3Insert | 16680156 | 16699853 | 14845
Reversed | 10000 | True | Partition | QuickSortMedian9Insert | 90001 | 97170 | 6022
Reversed | 10000 | True | Partition | QuickDualPivotSortInsert | 24994944 | 24999936 | 14976
Reversed | 10000 | True | Partition | QuickSortMedian3BinaryInsert | 16680381 | 16700033 | 14860
Reversed | 10000 | True | Partition | QuickSortMedian9BinaryInsert | 90121 | 97266 | 6030
Reversed | 10000 | True | Partition | QuickDualPivotSortBinaryInsert | 24994944 | 24999936 | 14976
Reversed | 10000 | True | Merge | MergeSort | 148015 | 64608 | 133616
Reversed | 10000 | True | Merge | MergeSort2 | 143615 | 69008 | 133616
Reversed | 10000 | True | Merge | ShiftSort | 242280 | 180856 | 4999
Reversed | 10000 | True | Distributed | BucketSort | 20000 | 10000 | 0
Reversed | 10000 | True | Distributed | RadixLSD10Sort | 80040 | 40000 | 0
Reversed | 10000 | True | Distributed | RadixLSD4Sort | 81750 | 0 | 0
Reversed | 10000 | True | Distributed | CountingSort | 39998 | 0 | 0
Reversed | 10000 | True | Hybrid | IntroSortMedian9 | 90001 | 98184 | 5008

### Mountain

#### Size : 100

InputType | ArraySize | IsSorted | SortType | Algorithm | IndexAccessCount | CompareCount | SwapCount
---- | ---- | ---- | ---- | ---- | ---- | ---- | ----
Mountain | 100 | True | Exchange | BubbleSort | 4950 | 4950 | 2450
Mountain | 100 | True | Exchange | OddEvenSort | 4950 | 4950 | 2450
Mountain | 100 | True | Exchange | CocktailShakerSort | 3333 | 3333 | 2450
Mountain | 100 | True | Exchange | CocktailShakerSort2 | 4454 | 4454 | 2450
Mountain | 100 | True | Exchange | CombSort | 1195 | 1195 | 149
Mountain | 100 | True | Exchange | CycleSort | 14555 | 14602 | 98
Mountain | 100 | True | Exchange | GnomeSort | 2550 | 2450 | 2450
Mountain | 100 | True | Exchange | GnomeSort1 | 4999 | 4999 | 2450
Mountain | 100 | True | Exchange | GnomeSort2 | 2500 | 2549 | 2450
Mountain | 100 | True | Exchange | GnomeSort3 | 5000 | 5000 | 2450
Mountain | 100 | True | Selection | SelectionSort | 4950 | 4950 | 100
Mountain | 100 | True | Selection | HeapSort | 767 | 1097 | 718
Mountain | 100 | True | Insertion | InsertSort | 2450 | 2450 | 2450
Mountain | 100 | True | Insertion | BinaryInsertSort | 3154 | 506 | 2549
Mountain | 100 | True | Insertion | ShellSort | 280 | 282 | 280
Mountain | 100 | True | Insertion | BinaryTreeSort | 100 | 2549 | 0
Mountain | 100 | True | Partition | QuickSortMedian3 | 833 | 1125 | 209
Mountain | 100 | True | Partition | QuickSortMedian9 | 324 | 1192 | 215
Mountain | 100 | True | Partition | QuickDualPivotSort | 592 | 1008 | 677
Mountain | 100 | True | Partition | QuickSortMedian3Insert | 682 | 692 | 133
Mountain | 100 | True | Partition | QuickSortMedian9Insert | 238 | 307 | 120
Mountain | 100 | True | Partition | QuickDualPivotSortInsert | 516 | 872 | 525
Mountain | 100 | True | Partition | QuickSortMedian3BinaryInsert | 775 | 752 | 144
Mountain | 100 | True | Partition | QuickSortMedian9BinaryInsert | 322 | 361 | 130
Mountain | 100 | True | Partition | QuickDualPivotSortBinaryInsert | 516 | 872 | 525
Mountain | 100 | True | Merge | MergeSort | 609 | 385 | 539
Mountain | 100 | True | Merge | MergeSort2 | 771 | 385 | 672
Mountain | 100 | True | Merge | ShiftSort | 650 | 518 | 24
Mountain | 100 | True | Distributed | BucketSort | 200 | 100 | 0
Mountain | 100 | True | Distributed | RadixLSD10Sort | 425 | 200 | 0
Mountain | 100 | True | Distributed | RadixLSD4Sort | 2795 | 0 | 0
Mountain | 100 | True | Distributed | CountingSort | 349 | 0 | 0
Mountain | 100 | True | Hybrid | IntroSortMedian9 | 203 | 273 | 104

#### Size : 1000

InputType | ArraySize | IsSorted | SortType | Algorithm | IndexAccessCount | CompareCount | SwapCount
---- | ---- | ---- | ---- | ---- | ---- | ---- | ----
Mountain | 1000 | True | Exchange | BubbleSort | 499500 | 499500 | 249500
Mountain | 1000 | True | Exchange | OddEvenSort | 499500 | 499500 | 249500
Mountain | 1000 | True | Exchange | CocktailShakerSort | 333333 | 333333 | 249500
Mountain | 1000 | True | Exchange | CocktailShakerSort2 | 444554 | 444554 | 249500
Mountain | 1000 | True | Exchange | CombSort | 20705 | 20705 | 2339
Mountain | 1000 | True | Exchange | CycleSort | 1469567 | 1470038 | 998
Mountain | 1000 | True | Exchange | GnomeSort | 250500 | 249500 | 249500
Mountain | 1000 | True | Exchange | GnomeSort1 | 499999 | 499999 | 249500
Mountain | 1000 | True | Exchange | GnomeSort2 | 250000 | 250499 | 249500
Mountain | 1000 | True | Exchange | GnomeSort3 | 500000 | 500000 | 249500
Mountain | 1000 | True | Selection | SelectionSort | 499500 | 499500 | 1000
Mountain | 1000 | True | Selection | HeapSort | 12495 | 19020 | 11996
Mountain | 1000 | True | Insertion | InsertSort | 249500 | 249500 | 249500
Mountain | 1000 | True | Insertion | BinaryInsertSort | 259784 | 8286 | 250499
Mountain | 1000 | True | Insertion | ShellSort | 3999 | 4821 | 3999
Mountain | 1000 | True | Insertion | BinaryTreeSort | 1000 | 250499 | 0
Mountain | 1000 | True | Partition | QuickSortMedian3 | 19300 | 22289 | 2879
Mountain | 1000 | True | Partition | QuickSortMedian9 | 5233 | 14039 | 2939
Mountain | 1000 | True | Partition | QuickDualPivotSort | 23130 | 32482 | 24040
Mountain | 1000 | True | Partition | QuickSortMedian3Insert | 17529 | 17775 | 1858
Mountain | 1000 | True | Partition | QuickSortMedian9Insert | 3971 | 4813 | 1701
Mountain | 1000 | True | Partition | QuickDualPivotSortInsert | 22690 | 31642 | 22840
Mountain | 1000 | True | Partition | QuickSortMedian3BinaryInsert | 17706 | 17907 | 1873
Mountain | 1000 | True | Partition | QuickSortMedian9BinaryInsert | 4086 | 4898 | 1711
Mountain | 1000 | True | Partition | QuickDualPivotSortBinaryInsert | 22690 | 31642 | 22840
Mountain | 1000 | True | Merge | MergeSort | 8316 | 5487 | 7760
Mountain | 1000 | True | Merge | MergeSort2 | 10975 | 5487 | 9976
Mountain | 1000 | True | Merge | ShiftSort | 9735 | 7727 | 249
Mountain | 1000 | True | Distributed | BucketSort | 2000 | 1000 | 0
Mountain | 1000 | True | Distributed | RadixLSD10Sort | 6035 | 3000 | 0
Mountain | 1000 | True | Distributed | RadixLSD4Sort | 9788 | 0 | 0
Mountain | 1000 | True | Distributed | CountingSort | 3499 | 0 | 0
Mountain | 1000 | True | Hybrid | IntroSortMedian9 | 3790 | 4610 | 1689

#### Size : 10000

InputType | ArraySize | IsSorted | SortType | Algorithm | IndexAccessCount | CompareCount | SwapCount
---- | ---- | ---- | ---- | ---- | ---- | ---- | ----
Mountain | 10000 | True | Exchange | BubbleSort | 49995000 | 49995000 | 24995000
Mountain | 10000 | True | Exchange | OddEvenSort | 49995000 | 49995000 | 24995000
Mountain | 10000 | True | Exchange | CocktailShakerSort | 33333333 | 33333333 | 24995000
Mountain | 10000 | True | Exchange | CocktailShakerSort2 | 44445554 | 44445554 | 24995000
Mountain | 10000 | True | Exchange | CombSort | 296728 | 296728 | 32199
Mountain | 10000 | True | Exchange | CycleSort | 149649511 | 149654482 | 9998
Mountain | 10000 | True | Exchange | GnomeSort | 25005000 | 24995000 | 24995000
Mountain | 10000 | True | Exchange | GnomeSort1 | 49999999 | 49999999 | 24995000
Mountain | 10000 | True | Exchange | GnomeSort2 | 25000000 | 25004999 | 24995000
Mountain | 10000 | True | Exchange | GnomeSort3 | 50000000 | 50000000 | 24995000
Mountain | 10000 | True | Selection | SelectionSort | 49995000 | 49995000 | 10000
Mountain | 10000 | True | Selection | HeapSort | 174291 | 272242 | 169292
Mountain | 10000 | True | Insertion | InsertSort | 24995000 | 24995000 | 24995000
Mountain | 10000 | True | Insertion | BinaryInsertSort | 25131333 | 116335 | 25004999
Mountain | 10000 | True | Insertion | ShellSort | 42065 | 75084 | 42065
Mountain | 10000 | True | Insertion | BinaryTreeSort | 10000 | 25004999 | 0
Mountain | 10000 | True | Partition | QuickSortMedian3 | 367464 | 397449 | 36263
Mountain | 10000 | True | Partition | QuickSortMedian9 | 71863 | 159750 | 37635
Mountain | 10000 | True | Partition | QuickDualPivotSort | 1046345 | 1181046 | 1055786
Mountain | 10000 | True | Partition | QuickSortMedian3Insert | 349626 | 352995 | 25417
Mountain | 10000 | True | Partition | QuickSortMedian9Insert | 58290 | 66258 | 24788
Mountain | 10000 | True | Partition | QuickDualPivotSortInsert | 1043966 | 1175344 | 1046000
Mountain | 10000 | True | Partition | QuickSortMedian3BinaryInsert | 349855 | 353179 | 25432
Mountain | 10000 | True | Partition | QuickSortMedian9BinaryInsert | 58441 | 66379 | 24798
Mountain | 10000 | True | Partition | QuickDualPivotSortBinaryInsert | 1043966 | 1175344 | 1046000
Mountain | 10000 | True | Merge | MergeSort | 111012 | 71807 | 103812
Mountain | 10000 | True | Merge | MergeSort2 | 143615 | 71807 | 133616
Mountain | 10000 | True | Merge | ShiftSort | 131133 | 102885 | 2499
Mountain | 10000 | True | Distributed | BucketSort | 20000 | 10000 | 0
Mountain | 10000 | True | Distributed | RadixLSD10Sort | 80045 | 40000 | 0
Mountain | 10000 | True | Distributed | RadixLSD4Sort | 81770 | 0 | 0
Mountain | 10000 | True | Distributed | CountingSort | 34999 | 0 | 0
Mountain | 10000 | True | Hybrid | IntroSortMedian9 | 56664 | 65097 | 24920

### NearlySorted

#### Size : 100

InputType | ArraySize | IsSorted | SortType | Algorithm | IndexAccessCount | CompareCount | SwapCount
---- | ---- | ---- | ---- | ---- | ---- | ---- | ----
NearlySorted | 100 | True | Exchange | BubbleSort | 4950 | 4950 | 208
NearlySorted | 100 | True | Exchange | OddEvenSort | 2079 | 2079 | 208
NearlySorted | 100 | True | Exchange | CocktailShakerSort | 538 | 538 | 208
NearlySorted | 100 | True | Exchange | CocktailShakerSort2 | 1629 | 1629 | 208
NearlySorted | 100 | True | Exchange | CombSort | 1195 | 1195 | 62
NearlySorted | 100 | True | Exchange | CycleSort | 6578 | 6525 | 43
NearlySorted | 100 | True | Exchange | GnomeSort | 308 | 208 | 208
NearlySorted | 100 | True | Exchange | GnomeSort1 | 515 | 515 | 208
NearlySorted | 100 | True | Exchange | GnomeSort2 | 298 | 307 | 208
NearlySorted | 100 | True | Exchange | GnomeSort3 | 516 | 516 | 208
NearlySorted | 100 | True | Selection | SelectionSort | 4950 | 4950 | 100
NearlySorted | 100 | True | Selection | HeapSort | 974 | 1339 | 966
NearlySorted | 100 | True | Insertion | InsertSort | 208 | 208 | 208
NearlySorted | 100 | True | Insertion | BinaryInsertSort | 892 | 486 | 307
NearlySorted | 100 | True | Insertion | ShellSort | 80 | 282 | 80
NearlySorted | 100 | True | Insertion | BinaryTreeSort | 100 | 4727 | 0
NearlySorted | 100 | True | Partition | QuickSortMedian3 | 1251 | 1541 | 124
NearlySorted | 100 | True | Partition | QuickSortMedian9 | 499 | 1320 | 128
NearlySorted | 100 | True | Partition | QuickDualPivotSort | 1014 | 1130 | 203
NearlySorted | 100 | True | Partition | QuickSortMedian3Insert | 1034 | 1058 | 92
NearlySorted | 100 | True | Partition | QuickSortMedian9Insert | 325 | 352 | 68
NearlySorted | 100 | True | Partition | QuickDualPivotSortInsert | 893 | 955 | 84
NearlySorted | 100 | True | Partition | QuickSortMedian3BinaryInsert | 1136 | 1124 | 104
NearlySorted | 100 | True | Partition | QuickSortMedian9BinaryInsert | 410 | 407 | 78
NearlySorted | 100 | True | Partition | QuickDualPivotSortBinaryInsert | 893 | 955 | 84
NearlySorted | 100 | True | Merge | MergeSort | 464 | 378 | 387
NearlySorted | 100 | True | Merge | MergeSort2 | 771 | 340 | 672
NearlySorted | 100 | True | Merge | ShiftSort | 197 | 180 | 1
NearlySorted | 100 | True | Distributed | BucketSort | 200 | 100 | 0
NearlySorted | 100 | True | Distributed | RadixLSD10Sort | 420 | 200 | 0
NearlySorted | 100 | True | Distributed | RadixLSD4Sort | 2754 | 0 | 0
NearlySorted | 100 | True | Distributed | CountingSort | 393 | 0 | 0
NearlySorted | 100 | True | Hybrid | IntroSortMedian9 | 282 | 355 | 20

#### Size : 1000

InputType | ArraySize | IsSorted | SortType | Algorithm | IndexAccessCount | CompareCount | SwapCount
---- | ---- | ---- | ---- | ---- | ---- | ---- | ----
NearlySorted | 1000 | True | Exchange | BubbleSort | 499500 | 499500 | 3392
NearlySorted | 1000 | True | Exchange | OddEvenSort | 481518 | 481518 | 3392
NearlySorted | 1000 | True | Exchange | CocktailShakerSort | 8615 | 8615 | 3392
NearlySorted | 1000 | True | Exchange | CocktailShakerSort2 | 19790 | 19790 | 3392
NearlySorted | 1000 | True | Exchange | CombSort | 20705 | 20705 | 1470
NearlySorted | 1000 | True | Exchange | CycleSort | 1218550 | 1218517 | 966
NearlySorted | 1000 | True | Exchange | GnomeSort | 4392 | 3392 | 3392
NearlySorted | 1000 | True | Exchange | GnomeSort1 | 7783 | 7783 | 3392
NearlySorted | 1000 | True | Exchange | GnomeSort2 | 4381 | 4391 | 3392
NearlySorted | 1000 | True | Exchange | GnomeSort3 | 7784 | 7784 | 3392
NearlySorted | 1000 | True | Selection | SelectionSort | 499500 | 499500 | 1000
NearlySorted | 1000 | True | Selection | HeapSort | 16815 | 23771 | 16806
NearlySorted | 1000 | True | Insertion | InsertSort | 3392 | 3392 | 3392
NearlySorted | 1000 | True | Insertion | BinaryInsertSort | 13387 | 7997 | 4391
NearlySorted | 1000 | True | Insertion | ShellSort | 1772 | 4821 | 1772
NearlySorted | 1000 | True | Insertion | BinaryTreeSort | 1000 | 496104 | 0
NearlySorted | 1000 | True | Partition | QuickSortMedian3 | 143630 | 146621 | 1385
NearlySorted | 1000 | True | Partition | QuickSortMedian9 | 7974 | 16943 | 1519
NearlySorted | 1000 | True | Partition | QuickDualPivotSort | 22794 | 41039 | 23328
NearlySorted | 1000 | True | Partition | QuickSortMedian3Insert | 143476 | 144525 | 1038
NearlySorted | 1000 | True | Partition | QuickSortMedian9Insert | 5937 | 6572 | 355
NearlySorted | 1000 | True | Partition | QuickDualPivotSortInsert | 20802 | 38669 | 20556
NearlySorted | 1000 | True | Partition | QuickSortMedian3BinaryInsert | 143651 | 144655 | 1053
NearlySorted | 1000 | True | Partition | QuickSortMedian9BinaryInsert | 6045 | 6653 | 364
NearlySorted | 1000 | True | Partition | QuickDualPivotSortBinaryInsert | 20802 | 38669 | 20556
NearlySorted | 1000 | True | Merge | MergeSort | 6058 | 5091 | 5106
NearlySorted | 1000 | True | Merge | MergeSort2 | 10975 | 4982 | 9976
NearlySorted | 1000 | True | Merge | ShiftSort | 2307 | 2290 | 1
NearlySorted | 1000 | True | Distributed | BucketSort | 2000 | 1000 | 0
NearlySorted | 1000 | True | Distributed | RadixLSD10Sort | 6030 | 3000 | 0
NearlySorted | 1000 | True | Distributed | RadixLSD4Sort | 9786 | 0 | 0
NearlySorted | 1000 | True | Distributed | CountingSort | 3989 | 0 | 0
NearlySorted | 1000 | True | Hybrid | IntroSortMedian9 | 6086 | 6770 | 256

#### Size : 10000

InputType | ArraySize | IsSorted | SortType | Algorithm | IndexAccessCount | CompareCount | SwapCount
---- | ---- | ---- | ---- | ---- | ---- | ---- | ----
NearlySorted | 10000 | True | Exchange | BubbleSort | 49995000 | 49995000 | 62107
NearlySorted | 10000 | True | Exchange | OddEvenSort | 49745025 | 49745025 | 62107
NearlySorted | 10000 | True | Exchange | CocktailShakerSort | 141506 | 141506 | 62107
NearlySorted | 10000 | True | Exchange | CocktailShakerSort2 | 219747 | 219747 | 62107
NearlySorted | 10000 | True | Exchange | CombSort | 296728 | 296728 | 19301
NearlySorted | 10000 | True | Exchange | CycleSort | 142370852 | 142370805 | 9948
NearlySorted | 10000 | True | Exchange | GnomeSort | 72107 | 62107 | 62107
NearlySorted | 10000 | True | Exchange | GnomeSort1 | 134213 | 134213 | 62107
NearlySorted | 10000 | True | Exchange | GnomeSort2 | 72096 | 72106 | 62107
NearlySorted | 10000 | True | Exchange | GnomeSort3 | 134214 | 134214 | 62107
NearlySorted | 10000 | True | Selection | SelectionSort | 49995000 | 49995000 | 10000
NearlySorted | 10000 | True | Selection | HeapSort | 236883 | 340535 | 236874
NearlySorted | 10000 | True | Insertion | InsertSort | 62107 | 62107 | 62107
NearlySorted | 10000 | True | Insertion | BinaryInsertSort | 195737 | 113632 | 72106
NearlySorted | 10000 | True | Insertion | ShellSort | 23451 | 75084 | 23451
NearlySorted | 10000 | True | Insertion | BinaryTreeSort | 10000 | 49932883 | 0
NearlySorted | 10000 | True | Partition | QuickSortMedian3 | 6231002 | 6260992 | 16280
NearlySorted | 10000 | True | Partition | QuickSortMedian9 | 103508 | 190255 | 19143
NearlySorted | 10000 | True | Partition | QuickDualPivotSort | 662183 | 1184046 | 666955
NearlySorted | 10000 | True | Partition | QuickSortMedian3Insert | 6218436 | 6225727 | 9973
NearlySorted | 10000 | True | Partition | QuickSortMedian9Insert | 90275 | 97930 | 5691
NearlySorted | 10000 | True | Partition | QuickDualPivotSortInsert | 651490 | 1171656 | 651777
NearlySorted | 10000 | True | Partition | QuickSortMedian3BinaryInsert | 6218573 | 6225837 | 9982
NearlySorted | 10000 | True | Partition | QuickSortMedian9BinaryInsert | 90489 | 98102 | 5705
NearlySorted | 10000 | True | Partition | QuickDualPivotSortBinaryInsert | 651490 | 1171656 | 651777
NearlySorted | 10000 | True | Merge | MergeSort | 79022 | 69100 | 69115
NearlySorted | 10000 | True | Merge | MergeSort2 | 143615 | 64705 | 133616
NearlySorted | 10000 | True | Merge | ShiftSort | 27530 | 27513 | 0
NearlySorted | 10000 | True | Distributed | BucketSort | 20000 | 10000 | 0
NearlySorted | 10000 | True | Distributed | RadixLSD10Sort | 80040 | 40000 | 0
NearlySorted | 10000 | True | Distributed | RadixLSD4Sort | 81750 | 0 | 0
NearlySorted | 10000 | True | Distributed | CountingSort | 39989 | 0 | 0
NearlySorted | 10000 | True | Hybrid | IntroSortMedian9 | 90753 | 98050 | 5061

### Sorted

#### Size : 100

InputType | ArraySize | IsSorted | SortType | Algorithm | IndexAccessCount | CompareCount | SwapCount
---- | ---- | ---- | ---- | ---- | ---- | ---- | ----
Sorted | 100 | True | Exchange | BubbleSort | 4950 | 4950 | 0
Sorted | 100 | True | Exchange | OddEvenSort | 99 | 99 | 0
Sorted | 100 | True | Exchange | CocktailShakerSort | 99 | 99 | 0
Sorted | 100 | True | Exchange | CocktailShakerSort2 | 197 | 197 | 0
Sorted | 100 | True | Exchange | CombSort | 1096 | 1096 | 0
Sorted | 100 | True | Exchange | CycleSort | 5049 | 4950 | 0
Sorted | 100 | True | Exchange | GnomeSort | 100 | 0 | 0
Sorted | 100 | True | Exchange | GnomeSort1 | 99 | 99 | 0
Sorted | 100 | True | Exchange | GnomeSort2 | 99 | 99 | 0
Sorted | 100 | True | Exchange | GnomeSort3 | 100 | 100 | 0
Sorted | 100 | True | Selection | SelectionSort | 4950 | 4950 | 100
Sorted | 100 | True | Selection | HeapSort | 959 | 1340 | 960
Sorted | 100 | True | Insertion | InsertSort | 0 | 0 | 0
Sorted | 100 | True | Insertion | BinaryInsertSort | 678 | 480 | 99
Sorted | 100 | True | Insertion | ShellSort | 0 | 282 | 0
Sorted | 100 | True | Insertion | BinaryTreeSort | 100 | 4950 | 0
Sorted | 100 | True | Partition | QuickSortMedian3 | 1804 | 2094 | 99
Sorted | 100 | True | Partition | QuickSortMedian9 | 573 | 1365 | 99
Sorted | 100 | True | Partition | QuickDualPivotSort | 2450 | 2500 | 100
Sorted | 100 | True | Partition | QuickSortMedian3Insert | 1503 | 1607 | 46
Sorted | 100 | True | Partition | QuickSortMedian9Insert | 300 | 360 | 7
Sorted | 100 | True | Partition | QuickDualPivotSortInsert | 2394 | 2436 | 84
Sorted | 100 | True | Partition | QuickSortMedian3BinaryInsert | 1623 | 1682 | 61
Sorted | 100 | True | Partition | QuickSortMedian9BinaryInsert | 388 | 415 | 18
Sorted | 100 | True | Partition | QuickDualPivotSortBinaryInsert | 2394 | 2436 | 84
Sorted | 100 | True | Merge | MergeSort | 455 | 356 | 356
Sorted | 100 | True | Merge | MergeSort2 | 771 | 316 | 672
Sorted | 100 | True | Merge | ShiftSort | 99 | 99 | 0
Sorted | 100 | True | Distributed | BucketSort | 200 | 100 | 0
Sorted | 100 | True | Distributed | RadixLSD10Sort | 420 | 200 | 0
Sorted | 100 | True | Distributed | RadixLSD4Sort | 2745 | 0 | 0
Sorted | 100 | True | Distributed | CountingSort | 399 | 0 | 0
Sorted | 100 | True | Hybrid | IntroSortMedian9 | 297 | 358 | 0

#### Size : 1000

InputType | ArraySize | IsSorted | SortType | Algorithm | IndexAccessCount | CompareCount | SwapCount
---- | ---- | ---- | ---- | ---- | ---- | ---- | ----
Sorted | 1000 | True | Exchange | BubbleSort | 499500 | 499500 | 0
Sorted | 1000 | True | Exchange | OddEvenSort | 999 | 999 | 0
Sorted | 1000 | True | Exchange | CocktailShakerSort | 999 | 999 | 0
Sorted | 1000 | True | Exchange | CocktailShakerSort2 | 1997 | 1997 | 0
Sorted | 1000 | True | Exchange | CombSort | 19706 | 19706 | 0
Sorted | 1000 | True | Exchange | CycleSort | 500499 | 499500 | 0
Sorted | 1000 | True | Exchange | GnomeSort | 1000 | 0 | 0
Sorted | 1000 | True | Exchange | GnomeSort1 | 999 | 999 | 0
Sorted | 1000 | True | Exchange | GnomeSort2 | 999 | 999 | 0
Sorted | 1000 | True | Exchange | GnomeSort3 | 1000 | 1000 | 0
Sorted | 1000 | True | Selection | SelectionSort | 499500 | 499500 | 1000
Sorted | 1000 | True | Selection | HeapSort | 15973 | 22961 | 15974
Sorted | 1000 | True | Insertion | InsertSort | 0 | 0 | 0
Sorted | 1000 | True | Insertion | BinaryInsertSort | 9985 | 7987 | 999
Sorted | 1000 | True | Insertion | ShellSort | 0 | 4821 | 0
Sorted | 1000 | True | Insertion | BinaryTreeSort | 1000 | 499500 | 0
Sorted | 1000 | True | Partition | QuickSortMedian3 | 168126 | 171113 | 999
Sorted | 1000 | True | Partition | QuickSortMedian9 | 8977 | 16969 | 999
Sorted | 1000 | True | Partition | QuickDualPivotSort | 249500 | 250000 | 1000
Sorted | 1000 | True | Partition | QuickSortMedian3Insert | 168239 | 170036 | 894
Sorted | 1000 | True | Partition | QuickSortMedian9Insert | 6000 | 6455 | 63
Sorted | 1000 | True | Partition | QuickDualPivotSortInsert | 249444 | 249936 | 984
Sorted | 1000 | True | Partition | QuickSortMedian3BinaryInsert | 168404 | 170156 | 909
Sorted | 1000 | True | Partition | QuickSortMedian9BinaryInsert | 6154 | 6567 | 77
Sorted | 1000 | True | Partition | QuickDualPivotSortBinaryInsert | 249444 | 249936 | 984
Sorted | 1000 | True | Merge | MergeSort | 6043 | 5044 | 5044
Sorted | 1000 | True | Merge | MergeSort2 | 10975 | 4932 | 9976
Sorted | 1000 | True | Merge | ShiftSort | 999 | 999 | 0
Sorted | 1000 | True | Distributed | BucketSort | 2000 | 1000 | 0
Sorted | 1000 | True | Distributed | RadixLSD10Sort | 6030 | 3000 | 0
Sorted | 1000 | True | Distributed | RadixLSD4Sort | 9786 | 0 | 0
Sorted | 1000 | True | Distributed | CountingSort | 3999 | 0 | 0
Sorted | 1000 | True | Hybrid | IntroSortMedian9 | 5994 | 6450 | 0

#### Size : 10000

InputType | ArraySize | IsSorted | SortType | Algorithm | IndexAccessCount | CompareCount | SwapCount
---- | ---- | ---- | ---- | ---- | ---- | ---- | ----
Sorted | 10000 | True | Exchange | BubbleSort | 49995000 | 49995000 | 0
Sorted | 10000 | True | Exchange | OddEvenSort | 9999 | 9999 | 0
Sorted | 10000 | True | Exchange | CocktailShakerSort | 9999 | 9999 | 0
Sorted | 10000 | True | Exchange | CocktailShakerSort2 | 19997 | 19997 | 0
Sorted | 10000 | True | Exchange | CombSort | 286729 | 286729 | 0
Sorted | 10000 | True | Exchange | CycleSort | 50004999 | 49995000 | 0
Sorted | 10000 | True | Exchange | GnomeSort | 10000 | 0 | 0
Sorted | 10000 | True | Exchange | GnomeSort1 | 9999 | 9999 | 0
Sorted | 10000 | True | Exchange | GnomeSort2 | 9999 | 9999 | 0
Sorted | 10000 | True | Exchange | GnomeSort3 | 10000 | 10000 | 0
Sorted | 10000 | True | Selection | SelectionSort | 49995000 | 49995000 | 10000
Sorted | 10000 | True | Selection | HeapSort | 227261 | 330893 | 227262
Sorted | 10000 | True | Insertion | InsertSort | 0 | 0 | 0
Sorted | 10000 | True | Insertion | BinaryInsertSort | 133629 | 113631 | 9999
Sorted | 10000 | True | Insertion | ShellSort | 0 | 75084 | 0
Sorted | 10000 | True | Insertion | BinaryTreeSort | 10000 | 49995000 | 0
Sorted | 10000 | True | Partition | QuickSortMedian3 | 16681461 | 16711444 | 9999
Sorted | 10000 | True | Partition | QuickSortMedian9 | 123617 | 203609 | 9999
Sorted | 10000 | True | Partition | QuickDualPivotSort | 24995000 | 25000000 | 10000
Sorted | 10000 | True | Partition | QuickSortMedian3Insert | 16690155 | 16709852 | 9846
Sorted | 10000 | True | Partition | QuickSortMedian9Insert | 100000 | 107169 | 1023
Sorted | 10000 | True | Partition | QuickDualPivotSortInsert | 24994944 | 24999936 | 9984
Sorted | 10000 | True | Partition | QuickSortMedian3BinaryInsert | 16690380 | 16710032 | 9861
Sorted | 10000 | True | Partition | QuickSortMedian9BinaryInsert | 100120 | 107265 | 1031
Sorted | 10000 | True | Partition | QuickDualPivotSortBinaryInsert | 24994944 | 24999936 | 9984
Sorted | 10000 | True | Merge | MergeSort | 79007 | 69008 | 69008
Sorted | 10000 | True | Merge | MergeSort2 | 143615 | 64608 | 133616
Sorted | 10000 | True | Merge | ShiftSort | 9999 | 9999 | 0
Sorted | 10000 | True | Distributed | BucketSort | 20000 | 10000 | 0
Sorted | 10000 | True | Distributed | RadixLSD10Sort | 80040 | 40000 | 0
Sorted | 10000 | True | Distributed | RadixLSD4Sort | 81750 | 0 | 0
Sorted | 10000 | True | Distributed | CountingSort | 39999 | 0 | 0
Sorted | 10000 | True | Hybrid | IntroSortMedian9 | 99990 | 107160 | 0

### SameValues

#### Size : 100

InputType | ArraySize | IsSorted | SortType | Algorithm | IndexAccessCount | CompareCount | SwapCount
---- | ---- | ---- | ---- | ---- | ---- | ---- | ----
SameValues | 100 | True | Exchange | BubbleSort | 4950 | 4950 | 2329
SameValues | 100 | True | Exchange | OddEvenSort | 4455 | 4455 | 2329
SameValues | 100 | True | Exchange | CocktailShakerSort | 3326 | 3326 | 2329
SameValues | 100 | True | Exchange | CocktailShakerSort2 | 4170 | 4170 | 2329
SameValues | 100 | True | Exchange | CombSort | 1195 | 1195 | 140
SameValues | 100 | True | Exchange | CycleSort | 12263 | 12619 | 88
SameValues | 100 | True | Exchange | GnomeSort | 2429 | 2329 | 2329
SameValues | 100 | True | Exchange | GnomeSort1 | 4756 | 4756 | 2329
SameValues | 100 | True | Exchange | GnomeSort2 | 2347 | 2427 | 2329
SameValues | 100 | True | Exchange | GnomeSort3 | 4758 | 4758 | 2329
SameValues | 100 | True | Selection | SelectionSort | 4950 | 4950 | 100
SameValues | 100 | True | Selection | HeapSort | 661 | 1009 | 566
SameValues | 100 | True | Insertion | InsertSort | 2329 | 2329 | 2329
SameValues | 100 | True | Insertion | BinaryInsertSort | 3039 | 512 | 2428
SameValues | 100 | True | Insertion | ShellSort | 329 | 282 | 329
SameValues | 100 | True | Insertion | BinaryTreeSort | 100 | 886 | 0
SameValues | 100 | True | Partition | QuickSortMedian3 | 277 | 562 | 261
SameValues | 100 | True | Partition | QuickSortMedian9 | 182 | 1027 | 271
SameValues | 100 | True | Partition | QuickDualPivotSort | 415 | 490 | 226
SameValues | 100 | True | Partition | QuickSortMedian3Insert | 231 | 261 | 89
SameValues | 100 | True | Partition | QuickSortMedian9Insert | 123 | 192 | 94
SameValues | 100 | True | Partition | QuickDualPivotSortInsert | 299 | 354 | 177
SameValues | 100 | True | Partition | QuickSortMedian3BinaryInsert | 343 | 331 | 103
SameValues | 100 | True | Partition | QuickSortMedian9BinaryInsert | 187 | 232 | 102
SameValues | 100 | True | Partition | QuickDualPivotSortBinaryInsert | 299 | 354 | 177
SameValues | 100 | True | Merge | MergeSort | 544 | 535 | 624
SameValues | 100 | True | Merge | MergeSort2 | 771 | 524 | 672
SameValues | 100 | True | Merge | ShiftSort | 866 | 642 | 13
SameValues | 100 | True | Distributed | BucketSort | 200 | 100 | 0
SameValues | 100 | True | Distributed | RadixLSD10Sort | 426 | 200 | 0
SameValues | 100 | True | Distributed | RadixLSD4Sort | 2835 | 0 | 0
SameValues | 100 | True | Distributed | CountingSort | 398 | 0 | 0
SameValues | 100 | True | Hybrid | IntroSortMedian9 | 143 | 214 | 111

#### Size : 1000

InputType | ArraySize | IsSorted | SortType | Algorithm | IndexAccessCount | CompareCount | SwapCount
---- | ---- | ---- | ---- | ---- | ---- | ---- | ----
SameValues | 1000 | True | Exchange | BubbleSort | 499500 | 499500 | 230690
SameValues | 1000 | True | Exchange | OddEvenSort | 455544 | 455544 | 230690
SameValues | 1000 | True | Exchange | CocktailShakerSort | 323073 | 323073 | 230690
SameValues | 1000 | True | Exchange | CocktailShakerSort2 | 376740 | 376740 | 230690
SameValues | 1000 | True | Exchange | CombSort | 20705 | 20705 | 1274
SameValues | 1000 | True | Exchange | CycleSort | 1265543 | 1305569 | 884
SameValues | 1000 | True | Exchange | GnomeSort | 231690 | 230690 | 230690
SameValues | 1000 | True | Exchange | GnomeSort1 | 462377 | 462377 | 230690
SameValues | 1000 | True | Exchange | GnomeSort2 | 230800 | 231687 | 230690
SameValues | 1000 | True | Exchange | GnomeSort3 | 462380 | 462380 | 230690
SameValues | 1000 | True | Selection | SelectionSort | 499500 | 499500 | 1000
SameValues | 1000 | True | Selection | HeapSort | 9507 | 15931 | 8511
SameValues | 1000 | True | Insertion | InsertSort | 230690 | 230690 | 230690
SameValues | 1000 | True | Insertion | BinaryInsertSort | 241214 | 8526 | 231689
SameValues | 1000 | True | Insertion | ShellSort | 4119 | 4821 | 4119
SameValues | 1000 | True | Insertion | BinaryTreeSort | 1000 | 52758 | 0
SameValues | 1000 | True | Partition | QuickSortMedian3 | 2134 | 5019 | 4293
SameValues | 1000 | True | Partition | QuickSortMedian9 | 2024 | 10136 | 4301
SameValues | 1000 | True | Partition | QuickDualPivotSort | 5020 | 5751 | 1620
SameValues | 1000 | True | Partition | QuickSortMedian3Insert | 2137 | 2302 | 2348
SameValues | 1000 | True | Partition | QuickSortMedian9Insert | 2034 | 2729 | 2384
SameValues | 1000 | True | Partition | QuickDualPivotSortInsert | 5020 | 5751 | 1620
SameValues | 1000 | True | Partition | QuickSortMedian3BinaryInsert | 2214 | 2358 | 2355
SameValues | 1000 | True | Partition | QuickSortMedian9BinaryInsert | 2111 | 2785 | 2391
SameValues | 1000 | True | Partition | QuickDualPivotSortBinaryInsert | 5020 | 5751 | 1620
SameValues | 1000 | True | Merge | MergeSort | 7158 | 8396 | 9511
SameValues | 1000 | True | Merge | MergeSort2 | 10975 | 8441 | 9976
SameValues | 1000 | True | Merge | ShiftSort | 14114 | 10316 | 130
SameValues | 1000 | True | Distributed | BucketSort | 2000 | 1000 | 0
SameValues | 1000 | True | Distributed | RadixLSD10Sort | 6039 | 3000 | 0
SameValues | 1000 | True | Distributed | RadixLSD4Sort | 10032 | 0 | 0
SameValues | 1000 | True | Distributed | CountingSort | 3965 | 0 | 0
SameValues | 1000 | True | Hybrid | IntroSortMedian9 | 2173 | 2854 | 2307

#### Size : 10000

InputType | ArraySize | IsSorted | SortType | Algorithm | IndexAccessCount | CompareCount | SwapCount
---- | ---- | ---- | ---- | ---- | ---- | ---- | ----
SameValues | 10000 | True | Exchange | BubbleSort | 49995000 | 49995000 | 22450443
SameValues | 10000 | True | Exchange | OddEvenSort | 45005499 | 45005499 | 22450443
SameValues | 10000 | True | Exchange | CocktailShakerSort | 32010280 | 32010280 | 22450443
SameValues | 10000 | True | Exchange | CocktailShakerSort2 | 37296720 | 37296720 | 22450443
SameValues | 10000 | True | Exchange | CombSort | 296728 | 296728 | 13103
SameValues | 10000 | True | Exchange | CycleSort | 126742580 | 130762800 | 9029
SameValues | 10000 | True | Exchange | GnomeSort | 22460443 | 22450443 | 22450443
SameValues | 10000 | True | Exchange | GnomeSort1 | 44910883 | 44910883 | 22450443
SameValues | 10000 | True | Exchange | GnomeSort2 | 22451441 | 22460440 | 22450443
SameValues | 10000 | True | Exchange | GnomeSort3 | 44910886 | 44910886 | 22450443
SameValues | 10000 | True | Selection | SelectionSort | 49995000 | 49995000 | 10000
SameValues | 10000 | True | Selection | HeapSort | 125192 | 218639 | 115195
SameValues | 10000 | True | Insertion | InsertSort | 22450443 | 22450443 | 22450443
SameValues | 10000 | True | Insertion | BinaryInsertSort | 22588936 | 118495 | 22460442
SameValues | 10000 | True | Insertion | ShellSort | 36442 | 75084 | 36442
SameValues | 10000 | True | Insertion | BinaryTreeSort | 10000 | 5035636 | 0
SameValues | 10000 | True | Partition | QuickSortMedian3 | 24416 | 53360 | 59578
SameValues | 10000 | True | Partition | QuickSortMedian9 | 20811 | 100982 | 59878
SameValues | 10000 | True | Partition | QuickDualPivotSort | 37910 | 47784 | 19048
SameValues | 10000 | True | Partition | QuickSortMedian3Insert | 25308 | 27081 | 40593
SameValues | 10000 | True | Partition | QuickSortMedian9Insert | 21664 | 28173 | 40599
SameValues | 10000 | True | Partition | QuickDualPivotSortInsert | 37910 | 47784 | 19048
SameValues | 10000 | True | Partition | QuickSortMedian3BinaryInsert | 25518 | 27249 | 40607
SameValues | 10000 | True | Partition | QuickSortMedian9BinaryInsert | 21784 | 28269 | 40607
SameValues | 10000 | True | Partition | QuickDualPivotSortBinaryInsert | 37910 | 47784 | 19048
SameValues | 10000 | True | Merge | MergeSort | 92050 | 116476 | 129519
SameValues | 10000 | True | Merge | MergeSort2 | 143615 | 116496 | 133616
SameValues | 10000 | True | Merge | ShiftSort | 202369 | 148651 | 1253
SameValues | 10000 | True | Distributed | BucketSort | 20000 | 10000 | 0
SameValues | 10000 | True | Distributed | RadixLSD10Sort | 80054 | 40000 | 0
SameValues | 10000 | True | Distributed | RadixLSD4Sort | 82029 | 0 | 0
SameValues | 10000 | True | Distributed | CountingSort | 37954 | 0 | 0
SameValues | 10000 | True | Hybrid | IntroSortMedian9 | 21267 | 28369 | 40246


## Charts

### By Performance

![](images/by_performance/o^2.png)
![](images/by_performance/onlogn.png)
![](images/by_performance/on.png)

### By SortType

![](images/by_sorttype/exchange_10000.png)
![](images/by_sorttype/insertion_10000.png)
![](images/by_sorttype/selection_10000.png)
![](images/by_sorttype/merge_10000.png)
![](images/by_sorttype/partition_10000.png)
![](images/by_sorttype/hybrid_10000.png)
![](images/by_sorttype/distributed_10000.png)

### By InputType

![](images/by_inputtype/random_10000.png)
![](images/by_inputtype/reversed_10000.png)
![](images/by_inputtype/mountain_10000.png)
![](images/by_inputtype/nearlysorted_10000.png)
![](images/by_inputtype/sorted_10000.png)
![](images/by_inputtype/samevalues_10000.png)
