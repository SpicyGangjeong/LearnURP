using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using DEFINES;
using DEFINES.ENUMS;
using DEFINES.STRUCTURES;
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
    public void BuildCardEffect()
    {
        m_pOnDrawCard += HELPERS.EmptyEvent;
        m_pOnPlayCard += HELPERS.EmptyEvent;
        m_pOnDiscardCard += HELPERS.EmptyEvent;
        m_pOnReturnCard += HELPERS.EmptyEvent;
        m_pOnDisappearCard += HELPERS.EmptyEvent;
        m_pOnShuffleCard += HELPERS.EmptyEvent;

    }
}


[Serializable]
public class CardInfo
{
    public int m_iCardID;
    public CardType m_iCardType;
    public int m_iCardCost;
    public string m_strCardName;
    public CardPile m_eCurrentPile = CardPile.END;
    [SerializeField]
    public List<CardEffectBlock> m_vCardEffects = new List<CardEffectBlock>();
    public string m_strCardDescription;

    public CardInfo() { }
    public CardInfo(CardInfo pInfo)
    {
        m_iCardID = pInfo.m_iCardID;
        m_iCardType = pInfo.m_iCardType;
        m_iCardCost = pInfo.m_iCardCost;
        m_strCardName = pInfo.m_strCardName;
        m_vCardEffects = new List<CardEffectBlock>(pInfo.m_vCardEffects);
        m_strCardDescription = pInfo.m_strCardDescription;
    }


    public void BuildCardDescription()
    {
        StringBuilder pSb = new StringBuilder();
        m_strCardDescription = pSb.ToString();
    }
}
