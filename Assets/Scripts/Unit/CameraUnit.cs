using Core;
using Defines.Enums;
using Logic;
using UnityEngine;

namespace View
{
    namespace Unit
    {
        public class CameraUnit : MonoBehaviour, IUnit
        {
            Group m_eCurrentGroup = Group.NONE;
            Animator m_pAnimator = null;
            private void Awake()
            {
                if (null == m_pAnimator)
                {
                    m_pAnimator = GetComponent<Animator>();
                }
            }
            public Group CurrentGroup()
            {
                return m_eCurrentGroup;
            }

            public void RegisterGroup(Group dstGroup)
            {
                InfoInstance.Instance.GroupInstance.RegistGroup(this, dstGroup);
                m_eCurrentGroup = dstGroup;
            }
            public Group DeregistGroup()
            {
                InfoInstance.Instance.GroupInstance.DeregistGroup(this, m_eCurrentGroup);
                m_eCurrentGroup = Group.NONE;
                return m_eCurrentGroup;
            }

            public Animator GetAnimator()
            {
                return m_pAnimator;
            }

            public GameObject GetTargetObject()
            {
                return gameObject;
            }

            public TransformHandle GetTransformHandle()
            {
                return transformHandle;
            }

            public void Triggered()
            {
                
            }

        }
    }
}