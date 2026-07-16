using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Core
{
    namespace Assets
    {
        public class InstantiateAddressable : MonoBehaviour
        {
            public AssetReferenceGameObject m_pEnemyPrefabRef;
            AsyncOperationHandle<GameObject> m_hInstantiateHandle;
            GameObject m_pSpawnedEnemy;

            async void Start()
            {
                if (false == m_pEnemyPrefabRef.RuntimeKeyIsValid())
                {
                    return;
                }

                m_hInstantiateHandle = m_pEnemyPrefabRef.InstantiateAsync(Vector3.zero, Quaternion.identity);

                m_pSpawnedEnemy = await m_hInstantiateHandle.Task;

                if (m_hInstantiateHandle.Status == AsyncOperationStatus.Succeeded
                    && m_pSpawnedEnemy != null)
                {
                    Debug.Log("Enemy prefab instantiated successfully.");
                }
                else
                {
                    Debug.LogError($"Failed to Instantiate prefab from AssetReference: {m_pEnemyPrefabRef.AssetGUID}");
                }
            }

            void OnDestroy()
            {
                if (m_hInstantiateHandle.IsValid())
                {
                    m_pEnemyPrefabRef.ReleaseInstance(m_pSpawnedEnemy);
                }
            }
        }
    }
}