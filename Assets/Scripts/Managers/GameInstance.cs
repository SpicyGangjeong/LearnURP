using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

public class CGameInstance : MonoBehaviour
{
    static CGameInstance s_pInstance = null;
    public static CGameInstance Instance { get { Init(); return s_pInstance; } }

    [SerializeField] SceneSO sceneSO = null;
    [SerializeField] CardDocumentSO cardDocumentSO = null;
    [SerializeField] List<CardInitialSetSO> cardInitialSetSO = null;
    private ContentUpdater contentUpdater = null;
    private DeckManager deckManager = null;
    private LevelManager levelManager = null;
    private AssetManager assetManager = null;
    private JobQueueManager jobQueueManager = null;
    private ObjectPoolManager objectPoolManager = null;
    Transform poolRoot = null;

    public ObjectPoolManager ObjectPools => objectPoolManager;

    public event DrawCard       OnDrawCard { add { deckManager.OnDrawCard += value; } remove { deckManager.OnDrawCard -= value; } }
    public event PlayCard       OnPlayCard { add { deckManager.OnPlayCard += value; } remove { deckManager.OnPlayCard -= value; } }
    public event DiscardCard    OnDiscardCard { add { deckManager.OnDiscardCard += value; } remove { deckManager.OnDiscardCard -= value; } }
    public event ReturnCard     OnReturnCard { add { deckManager.OnReturnCard += value; } remove { deckManager.OnReturnCard -= value; } }
    public event DisappearCard  OnDisappearCard { add { deckManager.OnDisappearCard += value; } remove { deckManager.OnDisappearCard -= value; } }
    public event ShuffleCard    OnShuffleCard { add { deckManager.OnShuffleCard += value; } remove { deckManager.OnShuffleCard -= value; } }
    public event EndTurn        OnEndTurn { add { deckManager.OnEndTurn += value; } remove { deckManager.OnEndTurn -= value; } }
    private CFSM fsm = null;
    public CardDocumentSO CardDocuments => cardDocumentSO;

    public void EnqueueJob(IJob job)
    {
        jobQueueManager.EnqueueJob(job);
    }

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
            s_pInstance.fsm = gameobject.AddComponent<CFSM>();
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
        s_pInstance.fsm.states.Add((int)DEFINES.SystemState.INITIALIZE, new CState_Sys_Initialize(
            new CState_Sys_Initialize.STATE_SYS_INITIALIZE_DESC((int)DEFINES.SystemState.INITIALIZE, s_pInstance, s_pInstance.fsm, s_pInstance, s_pInstance.BootstrapAsync)));
        s_pInstance.fsm.states.Add((int)DEFINES.SystemState.IDLE, new CState_Sys_Idle(
            new CState_Sys_Idle.STATE_SYS_IDLE_DESC((int)DEFINES.SystemState.IDLE, s_pInstance, s_pInstance.fsm, s_pInstance)));
        s_pInstance.fsm.states.Add((int)DEFINES.SystemState.PLAYING, new CState_Sys_Playing(
            new CState_Sys_Playing.STATE_SYS_PLAYING_DESC((int)DEFINES.SystemState.PLAYING, s_pInstance, s_pInstance.fsm, s_pInstance)));
            
        if (true == s_pInstance.fsm.Is_Valid_FSM((int)DEFINES.SystemState.END))
        {
            s_pInstance.fsm.Change_State((int)DEFINES.SystemState.INITIALIZE);
        }
    }
    private static bool Initialize(ref CGameInstance Instance)
    {
        Instance.contentUpdater = Instance.gameObject.AddComponent<ContentUpdater>();
        Instance.assetManager = new AssetManager();
        Instance.deckManager = new DeckManager();
        Instance.levelManager = new LevelManager(Instance.sceneSO.sceneReferences);
        Instance.jobQueueManager = new JobQueueManager();
        Instance.fsm = Instance.gameObject.GetComponent<CFSM>();
        Instance.poolRoot = EnsurePoolRoot(Instance.gameObject.transform);
        Instance.objectPoolManager = new ObjectPoolManager(Instance.poolRoot);
        if (null == Instance.contentUpdater || 
            null == Instance.assetManager ||
            null == Instance.deckManager || 
            null == Instance.levelManager ||
            null == Instance.jobQueueManager ||
            null == Instance.fsm ||
            null == Instance.objectPoolManager)
        {
            return false;
        }
        InitializeFSMStates();
        return true;
    }

    static Transform EnsurePoolRoot(Transform parent)
    {
        Transform existing = parent.Find("PoolRoot");
        if (null != existing)
        {
            return existing;
        }

        GameObject poolRootObject = new GameObject("PoolRoot");
        poolRootObject.transform.SetParent(parent, false);
        return poolRootObject.transform;
    }

    public T GetPooled<T>(string key, Transform parent = null) where T : Component
    {
        if (null == objectPoolManager)
        {
            Debug.LogError("ObjectPoolManager is null.");
            return null;
        }

        return objectPoolManager.Get<T>(key, parent);
    }

    public void ReleasePooled<T>(string key, T instance) where T : Component
    {
        if (null == objectPoolManager)
        {
            return;
        }

        objectPoolManager.Release(key, instance);
    }
    public void StartDeck(int initialSetIndex)
    {
        if (fsm.IsCurrentState((int)DEFINES.SystemState.IDLE))
        {
            deckManager.Initialize(cardInitialSetSO[initialSetIndex]);
            deckManager.StartGame();
        }
    }
    public int GetPileCount(DEFINES.CardPile pileType)
    {
        return deckManager.GetPileCount(pileType);
    }
    public IReadOnlyList<Card> GetCards(DEFINES.CardPile pileType)
    {
        return deckManager.GetCards(pileType);
    }

    public bool TryPlayCard(Card card)
    {
        if (false == CanPlayHandCard())
        {
            return false;
        }

        return deckManager.PlayCard(card);
    }

    public bool TryDiscardCard(Card card)
    {
        if (false == CanPlayHandCard())
        {
            return false;
        }

        return deckManager.DiscardCard(card);
    }

    public bool TryEndTurn()
    {
        if (false == CanPlayHandCard())
        {
            return false;
        }

        deckManager.EndTurn();
        return true;
    }

    bool CanPlayHandCard()
    {
        return fsm.IsCurrentState((int)DEFINES.SystemState.IDLE)
            || fsm.IsCurrentState((int)DEFINES.SystemState.PLAYING);
    }

    public async Task<Object> LoadAddressAssetAsync(string assetName)
    {
        AsyncOperationHandle<Object> handle = await assetManager.LoadAddressAssetAsync(assetName);
        return handle.Result;
    }
    private async Task LoadPrototypesAsync()
    {
        const int CARD_CANVAS_POOL_SIZE = 10;
        AsyncOperationHandle<Object> cardCanvasHandle = await assetManager.LoadAddressAssetAsync("CardCanvas");
        GameObject cardCanvasPrefab = cardCanvasHandle.Result as GameObject;
        if (null == cardCanvasPrefab)
        {
            Debug.LogError("CardCanvas prefab load failed.");
            return;
        }

        CardCanvas cardCanvasComponent = cardCanvasPrefab.GetComponent<CardCanvas>();
        if (null == cardCanvasComponent)
        {
            Debug.LogError("CardCanvas component missing on prefab.");
            return;
        }

        if (objectPoolManager.IsRegistered(PoolKeys.CardCanvas))
        {
            return;
        }

        objectPoolManager.Register(PoolKeys.CardCanvas, cardCanvasComponent, CARD_CANVAS_POOL_SIZE, maxSize: 0);
    }
    private async Task LoadCardDocumentsAsync()
    {
        if (null != cardDocumentSO)
        {
            return;
        }

        AsyncOperationHandle<Object> cardDocumentsHandle = await assetManager.LoadAddressAssetAsync("CardDocuments");
        cardDocumentSO = cardDocumentsHandle.Result as CardDocumentSO;
    }
    private async Task LoadCardInitialSetAsync()
    {
        if (0 != cardInitialSetSO.Count)
        {
            return;
        }

        AsyncOperationHandle<IList<Object>> cardInitialSetsHandle = await assetManager.LoadLabelAssetsAsync("CardInitialSets");
        cardInitialSetSO = cardInitialSetsHandle.Result.Cast<CardInitialSetSO>().ToList();

        InitializeCardInitialSets();
    }
    private void InitializeCardInitialSets()
    {
        foreach (CardInitialSetSO cardInitialSet in cardInitialSetSO)
        {
            cardInitialSet.SetCardDocumentSO(cardDocumentSO);
        }
    }

    public CardInfo GetCardInfo(int cardID)
    {
        if (null == cardDocumentSO)
        {
            Debug.LogError("CardDocuments is null");
            return null;
        }

        return cardDocumentSO.GetCard(cardID);
    }

    public void ChangeScene(DEFINES.SceneID sceneID)
    {
        levelManager.ChangeScene(sceneID);
    }

    private void Awake()
    {
        
    }
    private void Start()
    {
        Init();
    }
    public async Task BootstrapAsync()
    {
        await LoadPrototypesAsync();
        await LoadCardDocumentsAsync();
        await LoadCardInitialSetAsync();
    }

    private void OnDestroy()
    {
        if (null != objectPoolManager)
        {
            objectPoolManager.ClearAll();
        }

        assetManager.ReleaseAssets();
    }
}
