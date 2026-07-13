using Core;
using Defines;

namespace Logic
{
    namespace Card
    {
        public class CardInstance
        {
            CardData m_pData = null;
            public CardData Data => m_pData;
            public CardInstance(CardData pData)
            {
                m_pData = pData;
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
