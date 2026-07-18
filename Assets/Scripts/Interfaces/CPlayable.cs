using Defines.Enums;
using Defines.Expressions;
using UnityEngine;

namespace Logic
{
    public abstract class CPlayable : MonoBehaviour, IUnit
    {
        public abstract Group CurrentGroup();
        public abstract Group DeregistGroup();
        public abstract GameObject GetTargetObject();
        public abstract Animator GetAnimator();
        public abstract TransformHandle GetTransformHandle();
        public abstract void RegisterGroup(Group dstGroup);
        public abstract void Triggered();
        public abstract void Play();

    }
}