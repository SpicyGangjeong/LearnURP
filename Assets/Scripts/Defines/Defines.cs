using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.Controls;
namespace DEFINES
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
        END = 4,
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
        END = 3,
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