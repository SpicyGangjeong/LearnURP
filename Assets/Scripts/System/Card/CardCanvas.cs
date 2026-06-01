using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardCanvas : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] TextMeshProUGUI slotName = null;
    [SerializeField] TextMeshProUGUI slotCost = null;
    [SerializeField] TextMeshProUGUI slotDescription = null;
    [SerializeField] Image slotImage = null;
    [SerializeField] Image slotTypeImage = null;
    [SerializeField] Image slotQualityImage = null;

    Card refCard = null;

    public Card BoundCard => refCard;

    public void BindCard(Card card)
    {
        refCard = card;
        if (null == card || null == card.CardInfo)
        {
            return;
        }

        slotName.text = card.CardInfo.strCardName;
        slotCost.text = card.CardInfo.iCardCost.ToString();
        slotDescription.text = card.CardInfo.strCardDescription;
        // slotImage.sprite = card.CardInfo.sprite;
        // slotTypeImage.sprite = card.CardInfo.eCardType;
        // slotQualityImage.sprite = card.CardInfo.sprite;
    }

    public void RequestPlay()
    {
        if (null == refCard)
        {
            return;
        }
        
        CGameInstance.Instance.TryPlayCard(refCard);
    }

    public void RequestDiscard()
    {
        if (null == refCard)
        {
            return;
        }

        CGameInstance.Instance.TryDiscardCard(refCard);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            RequestPlay();
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            RequestDiscard();
        }
    }
}
