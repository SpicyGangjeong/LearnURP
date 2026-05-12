using System;
using System.Collections.Generic;
using UnityEngine;
namespace ENGINESTATES
{
    enum ENGINESTATES
    {
        STATE_INITIALIZE = 0,
        STATE_LOGO,
        STATE_TITLE,
        STATE_SETTING,
        STATE_STAGE,
        STATE_ENDING,
    }
}
class CFSM : MonoBehaviour
{
    HashSet<CState> states = null;
    CState currState = null;
    CState prevState = null;

    /// <summary>상태 없을 때도 null이 아니게 두어 틱에서 NRE가 나지 않게 한다.</summary>
    static void EmptyTick() { }

    Action onFixedUpdateTick = EmptyTick;
    Action onUpdateTick = EmptyTick;
    Action onLateUpdateTick = EmptyTick;

    void ResubscribeStateTicks(CState next)
    {
        onFixedUpdateTick = EmptyTick;
        onUpdateTick = EmptyTick;
        onLateUpdateTick = EmptyTick;
        if (next == null)
            return;
        onFixedUpdateTick += next.Fixed_Update_State;
        onUpdateTick += next.Update_State;
        onLateUpdateTick += next.Late_Update_State;
    }

    public void Change_State(CState newState)
    {
        if (null != currState)
        {
            currState.Exit();
            prevState = currState;
        }
        currState = newState;
        ResubscribeStateTicks(currState);
        if (null != currState)
        {
            currState.Enter();
        }
    }
    public void Fixed_Update_State()
    {
        onFixedUpdateTick();
    }

    public void Update_State()
    {
        onUpdateTick();
    }
    public void Late_Update_State()
    {
        onLateUpdateTick();
    }
    public CState Get_PrevState()
    {
        return prevState;
    }
    
    void Awake() { }
    void Start() { }
    void FixedUpdate() { }
    void Update() { }
    void LateUpdate() { Update_State(); }
}