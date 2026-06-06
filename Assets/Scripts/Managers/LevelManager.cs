using System.Collections.Generic;
using UnityEngine;

public class LevelManager
{
    Dictionary<DEFINES.SceneID, SceneAddressable> m_vSceneAddressables = new Dictionary<DEFINES.SceneID, SceneAddressable>();
    public LevelManager(List<SceneReference> vSceneReferences){
        foreach (SceneReference pSceneReference in vSceneReferences)
        {
            m_vSceneAddressables.Add(pSceneReference.m_iSceneID, new SceneAddressable(pSceneReference.m_pSceneReference));
        }
    }
    DEFINES.SceneID m_iCurrentSceneID = DEFINES.SceneID.NONE;
    async public void ChangeScene(DEFINES.SceneID iSceneID)
    {
        if (m_vSceneAddressables.TryGetValue(iSceneID, out SceneAddressable pSceneAddressable))
        {
            if (m_iCurrentSceneID != DEFINES.SceneID.NONE)
            {
                await m_vSceneAddressables[m_iCurrentSceneID].UnloadScene();
            }
            await pSceneAddressable.LoadScene();
            m_iCurrentSceneID = iSceneID;
        } else {
            Debug.LogError($"Scene not found: {iSceneID}");
        }
    }
}
