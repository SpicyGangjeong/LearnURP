using Core;
using Core.StateMachine;
using UnityEngine;

namespace Logic
{
    namespace State
    {
        public class CState_Sys_Idle : CState_Sys
        {
            public class STATE_SYS_IDLE_DESC : STATE_SYS_DESC
            {
                public STATE_SYS_IDLE_DESC(int iStateID, MonoBehaviour pOwner, CFSM pFsm, CGameInstance pGameInstance) : base(iStateID, pOwner, pFsm, pGameInstance)
                {
                }
            }
            public CState_Sys_Idle(STATE_SYS_IDLE_DESC pRefOwner) : base(pRefOwner) { }
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