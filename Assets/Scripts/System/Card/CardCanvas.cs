using Cysharp.Threading.Tasks;
using DEFINES;
using DEFINES.STRUCTURES;
using System;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

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
        HELPERS.ApplyMoveInfo(moveinfo, transform);
    }

    public void Update()
    {
        if (true == m_LerpInfo.IsLerping)
        {
            m_LerpInfo.Progress();
            HELPERS.ApplyMoveInfo(m_LerpInfo.GetMoveInfo(), transform);
        }
    }
    public void BindCard(Card pCard)
    {
        m_pRefCard = pCard;
        if (null == pCard || null == pCard.CardInfo)
        {
            return;
        }

        m_pSlotName.text = pCard.CardInfo.m_strCardName;
        m_pSlotCost.text = pCard.CardInfo.m_iCardCost.ToString();
        m_pSlotDescription.text = pCard.CardInfo.m_strCardDescription;
        // m_pSlotImage.sprite = pCard.CardInfo.sprite;
        // m_pSlotTypeImage.sprite = pCard.CardInfo.m_iCardType;
        // m_pSlotQualityImage.sprite = pCard.CardInfo.sprite;
    }
    public void StartLinearMove(float fDuration, in MoveInfo pDstMove, LerpModelCallback callback)
    {
        HELPERS.ExtractMoveInfo(out MoveInfo pStartMove, transform);
        StartMove(LerpInfo.Linear(fDuration, in pStartMove, in pDstMove, callback));
    }
    public UniTask StartBezierMoveAsync(float fDuration, in MoveInfo pCenterMove, in MoveInfo pDstMove){
        UniTaskCompletionSource pCompletion = new UniTaskCompletionSource();
        StartBezierMove(fDuration, in pCenterMove, in pDstMove, () => { pCompletion.TrySetResult(); });
        return pCompletion.Task;
    }
    public void StartBezierMove(float fDuration, in MoveInfo pCenterMove, in MoveInfo pDstMove, LerpModelCallback callback)
    {
        HELPERS.ExtractMoveInfo(out MoveInfo pStartMove, transform);
        StartMove(LerpInfo.Bezier(fDuration, in pStartMove, in pCenterMove, in pDstMove, callback));
    }
    public void RequestHandboardPop()
    {
        CGameInstance.Instance.TryHandboardPopCard(m_pRefCard);
    }
    public void RequestHandboardInsert()
    {
        CGameInstance.Instance.TryHandboardInsertCard(m_pRefCard, this);
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
            CGameInstance.Instance.RequestDiscardCard(m_pRefCard, this);
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
        }
    }
    private void CustomFinalize(){
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
            RequestHandboardPop();
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
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            RectTransform rectTransform = transform.parent.GetComponent<RectTransform>();
            Vector3[] vCorners = new Vector3[4];
            rectTransform.GetWorldCorners(vCorners);
            if (eventData.position.y > vCorners[1].y)
            {
                CGameInstance.Instance.RequestPlayCard(m_pRefCard, this);
            } else
            {
                RequestHandboardInsert();
            }
        }
    }
}
