using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "CardDocumentSO", menuName = "Scriptable Objects/CardDocumentSO")]
public class CardDocumentSO : ScriptableObject
{
    [SerializeField] List<CardInfo> m_vCardInfos = new List<CardInfo>();

    Dictionary<int, CardInfo> m_vCardLookup = null;
    Dictionary<string, CardInfo> m_vCardNameLookup = null;
    [ContextMenu("Rebuild Card Description")]
    void RebuildCardDescription()
    {
        foreach (CardInfo pCardInfo in m_vCardInfos)
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
        m_vCardLookup = new Dictionary<int, CardInfo>(m_vCardInfos.Count);
        m_vCardNameLookup = new Dictionary<string, CardInfo>(m_vCardInfos.Count);
        foreach (CardInfo pCardInfo in m_vCardInfos)
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

    public bool TryGetCard(int iCardID, out CardInfo pCardInfo)
    {
        if (null == m_vCardLookup)
        {
            BuildLookup();
        }

        return m_vCardLookup.TryGetValue(iCardID, out pCardInfo);
    }

    public CardInfo GetCard(int iCardID)
    {
        if (TryGetCard(iCardID, out CardInfo pCardInfo))
        {
            return pCardInfo;
        }

        Debug.LogError($"Card not found in CardDocuments: {iCardID}");
        return null;
    }

    public bool TryGetCardByName(string strCardName, out CardInfo pCardInfo)
    {
        if (null == m_vCardNameLookup)
        {
            BuildLookup();
        }

        return m_vCardNameLookup.TryGetValue(strCardName, out pCardInfo);
    }

    public CardInfo GetCardByName(string strCardName)
    {
        if (TryGetCardByName(strCardName, out CardInfo pCardInfo))
        {
            return pCardInfo;
        }

        Debug.LogError($"Card not found in CardDocuments: {strCardName}");
        return null;
    }

}
