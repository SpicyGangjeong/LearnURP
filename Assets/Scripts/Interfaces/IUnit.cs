using Defines.Enums;

namespace Logic
{
    public interface IUnit : ITargettable
    {
        void RegisterGroup(Group dstGroup);
        Group DeregistGroup();
        Group CurrentGroup();
    }
}