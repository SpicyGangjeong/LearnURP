using Core;
using Core.StateMachine;
using UnityEngine;

namespace Logic
{
    namespace State
    {
        public abstract class CState_System : CState
        {
            public enum SystemState : int
            {
                NONE = -1,
                INITIALIZE = 0,
                IDLE = 1,
                PLAYING = 2,
                END = 3,
            }
            public class STATE_SYSTEM_DESC : CSTATEDESC
            {
                public STATE_SYSTEM_DESC(int iStateID, MonoBehaviour pOwner, CFSM pFsm, CGameInstance pGameInstance) : base(iStateID, pOwner, pFsm)
                {
                    this.GameInstance = pGameInstance;
                }
                public CGameInstance GameInstance { get; private set; }
            }
            public CState_System(STATE_SYSTEM_DESC pRefOwner) : base(pRefOwner)
            {
                GameInstance = pRefOwner.GameInstance;
            }
            public CGameInstance GameInstance { get; private set; } = null;
        }
    }
}