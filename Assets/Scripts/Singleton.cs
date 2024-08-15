using UnityEngine;

namespace Supermarket
{
    /// <summary>
    /// simple singleton
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        public static T instance { get; private set; } = null;

        public static bool ready => instance != null;

        internal protected static void New()
        {
            New($"{typeof(T)} Manager");
        }

        internal protected static void New(string name)
        {
            var go = new GameObject(name);
            instance = go.AddComponent<T>();
        }
        /// <summary>
        /// Create new single ton Monobehaviour
        /// </summary>
        /// <param name="target"></param>
        /// <param name="dontDestrodOnLoad"></param>
        /// <param name="removeDuplicates"></param>
        protected void CreateInstance(T target, bool dontDestrodOnLoad = false, bool removeDuplicates = false)
        {
            if (ready && !removeDuplicates && instance != target)
            {
                //go away, I don't need you
                //Debug.Log("more than 1" + typeof(T).ToString(), this);
                Destroy(target.gameObject);
                return;
            }
            else if (ready && removeDuplicates && instance != target)
            {
                //go away, I don't need you more:)
                Destroy(instance.gameObject);
            }

            instance = target;

            /*T[] result = FindObjectsOfType<T>();
            if(result.Length > 1)
            {
                Debug.Log("more than " + typeof(T).ToString() + " found");
                for (int i = 0; i < result.Length; i++)
                {
                    Debug.Log(typeof(T).ToString() + $"[{i}]", result[i]);
                    if(i != 0)
                        Destroy(result[i].gameObject);
                }
            }
            instance = result[0];

            if (!instance)
            {
                var clone = new GameObject();
                clone.name = typeof(T).ToString() + "_SingleTon";
                instance = clone.AddComponent<T>();
            }*/

            if (dontDestrodOnLoad)
            {
                if (transform.parent == null)
                    DontDestroyOnLoad(target);
                else
                    Debug.LogWarning("should be root object");
            }
        }
#if UNITY_EDITOR
        static void DomainReload()
        {
            instance = null;
        }
#endif
    }
}