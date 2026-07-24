using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.AddressableAssets.ResourceLocators;

namespace Core
{
    namespace Assets
    {
        public class ContentUpdater : MonoBehaviour
        {
            void Start()
            {
                StartAsync().Forget();
            }
            async UniTaskVoid StartAsync()
            {
                await Addressables.InitializeAsync().ToUniTask();

                AsyncOperationHandle<List<string>> hCheckHandle = Addressables.CheckForCatalogUpdates(false);
                await hCheckHandle.ToUniTask();
                if (AsyncOperationStatus.Succeeded == hCheckHandle.Status)
                {
                    List<string> vCatalogsToUpdate = hCheckHandle.Result;
                    if (null != vCatalogsToUpdate && 0 < vCatalogsToUpdate.Count)
                    {
                        Debug.Log($"ContentUpdater: Catalog updates found: {vCatalogsToUpdate.Count} catalogs need updating.");

                        AsyncOperationHandle<List<IResourceLocator>> hUpdateHandle = Addressables.UpdateCatalogs(vCatalogsToUpdate, false);
                        await hUpdateHandle.ToUniTask();

                        if (AsyncOperationStatus.Succeeded == hUpdateHandle.Status)
                        {
                            List<IResourceLocator> vUpdatedLocators = hUpdateHandle.Result;
                            Debug.Log($"ContentUpdater: Catalogs updated successfully: {vUpdatedLocators.Count} catalogs updated.");
                        }
                        else
                        {
                            Debug.LogError("ContentUpdater: Failed to update catalogs.");
                        }
                    }
                    else
                    {
                        Debug.Log("ContentUpdater: No updates available.");
                    }
                    Addressables.Release(hCheckHandle);
                }

            }
        }
    }
}