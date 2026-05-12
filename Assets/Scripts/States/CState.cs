using UnityEngine;

public abstract class CState
{
    public class CSTATEDESC
    {
        public MonoBehaviour pOwner;
    }
    public CState(CSTATEDESC refOwner) {
        Owner = refOwner.pOwner;
    }
    public MonoBehaviour Owner = null;
    public abstract void Enter();

    public abstract void Fixed_Update_State();
    public abstract void Update_State();
    public abstract void Late_Update_State();
    public abstract void Exit();

}
