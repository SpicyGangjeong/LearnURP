using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class CardFrameRenderer : MonoBehaviour
{
    CGameInstance m_pGameInstance = null;
    IReadOnlyList<Card> m_vCards = null;
    CardCanvas[] m_vCardCanvasSlots;
    float3[] m_vCardSlotPositions;

    public void RenderCardFrames(DEFINES.CardPile cardPile)
    {
        if (null == m_vCards)
        {
            m_vCards = m_pGameInstance.GetCards(cardPile);

            RebuildCanvases();
        }
    }

    private void RebuildCanvases()
    {
        foreach (CardCanvas pCanvas in m_vCardCanvasSlots)
        {
            m_pGameInstance.ReleasePooled<CardCanvas>(PoolKeys.s_strCardCanvas, pCanvas);
        }

        m_vCardCanvasSlots = new CardCanvas[m_vCards.Count];
        if (!EnsureCardPool())
        {
            Debug.LogError("Failed to ensure card pool.");
        }

        RebuildSlotPositions();

        for (int i = 0; i < m_vCardCanvasSlots.Length; i++)
        {
            m_vCardCanvasSlots[i].BindCard(m_vCards[i]);
            m_vCardCanvasSlots[i].gameObject.SetActive(true);
        }
    }

    private void RebuildSlotPositions()
    {
        m_vCardSlotPositions = new float3[m_vCards.Count];
        RectTransform pBoardRect = transform as RectTransform;
        Vector3[] vCorners = new Vector3[4];
        pBoardRect.GetWorldCorners(vCorners);
    }

    void Awake()
    {
        m_pGameInstance = CGameInstance.Instance;
    }

    void Update()
    {
        
    }

    private bool EnsureCardPool()
    {
        if (false == m_pGameInstance.ObjectPools.IsRegistered(PoolKeys.s_strCardCanvas))
        {
            Debug.LogError("CardCanvas pool is not registered. Run Bootstrap first.");
            return false;
        }
        
        for (int i = 0; i < m_vCards.Count; i++)
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
