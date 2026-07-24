using Core;
using Core.StateMachine;
using Cysharp.Threading.Tasks;
using Defines.Expressions;
using System;
using UnityEngine;
using UnityEngine.InputSystem.XR;
namespace Logic
{
    namespace State
    {
        [Serializable]
        public sealed class CState_Player_Initialize : CState_Player
        {
            public class STATE_PLAYER_INITIALIZE_DESC : STATE_PLAYER_DESC
            {
                public STATE_PLAYER_INITIALIZE_DESC(MonoBehaviour pOwner, FSM pFsm, IController controller)
                    : base((int)PlayerState.INITIALIZE, pOwner, pFsm, controller)
                {

                }
            }
            public CState_Player_Initialize(STATE_PLAYER_INITIALIZE_DESC pRefOwner) : base(pRefOwner)
            {

            }

            public override void Enter()
            {
                Debug.Log("CState_Sys_Initialize Enter");
            }

            public override void Fixed_Update_State()
            {

            }
            async public override void Update_State()
            {
                if (ERESULT.TRUE == GetRaycastHit(out RaycastHit hit))
                {
                    CPlayable target = hit.transform.GetComponent<CPlayable>();
                    if (null != target)
                    {
                        InfoInstance.Instance.PlayerInstance.PlayerRegist(target);
                    }
                }
                await UniTask.CompletedTask;
            }
            public ERESULT GetRaycastHit(out RaycastHit hitOut)
            {
                Vector3 vMousePos = GameInstance.Instance.Mouse_Pos;
                Camera currentCamera = GameInstance.Instance.Main_Camera;
                Ray ray = currentCamera.ScreenPointToRay(vMousePos);
                if (true == Physics.Raycast(ray, out hitOut))
                {
                    return ERESULT.TRUE;
                }
                else
                {
                    Debug.Log("Nothing was hit");
                }
                return ERESULT.FALSE;
            }



            public override void Late_Update_State()
            {

            }

            public override void Exit()
            {
            }
        }
    }
}
