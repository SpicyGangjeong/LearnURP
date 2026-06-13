using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class HandBoard : MonoBehaviour
{
    static private readonly int s_iMaxHandSlots = 10;
    static private readonly float s_fCurveHeight = 120f;

    [SerializeField] private float m_fCurveHeight = s_fCurveHeight;
    readonly CardCanvas[] m_vCardCanvasSlots = new CardCanvas[s_iMaxHandSlots];
    IReadOnlyList<Card> m_vHandCards = null;
    CGameInstance m_pGameInstance = null;

    DEFINES.STRUCTURES.MoveInfo[] m_vHandMoveInfo = new DEFINES.STRUCTURES.MoveInfo[s_iMaxHandSlots];

    void Start()
    {
        m_pGameInstance.OnDrawCard += OnHandChanged;
        m_pGameInstance.OnPlayCard += OnHandChanged;
        m_pGameInstance.OnDiscardCard += OnHandChanged;
        m_pGameInstance.OnEndTurn += OnEndTurn;

        m_pGameInstance = CGameInstance.Instance;
        m_vHandCards = m_pGameInstance.GetCards(DEFINES.ENUMS.CardPile.HAND);
        EnsureCardPool();
        RebuildHand();
    }

    private void OnDestroy()
    {
        if (m_pGameInstance != null)
        {
            m_pGameInstance.OnDrawCard -= OnHandChanged;
            m_pGameInstance.OnPlayCard -= OnHandChanged;
            m_pGameInstance.OnDiscardCard -= OnHandChanged;
            m_pGameInstance.OnEndTurn -= OnEndTurn;
        }
    }
    private void OnHandChanged(Card pCard)
    {
        RebuildHand();
    }
    private void OnEndTurn()
    {
        RebuildHand();
    }
    private void RebuildHand()
    {
        /*
         
         */
        int iActiveCardCount = m_vHandCards.Count;
        {

        }
        for (int i = 0; i < iActiveCardCount; i++)
        {
            CardCanvas pCardCanvas = m_vCardCanvasSlots[i];
            pCardCanvas.gameObject.SetActive(true);
            pCardCanvas.BindCard(m_vHandCards[i]);
            pCardCanvas.StartMove(m_vHandMoveInfo[i]);
        }
        for (int i = iActiveCardCount; i < m_vCardCanvasSlots.Length; i++)
        {
            m_vCardCanvasSlots[i].gameObject.SetActive(false);
        }
    }
    private void EnsureCardPool()
    {
        for (int i = 0; i < s_iMaxHandSlots; i++)
        {
            CardCanvas pCardCanvas = m_pGameInstance.GetPooled<CardCanvas>(PoolKeys.s_strCardCanvas, transform);
            pCardCanvas.gameObject.SetActive(false);
            m_vCardCanvasSlots[i] = pCardCanvas;
        }
    }
}
