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

    public void Change_State(CState newState)
    {
        if (null != currState)
        {
            currState.Exit();
            prevState = currState;
        }
        currState = newState;
        if (null != currState)
        {
            currState.Enter();
        }
    }
    public void Fixed_Update_State()
    {
        if (null != currState)
        {
            currState.Fixed_Update_State();
        }
    }

    public void Update_State()
    {
        if (null != currState)
        {
            currState.Update_State();
        }
    }
    public void Late_Update_State()
    {
        if (null != currState)
        {
            currState.Late_Update_State();
        }
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