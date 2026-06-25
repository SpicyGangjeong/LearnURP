using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Serialization;

[Serializable]
public class SceneReference
{
    public DEFINES.ENUMS.SceneID m_iSceneID = DEFINES.ENUMS.SceneID.NONE;
    public AssetReference m_pSceneReference = null;
}

[CreateAssetMenu(fileName = "SceneSO", menuName = "Scriptable Objects/SceneSO")]
public class SceneSO : ScriptableObject
{
    public List<SceneReference> m_vSceneReferences = new List<SceneReference>();
}
