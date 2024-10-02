using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UISlot : MonoBehaviour
{
    public InventorySlot slot;
    private Image slotImage;
    private Image iconImage;
    private TextMeshProUGUI amountText;

    void Awake()
    {
        slotImage = GetComponent<Image>();
        iconImage = transform.Find("IconImage").GetComponent<Image>();
        amountText = transform.Find("AmountText").GetComponent<TextMeshProUGUI>();
    }

    public void UpdateUI(InventorySlot slot)
    {
        this.slot = slot;

        if (slot.IsEmpty)
        {
            iconImage.gameObject.SetActive(false);
            amountText.gameObject.SetActive(false);
        }
        else if (slot.item != null)
        {
            iconImage.gameObject.SetActive(true);
            iconImage.sprite = slot.item.icon;

            if ((slot.item.data.itemType == ItemType.Consumable || slot.item.data.itemType == ItemType.Material) && slot.amount > 1)
            {
                amountText.gameObject.SetActive(true);
                amountText.text = slot.amount.ToString();
            }
            else
            {
                amountText.gameObject.SetActive(false);
            }
        }
        else if (slot.skill != null)
        {
            iconImage.gameObject.SetActive(true);
            iconImage.sprite = slot.skill.icon;
            amountText.gameObject.SetActive(false);
        }
    }
}
