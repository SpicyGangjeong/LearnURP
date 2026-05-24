using System;
using System.Collections.Generic;
using System.Text;

class Card
{
    public int CardID { get; }
    public DEFINES.CardType eCardType { get; }
    public int iCardCost { get; }
    public string strCardName { get; }
    public string strCardDescription { get; }

    public Card(int cardID = 0)
    {
        CardInfo cardInfo = CGameInstance.Instance.GetCardInfo(cardID);
        if (null == cardInfo)
        {
            return;
        }

        CardID = cardInfo.iCardID;
        eCardType = cardInfo.eCardType;
        iCardCost = cardInfo.iCardCost;
        strCardName = cardInfo.strCardName;
        strCardDescription = cardInfo.strCardDescription;
    }

    public delegate void DrawCard();
    public delegate void PlayCard();
    public delegate void DiscardCard();
    public delegate void ReturnCard();
    public delegate void DisappearCard();
    public delegate void ShuffleCard();
    DrawCard drawCard = null;
    PlayCard playCard = null;
    DiscardCard discardCard = null;
    ReturnCard returnCard = null;
    DisappearCard disappearCard = null;
    ShuffleCard shuffleCard = null;
}
