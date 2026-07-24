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
            public class STATE_DESC
            {
                public STATE_DESC(int iStateID, MonoBehaviour pOwner, FSM pFsm)
                {
                    this.StateID = iStateID;
                    this.Owner = pOwner;
                    this.FSM = pFsm;
                }
                public int StateID { get; private set; }
                public MonoBehaviour Owner { get; private set; }
                public FSM FSM { get; private set; } = null;
            }
            public CState(STATE_DESC pRefOwner)
            {
                StateID = pRefOwner.StateID;
                Owner = pRefOwner.Owner;
                FSM = pRefOwner.FSM;

                FSM.m_vStates.Add(StateID, this);
            }
            public int StateID { get; private set; } = -1;
            public MonoBehaviour Owner { get; private set; } = null;
            public FSM FSM { get; private set; } = null;
            public abstract void Enter();
            public abstract void Fixed_Update_State();
            public abstract void Update_State();
            public abstract void Late_Update_State();
            public abstract void Exit();

        }

    }
}
