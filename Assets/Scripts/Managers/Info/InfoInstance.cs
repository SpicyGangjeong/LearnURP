using Core;
using Core.Assets;
using Core.Deck;
using Core.Job;
using Core.Pool;
using Core.Scene;
using Core.StateMachine;
using Core.Variables;
using UnityEngine;

namespace Core
{
    public class CInfoInstance : MonoBehaviour
    {
        static CInfoInstance s_pInstance = null;
        public static CInfoInstance Instance { get { Init(); return s_pInstance; } }
        public void Awake()
        {
            Init();
        }
        private static void Init()
        {
            if (null == s_pInstance)
            {
                string strInstance = "@InfoInstance";
                GameObject pGameObject = GameObject.Find(strInstance);
                if (null == pGameObject)
                {
                    pGameObject = new GameObject(strInstance);
                    pGameObject.AddComponent<CGameInstance>();
                }
                DontDestroyOnLoad(pGameObject);
                s_pInstance = pGameObject.GetComponent<CInfoInstance>();
                s_pInstance.Initialize();
            }
        }
        private bool Initialize()
        {
            return true;
        }
        public void Update()
        {
            int a = 0;
            int b = 0;
            a = b;
            b = a;
        }
        private void OnDestroy()
        {

        }
    }
}