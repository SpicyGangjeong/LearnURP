using UnityEngine;
using UnityEngine.Android;

public class CGameInstance : MonoBehaviour
{
    static CGameInstance s_pInstance = null;
    static CGameInstance Instance { get { Init(); return s_pInstance; } }

    CFSM GameStateMachine = null;
    static private void Init()
    {
        if (null == s_pInstance)
        {
            string strInstance = "@GameInstance";
            GameObject gameobject = GameObject.Find(strInstance);
            if (null == gameobject)
            {
                gameobject = new GameObject(strInstance);
                gameobject.AddComponent<CGameInstance>();
                gameobject.AddComponent<CFSM>();
            }
            DontDestroyOnLoad(gameobject);
            s_pInstance = gameobject.GetComponent<CGameInstance>();
            s_pInstance.GameStateMachine = gameobject.GetComponent<CFSM>();
        }
    }
    static private bool Initialize(ref CGameInstance Instance)
    {

        return true;
    }

    private void Awake()
    {
        
    }
    void Start()
    {
        Init();
    }

    void Update()
    {
        
    }
}
