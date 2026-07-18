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
        public enum RoomType : int
        {
            NONE = -1,
            COMBAT = 0,
            SHOP = 1,
            REST = 2,
            EVENT = 3,
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
        public enum Group : int
        {
            NONE = -1,
            PLAYER = 0,
            MONSTER = 1,
            PROP = 2,
            END
        }
        namespace TargetRule
        {
            public enum Entity : int
            {
                NONE = -1,
                UNIT = 0,
                CARD = 1,
                BUFF = 2,
                MANAGER = 3,
                END,
            }

            public enum Scope : int
            {
                NONE = -1,
                SELF = 0,
                SELECTED = 1,
                ALLY = 2,
                ENEMY = 3,
                ALL_UNITS = 4,
                HAND = 5,
                DECK = 6,
                DISCARD = 7,
                FIELD = 8,
                END,
            }

            public enum Select : int
            {
                NONE = -1,
                SINGLE = 0,
                ALL = 1,
                RANDOM = 2,
                FIRST = 3,
                END,
            }
        }

    }
}