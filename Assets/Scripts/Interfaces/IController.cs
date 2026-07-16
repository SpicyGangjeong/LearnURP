namespace Logic
{
    public interface IController
    {
        void Control();
        void Ready_FSM();
        void BindUnit(IUnit unit);
    }
}