using System;

namespace Logic
{
    namespace Room
    {
        public enum RoomActivationKind : int
        {
            NONE = -1,
            FLAG = 0,
            SPECIAL_EVENT = 1,
            END,
        }

        public enum RoomCompletionKind : int
        {
            NONE = -1,
            ALL_MONSTERS_DEFEATED = 0,
            EVENT_TRIGGER = 1,
            END,
        }

        [Serializable]
        public class RoomActivationCondition
        {
            public RoomActivationKind m_eKind = RoomActivationKind.NONE;
            public string m_strKey = string.Empty;
        }

        [Serializable]
        public class RoomCompletionCondition
        {
            public RoomCompletionKind m_eKind = RoomCompletionKind.NONE;
            public string m_strKey = string.Empty;
        }
    }
}
