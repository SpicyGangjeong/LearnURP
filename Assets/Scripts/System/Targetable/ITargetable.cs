namespace Logic
{
    public interface ITargetable
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