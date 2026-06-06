using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public struct CardNameCount
{
    [FormerlySerializedAs("strCardName")]
    public string m_strCardName;
    [FormerlySerializedAs("iCount")]
    public int m_iCount;
}

[CreateAssetMenu(fileName = "CardInitialSetSO", menuName = "Scriptable Objects/CardInitialSetSO")]
public class CardInitialSetSO : ScriptableObject
{
    [FormerlySerializedAs("strCardInitialSetName")]
    [SerializeField] string m_strCardInitialSetName = string.Empty;
    [FormerlySerializedAs("cardDocumentSO")]
    [SerializeField] CardDocumentSO m_pCardDocumentSO = null;
    [FormerlySerializedAs("cardEntries")]
    [SerializeField] List<CardNameCount> m_vCardEntries = new List<CardNameCount>();
    Dictionary<CardInfo, int> m_vCardInitialSetLookup = null;
    

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
        m_vCardInitialSetLookup = new Dictionary<CardInfo, int>();
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

            if (false == m_pCardDocumentSO.TryGetCardByName(pEntry.m_strCardName, out CardInfo pCardInfo))
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

    public IReadOnlyDictionary<CardInfo, int> GetCardInitialSet()
    {
        if (null == m_vCardInitialSetLookup)
        {
            BuildLookup();
        }

        return m_vCardInitialSetLookup;
    }
}
