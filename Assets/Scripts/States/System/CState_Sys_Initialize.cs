using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

public class CState_Sys_Initialize : CState_Sys
{
    public delegate Task BootstrapAsyncDelegate();
    public class STATE_SYS_INITIALIZE_DESC : STATE_SYS_DESC
    {
        public STATE_SYS_INITIALIZE_DESC(int iStateID, MonoBehaviour pOwner, CFSM pFsm, CGameInstance pGameInstance, 
        BootstrapAsyncDelegate pDelegateBootstrapAsync) : base(iStateID, pOwner, pFsm, pGameInstance)
        {
            this.delegateBootstrapAsync = pDelegateBootstrapAsync;
        }
        public BootstrapAsyncDelegate delegateBootstrapAsync { get; private set; } = null;
    }
    public CState_Sys_Initialize(STATE_SYS_INITIALIZE_DESC pRefOwner) : base(pRefOwner)
    {
        m_pDelegateBootstrapAsync = pRefOwner.delegateBootstrapAsync;
    }
    BootstrapAsyncDelegate m_pDelegateBootstrapAsync = null;
    bool m_bIsLoading = false;


    public override void Enter()
    {
        Debug.Log("CState_Sys_Initialize Enter");
    }

    public override void Fixed_Update_State()
    {
        
    }
    async public override void Update_State()
    {
        if (null != m_pDelegateBootstrapAsync)
        {
            if (false == m_bIsLoading)
            {
                m_bIsLoading = true;
                await m_pDelegateBootstrapAsync.Invoke();
                m_bIsLoading = false;
                m_pDelegateBootstrapAsync = null;
                FSM.Change_State((int)DEFINES.ENUMS.SystemState.IDLE);
            }
        }
    }


    public override void Late_Update_State()
    {
        
    }

    public override void Exit()
    {
        Debug.Log("CState_Sys_Initialize Exit");
        GameInstance.ChangeScene(DEFINES.ENUMS.SceneID.MAIN_MENU);
    }
}