using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CardDocumentSO", menuName = "Scriptable Objects/CardDocumentSO")]
public class CardDocumentSO : ScriptableObject
{
    [SerializeField] List<CardInfo> cardInfos = new List<CardInfo>();

    Dictionary<int, CardInfo> cardLookup = null;
    Dictionary<string, CardInfo> cardNameLookup = null;
    [ContextMenu("Rebuild Card Description")]
    void RebuildCardDescription()
    {
        foreach (CardInfo cardInfo in cardInfos)
        {
            cardInfo.BuildCardDescription();
        }
    }
    void OnEnable()
    {
        BuildLookup();
    }

    void BuildLookup()
    {
        cardLookup = new Dictionary<int, CardInfo>(cardInfos.Count);
        cardNameLookup = new Dictionary<string, CardInfo>(cardInfos.Count);
        foreach (CardInfo cardInfo in cardInfos)
        {
            if (cardLookup.ContainsKey(cardInfo.iCardID))
            {
                Debug.LogWarning($"Duplicate card ID in CardDocuments: {cardInfo.iCardID}");
                continue;
            }

            cardLookup.Add(cardInfo.iCardID, cardInfo);

            if (string.IsNullOrEmpty(cardInfo.strCardName))
            {
                continue;
            }

            if (cardNameLookup.ContainsKey(cardInfo.strCardName))
            {
                Debug.LogWarning($"Duplicate card name in CardDocuments: {cardInfo.strCardName}");
                continue;
            }

            cardNameLookup.Add(cardInfo.strCardName, cardInfo);
        }
    }

    public bool TryGetCard(int cardID, out CardInfo cardInfo)
    {
        if (null == cardLookup)
        {
            BuildLookup();
        }

        return cardLookup.TryGetValue(cardID, out cardInfo);
    }

    public CardInfo GetCard(int cardID)
    {
        if (TryGetCard(cardID, out CardInfo cardInfo))
        {
            return cardInfo;
        }

        Debug.LogError($"Card not found in CardDocuments: {cardID}");
        return null;
    }

    public bool TryGetCardByName(string cardName, out CardInfo cardInfo)
    {
        if (null == cardNameLookup)
        {
            BuildLookup();
        }

        return cardNameLookup.TryGetValue(cardName, out cardInfo);
    }

    public CardInfo GetCardByName(string cardName)
    {
        if (TryGetCardByName(cardName, out CardInfo cardInfo))
        {
            return cardInfo;
        }

        Debug.LogError($"Card not found in CardDocuments: {cardName}");
        return null;
    }

}
