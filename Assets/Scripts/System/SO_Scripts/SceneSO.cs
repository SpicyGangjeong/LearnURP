using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace SO
{
    [Serializable]
    public class SceneReference
    {
        public Defines.Enums.SceneID m_iSceneID = Defines.Enums.SceneID.NONE;
        public AssetReference m_pSceneReference = null;
    }

    [CreateAssetMenu(fileName = "SceneSO", menuName = "Scriptable Objects/SceneSO")]
    public class SceneSO : ScriptableObject, ICloneable
    {
        public List<SceneReference> m_vSceneReferences = new List<SceneReference>();


        public object Clone()
        {
            SceneSO pCloneData = ScriptableObject.CreateInstance<SceneSO>();
            pCloneData.CopyFrom(this);
            return pCloneData;
        }
        private void CopyFrom(SceneSO pOriginal)
        {
            foreach (SceneReference r in pOriginal.m_vSceneReferences)
            {
                m_vSceneReferences.Add(r);
            }
        }
        private SceneSO() { }
        private SceneSO(SceneSO pOther) { }
    }
}
