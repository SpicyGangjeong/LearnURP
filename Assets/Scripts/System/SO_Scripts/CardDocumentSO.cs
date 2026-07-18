using Defines.Bases;
using Logic.Card;
using System.Collections.Generic;
using UnityEngine;

namespace SO
{
    [CreateAssetMenu(fileName = "CardDocumentSO", menuName = "Scriptable Objects/CardDocumentSO")]
    public class CardDocumentSO : ScriptableObjectCloneable<CardDocumentSO>
    {
        [SerializeField] List<CardDataSO> m_vCardInfos = new List<CardDataSO>();

        Dictionary<string, CardDataSO> m_vCardLookup = null;
        [ContextMenu("Rebuild Card Description")]
        void RebuildCardDescription()
        {
            foreach (CardDataSO pCardInfo in m_vCardInfos)
            {
                pCardInfo.BuildCardDescription();
            }
        }
        void OnEnable()
        {
            BuildLookup();
        }

        void BuildLookup()
        {
            m_vCardLookup = new Dictionary<string, CardDataSO>(m_vCardInfos.Count);
            foreach (CardDataSO pCardInfo in m_vCardInfos)
            {
                if (string.IsNullOrEmpty(pCardInfo.m_ScriptedObject.strName))
                {
                    continue;
                }

                if (m_vCardLookup.ContainsKey(pCardInfo.m_ScriptedObject.strName))
                {
                    Debug.LogWarning($"Duplicate card name in CardDocuments: {pCardInfo.m_ScriptedObject.strName}");
                    continue;
                }

                m_vCardLookup.Add(pCardInfo.m_ScriptedObject.strName, pCardInfo.Clone());
            }
            RebuildCardDescription();
        }

        public bool TryGetCardByName(string strCardName, out CardDataSO pCardInfo)
        {
            if (null == m_vCardLookup)
            {
                BuildLookup();
            }

            if (false == m_vCardLookup.TryGetValue(strCardName, out CardDataSO pOriginal))
            {
                pCardInfo = null;
                return false;
            }

            pCardInfo = pOriginal.Clone();
            return true;
        }

        protected override void CopyFrom(CardDocumentSO pOriginal)
        {
            foreach (CardDataSO cardDataSO in pOriginal.m_vCardInfos)
            {
                m_vCardInfos.Add(cardDataSO.Clone());
            }
            m_vCardLookup = null;
            BuildLookup();
        }
        private CardDocumentSO() { }
        private CardDocumentSO(CardDocumentSO pOriginal) { }
    }

}