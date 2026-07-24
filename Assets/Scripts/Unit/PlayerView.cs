using Core;
using Defines.Enums;
using Defines.Expressions;
using Logic;
using UnityEngine;

namespace View
{
    namespace Unit
    {
        public class PlayerView : CPlayable
        {
            Group m_eCurrentGroup = Group.NONE;
            Animator m_pAnimator = null;
            private void Awake()
            {
                if (null == m_pAnimator)
                {
                    m_pAnimator = GetComponentInChildren<Animator>();
                }
            }
            public override Group CurrentGroup()
            {
                return m_eCurrentGroup;
            }

            public override void RegisterGroup(Group dstGroup)
            {
                InfoInstance.Instance.GroupInstance.RegistGroup(this, dstGroup);
                m_eCurrentGroup = dstGroup;
            }
            public override Group DeregistGroup()
            {
                InfoInstance.Instance.GroupInstance.DeregistGroup(this, m_eCurrentGroup);
                m_eCurrentGroup = Group.NONE;
                return m_eCurrentGroup;
            }

            public override Animator GetAnimator()
            {
                return m_pAnimator;
            }

            public override GameObject GetTargetObject()
            {
                return gameObject;
            }

            public override TransformHandle GetTransformHandle()
            {
                return transformHandle;
            }

            public override void Play()
            {
            }

            public override void Triggered()
            {
                throw new System.NotImplementedException();
            }
        }
    }
}