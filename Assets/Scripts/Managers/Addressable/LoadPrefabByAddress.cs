using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.Serialization;

// 단일 에셋을 주소로 로드합니다.
// 주소는 Addressable Asset System에서 설정한 주소와 일치해야 합니다.
public class LoadPrefabByAddress : MonoBehaviour
{
    [FormerlySerializedAs("prefabAddress")]
    public string m_strPrefabAddress = "Assets/Prefabs/MyPrefab.prefab";
    AsyncOperationHandle<GameObject> m_hLoadHandle;

    async void Start()
    {
        m_hLoadHandle = Addressables.LoadAssetAsync<GameObject>(m_strPrefabAddress);
        await m_hLoadHandle.Task;

        if (m_hLoadHandle.Status == AsyncOperationStatus.Succeeded)
        {
            GameObject pPrefab = m_hLoadHandle.Result;
            Instantiate(pPrefab, Vector3.zero, Quaternion.identity);
            Debug.Log("Prefab loaded and instantiated successfully.");
        }
        else
        {
            Debug.LogError($"Failed to Load prefab at address: {m_strPrefabAddress}");
        }
    }
    void OnDestroy()
    {
        if (m_hLoadHandle.IsValid())
        {
            Addressables.Release(m_hLoadHandle);
        }
    }
}
