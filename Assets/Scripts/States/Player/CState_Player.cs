using Core;
using Core.StateMachine;
using System;
using UnityEngine;
namespace Logic
{
    namespace State
    {
        [Serializable]
        public abstract class CState_Player : CState
        {
            public enum PlayerState : int
            {
                NONE = -1,
                INITIALIZE = 0,
                IDLE = 1,
                END,
            }
            public class STATE_PLAYER_DESC : CSTATEDESC
            {
                public STATE_PLAYER_DESC(int iStateID, MonoBehaviour pOwner, CFSM pFsm, CGameInstance pGameInstance) : base(iStateID, pOwner, pFsm)
                {

                }

            }
            public CState_Player(STATE_PLAYER_DESC pRefOwner) : base(pRefOwner)
            {

            }

        }
    }
}