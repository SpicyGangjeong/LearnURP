using TMPro;
using UnityEngine;
using DEFINES;
using DEFINES.ENUMS;
using DEFINES.STRUCTURES;

public class GamePlayCanvas : MonoBehaviour
{
    TextMeshProUGUI m_pTmpDeckPile = null;
    TextMeshProUGUI m_pTmpDiscardPile = null;
    TextMeshProUGUI m_pTmpDisappearPile = null;

    [SerializeField]
    GameObject m_pCardDrawTable = null;
    CGameInstance m_pGameInstance = null;

    [SerializeField]
    Transform[] m_pPvts = new Transform[(int)GamePlayCanvasPvtPos.END];
    MoveInfo[] m_MoveInfos = new MoveInfo[(int)GamePlayCanvasPvtPos.END];

    void Start()
    {
        m_pGameInstance = CGameInstance.Instance;
        m_pGameInstance.m_pRequestPlayCard += RequestPlay;
        m_pGameInstance.m_pRequestDiscardCard += RequestDiscard;
        m_pGameInstance.m_pRequestDrawCard += RequestDraw;
        for (int i = 0; i <  m_MoveInfos.Length; i++)
        {
            HELPERS.ExtractMoveInfo(out m_MoveInfos[i], m_pPvts[i]);
        }
        m_pTmpDeckPile = m_pPvts[(int)GamePlayCanvasPvtPos.DECK].GetComponent<TextMeshProUGUI>();
        m_pTmpDiscardPile = m_pPvts[(int)GamePlayCanvasPvtPos.DISCARD].GetComponent<TextMeshProUGUI>();
        m_pTmpDisappearPile = m_pPvts[(int)GamePlayCanvasPvtPos.DISAPPEAR].GetComponent<TextMeshProUGUI>();
    }
    private void OnDestroy()
    {
        m_pGameInstance.m_pRequestPlayCard -= RequestPlay;
        m_pGameInstance.m_pRequestDiscardCard -= RequestDiscard;
        m_pGameInstance.m_pRequestDrawCard -= RequestDraw;
    }

    void Update()
    {
        ChangePileSize();
    }

    void ChangePileSize()
    {
        if (m_pTmpDeckPile != null)
        {
            m_pTmpDeckPile.text = m_pGameInstance.GetPileCount(CardPile.DECK).ToString();
            m_pTmpDiscardPile.text = m_pGameInstance.GetPileCount(CardPile.DISCARD).ToString();
            m_pTmpDisappearPile.text = m_pGameInstance.GetPileCount(CardPile.DISAPPEARED).ToString();
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
        CGameInstance.Instance.TryEndTurn();
    }
    public void PoolCard(Card pCard, out CardCanvas pCardCanvas)
    {
        pCardCanvas = m_pGameInstance.GetPooled<CardCanvas>(PoolKeys.s_strCardCanvas,
            m_pPvts[(int)GamePlayCanvasPvtPos.HANDBOARD]);
        
        pCardCanvas.BindCard(pCard);
        HELPERS.ApplyMoveInfo(m_MoveInfos[(int)GamePlayCanvasPvtPos.DECK], pCardCanvas.transform);
    }
    public void RequestDraw(Card pCard, CardCanvas pCardCanvas)
    {
        if (null == pCardCanvas)
        {
            PoolCard(pCard, out pCardCanvas);
        }
        CGameInstance.Instance.MoveToFieldCard(pCard);
        IJob job = new JobDrawCard(pCard, pCardCanvas);
        pCardCanvas.StartBezierMove(0.5f,
                                    m_MoveInfos[(int)GamePlayCanvasPvtPos.LEFT],
                                    m_MoveInfos[(int)GamePlayCanvasPvtPos.HANDBOARD],
                                    () => {
                                        CGameInstance.Instance.EnqueueJob(job);
                                    });
    }
    public void RequestPlay(Card pCard, CardCanvas pCardCanvas)
    {
        CGameInstance.Instance.MoveToFieldCard(pCard);
        IJob job = new JobPlayCard(pCard, pCardCanvas);
        pCardCanvas.StartBezierMove(0.5f,
                                    m_MoveInfos[(int)GamePlayCanvasPvtPos.RIGHT], 
                                    m_MoveInfos[(int)GamePlayCanvasPvtPos.DISCARD],
                                    () => { CGameInstance.Instance.EnqueueJob(job); 
                                });
    }
    public void RequestDiscard(Card pCard, CardCanvas pCardCanvas)
    {
        CGameInstance.Instance.MoveToFieldCard(pCard);
        IJob job = new JobDiscardCard(pCard, pCardCanvas);
        pCardCanvas.StartBezierMove(0.5f,
                                    m_MoveInfos[(int)GamePlayCanvasPvtPos.RIGHT],
                                    m_MoveInfos[(int)GamePlayCanvasPvtPos.DISCARD],
                                    () => {
                                        CGameInstance.Instance.EnqueueJob(job);
                                    });
    }
}
