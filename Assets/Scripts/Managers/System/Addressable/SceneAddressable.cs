using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

namespace Core
{
    namespace Assets
    {
        public class SceneAddressable
        {
            public AssetReference m_pSceneRef;
            LoadSceneMode m_iLoadSceneMode = LoadSceneMode.Single;
            AsyncOperationHandle<SceneInstance> m_hSceneLoadHandle;

            public bool IsLoaded
            {
                get
                {
                    return m_hSceneLoadHandle.IsValid()
                        && AsyncOperationStatus.Succeeded == m_hSceneLoadHandle.Status
                        && m_hSceneLoadHandle.Result.Scene.isLoaded;
                }
            }

            public SceneAddressable(AssetReference pSceneRef)
            {
                m_pSceneRef = pSceneRef;
            }

            public async UniTask LoadScene()
            {
                if (false == m_pSceneRef.RuntimeKeyIsValid())
                {
                    Debug.LogError("Scene AssetReference RuntimeKey is invalid.");
                    return;
                }

                if (true == IsLoaded)
                {
                    return;
                }

                m_hSceneLoadHandle = Addressables.LoadSceneAsync(m_pSceneRef, m_iLoadSceneMode);
                await m_hSceneLoadHandle.ToUniTask();

                if (AsyncOperationStatus.Succeeded == m_hSceneLoadHandle.Status)
                {
                    Debug.Log($"Scene '{m_hSceneLoadHandle.Result.Scene.name}' loaded successfully.");
                }
                else
                {
                    Debug.LogError($"Failed to Load scene from AssetReference: {m_pSceneRef.AssetGUID}");
                    m_hSceneLoadHandle = default;
                }
            }

            // Single 로드로 이전 씬이 이미 교체된 뒤, stale handle만 버린다. UnloadSceneAsync 호출 금지.
            public void InvalidateHandle()
            {
                m_hSceneLoadHandle = default;
            }

            public async UniTask UnloadScene()
            {
                if (false == m_hSceneLoadHandle.IsValid())
                {
                    return;
                }

                // 마지막 남은 씬은 Unload 불가 — 새 씬을 Single로 로드해서 교체해야 함.
                if (1 >= SceneManager.sceneCount)
                {
                    Debug.LogError(
                        $"Cannot unload last remaining scene '{m_hSceneLoadHandle.Result.Scene.name}'. " +
                        "Load the next scene with LoadSceneMode.Single instead.");
                    return;
                }

                string strSceneName = m_hSceneLoadHandle.Result.Scene.name;
                AsyncOperationHandle<SceneInstance> hUnloadHandle = Addressables.UnloadSceneAsync(
                    m_hSceneLoadHandle,
                    UnloadSceneOptions.None,
                    false);
                await hUnloadHandle.ToUniTask();

                if (AsyncOperationStatus.Succeeded == hUnloadHandle.Status)
                {
                    Debug.Log($"Scene '{strSceneName}' unloaded successfully.");
                }
                else
                {
                    Debug.LogError($"Failed to Unload scene from AssetReference: {m_pSceneRef.AssetGUID}");
                }

                if (hUnloadHandle.IsValid())
                {
                    Addressables.Release(hUnloadHandle);
                }
                m_hSceneLoadHandle = default;
            }
        }
    }
}
