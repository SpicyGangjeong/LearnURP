using Core;
using Core.StateMachine;
using UnityEngine;

namespace Logic
{
    namespace State
    {
        public class CState_System_Idle : CState_System
        {
            public class STATE_SYSTEM_IDLE_DESC : STATE_SYSTEM_DESC
            {
                public STATE_SYSTEM_IDLE_DESC(int iStateID, MonoBehaviour pOwner, CFSM pFsm, CGameInstance pGameInstance) : base(iStateID, pOwner, pFsm, pGameInstance)
                {
                }
            }
            public CState_System_Idle(STATE_SYSTEM_IDLE_DESC pRefOwner) : base(pRefOwner) { }
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