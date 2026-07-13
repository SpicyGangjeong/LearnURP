using Core;
using Core.Job;
using Core.Pool;
using Cysharp.Threading.Tasks;
using Defines;
using Defines.Structures;
using Logic.Card;
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
            [SerializeField]
            HandBoard m_pHandBoard = null;
            CGameInstance m_pGameInstance = null;

            [SerializeField]
            Transform[] m_pPvts = new Transform[(int)PvtPos.END];
            MoveInfo[] m_MoveInfos = new MoveInfo[(int)PvtPos.END];

            void Awake()
            {
                s_pInstance = this;
            }

            void Start()
            {
                m_pGameInstance = CGameInstance.Instance;
                if (null == m_pHandBoard)
                {
                    m_pHandBoard = FindFirstObjectByType<HandBoard>();
                }
                m_pGameInstance.Deck.m_pOnCardDrawn += PresentDraw;
                for (int i = 0; i < m_MoveInfos.Length; i++)
                {
                    Helpers.ExtractMoveInfo(out m_MoveInfos[i], m_pPvts[i]);
                }
                m_pTmpDeckPile = m_pPvts[(int)PvtPos.DECK].GetComponent<TextMeshProUGUI>();
                m_pTmpDiscardPile = m_pPvts[(int)PvtPos.DISCARD].GetComponent<TextMeshProUGUI>();
                m_pTmpDisappearPile = m_pPvts[(int)PvtPos.DISAPPEAR].GetComponent<TextMeshProUGUI>();
            }
            private void OnDestroy()
            {
                if (null != m_pGameInstance)
                {
                    m_pGameInstance.Deck.m_pOnCardDrawn -= PresentDraw;
                }
                if (s_pInstance == this)
                {
                    s_pInstance = null;
                }
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
                IJob jobEndTurn = new JobEndTurnCallback(async () =>
                {
                    await m_pGameInstance.Deck.EndTurn();
                });
                m_pGameInstance.EnqueueJob(jobEndTurn);
            }

            public HandBoard HandBoard => m_pHandBoard;

            public void PoolCard(CardInstance pCard, out CardCanvas pCardCanvas)
            {
                pCardCanvas = m_pGameInstance.GetPooled<CardCanvas>(PoolKeys.s_strCardCanvas,
                    m_pPvts[(int)PvtPos.HANDBOARD]);

                pCardCanvas.BindCard(pCard);
                Helpers.ApplyMoveInfo(m_MoveInfos[(int)PvtPos.DECK], pCardCanvas.transform);
            }

            void PresentDraw(CardInstance pCard)
            {
                IJob jobDraw = new JobDrawCallback(async () =>
                {
                    await UniTask.Delay(Constants.TIME_MS_DRAWING_INTERVAL);
                    PoolCard(pCard, out CardCanvas pCardCanvas);
                    m_pHandBoard.BindCard(pCard, pCardCanvas);
                    await pCardCanvas.StartBezierMoveAsync((float)Constants.TIME_MS_DRAWING_DURATION / Constants.TIME_MS_ASEC,
                                    m_MoveInfos[(int)PvtPos.LEFT],
                                    m_MoveInfos[(int)PvtPos.HANDBOARD]);
                    m_pHandBoard.UpdateHandLayout();
                });
                m_pGameInstance.EnqueueJob(jobDraw);
            }

            public void PresentPlay(CardInstance pCard, CardCanvas pCardCanvas)
            {
                m_pGameInstance.Deck.MoveToFieldCard(pCard);
                pCardCanvas.StartBezierMove((float)Constants.TIME_MS_DISCARD_DURATION / Constants.TIME_MS_ASEC,
                                            m_MoveInfos[(int)PvtPos.RIGHT],
                                            m_MoveInfos[(int)PvtPos.DISCARD],
                                            () =>
                                            {
                                                m_pGameInstance.Deck.PlayCard(pCard);
                                                m_pGameInstance.ReleasePooled<CardCanvas>(PoolKeys.s_strCardCanvas, pCardCanvas);
                                            });
            }

            public void PresentDiscard(CardInstance pCard, CardCanvas pCardCanvas)
            {
                m_pHandBoard.PopCardForPresentation(pCard);
                IJob jobDiscard = new JobDiscardCallback(async () =>
                {
                    await UniTask.Delay(Constants.TIME_MS_DISCARD_DURATION);
                    m_pGameInstance.Deck.MoveToFieldCard(pCard);
                    await pCardCanvas.StartBezierMoveAsync((float)Constants.TIME_MS_DRAWING_INTERVAL / Constants.TIME_MS_ASEC,
                                    m_MoveInfos[(int)PvtPos.RIGHT],
                                    m_MoveInfos[(int)PvtPos.DISCARD]);
                    m_pGameInstance.Deck.DiscardCard(pCard);
                    m_pGameInstance.ReleasePooled<CardCanvas>(PoolKeys.s_strCardCanvas, pCardCanvas);
                });
                m_pGameInstance.EnqueueJob(jobDiscard);
            }
        }

    }
}
