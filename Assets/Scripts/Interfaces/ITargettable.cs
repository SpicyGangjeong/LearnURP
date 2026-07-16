
using Defines.Expressions;
using UnityEngine;

namespace Logic
{
    public interface ITargettable : ITriggable
    {
        public TransformHandle GetTransformHandle();
        public ERESULT GetRaycastHit(out RaycastHit hitOut);
        public GameObject GetTargetObject();
    }
}