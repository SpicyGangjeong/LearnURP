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
}

[Serializable]
public class CardEffect
{
    [FormerlySerializedAs("eCardEffectTriggerType")]
    public DEFINES.CardEffectTriggerType m_iCardEffectTriggerType;
    [FormerlySerializedAs("eCardEffectTargetType")]
    public DEFINES.CardEffectTargetType m_iCardEffectTargetType;
    [FormerlySerializedAs("eCardEffectValueType")]
    public DEFINES.CardEffectValueType m_iCardEffectValueType;
    [FormerlySerializedAs("iCardEffectValue")]
    public int m_iCardEffectValue;
    [FormerlySerializedAs("bCardEffectOptional")]
    public bool m_bCardEffectOptional;
    [FormerlySerializedAs("iCardEffectOptionalValue")]
    public int m_iCardEffectOptionalValue;
}

[Serializable]
public class CardInfo
{
    [FormerlySerializedAs("iCardID")]
    public int m_iCardID;
    [FormerlySerializedAs("eCardType")]
    public DEFINES.CardType m_iCardType;
    [FormerlySerializedAs("iCardCost")]
    public int m_iCardCost;
    [FormerlySerializedAs("strCardName")]
    public string m_strCardName;
    [FormerlySerializedAs("listCardEffects")]
    [SerializeField] public List<CardEffect> m_vCardEffects = new List<CardEffect>();
    [FormerlySerializedAs("strCardDescription")]
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
                case DEFINES.CardEffectTriggerType.NONE:
                    break;
                case DEFINES.CardEffectTriggerType.DRAW:
                    pSb.Append("Draw");
                    break;
                case DEFINES.CardEffectTriggerType.PLAY:
                    pSb.Append("Play");
                    break;
                case DEFINES.CardEffectTriggerType.DISCARD:
                    pSb.Append("Discard");
                    break;
                case DEFINES.CardEffectTriggerType.RETURN:
                    pSb.Append("Return");
                    break;
                case DEFINES.CardEffectTriggerType.DISAPPEAR:
                    pSb.Append("Disappear");
                    break;
                case DEFINES.CardEffectTriggerType.SHUFFLE:
                    pSb.Append("Shuffle");
                    break;
                case DEFINES.CardEffectTriggerType.END:
                    pSb.Append("End");
                    break;
            }
            pSb.Append(", ");
            switch (pCardEffect.m_iCardEffectTargetType)
            {
                case DEFINES.CardEffectTargetType.NONE:
                    break;
                case DEFINES.CardEffectTargetType.SELF:
                    pSb.Append("Self ");
                    break;
                case DEFINES.CardEffectTargetType.SELECTED:
                    pSb.Append("Selected Unit ");
                    break;
                case DEFINES.CardEffectTargetType.ALL:
                    pSb.Append("All ");
                    break;
                case DEFINES.CardEffectTargetType.END:
                    pSb.Append("End ");
                    break;
            }
            switch (pCardEffect.m_iCardEffectValueType)
            {
                case DEFINES.CardEffectValueType.NONE:
                    break;
                case DEFINES.CardEffectValueType.DAMAGE:
                    pSb.Append("Damage ");
                    break;
                case DEFINES.CardEffectValueType.HEAL:
                    pSb.Append("Heal ");
                    break;
                case DEFINES.CardEffectValueType.SHIELD:
                    pSb.Append("Shield ");
                    break;
                case DEFINES.CardEffectValueType.BUFF:
                    pSb.Append("Buff ");
                    break;
                case DEFINES.CardEffectValueType.DEBUFF:
                    pSb.Append("Debuff ");
                    break;
                case DEFINES.CardEffectValueType.END:
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
