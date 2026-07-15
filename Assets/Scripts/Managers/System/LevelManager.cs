using Core.Assets;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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
            Task m_pChangeSceneTask = Task.CompletedTask;

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
                if (false == m_pChangeSceneTask.IsCompleted)
                {
                    await m_pChangeSceneTask;
                }

                if (m_ePendingSceneID != eSceneID)
                {
                    return;
                }

                m_pChangeSceneTask = ChangeSceneAsync(eSceneID);
                await m_pChangeSceneTask;
            }

            async Task ChangeSceneAsync(Defines.Enums.SceneID eSceneID)
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

                // Single 로드가 현재 씬을 교체한다. 마지막 씬 Unload는 Unity가 막으므로 하지 않는다.
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
