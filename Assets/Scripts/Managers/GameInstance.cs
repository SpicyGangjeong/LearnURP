using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Android;
using UnityEngine.SceneManagement;

public class CGameInstance : MonoBehaviour
{
    static CGameInstance s_pInstance = null;
    public static CGameInstance Instance { get { Init(); return s_pInstance; } }

    DeckManager deckManager = null;
    ContentUpdater contentUpdater = null;

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
            bool bResult = Initialize(ref s_pInstance);
        }
    }
    static private bool Initialize(ref CGameInstance Instance)
    {
        Instance.contentUpdater = Instance.gameObject.AddComponent<ContentUpdater>();
        Instance.deckManager = new DeckManager();
        if (null == Instance.contentUpdater || null == Instance.deckManager)
        {
            return false;
        }
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
