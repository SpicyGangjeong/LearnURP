using UnityEngine;

namespace Defines
{
    namespace Bases
    {
        public abstract class ScriptableObjectCloneable<_Ty> : ScriptableObject
            where _Ty : ScriptableObjectCloneable<_Ty>
        {
            public _Ty Clone()
            {

                _Ty pCloneData = ScriptableObject.CreateInstance<_Ty>();
                pCloneData.CopyFrom((_Ty)this);
                return pCloneData;
            }

            protected abstract void CopyFrom(_Ty pOriginal);
        }
    }
}