using Core;
using Core.Job;
using Core.StateMachine;
using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

namespace Logic
{
    namespace State
    {
        [Serializable]
        public sealed class CState_System_Initialize : CState_System
        {
            public class STATE_SYSTEM_INITIALIZE_DESC : STATE_SYSTEM_DESC
            {
                public STATE_SYSTEM_INITIALIZE_DESC(MonoBehaviour pOwner, FSM pFsm, GameInstance pGameInstance,
                Func<UniTask> pBootstrapAsync)
                    : base((int)SystemState.INITIALIZE, pOwner, pFsm, pGameInstance)
                {
                    this.BootstrapAsync = pBootstrapAsync;
                }
                public Func<UniTask> BootstrapAsync { get; private set; } = null;
            }
            public CState_System_Initialize(STATE_SYSTEM_INITIALIZE_DESC pRefOwner) : base(pRefOwner)
            {
                m_pBootstrapAsync = pRefOwner.BootstrapAsync;
            }
            Func<UniTask> m_pBootstrapAsync = null;
            bool m_bIsLoading = false;


            public override void Enter()
            {
                Debug.Log("CState_Sys_Initialize Enter");
            }

            public override void Fixed_Update_State()
            {

            }
            public override void Update_State()
            {
                if (null == m_pBootstrapAsync || true == m_bIsLoading)
                {
                    return;
                }
                RunBootstrapAsync().Forget();

                async UniTaskVoid RunBootstrapAsync()
                {
                    m_bIsLoading = true;
                    await m_pBootstrapAsync.Invoke();
                    m_bIsLoading = false;
                    m_pBootstrapAsync = null;
                    FSM.Change_State((int)SystemState.IDLE);
                }
            }


            public override void Late_Update_State()
            {

            }

            public override void Exit()
            {
                Debug.Log("CState_Sys_Initialize Exit");
                JobDeferredCallback callback = new JobDeferredCallback(
                    () => m_pGameInstance.ChangeScene(Defines.Enums.SceneID.MAIN_MENU), "Exiting_Initialize"
                    );
                m_pGameInstance.EnqueueJob(callback);
            }
        }
    }
}
