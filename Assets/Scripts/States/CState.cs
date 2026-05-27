using UnityEngine;

public abstract class CState
{
    public class CSTATEDESC
    {
        public CSTATEDESC(int StateID, MonoBehaviour Owner, CFSM FSM)
        {
            this.StateID = StateID;
            this.Owner = Owner;
            this.FSM = FSM;
        }
        public int StateID{ get; private set; }
        public MonoBehaviour Owner{ get; private set; }
        public CFSM FSM{ get; private set; } = null;
    }
    public CState(CSTATEDESC refOwner) {
        StateID = refOwner.StateID;
        Owner = refOwner.Owner;
        FSM = refOwner.FSM;
    }
    public int StateID { get; private set; } = -1;
    public MonoBehaviour Owner { get; private set; } = null;
    public CFSM FSM { get; private set; } = null;
    public abstract void Enter();
    public abstract void Fixed_Update_State();
    public abstract void Update_State();
    public abstract void Late_Update_State();
    public abstract void Exit();

}
