using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UISlot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public InventorySlot slot;
    private Image slotImage;
    private Image iconImage;
    private TextMeshProUGUI amountText;
    private DragDropManager dragDropManager;

    void Awake()
    {
        slotImage = GetComponent<Image>();
        iconImage = transform.Find("IconImage").GetComponent<Image>();
        amountText = transform.Find("AmountText").GetComponent<TextMeshProUGUI>();
        dragDropManager = FindObjectOfType<DragDropManager>();
    }

    public void UpdateUI(InventorySlot slot)
    {
        if (iconImage == null || amountText == null)
            return;

        this.slot = slot;
        if (slot == null || slot.IsEmpty)
        {
            SetSlotEmpty();
        }
        else if (slot.item != null)
        {
            UpdateItemSlot(slot.item);
        }
        else if (slot.skill != null)
        {
            UpdateSkillSlot(slot.skill);
        }
        else
        {
            Debug.LogWarning("Slot is not empty but contains neither item nor skill.");
            SetSlotEmpty();
        }
    }

    private void SetSlotEmpty()
    {
        iconImage.gameObject.SetActive(false);
        amountText.gameObject.SetActive(false);
    }

    private void UpdateItemSlot(Item item)
    {
        

        iconImage.gameObject.SetActive(true);
        if (item.icon != null)
        {
            iconImage.sprite = item.icon;
        }
        else
        {
            Debug.LogWarning($"Icon is null for item: {item.data.name}");
            iconImage.sprite = null; // 또는 기본 아이콘을 설정
        }

        if (item.data.ItemTypes.HasFlag(ItemType.Consumable) ||
            item.data.ItemTypes.HasFlag(ItemType.Material))
        {
            UpdateAmountText(slot.amount);
        }
        else
        {
            amountText.gameObject.SetActive(false);
        }
    }

    private void UpdateSkillSlot(Skill skill)
    {
        iconImage.gameObject.SetActive(true);
        if (skill.icon != null)
        {
            iconImage.sprite = skill.icon;
        }
        else
        {
            Debug.LogWarning($"Icon is null for skill: {skill.data.name}");
            iconImage.sprite = null; // 또는 기본 아이콘을 설정
        }
        amountText.gameObject.SetActive(false);
    }

    private void UpdateAmountText(int amount)
    {
        if (amount > 1)
        {
            amountText.gameObject.SetActive(true);
            amountText.text = amount.ToString();
        }
        else
        {
            amountText.gameObject.SetActive(false);
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (dragDropManager != null && slot != null)
        {
            dragDropManager.OnBeginDrag(slot);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (dragDropManager != null)
        {
            dragDropManager.OnDrag(eventData.position);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (dragDropManager != null)
        {
            GameObject hitObject = eventData.pointerCurrentRaycast.gameObject;
            UISlot targetSlot = hitObject?.GetComponent<UISlot>();
            dragDropManager.OnEndDrag(targetSlot?.slot);
        }
    }
}
