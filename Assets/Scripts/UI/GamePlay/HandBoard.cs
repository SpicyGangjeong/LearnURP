using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class HandBoard : MonoBehaviour
{
    static private readonly int s_iMaxHandSlots = 10;
    static private readonly float s_fCurveHeight = 120f;

    [SerializeField] private int m_iMaxSlots = s_iMaxHandSlots;
    [SerializeField] private float m_fCurveHeight = s_fCurveHeight;
    CGameInstance m_pGameInstance = null;
    IReadOnlyList<Card> m_vHandCards = null;
    CardCanvas[] m_vCardCanvasSlots = new CardCanvas[s_iMaxHandSlots];

    Vector3 m_pStartPos;
    Vector3 m_pEndPos;
    Vector3 m_pControlPos;
    Vector3[] m_vHandPos;

    int m_iCurrentHand = 10;

    void ScanCurrentHand()
    {
        int iActiveCount = 0;
        for (int i = 0; i < m_vCardCanvasSlots.Length; i++)
        {
            if (true == m_vCardCanvasSlots[i].gameObject.activeInHierarchy)
            {
                iActiveCount++;
            }

        }
        m_fCurveHeight = (iActiveCount + 1) * s_fCurveHeight;

        RebuildHand();
    }

    private void CalcHandPos(Vector3 pCenterPos)
    {
        Vector3 GetQuadraticBezierPoint(float fT, Vector3 p0, Vector3 p1, Vector3 p2)
        {
            float fOneMinusT = 1f - fT;
            return fOneMinusT * fOneMinusT * p0 + 2f * fOneMinusT * fT * p1 + fT * fT * p2;
        }

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
    void Awake()
    {
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
        ScanCurrentHand();
    }
    private void OnEndTurn()
    {
        RebuildHand();
    }
    private void RebuildHand()
    {
        Vector3 GetQuadraticBezierTangent(float fT, Vector3 p0, Vector3 p1, Vector3 p2)
        {
            float fOneMinusT = 1f - fT;
            return 2f * fOneMinusT * (p1 - p0) + 2f * fT * (p2 - p1);
        }

        float GetSlotT(int iIndex)
        {
            if (m_iCurrentHand <= 1)
            {
                return 0.5f;
            }

            float fSlotStep = 1f / Mathf.Max(1, m_iMaxSlots - 1);
            float fSpan = fSlotStep * (m_iCurrentHand - 1);
            float fStartT = 0.5f - fSpan * 0.5f;
            return Mathf.Clamp01(fStartT + iIndex * fSlotStep);
        }

        float GetSlotRotationZ(int iIndex)
        {
            float fT = GetSlotT(iIndex);
            Vector3 vTangent = GetQuadraticBezierTangent(fT, m_pStartPos, m_pControlPos, m_pEndPos);
            if (vTangent.sqrMagnitude < 0.0001f)
            {
                return 0f;
            }

            return Mathf.Atan2(vTangent.y, vTangent.x) * Mathf.Rad2Deg;
        }

        int iCardCount = Mathf.Min(m_vHandCards.Count, m_iMaxSlots);
        m_iCurrentHand = Mathf.Max(1, iCardCount);

        RectTransform pBoardRect = transform as RectTransform;
        Vector3[] vCorners = new Vector3[4];
        pBoardRect.GetWorldCorners(vCorners);
        m_pStartPos = Vector3.Lerp(vCorners[0], vCorners[1], 0.25f);
        m_pEndPos = Vector3.Lerp(vCorners[3], vCorners[2], 0.25f);
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
            pCardCanvas.transform.localEulerAngles = new Vector3(0f, 0f, GetSlotRotationZ(i));
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
