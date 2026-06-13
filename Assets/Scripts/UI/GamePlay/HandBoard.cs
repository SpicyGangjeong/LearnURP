using System.Collections.Generic;
using UnityEngine;

public class HandBoard : MonoBehaviour
{
    static private readonly int s_iMaxHandSlots = 10;
    static private readonly float s_fCurveHeight = 120f;

    [SerializeField] private float m_fCurveHeight = s_fCurveHeight;
    readonly Dictionary<Card, CardCanvas> m_vCardCanvases = new Dictionary<Card, CardCanvas>();
    IReadOnlyList<Card> m_vHandCards = null;
    CGameInstance m_pGameInstance = null;

    DEFINES.STRUCTURES.MoveInfo[] m_vHandMoveInfo = new DEFINES.STRUCTURES.MoveInfo[s_iMaxHandSlots];

    void Start()
    {
        m_pGameInstance = CGameInstance.Instance;
        m_pGameInstance.OnDrawCard += OnDrawCard;
        m_pGameInstance.OnPlayCard += OnPlayCard;
        m_pGameInstance.OnDiscardCard += OnDiscardCard;
        m_pGameInstance.OnEndTurn += OnEndTurn;

        m_vHandCards = m_pGameInstance.GetCards(DEFINES.ENUMS.CardPile.HAND);
        SyncExistingHand();
        UpdateHandLayout();
    }

    void OnDestroy()
    {
        if (null != m_pGameInstance)
        {
            m_pGameInstance.OnDrawCard -= OnDrawCard;
            m_pGameInstance.OnPlayCard -= OnPlayCard;
            m_pGameInstance.OnDiscardCard -= OnDiscardCard;
            m_pGameInstance.OnEndTurn -= OnEndTurn;
        }

        ReleaseAllHandCanvases();
    }

    void OnDrawCard(Card pCard)
    {
        if (null == pCard || m_vCardCanvases.ContainsKey(pCard))
        {
            return;
        }

        SpawnHandCanvas(pCard);
        UpdateHandLayout();
    }

    void OnPlayCard(Card pCard)
    {
        ReleaseHandCanvas(pCard);
        UpdateHandLayout();
    }

    void OnDiscardCard(Card pCard)
    {
        ReleaseHandCanvas(pCard);
        UpdateHandLayout();
    }

    void OnEndTurn()
    {
        UpdateHandLayout();
    }

    void SyncExistingHand()
    {
        if (null == m_vHandCards)
        {
            return;
        }

        foreach (Card pCard in m_vHandCards)
        {
            if (null == pCard || m_vCardCanvases.ContainsKey(pCard))
            {
                continue;
            }

            SpawnHandCanvas(pCard);
        }
    }

    void SpawnHandCanvas(Card pCard)
    {
        if (null == pCard)
        {
            return;
        }

        CardCanvas pCardCanvas = m_pGameInstance.GetPooled<CardCanvas>(PoolKeys.s_strCardCanvas, transform);
        if (null == pCardCanvas)
        {
            Debug.LogError("Failed to get CardCanvas from pool.");
            return;
        }

        pCardCanvas.BindCard(pCard);
        m_vCardCanvases[pCard] = pCardCanvas;
    }

    void ReleaseHandCanvas(Card pCard)
    {
        if (null == pCard)
        {
            return;
        }

        if (false == m_vCardCanvases.TryGetValue(pCard, out CardCanvas pCardCanvas))
        {
            return;
        }

        m_vCardCanvases.Remove(pCard);
        m_pGameInstance.ReleasePooled<CardCanvas>(PoolKeys.s_strCardCanvas, pCardCanvas);
    }

    void ReleaseAllHandCanvases()
    {
        if (null == m_pGameInstance)
        {
            m_vCardCanvases.Clear();
            return;
        }

        foreach (CardCanvas pCardCanvas in m_vCardCanvases.Values)
        {
            if (null == pCardCanvas)
            {
                continue;
            }

            m_pGameInstance.ReleasePooled<CardCanvas>(PoolKeys.s_strCardCanvas, pCardCanvas);
        }

        m_vCardCanvases.Clear();
    }

    void UpdateHandLayout()
    {
        if (null == m_vHandCards)
        {
            return;
        }

        int iActiveCardCount = m_vHandCards.Count;
        if (0 == iActiveCardCount)
        {
            return;
        }

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
            float fT = (1 == iActiveCardCount) ? 0.5f : Mathf.Clamp01(fStartT + i * fSlotStep);
            Vector3 vPos = DEFINES.HELPERS.GetQuadraticBezierPoint(fT, pStartPos, pCentralPos, pEndPos);
            Vector3 vTangent = DEFINES.HELPERS.GetQuadraticBezierTangent(fT, pStartPos, pCentralPos, pEndPos);
            float fRotZ = (vTangent.sqrMagnitude > DEFINES.CONSTANTS.FLT_EPSILON5) ? Mathf.Atan2(vTangent.y, vTangent.x) * Mathf.Rad2Deg : 0f;
            m_vHandMoveInfo[i].vPosition = vPos;
            m_vHandMoveInfo[i].vRotQ = Quaternion.Euler(0f, 0f, fRotZ);
        }

        for (int i = 0; i < iActiveCardCount; i++)
        {
            Card pCard = m_vHandCards[i];
            if (false == m_vCardCanvases.TryGetValue(pCard, out CardCanvas pCardCanvas) || null == pCardCanvas)
            {
                continue;
            }

            pCardCanvas.gameObject.SetActive(true);
            pCardCanvas.StartMove(m_vHandMoveInfo[i]);
        }
    }
}
