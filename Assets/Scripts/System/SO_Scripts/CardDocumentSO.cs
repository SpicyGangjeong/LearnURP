using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CardDocumentSO", menuName = "Scriptable Objects/CardDocumentSO")]
public class CardDocumentSO : ScriptableObject
{
    [SerializeField] List<CardData> m_vCardInfos = new List<CardData>();

    Dictionary<int, CardData> m_vCardLookup = null;
    Dictionary<string, CardData> m_vCardNameLookup = null;
    [ContextMenu("Rebuild Card Description")]
    void RebuildCardDescription()
    {
        foreach (CardData pCardInfo in m_vCardInfos)
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
        m_vCardLookup = new Dictionary<int, CardData>(m_vCardInfos.Count);
        m_vCardNameLookup = new Dictionary<string, CardData>(m_vCardInfos.Count);
        foreach (CardData pCardInfo in m_vCardInfos)
        {
            if (m_vCardLookup.ContainsKey(pCardInfo.m_iCardID))
            {
                Debug.LogWarning($"Duplicate card ID in CardDocuments: {pCardInfo.m_iCardID}");
                continue;
            }

            m_vCardLookup.Add(pCardInfo.m_iCardID, pCardInfo);

            if (string.IsNullOrEmpty(pCardInfo.m_strCardName))
            {
                continue;
            }

            if (m_vCardNameLookup.ContainsKey(pCardInfo.m_strCardName))
            {
                Debug.LogWarning($"Duplicate card name in CardDocuments: {pCardInfo.m_strCardName}");
                continue;
            }

            m_vCardNameLookup.Add(pCardInfo.m_strCardName, pCardInfo);
        }
        RebuildCardDescription();
    }

    public bool TryGetCard(int iCardID, out CardData pCardInfo)
    {
        if (null == m_vCardLookup)
        {
            BuildLookup();
        }

        return m_vCardLookup.TryGetValue(iCardID, out pCardInfo);
    }

    public CardData GetCard(int iCardID)
    {
        if (TryGetCard(iCardID, out CardData pCardInfo))
        {
            return pCardInfo;
        }

        Debug.LogError($"Card not found in CardDocuments: {iCardID}");
        return null;
    }

    public bool TryGetCardByName(string strCardName, out CardData pCardInfo)
    {
        if (null == m_vCardNameLookup)
        {
            BuildLookup();
        }

        return m_vCardNameLookup.TryGetValue(strCardName, out pCardInfo);
    }

    public CardData GetCardByName(string strCardName)
    {
        if (TryGetCardByName(strCardName, out CardData pCardInfo))
        {
            return pCardInfo;
        }

        Debug.LogError($"Card not found in CardDocuments: {strCardName}");
        return null;
    }

}
