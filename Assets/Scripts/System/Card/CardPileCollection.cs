using System.Collections.Generic;
using UnityEngine;

class CardPileCollection
{
    readonly Dictionary<DEFINES.CardPile, List<Card>> m_vPiles = new Dictionary<DEFINES.CardPile, List<Card>>();
    
    
    public CardPileCollection()
    {
        m_vPiles[DEFINES.CardPile.HAND] = new List<Card>();
        m_vPiles[DEFINES.CardPile.DISCARD] = new List<Card>();
        m_vPiles[DEFINES.CardPile.DECK] = new List<Card>();
        m_vPiles[DEFINES.CardPile.DISAPPEARED] = new List<Card>();
    }

    public static bool IsValidPile(DEFINES.CardPile iPileType)
    {
        return iPileType == DEFINES.CardPile.HAND
            || iPileType == DEFINES.CardPile.DISCARD
            || iPileType == DEFINES.CardPile.DECK
            || iPileType == DEFINES.CardPile.DISAPPEARED;
    }

    public List<Card> GetPile(DEFINES.CardPile iPileType)
    {
        if (false == IsValidPile(iPileType))
        {
            Debug.LogError($"Invalid card pile type: {iPileType}");
            return null;
        }

        return m_vPiles[iPileType];
    }

    public IReadOnlyList<Card> GetCards(DEFINES.CardPile iPileType)
    {
        return GetPile(iPileType);
    }

    public int GetCount(DEFINES.CardPile iPileType)
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

    public void Clear(DEFINES.CardPile iPileType)
    {
        List<Card> vPile = GetPile(iPileType);
        if (null != vPile)
        {
            vPile.Clear();
        }
    }

    public void Add(Card pCard, DEFINES.CardPile iPileType)
    {
        List<Card> vPile = GetPile(iPileType);
        if (null != vPile)
        {
            vPile.Add(pCard);
        }
    }

    public bool Remove(Card pCard, DEFINES.CardPile iPileType)
    {
        List<Card> vPile = GetPile(iPileType);
        if (null == vPile)
        {
            return false;
        }

        return vPile.Remove(pCard);
    }

    public bool Move(Card pCard, DEFINES.CardPile iFromPile, DEFINES.CardPile iToPile)
    {
        if (false == Remove(pCard, iFromPile))
        {
            return false;
        }

        Add(pCard, iToPile);
        return true;
    }

    public void AddRange(IEnumerable<Card> vCards, DEFINES.CardPile iPileType)
    {
        List<Card> vPile = GetPile(iPileType);
        if (null != vPile)
        {
            vPile.AddRange(vCards);
        }
    }

    public Card GetTopCard(DEFINES.CardPile iPileType)
    {
        List<Card> vPile = GetPile(iPileType);
        if (null == vPile || 0 == vPile.Count)
        {
            return null;
        }

        return vPile[0];
    }

    public void Shuffle(DEFINES.CardPile iPileType)
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
    }
}
