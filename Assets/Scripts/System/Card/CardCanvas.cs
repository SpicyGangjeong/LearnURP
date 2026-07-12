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

            Card m_pRefCard = null;
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
            public void BindCard(Card pCard)
            {
                m_pRefCard = pCard;
                if (null == pCard || null == pCard.CardInfo)
                {
                    return;
                }
                BindCardData(m_pRefCard.CardInfo);
            }
            public void BindCardData(in CardDataSO cardData)
            {
                m_pSlotName.text = cardData.m_strCardName;
                m_pSlotCost.text = cardData.m_iCardCost.ToString();
                m_pSlotDescription.text = cardData.m_strCardDescription;

                Sprite imgPortrait = null;
                switch (cardData.m_eCardPortrait)
                {
                    case CardDataSO.CardPortrait.NONE:
                    case CardDataSO.CardPortrait.END:
                        break;
                    default:
                        imgPortrait = Helpers.RequireAtlasSprite(
                            m_pImgAtlasPortrait,
                            cardData.m_eCardPortrait.ToString(),
                            "CardPortrait");
                        break;
                }
                m_pSlotImage.sprite = imgPortrait;

                Sprite imgType = null;
                switch (cardData.m_eCardType)
                {
                    case CardDataSO.CardType.ATTACK:
                    case CardDataSO.CardType.DEFENSE:
                    case CardDataSO.CardType.MAGIC:
                    case CardDataSO.CardType.ITEM:
                        imgType = Helpers.RequireAtlasSprite(
                            m_pImgAtlasType,
                            cardData.m_eCardType.ToString(),
                            "CardType");
                        break;
                    default:
                        break;
                }
                m_pSlotTypeImage.sprite = imgType;

                Sprite imgQuality = null;
                switch (cardData.m_eQuality)
                {
                    case CardDataSO.CardQuality.COMMON:
                    case CardDataSO.CardQuality.UNCOMMON:
                    case CardDataSO.CardQuality.RARE:
                    case CardDataSO.CardQuality.EPIC:
                    case CardDataSO.CardQuality.LEGEND:
                        imgQuality = Helpers.RequireAtlasSprite(
                            m_pImgAtlasQuality,
                            cardData.m_eQuality.ToString(),
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
            public void OnEnable()
            {
                CustomFinalize();
            }
            public void OnDisable()
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
                m_pSlotName.text = string.Empty;
                m_pSlotCost.text = string.Empty;
                m_pSlotDescription.text = string.Empty;
                m_pSlotImage.sprite = null;
                m_pSlotTypeImage.sprite = null;
                m_pSlotQualityImage.sprite = null;
                m_pSlotHighlight.enabled = false;
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
