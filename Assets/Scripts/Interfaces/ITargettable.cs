
using Defines.Expressions;
using UnityEngine;

namespace Logic
{
    public interface ITargettable : ITriggable
    {
        public TransformHandle GetTransformHandle();
        public GameObject GetTargetObject();
    }
}