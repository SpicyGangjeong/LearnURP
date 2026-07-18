using Core;
using Core.StateMachine;
using System;
using UnityEngine;
using UnityEngine.InputSystem.XR;

namespace Logic
{
    namespace State
    {
        [Serializable]
        public class CState_Player_Idle : CState_Player
        {
            public class STATE_PLAYER_IDLE_DESC : STATE_PLAYER_DESC
            {
                public STATE_PLAYER_IDLE_DESC(MonoBehaviour pOwner, CFSM pFsm, IController controller)
                    : base((int)PlayerState.IDLE, pOwner, pFsm, controller)
                {

                }
            }
            public CState_Player_Idle(STATE_PLAYER_IDLE_DESC pRefOwner) : base(pRefOwner)
            {

            }

            public override void Enter()
            {
                Debug.Log("CState_Player_Idle Enter");
            }

            public override void Fixed_Update_State()
            {

            }
            async public override void Update_State()
            {
            }

            public override void Late_Update_State()
            {

            }

            public override void Exit()
            {
            }
        }
    }
}
