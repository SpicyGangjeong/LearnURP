using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public interface IPoolable
{
    bool IsActive { get; }
    void CreateItem();
    void DestroyItem();

    void SpawnItem();
    void DespawnItem();
}

public class Pool
{
    IPoolable[] pool;
    int iIndex = 0;
    IPoolable Get()
    {
        if (iIndex > pool.Count) {
            iIndex = 0;
        }
        if (false == pool[iIndex].bActive)
        {
            pool[iIndex].bActive = true;
            return pool[iIndex++];
        }
    }
}

public class ObjectPoolManager
{
    //Dictionary<string, Pool> dictPool = { };

    IPoolable Get(string key)
    {
        return dictPool[key].Get();
    }

}