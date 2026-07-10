using System;
using System.Collections.Generic;
using UnityEngine;
using Logic.State;

namespace Core
{
    namespace StateMachine
    {
        public class CFSM : MonoBehaviour
        {
            public Dictionary<int, CState> m_vStates = null;
            CState m_pCurrState = null;
            CState m_pPrevState = null;
            static void EmptyTick() { }

            Action m_pOnFixedUpdateTick = EmptyTick;
            Action m_pOnUpdateTick = EmptyTick;
            Action m_pOnLateUpdateTick = EmptyTick;

            public bool IsCurrentState(int eStateID)
            {
                return m_pCurrState != null && m_pCurrState.StateID == eStateID;
            }

            void ResubscribeStateTicks(CState pNext)
            {
                m_pOnFixedUpdateTick = EmptyTick;
                m_pOnUpdateTick = EmptyTick;
                m_pOnLateUpdateTick = EmptyTick;
                if (pNext == null)
                    return;
                m_pOnFixedUpdateTick += pNext.Fixed_Update_State;
                m_pOnUpdateTick += pNext.Update_State;
                m_pOnLateUpdateTick += pNext.Late_Update_State;
            }

            public void Change_State(int eStateID)
            {
                if (null != m_pCurrState)
                {
                    m_pCurrState.Exit();
                    m_pPrevState = m_pCurrState;
                }
                if (false == m_vStates.TryGetValue(eStateID, out m_pCurrState))
                {
                    Debug.LogError($"State not found: {eStateID} {m_vStates.Count}");
                    return;
                }
                ResubscribeStateTicks(m_pCurrState);
                if (null != m_pCurrState)
                {
                    m_pCurrState.Enter();
                }
            }
            public bool Is_Valid_FSM(int eStateEndID)
            {
                if (null == m_vStates)
                {
                    Debug.LogError("FSM is not valid: states is null");
                    return false;
                }
                for (int i = 0; i < eStateEndID; i++)
                {
                    if (false == m_vStates.TryGetValue(i, out CState pState))
                    {
                        Debug.LogError($"FSM is not valid: {i} is not found in states");
                        return false;
                    }
                }
                return true;
            }
            public void Fixed_Update_State()
            {
                m_pOnFixedUpdateTick();
            }

            public void Update_State()
            {
                m_pOnUpdateTick();
            }
            public void Late_Update_State()
            {
                m_pOnLateUpdateTick();
            }
            public CState Get_PrevState()
            {
                return m_pPrevState;
            }
            public CState Get_CurrentState()
            {
                return m_pCurrState;
            }
            void Awake() { }
            void Start() { }
            void FixedUpdate() { Fixed_Update_State(); }
            void Update() { Update_State(); }
            void LateUpdate() { Late_Update_State(); }
        }

    }
}