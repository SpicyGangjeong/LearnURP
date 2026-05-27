using UnityEngine;

public class CState_Sys_Initialize : CState_Sys
{
    public class STATE_SYS_INITIALIZE_DESC : STATE_SYS_DESC
    {
        public STATE_SYS_INITIALIZE_DESC(int StateID, MonoBehaviour Owner, CGameInstance GameInstance) : base(StateID, Owner, GameInstance)
        {
        }
    }
    public CState_Sys_Initialize(STATE_SYS_INITIALIZE_DESC refOwner) : base(refOwner)
    {

    }   

    public override void Enter()
    {
        Debug.Log("CState_Sys_Initialize Enter");
    }

    public override void Fixed_Update_State()
    {
        
    }

    public override void Update_State()
    {
        
    }

    public override void Late_Update_State()
    {
        
    }

    public override void Exit()
    {
        Debug.Log("CState_Sys_Initialize Exit");
    }
}