using System.Collections.Generic;
using UnityEngine;

public class LevelManager
{
    Dictionary<DEFINES.SceneID, SceneAddressable> sceneAddressables = new Dictionary<DEFINES.SceneID, SceneAddressable>();
    public LevelManager(List<SceneReference> sceneReferences){
        foreach (SceneReference sceneReference in sceneReferences)
        {
            sceneAddressables.Add(sceneReference.sceneID, new SceneAddressable(sceneReference.sceneReference));
        }
    }
    private DEFINES.SceneID iCurrentSceneID = DEFINES.SceneID.NONE;
    async public void ChangeScene(DEFINES.SceneID sceneID)
    {
        if (sceneAddressables.TryGetValue(sceneID, out SceneAddressable sceneAddressable))
        {
            if (iCurrentSceneID != DEFINES.SceneID.NONE)
            {
                await sceneAddressables[iCurrentSceneID].UnloadScene();
            }
            await sceneAddressable.LoadScene();
            iCurrentSceneID = sceneID;
        } else {
            Debug.LogError($"Scene not found: {sceneID}");
        }
    }
}
