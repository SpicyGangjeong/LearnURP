using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

public class CState_Sys_Initialize : CState_Sys
{
    public delegate Task BootstrapAsyncDelegate();
    public class STATE_SYS_INITIALIZE_DESC : STATE_SYS_DESC
    {
        public STATE_SYS_INITIALIZE_DESC(int StateID, MonoBehaviour Owner, CFSM FSM, CGameInstance GameInstance, 
        BootstrapAsyncDelegate delegateBootstrapAsync) : base(StateID, Owner, FSM, GameInstance)
        {
            this.delegateBootstrapAsync = delegateBootstrapAsync;
        }
        public BootstrapAsyncDelegate delegateBootstrapAsync { get; private set; } = null;
    }
    public CState_Sys_Initialize(STATE_SYS_INITIALIZE_DESC refOwner) : base(refOwner)
    {
        delegateBootstrapAsync = refOwner.delegateBootstrapAsync;
    }
    private BootstrapAsyncDelegate delegateBootstrapAsync = null;
    private bool isLoading = false;


    public override void Enter()
    {
        Debug.Log("CState_Sys_Initialize Enter");
    }

    public override void Fixed_Update_State()
    {
        
    }
    async public override void Update_State()
    {
        if (null != delegateBootstrapAsync)
        {
            if (false == isLoading)
            {
                isLoading = true;
                await delegateBootstrapAsync.Invoke();
                isLoading = false;
                delegateBootstrapAsync = null;
                FSM.Change_State((int)DEFINES.SystemState.IDLE);
            }
        }
    }


    public override void Late_Update_State()
    {
        
    }

    public override void Exit()
    {
        Debug.Log("CState_Sys_Initialize Exit");
        GameInstance.ChangeScene(DEFINES.SceneID.MAIN_MENU);
    }
}