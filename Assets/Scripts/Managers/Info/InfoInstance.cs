using Core.Client;
using Defines.Expressions;
using Logic;
using UnityEngine;

namespace Core
{
    public class CInfoInstance : MonoBehaviour
    {
        static CInfoInstance s_pInstance = null;
        static CGameInstance s_pGameInstance = null;
        public static CInfoInstance Instance { get { Init(); return s_pInstance; } }
        PlayerInstance m_pPlayerInstance = null;
        GroupInstance m_groupInstance = null;
        public PlayerInstance PlayerInstance => m_pPlayerInstance;
        public GroupInstance GroupInstance => m_groupInstance;

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
                    pGameObject.AddComponent<CInfoInstance>();
                }
                DontDestroyOnLoad(pGameObject);
                s_pInstance = pGameObject.GetComponent<CInfoInstance>();
                if (ERESULT.TRUE == s_pInstance.Initialize())
                {

                }
                if (null == s_pGameInstance)
                {
                    s_pGameInstance = CGameInstance.Instance;
                }
            }
        }
        private ERESULT Initialize()
        {
            m_pPlayerInstance = new PlayerInstance();
            m_groupInstance = new GroupInstance();
            return ERESULT.TRUE;
        }
        public ERESULT StartGame()
        {
            if (ERESULT.FALSE == GroupInstance.StartGame(out IUnit outPlayer))
            {
                Debug.LogError("Start Failed");
                return ERESULT.FALSE;
            }
            if (ERESULT.FALSE == PlayerInstance.StartGame())
            {
                //GroupInstance.
                Debug.LogError("Start Failed");
                return ERESULT.FALSE;
            }
            return ERESULT.TRUE;
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
            s_pInstance = null;
            s_pGameInstance = null;
        }
    }
}