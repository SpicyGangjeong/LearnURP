using Defines.Bases;
using System;
using UnityEngine;

namespace Logic
{
    namespace Card
    {
        [CreateAssetMenu(fileName = "CardDataSO", menuName = "Scriptable Objects/CardDataSO")]
        [Serializable]
        public class CardDataSO : ScriptableObjectCloneable<CardDataSO>
        {
            public CardInformation m_ScriptedObject;

            public void BuildCardDescription()
            {
                if (null == m_ScriptedObject.vEffects)
                {
                    m_ScriptedObject.vEffects = new CardEffect();
                }
                m_ScriptedObject.BuildCardDescription();
            }

            void OnValidate()
            {
                if (m_ScriptedObject.iID < 0)
                {
                    m_ScriptedObject.iID = 0;
                }
                if (m_ScriptedObject.iCost < 0)
                {
                    m_ScriptedObject.iCost = 0;
                }
                BuildCardDescription();
            }

            public CardData Instantiate()
            {
                return CardData.Create(m_ScriptedObject);
            }

            protected override void CopyFrom(CardDataSO pOriginal)
            {
                m_ScriptedObject = pOriginal.m_ScriptedObject;
                m_ScriptedObject.vEffects = new CardEffect(pOriginal.m_ScriptedObject.vEffects);
            }

            private CardDataSO() { }
            private CardDataSO(CardDataSO pOriginal) { }
        }
    }
}
