using System;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

class Card
{
    CardInfo cardInfo = null;

    public Card(int cardID = 0)
    {
        cardInfo = CGameInstance.Instance.GetCardInfo(cardID);
        if (null == cardInfo)
        {
            Debug.LogError($"Card not found in CardDocuments: {cardID}");
            return;
        }
    }

    public delegate void DrawCard();
    public delegate void PlayCard();
    public delegate void DiscardCard();
    public delegate void ReturnCard();
    public delegate void DisappearCard();
    public delegate void ShuffleCard();
    DrawCard drawCard = null;
    PlayCard playCard = null;
    DiscardCard discardCard = null;
    ReturnCard returnCard = null;
    DisappearCard disappearCard = null;
    ShuffleCard shuffleCard = null;
}

[Serializable]
public class CardEffect
{
    public DEFINES.CardEffectTriggerType eCardEffectTriggerType;
    public DEFINES.CardEffectTargetType eCardEffectTargetType;
    public DEFINES.CardEffectValueType eCardEffectValueType;
    public int iCardEffectValue;
    public bool bCardEffectOptional;
    public int iCardEffectOptionalValue;
}

[Serializable]
public class CardInfo
{
    public int iCardID;
    public DEFINES.CardType eCardType;
    public int iCardCost;
    public string strCardName;
    [SerializeField] public List<CardEffect> listCardEffects = new List<CardEffect>();
    public string strCardDescription;
    public void BuildCardDescription()
    {
        StringBuilder sb = new StringBuilder();
        foreach (CardEffect cardEffect in listCardEffects)
        {
            sb.Append("When ");
            switch (cardEffect.eCardEffectTriggerType)
            {
                case DEFINES.CardEffectTriggerType.NONE:
                    break;
                case DEFINES.CardEffectTriggerType.DRAW:
                    sb.Append("Draw");
                    break;
                case DEFINES.CardEffectTriggerType.PLAY:
                    sb.Append("Play");
                    break;
                case DEFINES.CardEffectTriggerType.DISCARD:
                    sb.Append("Discard");
                    break;
                case DEFINES.CardEffectTriggerType.RETURN:
                    sb.Append("Return");
                    break;
                case DEFINES.CardEffectTriggerType.DISAPPEAR:
                    sb.Append("Disappear");
                    break;
                case DEFINES.CardEffectTriggerType.SHUFFLE:
                    sb.Append("Shuffle");
                    break;
                case DEFINES.CardEffectTriggerType.END:
                    sb.Append("End");
                    break;
            }
            sb.Append(", ");
            switch (cardEffect.eCardEffectTargetType)
            {
                case DEFINES.CardEffectTargetType.NONE:
                    break;
                case DEFINES.CardEffectTargetType.SELF:
                    sb.Append("Self ");
                    break;
                case DEFINES.CardEffectTargetType.SELECTED:
                    sb.Append("Selected Unit ");
                    break;
                case DEFINES.CardEffectTargetType.ALL:
                    sb.Append("All ");
                    break;
                case DEFINES.CardEffectTargetType.END:
                    sb.Append("End ");
                    break;
            }
            switch (cardEffect.eCardEffectValueType)
            {
                case DEFINES.CardEffectValueType.NONE:
                    break;
                case DEFINES.CardEffectValueType.DAMAGE:
                    sb.Append("Damage ");
                    break;
                case DEFINES.CardEffectValueType.HEAL:
                    sb.Append("Heal ");
                    break;
                case DEFINES.CardEffectValueType.SHIELD:
                    sb.Append("Shield ");
                    break;
                case DEFINES.CardEffectValueType.BUFF:
                    sb.Append("Buff ");
                    break;
                case DEFINES.CardEffectValueType.DEBUFF:
                    sb.Append("Debuff ");
                    break;
                case DEFINES.CardEffectValueType.END:
                    sb.Append("End ");
                    break;
            }
            sb.AppendFormat("by {0} points.", cardEffect.iCardEffectValue);
            if (cardEffect.bCardEffectOptional)
            {
                sb.AppendFormat(" (Optional: {0} points)", cardEffect.iCardEffectOptionalValue);
            }
            sb.AppendLine();
        }
        strCardDescription = sb.ToString();
    }
}
