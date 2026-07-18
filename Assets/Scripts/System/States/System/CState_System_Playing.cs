using Core;
using Core.Job;
using Core.StateMachine;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using View.UI;

namespace Logic
{
    namespace State
    {
        [Serializable]
        public class CState_System_Playing : CState_System
        {
            public class STATE_SYSTEM_PLAYING_DESC : STATE_SYSTEM_DESC
            {
                public STATE_SYSTEM_PLAYING_DESC(MonoBehaviour pOwner, CFSM pFsm, CGameInstance pGameInstance)
                    : base((int)SystemState.PLAYING, pOwner, pFsm, pGameInstance)
                {
                }
            }
            public CState_System_Playing(STATE_SYSTEM_PLAYING_DESC pRefOwner) : base(pRefOwner) { }
            public override void Fixed_Update_State()
            {

            }

            public override void Late_Update_State()
            {

            }
            EventSystem m_pEventSystem = null;
            GamePlayCanvas m_pGameCanvas = null;
            public override void Enter()
            {
                if (null == m_pEventSystem)
                {
                    m_pEventSystem = EventSystem.current;
                }
                if (null == m_pGameCanvas)
                {
                    m_pGameCanvas = GamePlayCanvas.Instance;
                    if (null == m_pGameCanvas)
                    {
                        m_pGameCanvas = GameInstance.LoadInstance<GamePlayCanvas>(Defines.Constants.s_strGamePlayCanvas);
                        m_pGameCanvas.gameObject.name = Defines.Constants.s_strGamePlayCanvas;
                    }
                    if (null == m_pGameCanvas)
                    {
                        m_pGameCanvas = GamePlayCanvas.Instance;
                        Debug.LogError("GameCanvas Load Failure");
                    }
                }
                m_pGameCanvas.gameObject.SetActive(true);
                JobDeferredCallback callback = new JobDeferredCallback(
                    () => GameInstance.ChangeScene(Defines.Enums.SceneID.GAME_PLAY), "Changing_Scene"
                    );
                GameInstance.EnqueueJob(callback);
            }
            public override void Update_State()
            {

            }
            public override void Exit()
            {
                m_pGameCanvas.gameObject.SetActive(false);
                JobDeferredCallback callback = new JobDeferredCallback(
                    () => GameInstance.ChangeScene(Defines.Enums.SceneID.MAIN_MENU), "Changing_Scene"
                    );
                GameInstance.EnqueueJob(callback);
            }
        }
    }
}
