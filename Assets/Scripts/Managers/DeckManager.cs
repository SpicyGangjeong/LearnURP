using System.Collections.Generic;
using UnityEngine;

class DeckManager
{
    List<Card> deckOriginal = new List<Card>();
    CardPileCollection piles = new CardPileCollection();

    public void Initialize(List<Card> deckOriginal)
    {
        if (null == deckOriginal)
        {
            Debug.LogError("deckOriginal is null");
            return;
        }

        this.deckOriginal = new List<Card>(deckOriginal);
        piles.ClearAll();
    }

    public void StartGame()
    {
        piles.ClearAll();
        piles.AddRange(deckOriginal, DEFINES.CardPile.DECK);
        ShuffleDeck();
    }

    public void ShuffleDeck()
    {
        piles.Shuffle(DEFINES.CardPile.DECK);
    }

    public void DrawCard(int count)
    {
        for (int i = 0; i < count; i++)
        {
            if (0 == piles.GetCount(DEFINES.CardPile.DECK))
            {
                ReCycleCard();
                if (0 == piles.GetCount(DEFINES.CardPile.DECK))
                {
                    return;
                }
            }

            Card card = piles.GetTopCard(DEFINES.CardPile.DECK);
            if (null == card)
            {
                return;
            }

            MoveCard(card, DEFINES.CardPile.DECK, DEFINES.CardPile.HAND);
        }
    }

    public void ReCycleCard()
    {
        piles.AddRange(piles.GetCards(DEFINES.CardPile.DISCARD), DEFINES.CardPile.DECK);
        piles.Clear(DEFINES.CardPile.DISCARD);
        ShuffleDeck();
    }

    public void DiscardCard(Card card)
    {
        MoveCard(card, DEFINES.CardPile.HAND, DEFINES.CardPile.DISCARD);
    }

    public void DisappearCard(Card card)
    {
        MoveCard(card, DEFINES.CardPile.HAND, DEFINES.CardPile.DISAPPEARED);
    }

    public void MoveCard(Card card, DEFINES.CardPile fromPile, DEFINES.CardPile toPile)
    {
        piles.Move(card, fromPile, toPile);
    }

    public void AddCard(Card card, DEFINES.CardPile pileType)
    {
        piles.Add(card, pileType);
    }

    public void RemoveCard(Card card, DEFINES.CardPile pileType)
    {
        piles.Remove(card, pileType);
    }

    public int GetPileCount(DEFINES.CardPile pileType)
    {
        return piles.GetCount(pileType);
    }

    public IReadOnlyList<Card> GetPileCards(DEFINES.CardPile pileType)
    {
        return piles.GetCards(pileType);
    }
}
