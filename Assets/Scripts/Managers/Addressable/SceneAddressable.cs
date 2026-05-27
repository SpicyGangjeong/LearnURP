using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;

public class SceneAddressable
{
    public AssetReference sceneRef;
    [SerializeField] LoadSceneMode loadSceneMode = LoadSceneMode.Single;
    private AsyncOperationHandle<SceneInstance> sceneLoadHandle;

    public SceneAddressable(AssetReference sceneRef)
    {
        this.sceneRef = sceneRef;
    }
    public async Task LoadScene()
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
        } 
        else
        {
            Debug.LogError($"Failed to Load scene from AssetReference: {sceneRef.AssetGUID}");
        }
    }

    public async Task UnloadScene()
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

    void ChangeScene()
    {
        if (sceneLoadHandle.IsValid() && sceneLoadHandle.Status == AsyncOperationStatus.Succeeded)
        {
            Addressables.UnloadSceneAsync(sceneLoadHandle);
        }
    }
}