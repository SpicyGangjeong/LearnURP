using System.Collections.Generic;
using UnityEngine;

public class LevelManager
{
    Dictionary<Defines.Enums.SceneID, SceneAddressable> m_vSceneAddressables = new Dictionary<Defines.Enums.SceneID, SceneAddressable>();
    public LevelManager(List<SceneReference> vSceneReferences){
        foreach (SceneReference pSceneReference in vSceneReferences)
        {
            m_vSceneAddressables.Add(pSceneReference.m_iSceneID, new SceneAddressable(pSceneReference.m_pSceneReference));
        }
    }
    Defines.Enums.SceneID m_eCurrentSceneID = Defines.Enums.SceneID.NONE;
    async public void ChangeScene(Defines.Enums.SceneID eSceneID)
    {
        if (m_vSceneAddressables.TryGetValue(eSceneID, out SceneAddressable pSceneAddressable))
        {
            if (m_eCurrentSceneID != Defines.Enums.SceneID.NONE)
            {
                await m_vSceneAddressables[m_eCurrentSceneID].UnloadScene();
            }
            await pSceneAddressable.LoadScene();
            m_eCurrentSceneID = eSceneID;
        } else {
            Debug.LogError($"Scene not found: {eSceneID}");
        }
    }
}
