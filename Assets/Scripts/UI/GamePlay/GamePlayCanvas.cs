using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class GamePlayCanvas : MonoBehaviour
{
    CGameInstance m_pGameInstance = null;
    [FormerlySerializedAs("tmpDeckPile")]
    [SerializeField]
    TextMeshProUGUI m_pTmpDeckPile = null;
    [FormerlySerializedAs("tmpDiscardPile")]
    [SerializeField]
    TextMeshProUGUI m_pTmpDiscardPile = null;
    [FormerlySerializedAs("tmpDisappearPile")]
    [SerializeField]
    TextMeshProUGUI m_pTmpDisappearPile = null;
    [SerializeField]
    GameObject m_pCardDrawTable = null;

    bool m_bDirty = false;

    void Start()
    {
        m_pGameInstance = CGameInstance.Instance;
    }

    void Update()
    {
        ChangePileSize();
    }

    void ChangePileSize()
    {
        if (m_pTmpDeckPile != null)
        {
            m_pTmpDeckPile.text = m_pGameInstance.GetPileCount(DEFINES.CardPile.DECK).ToString();
            m_pTmpDiscardPile.text = m_pGameInstance.GetPileCount(DEFINES.CardPile.DISCARD).ToString();
            m_pTmpDisappearPile.text = m_pGameInstance.GetPileCount(DEFINES.CardPile.DISAPPEARED).ToString();
        }
    }

    [EnumAction(typeof(DEFINES.CardPile))]
    public void RenderDrawTable(int pPile)
    {
        DEFINES.CardPile pile = (DEFINES.CardPile)pPile;
        switch (pile)
        {
            case DEFINES.CardPile.DECK:
                m_pCardDrawTable.SetActive(true);
                break;
            case DEFINES.CardPile.DISCARD:
                m_pCardDrawTable.SetActive(true);
                break;
            case DEFINES.CardPile.DISAPPEARED:
                m_pCardDrawTable.SetActive(true);
                break;
            case DEFINES.CardPile.ALL:
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
}
