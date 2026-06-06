using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Serialization;

[Serializable]
public class SceneReference
{
    [FormerlySerializedAs("sceneID")]
    public DEFINES.SceneID m_iSceneID = DEFINES.SceneID.NONE;
    [FormerlySerializedAs("sceneReference")]
    public AssetReference m_pSceneReference = null;
}

[CreateAssetMenu(fileName = "SceneSO", menuName = "Scriptable Objects/SceneSO")]
public class SceneSO : ScriptableObject
{
    [FormerlySerializedAs("sceneReferences")]
    public List<SceneReference> m_vSceneReferences = new List<SceneReference>();
}
