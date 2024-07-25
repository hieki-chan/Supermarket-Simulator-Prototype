using System.Collections.Generic;
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

        public static List<T> ForeachRandomStart<T>(this List<T> list, System.Action<T> callback)
        {
            int count = list.Count;
            int realIndex = Random.Range(0, count);
            int loopIndex = realIndex;

            for (int i = 0; i < count; i++)
            {
                var item = list[realIndex];

                callback?.Invoke(item);

                loopIndex++;
                realIndex = (int)Mathf.Repeat(loopIndex, count - 1);
            }

            return list ;
        }
    }
}
