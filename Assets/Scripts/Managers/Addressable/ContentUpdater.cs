using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Collections.Generic;

// Addressable Asset System을 사용하여 카탈로그 업데이트를 확인하고 필요한 경우 업데이트하는 예제입니다.
public class ContentUpdater : MonoBehaviour
{
    async void Start()
    {
        await Addressables.InitializeAsync().Task;

        AsyncOperationHandle<List<string>> checkHandle = Addressables.CheckForCatalogUpdates(false);
        await checkHandle.Task;

        if (checkHandle.Status == AsyncOperationStatus.Succeeded)
        {
            List<string> catalogsToUpdate = checkHandle.Result;
            if (catalogsToUpdate != null && catalogsToUpdate.Count > 0)
            {
                Debug.Log($"Catalog updates found: {catalogsToUpdate.Count} catalogs need updating.");

                AsyncOperationHandle<List<UnityEngine.AddressableAssets.ResourceLocators
                    .IResourceLocator>> updateHandle = Addressables.UpdateCatalogs(catalogsToUpdate, false);
                await updateHandle.Task;

                if (updateHandle.Status == AsyncOperationStatus.Succeeded)
                {
                    List<UnityEngine.AddressableAssets.ResourceLocators
                        .IResourceLocator> updatedLocators = updateHandle.Result;
                    Debug.Log($"Catalogs updated successfully: {updatedLocators.Count} catalogs updated.");
                } else
                {
                    Debug.LogError("Failed to update catalogs.");
                }

            } else
            {
                Debug.Log("no updates available.");
            }
            Addressables.Release(checkHandle);
        }
    }
}