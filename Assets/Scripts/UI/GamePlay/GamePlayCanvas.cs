using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class GamePlayCanvas : MonoBehaviour
{
    CGameInstance m_pGameInstance = null;
    [SerializeField]
    TextMeshProUGUI m_pTmpDeckPile = null;
    [SerializeField]
    TextMeshProUGUI m_pTmpDiscardPile = null;
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
            m_pTmpDeckPile.text = m_pGameInstance.GetPileCount(DEFINES.ENUMS.CardPile.DECK).ToString();
            m_pTmpDiscardPile.text = m_pGameInstance.GetPileCount(DEFINES.ENUMS.CardPile.DISCARD).ToString();
            m_pTmpDisappearPile.text = m_pGameInstance.GetPileCount(DEFINES.ENUMS.CardPile.DISAPPEARED).ToString();
        }
    }

    [EnumAction(typeof(DEFINES.ENUMS.CardPile))]
    public void RenderDrawTable(int pPile)
    {
        DEFINES.ENUMS.CardPile pile = (DEFINES.ENUMS.CardPile)pPile;
        switch (pile)
        {
            case DEFINES.ENUMS.CardPile.DECK:
                m_pCardDrawTable.SetActive(true);
                break;
            case DEFINES.ENUMS.CardPile.DISCARD:
                m_pCardDrawTable.SetActive(true);
                break;
            case DEFINES.ENUMS.CardPile.DISAPPEARED:
                m_pCardDrawTable.SetActive(true);
                break;
            case DEFINES.ENUMS.CardPile.ALL:
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
