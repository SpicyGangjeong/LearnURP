using UnityEngine;

public abstract class CState
{
    public class CSTATEDESC
    {
        public MonoBehaviour pOwner;
    }
    public CState(ref CSTATEDESC refOwner) {
        Owner = refOwner.pOwner;
    }
    public MonoBehaviour Owner = null;
    public abstract void Enter();
    public abstract void Update();
    public abstract void Exit();

}
