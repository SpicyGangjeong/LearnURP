using UnityEngine;

namespace Core
{
    namespace Variables
    {
        public class GlobalVariables
        {
            private GlobalVariables() { }

            public Vector2 TargetViewPort { get; private set; }

            private bool Initialize()
            {
                TargetViewPort = Defines.Constants.TargetPC;
                return true;
            }
            public static GlobalVariables Create()
            {
                GlobalVariables pInstance = new GlobalVariables();
                if (false == pInstance.Initialize())
                {
                    pInstance = null;
                }
                return pInstance;
            }
        }
    }
}