using DEFINES;
using DEFINES.STRUCTURES;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class HandBoard : MonoBehaviour
{
    static private readonly int s_iMaxHandSlots = 10;
    static private readonly float s_fCurveHeight = 120f;

    readonly List<KeyValuePair<Card, CardCanvas>> m_vCardCanvases = new List<KeyValuePair<Card, CardCanvas>>();
    IReadOnlyList<Card> m_vHandCards = null;
    CGameInstance m_pGameInstance = null;
    GamePlayCanvas m_pGamePlayCanvas = null;

    MoveInfo[] m_vHandMoveInfo = new MoveInfo[s_iMaxHandSlots];

    void Start()
    {
        m_pGameInstance = CGameInstance.Instance;
        m_pGamePlayCanvas = GamePlayCanvas.Instance;
        if (null == m_pGamePlayCanvas)
        {
            m_pGamePlayCanvas = FindFirstObjectByType<GamePlayCanvas>();
        }
        m_pGameInstance.Deck.m_pOnCardPlayed += OnCardPlayed;
        m_pGameInstance.Deck.m_pOnCardDiscarded += OnCardDiscarded;
        m_pGameInstance.Deck.m_pOnTurnEnded += OnTurnEnded;

        m_vHandCards = m_pGameInstance.Deck.GetCards(DEFINES.ENUMS.CardPile.HAND);
    }

    void OnDestroy()
    {
        if (null != m_pGameInstance)
        {
            m_pGameInstance.Deck.m_pOnCardPlayed -= OnCardPlayed;
            m_pGameInstance.Deck.m_pOnCardDiscarded -= OnCardDiscarded;
            m_pGameInstance.Deck.m_pOnTurnEnded -= OnTurnEnded;
        }

        ReleaseAllHandCanvases();
    }

    public void PopCardForPresentation(Card pCard)
    {
        RemoveHandCard(pCard);
        UpdateHandLayout();
    }

    public void OnDragBegin(Card pCard)
    {
        RemoveHandCard(pCard);
        UpdateHandLayout();
    }

    public void OnDragCancel(Card pCard, CardCanvas pCardCanvas)
    {
        BindCard(pCard, pCardCanvas);
        UpdateHandLayout();
    }

    public void BindCard(Card pCard, CardCanvas pCardCanvas)
    {
        int FindProperIndex(CardCanvas pCardCanvas, int iIndex)
        {
            Vector3 vSrcPos = pCardCanvas.transformHandle.localPosition;
            for (int i = 0; i < m_vCardCanvases.Count; ++i)
            {
                Vector3 vTargetPos = m_vCardCanvases[i].Value.transformHandle.localPosition;
                if (vTargetPos.x > vSrcPos.x)
                {
                    iIndex = i;
                    break;
                }
            }

            return iIndex;
        }

        pCardCanvas.BindCard(pCard);
        KeyValuePair<Card, CardCanvas> element = new KeyValuePair<Card, CardCanvas>(pCard, pCardCanvas);
        m_vCardCanvases.Insert(FindProperIndex(pCardCanvas, m_vCardCanvases.Count), element);

    }

    public void UpdateHandLayout()
    {
        void CalcHandPositions(int iActiveCardCount)
        {
            Vector3[] vCorners = new Vector3[4];
            (transform as RectTransform).GetWorldCorners(vCorners);

            Vector3 pStartPos = Vector3.Lerp(vCorners[0], vCorners[1], 0.25f);
            Vector3 pEndPos = Vector3.Lerp(vCorners[3], vCorners[2], 0.25f);
            Vector3 pCentralPos = Vector3.Lerp(pStartPos, pEndPos, 0.5f) + Vector3.up * s_fCurveHeight;

            float fSlotStep = 1f / Mathf.Max(1, s_iMaxHandSlots - 1);
            float fSpan = fSlotStep * (iActiveCardCount - 1);
            float fStartT = 0.5f - fSpan * 0.5f;

            for (int i = 0; i < iActiveCardCount; i++)
            {
                float fT = (1 == iActiveCardCount) ? 0.5f : Mathf.Clamp01(fStartT + i * fSlotStep);
                Vector3 vPos = HELPERS.GetQuadraticBezierPoint(fT, pStartPos, pCentralPos, pEndPos);
                Vector3 vTangent = HELPERS.GetQuadraticBezierTangent(fT, pStartPos, pCentralPos, pEndPos);
                float fRotZ = (vTangent.sqrMagnitude > CONSTANTS.FLT_EPSILON5) ? Mathf.Atan2(vTangent.y, vTangent.x) * Mathf.Rad2Deg : 0f;
                m_vHandMoveInfo[i].vPosition = vPos;
                m_vHandMoveInfo[i].vRotQ = Quaternion.Euler(0f, 0f, fRotZ);
            }
        }
        void StartLinearMove(int iActiveCardCount)
        {
            for (int i = 0; i < iActiveCardCount; i++)
            {
                m_vCardCanvases[i].Value.StartLinearMove(
                    (float)CONSTANTS.TIME_MS_SORTING_TIMEOUT / CONSTANTS.TIME_MS_ASEC,
                    m_vHandMoveInfo[i], HELPERS.EmptyEvent);
                m_vCardCanvases[i].Value.transform.SetAsLastSibling();
            }
        }


        int iActiveCardCount = m_vCardCanvases.Count;
        if (0 == iActiveCardCount)
        {
            return;
        }
        CalcHandPositions(iActiveCardCount);
        StartLinearMove(iActiveCardCount);
    }

    void OnCardPlayed(Card pCard)
    {
        RemoveHandCard(pCard);
        UpdateHandLayout();
    }

    void OnCardDiscarded(Card pCard)
    {
        RemoveHandCard(pCard);
        UpdateHandLayout();
    }

    void OnTurnEnded()
    {
        List<KeyValuePair<Card, CardCanvas>> vCapturedCanvases = new List<KeyValuePair<Card, CardCanvas>>(m_vCardCanvases);
        foreach (KeyValuePair<Card, CardCanvas> pairCard in vCapturedCanvases)
        {
            if (null != m_pGamePlayCanvas)
            {
                m_pGamePlayCanvas.PresentDiscard(pairCard.Key, pairCard.Value);
            }
        }
    }

    void RemoveHandCard(Card pCard)
    {
        m_vCardCanvases.RemoveAll((element) => { return element.Key == pCard; });
    }

    void ReleaseAllHandCanvases()
    {
        if (false == m_pGameInstance.ObjectPools.IsRegistered(PoolKeys.s_strCardCanvas))
        {
            m_vCardCanvases.Clear();
            return;
        }

        foreach (KeyValuePair<Card, CardCanvas> pair in m_vCardCanvases)
        {
            m_pGameInstance.ReleasePooled<CardCanvas>(PoolKeys.s_strCardCanvas, pair.Value);
        }

        m_vCardCanvases.Clear();
    }
}
