namespace Logic
{
    public interface ISlot : ITargettable
    {
        IUnit GetCurrentUnit();
        void SetCurrentUnit(IUnit pUnit);
    }
}