using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

// 단일 에셋을 주소로 로드합니다.
// 주소는 Addressable Asset System에서 설정한 주소와 일치해야 합니다.
public class LoadPrefabByAddress : MonoBehaviour
{
    public string prefabAddress = "Assets/Prefabs/MyPrefab.prefab";

    private AsyncOperationHandle<GameObject> loadHandle;

    async void Start()
    {
        loadHandle = Addressables.LoadAssetAsync<GameObject>(prefabAddress);
        await loadHandle.Task;

        if (loadHandle.Status == AsyncOperationStatus.Succeeded)
        {
            GameObject prefab = loadHandle.Result;
            Instantiate(prefab, Vector3.zero, Quaternion.identity);
            Debug.Log("Prefab loaded and instantiated successfully.");
        }
        else
        {
            Debug.LogError($"Failed to Load prefab at address: {prefabAddress}");
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