using System.Collections.Generic;
using UnityEngine;

class CardPileCollection
{
    readonly Dictionary<DEFINES.CardPile, List<Card>> piles = new Dictionary<DEFINES.CardPile, List<Card>>();

    public CardPileCollection()
    {
        piles[DEFINES.CardPile.HAND] = new List<Card>();
        piles[DEFINES.CardPile.DISCARD] = new List<Card>();
        piles[DEFINES.CardPile.DECK] = new List<Card>();
        piles[DEFINES.CardPile.DISAPPEARED] = new List<Card>();
    }

    public static bool IsValidPile(DEFINES.CardPile pileType)
    {
        return pileType == DEFINES.CardPile.HAND
            || pileType == DEFINES.CardPile.DISCARD
            || pileType == DEFINES.CardPile.DECK
            || pileType == DEFINES.CardPile.DISAPPEARED;
    }

    public List<Card> GetPile(DEFINES.CardPile pileType)
    {
        if (false == IsValidPile(pileType))
        {
            Debug.LogError($"Invalid card pile type: {pileType}");
            return null;
        }

        return piles[pileType];
    }

    public IReadOnlyList<Card> GetCards(DEFINES.CardPile pileType)
    {
        return GetPile(pileType);
    }

    public int GetCount(DEFINES.CardPile pileType)
    {
        List<Card> pile = GetPile(pileType);
        if (null == pile)
        {
            return 0;
        }

        return pile.Count;
    }

    public void ClearAll()
    {
        foreach (List<Card> pile in piles.Values)
        {
            pile.Clear();
        }
    }

    public void Clear(DEFINES.CardPile pileType)
    {
        List<Card> pile = GetPile(pileType);
        if (null != pile)
        {
            pile.Clear();
        }
    }

    public void Add(Card card, DEFINES.CardPile pileType)
    {
        List<Card> pile = GetPile(pileType);
        if (null != pile)
        {
            pile.Add(card);
        }
    }

    public bool Remove(Card card, DEFINES.CardPile pileType)
    {
        List<Card> pile = GetPile(pileType);
        if (null == pile)
        {
            return false;
        }

        return pile.Remove(card);
    }

    public void Move(Card card, DEFINES.CardPile fromPile, DEFINES.CardPile toPile)
    {
        Remove(card, fromPile);
        Add(card, toPile);
    }

    public void AddRange(IEnumerable<Card> cards, DEFINES.CardPile pileType)
    {
        List<Card> pile = GetPile(pileType);
        if (null != pile)
        {
            pile.AddRange(cards);
        }
    }

    public Card GetTopCard(DEFINES.CardPile pileType)
    {
        List<Card> pile = GetPile(pileType);
        if (null == pile || 0 == pile.Count)
        {
            return null;
        }

        return pile[0];
    }

    public void Shuffle(DEFINES.CardPile pileType)
    {
        List<Card> pile = GetPile(pileType);
        if (null == pile)
        {
            return;
        }

        for (int i = 0; i < pile.Count; i++)
        {
            int randomIndex = Random.Range(0, pile.Count);
            Card tempCard = pile[i];
            pile[i] = pile[randomIndex];
            pile[randomIndex] = tempCard;
        }
    }
}
