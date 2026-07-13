using Core.StateMachine;
using UnityEngine;

using Core;
namespace Logic
{
    namespace State
    {
        public abstract class CState_Player : CState
        {
            public enum PlayerState : int
            {
                NONE = -1,
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