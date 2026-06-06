using UnityEngine;

public class CState_Sys_Playing : CState_Sys
{
    public class STATE_SYS_PLAYING_DESC : STATE_SYS_DESC
    {
        public STATE_SYS_PLAYING_DESC(int iStateID, MonoBehaviour pOwner, CFSM pFsm, CGameInstance pGameInstance) : base(iStateID, pOwner, pFsm, pGameInstance)
        {
        }
    }
    public CState_Sys_Playing(STATE_SYS_PLAYING_DESC pRefOwner) : base(pRefOwner) { }
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