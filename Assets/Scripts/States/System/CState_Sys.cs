using UnityEngine;

public abstract class CState_Sys : CState
{
    public class STATE_SYS_DESC : CSTATEDESC
    {
        public STATE_SYS_DESC(int StateID, MonoBehaviour Owner, CGameInstance GameInstance) : base(StateID, Owner)
        {
            this.GameInstance = GameInstance;
        }
        public CGameInstance GameInstance{ get; private set; }
    }
    public CState_Sys(STATE_SYS_DESC refOwner) : base(refOwner) {
        GameInstance = refOwner.GameInstance;
    }
    public CGameInstance GameInstance { get; private set; } = null;
}