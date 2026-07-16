using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Core
{
    namespace Assets
    {
        public class ContentUpdater : MonoBehaviour
        {
            async void Start()
            {
                await Addressables.InitializeAsync().Task;

                AsyncOperationHandle<List<string>> hCheckHandle = Addressables.CheckForCatalogUpdates(false);
                await hCheckHandle.Task;

                if (hCheckHandle.Status == AsyncOperationStatus.Succeeded)
                {
                    List<string> vCatalogsToUpdate = hCheckHandle.Result;
                    if (vCatalogsToUpdate != null && vCatalogsToUpdate.Count > 0)
                    {
                        Debug.Log($"Catalog updates found: {vCatalogsToUpdate.Count} catalogs need updating.");

                        AsyncOperationHandle<List<UnityEngine.AddressableAssets.ResourceLocators
                            .IResourceLocator>> hUpdateHandle = Addressables.UpdateCatalogs(vCatalogsToUpdate, false);
                        await hUpdateHandle.Task;

                        if (hUpdateHandle.Status == AsyncOperationStatus.Succeeded)
                        {
                            List<UnityEngine.AddressableAssets.ResourceLocators
                                .IResourceLocator> vUpdatedLocators = hUpdateHandle.Result;
                            Debug.Log($"Catalogs updated successfully: {vUpdatedLocators.Count} catalogs updated.");
                        }
                        else
                        {
                            Debug.LogError("Failed to update catalogs.");
                        }

                    }
                    else
                    {
                        Debug.Log("no updates available.");
                    }
                    Addressables.Release(hCheckHandle);
                }
            }
        }
    }
}