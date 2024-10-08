using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class DragDropManager : MonoBehaviour
{
    private InventoryManager inventoryManager;
    private InventorySlot draggedSlot;
    private GameObject draggedObj;
    private int dragAmount;
    private Image draggedImage;
    private CanvasGroup draggedCanvasGroup;
    private Canvas uiCanvas;

    public GameObject amountInputUI;
    public TMP_InputField amountInputField;
    public Button confirmButton;

    private bool isShiftDrag = false;
    private InventorySlot targetSlot;

    void Start()
    {
        inventoryManager = GetComponent<InventoryManager>();
       
        uiCanvas = GameObject.Find("UI_Canvas").GetComponent<Canvas>();
        InitializeDraggedObject();

        confirmButton.onClick.AddListener(OnAmountConfirmed);
    }

    void InitializeDraggedObject()
    {
        draggedObj = new GameObject("DraggedItem");
        RectTransform rectTransform = draggedObj.AddComponent<RectTransform>();
        draggedObj.transform.SetParent(uiCanvas.transform);

        draggedImage = draggedObj.AddComponent<Image>();
        draggedCanvasGroup = draggedObj.AddComponent<CanvasGroup>();

        draggedImage.raycastTarget = false;
        draggedCanvasGroup.alpha = 0.6f;
        draggedCanvasGroup.blocksRaycasts = false;

        rectTransform.sizeDelta = new Vector2(50, 50);

        draggedObj.SetActive(false);
    }

    public void OnBeginDrag(InventorySlot slot)
    {
        if (slot.IsEmpty) return;
        draggedSlot = slot;
        draggedImage.sprite = slot.item.icon;
        draggedObj.SetActive(true);
        draggedObj.transform.SetAsLastSibling();

        isShiftDrag = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);

        if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
        {
            dragAmount = 1;
        }
        else
        {
            dragAmount = slot.amount;
        }

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

        this.targetSlot = targetSlot;

        if (isShiftDrag && targetSlot != draggedSlot)
        {
            ShowAmountInputUI();
        }
        else
        {
            CompleteItemTransfer(dragAmount);
        }

        draggedObj.SetActive(false);
    }

    private void ShowAmountInputUI()
    {
        amountInputUI.SetActive(true);
        amountInputField.text = dragAmount.ToString();
        amountInputField.Select();
        amountInputField.ActivateInputField();
    }

    private void OnAmountConfirmed()
    {
        if (int.TryParse(amountInputField.text, out int amount))
        {
            dragAmount = Mathf.Clamp(amount, 1, draggedSlot.amount);
            CompleteItemTransfer(dragAmount);
        }
        amountInputUI.SetActive(false);
    }

    private void CompleteItemTransfer(int amount)
    {
        string equipSlotType = GetEquipmentSlotType(targetSlot);
        if (equipSlotType != null)
        {
            HandleEquipmentSlot(equipSlotType);
        }
        else
        {
            HandleInventorySlot(targetSlot, amount);
        }

        draggedSlot = null;
        inventoryManager.UpdateAllUI();
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

    private void HandleInventorySlot(InventorySlot targetSlot, int amount)
    {
        if (targetSlot == null) return;
        if (targetSlot.IsEmpty)
        {
            inventoryManager.MoveItem(draggedSlot, targetSlot, amount);
        }
        else if (draggedSlot.item.data.id == targetSlot.item.data.id &&
                 !(draggedSlot.item.data.ItemTypes.HasFlag(ItemType.Equipment) ||
                   draggedSlot.item.data.ItemTypes.HasFlag(ItemType.Decorative)))
        {
            int amountToAdd = Mathf.Min(amount, draggedSlot.amount);
            targetSlot.amount += amountToAdd;
            draggedSlot.amount -= amountToAdd;
            if (draggedSlot.amount <= 0)
            {
                draggedSlot.Clear();
            }
        }
        else
        {
            // 아이템 교환 로직
            Item tempItem = targetSlot.item;
            int tempAmount = targetSlot.amount;
            targetSlot.SetItem(draggedSlot.item, amount);
            if (amount < draggedSlot.amount)
            {
                draggedSlot.amount -= amount;
            }
            else
            {
                draggedSlot.SetItem(tempItem, tempAmount);
            }
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
