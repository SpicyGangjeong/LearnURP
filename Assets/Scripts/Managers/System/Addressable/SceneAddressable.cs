using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;

namespace Core
{
    namespace Assets
    {
        public class SceneAddressable
        {
            public AssetReference m_pSceneRef;
            [SerializeField] LoadSceneMode m_iLoadSceneMode = LoadSceneMode.Single;
            AsyncOperationHandle<SceneInstance> m_hSceneLoadHandle;

            public SceneAddressable(AssetReference pSceneRef)
            {
                m_pSceneRef = pSceneRef;
            }
            public async Task LoadScene()
            {
                if (false == m_pSceneRef.RuntimeKeyIsValid())
                {
                    return;
                }
                m_hSceneLoadHandle = Addressables.LoadSceneAsync(m_pSceneRef, m_iLoadSceneMode);
                await m_hSceneLoadHandle.Task;

                if (m_hSceneLoadHandle.Status == AsyncOperationStatus.Succeeded)
                {
                    Debug.Log($"Scene '{m_hSceneLoadHandle.Result.Scene.name}' loaded successfully.");
                }
                else
                {
                    Debug.LogError($"Failed to Load scene from AssetReference: {m_pSceneRef.AssetGUID}");
                }
            }

            public async Task UnloadScene()
            {
                if (m_hSceneLoadHandle.IsValid())
                {
                    string strSceneName = m_hSceneLoadHandle.Result.Scene.name;
                    AsyncOperationHandle<SceneInstance> hUnloadHandle = Addressables.UnloadSceneAsync(m_hSceneLoadHandle);
                    await hUnloadHandle.Task;

                    if (hUnloadHandle.Status == AsyncOperationStatus.Succeeded)
                    {
                        Debug.Log($"Scene '{strSceneName}' unloaded successfully.");
                    }
                    else
                    {
                        Debug.LogError($"Failed to Unload scene from AssetReference: {m_pSceneRef.AssetGUID}");
                    }
                }
            }

            void ChangeScene()
            {
                if (m_hSceneLoadHandle.IsValid() && m_hSceneLoadHandle.Status == AsyncOperationStatus.Succeeded)
                {
                    Addressables.UnloadSceneAsync(m_hSceneLoadHandle);
                }
            }
        }
    }
}
