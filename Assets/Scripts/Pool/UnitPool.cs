using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Supermarket
{
    //[System.Serializable]
    public class MonoPool<T> where T : Component
    {
        public Queue<T> pool = new Queue<T>();
        T prefab;

        public MonoPool()
        {

        }

        public MonoPool(T prefab)
        {
            this.prefab = prefab;
        }

        public MonoPool(T prefab, int size)
        {
            this.prefab = prefab;
            Create(size);
        }

        public void Create(int size, Action<T> OnCreated = null)
        {
            if (prefab)
                Create(prefab, size, OnCreated);
            else
                Debug.LogWarning("No prefab has been assigned");
        }

        public void Create(T component, int size, Action<T> OnCreated = null)
        {
            for (int i = 0; i < size; i++)
            {
                T go = Object.Instantiate(component);
                go.gameObject.SetActive(false);
                pool.Enqueue(go);

                OnCreated?.Invoke(go);
            }
        }

        public T Get(bool activeObject = true, Action<T> OnGet = null)
        {
            if (!pool.TryDequeue(out T cpn))
            {
                return null;
            }
            OnGet?.Invoke(cpn);
            cpn.gameObject.SetActive(activeObject);
            return cpn;
        }

        public T Get(Vector3 position, Quaternion rotation, bool activeObject = true)
        {
            if (!pool.TryDequeue(out T cpn))
            {
                return null;
            }
            cpn.transform.SetPositionAndRotation(position, rotation);
            cpn.gameObject.SetActive(activeObject);
            return cpn;
        }

        public T Get(Vector3 position, Quaternion rotation, Action<T> OnGet, bool activeObject = true)
        {
            if (!pool.TryDequeue(out T cpn))
            {
                return null;
            }
            OnGet?.Invoke(cpn);
            cpn.transform.SetPositionAndRotation(position, rotation);
            cpn.gameObject.SetActive(activeObject);
            return cpn;
        }

        public T GetOrCreate(bool activeObject = true, int createCount = 1)
        {
            if (pool.Count == 0)
                Create(createCount);

            return Get(activeObject);
        }

        public T GetOrCreate(Vector3 position, Quaternion rotation, bool activeObject = true, int createCount = 1)
        {
            if (pool.Count == 0)
                Create(createCount);

            return Get(position, rotation, activeObject);
        }

        public T GetOrCreate(T component, Vector3 position, Quaternion rotation, bool activeObject = true, int createCount = 1)
        {
            if (pool.Count == 0)
                Create(component, createCount);

            return Get(position, rotation, activeObject);
        }

        public T GetOrCreate(T component, Vector3 position, Quaternion rotation, Action<T> OnGet, bool activeObject = true, int createCount = 1)
        {
            if (pool.Count == 0)
                Create(component, createCount);

            return Get(position, rotation, OnGet, activeObject);
        }

        public void AddExisting(T component)
        {
            pool.Enqueue(component);
        }

        public void Return(T component, bool active = false)
        {
            pool.Enqueue(component);
            component.gameObject.SetActive(active);
        }
    }

    public class UnitPool<TKey, TValue> where TValue : Component
    {
        Dictionary<TKey, MonoPool<TValue>> m_Pools = new Dictionary<TKey, MonoPool<TValue>>();
        public int Capacity = 1;

        public UnitPool() { }

        public UnitPool(int Capacity)
        {
            this.Capacity = Capacity;
        }

        public void Create(TKey key, TValue component, int size)
        {
            if (!m_Pools.TryGetValue(key, out MonoPool<TValue> pool))
            {
                pool = new MonoPool<TValue>();
                m_Pools.Add(key, pool);
            }

            pool.Create(component, size);
        }

        public TValue Get(TKey key, Vector3 position, Quaternion rotation)
        {
            if (!m_Pools.TryGetValue(key, out MonoPool<TValue> pool))
            {
                return null;
            }

            return pool.Get(position, rotation);
        }

        public TValue GetOrCreate(TKey key, TValue component, Vector3 position, Quaternion rotation, bool activeObject = true)
        {
            if (!m_Pools.TryGetValue(key, out MonoPool<TValue> pool))
            {
                pool = new MonoPool<TValue>();
                m_Pools.Add(key, pool);
            }

            return pool.GetOrCreate(component, position, rotation, activeObject, Capacity);
        }

        public T GetOrCreate<T>(TKey key, TValue component, Vector3 position, Quaternion rotation, bool activeObject = true) where T : TValue
        {
            if (!m_Pools.TryGetValue(key, out MonoPool<TValue> pool))
            {
                pool = new MonoPool<TValue>();
                m_Pools.Add(key, pool);
            }

            return pool.GetOrCreate(component, position, rotation, activeObject, Capacity) as T;
        }

        public TValue GetOrCreate(TKey key, TValue component, Vector3 position, Quaternion rotation, Action<TValue> OnGet, bool activeObject = true)
        {
            if (!m_Pools.TryGetValue(key, out MonoPool<TValue> pool))
            {
                pool = new MonoPool<TValue>();
                m_Pools.Add(key, pool);
            }

            return pool.GetOrCreate(component, position, rotation, OnGet, activeObject, Capacity);
        }

        public void Return(TKey key, TValue component)
        {
            if (!m_Pools.TryGetValue(key, out MonoPool<TValue> pool))
            {
                pool = new MonoPool<TValue>();
                m_Pools.Add(key, pool);
            }

            pool.Return(component);
        }
    }


    /*
    [Serializable]
    public class PoolObject<TKey, TValue>
    {
        public TKey key;
        public TValue component;
    }*/
}
