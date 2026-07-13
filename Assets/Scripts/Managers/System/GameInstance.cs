using Core.Assets;
using Core.Deck;
using Core.Pool;
using Core.Job;
using Core.Scene;
using Core.StateMachine;
using Logic.State;

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using Core.Variables;

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
        public static CGameInstance Instance { get { Init(); return s_pInstance; } }

        [SerializeField] SO.SceneSO m_pSceneSO = null;
        SO.CardDocumentSO m_pCardDocumentSO = null;
        List<SO.CardInitialSetSO> m_vCardInitialSetSO = new List<SO.CardInitialSetSO>();
        ContentUpdater m_pContentUpdater = null;
        DeckManager m_pDeckManager = null;
        LevelManager m_pLevelManager = null;
        AssetManager m_pAssetManager = null;
        JobQueueManager m_pJobQueueManager = null;
        ObjectPoolManager m_pObjectPoolManager = null;
        Transform m_pPoolRoot = null;
        GlobalVariables m_pGlobalVariables = null;
        public DeckManager Deck => m_pDeckManager;
        public ObjectPoolManager ObjectPools => m_pObjectPoolManager;
        public JobQueueManager JobQueues => m_pJobQueueManager;

        public GlobalVariables Variables => m_pGlobalVariables;

        CFSM m_pFsm = null;
        public SO.CardDocumentSO CardDocuments => m_pCardDocumentSO;

        public void EnqueueJob(Job.IJob pJob)
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
                s_pInstance = pGameObject.GetComponent<CGameInstance>();
                s_pInstance.m_pFsm = pGameObject.AddComponent<CFSM>();
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
            if (null == s_pInstance.m_pFsm)
            {
                Debug.LogError("fsm is null");
                return;
            }
            s_pInstance.m_pFsm.m_vStates = new Dictionary<int, CState>();
            s_pInstance.m_pFsm.m_vStates.Add((int)CState_System.SystemState.INITIALIZE, new CState_System_Initialize(
                new CState_System_Initialize.STATE_SYSTEM_INITIALIZE_DESC((int)CState_System.SystemState.INITIALIZE, s_pInstance, s_pInstance.m_pFsm, s_pInstance, s_pInstance.BootstrapAsync)));
            s_pInstance.m_pFsm.m_vStates.Add((int)CState_System.SystemState.IDLE, new CState_System_Idle(
                new CState_System_Idle.STATE_SYSTEM_IDLE_DESC((int)CState_System.SystemState.IDLE, s_pInstance, s_pInstance.m_pFsm, s_pInstance)));
            s_pInstance.m_pFsm.m_vStates.Add((int)CState_System.SystemState.PLAYING, new CState_System_Playing(
                new CState_System_Playing.STATE_SYSTEM_PLAYING_DESC((int)CState_System.SystemState.PLAYING, s_pInstance, s_pInstance.m_pFsm, s_pInstance)));

            if (true == s_pInstance.m_pFsm.Is_Valid_FSM((int)CState_System.SystemState.END))
            {
                s_pInstance.m_pFsm.Change_State((int)CState_System.SystemState.INITIALIZE);
            }
        }
        private static bool Initialize(ref CGameInstance pInstance)
        {
            pInstance.m_pGlobalVariables = GlobalVariables.Create();
            pInstance.m_pContentUpdater = pInstance.gameObject.AddComponent<ContentUpdater>();
            pInstance.m_pAssetManager = new AssetManager();
            pInstance.m_pDeckManager = new DeckManager();
            pInstance.m_pSceneSO = pInstance.m_pSceneSO.Clone() as SO.SceneSO;
            pInstance.m_pLevelManager = new LevelManager(pInstance.m_pSceneSO.m_vSceneReferences);
            pInstance.m_pJobQueueManager = new JobQueueManager();
            pInstance.m_pFsm = pInstance.gameObject.GetComponent<CFSM>();
            pInstance.m_pPoolRoot = EnsurePoolRoot(pInstance.gameObject.transform);
            pInstance.m_pObjectPoolManager = new ObjectPoolManager(pInstance.m_pPoolRoot);
            if (null == pInstance.m_pContentUpdater ||
                null == pInstance.m_pAssetManager ||
                null == pInstance.m_pDeckManager ||
                null == pInstance.m_pLevelManager ||
                null == pInstance.m_pJobQueueManager ||
                null == pInstance.m_pFsm ||
                null == pInstance.m_pObjectPoolManager)
            {
                return false;
            }
            InitializeFSMStates();
            return true;
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
        public void StartDeck(int iInitialSetIndex)
        {
            if (m_pFsm.IsCurrentState((int)CState_System.SystemState.IDLE))
            {
                m_pDeckManager.Initialize(m_vCardInitialSetSO[iInitialSetIndex]);
                m_pDeckManager.StartGame();
            }
        }
        public async Task<Object> LoadAddressAssetAsync(string strAssetName)
        {
            AsyncOperationHandle<Object> hHandle = await m_pAssetManager.LoadAddressAssetAsync(strAssetName);
            return hHandle.Result;
        }
        private async Task LoadPrototypesAsync()
        {
            const int iCardCanvasPoolSize = 10;
            AsyncOperationHandle<Object> hCardCanvasHandle = await m_pAssetManager.LoadAddressAssetAsync("CardCanvas");
            GameObject pCardCanvasPrefab = hCardCanvasHandle.Result as GameObject;
            if (null == pCardCanvasPrefab)
            {
                Debug.LogError("CardCanvas prefab load failed.");
                return;
            }

            View.UI.CardCanvas pCardCanvasComponent = pCardCanvasPrefab.GetComponent<View.UI.CardCanvas>();
            if (null == pCardCanvasComponent)
            {
                Debug.LogError("CardCanvas component missing on prefab.");
                return;
            }

            if (m_pObjectPoolManager.IsRegistered(PoolKeys.s_strCardCanvas))
            {
                return;
            }

            m_pObjectPoolManager.Register(PoolKeys.s_strCardCanvas, pCardCanvasComponent, iCardCanvasPoolSize, iMaxSize: 0);
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
        public void ChangeScene(Defines.Enums.SceneID eSceneID)
        {
            m_pLevelManager.ChangeScene(eSceneID);
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