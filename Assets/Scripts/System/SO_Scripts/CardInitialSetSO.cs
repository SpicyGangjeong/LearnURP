using Logic.Card;
using System;
using System.Collections.Generic;
using UnityEngine;
namespace SO
{
    [Serializable]
    public struct CardNameCount
    {
        public string m_strCardName;
        public int m_iCount;
    }

    [CreateAssetMenu(fileName = "CardInitialSetSO", menuName = "Scriptable Objects/CardInitialSetSO")]
    public class CardInitialSetSO : ScriptableObject
    {
        [SerializeField] string m_strCardInitialSetName = string.Empty;
        [SerializeField] CardDocumentSO m_pCardDocumentSO = null;
        [SerializeField] List<CardNameCount> m_vCardEntries = new List<CardNameCount>();
        Dictionary<CardData, int> m_vCardInitialSetLookup = null;


        public IReadOnlyList<CardNameCount> CardEntries => m_vCardEntries;

        void OnEnable()
        {
            if (null != m_pCardDocumentSO)
            {
                BuildLookup();
            }
        }

        public void SetCardDocumentSO(CardDocumentSO pDocuments)
        {
            m_pCardDocumentSO = pDocuments;
            BuildLookup();
        }

        void BuildLookup()
        {
            m_vCardInitialSetLookup = new Dictionary<CardData, int>();
            if (null == m_pCardDocumentSO)
            {
                return;
            }

            foreach (CardNameCount pEntry in m_vCardEntries)
            {
                if (string.IsNullOrEmpty(pEntry.m_strCardName) || pEntry.m_iCount <= 0)
                {
                    continue;
                }

                if (false == m_pCardDocumentSO.TryGetCardByName(pEntry.m_strCardName, out CardData pCardInfo))
                {
                    Debug.LogWarning($"Card not found in CardDocumentSO: {pEntry.m_strCardName}");
                    continue;
                }

                if (m_vCardInitialSetLookup.ContainsKey(pCardInfo))
                {
                    m_vCardInitialSetLookup[pCardInfo] += pEntry.m_iCount;
                }
                else
                {
                    m_vCardInitialSetLookup.Add(pCardInfo, pEntry.m_iCount);
                }
            }
        }

        public IReadOnlyDictionary<CardData, int> GetCardInitialSet()
        {
            if (null == m_vCardInitialSetLookup)
            {
                BuildLookup();
            }

            return m_vCardInitialSetLookup;
        }
    }

}