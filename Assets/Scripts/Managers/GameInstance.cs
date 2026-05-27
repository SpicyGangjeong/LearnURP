using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Android;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting;

public class CGameInstance : MonoBehaviour
{
    static CGameInstance s_pInstance = null;
    public static CGameInstance Instance { get { Init(); return s_pInstance; } }

    
    [SerializeField] CardDocuments cardDocuments = null;
    [SerializeField] List<CardInitialSet> cardInitialSets = null;
    private DeckManager deckManager = null;
    private ContentUpdater contentUpdater = null;
    private CFSM fsm = null;
    private AsyncOperationHandle<IList<CardDocuments>> cardDocumentsHandle;
    private AsyncOperationHandle<IList<CardInitialSet>> cardInitialSetsHandle;
    public CardDocuments CardDocuments => cardDocuments;

    private static void Init()
    {
        if (null == s_pInstance)
        {
            string strInstance = "@GameInstance";
            GameObject gameobject = GameObject.Find(strInstance);
            if (null == gameobject)
            {
                gameobject = new GameObject(strInstance);
                gameobject.AddComponent<CGameInstance>();
            }
            DontDestroyOnLoad(gameobject);
            s_pInstance = gameobject.GetComponent<CGameInstance>();
            s_pInstance.fsm = gameobject.GetOrAddComponent<CFSM>();
            Initialize(ref s_pInstance);
        }
    }

    private static void InitializeFSMStates()
    {
        if (null == s_pInstance)
        {
            Debug.LogError("s_pInstance is null");
            return;
        }
        if (null == s_pInstance.fsm)
        {
            Debug.LogError("fsm is null");
            return;
        }
        s_pInstance.fsm.states = new Dictionary<int, CState>();
        s_pInstance.fsm.states.Add((int)DEFINES.SystemState.INITIALIZE, 
            new CState_Sys_Initialize(new CState_Sys_Initialize.STATE_SYS_INITIALIZE_DESC((int)DEFINES.SystemState.INITIALIZE, s_pInstance, s_pInstance)));
        s_pInstance.fsm.states.Add((int)DEFINES.SystemState.IDLE,
            new CState_Sys_Idle(new CState_Sys_Idle.STATE_SYS_IDLE_DESC((int)DEFINES.SystemState.IDLE, s_pInstance, s_pInstance)));
        s_pInstance.fsm.states.Add((int)DEFINES.SystemState.PLAYING,
            new CState_Sys_Playing(new CState_Sys_Playing.STATE_SYS_PLAYING_DESC((int)DEFINES.SystemState.PLAYING, s_pInstance, s_pInstance)));
        if (true == s_pInstance.fsm.Is_Valid_FSM((int)DEFINES.SystemState.END))
        {
            s_pInstance.fsm.Change_State((int)DEFINES.SystemState.INITIALIZE);
        }
    }
    private static bool Initialize(ref CGameInstance Instance)
    {
        Instance.contentUpdater = Instance.gameObject.AddComponent<ContentUpdater>();
        Instance.deckManager = new DeckManager();
        Instance.fsm = Instance.gameObject.GetComponent<CFSM>();
        if (null == Instance.contentUpdater || 
        null == Instance.deckManager || 
        null == Instance.fsm)
        {
            return false;
        }
        InitializeFSMStates();
        return true;
    }

    async Task LoadCardDocumentsAsync()
    {
        if (null != cardDocuments)
        {
            return;
        }

        LoadAssetsByLabel<CardDocuments> loader = new LoadAssetsByLabel<CardDocuments>("CardDocuments");
        cardDocumentsHandle = await loader.LoadAsync();
        if (false == cardDocumentsHandle.IsValid() ||
         null == cardDocumentsHandle.Result ||
          0 == cardDocumentsHandle.Result.Count)
        {
            Debug.LogError("CardDocuments not found via Addressables label 'CardDocuments'.");
            return;
        }
        
        cardDocuments = cardDocumentsHandle.Result[0];
    }
    private async Task LoadCardInitialSetAsync()
    {
        if (null == cardInitialSets)
        {
            LoadAssetsByLabel<CardInitialSet> loader = new LoadAssetsByLabel<CardInitialSet>("CardInitialSets");
            cardInitialSetsHandle = await loader.LoadAsync();
            if (false == cardInitialSetsHandle.IsValid() ||
             null == cardInitialSetsHandle.Result ||
              0 == cardInitialSetsHandle.Result.Count)
            {
                Debug.LogError("CardInitialSets not found via Addressables label 'CardInitialSets'.");
                return;
            }

            cardInitialSets = new List<CardInitialSet>(cardInitialSetsHandle.Result);
        }

        InitializeCardInitialSets();
    }

    private void InitializeCardInitialSets()
    {
        if (null == cardDocuments)
        {
            Debug.LogError("Cannot initialize CardInitialSets: CardDocuments is null.");
            return;
        }

        if (null == cardInitialSets)
        {
            return;
        }

        foreach (CardInitialSet cardInitialSet in cardInitialSets)
        {
            if (null == cardInitialSet)
            {
                continue;
            }

            cardInitialSet.SetCardDocuments(cardDocuments);
        }
    }

    public CardInfo GetCardInfo(int cardID)
    {
        if (null == cardDocuments)
        {
            Debug.LogError("CardDocuments is null");
            return null;
        }

        return cardDocuments.GetCard(cardID);
    }

    private void Awake()
    {
        
    }
    private async void Start()
    {
        Init();
        await LoadCardDocumentsAsync();
        await LoadCardInitialSetAsync();
        fsm.Change_State((int)DEFINES.SystemState.IDLE);
    }

    private void OnDestroy()
    {
        if (cardDocumentsHandle.IsValid())
        {
            Addressables.Release(cardDocumentsHandle);
        }
        if (cardInitialSetsHandle.IsValid())
        {
            Addressables.Release(cardInitialSetsHandle);
        }
    }
}
