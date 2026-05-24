using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Android;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Threading.Tasks;

public class CGameInstance : MonoBehaviour
{
    static CGameInstance s_pInstance = null;
    public static CGameInstance Instance { get { Init(); return s_pInstance; } }

    
    [SerializeField] CardDocuments cardDocuments = null;
    [SerializeField] List<CardInitialSet> cardInitialSets = null;
    DeckManager deckManager = null;
    ContentUpdater contentUpdater = null;
    AsyncOperationHandle<IList<CardDocuments>> cardDocumentsHandle;
    AsyncOperationHandle<IList<CardInitialSet>> cardInitialSetsHandle;
    public CardDocuments CardDocuments => cardDocuments;
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
    async Task LoadCardInitialSetAsync()
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

    void InitializeCardInitialSets()
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
    async void Start()
    {
        Init();
        await LoadCardDocumentsAsync();
        await LoadCardInitialSetAsync();
    }

    void OnDestroy()
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
