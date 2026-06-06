using System.Collections.Generic;
using UnityEngine;

public interface IPoolable
{
    void OnCreate();
    void OnDestroy();
    void OnSpawn();
    void OnDespawn();
}

public static class PoolKeys
{
    public const string CardCanvas = "CardCanvas";
}

interface IObjectPool

{
    void Clear();
    int TotalCount { get; }
    int InactiveCount { get; }
}

public class ObjectPool<T> : IObjectPool where T : Component
{
    readonly Stack<T> inactive = new Stack<T>();
    readonly List<T> allInstances = new List<T>();
    readonly T prefab;
    readonly Transform poolRoot;
    readonly int maxSize;
    int totalCount;

    public int TotalCount => totalCount;
    public int InactiveCount => inactive.Count;

    public ObjectPool(T prefab, Transform poolRoot, int warmCount, int maxSize)
    {
        this.prefab = prefab;
        this.poolRoot = poolRoot;
        this.maxSize = maxSize;

        int initialCount = Mathf.Max(0, warmCount);
        for (int i = 0; i < initialCount; i++)
        {
            inactive.Push(CreateInstance());
        }
    }

    void ExpandPool()
    {
        if (maxSize > 0 && totalCount >= maxSize)
        {
            return;
        }

        int growCount = Mathf.Max(1, totalCount / 2);
        if (maxSize > 0)
        {
            growCount = Mathf.Min(growCount, maxSize - totalCount);
        }

        for (int i = 0; i < growCount; i++)
        {
            inactive.Push(CreateInstance());
        }
    }

    T CreateInstance()
    {
        T instance = Object.Instantiate(prefab, poolRoot);
        instance.gameObject.SetActive(false);
        allInstances.Add(instance);
        totalCount++;

        IPoolable poolable = instance as IPoolable;
        if (null != poolable)
        {
            poolable.OnCreate();
        }

        return instance;
    }

    public T Get(Transform parent)
    {
        if (0 == inactive.Count)
        {
            ExpandPool();
        }

        if (0 == inactive.Count)
        {
            Debug.LogError($"ObjectPool<{typeof(T).Name}> exhausted (total {totalCount}, max {(maxSize <= 0 ? "unlimited" : maxSize.ToString())}).");
            return null;
        }

        T instance = inactive.Pop();

        Transform instanceTransform = instance.transform;
        if (null != parent)
        {
            instanceTransform.SetParent(parent, false);
        }

        instance.gameObject.SetActive(true);

        IPoolable poolable = instance as IPoolable;
        if (null != poolable)
        {
            poolable.OnSpawn();
        }

        return instance;
    }

    public void Release(T instance)
    {
        if (null == instance)
        {
            return;
        }

        IPoolable poolable = instance as IPoolable;
        if (null != poolable)
        {
            poolable.OnDespawn();
        }

        instance.gameObject.SetActive(false);
        instance.transform.SetParent(poolRoot, false);
        inactive.Push(instance);
    }

    public void Clear()
    {
        for (int i = allInstances.Count - 1; i >= 0; i--)
        {
            T instance = allInstances[i];
            if (null == instance)
            {
                continue;
            }

            IPoolable poolable = instance as IPoolable;
            if (null != poolable)
            {
                poolable.OnDestroy();
            }

            Object.Destroy(instance.gameObject);
        }

        allInstances.Clear();
        inactive.Clear();
        totalCount = 0;
    }
}

public class ObjectPoolManager
{
    readonly Dictionary<string, IObjectPool> pools = new Dictionary<string, IObjectPool>();
    readonly Transform poolRoot;

    public ObjectPoolManager(Transform poolRoot)
    {
        this.poolRoot = poolRoot;
    }

    public void Register<T>(string key, T prefab, int warmCount, int maxSize = 0) where T : Component
    {
        if (null == prefab)
        {
            Debug.LogError($"ObjectPoolManager.Register: prefab is null for key '{key}'.");
            return;
        }

        pools[key] = new ObjectPool<T>(prefab, poolRoot, warmCount, maxSize);
    }

    public T Get<T>(string key, Transform parent = null) where T : Component
    {
        if (false == pools.TryGetValue(key, out IObjectPool pool))
        {
            Debug.LogError($"ObjectPoolManager.Get: pool not registered for key '{key}'.");
            return null;
        }

        return ((ObjectPool<T>)pool).Get(parent);
    }

    public void Release<T>(string key, T instance) where T : Component
    {
        if (false == pools.TryGetValue(key, out IObjectPool pool))
        {
            Debug.LogError($"ObjectPoolManager.Release: pool not registered for key '{key}'.");
            return;
        }

        ((ObjectPool<T>)pool).Release(instance);
    }

    public bool IsRegistered(string key)
    {
        return pools.ContainsKey(key);
    }

    public void ClearPool(string key)
    {
        if (pools.TryGetValue(key, out IObjectPool pool))
        {
            pool.Clear();
            pools.Remove(key);
        }
    }

    public void ClearAll()
    {
        foreach (IObjectPool pool in pools.Values)
        {
            pool.Clear();
        }

        pools.Clear();
    }
}
