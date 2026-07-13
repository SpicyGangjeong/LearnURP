using Core;
using Core.StateMachine;
using UnityEngine;

namespace Logic
{
    namespace State
    {
        public class CState_System_Playing : CState_System
        {
            public class STATE_SYSTEM_PLAYING_DESC : STATE_SYSTEM_DESC
            {
                public STATE_SYSTEM_PLAYING_DESC(int iStateID, MonoBehaviour pOwner, CFSM pFsm, CGameInstance pGameInstance) : base(iStateID, pOwner, pFsm, pGameInstance)
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

            public override void Exit()
            {

            }

            public override void Update_State()
            {

            }

            public override void Enter()
            {

            }
        }
    }
}
