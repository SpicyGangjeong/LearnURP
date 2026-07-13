using Core;
using Defines;
using System;
using System.Text;
using UnityEngine;

namespace Logic
{
    namespace Card
    {
        public class Card
        {
            CardDataSO m_pCardInfo = null;
            public CardDataSO CardInfo => m_pCardInfo;
            public Card(CardDataSO pInfoOriginal)
            {
                m_pCardInfo = pInfoOriginal.Clone() as CardDataSO;
                BuildCardEffect();
            }
            public event DrawCard m_pOnDrawCard;
            public event PlayCard m_pOnPlayCard;
            public event DiscardCard m_pOnDiscardCard;
            public event ReturnCard m_pOnReturnCard;
            public event DisappearCard m_pOnDisappearCard;
            public event ShuffleCard m_pOnShuffleCard;

            public void OnDrawCard()
            {
                m_pOnDrawCard.Invoke(this);
            }
            public void OnPlayCard()
            {
                m_pOnPlayCard.Invoke(this);
            }
            public void OnDiscardCard()
            {
                m_pOnDiscardCard.Invoke(this);
            }
            public void OnReturnCard()
            {
                m_pOnReturnCard.Invoke(this);
            }
            public void OnDisappearCard()
            {
                m_pOnDisappearCard.Invoke(this);
            }
            public void OnShuffleCard()
            {
                m_pOnShuffleCard.Invoke();
            }
            public void BuildCardEffect()
            {
                m_pOnDrawCard += Helpers.EmptyEvent;
                m_pOnPlayCard += Helpers.EmptyEvent;
                m_pOnDiscardCard += Helpers.EmptyEvent;
                m_pOnReturnCard += Helpers.EmptyEvent;
                m_pOnDisappearCard += Helpers.EmptyEvent;
                m_pOnShuffleCard += Helpers.EmptyEvent;

            }
        }
    }
}
