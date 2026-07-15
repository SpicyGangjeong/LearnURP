using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using Logic.Card;
using Core.Job;
using System;

namespace Core
{
    namespace Deck
    {
        public delegate void CardDrawn(CardInstance pCard);
        public delegate void CardPlayed(CardInstance pCard);
        public delegate void CardDiscarded(CardInstance pCard);
        public delegate void CardReturned(CardInstance pCard);
        public delegate void CardDisappeared(CardInstance pCard);
        public delegate void DeckShuffled();
        public delegate void TurnEnded();
        public delegate void TurnStarted();

        [Serializable]
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

            [SerializeField, Defines.Attribute.ReadOnly]
            List<CardInstance> m_vDeckOriginal = new List<CardInstance>();

            [SerializeField, Defines.Attribute.ReadOnly]
            CardPileCollection m_pPiles = new CardPileCollection();

            public void Initialize(SO.CardInitialSetSO pInitialSetSO)
            {
                IReadOnlyDictionary<CardDataSO, int> vCardInitialSet = pInitialSetSO.GetCardInitialSet();
                foreach (KeyValuePair<CardDataSO, int> pEntry in vCardInitialSet)
                {
                    CardDataSO pCardInfo = pEntry.Key;
                    int iCount = pEntry.Value;
                    for (int i = 0; i < iCount; i++)
                    {
                        CardInstance pCard = new CardInstance(pCardInfo.Instantiate());
                        m_vDeckOriginal.Add(pCard);
                    }
                }
                m_pOnCardDrawn += Defines.Helpers.EmptyEvent;
                m_pOnCardPlayed += Defines.Helpers.EmptyEvent;
                m_pOnCardDiscarded += Defines.Helpers.EmptyEvent;
                m_pOnCardReturned += Defines.Helpers.EmptyEvent;
                m_pOnCardDisappeared += Defines.Helpers.EmptyEvent;
                m_pOnDeckShuffled += Defines.Helpers.EmptyEvent;
                m_pOnTurnEnded += Defines.Helpers.EmptyEvent;
                m_pOnTurnStarted += Defines.Helpers.EmptyEvent;
            }
            public void Initialize(List<CardInstance> vDeckOriginal)
            {
                if (null == vDeckOriginal)
                {
                    Debug.LogError("deckOriginal is null");
                    return;
                }

                m_vDeckOriginal = new List<CardInstance>(vDeckOriginal);
                m_pPiles.ClearAll();
            }

            public void StartGame()
            {
                m_pPiles.ClearAll();
                m_pPiles.AddRange(m_vDeckOriginal, Defines.Enums.CardPile.DECK);
                ShuffleDeck();
                JobQueueManager pJobQueue = CGameInstance.Instance.JobQueues;
                pJobQueue.State = Defines.Helpers.BIT.Set(pJobQueue.State, Job.JobBase.JobStates.JOB_END_TURN);
                CGameInstance.Instance.JobQueues.EnqueueJob(
                    new JobDeferredCallback(async () => { await StartTurn(); }, "Starting_Turn"));
            }

            public async UniTask EndTurn()
            {
                JobQueueManager pJobQueue = CGameInstance.Instance.JobQueues;
                if (true == Defines.Helpers.BIT.Has(pJobQueue.State, Job.JobBase.JobStates.JOB_END_TURN))
                {
                    // Debug.LogError("EndTurn is already in progress.");
                    return;
                }
                pJobQueue.State = Defines.Helpers.BIT.Set(pJobQueue.State, Job.JobBase.JobStates.JOB_END_TURN);

                m_pOnTurnEnded();
                await AwaitingEnd();
            }
            public async UniTask AwaitingEnd()
            {
                JobQueueManager pJobQueue = CGameInstance.Instance.JobQueues;
                Assert.IsTrue(Defines.Helpers.BIT.Has(pJobQueue.State, Job.JobBase.JobStates.JOB_END_TURN));


                if (true == Defines.Helpers.BIT.Has(pJobQueue.State, Job.JobBase.JobStates.JOB_END_TURN) &&
                    0 == pJobQueue.JobCount)
                {// Awaiting Done;
                    CGameInstance.Instance.JobQueues.EnqueueJob(
                        new JobDeferredCallback(async () => { await StartTurn(); }, "Starting_Turn"));
                }
                else
                {// Awaiting
                    CGameInstance.Instance.JobQueues.EnqueueJob(
                        new JobDeferredCallback(async () => { await AwaitingEnd(); }, "Awaiting_End"));
                }
                await UniTask.CompletedTask;
            }
            public async UniTask StartTurn()
            {
                JobQueueManager pJobQueue = CGameInstance.Instance.JobQueues;
                if (false == Defines.Helpers.BIT.Has(pJobQueue.State, Job.JobBase.JobStates.JOB_END_TURN))
                {
                    // Debug.LogError("StartTurn is called without EndTurn.");
                    return;
                }

                pJobQueue.State = Defines.Helpers.BIT.Clear(pJobQueue.State, Job.JobBase.JobStates.JOB_END_TURN);
                m_pOnTurnStarted();
                for (int i = 0; i < s_iHandDrawCount; i++)
                {
                    CardInstance pCard = PopFrontDeck();
                    if (null != pCard)
                    {
                        DrawCard(pCard);
                    }
                }
                await UniTask.CompletedTask;
            }
            public bool PlayCard(CardInstance pCard)
            {
                MoveCard(pCard, Defines.Enums.CardPile.DISCARD);
                m_pOnCardPlayed(pCard);
                pCard.OnPlayCard();
                return true;
            }


            public void ShuffleDeck()
            {
                m_pPiles.Shuffle(Defines.Enums.CardPile.DECK);
                m_pOnDeckShuffled();
            }

            public CardInstance PopFrontDeck()
            {
                CardInstance pCard = null;
                if (0 == m_pPiles.GetCount(Defines.Enums.CardPile.DECK))
                {
                    ReturnCard();
                    if (0 == m_pPiles.GetCount(Defines.Enums.CardPile.DECK))
                    {
                        return pCard;
                    }
                }

                pCard = m_pPiles.GetTopCard(Defines.Enums.CardPile.DECK);
                MoveToFieldCard(pCard);
                return pCard;
            }

            public void ReturnCard()
            {
                List<CardInstance> vDiscardSnapshot = new List<CardInstance>(m_pPiles.GetCards(Defines.Enums.CardPile.DISCARD));
                if (0 == vDiscardSnapshot.Count)
                {
                    return;
                }

                foreach (CardInstance pCard in vDiscardSnapshot)
                {
                    MoveCard(pCard, Defines.Enums.CardPile.DECK);
                }

                foreach (CardInstance pCard in vDiscardSnapshot)
                {
                    m_pOnCardReturned(pCard);
                    pCard.OnReturnCard();
                }

                ShuffleDeck();
            }

            public bool DiscardCard(CardInstance pCard)
            {
                MoveCard(pCard, Defines.Enums.CardPile.DISCARD);
                m_pOnCardDiscarded(pCard);
                pCard.OnDiscardCard();
                return true;
            }

            public bool DrawCard(CardInstance pCard)
            {
                MoveCard(pCard, Defines.Enums.CardPile.HAND);
                m_pOnCardDrawn(pCard);
                pCard.OnDrawCard();
                return true;
            }

            public void DisappearCard(CardInstance pCard)
            {
                MoveCard(pCard, Defines.Enums.CardPile.DISAPPEARED);
                m_pOnCardDisappeared(pCard);
                pCard.OnDisappearCard();
            }

            public void MoveCard(CardInstance pCard, Defines.Enums.CardPile eToPile)
            {
                m_pPiles.Move(pCard, eToPile);
            }
            public void MoveToFieldCard(CardInstance pCard)
            {
                MoveCard(pCard, Defines.Enums.CardPile.FIELD);
            }
            public void AddCard(CardInstance pCard, Defines.Enums.CardPile ePileType)
            {
                m_pPiles.Add(pCard, ePileType);
            }

            public void RemoveCard(CardInstance pCard)
            {
                m_pPiles.Remove(pCard);
            }

            public int GetPileCount(Defines.Enums.CardPile ePileType)
            {
                return m_pPiles.GetCount(ePileType);
            }

            public IReadOnlyList<CardInstance> GetCards(Defines.Enums.CardPile ePileType)
            {
                return m_pPiles.GetCards(ePileType);
            }

            public List<CardInstance> GetPile(Defines.Enums.CardPile ePileType)
            {
                return m_pPiles.GetPile(ePileType);
            }
        }

    }
}