using Defines;
using System;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

namespace Logic
{
    namespace Card
    {
        [CreateAssetMenu(fileName = "CardDataSO", menuName = "Scriptable Objects/CardDataSO")]
        [Serializable]
        public class CardDataSO : ScriptableObject, IClonable
        {
            public enum CardPortrait : int
            {
                NONE = -1,
                ACROBAT = 0,
                ADRENALINE,
                ALCHEMY,
                ANTIDOTE,
                ARMYMAN,
                ARROW_BARRAGE,
                BACKSTAB,
                BEAM,
                BEAST,
                BLACKHOLE,
                BUFF,
                CHOP,
                CLAW,
                DASH,
                ENERGETIC,
                FALC_MIXTURE,
                FIGHTER,
                FIST,
                FLY,
                GEMOSTATIC,
                HEAL,
                HIGHKICK,
                KNIFEMASTERY,
                LOWKICK,
                MACHINE,
                METEORSHOWER,
                OFI,
                PACKAGING,
                PAINKILLERS,
                PAINKILLERS2,
                PISTOL,
                POUND,
                POWERSTRIKE,
                PSYCICATTACK,
                PUNCHES,
                PUNISHER,
                RAGE_POTION,
                RECON,
                REFLECT,
                RELOAD,
                REPAIR,
                REVIVE,
                RUNNER,
                RUNNINGFIST,
                RUNNINGSTRIKE,
                SALVE,
                SLASH,
                SLIDE,
                SPIRITARROWS,
                STURDY,
                END,
            }
            public enum CardType : int
            {
                NONE = -1,
                ATTACK = 0,
                DEFENSE = 1,
                MAGIC = 2,
                ITEM = 3,
                END = 4,
            }
            public enum CardQuality : int
            {
                NONE = -1,
                COMMON = 0,
                UNCOMMON = 1,
                RARE = 2,
                EPIC = 3,
                LEGEND = 4,
                END
            }
            public string m_strCardName = "";
            public int m_iCardID = 0;
            public CardPortrait m_eCardPortrait = CardPortrait.NONE;
            public CardType m_eCardType = CardType.NONE;
            public CardQuality m_eQuality = CardQuality.NONE;
            public int m_iCardCost = 0;
            public CardEffect m_vCardEffects = new CardEffect();

            [TextArea(3, 10)]
            public string m_strCardDescription;

            [NonSerialized]
            public Defines.Enums.CardPile m_eCurrentPile = Defines.Enums.CardPile.END;
            private void CopyFrom(CardDataSO pInfo)
            {
                m_iCardID = pInfo.m_iCardID;
                m_eCardType = pInfo.m_eCardType;
                m_eQuality = pInfo.m_eQuality;
                m_eCardPortrait = pInfo.m_eCardPortrait;
                m_iCardCost = pInfo.m_iCardCost;
                m_strCardName = pInfo.m_strCardName;
                m_vCardEffects = new CardEffect(pInfo.m_vCardEffects);
                m_strCardDescription = pInfo.m_strCardDescription;
            }
            public void BuildCardDescription()
            {
                StringBuilder pSb = new StringBuilder();
                m_vCardEffects.BuildExpression(pSb);
                m_strCardDescription = pSb.ToString();
            }
            private void OnValidate()
            {
                Debug.Log("OnValidate Call", this);
                if (m_iCardID < 0)
                {
                    m_iCardID = 0;
                }
                if (m_iCardCost < 0)
                {
                    m_iCardCost = 0;
                }
                BuildCardDescription();
            }

            public IClonable Clone()
            {
                CardDataSO pCloneData = ScriptableObject.CreateInstance<CardDataSO>();
                pCloneData.CopyFrom(this);
                return pCloneData;
            }
            private CardDataSO() { }
            private CardDataSO(CardDataSO pOriginal) { }
        }
    }
}