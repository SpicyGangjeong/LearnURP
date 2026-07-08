using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Defines;
using Defines.Structures;
using System.Text;
using Unity.VisualScripting;


public class Card
{
    CardData m_pCardInfo = null;
    public CardData CardInfo => m_pCardInfo;
    public Card(CardData pInfo)
    {
        m_pCardInfo = new CardData(pInfo);
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
        m_pOnDrawCard += Helpers.EmptyEvent;
        m_pOnPlayCard += Helpers.EmptyEvent;
        m_pOnDiscardCard += Helpers.EmptyEvent;
        m_pOnReturnCard += Helpers.EmptyEvent;
        m_pOnDisappearCard += Helpers.EmptyEvent;
        m_pOnShuffleCard += Helpers.EmptyEvent;

    }
}


[Serializable]
public class CardData
{
    public enum CardType : int
    {
        NONE = -1,
        ATTACK = 0,
        DEFENSE = 1,
        MAGIC = 2,
        ITEM = 3,
        END = 4,
    }
    public string m_strCardName;
    public int m_iCardID;
    public CardType m_eCardType;
    public int m_iCardCost;
    public CardEffect m_vCardEffects = new CardEffect();
    public string m_strCardDescription;

    [NonSerialized]
    public Defines.Enums.CardPile m_eCurrentPile = Defines.Enums.CardPile.END;
    public CardData() { }
    public CardData(CardData pInfo)
    {
        m_iCardID = pInfo.m_iCardID;
        m_eCardType = pInfo.m_eCardType;
        m_iCardCost = pInfo.m_iCardCost;
        m_strCardName = pInfo.m_strCardName;
        m_vCardEffects = new CardEffect(pInfo.m_vCardEffects);
        m_strCardDescription = pInfo.m_strCardDescription;
    }
    public void BuildCardDescription()
    {
        StringBuilder pSb = new StringBuilder();
        m_strCardDescription = pSb.ToString();
    }
}
