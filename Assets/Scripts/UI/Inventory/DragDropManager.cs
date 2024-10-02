using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DragDropManager : MonoBehaviour
{
    private InventoryManager inventoryManager;
    private UIManager uiManager;

    private InventorySlot draggedSlot;
    private Image draggedImage;

    void Start()
    {
        inventoryManager = GetComponent<InventoryManager>();
        uiManager = GetComponent<UIManager>();

        // Initialize dragged image
        GameObject draggedObj = new GameObject("DraggedItem");
        draggedObj.transform.SetParent(transform);
        draggedImage = draggedObj.AddComponent<Image>();
        draggedImage.raycastTarget = false;
        draggedImage.gameObject.SetActive(false);
    }

    public void OnBeginDrag(InventorySlot slot)
    {
        if (slot.IsEmpty) return;

        draggedSlot = slot;
        draggedImage.sprite = slot.item.icon;
        draggedImage.gameObject.SetActive(true);
    }

    public void OnDrag(Vector2 position)
    {
        draggedImage.transform.position = position;
    }

    public void OnEndDrag(InventorySlot targetSlot)
    {
        if (draggedSlot == null) return;

        // 장비 슬롯인지 확인
        string equipSlotType = GetEquipmentSlotType(targetSlot);
        if (equipSlotType != null)
        {
            inventoryManager.EquipItem(draggedSlot, equipSlotType);
        }
        else
        {
            inventoryManager.MoveItem(draggedSlot, targetSlot);
        }

        draggedSlot = null;
        draggedImage.gameObject.SetActive(false);
        uiManager.UpdateAllUI();
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
