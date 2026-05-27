using UnityEngine;

public abstract class CState
{
    public class CSTATEDESC
    {
        public CSTATEDESC(int StateID, MonoBehaviour Owner)
        {
            this.StateID = StateID;
            this.Owner = Owner;
        }
        public int StateID{ get; private set; }
        public MonoBehaviour Owner{ get; private set; }
    }
    public CState(CSTATEDESC refOwner) {
        StateID = refOwner.StateID;
        Owner = refOwner.Owner;
    }
    public int StateID { get; private set; } = -1;
    public MonoBehaviour Owner { get; private set; } = null;
    public abstract void Enter();
    public abstract void Fixed_Update_State();
    public abstract void Update_State();
    public abstract void Late_Update_State();
    public abstract void Exit();

}
