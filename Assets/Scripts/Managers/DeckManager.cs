using System.Collections.Generic;
using UnityEngine;

public delegate void DrawCard(Card card);
public delegate void PlayCard(Card card);
public delegate void DiscardCard(Card card);
public delegate void ReturnCard(Card card);
public delegate void DisappearCard(Card card);
public delegate void ShuffleCard();
public delegate void EndTurn();
class DeckManager
{
    const int HAND_DRAW_COUNT = 5;

    static void EmptyEvent() { }
    static void EmptyEvent(Card card) { }

    public event DrawCard OnDrawCard;
    public event PlayCard OnPlayCard;
    public event DiscardCard OnDiscardCard;
    public event ReturnCard OnReturnCard;
    public event DisappearCard OnDisappearCard;
    public event ShuffleCard OnShuffleCard;
    public event EndTurn OnEndTurn;
    List<Card> deckOriginal = new List<Card>();
    CardPileCollection piles = new CardPileCollection();

    public void Initialize(CardInitialSetSO initialSetSO)
    {
        IReadOnlyDictionary<CardInfo, int> cardInitialSet = initialSetSO.GetCardInitialSet();
        foreach (KeyValuePair<CardInfo, int> entry in cardInitialSet)
        {
            CardInfo cardInfo = entry.Key;
            int count = entry.Value;
            for (int i = 0; i < count; i++)
            {
                Card card = new Card(cardInfo);
                deckOriginal.Add(card);
            }
        }
        OnDrawCard += EmptyEvent;
        OnPlayCard += EmptyEvent;
        OnDiscardCard += EmptyEvent;
        OnReturnCard += EmptyEvent;
        OnDisappearCard += EmptyEvent;
        OnShuffleCard += EmptyEvent;
        OnEndTurn += EmptyEvent;
    }
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
        DrawCard(HAND_DRAW_COUNT);
    }

    public void EndTurn(int drawCount = HAND_DRAW_COUNT)
    {
        DiscardAllHand();
        DrawCard(drawCount);
        OnEndTurn();
    }

    void DiscardAllHand()
    {
        List<Card> handSnapshot = new List<Card>(piles.GetCards(DEFINES.CardPile.HAND));
        foreach (Card card in handSnapshot)
        {
            MoveCard(card, DEFINES.CardPile.HAND, DEFINES.CardPile.DISCARD);
            OnDiscardCard(card);
        }
    }
    public bool PlayCard(Card card)
    {
        if (false == IsInHand(card))
        {
            return false;
        }

        MoveCard(card, DEFINES.CardPile.HAND, DEFINES.CardPile.DISCARD);
        OnPlayCard(card);
        return true;
    }

    public IReadOnlyList<Card> GetCards(DEFINES.CardPile pileType)
    {
        return piles.GetCards(pileType);
    }

    public void ShuffleDeck()
    {
        piles.Shuffle(DEFINES.CardPile.DECK);
        OnShuffleCard();
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
            OnDrawCard(card);
        }
    }

    public void ReCycleCard()
    {
        List<Card> cards = new List<Card>(piles.GetPile(DEFINES.CardPile.DISCARD));
        foreach (Card card in cards)
        {
            MoveCard(card, DEFINES.CardPile.DISCARD, DEFINES.CardPile.DECK);
            OnReturnCard(card);
        }
        ShuffleDeck();
    }

    public bool DiscardCard(Card card)
    {
        if (false == IsInHand(card))
        {
            return false;
        }

        MoveCard(card, DEFINES.CardPile.HAND, DEFINES.CardPile.DISCARD);
        OnDiscardCard(card);
        return true;
    }

    public void DisappearCard(Card card)
    {
        MoveCard(card, DEFINES.CardPile.HAND, DEFINES.CardPile.DISAPPEARED);
        OnDisappearCard(card);
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

    bool IsInHand(Card card)
    {
        if (null == card)
        {
            return false;
        }

        foreach (Card handCard in piles.GetCards(DEFINES.CardPile.HAND))
        {
            if (handCard == card)
            {
                return true;
            }
        }

        return false;
    }
}
