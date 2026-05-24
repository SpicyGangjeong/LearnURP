using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CardDocuments", menuName = "Scriptable Objects/CardDocuments")]
public class CardDocuments : ScriptableObject
{
    [SerializeField] List<CardInfo> cardInfos = new List<CardInfo>();

    Dictionary<int, CardInfo> cardLookup = null;
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
        foreach (CardInfo cardInfo in cardInfos)
        {
            if (cardLookup.ContainsKey(cardInfo.iCardID))
            {
                Debug.LogWarning($"Duplicate card ID in CardDocuments: {cardInfo.iCardID}");
                continue;
            }

            cardLookup.Add(cardInfo.iCardID, cardInfo);
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

}
