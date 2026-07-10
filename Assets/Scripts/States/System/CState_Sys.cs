using Core;
using Core.StateMachine;
using UnityEngine;

namespace Logic
{
    namespace State
    {
        public abstract class CState_Sys : CState
        {
            public enum SystemState : int
            {
                NONE = -1,
                INITIALIZE = 0,
                IDLE = 1,
                PLAYING = 2,
                END = 3,
            }
            public class STATE_SYS_DESC : CSTATEDESC
            {
                public STATE_SYS_DESC(int iStateID, MonoBehaviour pOwner, CFSM pFsm, CGameInstance pGameInstance) : base(iStateID, pOwner, pFsm)
                {
                    this.GameInstance = pGameInstance;
                }
                public CGameInstance GameInstance { get; private set; }
            }
            public CState_Sys(STATE_SYS_DESC pRefOwner) : base(pRefOwner)
            {
                GameInstance = pRefOwner.GameInstance;
            }
            public CGameInstance GameInstance { get; private set; } = null;
        }
    }
}