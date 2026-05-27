using UnityEngine;

public class CState_Sys_Idle : CState_Sys
{
    public class STATE_SYS_IDLE_DESC : STATE_SYS_DESC
    {
        public STATE_SYS_IDLE_DESC(int StateID, MonoBehaviour Owner, CFSM FSM, CGameInstance GameInstance) : base(StateID, Owner, FSM, GameInstance)
        {
        }
    }
    public CState_Sys_Idle(STATE_SYS_IDLE_DESC refOwner) : base(refOwner) { }
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