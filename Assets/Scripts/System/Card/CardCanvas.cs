using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UnityEngine.EventSystems.PointerEventData;

interface ICardPointerHandler :
    IPointerEnterHandler, IPointerExitHandler,
    IPointerMoveHandler, IPointerUpHandler, IPointerDownHandler
{
}
public class CardCanvas : MonoBehaviour, IPoolable, ICardPointerHandler
{
    [SerializeField] TextMeshProUGUI m_pSlotName = null;
    [SerializeField] TextMeshProUGUI m_pSlotCost = null;
    [SerializeField] TextMeshProUGUI m_pSlotDescription = null;

    [SerializeField] UnityEngine.UI.Image m_pSlotImage = null;
    [SerializeField] UnityEngine.UI.Image m_pSlotTypeImage = null;
    [SerializeField] UnityEngine.UI.Image m_pSlotQualityImage = null;
    [SerializeField] Outline m_pSlotHighlight = null;

    Card m_pRefCard = null;
    public Card BoundCard => m_pRefCard;
    public bool bHighlighted { get; private set; } = false;

    DEFINES.STRUCTURES.MoveInfo m_SrcMove = new DEFINES.STRUCTURES.MoveInfo();
    DEFINES.STRUCTURES.MoveInfo m_DstMove = new DEFINES.STRUCTURES.MoveInfo();
    bool bMoving = false;
    Vector2 m_vLerpTimer = Vector2.up;

    public void StartMove(DEFINES.STRUCTURES.MoveInfo pDstMove)
    {
        m_SrcMove.vRotQ = transform.rotation;
        m_SrcMove.vPosition = transform.position;

        m_DstMove = pDstMove;

        bMoving = true;
        m_vLerpTimer = Vector2.up;
    }
    private void LerpTransfrom(float fRatio = 0.5f)
    {
        Vector3 vNewPosition = Vector3.Slerp(m_SrcMove.vPosition, m_DstMove.vPosition, fRatio);
        Quaternion vNewQuaternion = Quaternion.Slerp(m_SrcMove.vRotQ, m_DstMove.vRotQ, fRatio);

        transform.SetPositionAndRotation(vNewPosition, vNewQuaternion);
    }

    public void Update()
    {
        if (true == bMoving)
        {
            if (m_vLerpTimer.x != m_vLerpTimer.y) // Lerping
            {
                m_vLerpTimer.x += Time.deltaTime;
                float fRatio = m_vLerpTimer.x / m_vLerpTimer.y;
                if (fRatio >= 1f)
                {
                    bMoving = false;
                    fRatio = 1f;
                    m_vLerpTimer.x = m_vLerpTimer.y;
                }
                LerpTransfrom(fRatio);
            }

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
    public void RequestPlay()
    {
        if (null == m_pRefCard)
        {
            return;
        }

        CGameInstance.Instance.TryPlayCard(m_pRefCard);
    }
    public void RequestDiscard()
    {
        if (null == m_pRefCard)
        {
            return;
        }

        CGameInstance.Instance.TryDiscardCard(m_pRefCard);
    }
    public void OnCreate()
    {
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
        if (eventData.dragging && 
            eventData.button == InputButton.Left) {
            
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button == InputButton.Left)
        {
            CGameInstance.Instance.TryPlayCard(m_pRefCard);
        }
        if (eventData.button == InputButton.Right)
        {
            CGameInstance.Instance.TryDiscardCard(m_pRefCard);
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
}
