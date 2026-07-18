using System;
using System.Text;
using UnityEngine;

namespace Logic
{
    namespace Card
    {
        [Serializable]
        public class CardData
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
            [SerializeField]
            private CardInformation m_Data = new CardInformation();
            #region getter
            public string Name => m_Data.strName;
            public int ID => m_Data.iID;
            public CardPortrait Portrait => m_Data.ePortrait;
            public CardType Type => m_Data.eType;
            public CardQuality Quality => m_Data.eQuality;
            public int Cost => m_Data.iCost;
            public CardEffect Effect => m_Data.vEffects;

            #endregion
            public string Description => m_Data.strDescription;

            public Defines.Enums.CardPile m_eCurrentPile = Defines.Enums.CardPile.END;

            private CardData() { }
            private CardData(CardData other) { }
            private bool Initialize(in CardInformation pOriginal)
            {
                m_Data = pOriginal;
                m_Data.vEffects = new CardEffect(pOriginal.vEffects);
                return true;
            }
            public static CardData Create(in CardInformation pInformation)
            {
                CardData pInstance = new CardData();
                if (false == pInstance.Initialize(pInformation))
                {
                    pInstance = null;
                }
                return pInstance;
            }
        }
        [Serializable]
        public struct CardInformation
        {
            public string strName;
            public int iID;
            public CardData.CardPortrait ePortrait;
            public CardData.CardType eType;
            public CardData.CardQuality eQuality;
            public int iCost;
            public CardEffect vEffects;

            [TextArea(3, 10)]
            public string strDescription;



            public void BuildCardDescription()
            {
                StringBuilder pSb = new StringBuilder();
                vEffects.BuildExpression(pSb);
                strDescription = pSb.ToString();
            }
        }
    }
}
