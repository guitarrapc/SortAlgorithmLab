using System;
using System.Collections.Generic;
using System.Text;

namespace SortAlgorithm.Logics
{
    /// <summary>
    /// リストの先頭に戻る前に、前回の位置を覚えておくことで<see cref="GnomeSort{T}"/>を最適化している。これにより、<see cref="InsertSort{T}"/>と同程度の計算量になる。
    /// </summary>
    /// <remarks>
    /// stable : yes
    /// inplace : yes
    /// Compare : 
    /// Swap : 
    /// Order : O(n^2)
    /// sortKind : GnomeSortOptimized, ArraySize : 100, IndexAccessCount : 5008, CompareCount : 5008, SwapCount : 2456
    /// </remarks>
    /// <typeparam name="T"></typeparam>
    public class GnomeSortOptimized<T> : SortBase<T> where T : IComparable<T>
    {
        public override T[] Sort(T[] array)
        {
            base.sortStatics.Reset(array.Length);
            for (var i = 0; i < array.Length; i++)
            {
                base.sortStatics.AddIndexAccess();

                while (i > 0 && array[i - 1].CompareTo(array[i]) > 0)
                {
                    base.sortStatics.AddCompareCount();
                    base.sortStatics.AddSwapCount();
                    Swap(ref array[i - 1], ref array[i]);
                    i--;
                }
            }
            return array;
        }
    }
}
