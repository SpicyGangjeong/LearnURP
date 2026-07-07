using TMPro;
using UnityEngine;
using DEFINES;
using DEFINES.ENUMS;
using DEFINES.STRUCTURES;
using Cysharp.Threading.Tasks;

public class GamePlayCanvas : MonoBehaviour
{
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
    Transform[] m_pPvts = new Transform[(int)GamePlayCanvasPvtPos.END];
    MoveInfo[] m_MoveInfos = new MoveInfo[(int)GamePlayCanvasPvtPos.END];

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
            HELPERS.ExtractMoveInfo(out m_MoveInfos[i], m_pPvts[i]);
        }
        m_pTmpDeckPile = m_pPvts[(int)GamePlayCanvasPvtPos.DECK].GetComponent<TextMeshProUGUI>();
        m_pTmpDiscardPile = m_pPvts[(int)GamePlayCanvasPvtPos.DISCARD].GetComponent<TextMeshProUGUI>();
        m_pTmpDisappearPile = m_pPvts[(int)GamePlayCanvasPvtPos.DISAPPEAR].GetComponent<TextMeshProUGUI>();
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
            m_pTmpDeckPile.text = m_pGameInstance.Deck.GetPileCount(CardPile.DECK).ToString();
            m_pTmpDiscardPile.text = m_pGameInstance.Deck.GetPileCount(CardPile.DISCARD).ToString();
            m_pTmpDisappearPile.text = m_pGameInstance.Deck.GetPileCount(CardPile.DISAPPEARED).ToString();
        }
    }

    [EnumAction(typeof(CardPile))]
    public void RenderDrawTable(int pPile)
    {
        CardPile pile = (CardPile)pPile;
        switch (pile)
        {
            case CardPile.DECK:
                m_pCardDrawTable.SetActive(true);
                break;
            case CardPile.DISCARD:
                m_pCardDrawTable.SetActive(true);
                break;
            case CardPile.DISAPPEARED:
                m_pCardDrawTable.SetActive(true);
                break;
            case CardPile.ALL:
                m_pCardDrawTable.SetActive(true);
                break;
            default:
                m_pCardDrawTable.SetActive(false);
                break;
        }
        m_pCardDrawTable.GetComponent<DrawTable>().ShowCards(pile);
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

    public void PoolCard(Card pCard, out CardCanvas pCardCanvas)
    {
        pCardCanvas = m_pGameInstance.GetPooled<CardCanvas>(PoolKeys.s_strCardCanvas,
            m_pPvts[(int)GamePlayCanvasPvtPos.HANDBOARD]);

        pCardCanvas.BindCard(pCard);
        HELPERS.ApplyMoveInfo(m_MoveInfos[(int)GamePlayCanvasPvtPos.DECK], pCardCanvas.transform);
    }

    void PresentDraw(Card pCard)
    {
        IJob jobDraw = new JobDrawCallback(async () =>
        {
            await UniTask.Delay(CONSTANTS.TIME_MS_DRAWING_INTERVAL);
            PoolCard(pCard, out CardCanvas pCardCanvas);
            m_pHandBoard.BindCard(pCard, pCardCanvas);
            await pCardCanvas.StartBezierMoveAsync((float)CONSTANTS.TIME_MS_DRAWING_DURATION / CONSTANTS.TIME_MS_ASEC,
                            m_MoveInfos[(int)GamePlayCanvasPvtPos.LEFT],
                            m_MoveInfos[(int)GamePlayCanvasPvtPos.HANDBOARD]);
            m_pHandBoard.UpdateHandLayout();
        });
        m_pGameInstance.EnqueueJob(jobDraw);
    }

    public void PresentPlay(Card pCard, CardCanvas pCardCanvas)
    {
        m_pGameInstance.Deck.MoveToFieldCard(pCard);
        pCardCanvas.StartBezierMove((float)CONSTANTS.TIME_MS_DISCARD_DURATION / CONSTANTS.TIME_MS_ASEC,
                                    m_MoveInfos[(int)GamePlayCanvasPvtPos.RIGHT],
                                    m_MoveInfos[(int)GamePlayCanvasPvtPos.DISCARD],
                                    () =>
                                    {
                                        m_pGameInstance.Deck.PlayCard(pCard);
                                        m_pGameInstance.ReleasePooled<CardCanvas>(PoolKeys.s_strCardCanvas, pCardCanvas);
                                    });
    }

    public void PresentDiscard(Card pCard, CardCanvas pCardCanvas)
    {
        m_pHandBoard.PopCardForPresentation(pCard);
        IJob jobDiscard = new JobDiscardCallback(async () =>
        {
            await UniTask.Delay(CONSTANTS.TIME_MS_DISCARD_DURATION);
            m_pGameInstance.Deck.MoveToFieldCard(pCard);
            await pCardCanvas.StartBezierMoveAsync((float)CONSTANTS.TIME_MS_DRAWING_INTERVAL / CONSTANTS.TIME_MS_ASEC,
                            m_MoveInfos[(int)GamePlayCanvasPvtPos.RIGHT],
                            m_MoveInfos[(int)GamePlayCanvasPvtPos.DISCARD]);
            m_pGameInstance.Deck.DiscardCard(pCard);
            m_pGameInstance.ReleasePooled<CardCanvas>(PoolKeys.s_strCardCanvas, pCardCanvas);
        });
        m_pGameInstance.EnqueueJob(jobDiscard);
    }
}
