using Core;
using Core.Pool;
using Logic.Card;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;


namespace View
{
    namespace UI
    {
        public class CardFrameRenderer : MonoBehaviour
        {
            [SerializeField] RectTransform m_pContentRect = null;
            [SerializeField] Vector2 m_vCardSlotSize = new Vector2(200f, 280f);
            [SerializeField] float m_fSlotSpacing = 12f;

            CGameInstance m_pGameInstance = null;
            IReadOnlyList<CardInstance> m_vCards = null;
            CardCanvas[] m_vCardCanvasSlots = null;
            float3[] m_vCardSlotPositions = null;
            int m_iColumnCount = 1;

            public void RenderCardFrames(Defines.Enums.CardPile ePileType)
            {
                m_vCards = m_pGameInstance.Deck.GetCards(ePileType);
                if (null == m_vCards || 0 == m_vCards.Count)
                {
                    ClearCanvases();
                    return;
                }

                RebuildCanvases();
            }

            void Awake()
            {
                m_pGameInstance = CGameInstance.Instance;
                EnsureContentRect();
            }

            void EnsureContentRect()
            {
                if (null != m_pContentRect)
                {
                    return;
                }

                Transform pExisting = transform.Find("Content");
                if (null != pExisting)
                {
                    m_pContentRect = pExisting as RectTransform;
                    return;
                }

                GameObject pContentObject = new GameObject("Content", typeof(RectTransform));
                m_pContentRect = pContentObject.GetComponent<RectTransform>();
                m_pContentRect.SetParent(transform, false);
                m_pContentRect.anchorMin = new Vector2(0f, 1f);
                m_pContentRect.anchorMax = new Vector2(1f, 1f);
                m_pContentRect.pivot = new Vector2(0.5f, 1f);
                m_pContentRect.anchoredPosition = Vector2.zero;
                m_pContentRect.sizeDelta = Vector2.zero;
            }

            void ClearCanvases()
            {
                ReleaseAllSlots();
                m_vCardCanvasSlots = null;
                m_vCardSlotPositions = null;
                UpdateContentSize(0);
            }

            void ReleaseAllSlots()
            {
                if (null == m_vCardCanvasSlots)
                {
                    return;
                }

                foreach (CardCanvas pCanvas in m_vCardCanvasSlots)
                {
                    if (null == pCanvas)
                    {
                        continue;
                    }

                    m_pGameInstance.ReleasePooled<CardCanvas>(Defines.Constants.s_strCardCanvas, pCanvas);
                }
            }

            void RebuildCanvases()
            {
                ReleaseAllSlots();

                int iCardCount = m_vCards.Count;
                m_vCardCanvasSlots = new CardCanvas[iCardCount];
                if (false == EnsureCardPool())
                {
                    Debug.LogError("Failed to ensure card pool.");
                    return;
                }

                RebuildSlotPositions();

                for (int i = 0; i < iCardCount; i++)
                {
                    CardCanvas pCardCanvas = m_vCardCanvasSlots[i];
                    if (null == pCardCanvas)
                    {
                        continue;
                    }

                    pCardCanvas.BindCard(m_vCards[i]);
                    ApplySlotLayout(pCardCanvas, i);
                    pCardCanvas.gameObject.SetActive(true);
                }
            }

            int CalcColumnCount(int iCardCount)
            {
                RectTransform pLayoutRect = transform as RectTransform;
                if (null == pLayoutRect)
                {
                    return 1;
                }

                float fCellWidth = m_vCardSlotSize.x + m_fSlotSpacing;
                if (fCellWidth <= 0f)
                {
                    return 1;
                }

                float fAvailableWidth = pLayoutRect.rect.width;
                int iColumnCount = Mathf.Max(1, Mathf.FloorToInt((fAvailableWidth + m_fSlotSpacing) / fCellWidth));
                return Mathf.Min(iColumnCount, Mathf.Max(1, iCardCount));
            }

            void RebuildSlotPositions()
            {
                int iCardCount = m_vCards.Count;
                m_iColumnCount = CalcColumnCount(iCardCount);
                m_vCardSlotPositions = new float3[iCardCount];

                float fStepX = m_vCardSlotSize.x + m_fSlotSpacing;
                float fStepY = m_vCardSlotSize.y + m_fSlotSpacing;
                float fGridWidth = m_iColumnCount * m_vCardSlotSize.x + (m_iColumnCount - 1) * m_fSlotSpacing;
                float fStartX = -fGridWidth * 0.5f + m_vCardSlotSize.x * 0.5f;

                for (int i = 0; i < iCardCount; i++)
                {
                    int iRow = i / m_iColumnCount;
                    int iCol = i % m_iColumnCount;
                    float fX = fStartX + iCol * fStepX;
                    float fY = -iRow * fStepY;
                    m_vCardSlotPositions[i] = new float3(fX, fY, 0f);
                }

                int iRowCount = Mathf.CeilToInt((float)iCardCount / m_iColumnCount);
                UpdateContentSize(iRowCount, fStepY);
            }

            void UpdateContentSize(int iRowCount, float fStepY)
            {
                if (null == m_pContentRect)
                {
                    return;
                }

                float fContentHeight = 0f;
                if (iRowCount > 0)
                {
                    fContentHeight = iRowCount * fStepY - m_fSlotSpacing;
                }

                m_pContentRect.sizeDelta = new Vector2(m_pContentRect.sizeDelta.x, fContentHeight);
            }

            void UpdateContentSize(int iRowCount)
            {
                float fStepY = m_vCardSlotSize.y + m_fSlotSpacing;
                UpdateContentSize(iRowCount, fStepY);
            }

            void ApplySlotLayout(CardCanvas pCardCanvas, int iIndex)
            {
                RectTransform pCardRect = pCardCanvas.transform as RectTransform;
                if (null == pCardRect || null == m_vCardSlotPositions || iIndex >= m_vCardSlotPositions.Length)
                {
                    return;
                }

                float3 pSlotPosition = m_vCardSlotPositions[iIndex];
                pCardRect.SetParent(m_pContentRect, false);
                pCardRect.anchorMin = new Vector2(0.5f, 1f);
                pCardRect.anchorMax = new Vector2(0.5f, 1f);
                pCardRect.pivot = new Vector2(0.5f, 1f);
                pCardRect.sizeDelta = m_vCardSlotSize;
                pCardRect.anchoredPosition = new Vector2(pSlotPosition.x, pSlotPosition.y);
                pCardRect.localScale = Vector3.one;
            }

            bool EnsureCardPool()
            {
                if (false == m_pGameInstance.ObjectPools.IsRegistered(Defines.Constants.s_strCardCanvas))
                {
                    Debug.LogError("CardCanvas pool is not registered. Run Bootstrap first.");
                    return false;
                }

                EnsureContentRect();

                for (int i = 0; i < m_vCards.Count; i++)
                {
                    if (null != m_vCardCanvasSlots[i])
                    {
                        continue;
                    }

                    CardCanvas pCardCanvas = m_pGameInstance.GetPooled<CardCanvas>(Defines.Constants.s_strCardCanvas, m_pContentRect);
                    if (null == pCardCanvas)
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

    }

}

