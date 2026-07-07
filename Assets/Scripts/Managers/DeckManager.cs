using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public delegate void CardDrawn(Card pCard);
public delegate void CardPlayed(Card pCard);
public delegate void CardDiscarded(Card pCard);
public delegate void CardReturned(Card pCard);
public delegate void CardDisappeared(Card pCard);
public delegate void DeckShuffled();
public delegate void TurnEnded();
public delegate void TurnStarted();

public class DeckManager
{
    const int s_iHandDrawCount = 5;

    public event CardDrawn m_pOnCardDrawn;
    public event CardPlayed m_pOnCardPlayed;
    public event CardDiscarded m_pOnCardDiscarded;
    public event CardReturned m_pOnCardReturned;
    public event CardDisappeared m_pOnCardDisappeared;
    public event DeckShuffled m_pOnDeckShuffled;
    public event TurnEnded m_pOnTurnEnded;
    public event TurnStarted m_pOnTurnStarted;
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
        m_pOnCardDrawn += DEFINES.HELPERS.EmptyEvent;
        m_pOnCardPlayed += DEFINES.HELPERS.EmptyEvent;
        m_pOnCardDiscarded += DEFINES.HELPERS.EmptyEvent;
        m_pOnCardReturned += DEFINES.HELPERS.EmptyEvent;
        m_pOnCardDisappeared += DEFINES.HELPERS.EmptyEvent;
        m_pOnDeckShuffled += DEFINES.HELPERS.EmptyEvent;
        m_pOnTurnEnded += DEFINES.HELPERS.EmptyEvent;
        m_pOnTurnStarted += DEFINES.HELPERS.EmptyEvent;
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
        JobQueueManager pJobQueue = CGameInstance.Instance.JobQueues;
        pJobQueue.State = DEFINES.HELPERS.BIT.Set(pJobQueue.State, DEFINES.ENUMS.JobStates.JOB_END_TURN);
        CGameInstance.Instance.JobQueues.EnqueueJob(
            new JobDeferredCallback(async () => { await StartTurn(); }));
    }

    public async UniTask EndTurn()
    {
        JobQueueManager pJobQueue = CGameInstance.Instance.JobQueues;
        if (true == DEFINES.HELPERS.BIT.Has(pJobQueue.State, DEFINES.ENUMS.JobStates.JOB_END_TURN))
        {
            // Debug.LogError("EndTurn is already in progress.");
            return;
        }
        pJobQueue.State = DEFINES.HELPERS.BIT.Set(pJobQueue.State, DEFINES.ENUMS.JobStates.JOB_END_TURN);

        m_pOnTurnEnded();
        await AwaitingEnd();
    }
    public async UniTask AwaitingEnd()
    {
        JobQueueManager pJobQueue = CGameInstance.Instance.JobQueues;
        Assert.IsTrue(DEFINES.HELPERS.BIT.Has(pJobQueue.State, DEFINES.ENUMS.JobStates.JOB_END_TURN));


        if (true == DEFINES.HELPERS.BIT.Has(pJobQueue.State, DEFINES.ENUMS.JobStates.JOB_END_TURN) &&
            0 == pJobQueue.JobCount)
        {// Awaiting Done;
            CGameInstance.Instance.JobQueues.EnqueueJob(
                new JobDeferredCallback(async () => { await StartTurn(); }));
        } else
        {// Awaiting
            CGameInstance.Instance.JobQueues.EnqueueJob(
                new JobDeferredCallback(async () => { await AwaitingEnd(); }));
        }
    }
    public async UniTask StartTurn()
    {
        JobQueueManager pJobQueue = CGameInstance.Instance.JobQueues;
        if (false == DEFINES.HELPERS.BIT.Has(pJobQueue.State, DEFINES.ENUMS.JobStates.JOB_END_TURN))
        {
            // Debug.LogError("StartTurn is called without EndTurn.");
            return;
        }

        pJobQueue.State = DEFINES.HELPERS.BIT.Clear(pJobQueue.State, DEFINES.ENUMS.JobStates.JOB_END_TURN);
        m_pOnTurnStarted();
        for (int i = 0; i < s_iHandDrawCount; i++)
        {
            Card pCard = PopFrontDeck();
            if (null != pCard)
            {
                DrawCard(pCard);
            }
        }
        await UniTask.CompletedTask;
    }
    public bool PlayCard(Card pCard)
    {
        MoveCard(pCard, DEFINES.ENUMS.CardPile.DISCARD);
        m_pOnCardPlayed(pCard);
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
        m_pOnDeckShuffled();
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
            m_pOnCardReturned(pCard);
            pCard.OnReturnCard();
        }

        ShuffleDeck();
    }

    public bool DiscardCard(Card pCard)
    {
        MoveCard(pCard, DEFINES.ENUMS.CardPile.DISCARD);
        m_pOnCardDiscarded(pCard);
        pCard.OnDiscardCard();
        return true;
    }

    public bool DrawCard(Card pCard)
    {
        MoveCard(pCard, DEFINES.ENUMS.CardPile.HAND);
        m_pOnCardDrawn(pCard);
        pCard.OnDrawCard();
        return true;
    }

    public void DisappearCard(Card pCard)
    {
        MoveCard(pCard, DEFINES.ENUMS.CardPile.DISAPPEARED);
        m_pOnCardDisappeared(pCard);
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
