using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

enum CardPile
{
    NONE = -1,
    HAND = 0,
    DISCARD = 1,
    DECK = 2,
    DISAPPEARED = 3,
}
struct CardPileInfo
{
    public CardPile ePileType { get; set; }
    public List<Card> listCards { get; set; }
    public CardPileInfo(CardPile ePileType = CardPile.NONE, List<Card> listCards = null) : this()
    {
        if (null == listCards)
        {
            listCards = new List<Card>();
        }
        this.ePileType = ePileType;
        this.listCards = listCards;
    }
}
class DeckManager
{
    List<Card> deckOriginal = new List<Card>();
    List<Card> PileofHand = new List<Card>();
    List<Card> PileofDiscard = new List<Card>();
    List<Card> PileofDeck = new List<Card>();
    List<Card> PileofDisappeared = new List<Card>();

    public DeckManager()
    {

    }
}
