using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
class DeckManager
{
    const int s_iHandDrawCount = 5;

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
        m_pOnDrawCard += DEFINES.HELPERS.EmptyEvent;
        m_pOnPlayCard += DEFINES.HELPERS.EmptyEvent;
        m_pOnDiscardCard += DEFINES.HELPERS.EmptyEvent;
        m_pOnReturnCard += DEFINES.HELPERS.EmptyEvent;
        m_pOnDisappearCard += DEFINES.HELPERS.EmptyEvent;
        m_pOnShuffleCard += DEFINES.HELPERS.EmptyEvent;
        m_pOnEndTurn += DEFINES.HELPERS.EmptyEvent;
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
        m_pPiles.AddRange(m_vDeckOriginal, DEFINES.ENUMS.CardPile.DECK);
        ShuffleDeck();
        for (int i = 0; i < s_iHandDrawCount; i++)
        {
            CGameInstance.Instance.RequestDrawCard(PopFrontDeck(), null);
        }
    }

    public async UniTask EndTurn()
    {
        CGameInstance.Instance.JobQueues.State = DEFINES.HELPERS.BIT.Toggle(CGameInstance.Instance.JobQueues.State, DEFINES.ENUMS.JobStates.JOB_END_TURN);
        m_pOnEndTurn();
        await UniTask.CompletedTask;
        for (int i = 0; i < s_iHandDrawCount; i++)
        {
            CGameInstance.Instance.RequestDrawCard(PopFrontDeck(), null);
        }
        CGameInstance.Instance.JobQueues.State = DEFINES.HELPERS.BIT.Toggle(CGameInstance.Instance.JobQueues.State, DEFINES.ENUMS.JobStates.JOB_END_TURN);
    }
    public bool PlayCard(Card pCard)
    {
        MoveCard(pCard, DEFINES.ENUMS.CardPile.DISCARD);
        m_pOnPlayCard(pCard);
        pCard.OnPlayCard();
        return true;
    }

    public IReadOnlyList<Card> GetCards(DEFINES.ENUMS.CardPile ePileType)
    {
        return m_pPiles.GetCards(ePileType);
    }

    public void ShuffleDeck()
    {
        m_pPiles.Shuffle(DEFINES.ENUMS.CardPile.DECK);
        m_pOnShuffleCard();
    }

    public Card PopFrontDeck()
    {
        Card pCard = null;
        if (0 == m_pPiles.GetCount(DEFINES.ENUMS.CardPile.DECK))
        {
            ReturnCard();
            if (0 == m_pPiles.GetCount(DEFINES.ENUMS.CardPile.DECK))
            {
                return pCard;
            }
        }

        pCard = m_pPiles.GetTopCard(DEFINES.ENUMS.CardPile.DECK);
        MoveToFieldCard(pCard);
        return pCard;
    }

    public void ReturnCard()
    {
        List<Card> vDiscardSnapshot = new List<Card>(m_pPiles.GetCards(DEFINES.ENUMS.CardPile.DISCARD));
        if (0 == vDiscardSnapshot.Count)
        {
            return;
        }

        foreach (Card pCard in vDiscardSnapshot)
        {
            MoveCard(pCard, DEFINES.ENUMS.CardPile.DECK);
        }

        foreach (Card pCard in vDiscardSnapshot)
        {
            m_pOnReturnCard(pCard);
            pCard.OnReturnCard();
        }

        ShuffleDeck();
    }

    public bool DiscardCard(Card pCard)
    {
        MoveCard(pCard, DEFINES.ENUMS.CardPile.DISCARD);
        m_pOnDiscardCard(pCard);
        pCard.OnDiscardCard();
        return true;
    }

    public bool DrawCard(Card pCard)
    {
        MoveCard(pCard, DEFINES.ENUMS.CardPile.HAND);
        m_pOnDrawCard(pCard);
        pCard.OnDrawCard();
        return true;
    }

    public void DisappearCard(Card pCard)
    {
        MoveCard(pCard, DEFINES.ENUMS.CardPile.DISAPPEARED);
        m_pOnDisappearCard(pCard);
        pCard.OnDisappearCard();
    }

    public void MoveCard(Card pCard, DEFINES.ENUMS.CardPile iToPile)
    {
        m_pPiles.Move(pCard, iToPile);
    }
    public void MoveToFieldCard(Card pCard)
    {
        MoveCard(pCard, DEFINES.ENUMS.CardPile.FIELD);
    }
    public void AddCard(Card pCard, DEFINES.ENUMS.CardPile ePileType)
    {
        m_pPiles.Add(pCard, ePileType);
    }

    public void RemoveCard(Card pCard)
    {
        m_pPiles.Remove(pCard);
    }

    public int GetPileCount(DEFINES.ENUMS.CardPile ePileType)
    {
        return m_pPiles.GetCount(ePileType);
    }

    public IReadOnlyList<Card> GetPileCards(DEFINES.ENUMS.CardPile ePileType)
    {
        return m_pPiles.GetCards(ePileType);
    }

    bool IsInHand(Card pCard)
    {
        if (null == pCard)
        {
            return false;
        }

        foreach (Card pHandCard in m_pPiles.GetCards(DEFINES.ENUMS.CardPile.HAND))
        {
            if (pHandCard == pCard)
            {
                return true;
            }
        }

        return false;
    }
}
