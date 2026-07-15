using System;
using System.Collections.Generic;
using UnityEngine;

namespace Logic
{
    namespace Card
    {
        [Serializable]
        public class CardPileEntry
        {
            public Defines.Enums.CardPile ePile;
            public List<CardInstance> vCards = new List<CardInstance>();
        }

        [Serializable]
        public class CardPileCollection : ISerializationCallbackReceiver
        {
            Dictionary<Defines.Enums.CardPile, List<CardInstance>> m_vPiles = new Dictionary<Defines.Enums.CardPile, List<CardInstance>>();
            [SerializeField] List<CardPileEntry> m_vPileEntries = null;

            public CardPileCollection()
            {
                EnsureAllPiles();
            }

            public List<CardInstance> GetPile(Defines.Enums.CardPile ePileType)
            {
                if (false == m_vPiles.TryGetValue(ePileType, out List<CardInstance> vPile))
                {
                    return null;
                }

                return vPile;
            }

            public IReadOnlyList<CardInstance> GetCards(Defines.Enums.CardPile ePileType)
            {
                return GetPile(ePileType);
            }

            public int GetCount(Defines.Enums.CardPile ePileType)
            {
                List<CardInstance> vPile = GetPile(ePileType);
                if (null == vPile)
                {
                    return 0;
                }

                return vPile.Count;
            }

            public void ClearAll()
            {
                foreach (List<CardInstance> vPile in m_vPiles.Values)
                {
                    vPile.Clear();
                }
            }

            public void Clear(Defines.Enums.CardPile ePileType)
            {
                List<CardInstance> vPile = GetPile(ePileType);
                if (null != vPile)
                {
                    vPile.Clear();
                }
            }

            public void Add(CardInstance pCard, Defines.Enums.CardPile ePileType)
            {
                List<CardInstance> vPile = GetPile(ePileType);
                pCard.Data.m_eCurrentPile = ePileType;

                if (null != vPile)
                {
                    vPile.Add(pCard);
                }
            }

            public void AddRange(IEnumerable<CardInstance> vCards, Defines.Enums.CardPile ePileType)
            {
                foreach (CardInstance pCard in vCards)
                {
                    Add(pCard, ePileType);
                }
            }

            public bool Remove(CardInstance pCard)
            {
                List<CardInstance> vPile = GetPile(pCard.Data.m_eCurrentPile);
                if (null == vPile)
                {
                    pCard.Data.m_eCurrentPile = Defines.Enums.CardPile.END;
                    return false;
                }

                return vPile.Remove(pCard);
            }

            public bool Move(CardInstance pCard, Defines.Enums.CardPile eToPile)
            {
                if (false == Remove(pCard))
                {
                    return false;
                }

                Add(pCard, eToPile);
                return true;
            }

            public CardInstance GetTopCard(Defines.Enums.CardPile ePileType)
            {
                List<CardInstance> vPile = GetPile(ePileType);
                if (null == vPile || 0 == vPile.Count)
                {
                    return null;
                }

                return vPile[0];
            }

            public void Shuffle(Defines.Enums.CardPile ePileType)
            {
                List<CardInstance> vPile = GetPile(ePileType);
                if (null == vPile)
                {
                    return;
                }

                for (int i = 0; i < vPile.Count; i++)
                {
                    int iRandomIndex = UnityEngine.Random.Range(0, vPile.Count);
                    CardInstance pTempCard = vPile[i];
                    vPile[i] = vPile[iRandomIndex];
                    vPile[iRandomIndex] = pTempCard;
                }
                foreach (CardInstance p in vPile)
                {
                    p.OnShuffleCard();
                }
            }

            public void OnBeforeSerialize()
            {
                if (null == m_vPileEntries)
                {
                    m_vPileEntries = new List<CardPileEntry>();
                    foreach (KeyValuePair<Defines.Enums.CardPile, List<CardInstance>> kvp in m_vPiles)
                    {
                        CardPileEntry pEntry = new CardPileEntry();
                        pEntry.ePile = kvp.Key;
                        pEntry.vCards = GetPile(pEntry.ePile);
                        m_vPileEntries.Add(pEntry);
                    }
                }
            }

            public void OnAfterDeserialize()
            {
                return;
            }

            void EnsureAllPiles()
            {
                if (null == m_vPiles)
                {
                    m_vPiles = new Dictionary<Defines.Enums.CardPile, List<CardInstance>>();
                }

                for (Defines.Enums.CardPile ePile = Defines.Enums.CardPile.NONE + 1;
                    ePile != Defines.Enums.CardPile.ALL;
                    ePile++)
                {
                    if (false == m_vPiles.ContainsKey(ePile))
                    {
                        m_vPiles[ePile] = new List<CardInstance>();
                    }
                }
            }
        }
    }
}
