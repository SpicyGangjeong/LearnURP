using Core;
using Core.Pool;
using Cysharp.Threading.Tasks;
using Defines;
using Defines.Structures;
using Logic.Card;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.U2D;
using UnityEngine.UI;

namespace View
{
    namespace UI
    {
        interface ICardPointerHandler :
            IPointerEnterHandler, IPointerExitHandler,
            IPointerMoveHandler, IPointerUpHandler, IPointerDownHandler,
            IBeginDragHandler, IDragHandler, IEndDragHandler
        {
        }
        public class CardCanvas : MonoBehaviour, IPoolable, ICardPointerHandler
        {
            private static readonly Vector2 s_vDefaultAnchoredPosition = new Vector2(0f, -0.5f);
            private static readonly Vector2 s_vDefaultAnchor = new Vector2(0.5f, 0.5f);
            private static readonly Vector2 s_vDefaultPivot = new Vector2(0.5f, 0.5f);
            private static readonly Vector2 s_vDefaultSizeDelta = new Vector2(255f, 386f);
            private static int s_iCardCanvasPoolID = 0;
            [SerializeField] TextMeshProUGUI m_pSlotName = null;
            [SerializeField] TextMeshProUGUI m_pSlotCost = null;
            [SerializeField] TextMeshProUGUI m_pSlotDescription = null;

            [SerializeField] UnityEngine.UI.Image m_pSlotImage = null;
            [SerializeField] UnityEngine.UI.Image m_pSlotTypeImage = null;
            [SerializeField] UnityEngine.UI.Image m_pSlotQualityImage = null;

            [SerializeField] Outline m_pSlotHighlight = null;
            [SerializeField] SpriteAtlas m_pImgAtlasPortrait = null;
            [SerializeField] SpriteAtlas m_pImgAtlasType = null;
            [SerializeField] SpriteAtlas m_pImgAtlasQuality = null;

            CardInstance m_pRefCard = null;
            public bool bHighlighted { get; private set; } = false;

            LerpInfo m_LerpInfo;

            public void StartMove(in LerpInfo lerpInfo)
            {
                m_LerpInfo = lerpInfo;
            }
            private void InstantMove(in MoveInfo moveinfo)
            {
                m_LerpInfo.SetFinish();
                Helpers.ApplyMoveInfo(moveinfo, transform);
            }

            public void Update()
            {
                if (true == m_LerpInfo.IsLerping)
                {
                    m_LerpInfo.Progress();
                    Helpers.ApplyMoveInfo(m_LerpInfo.GetMoveInfo(), transform);
                }
            }
            public void BindCard(CardInstance pCard)
            {
                m_pRefCard = pCard;
                if (null == pCard || null == pCard.Data)
                {
                    return;
                }
                BindCardData(m_pRefCard.Data);
            }
            public void BindCardData(in CardData cardData)
            {
                m_pSlotName.text = cardData.Name;
                m_pSlotCost.text = cardData.Cost.ToString();
                m_pSlotDescription.text = cardData.Description;

                Sprite imgPortrait = null;
                switch (cardData.Portrait)
                {
                    case CardData.CardPortrait.NONE:
                    case CardData.CardPortrait.END:
                        break;
                    default:
                        imgPortrait = Helpers.RequireAtlasSprite(
                            m_pImgAtlasPortrait,
                            cardData.Portrait.ToString(),
                            "CardPortrait");
                        break;
                }
                m_pSlotImage.sprite = imgPortrait;

                Sprite imgType = null;
                switch (cardData.Type)
                {
                    case CardData.CardType.ATTACK:
                    case CardData.CardType.DEFENSE:
                    case CardData.CardType.MAGIC:
                    case CardData.CardType.ITEM:
                        imgType = Helpers.RequireAtlasSprite(
                            m_pImgAtlasType,
                            cardData.Type.ToString(),
                            "CardType");
                        break;
                    default:
                        break;
                }
                m_pSlotTypeImage.sprite = imgType;

                Sprite imgQuality = null;
                switch (cardData.Quality)
                {
                    case CardData.CardQuality.COMMON:
                    case CardData.CardQuality.UNCOMMON:
                    case CardData.CardQuality.RARE:
                    case CardData.CardQuality.EPIC:
                    case CardData.CardQuality.LEGEND:
                        imgQuality = Helpers.RequireAtlasSprite(
                            m_pImgAtlasQuality,
                            cardData.Quality.ToString(),
                            "CardQuality");
                        break;
                    default:
                        break;
                }
                m_pSlotQualityImage.sprite = imgQuality;
            }


            public void StartLinearMove(float fDuration, in MoveInfo pDstMove, LerpModelCallback callback)
            {
                Helpers.ExtractMoveInfo(out MoveInfo pStartMove, transform);
                StartMove(LerpInfo.Linear(fDuration, in pStartMove, in pDstMove, callback));
            }
            public UniTask StartBezierMoveAsync(float fDuration, in MoveInfo pCenterMove, in MoveInfo pDstMove)
            {
                UniTaskCompletionSource pCompletion = new UniTaskCompletionSource();
                StartBezierMove(fDuration, in pCenterMove, in pDstMove, () => { pCompletion.TrySetResult(); });
                return pCompletion.Task;
            }
            public void StartBezierMove(float fDuration, in MoveInfo pCenterMove, in MoveInfo pDstMove, LerpModelCallback callback)
            {
                Helpers.ExtractMoveInfo(out MoveInfo pStartMove, transform);
                StartMove(LerpInfo.Bezier(fDuration, in pStartMove, in pCenterMove, in pDstMove, callback));
            }
            public void OnCreate()
            {
                gameObject.name = "CardCanvas_" + s_iCardCanvasPoolID++;
            }
            public void OnSpawn()
            {
            }
            public void OnDespawn()
            {
                CustomFinalize();
            }
            public void OnExtinct()
            {
                CustomFinalize();
            }

            public void OnPointerMove(PointerEventData eventData)
            {
            }

            public void OnPointerUp(PointerEventData eventData)
            {
            }

            public void OnPointerDown(PointerEventData eventData)
            {
                if (eventData.button == PointerEventData.InputButton.Right)
                {
                    GamePlayCanvas pController = GamePlayCanvas.Instance;
                    if (null != pController)
                    {
                        pController.PresentDiscard(m_pRefCard, this);
                    }
                }
            }

            public void OnPointerEnter(PointerEventData eventData)
            {
                OnHighlight();
            }

            public void OnPointerExit(PointerEventData eventData)
            {
                OffHighlight();
            }

            private void OffHighlight()
            {
                if (true == bHighlighted)
                {
                    bHighlighted = false;
                    m_pSlotHighlight.enabled = bHighlighted;
                }
            }

            private void OnHighlight()
            {
                if (false == bHighlighted)
                {
                    bHighlighted = true;
                    m_pSlotHighlight.enabled = bHighlighted;
                    transform.SetAsLastSibling();
                }
            }
            private void CustomFinalize()
            {
                m_pRefCard = null;
                OffHighlight();
                m_LerpInfo.Abort();

                m_pSlotName.text = string.Empty;
                m_pSlotCost.text = string.Empty;
                m_pSlotDescription.text = string.Empty;
                m_pSlotImage.sprite = null;
                m_pSlotTypeImage.sprite = null;
                m_pSlotQualityImage.sprite = null;
                m_pSlotHighlight.enabled = false;
                ResetRectTransform();
            }
            private void ResetRectTransform()
            {
                RectTransform pRect = transform as RectTransform;
                if (null == pRect)
                {
                    return;
                }
                pRect.anchorMin = s_vDefaultAnchor;
                pRect.anchorMax = s_vDefaultAnchor;
                pRect.pivot = s_vDefaultPivot;
                pRect.sizeDelta = s_vDefaultSizeDelta;
                pRect.anchoredPosition = s_vDefaultAnchoredPosition;
                pRect.localRotation = Quaternion.identity;
                pRect.localScale = Vector3.one;
            }

            public void OnBeginDrag(PointerEventData eventData)
            {
                if (eventData.button == PointerEventData.InputButton.Left)
                {
                    GamePlayCanvas pController = GamePlayCanvas.Instance;
                    if (null != pController && null != pController.HandBoard)
                    {
                        pController.HandBoard.OnDragBegin(m_pRefCard);
                    }
                    InstantMove(
                        new MoveInfo(eventData.position, Quaternion.identity)
                        );
                }
            }

            public void OnDrag(PointerEventData eventData)
            {
                if (eventData.button == PointerEventData.InputButton.Left)
                {
                    InstantMove(
                        new MoveInfo(eventData.position, Quaternion.identity)
                        );
                    transform.SetAsLastSibling();
                }
            }

            public void OnEndDrag(PointerEventData eventData)
            {
                if (eventData.button == PointerEventData.InputButton.Left)
                {
                    GamePlayCanvas pController = GamePlayCanvas.Instance;
                    if (null == pController)
                    {
                        return;
                    }

                    RectTransform rectTransform = transform.parent.GetComponent<RectTransform>();
                    Vector3[] vCorners = new Vector3[4];
                    rectTransform.GetWorldCorners(vCorners);
                    if (null == m_pRefCard)
                    {
                        return;
                    }
                    if (eventData.position.y > vCorners[1].y)
                    {
                        pController.PresentPlay(m_pRefCard, this);
                    }
                    else
                    {
                        if (null != pController.HandBoard)
                        {
                            pController.HandBoard.OnDragCancel(m_pRefCard, this);
                        }
                    }
                }
            }
        }
    }
}
