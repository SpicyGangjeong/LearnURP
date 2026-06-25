using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public interface IPoolable
{
    abstract void OnCreate();
    abstract void OnExtinct();
    abstract void OnSpawn();
    abstract void OnDespawn();
}

public static class PoolKeys
{
    public const string s_strCardCanvas = "CardCanvas";
}

public interface IComponentObjectPool
{
    Component Get(Transform pParent);
    void Release(Component pInstance);
    void Clear();
    int CountInactive { get; }
}

public class ComponentObjectPool<T> : IComponentObjectPool where T : Component
{
    ObjectPool<T> m_pPool;
    Transform m_pPoolRoot;

    public int CountInactive => m_pPool.CountInactive;

    public ComponentObjectPool(T pPrefab, Transform pPoolRoot, int iInitialCount, int iMaxSize)
    {
        m_pPoolRoot = pPoolRoot;

        int iCapacity = Mathf.Max(0, iInitialCount);
        int iPoolMaxSize = iMaxSize <= 0 ? int.MaxValue : iMaxSize;

        m_pPool = new ObjectPool<T>(
            () => CreateInstance(pPrefab),
            OnGet,
            OnRelease,
            OnExtinct,
            collectionCheck: true,
            defaultCapacity: iCapacity,
            maxSize: iPoolMaxSize
        );

        for (int i = 0; i < iCapacity; i++)
        {
            m_pPool.Get();
        }
    }

    T CreateInstance(T pPrefab)
    {
        T pInstance = Object.Instantiate(pPrefab, m_pPoolRoot);
        pInstance.gameObject.SetActive(false);

        if (pInstance is IPoolable pPoolable)
        {
            pPoolable.OnCreate();
        }

        return pInstance;
    }

    void OnGet(T pInstance)
    {
        if (pInstance is IPoolable pPoolable)
        {
            pPoolable.OnSpawn();
        }
    }

    void OnRelease(T pInstance)
    {
        if (pInstance is IPoolable pPoolable)
        {
            pPoolable.OnDespawn();
        }

        pInstance.gameObject.SetActive(false);
        pInstance.transform.SetParent(m_pPoolRoot, false);
    }

    void OnExtinct(T pInstance)
    {
        if (pInstance is IPoolable pPoolable)
        {
            pPoolable.OnExtinct();
        }

        Object.Destroy(pInstance.gameObject);
    }

    public T Get(Transform pParent)
    {
        T pInstance = m_pPool.Get();

        if (null != pParent)
        {
            pInstance.transform.SetParent(pParent, false);
        }
        pInstance.gameObject.SetActive(true);

        return pInstance;
    }

    public void Release(T pInstance)
    {
        if (null == pInstance)
        {
            return;
        }

        m_pPool.Release(pInstance);
    }

    Component IComponentObjectPool.Get(Transform pParent) => Get(pParent);

    void IComponentObjectPool.Release(Component pInstance)
    {
        if (pInstance is T pTypedInstance)
        {
            Release(pTypedInstance);
            return;
        }

        Debug.LogError($"ComponentObjectPool<{typeof(T).Name}>.Release: type mismatch ({pInstance.GetType().Name}).");
    }

    public void Clear()
    {
        m_pPool.Clear();
    }
}

public class ObjectPoolManager
{
    readonly Dictionary<string, IComponentObjectPool> m_vPools = new Dictionary<string, IComponentObjectPool>();
    readonly Transform m_pPoolRoot;

    public ObjectPoolManager(Transform pPoolRoot)
    {
        m_pPoolRoot = pPoolRoot;
    }

    public void Register<T>(string strKey, T pPrefab, int iStartCount, int iMaxSize = 0) where T : Component
    {
        if (null == pPrefab)
        {
            Debug.LogError($"ObjectPoolManager.Register: prefab is null for key '{strKey}'.");
            return;
        }

        m_vPools[strKey] = new ComponentObjectPool<T>(pPrefab, m_pPoolRoot, iStartCount, iMaxSize);
    }

    public Component Get(string strKey, Transform pParent = null)
    {
        if (false == m_vPools.TryGetValue(strKey, out IComponentObjectPool pPool))
        {
            Debug.LogError($"ObjectPoolManager.Get: pool not registered for key '{strKey}'.");
            return null;
        }

        return pPool.Get(pParent);
    }

    public T Get<T>(string strKey, Transform pParent = null) where T : Component
    {
        return Get(strKey, pParent) as T;
    }

    public void Release(string strKey, Component pInstance)
    {
        if (false == m_vPools.TryGetValue(strKey, out IComponentObjectPool pPool))
        {
            Debug.LogError($"ObjectPoolManager.Release: pool not registered for key '{strKey}'.");
            return;
        }
        
        pPool.Release(pInstance);
    }

    public void Release<T>(string strKey, T pInstance) where T : Component
    {
        Release(strKey, (Component)pInstance);
    }

    public bool IsRegistered(string strKey)
    {
        return m_vPools.ContainsKey(strKey);
    }

    public void ClearPool(string strKey)
    {
        if (m_vPools.TryGetValue(strKey, out IComponentObjectPool pPool))
        {
            pPool.Clear();
            m_vPools.Remove(strKey);
        }
    }

    public void ClearAll()
    {
        foreach (IComponentObjectPool pPool in m_vPools.Values)
        {
            pPool.Clear();
        }

        m_vPools.Clear();
    }
}
