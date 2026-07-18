using Core.StateMachine;
using System;
using UnityEngine;
namespace Logic
{
    namespace State
    {
        [Serializable]
        public abstract class CState_Unit : CState
        {
            public class STATE_UNIT_DESC : CSTATEDESC
            {
                public IController pController = null;
                public STATE_UNIT_DESC(int iStateID, MonoBehaviour pOwner, CFSM pFsm, IController controller) : base(iStateID, pOwner, pFsm)
                {
                    pController = controller;
                }
            }
            public CState_Unit(STATE_UNIT_DESC pRefOwner) : base(pRefOwner)
            {
                m_pController = pRefOwner.pController;
            }
            IController m_pController = null;
        }
    }
}