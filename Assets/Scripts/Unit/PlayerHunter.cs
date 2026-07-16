using Core;
using Defines.Enums;
using Defines.Expressions;
using Logic;
using UnityEngine;

namespace View
{
    namespace Unit
    {
        public class PlayerHunter : CPlayable
        {
            Group m_eCurrentGroup = Group.NONE;
            public override Group CurrentGroup()
            {
                return m_eCurrentGroup;
            }

            public override Group DeregistGroup()
            {
                CInfoInstance.Instance.GroupInstance.DeregistGroup(this, m_eCurrentGroup);
                m_eCurrentGroup = Group.NONE;
                return m_eCurrentGroup;
            }

            public override ERESULT GetRaycastHit(out RaycastHit hitOut)
            {
                hitOut = new RaycastHit();
                return ERESULT.FALSE;
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

            public override void RegisterGroup(Group dstGroup)
            {
                CInfoInstance.Instance.GroupInstance.RegistGroup(this, dstGroup);
                m_eCurrentGroup = dstGroup;
            }

            public override void Triggered()
            {
                throw new System.NotImplementedException();
            }
        }
    }
}