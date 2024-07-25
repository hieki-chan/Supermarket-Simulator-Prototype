using UnityEngine;

namespace Hieki.Utils
{
    public static class ArrayExtensions
    {
        public static T PickOne<T>(this T[] array)
        {
            if (array == null || array.Length == 0)
                return default;

            return array[Random.Range(0, array.Length)];
        }
    }
}
