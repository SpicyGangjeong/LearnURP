using System;
using System.Collections.Generic;
using UnityEngine;

class CFSM : MonoBehaviour
{
    public Dictionary<int, CState> states = null;
    CState currState = null;
    CState prevState = null;
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

    public void Change_State(int iStateID)
    {
        if (null != currState)
        {
            currState.Exit();
            prevState = currState;
        }
        if (false == states.TryGetValue(iStateID, out currState))
        {
            Debug.LogError($"State not found: {iStateID} {states.Count}");
            return;
        }
        ResubscribeStateTicks(currState);
        if (null != currState)
        {
            currState.Enter();
        }
    }
    public bool Is_Valid_FSM(int iStateEndID){
        if (null == states)
        {
            Debug.LogError("FSM is not valid: states is null");
            return false;
        }
        for (int i = 0; i < iStateEndID; i++)
        {
            if (false == states.TryGetValue(i, out CState state)){
                Debug.LogError($"FSM is not valid: {i} is not found in states");
                return false;
            }
        }
        return true;
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
    public CState Get_CurrentState()
    {
        return currState;
    }
    void Awake() { }
    void Start() { }
    void FixedUpdate() { Fixed_Update_State(); }
    void Update() { Update_State(); }
    void LateUpdate() { Late_Update_State(); }
}