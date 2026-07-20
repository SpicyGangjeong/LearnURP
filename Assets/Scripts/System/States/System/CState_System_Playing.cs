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
            public override void Enter()
            {
                JobDeferredCallback callback = new JobDeferredCallback(
                    async () => { 
                        await GameInstance.ChangeScene(Defines.Enums.SceneID.GAME_PLAY);
                        GamePlayCanvas pPlayCanvas = GameInstance.LoadInstance<GamePlayCanvas>(Defines.Constants.s_strGamePlayCanvas);
                        pPlayCanvas.gameObject.name = Defines.Constants.s_strGamePlayCanvas;
                    }, 
                    "Changing_Scene" );
                GameInstance.EnqueueJob(callback);
            }
            public override void Update_State()
            {

            }
            public override void Exit()
            {
                JobDeferredCallback callback = new JobDeferredCallback(
                    () => GameInstance.ChangeScene(Defines.Enums.SceneID.MAIN_MENU), "Changing_Scene"
                    );
                GameInstance.EnqueueJob(callback);
            }
        }
    }
}
