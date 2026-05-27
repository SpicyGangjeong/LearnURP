using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct CardNameCount
{
    public string strCardName;
    public int iCount;
}

[CreateAssetMenu(fileName = "CardInitialSet", menuName = "Scriptable Objects/CardInitialSet")]
public class CardInitialSet : ScriptableObject
{
    [SerializeField] string strCardInitialSetName = string.Empty;
    [SerializeField] CardDocuments cardDocuments = null;
    [SerializeField] List<CardNameCount> cardEntries = new List<CardNameCount>();
    Dictionary<CardInfo, int> cardInitialSetLookup = null;
    

    public IReadOnlyList<CardNameCount> CardEntries => cardEntries;

    void OnEnable()
    {
        if (null != cardDocuments)
        {
            BuildLookup();
        }
    }

    public void SetCardDocuments(CardDocuments documents)
    {
        cardDocuments = documents;
        BuildLookup();
    }

    void BuildLookup()
    {
        cardInitialSetLookup = new Dictionary<CardInfo, int>();
        if (null == cardDocuments)
        {
            return;
        }

        foreach (CardNameCount entry in cardEntries)
        {
            if (string.IsNullOrEmpty(entry.strCardName) || entry.iCount <= 0)
            {
                continue;
            }

            if (false == cardDocuments.TryGetCardByName(entry.strCardName, out CardInfo cardInfo))
            {
                Debug.LogWarning($"Card not found in CardDocuments: {entry.strCardName}");
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
