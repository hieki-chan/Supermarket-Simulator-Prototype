using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Hieki.Utils
{
    public static class ListExtensions
    {
        public static List<T> RemoveAtSwapBack<T>(this List<T> list, int index)
        {
            if (list == null || index < 0 || index > list.Count)
            {
                return list;
            }

            int count = list.Count;
            list[index] = list[count - 1];
            list.RemoveAt(count - 1);

            return list;
        }

        public static T PickOne<T>(this List<T> list)
        {
            if (list == null || list.Count == 0)
                return default;

            return list[Random.Range(0, list.Count)];
        }

        /// <summary>
        /// Swaps two elements in the list at the specified indices.
        /// </summary>
        public static void SwapAt<T>(this List<T> source, int index1, int index2)
        {
            if (index1 >= source.Count || index2 >= source.Count || index1 < 0 || index2 < 0)
            {
                Debug.Log("Swaping List: Index is out of range");
                return;
            }

            T val = source[index1];
            source[index1] = source[index2];
            source[index2] = val;
        }

        public static List<T> ForeachRandomStart<T>(this List<T> list, System.Func<T, bool> callback)
        {
            int count = list.Count;
            int realIndex = Random.Range(0, count);
            int loopIndex = realIndex;

            for (int i = 0; i < count; i++)
            {
                var item = list[realIndex];

                if((bool)(callback?.Invoke(item)))
                {
                    return list;
                }

                loopIndex++;
                realIndex = loopIndex >= count ? loopIndex - count : loopIndex;
            }

            return list ;
        }
    }
}
