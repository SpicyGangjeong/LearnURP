using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardCanvas : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI slotName = null;
    [SerializeField] TextMeshProUGUI slotCost = null;
    [SerializeField] TextMeshProUGUI slotDescription = null;
    [SerializeField] Image slotImage = null;
    [SerializeField] Image slotTypeImage = null;
    [SerializeField] Image slotQualityImage = null;

    Card refCard = null;
    void Start()
    {
        
    }

    void Update()
    {
        
    }
    public void BindCard(Card card)
    {
        Debug.Log($"BindCard: {card.CardInfo.iCardID}");
        slotName.text = card.CardInfo.strCardName;
        slotCost.text = card.CardInfo.iCardCost.ToString();
        slotDescription.text = card.CardInfo.strCardDescription;
        // slotImage.sprite = card.CardInfo.sprite;
        // slotTypeImage.sprite = card.CardInfo.eCardType;
        // slotQualityImage.sprite = card.CardInfo.sprite;

    }
}
