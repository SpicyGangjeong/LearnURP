using System.Collections.Generic;
using UnityEngine;

class CardPileCollection
{
    readonly Dictionary<Defines.Enums.CardPile, List<Card>> m_vPiles = new Dictionary<Defines.Enums.CardPile, List<Card>>();
    
    
    public CardPileCollection()
    {
        for (Defines.Enums.CardPile ePile = Defines.Enums.CardPile.NONE + 1; 
            ePile != Defines.Enums.CardPile.ALL; ePile++)
        {
            m_vPiles[ePile] = new List<Card>();
        }
    }
    public List<Card> GetPile(Defines.Enums.CardPile ePileType)
    {
        return m_vPiles[ePileType];
    }

    public IReadOnlyList<Card> GetCards(Defines.Enums.CardPile ePileType)
    {
        return GetPile(ePileType);
    }

    public int GetCount(Defines.Enums.CardPile ePileType)
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

    public void Clear(Defines.Enums.CardPile ePileType)
    {
        List<Card> vPile = GetPile(ePileType);
        if (null != vPile)
        {
            vPile.Clear();
        }
    }

    public void Add(Card pCard, Defines.Enums.CardPile ePileType)
    {
        List<Card> vPile = GetPile(ePileType);
        pCard.CardInfo.m_eCurrentPile = ePileType;

        if (null != vPile)
        {
            vPile.Add(pCard);
        }
    }
    public void AddRange(IEnumerable<Card> vCards, Defines.Enums.CardPile ePileType)
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
            pCard.CardInfo.m_eCurrentPile = Defines.Enums.CardPile.END;
            return false;
        }

        return vPile.Remove(pCard);
    }

    public bool Move(Card pCard, Defines.Enums.CardPile eToPile)
    {
        if (false == Remove(pCard))
        {
            return false;
        }

        Add(pCard, eToPile);
        return true;
    }


    public Card GetTopCard(Defines.Enums.CardPile ePileType)
    {
        List<Card> vPile = GetPile(ePileType);
        if (null == vPile || 0 == vPile.Count)
        {
            return null;
        }

        return vPile[0];
    }

    public void Shuffle(Defines.Enums.CardPile ePileType)
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
