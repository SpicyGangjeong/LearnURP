using Core.StateMachine;
using System;
using UnityEngine;

namespace Logic
{
    namespace State
    {
        [Serializable]
        public abstract class CState
        {
            public class CSTATEDESC
            {
                public CSTATEDESC(int iStateID, MonoBehaviour pOwner, CFSM pFsm)
                {
                    this.StateID = iStateID;
                    this.Owner = pOwner;
                    this.FSM = pFsm;
                }
                public int StateID { get; private set; }
                public MonoBehaviour Owner { get; private set; }
                public CFSM FSM { get; private set; } = null;
            }
            public CState(CSTATEDESC pRefOwner)
            {
                StateID = pRefOwner.StateID;
                Owner = pRefOwner.Owner;
                FSM = pRefOwner.FSM;

                FSM.m_vStates.Add(StateID, this);
            }
            public int StateID { get; private set; } = -1;
            public MonoBehaviour Owner { get; private set; } = null;
            public CFSM FSM { get; private set; } = null;
            public abstract void Enter();
            public abstract void Fixed_Update_State();
            public abstract void Update_State();
            public abstract void Late_Update_State();
            public abstract void Exit();

        }

    }
}
