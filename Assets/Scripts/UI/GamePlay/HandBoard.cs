using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class HandBoard : MonoBehaviour
{
    private const int s_iMaxHandSlots = 10;
    [FormerlySerializedAs("_maxSlots")]
    [SerializeField] private int m_iMaxSlots = s_iMaxHandSlots;
    [FormerlySerializedAs("_curveHeight")]
    [SerializeField] private float m_fCurveHeight = 120f;
    CGameInstance m_pGameInstance = null;
    IReadOnlyList<Card> m_vHandCards = null;
    CardCanvas[] m_vCardCanvasSlots = new CardCanvas[s_iMaxHandSlots];

    Vector3 m_pStartPos;
    Vector3 m_pEndPos;
    Vector3 m_pControlPos;
    Vector3[] m_vHandPos;

    int m_iCurrentHand = 10;

    private void CalcHandPos(Vector3 pCenterPos)
    {
        m_vHandPos = new Vector3[Mathf.Max(1, m_iMaxSlots)];
        if (m_iCurrentHand == 1)
        {
            m_vHandPos[0] = pCenterPos;
            return;
        }

        float fSlotStep = 1f / Mathf.Max(1, m_iMaxSlots - 1);
        float fSpan = fSlotStep * (m_iCurrentHand - 1);
        float fStartT = 0.5f - fSpan * 0.5f;

        for (int i = 0; i < m_iCurrentHand; i++)
        {
            float fT = Mathf.Clamp01(fStartT + i * fSlotStep);
            m_vHandPos[i] = GetQuadraticBezierPoint(fT, m_pStartPos, m_pControlPos, m_pEndPos);
        }
    }
    public Vector3 GetSlotPosition(int iIndex)
    {
        if (m_vHandPos == null || m_vHandPos.Length == 0)
            return transform.position;

        iIndex = Mathf.Clamp(iIndex, 0, m_vHandPos.Length - 1);
        return m_vHandPos[iIndex];
    }
    private Vector3 GetQuadraticBezierPoint(float fT, Vector3 p0, Vector3 p1, Vector3 p2)
    {
        float fOneMinusT = 1f - fT;
        return fOneMinusT * fOneMinusT * p0 + 2f * fOneMinusT * fT * p1 + fT * fT * p2;
    }

    void Awake()
    {
        RectTransform pRectTransform = GetComponent<RectTransform>();
        Vector3[] vCorners = new Vector3[4];
        pRectTransform.GetWorldCorners(vCorners);

        m_pStartPos = Vector3.Lerp(vCorners[0], vCorners[1], 0.5f); // 좌측 중앙
        m_pEndPos = Vector3.Lerp(vCorners[3], vCorners[2], 0.5f);   // 우측 중앙
        Vector3 pCenterPos = Vector3.Lerp(m_pStartPos, m_pEndPos, 0.5f);
        m_pControlPos = pCenterPos + Vector3.up * m_fCurveHeight;

        CalcHandPos(pCenterPos);
    }

    void Start()
    {
        m_pGameInstance = CGameInstance.Instance;
        if (null == m_vHandCards)
        {
            m_vHandCards = m_pGameInstance.GetCards(DEFINES.CardPile.HAND);
        }
        m_pGameInstance.OnDrawCard += OnHandChanged;
        m_pGameInstance.OnPlayCard += OnHandChanged;
        m_pGameInstance.OnDiscardCard += OnHandChanged;
        m_pGameInstance.OnEndTurn += OnEndTurn;
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

    private void Update()
    {
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
        if (m_vHandCards == null)
        {
            return;
        }

        int iCardCount = Mathf.Min(m_vHandCards.Count, m_iMaxSlots);
        m_iCurrentHand = Mathf.Max(1, iCardCount);

        RectTransform pBoardRect = transform as RectTransform;
        if (pBoardRect == null)
        {
            Debug.LogError("HandBoard requires RectTransform.");
            return;
        }

        Vector3[] vCorners = new Vector3[4];
        pBoardRect.GetWorldCorners(vCorners);
        m_pStartPos = Vector3.Lerp(vCorners[0], vCorners[1], 0.5f);
        m_pEndPos = Vector3.Lerp(vCorners[3], vCorners[2], 0.5f);
        Vector3 pCenterPos = Vector3.Lerp(m_pStartPos, m_pEndPos, 0.5f);
        m_pControlPos = pCenterPos + Vector3.up * m_fCurveHeight;
        CalcHandPos(pCenterPos);

        if (!EnsureCardPool())
        {
            return;
        }

        for (int i = 0; i < iCardCount; i++)
        {
            CardCanvas pCardCanvas = m_vCardCanvasSlots[i];
            if (pCardCanvas == null)
            {
                continue;
            }

            pCardCanvas.gameObject.SetActive(true);
            pCardCanvas.BindCard(m_vHandCards[i]);
            pCardCanvas.transform.position = GetSlotPosition(i);
            pCardCanvas.transform.localScale = Vector3.one;
        }

        for (int i = iCardCount; i < m_vCardCanvasSlots.Length; i++)
        {
            if (m_vCardCanvasSlots[i] != null)
            {
                m_vCardCanvasSlots[i].gameObject.SetActive(false);
            }
        }
    }

    private bool EnsureCardPool()
    {
        if (false == m_pGameInstance.ObjectPools.IsRegistered(PoolKeys.s_strCardCanvas))
        {
            Debug.LogError("CardCanvas pool is not registered. Run Bootstrap first.");
            return false;
        }

        for (int i = 0; i < m_iMaxSlots; i++)
        {
            if (m_vCardCanvasSlots[i] != null)
            {
                continue;
            }

            CardCanvas pCardCanvas = m_pGameInstance.GetPooled<CardCanvas>(PoolKeys.s_strCardCanvas, transform);
            if (pCardCanvas == null)
            {
                Debug.LogError("Failed to get CardCanvas from pool.");
                return false;
            }

            pCardCanvas.gameObject.SetActive(false);
            m_vCardCanvasSlots[i] = pCardCanvas;
        }

        return true;
    }
}
