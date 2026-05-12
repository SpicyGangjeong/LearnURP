using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

// Inspector에서 할당한 AssetReference를 사용하여
// 프리팹을 로드하고 인스턴스화하는 예제입니다.
public class LoadFromAssetReference : MonoBehaviour
{
    public AssetReferenceGameObject playerPrefabRef;
    private AsyncOperationHandle<GameObject> loadHandle;

    async void Start()
    {
        if (null == playerPrefabRef || false == playerPrefabRef.RuntimeKeyIsValid())
        {
            Debug.LogError("AssetReference is Not set or Invalid.");
            return;
        }

        loadHandle = playerPrefabRef.LoadAssetAsync<GameObject>();
        await loadHandle.Task;
        if (loadHandle.Status == AsyncOperationStatus.Succeeded)
        {
            Instantiate(loadHandle.Result);
            Debug.Log("Loaded and instantiated via AssetReference.");
        } else
        {
            Debug.LogError($"Failed to Load Prefab from AssetReference: {playerPrefabRef.AssetGUID}");
        }
    }

    private void OnDestroy()
    {
        if (loadHandle.IsValid())
        {
            playerPrefabRef.ReleaseAsset();
        }
    }
}