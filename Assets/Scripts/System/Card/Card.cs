using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using System.Text;


public class Card
{
    CardInfo m_pCardInfo = null;
    public CardInfo CardInfo => m_pCardInfo;

    public Card(CardInfo pInfo)
    {
        m_pCardInfo = new CardInfo(pInfo);
        BuildCardEffect();
    }
    public Card(int iCardID = 0)
    {
        m_pCardInfo = CGameInstance.Instance.GetCardInfo(iCardID);
        if (null == m_pCardInfo)
        {
            Debug.LogError($"Card not found in CardDocuments: {iCardID}");
            return;
        }
    }

    public event DrawCard m_pOnDrawCard;
    public event PlayCard m_pOnPlayCard;
    public event DiscardCard m_pOnDiscardCard;
    public event ReturnCard m_pOnReturnCard;
    public event DisappearCard m_pOnDisappearCard;
    public event ShuffleCard m_pOnShuffleCard;

    public void OnDrawCard()
    {
        m_pOnDrawCard.Invoke(this);
    }
    public void OnPlayCard()
    {
        m_pOnPlayCard.Invoke(this);
    }
    public void OnDiscardCard()
    {
        m_pOnDiscardCard.Invoke(this);
    }
    public void OnReturnCard()
    {
        m_pOnReturnCard.Invoke(this);
    }
    public void OnDisappearCard()
    {
        m_pOnDisappearCard.Invoke(this);
    }
    public void OnShuffleCard()
    {
        m_pOnShuffleCard.Invoke();
    }
    private void EmptyEvent()
    {
        EmptyEvent(null);
    }
    private void EmptyEvent(Card card)
    {
    }
    private void TriggerEvent()
    {
        TriggerEvent(null);
    }
    private void TriggerEvent(Card card)
    {
        Debug.Log($"Event triggered for card: {CardInfo.m_strCardName}");
        foreach (CardEffect pCardEffect in CardInfo.m_vCardEffects)
        {
            Debug.Log($"Card Effect: TriggerType={pCardEffect.m_iCardEffectTriggerType}, " +
                                     $"TargetType={pCardEffect.m_iCardEffectTargetType}, " +
                                     $"ValueType={pCardEffect.m_iCardEffectValueType}, " +
                                     $"Value={pCardEffect.m_iCardEffectValue}, " +
                                     $"Optional={pCardEffect.m_bCardEffectOptional}, " +
                                     $"OptionalValue={pCardEffect.m_iCardEffectOptionalValue}");
        }
    }
    public void BuildCardEffect()
    {
        m_pOnDrawCard += EmptyEvent;
        m_pOnPlayCard += EmptyEvent;
        m_pOnDiscardCard += EmptyEvent;
        m_pOnReturnCard += EmptyEvent;
        m_pOnDisappearCard += EmptyEvent;
        m_pOnShuffleCard += EmptyEvent;

        foreach (CardEffect pCardEffect in CardInfo.m_vCardEffects)
        {
            switch (pCardEffect.m_iCardEffectTriggerType)
            {
                case DEFINES.ENUMS.CardEffectTriggerType.NONE:
                    break;
                case DEFINES.ENUMS.CardEffectTriggerType.DRAW:
                    m_pOnDrawCard += TriggerEvent;
                    break;
                case DEFINES.ENUMS.CardEffectTriggerType.PLAY:
                    m_pOnPlayCard += TriggerEvent;
                    break;
                case DEFINES.ENUMS.CardEffectTriggerType.DISCARD:
                    m_pOnDiscardCard += TriggerEvent;
                    break;
                case DEFINES.ENUMS.CardEffectTriggerType.RETURN:
                    m_pOnReturnCard += TriggerEvent;
                    break;
                case DEFINES.ENUMS.CardEffectTriggerType.DISAPPEAR:
                    m_pOnDisappearCard += TriggerEvent;
                    break;
                case DEFINES.ENUMS.CardEffectTriggerType.SHUFFLE:
                    m_pOnShuffleCard += TriggerEvent;
                    break;
                case DEFINES.ENUMS.CardEffectTriggerType.END:
                    break;
            }
        }
    }
}

[Serializable]
public class CardEffect
{
    public DEFINES.ENUMS.CardEffectTriggerType m_iCardEffectTriggerType;
    public DEFINES.ENUMS.CardEffectTargetType m_iCardEffectTargetType;
    public DEFINES.ENUMS.CardEffectValueType m_iCardEffectValueType;
    public int m_iCardEffectValue;
    public bool m_bCardEffectOptional;
    public int m_iCardEffectOptionalValue;
}

[Serializable]
public class CardInfo
{
    public int m_iCardID;
    public DEFINES.ENUMS.CardType m_iCardType;
    public int m_iCardCost;
    public string m_strCardName;
    [SerializeField] public List<CardEffect> m_vCardEffects = new List<CardEffect>();
    public string m_strCardDescription;

    public CardInfo() { }
    public CardInfo(CardInfo pInfo)
    {
        m_iCardID = pInfo.m_iCardID;
        m_iCardType = pInfo.m_iCardType;
        m_iCardCost = pInfo.m_iCardCost;
        m_strCardName = pInfo.m_strCardName;
        m_vCardEffects = new List<CardEffect>(pInfo.m_vCardEffects);
        m_strCardDescription = pInfo.m_strCardDescription;
    }


    public void BuildCardDescription()
    {
        StringBuilder pSb = new StringBuilder();
        foreach (CardEffect pCardEffect in m_vCardEffects)
        {
            pSb.Append("When ");
            switch (pCardEffect.m_iCardEffectTriggerType)
            {
                case DEFINES.ENUMS.CardEffectTriggerType.NONE:
                    break;
                case DEFINES.ENUMS.CardEffectTriggerType.DRAW:
                    pSb.Append("Draw");
                    break;
                case DEFINES.ENUMS.CardEffectTriggerType.PLAY:
                    pSb.Append("Play");
                    break;
                case DEFINES.ENUMS.CardEffectTriggerType.DISCARD:
                    pSb.Append("Discard");
                    break;
                case DEFINES.ENUMS.CardEffectTriggerType.RETURN:
                    pSb.Append("Return");
                    break;
                case DEFINES.ENUMS.CardEffectTriggerType.DISAPPEAR:
                    pSb.Append("Disappear");
                    break;
                case DEFINES.ENUMS.CardEffectTriggerType.SHUFFLE:
                    pSb.Append("Shuffle");
                    break;
                case DEFINES.ENUMS.CardEffectTriggerType.END:
                    pSb.Append("End");
                    break;
            }
            pSb.Append(", ");
            switch (pCardEffect.m_iCardEffectTargetType)
            {
                case DEFINES.ENUMS.CardEffectTargetType.NONE:
                    break;
                case DEFINES.ENUMS.CardEffectTargetType.SELF:
                    pSb.Append("Self ");
                    break;
                case DEFINES.ENUMS.CardEffectTargetType.SELECTED:
                    pSb.Append("Selected Unit ");
                    break;
                case DEFINES.ENUMS.CardEffectTargetType.ALL:
                    pSb.Append("All ");
                    break;
                case DEFINES.ENUMS.CardEffectTargetType.END:
                    pSb.Append("End ");
                    break;
            }
            switch (pCardEffect.m_iCardEffectValueType)
            {
                case DEFINES.ENUMS.CardEffectValueType.NONE:
                    break;
                case DEFINES.ENUMS.CardEffectValueType.DAMAGE:
                    pSb.Append("Damage ");
                    break;
                case DEFINES.ENUMS.CardEffectValueType.HEAL:
                    pSb.Append("Heal ");
                    break;
                case DEFINES.ENUMS.CardEffectValueType.SHIELD:
                    pSb.Append("Shield ");
                    break;
                case DEFINES.ENUMS.CardEffectValueType.BUFF:
                    pSb.Append("Buff ");
                    break;
                case DEFINES.ENUMS.CardEffectValueType.DEBUFF:
                    pSb.Append("Debuff ");
                    break;
                case DEFINES.ENUMS.CardEffectValueType.END:
                    pSb.Append("End ");
                    break;
            }
            pSb.AppendFormat("by {0} points.", pCardEffect.m_iCardEffectValue);
            if (pCardEffect.m_bCardEffectOptional)
            {
                pSb.AppendFormat(" (Optional: {0} points)", pCardEffect.m_iCardEffectOptionalValue);
            }
            pSb.AppendLine();
        }
        m_strCardDescription = pSb.ToString();
    }
}
