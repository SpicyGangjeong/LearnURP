using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

// Addressable Asset System을 사용하여 씬을 로드하고 언로드하는 예제입니다.
public class LoadSceneAddressable: MonoBehaviour
{
    public AssetReference sceneRef;
    [SerializeField] LoadSceneMode loadSceneMode = LoadSceneMode.Single;
    private AsyncOperationHandle<SceneInstance> sceneLoadHandle;

    public async void LoadScene()
    {
        if (false == sceneRef.RuntimeKeyIsValid())
        {
            return;
        }
        sceneLoadHandle = Addressables.LoadSceneAsync(sceneRef, loadSceneMode);
        await sceneLoadHandle.Task;

        if (sceneLoadHandle.Status == AsyncOperationStatus.Succeeded)
        {
            Debug.Log($"Scene '{sceneRef.editorAsset.name}' loaded successfully.");
        } else
        {
            Debug.LogError($"Failed to Load scene from AssetReference: {sceneRef.AssetGUID}");
        }
    }

    public async void UnloadScene()
    {
        if (sceneLoadHandle.IsValid())
        {
            var unloadHandle = Addressables.UnloadSceneAsync(sceneLoadHandle);
            await unloadHandle.Task;

            if (unloadHandle.Status == AsyncOperationStatus.Succeeded)
            {
                Debug.Log($"Scene '{sceneRef.editorAsset.name}' unloaded successfully.");
            }
            else
            {
                Debug.LogError($"Failed to Unload scene from AssetReference: {sceneRef.AssetGUID}");
            }
        }
    }

    void OnDestroy()
    {
        if (sceneLoadHandle.IsValid() && sceneLoadHandle.Status == AsyncOperationStatus.Succeeded)
        {
            Addressables.UnloadSceneAsync(sceneLoadHandle);
        }
    }
}