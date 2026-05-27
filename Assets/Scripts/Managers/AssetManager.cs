using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

class AssetManager
{
    Dictionary<string, AsyncOperationHandle<Object>> assetHandles =
        new Dictionary<string, AsyncOperationHandle<Object>>();
    Dictionary<string, AsyncOperationHandle<IList<Object>>> assetLabelHandles =
        new Dictionary<string, AsyncOperationHandle<IList<Object>>>();
    public async Task<AsyncOperationHandle<Object>> LoadAddressAssetAsync(string assetName)
    {
        if (assetHandles.ContainsKey(assetName))
        {
            return assetHandles[assetName];
        }

        AsyncOperationHandle<Object> handle = Addressables.LoadAssetAsync<Object>(assetName);
        await handle.Task;

        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            assetHandles[assetName] = handle;
            return handle;
        } else {
            Debug.LogError($"Failed to load asset: '{assetName}'");
            if (handle.IsValid())
            {
                Addressables.Release(handle);
            }   
            return default;
        }
    }
    public async Task<AsyncOperationHandle<IList<Object>>> LoadLabelAssetsAsync(string assetLabelName)
    {
        if (assetLabelHandles.ContainsKey(assetLabelName))
        {
            return assetLabelHandles[assetLabelName];
        }

        AsyncOperationHandle<IList<Object>> handle = Addressables.LoadAssetsAsync<Object>(
            assetLabelName,
            null,
            Addressables.MergeMode.Union);

        await handle.Task;

        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            assetLabelHandles[assetLabelName] = handle;
            return handle;
        } else {
            Debug.LogError($"Failed to load assets with label: '{assetLabelName}'");
            if (handle.IsValid())
            {
                Addressables.Release(handle);
            }
            return default;
        }
    }

    public void ReleaseAssets(){
        foreach (var handle in assetLabelHandles)
        {
            if (handle.Value.IsValid())
            {
                Addressables.Release(handle.Value);
            }
        }
        foreach (var handle in assetHandles)
        {
            if (handle.Value.IsValid())
            {
                Addressables.Release(handle.Value);
            }
        }
    }
}
