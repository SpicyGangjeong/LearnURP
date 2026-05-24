using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class LoadAssetsByLabel<T> where T : Object
{
    readonly string assetLabel;

    public LoadAssetsByLabel(string assetLabel)
    {
        this.assetLabel = assetLabel;
    }

    public async Task<AsyncOperationHandle<IList<T>>> LoadAsync()
    {
        AsyncOperationHandle<IList<T>> handle = Addressables.LoadAssetsAsync<T>(
            assetLabel,
            null,
            Addressables.MergeMode.Union);

        await handle.Task;

        if (handle.Status != AsyncOperationStatus.Succeeded)
        {
            Debug.LogError($"Failed to load assets with label: '{assetLabel}'");
            if (handle.IsValid())
            {
                Addressables.Release(handle);
            }
            return default;
        }

        Debug.Log($"Loaded {handle.Result.Count} assets with label '{assetLabel}'.");
        return handle;
    }
}
