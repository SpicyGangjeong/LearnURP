using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

class AssetManager
{
    Dictionary<string, AsyncOperationHandle<Object>> m_vAssetHandles =
        new Dictionary<string, AsyncOperationHandle<Object>>();
    Dictionary<string, AsyncOperationHandle<IList<Object>>> m_vAssetLabelHandles =
        new Dictionary<string, AsyncOperationHandle<IList<Object>>>();
    public async Task<AsyncOperationHandle<Object>> LoadAddressAssetAsync(string strAssetName)
    {
        if (m_vAssetHandles.ContainsKey(strAssetName))
        {
            return m_vAssetHandles[strAssetName];
        }

        AsyncOperationHandle<Object> hHandle = Addressables.LoadAssetAsync<Object>(strAssetName);
        await hHandle.Task;

        if (hHandle.Status == AsyncOperationStatus.Succeeded)
        {
            m_vAssetHandles[strAssetName] = hHandle;
            return hHandle;
        } else {
            Debug.LogError($"Failed to load asset: '{strAssetName}'");
            if (hHandle.IsValid())
            {
                Addressables.Release(hHandle);
            }   
            return default;
        }
    }
    public async Task<AsyncOperationHandle<IList<Object>>> LoadLabelAssetsAsync(string strAssetLabelName)
    {
        if (m_vAssetLabelHandles.ContainsKey(strAssetLabelName))
        {
            return m_vAssetLabelHandles[strAssetLabelName];
        }

        AsyncOperationHandle<IList<Object>> hHandle = Addressables.LoadAssetsAsync<Object>(
            strAssetLabelName,
            null,
            Addressables.MergeMode.Union);

        await hHandle.Task;

        if (hHandle.Status == AsyncOperationStatus.Succeeded)
        {
            m_vAssetLabelHandles[strAssetLabelName] = hHandle;
            return hHandle;
        } else {
            Debug.LogError($"Failed to load assets with label: '{strAssetLabelName}'");
            if (hHandle.IsValid())
            {
                Addressables.Release(hHandle);
            }
            return default;
        }
    }

    public void ReleaseAssets(){
        foreach (KeyValuePair<string, AsyncOperationHandle<IList<Object>>> pPair in m_vAssetLabelHandles)
        {
            if (pPair.Value.IsValid())
            {
                Addressables.Release(pPair.Value);
            }
        }
        foreach (KeyValuePair<string, AsyncOperationHandle<Object>> pPair in m_vAssetHandles)
        {
            if (pPair.Value.IsValid())
            {
                Addressables.Release(pPair.Value);
            }
        }
    }
}
