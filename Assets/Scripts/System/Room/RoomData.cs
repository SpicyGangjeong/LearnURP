using System;
using System.Collections.Generic;
using UnityEngine;

namespace Logic
{
    namespace Room
    {
        [Serializable]
        public struct RoomInformation
        {
            public string strName;
            public int iID;
            public Defines.Enums.RoomType eType;
            public List<int> vConnectedRoomIDs;
            public string strPrefabKey;
            public RoomActivationCondition m_pActivation;
            public RoomCompletionCondition m_pCompletion;
        }

        [Serializable]
        public class RoomData
        {
            [SerializeField]
            RoomInformation m_Data = new RoomInformation();

            public string Name => m_Data.strName;
            public int ID => m_Data.iID;
            public Defines.Enums.RoomType Type => m_Data.eType;
            public IReadOnlyList<int> ConnectedRoomIDs => m_Data.vConnectedRoomIDs;
            public string PrefabKey => m_Data.strPrefabKey;
            public RoomActivationCondition Activation => m_Data.m_pActivation;
            public RoomCompletionCondition Completion => m_Data.m_pCompletion;

            private RoomData()
            {
            }

            private bool Initialize(in RoomInformation pOriginal)
            {
                m_Data = pOriginal;
                if (null == m_Data.vConnectedRoomIDs)
                {
                    m_Data.vConnectedRoomIDs = new List<int>();
                }
                else
                {
                    m_Data.vConnectedRoomIDs = new List<int>(pOriginal.vConnectedRoomIDs);
                }

                if (null == pOriginal.m_pActivation)
                {
                    m_Data.m_pActivation = new RoomActivationCondition();
                }
                else
                {
                    m_Data.m_pActivation = new RoomActivationCondition
                    {
                        m_eKind = pOriginal.m_pActivation.m_eKind,
                        m_strKey = pOriginal.m_pActivation.m_strKey,
                    };
                }

                if (null == pOriginal.m_pCompletion)
                {
                    m_Data.m_pCompletion = new RoomCompletionCondition();
                }
                else
                {
                    m_Data.m_pCompletion = new RoomCompletionCondition
                    {
                        m_eKind = pOriginal.m_pCompletion.m_eKind,
                        m_strKey = pOriginal.m_pCompletion.m_strKey,
                    };
                }
                return true;
            }

            public static RoomData Create(in RoomInformation pInformation)
            {
                RoomData pInstance = new RoomData();
                if (false == pInstance.Initialize(pInformation))
                {
                    pInstance = null;
                }
                return pInstance;
            }
        }
    }
}
