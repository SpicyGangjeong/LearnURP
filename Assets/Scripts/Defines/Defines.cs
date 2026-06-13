using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.Controls;
namespace DEFINES
{
    namespace STRUCTURES
    {
        public struct MoveInfo
        {
            public Quaternion vRotQ;
            public Vector3 vPosition;
        }
    }
    public static class HELPERS
    {
        public static Vector3 GetQuadraticBezierPoint(float fT, Vector3 p0, Vector3 p1, Vector3 p2)
        {
            float fOneMinusT = 1f - fT;
            return fOneMinusT * fOneMinusT * p0 + 2f * fOneMinusT * fT * p1 + fT * fT * p2;
        }
        public static Vector3 GetQuadraticBezierTangent(float fT, Vector3 p0, Vector3 p1, Vector3 p2)
        {
            float fOneMinusT = 1f - fT;
            return 2f * fOneMinusT * (p1 - p0) + 2f * fT * (p2 - p1);
        }
    }
    namespace ENUMS
    {
        enum SystemState : int
        {
            NONE = -1,
            INITIALIZE = 0,
            IDLE = 1,
            PLAYING = 2,
            END = 3,
        }
        public enum SceneID
        {
            NONE = -1,
            MAIN_MENU = 0,
            GAME_PLAY = 1,
            END,
        }
        public enum CardPile
        {
            NONE = -1,
            HAND = 0,
            DISCARD = 1,
            DECK = 2,
            DISAPPEARED = 3,
            ALL = 4,
            END = 5,
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
        public enum CardEffectTriggerType
        {
            NONE = -1,
            DRAW = 0,
            PLAY = 1,
            DISCARD = 2,
            RETURN = 3,
            DISAPPEAR = 4,
            SHUFFLE = 5,
            END = 6,
        }
        public enum CardEffectTargetType
        {
            NONE = -1,
            SELF = 0,
            SELECTED = 1,
            ALL = 2,
            CARD = 3,
            END,
        }
        public enum CardEffectValueType
        {
            NONE = -1,
            DAMAGE = 0,
            HEAL = 1,
            SHIELD = 2,
            BUFF = 3,
            DEBUFF = 4,
            END = 5,
        }
    }
}