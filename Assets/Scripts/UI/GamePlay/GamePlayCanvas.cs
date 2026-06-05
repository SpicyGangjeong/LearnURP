using TMPro;
using UnityEngine;

public class GamePlayCanvas : MonoBehaviour
{
    CGameInstance gameInstance = null;
    [SerializeField]
    private TextMeshProUGUI tmpDeckPile = null;
    [SerializeField]
    private TextMeshProUGUI tmpDiscardPile = null;
    [SerializeField]
    private TextMeshProUGUI tmpDisappearPile = null;

    private bool bDirty = false;

    void Start()
    {
        gameInstance = CGameInstance.Instance;
    }

    void Update()
    {
        ChangePileSize();
    }

    void ChangePileSize()
    {
        if (tmpDeckPile != null)
        {
            tmpDeckPile.text = gameInstance.GetPileCount(DEFINES.CardPile.DECK).ToString();
            tmpDiscardPile.text = gameInstance.GetPileCount(DEFINES.CardPile.DISCARD).ToString();
            tmpDisappearPile.text = gameInstance.GetPileCount(DEFINES.CardPile.DISAPPEARED).ToString();
        }
    }

    public void RequestEndTurn()
    {
        CGameInstance.Instance.TryEndTurn();
    }
}
