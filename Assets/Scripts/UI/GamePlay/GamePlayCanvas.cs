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

    public void RequestEndTurn()
    {
        CGameInstance.Instance.TryEndTurn();
    }
}
