using Core;
using Core.StateMachine;
using System;
using UnityEngine;
namespace Logic
{
    namespace State
    {
        [Serializable]
        public abstract class CState_Player : CState_Unit
        {
            public enum PlayerState : int
            {
                NONE = -1,
                INITIALIZE = 0,
                IDLE = 1,
                END,
            }
            public class STATE_PLAYER_DESC : STATE_UNIT_DESC
            {
                public STATE_PLAYER_DESC(int iStateID, MonoBehaviour pOwner, CFSM pFsm, IController controller) : base(iStateID, pOwner, pFsm, controller)
                {

                }

            }
            public CState_Player(STATE_PLAYER_DESC pRefOwner) : base(pRefOwner)
            {

            }

        }
    }
}