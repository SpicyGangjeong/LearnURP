using System;

namespace Defines
{
    namespace Enums
    {
        public enum SceneID : int
        {
            NONE = -1,
            MAIN_MENU = 0,
            GAME_PLAY = 1,
            END,
        }
        public enum CardPile : int
        {
            NONE = -1,
            HAND = 0,
            DISCARD = 1,
            DECK = 2,
            DISAPPEARED = 3,
            FIELD = 4,
            ALL = 5,
            END = 6,
        }
    }
}