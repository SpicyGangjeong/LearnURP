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
        m_pGameInstance = CGameInstance.Instance;
        m_pGameInstance.OnDrawCard += OnHandChanged;
        m_pGameInstance.OnPlayCard += OnHandChanged;
        m_pGameInstance.OnDiscardCard += OnHandChanged;
        m_pGameInstance.OnEndTurn += OnEndTurn;

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
            RectTransform pBoardRect = transform as RectTransform;
            Vector3[] vCorners = new Vector3[4];
            pBoardRect.GetWorldCorners(vCorners);
            Vector3 pStartPos = Vector3.Lerp(vCorners[0], vCorners[1], 0.25f);
            Vector3 pEndPos = Vector3.Lerp(vCorners[3], vCorners[2], 0.25f);
            Vector3 pCentralPos = Vector3.Lerp(pStartPos, pEndPos, 0.5f) + Vector3.up * m_fCurveHeight;

            float fSlotStep = 1f / Mathf.Max(1, s_iMaxHandSlots - 1);
            float fSpan = fSlotStep * (iActiveCardCount - 1);
            float fStartT = 0.5f - fSpan * 0.5f;

            for (int i = 0; i < iActiveCardCount; i++)
            {
                float fT = (iActiveCardCount == 1) ? 0.5f : Mathf.Clamp01(fStartT + i * fSlotStep);
                Vector3 vPos = DEFINES.HELPERS.GetQuadraticBezierPoint(fT, pStartPos, pCentralPos, pEndPos);
                Vector3 vTangent = DEFINES.HELPERS.GetQuadraticBezierTangent(fT, pStartPos, pCentralPos, pEndPos);
                float fRotZ = (vTangent.sqrMagnitude > DEFINES.CONSTANTS.FLT_EPSILON5) ? Mathf.Atan2(vTangent.y, vTangent.x) * Mathf.Rad2Deg : 0f;
                m_vHandMoveInfo[i].vPosition = vPos;
                m_vHandMoveInfo[i].vRotQ = Quaternion.Euler(0f, 0f, fRotZ);
            }
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
