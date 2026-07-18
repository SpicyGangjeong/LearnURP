using Core;
using Core.StateMachine;
using Logic;
using Logic.State;
using UnityEngine;
using static Logic.State.CState_Player_Idle;
using static Logic.State.CState_Player_Initialize;
namespace Controller
{
    namespace Unit
    {
        [RequireComponent(typeof(CFSM))]
        public class PlayerController : MonoBehaviour, IController
        {
            CFSM m_pFSM = null;
            Animator m_pAnimator = null;
            IUnit m_pPlayer = null;
            void Awake()
            {
                Ready_FSM();
                CInfoInstance.Instance.PlayerInstance.BindController(this);
                DontDestroyOnLoad(this);
            }
            void Start()
            {

            }

            void Update()
            {
            }

            public void BindUnit(IUnit unit)
            {
                TransformHandle targetHandle = unit.GetTransformHandle();
                targetHandle.SetParent(transform.transformHandle);
                m_pPlayer = unit;
                m_pAnimator = unit.GetAnimator();
                m_pFSM.Change_State((int)CState_Player.PlayerState.IDLE);
            }

            public void Ready_FSM()
            {
                if (null == m_pFSM)
                {
                    m_pFSM = GetComponent<CFSM>();
                    new CState_Player_Initialize(
                        new STATE_PLAYER_INITIALIZE_DESC(
                            this, m_pFSM, this
                            )
                        );
                    new CState_Player_Idle(
                        new STATE_PLAYER_IDLE_DESC(
                            this, m_pFSM, this
                            )
                        );
                    if (true == m_pFSM.Is_Valid_FSM((int)CState_Player.PlayerState.END))
                    {
                        m_pFSM.Change_State((int)CState_Player.PlayerState.INITIALIZE);
                    }
                }
            }

            public void Control()
            {
                
            }
        }
    }
}