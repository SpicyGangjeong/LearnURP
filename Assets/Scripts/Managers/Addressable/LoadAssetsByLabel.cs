using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Collections.Generic;

// 특정 라벨이 붙은 에셋들을 모두 로드합니다.
public class LoadAssetsByLabel : MonoBehaviour
{
    public string assetLabel = "UI_Icons";
    private AsyncOperationHandle<IList<Texture2D>> loadHandle;

    async void Start()
    {
        loadHandle = Addressables.LoadAssetAsync<IList<Texture2D>>(assetLabel);
        await loadHandle.Task;
        if (loadHandle.Status == AsyncOperationStatus.Succeeded)
        {
            IList<Texture2D> icons = loadHandle.Result;
            Debug.Log($"Loaded {icons.Count} icons with label : '{assetLabel}'.");
        } else
        {
            Debug.LogError($"Failed to Load assets with label: '{assetLabel}'");
        }
    }
    void OnDestroy()
    {
        if (loadHandle.IsValid())
        {
            Addressables.Release(loadHandle);
        }
    }
}