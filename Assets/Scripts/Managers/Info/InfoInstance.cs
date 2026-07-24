using Core.Client;
using Defines.Expressions;
using Logic;
using UnityEngine;

namespace Core
{
    public class InfoInstance : MonoBehaviour
    {
        static InfoInstance s_pInstance = null;
        static GameInstance s_pGameInstance = null;
        public static InfoInstance Instance { get { Init(); return s_pInstance; } }
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
                    pGameObject.AddComponent<InfoInstance>();
                }
                DontDestroyOnLoad(pGameObject);
                s_pInstance = pGameObject.GetComponent<InfoInstance>();
                if (ERESULT.TRUE == s_pInstance.Initialize())
                {

                }
                if (null == s_pGameInstance)
                {
                    s_pGameInstance = GameInstance.Instance;
                }
            }
        }
        private ERESULT Initialize()
        {
            m_pPlayerInstance = new PlayerInstance();
            m_groupInstance = new GroupInstance();
            return ERESULT.TRUE;
        }
        public ERESULT StartFieldLevel()
        {
            if (ERESULT.FALSE == GroupInstance.StartFieldLevel(out IUnit outPlayer))
            {
                Debug.LogError("Start Failed");
                return ERESULT.FALSE;
            }
            if (ERESULT.FALSE == PlayerInstance.StartFieldLevel())
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
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ReloadOnLoad()
        {
            s_pInstance = null;
            s_pGameInstance = null;
        }
    }
}