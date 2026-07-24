using Core;
using Core.Job;
using Core.Room;
using Cysharp.Threading.Tasks;
using Defines;
using Defines.Structures;
using Logic.Card;
using System.ComponentModel;
using TMPro;
using UnityEngine;

namespace View
{
    namespace UI
    {
        public class GamePlayCanvas : MonoBehaviour
        {
            private enum PvtPos : int
            {
                DECK = 0,
                DISCARD = 1,
                DISAPPEAR = 2,
                LEFT = 3,
                RIGHT = 4,
                HANDBOARD = 5,
                END = 6,
            }
            static GamePlayCanvas s_pInstance = null;
            public static GamePlayCanvas Instance => s_pInstance;

            TextMeshProUGUI m_pTmpDeckPile = null;
            TextMeshProUGUI m_pTmpDiscardPile = null;
            TextMeshProUGUI m_pTmpDisappearPile = null;

            [SerializeField]
            GameObject m_pCardDrawTable = null;
            [SerializeField, ReadOnly(true)]
            HandBoard m_pHandBoard = null;
            [SerializeField]
            RoomBoard m_pRoomBoard = null;
            [SerializeField]
            GameObject m_pDeckUIRoot = null;
            GameInstance m_pGameInstance = null;
            RoomManager m_pRoomManager = null;

            [SerializeField]
            Transform[] m_pPvts = new Transform[(int)PvtPos.END];
            MoveInfo[] m_MoveInfos = new MoveInfo[(int)PvtPos.END];

            void Awake()
            {
                s_pInstance = this;
                m_pGameInstance = GameInstance.Instance;
                m_pGameInstance.Deck.m_pOnCardDrawn += PresentDraw;
                for (int i = 0; i < m_MoveInfos.Length; i++)
                {
                    Helpers.ExtractMoveInfo(out m_MoveInfos[i], m_pPvts[i]);
                }
                m_pTmpDeckPile = m_pPvts[(int)PvtPos.DECK].GetComponent<TextMeshProUGUI>();
                m_pTmpDiscardPile = m_pPvts[(int)PvtPos.DISCARD].GetComponent<TextMeshProUGUI>();
                m_pTmpDisappearPile = m_pPvts[(int)PvtPos.DISAPPEAR].GetComponent<TextMeshProUGUI>();
            }
            void Start()
            {
                m_pRoomManager = m_pGameInstance.Rooms;
                m_pRoomManager.m_pOnMapGenerated += OnMapGenerated;
                m_pRoomManager.m_pOnRoomEntered += OnRoomEntered;
                m_pRoomManager.m_pOnRoomExited += OnRoomExited;
                ShowLobby();
            }

            private void OnDestroy()
            {
                if (null != m_pGameInstance)
                {
                    m_pGameInstance.Deck.m_pOnCardDrawn -= PresentDraw;
                }
                if (null != m_pRoomManager)
                {
                    m_pRoomManager.m_pOnMapGenerated -= OnMapGenerated;
                    m_pRoomManager.m_pOnRoomEntered -= OnRoomEntered;
                    m_pRoomManager.m_pOnRoomExited -= OnRoomExited;
                }
                if (s_pInstance == this)
                {
                    s_pInstance = null;
                }
            }

            void OnMapGenerated(System.Collections.Generic.IReadOnlyList<Logic.Room.Room> vRooms)
            {
                ShowLobby();
            }

            void OnRoomEntered(Logic.Room.Room pRoom)
            {
                ShowArena();
            }

            void OnRoomExited(Logic.Room.Room pRoom)
            {
                ShowLobby();
            }

            public void ShowLobby()
            {
                if (null != m_pRoomBoard)
                {
                    m_pRoomBoard.SetMapVisible(true);
                }
                // Do not SetActive HandBoard — it is also the card parent pivot.
                // CardCanvas.OnEnable/OnDisable clears m_pRefCard via CustomFinalize.
                SetHandBoardVisible(false);
                SetDeckUIVisible(false);
            }

            public void ShowArena()
            {
                // Keep RoomBoard active so Exit remains usable after Cleared.
                if (null != m_pRoomBoard)
                {
                    m_pRoomBoard.SetMapVisible(false);
                }
                SetHandBoardVisible(true);
                SetDeckUIVisible(true);
            }

            void SetHandBoardVisible(bool bVisible)
            {
                if (null == m_pHandBoard)
                {
                    return;
                }
                // Recover if an older ShowLobby used SetActive(false) on the card parent.
                if (false == m_pHandBoard.gameObject.activeSelf)
                {
                    m_pHandBoard.gameObject.SetActive(true);
                }
                CanvasGroup pGroup = m_pHandBoard.GetComponent<CanvasGroup>();
                if (null == pGroup)
                {
                    pGroup = m_pHandBoard.gameObject.AddComponent<CanvasGroup>();
                }
                pGroup.alpha = true == bVisible ? 1f : 0f;
                pGroup.interactable = bVisible;
                pGroup.blocksRaycasts = bVisible;
            }

            void SetDeckUIVisible(bool bVisible)
            {
                if (null != m_pDeckUIRoot)
                {
                    m_pDeckUIRoot.SetActive(bVisible);
                    return;
                }
                // Toggle TMP only — never the HandBoard/card parent pivot.
                SetPivotVisible(PvtPos.DECK, bVisible);
                SetPivotVisible(PvtPos.DISCARD, bVisible);
                SetPivotVisible(PvtPos.DISAPPEAR, bVisible);
            }

            void SetPivotVisible(PvtPos ePos, bool bVisible)
            {
                Transform pPivot = m_pPvts[(int)ePos];
                if (null == pPivot)
                {
                    return;
                }
                // HandBoard shares the HANDBOARD pivot; never deactivate it here.
                if (pPivot == m_pHandBoard.transform)
                {
                    return;
                }
                CanvasGroup pGroup = pPivot.GetComponent<CanvasGroup>();
                if (null == pGroup)
                {
                    pGroup = pPivot.gameObject.AddComponent<CanvasGroup>();
                }
                pGroup.alpha = true == bVisible ? 1f : 0f;
                pGroup.interactable = bVisible;
                pGroup.blocksRaycasts = bVisible;
            }

            void Update()
            {
                ChangePileSize();
            }

            void ChangePileSize()
            {
                if (m_pTmpDeckPile != null)
                {
                    m_pTmpDeckPile.text = m_pGameInstance.Deck.GetPileCount(Defines.Enums.CardPile.DECK).ToString();
                    m_pTmpDiscardPile.text = m_pGameInstance.Deck.GetPileCount(Defines.Enums.CardPile.DISCARD).ToString();
                    m_pTmpDisappearPile.text = m_pGameInstance.Deck.GetPileCount(Defines.Enums.CardPile.DISAPPEARED).ToString();
                }
            }

            public void RenderDrawTable(int iPile)
            {
                RenderDrawTable((Defines.Enums.CardPile)iPile);
            }
            public void RenderDrawTable(Defines.Enums.CardPile ePile)
            {
                switch (ePile)
                {
                    case Defines.Enums.CardPile.DECK:
                        m_pCardDrawTable.SetActive(true);
                        break;
                    case Defines.Enums.CardPile.DISCARD:
                        m_pCardDrawTable.SetActive(true);
                        break;
                    case Defines.Enums.CardPile.DISAPPEARED:
                        m_pCardDrawTable.SetActive(true);
                        break;
                    case Defines.Enums.CardPile.ALL:
                        m_pCardDrawTable.SetActive(true);
                        break;
                    default:
                        m_pCardDrawTable.SetActive(false);
                        break;
                }
                m_pCardDrawTable.GetComponent<DrawTable>().ShowCards(ePile);
            }

            public void RequestEndTurn()
            {
                EndTurn();
            }

            public void EndTurn()
            {
                JobBase jobEndTurn = new JobDeferredCallback(async () =>
                {
                    await m_pGameInstance.Deck.EndTurn();
                }, "Ending_Turn");
                m_pGameInstance.EnqueueJob(jobEndTurn);
            }

            public HandBoard HandBoard => m_pHandBoard;

            public void PoolCard(CardInstance pCard, out CardCanvas pCardCanvas)
            {
                pCardCanvas = m_pGameInstance.GetPooled<CardCanvas>(Defines.Constants.s_strCardCanvas,
                    m_pPvts[(int)PvtPos.HANDBOARD]);

                pCardCanvas.BindCard(pCard);
                Helpers.ApplyMoveInfo(m_MoveInfos[(int)PvtPos.DECK], pCardCanvas.transform);
            }

            void PresentDraw(CardInstance pCard)
            {
                JobBase jobDraw = new JobDeferredCallback(async () =>
                {
                    await UniTask.Delay(Constants.TIME_MS_DRAWING_INTERVAL);
                    PoolCard(pCard, out CardCanvas pCardCanvas);
                    m_pHandBoard.BindCard(pCard, pCardCanvas);
                    await pCardCanvas.StartBezierMoveAsync((float)Constants.TIME_MS_DRAWING_DURATION / Constants.TIME_MS_ASEC,
                                    m_MoveInfos[(int)PvtPos.LEFT],
                                    m_MoveInfos[(int)PvtPos.HANDBOARD]);
                    m_pHandBoard.UpdateHandLayout();
                }, "Drawing_Card");
                m_pGameInstance.EnqueueJob(jobDraw);
            }

            public void PresentPlay(CardInstance pCard, CardCanvas pCardCanvas)
            {
                if (null == pCard || null == pCard.Data)
                {
                    return;
                }
                if (null == pCardCanvas)
                {
                    return;
                }
                m_pGameInstance.Deck.MoveToFieldCard(pCard);
                pCardCanvas.StartBezierMove((float)Constants.TIME_MS_DISCARD_DURATION / Constants.TIME_MS_ASEC,
                                            m_MoveInfos[(int)PvtPos.RIGHT],
                                            m_MoveInfos[(int)PvtPos.DISCARD],
                                            () =>
                                            {
                                                m_pGameInstance.Deck.PlayCard(pCard);
                                                m_pGameInstance.ReleasePooled<CardCanvas>(Defines.Constants.s_strCardCanvas, pCardCanvas);
                                            });
            }

            public void PresentDiscard(CardInstance pCard, CardCanvas pCardCanvas)
            {
                m_pHandBoard.PopCardForPresentation(pCard);
                JobBase jobDiscard = new JobDeferredCallback(async () =>
                {
                    await UniTask.Delay(Constants.TIME_MS_DISCARD_DURATION);
                    m_pGameInstance.Deck.MoveToFieldCard(pCard);
                    await pCardCanvas.StartBezierMoveAsync((float)Constants.TIME_MS_DRAWING_INTERVAL / Constants.TIME_MS_ASEC,
                                    m_MoveInfos[(int)PvtPos.RIGHT],
                                    m_MoveInfos[(int)PvtPos.DISCARD]);
                    m_pGameInstance.Deck.DiscardCard(pCard);
                    m_pGameInstance.ReleasePooled<CardCanvas>(Defines.Constants.s_strCardCanvas, pCardCanvas);
                }, "Discarding_Card");
                m_pGameInstance.EnqueueJob(jobDiscard);
            }
            [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
            private static void ReloadOnLoad()
            {
                s_pInstance = null;
            }
        }

    }
}
