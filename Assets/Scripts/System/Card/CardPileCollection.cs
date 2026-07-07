using System.Collections.Generic;
using UnityEngine;

class CardPileCollection
{
    readonly Dictionary<DEFINES.ENUMS.CardPile, List<Card>> m_vPiles = new Dictionary<DEFINES.ENUMS.CardPile, List<Card>>();
    
    
    public CardPileCollection()
    {
        for (DEFINES.ENUMS.CardPile ePile = DEFINES.ENUMS.CardPile.NONE + 1; 
            ePile != DEFINES.ENUMS.CardPile.ALL; ePile++)
        {
            m_vPiles[ePile] = new List<Card>();
        }
    }
    public List<Card> GetPile(DEFINES.ENUMS.CardPile ePileType)
    {
        return m_vPiles[ePileType];
    }

    public IReadOnlyList<Card> GetCards(DEFINES.ENUMS.CardPile ePileType)
    {
        return GetPile(ePileType);
    }

    public int GetCount(DEFINES.ENUMS.CardPile ePileType)
    {
        List<Card> vPile = GetPile(ePileType);
        if (null == vPile)
        {
            return 0;
        }

        return vPile.Count;
    }

    public void ClearAll()
    {
        foreach (List<Card> vPile in m_vPiles.Values)
        {
            vPile.Clear();
        }
    }

    public void Clear(DEFINES.ENUMS.CardPile ePileType)
    {
        List<Card> vPile = GetPile(ePileType);
        if (null != vPile)
        {
            vPile.Clear();
        }
    }

    public void Add(Card pCard, DEFINES.ENUMS.CardPile ePileType)
    {
        List<Card> vPile = GetPile(ePileType);
        pCard.CardInfo.m_eCurrentPile = ePileType;

        if (null != vPile)
        {
            vPile.Add(pCard);
        }
    }
    public void AddRange(IEnumerable<Card> vCards, DEFINES.ENUMS.CardPile ePileType)
    {
        foreach (Card pCard in vCards)
        {
            Add(pCard, ePileType);
        }
    }

    public bool Remove(Card pCard)
    {
        List<Card> vPile = GetPile(pCard.CardInfo.m_eCurrentPile);
        if (null == vPile)
        {
            pCard.CardInfo.m_eCurrentPile = DEFINES.ENUMS.CardPile.END;
            return false;
        }

        return vPile.Remove(pCard);
    }

    public bool Move(Card pCard, DEFINES.ENUMS.CardPile eToPile)
    {
        if (false == Remove(pCard))
        {
            return false;
        }

        Add(pCard, eToPile);
        return true;
    }


    public Card GetTopCard(DEFINES.ENUMS.CardPile ePileType)
    {
        List<Card> vPile = GetPile(ePileType);
        if (null == vPile || 0 == vPile.Count)
        {
            return null;
        }

        return vPile[0];
    }

    public void Shuffle(DEFINES.ENUMS.CardPile ePileType)
    {
        List<Card> vPile = GetPile(ePileType);
        if (null == vPile)
        {
            return;
        }

        for (int i = 0; i < vPile.Count; i++)
        {
            int iRandomIndex = Random.Range(0, vPile.Count);
            Card pTempCard = vPile[i];
            vPile[i] = vPile[iRandomIndex];
            vPile[iRandomIndex] = pTempCard;
        }
        foreach (Card p in vPile)
        {
            p.OnShuffleCard();
        }
    }
}
