using System;
using System.Collections.Generic;
using UnityEngine;

namespace Hieki.Utils
{
    public static class TransformExtensions
    {
        /// <summary>
        /// ActivatesDeactivates the <see cref="Transform"/>, depending on the given true or false/ value.
        /// </summary>
        public static void SetActive(this Transform transform, bool value)
        {
            transform.gameObject.SetActive(value);
        }
        /// <summary>
        /// Get position by given offset.
        /// </summary>
        public static Vector3 GetPositionByOffset(this Transform transform, Vector3 offset)
           => transform.position + transform.right * offset.x + transform.forward * offset.z + transform.up * offset.y;

        /// <summary>
        /// Find the closet <see cref="Transform"/> to the  given one.
        /// </summary>
        /// <returns></returns>
        public static T FindCloset<T>(this Transform source, IEnumerable<T> others) where T : Component
        {
            float shortestDistance = float.PositiveInfinity;
            T closetTransform = null;

            foreach (var component in others)
            {
                float distance = (source.position - component.transform.position).sqrMagnitude;

                if (distance < shortestDistance)
                {
                    shortestDistance = distance;
                    closetTransform = component;
                }
            }

            return closetTransform;
        }

        /// <summary>
        /// Sorts the list of components by their distance to the specified position. (ascending)
        /// </summary>
        public static void SortedByDistanceTo<T>(this List<T> source, Vector3 position) where T : Component
        {
            int count = source.Count;
            for (int i = 0; i < count - 1; i++)
            {
                int current_min_j = i;
                float distance_min_j = (position - source[i].transform.position).sqrMagnitude;

                for (int j = i + 1; j < count; j++)
                {
                    float distance_j = (position - source[j].transform.position).sqrMagnitude;

                    if (distance_j < distance_min_j)
                    {
                        current_min_j = j;
                        distance_min_j = distance_j;
                    }
                }

                if (i != current_min_j)
                    source.SwapAt(i, current_min_j);
            }
        }

        /// <summary>
        /// Sorts the list of components according to the specified comparer function.
        /// </summary>
        public static void SortedBy<T>(this List<T> source, Func<T, T, bool> comparer) where T : Component
        {
            int count = source.Count;
            for (int i = 0; i < count - 1; i++)
            {
                int current_min_j = i;

                for (int j = i + 1; j < count; j++)
                {
                    if (comparer(source[i], source[j]))
                    {
                        current_min_j = j;
                    }
                }

                source.SwapAt(i, current_min_j);
            }
        }

        /// <summary>
        /// Loop through all child of this <see cref="Transform"/>.
        /// </summary>
        /// <param name="transform">The parent Transform.</param>
        /// <param name="callback">The callback function to be executed for each child Transform.</param>
        public static void ForeachChilds(this Transform transform, Action<Transform> callback)
        {
            int childCount = transform.childCount;
            for (int i = 0; i < childCount; i++)
            {
                Transform child = transform.GetChild(i);
                callback?.Invoke(child);
            }
        }

        public static void ForeachChilds<T>(this Transform transform, Action<T> callback) where T : Component
        {
            int childCount = transform.childCount;
            for (int i = 0; i < childCount; i++)
            {
                Transform child = transform.GetChild(i);
                T component = child.GetComponent<T>();
                if(component)
                    callback?.Invoke(component);
            }
        }

        /// <summary>
        /// Get an array of child components of type T from the given parent.
        /// </summary>
        /// <typeparam name="T">The type of component to retrieve.</typeparam>
        /// <param name="parent">The parent Transform.</param>
        /// <returns>An array of child components of type T.</returns>
        public static T[] GetChilds<T>(this Transform parent) where T : Component
        {
            int count = parent.childCount;
            List<T> result = new List<T>();
            for (int i = 0; i < count; i++)
            {
                var t = parent.GetChild(i).GetComponent<T>();
                if (t)
                    result.Add(t);
            }
            return result.ToArray();
        }

        /// <summary>
        /// Get all children of this <see cref="Transform"/>
        /// </summary>
        /// <param name="parent">The parent Transform.</param>
        /// <returns>An array of child Transforms.</returns>
        public static Transform[] GetChilds(this Transform parent)
        {
            int count = parent.childCount;
            List<Transform> result = new List<Transform>();
            for (int i = 0; i < count; i++)
            {
                result.Add(parent.GetChild(i));
            }
            return result.ToArray();
        }
    }
}