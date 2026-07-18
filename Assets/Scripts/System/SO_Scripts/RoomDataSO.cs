using Defines.Bases;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Logic
{
    namespace Room
    {
        [CreateAssetMenu(fileName = "RoomDataSO", menuName = "Scriptable Objects/RoomDataSO")]
        [Serializable]
        public class RoomDataSO : ScriptableObjectCloneable<RoomDataSO>
        {
            public RoomInformation m_ScriptedObject;

            void OnValidate()
            {
                if (m_ScriptedObject.iID < 0)
                {
                    m_ScriptedObject.iID = 0;
                }
                if (null == m_ScriptedObject.vConnectedRoomIDs)
                {
                    m_ScriptedObject.vConnectedRoomIDs = new List<int>();
                }
                if (null == m_ScriptedObject.m_pActivation)
                {
                    m_ScriptedObject.m_pActivation = new RoomActivationCondition();
                }
                if (null == m_ScriptedObject.m_pCompletion)
                {
                    m_ScriptedObject.m_pCompletion = new RoomCompletionCondition();
                }
            }

            public RoomData Instantiate()
            {
                return RoomData.Create(m_ScriptedObject);
            }

            protected override void CopyFrom(RoomDataSO pOriginal)
            {
                m_ScriptedObject = pOriginal.m_ScriptedObject;
                if (null != pOriginal.m_ScriptedObject.vConnectedRoomIDs)
                {
                    m_ScriptedObject.vConnectedRoomIDs = new List<int>(pOriginal.m_ScriptedObject.vConnectedRoomIDs);
                }
                else
                {
                    m_ScriptedObject.vConnectedRoomIDs = new List<int>();
                }

                if (null == pOriginal.m_ScriptedObject.m_pActivation)
                {
                    m_ScriptedObject.m_pActivation = new RoomActivationCondition();
                }
                else
                {
                    m_ScriptedObject.m_pActivation = new RoomActivationCondition
                    {
                        m_eKind = pOriginal.m_ScriptedObject.m_pActivation.m_eKind,
                        m_strKey = pOriginal.m_ScriptedObject.m_pActivation.m_strKey,
                    };
                }

                if (null == pOriginal.m_ScriptedObject.m_pCompletion)
                {
                    m_ScriptedObject.m_pCompletion = new RoomCompletionCondition();
                }
                else
                {
                    m_ScriptedObject.m_pCompletion = new RoomCompletionCondition
                    {
                        m_eKind = pOriginal.m_ScriptedObject.m_pCompletion.m_eKind,
                        m_strKey = pOriginal.m_ScriptedObject.m_pCompletion.m_strKey,
                    };
                }
            }

            private RoomDataSO()
            {
            }

            private RoomDataSO(RoomDataSO pOriginal)
            {
            }
        }
    }
}
