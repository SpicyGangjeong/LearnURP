using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.Serialization;

// Inspector에서 할당한 AssetReference를 사용하여
// 프리팹을 로드하고 인스턴스화하는 예제입니다.
public class LoadFromAssetReference : MonoBehaviour
{
    public AssetReferenceGameObject m_pPlayerPrefabRef;
    AsyncOperationHandle<GameObject> m_hLoadHandle;

    async void Start()
    {
        if (null == m_pPlayerPrefabRef || false == m_pPlayerPrefabRef.RuntimeKeyIsValid())
        {
            Debug.LogError("AssetReference is Not set or Invalid.");
            return;
        }

        m_hLoadHandle = m_pPlayerPrefabRef.LoadAssetAsync<GameObject>();
        await m_hLoadHandle.Task;
        if (m_hLoadHandle.Status == AsyncOperationStatus.Succeeded)
        {
            Instantiate(m_hLoadHandle.Result);
            Debug.Log("Loaded and instantiated via AssetReference.");
        } else
        {
            Debug.LogError($"Failed to Load Prefab from AssetReference: {m_pPlayerPrefabRef.AssetGUID}");
        }
    }

    private void OnDestroy()
    {
        if (m_hLoadHandle.IsValid())
        {
            m_pPlayerPrefabRef.ReleaseAsset();
        }
    }
}
