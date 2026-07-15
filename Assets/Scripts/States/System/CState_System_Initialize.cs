using Core;
using Core.StateMachine;
using Core.Job;
using System.Threading.Tasks;
using UnityEngine;

namespace Logic
{
    namespace State
    {
        public class CState_System_Initialize : CState_System
        {
            public delegate Task BootstrapAsyncDelegate();
            public class STATE_SYSTEM_INITIALIZE_DESC : STATE_SYSTEM_DESC
            {
                public STATE_SYSTEM_INITIALIZE_DESC(int iStateID, MonoBehaviour pOwner, CFSM pFsm, CGameInstance pGameInstance,
                BootstrapAsyncDelegate pDelegateBootstrapAsync) : base(iStateID, pOwner, pFsm, pGameInstance)
                {
                    this.delegateBootstrapAsync = pDelegateBootstrapAsync;
                }
                public BootstrapAsyncDelegate delegateBootstrapAsync { get; private set; } = null;
            }
            public CState_System_Initialize(STATE_SYSTEM_INITIALIZE_DESC pRefOwner) : base(pRefOwner)
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
                        FSM.Change_State((int)SystemState.IDLE);
                    }
                }
            }


            public override void Late_Update_State()
            {

            }

            public override void Exit()
            {
                Debug.Log("CState_Sys_Initialize Exit");
                JobDeferredCallback callback = new JobDeferredCallback(
                    () => GameInstance.ChangeScene(Defines.Enums.SceneID.MAIN_MENU), "Exiting_Initialize"
                    );
                GameInstance.EnqueueJob(callback);
            }
        }
    }
}
