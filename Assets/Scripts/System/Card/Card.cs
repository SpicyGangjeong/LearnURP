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
            public Card(CardDataSO pInfo)
            {
                m_pCardInfo = ScriptableObject.CreateInstance<CardDataSO>();
                m_pCardInfo.hideFlags = HideFlags.HideAndDontSave;
                m_pCardInfo.CopyFrom(pInfo);
                BuildCardEffect();
            }
            public Card(int iCardID = 0)
            {
                m_pCardInfo = CGameInstance.Instance.GetCardInfo(iCardID);
                if (null == m_pCardInfo)
                {
                    Debug.LogError($"Card not found in CardDocuments: {iCardID}");
                    return;
                }
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
