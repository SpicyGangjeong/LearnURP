using DEFINES;
using DEFINES.STRUCTURES;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.Rendering.DebugUI;

public class HandBoard : MonoBehaviour
{
    static private readonly int s_iMaxHandSlots = 10;
    static private readonly float s_fCurveHeight = 120f;

    readonly Dictionary<Card, CardCanvas> m_vCardCanvases = new Dictionary<Card, CardCanvas>();
    IReadOnlyList<Card> m_vHandCards = null;
    CGameInstance m_pGameInstance = null;

    MoveInfo[] m_vHandMoveInfo = new MoveInfo[s_iMaxHandSlots];

    void Start()
    {
        m_pGameInstance = CGameInstance.Instance;
        m_pGameInstance.OnPlayCard += OnPlayCard;
        m_pGameInstance.OnDiscardCard += OnDiscardCard;
        m_pGameInstance.OnEndTurn += OnEndTurn;
        m_pGameInstance.m_pOnHandboardInsertCard += InsertCard;
        m_pGameInstance.m_pOnHandboardPopCard += PopCard;

        m_vHandCards = m_pGameInstance.GetCards(DEFINES.ENUMS.CardPile.HAND);

        SyncExistingHand();
        UpdateHandLayout();
    }

    void OnDestroy()
    {
        if (null != m_pGameInstance)
        {
            m_pGameInstance.OnPlayCard -= OnPlayCard;
            m_pGameInstance.OnDiscardCard -= OnDiscardCard;
            m_pGameInstance.OnEndTurn -= OnEndTurn;
            m_pGameInstance.m_pOnHandboardInsertCard -= InsertCard;
            m_pGameInstance.m_pOnHandboardPopCard -= PopCard;
        }

        ReleaseAllHandCanvases();
    }
    public void PopCard(Card pCard)
    {
        RemoveHandCard(pCard);
        UpdateHandLayout();
    }
    public void InsertCard(Card pCard, CardCanvas pCardCanvas)
    {
        BindCard(pCard, pCardCanvas);
        UpdateHandLayout();
    }

    void OnPlayCard(Card pCard)
    {
        RemoveHandCard(pCard);
        UpdateHandLayout();
    }

    void OnDiscardCard(Card pCard)
    {
        CardCanvas pCardCanvas = RemoveHandCard(pCard);
        UpdateHandLayout();
    }

    void OnEndTurn()
    {
        Dictionary<Card, CardCanvas> CapturedCanvases =  new Dictionary<Card, CardCanvas>(m_vCardCanvases);
        m_vCardCanvases.Clear();
        foreach (KeyValuePair<Card, CardCanvas> pairCard in CapturedCanvases)
        {
            m_pGameInstance.RequestDiscardCard(pairCard.Key, pairCard.Value);
        }
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

            BindCard(pCard, GetPoolCanvas(pCard));
        }
    }

    CardCanvas GetPoolCanvas(Card pCard)
    {
        return m_pGameInstance.GetPooled<CardCanvas>(PoolKeys.s_strCardCanvas, transform);
    }

    private void BindCard(Card pCard, CardCanvas pCardCanvas)
    {
        pCardCanvas.BindCard(pCard);
        m_vCardCanvases[pCard] = pCardCanvas;
    }

    CardCanvas RemoveHandCard(Card pCard)
    {
        m_vCardCanvases.TryGetValue(pCard, out CardCanvas pCardCanvas);
        m_vCardCanvases.Remove(pCard);
        return pCardCanvas;
    }

    void ReleaseAllHandCanvases()
    {
        if (false == m_pGameInstance.ObjectPools.IsRegistered(PoolKeys.s_strCardCanvas))
        {
            m_vCardCanvases.Clear();
            return;
        }

        foreach (CardCanvas pCardCanvas in m_vCardCanvases.Values)
        {
            m_pGameInstance.ReleasePooled<CardCanvas>(PoolKeys.s_strCardCanvas, pCardCanvas);
        }

        m_vCardCanvases.Clear();
    }

    void UpdateHandLayout()
    {
        int iActiveCardCount = m_vCardCanvases.Count;
        if (0 == iActiveCardCount)
        {
            return;
        }

        RectTransform pBoardRect = transform as RectTransform;
        Vector3[] vCorners = new Vector3[4];
        pBoardRect.GetWorldCorners(vCorners);
        Vector3 pStartPos = Vector3.Lerp(vCorners[0], vCorners[1], 0.25f);
        Vector3 pEndPos = Vector3.Lerp(vCorners[3], vCorners[2], 0.25f);
        Vector3 pCentralPos = Vector3.Lerp(pStartPos, pEndPos, 0.5f) + Vector3.up * s_fCurveHeight;

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
        {
            int i = 0;
            foreach (CardCanvas pCardCanvas in m_vCardCanvases.Values)
            {
                pCardCanvas.StartLinearMove(0.5f, m_vHandMoveInfo[i++], DEFINES.HELPERS.EmptyEvent);
            }
        }
    }
}
