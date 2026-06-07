using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Serialization;

public class CardCanvas : MonoBehaviour, IPointerClickHandler, IPoolable
{
    [FormerlySerializedAs("slotName")]
    [SerializeField] TextMeshProUGUI m_pSlotName = null;
    [FormerlySerializedAs("slotCost")]
    [SerializeField] TextMeshProUGUI m_pSlotCost = null;
    [FormerlySerializedAs("slotDescription")]
    [SerializeField] TextMeshProUGUI m_pSlotDescription = null;
    [FormerlySerializedAs("slotImage")]
    [SerializeField] Image m_pSlotImage = null;
    [FormerlySerializedAs("slotTypeImage")]
    [SerializeField] Image m_pSlotTypeImage = null;
    [FormerlySerializedAs("slotQualityImage")]
    [SerializeField] Image m_pSlotQualityImage = null;

    Card m_pRefCard = null;

    public Card BoundCard => m_pRefCard;

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

    public void OnPointerClick(PointerEventData pEventData)
    {
        if (pEventData.button == PointerEventData.InputButton.Left)
        {
            RequestPlay();
        }
        else if (pEventData.button == PointerEventData.InputButton.Right)
        {
            RequestDiscard();
        }
    }

    public void OnCreate()
    {
    }
    public void OnSpawn()
    {
    }

    public void OnDespawn()
    {
        m_pRefCard = null;
    }

    public void OnExtinct()
    {
    }
    public void OnDestroy()
    {
    }
}
