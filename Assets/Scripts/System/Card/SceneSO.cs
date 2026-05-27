using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

[Serializable]
public class SceneReference
{
    public DEFINES.SceneID sceneID = DEFINES.SceneID.NONE;
    public AssetReference sceneReference = null;
}

[CreateAssetMenu(fileName = "SceneSO", menuName = "Scriptable Objects/SceneSO")]
public class SceneSO : ScriptableObject
{
    public List<SceneReference> sceneReferences = new List<SceneReference>();
}
