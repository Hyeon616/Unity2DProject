using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DragDropManager : MonoBehaviour
{
    private InventoryManager inventoryManager;
    private UIManager uiManager;
    private InventorySlot draggedSlot;
    private GameObject draggedObj;
    private Image draggedImage;
    private CanvasGroup draggedCanvasGroup;

    void Start()
    {
        inventoryManager = GetComponent<InventoryManager>();
        uiManager = GetComponent<UIManager>();
        InitializeDraggedObject();
    }

    void InitializeDraggedObject()
    {
        draggedObj = new GameObject("DraggedItem");
        draggedObj.transform.SetParent(transform);
        draggedImage = draggedObj.AddComponent<Image>();
        draggedCanvasGroup = draggedObj.AddComponent<CanvasGroup>();
        draggedImage.raycastTarget = false;
        draggedCanvasGroup.alpha = 0.6f;
        draggedObj.SetActive(false);
    }

    public void OnBeginDrag(InventorySlot slot)
    {
        if (slot.IsEmpty) return;
        draggedSlot = slot;
        draggedImage.sprite = slot.item.icon;
        draggedObj.SetActive(true);
    }

    public void OnDrag(Vector2 position)
    {
        if (draggedObj.activeSelf)
        {
            draggedObj.transform.position = position;
        }
    }

    public void OnEndDrag(InventorySlot targetSlot)
    {
        if (draggedSlot == null) return;

        string equipSlotType = GetEquipmentSlotType(targetSlot);
        if (equipSlotType != null)
        {
            HandleEquipmentSlot(equipSlotType);
        }
        else
        {
            HandleInventorySlot(targetSlot);
        }

        draggedSlot = null;
        draggedObj.SetActive(false);
        uiManager.UpdateAllUI();
    }

    private void HandleEquipmentSlot(string equipSlotType)
    {
        if (draggedSlot.item.data.ItemTypes.HasFlag(ItemType.Equipment))
        {
            inventoryManager.EquipItem(draggedSlot, equipSlotType);
        }
        else
        {
            Debug.Log("This item cannot be equipped in this slot.");
        }
    }

    private void HandleInventorySlot(InventorySlot targetSlot)
    {
        if (targetSlot.IsEmpty)
        {
            inventoryManager.MoveItem(draggedSlot, targetSlot);
        }
        else if (draggedSlot.item.data.id == targetSlot.item.data.id &&
                 !(draggedSlot.item.data.ItemTypes.HasFlag(ItemType.Equipment) ||
                   draggedSlot.item.data.ItemTypes.HasFlag(ItemType.Decorative)))
        {
            // 같은 아이템이고 장비나 장식품이 아닌 경우 스택
            targetSlot.amount += draggedSlot.amount;
            draggedSlot.Clear();
        }
        else
        {
            // 다른 아이템이거나 장비/장식품인 경우 교환
            Item tempItem = targetSlot.item;
            int tempAmount = targetSlot.amount;
            targetSlot.SetItem(draggedSlot.item, draggedSlot.amount);
            draggedSlot.SetItem(tempItem, tempAmount);
        }
    }

    private string GetEquipmentSlotType(InventorySlot slot)
    {
        foreach (var kvp in inventoryManager.equipmentSlots)
        {
            if (kvp.Value == slot)
            {
                return kvp.Key;
            }
        }
        return null;
    }
}
