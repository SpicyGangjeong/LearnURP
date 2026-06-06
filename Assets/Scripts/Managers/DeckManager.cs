using System.Collections.Generic;
using UnityEngine;

public delegate void DrawCard(Card pCard);
public delegate void PlayCard(Card pCard);
public delegate void DiscardCard(Card pCard);
public delegate void ReturnCard(Card pCard);
public delegate void DisappearCard(Card pCard);
public delegate void ShuffleCard();
public delegate void EndTurn();
class DeckManager
{
    const int s_iHandDrawCount = 5;

    static void EmptyEvent() { }
    static void EmptyEvent(Card pCard) { }

    public event DrawCard m_pOnDrawCard;
    public event PlayCard m_pOnPlayCard;
    public event DiscardCard m_pOnDiscardCard;
    public event ReturnCard m_pOnReturnCard;
    public event DisappearCard m_pOnDisappearCard;
    public event ShuffleCard m_pOnShuffleCard;
    public event EndTurn m_pOnEndTurn;
    List<Card> m_vDeckOriginal = new List<Card>();
    CardPileCollection m_pPiles = new CardPileCollection();

    public void Initialize(CardInitialSetSO pInitialSetSO)
    {
        IReadOnlyDictionary<CardInfo, int> vCardInitialSet = pInitialSetSO.GetCardInitialSet();
        foreach (KeyValuePair<CardInfo, int> pEntry in vCardInitialSet)
        {
            CardInfo pCardInfo = pEntry.Key;
            int iCount = pEntry.Value;
            for (int i = 0; i < iCount; i++)
            {
                Card pCard = new Card(pCardInfo);
                m_vDeckOriginal.Add(pCard);
            }
        }
        m_pOnDrawCard += EmptyEvent;
        m_pOnPlayCard += EmptyEvent;
        m_pOnDiscardCard += EmptyEvent;
        m_pOnReturnCard += EmptyEvent;
        m_pOnDisappearCard += EmptyEvent;
        m_pOnShuffleCard += EmptyEvent;
        m_pOnEndTurn += EmptyEvent;
    }
    public void Initialize(List<Card> vDeckOriginal)
    {
        if (null == vDeckOriginal)
        {
            Debug.LogError("deckOriginal is null");
            return;
        }

        m_vDeckOriginal = new List<Card>(vDeckOriginal);
        m_pPiles.ClearAll();
    }

    public void StartGame()
    {
        m_pPiles.ClearAll();
        m_pPiles.AddRange(m_vDeckOriginal, DEFINES.CardPile.DECK);
        ShuffleDeck();
        DrawCard(s_iHandDrawCount);
    }

    public void EndTurn(int iDrawCount = s_iHandDrawCount)
    {
        DiscardAllHand();
        DrawCard(iDrawCount);
        m_pOnEndTurn();
    }

    void DiscardAllHand()
    {
        List<Card> vHandSnapshot = new List<Card>(m_pPiles.GetCards(DEFINES.CardPile.HAND));
        foreach (Card pCard in vHandSnapshot)
        {
            MoveCard(pCard, DEFINES.CardPile.HAND, DEFINES.CardPile.DISCARD);
            m_pOnDiscardCard(pCard);
        }
    }
    public bool PlayCard(Card pCard)
    {
        if (false == IsInHand(pCard))
        {
            return false;
        }

        MoveCard(pCard, DEFINES.CardPile.HAND, DEFINES.CardPile.DISCARD);
        m_pOnPlayCard(pCard);
        return true;
    }

    public IReadOnlyList<Card> GetCards(DEFINES.CardPile iPileType)
    {
        return m_pPiles.GetCards(iPileType);
    }

    public void ShuffleDeck()
    {
        m_pPiles.Shuffle(DEFINES.CardPile.DECK);
        m_pOnShuffleCard();
    }

    public void DrawCard(int iCount)
    {
        for (int i = 0; i < iCount; i++)
        {
            if (0 == m_pPiles.GetCount(DEFINES.CardPile.DECK))
            {
                ReCycleCard();
                if (0 == m_pPiles.GetCount(DEFINES.CardPile.DECK))
                {
                    return;
                }
            }

            Card pCard = m_pPiles.GetTopCard(DEFINES.CardPile.DECK);
            if (null == pCard)
            {
                return;
            }

            MoveCard(pCard, DEFINES.CardPile.DECK, DEFINES.CardPile.HAND);
            m_pOnDrawCard(pCard);
        }
    }

    public void ReCycleCard()
    {
        List<Card> vDiscardSnapshot = new List<Card>(m_pPiles.GetCards(DEFINES.CardPile.DISCARD));
        if (0 == vDiscardSnapshot.Count)
        {
            return;
        }

        foreach (Card pCard in vDiscardSnapshot)
        {
            MoveCard(pCard, DEFINES.CardPile.DISCARD, DEFINES.CardPile.DECK);
        }

        foreach (Card pCard in vDiscardSnapshot)
        {
            m_pOnReturnCard(pCard);
        }

        ShuffleDeck();
    }

    public bool DiscardCard(Card pCard)
    {
        if (false == IsInHand(pCard))
        {
            return false;
        }

        MoveCard(pCard, DEFINES.CardPile.HAND, DEFINES.CardPile.DISCARD);
        m_pOnDiscardCard(pCard);
        return true;
    }

    public void DisappearCard(Card pCard)
    {
        MoveCard(pCard, DEFINES.CardPile.HAND, DEFINES.CardPile.DISAPPEARED);
        m_pOnDisappearCard(pCard);
    }

    public void MoveCard(Card pCard, DEFINES.CardPile iFromPile, DEFINES.CardPile iToPile)
    {
        m_pPiles.Move(pCard, iFromPile, iToPile);
    }

    public void AddCard(Card pCard, DEFINES.CardPile iPileType)
    {
        m_pPiles.Add(pCard, iPileType);
    }

    public void RemoveCard(Card pCard, DEFINES.CardPile iPileType)
    {
        m_pPiles.Remove(pCard, iPileType);
    }

    public int GetPileCount(DEFINES.CardPile iPileType)
    {
        return m_pPiles.GetCount(iPileType);
    }

    public IReadOnlyList<Card> GetPileCards(DEFINES.CardPile iPileType)
    {
        return m_pPiles.GetCards(iPileType);
    }

    bool IsInHand(Card pCard)
    {
        if (null == pCard)
        {
            return false;
        }

        foreach (Card pHandCard in m_pPiles.GetCards(DEFINES.CardPile.HAND))
        {
            if (pHandCard == pCard)
            {
                return true;
            }
        }

        return false;
    }
}
