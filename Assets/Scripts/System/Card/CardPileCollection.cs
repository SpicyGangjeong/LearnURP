using System.Collections.Generic;
using UnityEngine;

class CardPileCollection
{
    readonly Dictionary<DEFINES.ENUMS.CardPile, List<Card>> m_vPiles = new Dictionary<DEFINES.ENUMS.CardPile, List<Card>>();
    
    
    public CardPileCollection()
    {
        m_vPiles[DEFINES.ENUMS.CardPile.HAND] = new List<Card>();
        m_vPiles[DEFINES.ENUMS.CardPile.DISCARD] = new List<Card>();
        m_vPiles[DEFINES.ENUMS.CardPile.DECK] = new List<Card>();
        m_vPiles[DEFINES.ENUMS.CardPile.DISAPPEARED] = new List<Card>();
    }

    public static bool IsValidPile(DEFINES.ENUMS.CardPile iPileType)
    {
        return iPileType == DEFINES.ENUMS.CardPile.HAND
            || iPileType == DEFINES.ENUMS.CardPile.DISCARD
            || iPileType == DEFINES.ENUMS.CardPile.DECK
            || iPileType == DEFINES.ENUMS.CardPile.DISAPPEARED;
    }

    public List<Card> GetPile(DEFINES.ENUMS.CardPile iPileType)
    {
        if (false == IsValidPile(iPileType))
        {
            Debug.LogError($"Invalid card pile type: {iPileType}");
            return null;
        }

        return m_vPiles[iPileType];
    }

    public IReadOnlyList<Card> GetCards(DEFINES.ENUMS.CardPile iPileType)
    {
        return GetPile(iPileType);
    }

    public int GetCount(DEFINES.ENUMS.CardPile iPileType)
    {
        List<Card> vPile = GetPile(iPileType);
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

    public void Clear(DEFINES.ENUMS.CardPile iPileType)
    {
        List<Card> vPile = GetPile(iPileType);
        if (null != vPile)
        {
            vPile.Clear();
        }
    }

    public void Add(Card pCard, DEFINES.ENUMS.CardPile iPileType)
    {
        List<Card> vPile = GetPile(iPileType);
        if (null != vPile)
        {
            vPile.Add(pCard);
        }
    }

    public bool Remove(Card pCard, DEFINES.ENUMS.CardPile iPileType)
    {
        List<Card> vPile = GetPile(iPileType);
        if (null == vPile)
        {
            return false;
        }

        return vPile.Remove(pCard);
    }

    public bool Move(Card pCard, DEFINES.ENUMS.CardPile iFromPile, DEFINES.ENUMS.CardPile iToPile)
    {
        if (false == Remove(pCard, iFromPile))
        {
            return false;
        }

        Add(pCard, iToPile);
        return true;
    }

    public void AddRange(IEnumerable<Card> vCards, DEFINES.ENUMS.CardPile iPileType)
    {
        List<Card> vPile = GetPile(iPileType);
        if (null != vPile)
        {
            vPile.AddRange(vCards);
        }
    }

    public Card GetTopCard(DEFINES.ENUMS.CardPile iPileType)
    {
        List<Card> vPile = GetPile(iPileType);
        if (null == vPile || 0 == vPile.Count)
        {
            return null;
        }

        return vPile[0];
    }

    public void Shuffle(DEFINES.ENUMS.CardPile iPileType)
    {
        List<Card> vPile = GetPile(iPileType);
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
