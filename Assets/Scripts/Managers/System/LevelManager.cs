using Core.Assets;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

namespace Core
{
    namespace Scene
    {
        public class LevelManager
        {
            Dictionary<Defines.Enums.SceneID, SceneAddressable> m_vSceneAddressables = new Dictionary<Defines.Enums.SceneID, SceneAddressable>();
            Defines.Enums.SceneID m_eCurrentSceneID = Defines.Enums.SceneID.NONE;
            Defines.Enums.SceneID m_ePendingSceneID = Defines.Enums.SceneID.NONE;
            bool m_bChanging = false;

            public LevelManager(List<SO.SceneReference> vSceneReferences)
            {
                foreach (SO.SceneReference pSceneReference in vSceneReferences)
                {
                    m_vSceneAddressables.Add(pSceneReference.m_iSceneID, new SceneAddressable(pSceneReference.m_pSceneReference));
                }
            }

            public async UniTask ChangeScene(Defines.Enums.SceneID eSceneID)
            {
                m_ePendingSceneID = eSceneID;
                while (true == m_bChanging)
                {
                    await UniTask.Yield();
                }

                if (m_ePendingSceneID != eSceneID)
                {
                    return;
                }
                m_bChanging = true;
                try
                {
                    await ChangeSceneAsync(eSceneID);
                }
                finally
                {
                    m_bChanging = false;
                }
            }
            async UniTask ChangeSceneAsync(Defines.Enums.SceneID eSceneID)
            {
                if (false == m_vSceneAddressables.TryGetValue(eSceneID, out SceneAddressable pSceneAddressable))
                {
                    Debug.LogError($"Scene not found: {eSceneID}");
                    return;
                }

                if (m_eCurrentSceneID == eSceneID)
                {
                    return;
                }

                Defines.Enums.SceneID ePreviousSceneID = m_eCurrentSceneID;

                await pSceneAddressable.LoadScene();

                if (Defines.Enums.SceneID.NONE != ePreviousSceneID
                    && true == m_vSceneAddressables.TryGetValue(ePreviousSceneID, out SceneAddressable pPreviousScene))
                {
                    pPreviousScene.InvalidateHandle();
                }

                m_eCurrentSceneID = eSceneID;
            }
        }
    }
}
