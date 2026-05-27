using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct CardNameCount
{
    public string strCardName;
    public int iCount;
}

[CreateAssetMenu(fileName = "CardInitialSetSO", menuName = "Scriptable Objects/CardInitialSetSO")]
public class CardInitialSetSO : ScriptableObject
{
    [SerializeField] string strCardInitialSetName = string.Empty;
    [SerializeField] CardDocumentSO cardDocumentSO = null;
    [SerializeField] List<CardNameCount> cardEntries = new List<CardNameCount>();
    Dictionary<CardInfo, int> cardInitialSetLookup = null;
    

    public IReadOnlyList<CardNameCount> CardEntries => cardEntries;

    void OnEnable()
    {
        if (null != cardDocumentSO)
        {
            BuildLookup();
        }
    }

    public void SetCardDocumentSO(CardDocumentSO documents)
    {
        cardDocumentSO = documents;
        BuildLookup();
    }

    void BuildLookup()
    {
        cardInitialSetLookup = new Dictionary<CardInfo, int>();
        if (null == cardDocumentSO)
        {
            return;
        }

        foreach (CardNameCount entry in cardEntries)
        {
            if (string.IsNullOrEmpty(entry.strCardName) || entry.iCount <= 0)
            {
                continue;
            }

            if (false == cardDocumentSO.TryGetCardByName(entry.strCardName, out CardInfo cardInfo))
            {
                Debug.LogWarning($"Card not found in CardDocumentSO: {entry.strCardName}");
                continue;
            }

            if (cardInitialSetLookup.ContainsKey(cardInfo))
            {
                cardInitialSetLookup[cardInfo] += entry.iCount;
            }
            else
            {
                cardInitialSetLookup.Add(cardInfo, entry.iCount);
            }
        }
    }

    public IReadOnlyDictionary<CardInfo, int> GetCardInitialSet()
    {
        if (null == cardInitialSetLookup)
        {
            BuildLookup();
        }

        return cardInitialSetLookup;
    }
}
