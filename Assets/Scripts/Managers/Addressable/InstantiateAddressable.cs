using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

// AssetReference를 사용하여 프리팹을 인스턴스화하는 예제입니다.
public class InstantiateAddressable : MonoBehaviour
{
    public AssetReferenceGameObject enemyPrefabRef;
    private AsyncOperationHandle<GameObject> instantiateHandle;
    private GameObject spawnedEnemy;

    async void Start()
    {
        if (false == enemyPrefabRef.RuntimeKeyIsValid())
        {
            return;
        }

        instantiateHandle = enemyPrefabRef.InstantiateAsync(Vector3.zero, Quaternion.identity);

        spawnedEnemy = await instantiateHandle.Task;

        if (instantiateHandle.Status == AsyncOperationStatus.Succeeded 
            && spawnedEnemy != null)
        {
            Debug.Log("Enemy prefab instantiated successfully.");
        } else
        {
            Debug.LogError($"Failed to Instantiate prefab from AssetReference: {enemyPrefabRef.AssetGUID}");
        }
    }

    void OnDestroy()
    {
        if (instantiateHandle.IsValid())
        {
            enemyPrefabRef.ReleaseInstance(spawnedEnemy);
        }
    }
}
