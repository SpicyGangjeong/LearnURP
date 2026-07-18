using Core.Assets;
using Core.Deck;
using Core.Job;
using Core.Pool;
using Core.Scene;
using Core.StateMachine;
using Core.Variables;
using Cysharp.Threading.Tasks;
using Logic.State;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Core
{
    public delegate void DrawCard(Logic.Card.CardInstance pCard);
    public delegate void PlayCard(Logic.Card.CardInstance pCard);
    public delegate void DiscardCard(Logic.Card.CardInstance pCard);
    public delegate void ReturnCard(Logic.Card.CardInstance pCard);
    public delegate void DisappearCard(Logic.Card.CardInstance pCard);
    public delegate void ShuffleCard();

    public delegate void LerpModelCallback();


    public class CGameInstance : MonoBehaviour
    {
        static CGameInstance s_pInstance = null;
        static CInfoInstance s_pInfoInstance = null;
        public static CGameInstance Instance { get { Init(); return s_pInstance; } }

        [SerializeField] SO.SceneSO m_pSceneSO = null;
        SO.CardDocumentSO m_pCardDocumentSO = null;
        List<SO.CardInitialSetSO> m_vCardInitialSetSO = new List<SO.CardInitialSetSO>();
        ContentUpdater m_pContentUpdater = null;
        [SerializeField, Defines.Attribute.ReadOnly] DeckManager m_pDeckManager = null;
        LevelManager m_pLevelManager = null;
        AssetManager m_pAssetManager = null;
        [SerializeField, Defines.Attribute.ReadOnly] JobQueueManager m_pJobQueueManager = null;
        ObjectPoolManager m_pObjectPoolManager = null;
        Transform m_pPoolRoot = null;
        GlobalVariables m_pGlobalVariables = null;
        public DeckManager Deck => m_pDeckManager;
        public ObjectPoolManager ObjectPools => m_pObjectPoolManager;
        public JobQueueManager JobQueues => m_pJobQueueManager;
        public GlobalVariables Variables => m_pGlobalVariables;
        public Camera Main_Camera => Camera.main;
        public Vector2 Mouse_Pos => Mouse.current.position.ReadValue();

        CFSM m_pFSM = null;
        public SO.CardDocumentSO CardDocuments => m_pCardDocumentSO;

        public void EnqueueJob(Job.JobBase pJob)
        {
            m_pJobQueueManager.EnqueueJob(pJob);
        }
        public void Awake()
        {
            Init();
        }

        private static void Init()
        {
            if (null == s_pInstance)
            {
                string strInstance = "@GameInstance";
                GameObject pGameObject = GameObject.Find(strInstance);
                if (null == pGameObject)
                {
                    pGameObject = new GameObject(strInstance);
                    pGameObject.AddComponent<CGameInstance>();
                }
                DontDestroyOnLoad(pGameObject);
                DontDestroyOnLoad(EventSystem.current);
                s_pInstance = pGameObject.GetComponent<CGameInstance>();
                s_pInstance.m_pFSM = pGameObject.AddComponent<CFSM>();
                s_pInstance.Initialize();
                if (null == s_pInfoInstance)
                {
                    s_pInfoInstance = CInfoInstance.Instance;
                }
            }
        }

        private void InitializeFSMStates()
        {
            new CState_System_Initialize(
                new CState_System_Initialize.STATE_SYSTEM_INITIALIZE_DESC(s_pInstance, m_pFSM, s_pInstance, BootstrapAsync));
            new CState_System_Idle(
                new CState_System_Idle.STATE_SYSTEM_IDLE_DESC(s_pInstance, m_pFSM, s_pInstance));
            new CState_System_Playing(
                new CState_System_Playing.STATE_SYSTEM_PLAYING_DESC(s_pInstance, m_pFSM, s_pInstance));
            if (true == m_pFSM.Is_Valid_FSM((int)CState_System.SystemState.END))
            {
                m_pFSM.Change_State((int)CState_System.SystemState.INITIALIZE);
            }
        }
        private bool Initialize()
        {
            m_pGlobalVariables = GlobalVariables.Create();
            m_pContentUpdater = gameObject.AddComponent<ContentUpdater>();
            m_pAssetManager = new AssetManager();
            m_pDeckManager = new DeckManager();
            m_pSceneSO = m_pSceneSO.Clone() as SO.SceneSO;
            m_pLevelManager = new LevelManager(m_pSceneSO.m_vSceneReferences);
            m_pJobQueueManager = new JobQueueManager();
            m_pFSM = gameObject.GetComponent<CFSM>();
            m_pPoolRoot = EnsurePoolRoot(gameObject.transform);
            m_pObjectPoolManager = new ObjectPoolManager(m_pPoolRoot);
            if (null == m_pContentUpdater ||
                null == m_pAssetManager ||
                null == m_pDeckManager ||
                null == m_pLevelManager ||
                null == m_pJobQueueManager ||
                null == m_pFSM ||
                null == m_pObjectPoolManager)
            {
                return false;
            }
            InitializeFSMStates();
            return true;
        }
        public void Update()
        {
            int a = 0;
            int b = 0;
            a = b;
            b = a;
        }

        static Transform EnsurePoolRoot(Transform pParent)
        {
            Transform pExisting = pParent.Find("PoolRoot");
            if (null != pExisting)
            {
                return pExisting;
            }

            GameObject pPoolRootObject = new GameObject("PoolRoot");
            pPoolRootObject.transform.SetParent(pParent, false);
            return pPoolRootObject.transform;
        }

        public T GetPooled<T>(string strKey, Transform pParent = null) where T : Component
        {
            if (null == m_pObjectPoolManager)
            {
                Debug.LogError("ObjectPoolManager is null.");
                return null;
            }

            return m_pObjectPoolManager.Get<T>(strKey, pParent);
        }

        public void ReleasePooled<T>(string strKey, T pInstance) where T : Component
        {
            if (null == m_pObjectPoolManager)
            {
                return;
            }

            m_pObjectPoolManager.Release(strKey, pInstance);
        }
        public void StartGame(int iInitialSetIndex)
        {
            if (m_pFSM.IsCurrentState((int)CState_System.SystemState.IDLE))
            {
                CInfoInstance.Instance.StartGame();
                m_pFSM.Change_State((int)CState_System.SystemState.PLAYING);
                m_pDeckManager.Initialize(m_vCardInitialSetSO[iInitialSetIndex]);
                m_pDeckManager.StartGame();
            }
        }
        public _Ty LoadInstance<_Ty>(string strAssetName) where _Ty : Object
        {
            Object pPrototype = m_pAssetManager.Find_Prototype(strAssetName).Result;
            _Ty pInstance = Object.Instantiate(pPrototype).GetComponent<_Ty>();
            return pInstance;
        }
        public async Task<Object> LoadAddressAssetAsync(string strAssetName)
        {
            AsyncOperationHandle<Object> hHandle = await m_pAssetManager.LoadAddressAssetAsync(strAssetName);
            return hHandle.Result;
        }
        private async Task LoadPrototypesAsync()
        {
            bool flowControl = true;
            flowControl = await ReadyPrototypeAsync(Defines.Constants.s_strGamePlayCanvas);
            if (false == flowControl)
            {
                return;
            }
            flowControl = await ReadyPrototypePoolAsync<View.UI.CardCanvas>(Defines.Constants.s_strCardCanvas, 10, 0);
            if (false == flowControl)
            {
                return;
            }
            async Task<bool> ReadyPrototypeAsync(string ProtoTypeKey)
            {
                AsyncOperationHandle<Object> hGamePlayCanvasHandle = await m_pAssetManager.LoadAddressAssetAsync(ProtoTypeKey);
                return hGamePlayCanvasHandle.Result;
            }
            async Task<bool> ReadyPrototypePoolAsync<_Ty>(string ProtoTypeKey, int iPoolCount, int iMaxPoolCount) where _Ty : Component
            {
                AsyncOperationHandle<Object> hGamePlayCanvasHandle = await m_pAssetManager.LoadAddressAssetAsync(ProtoTypeKey);
                GameObject pPrefab = hGamePlayCanvasHandle.Result as GameObject;
                _Ty pComponent = pPrefab.GetComponent<_Ty>();
                if (m_pObjectPoolManager.IsRegistered(ProtoTypeKey))
                {
                    return false;
                }

                m_pObjectPoolManager.Register(ProtoTypeKey, pComponent, iPoolCount, iMaxSize: iMaxPoolCount);
                return true;
            }
        }
        private async Task LoadCardDocumentsAsync()
        {
            if (null != m_pCardDocumentSO)
            {
                return;
            }

            AsyncOperationHandle<Object> hCardDocumentsHandle = await m_pAssetManager.LoadAddressAssetAsync("CardDocuments");
            m_pCardDocumentSO = (hCardDocumentsHandle.Result as SO.CardDocumentSO).Clone();
        }
        private async Task LoadCardInitialSetAsync()
        {
            if (0 != m_vCardInitialSetSO.Count)
            {
                return;
            }

            AsyncOperationHandle<IList<Object>> hCardInitialSetsHandle = await m_pAssetManager.LoadLabelAssetsAsync("CardInitialSets");
            List<SO.CardInitialSetSO> temp = hCardInitialSetsHandle.Result.Cast<SO.CardInitialSetSO>().ToList();
            foreach (SO.CardInitialSetSO pCardInitialSetSO in temp)
            {
                m_vCardInitialSetSO.Add((pCardInitialSetSO.Clone()));
            }

            InitializeCardInitialSets();
        }
        private void InitializeCardInitialSets()
        {
            foreach (SO.CardInitialSetSO pCardInitialSet in m_vCardInitialSetSO)
            {
                pCardInitialSet.SetCardDocumentSO(m_pCardDocumentSO);
            }
        }
        public async UniTask ChangeScene(Defines.Enums.SceneID eSceneID)
        {
            await m_pLevelManager.ChangeScene(eSceneID);
        }
        public async Task BootstrapAsync()
        {
            await LoadPrototypesAsync();
            await LoadCardDocumentsAsync();
            await LoadCardInitialSetAsync();
        }
        private void OnDestroy()
        {
            if (null != m_pObjectPoolManager)
            {
                m_pObjectPoolManager.ClearAll();
            }

            m_pAssetManager.ReleaseAssets();
        }
    }

}