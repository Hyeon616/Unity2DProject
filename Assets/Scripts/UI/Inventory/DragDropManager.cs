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
    private bool isCtrlDrag = false;
    private InventorySlot targetSlot;

    void Start()
    {
        inventoryManager = GetComponent<InventoryManager>();
       
        uiCanvas = GameObject.Find("UI_Canvas").GetComponent<Canvas>();
        InitializeDraggedObject();

       // confirmButton.onClick.AddListener(OnAmountConfirmed);
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
        isCtrlDrag = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);

        dragAmount = isCtrlDrag ? 1 : slot.amount;

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
            // UI 작업이 완료되면 이 부분의 주석을 해제하고 else 부분을 주석 처리하세요
            // ShowAmountInputUI();
            ItemTransfer(dragAmount);
        }
        else
        {
            ItemTransfer(dragAmount);
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
            ItemTransfer(dragAmount);
        }
        amountInputUI.SetActive(false);
    }

    private void ItemTransfer(int amount)
    {
        if (targetSlot == null)
        {
            Debug.Log("Target slot is null, aborting transfer");
            return;
        }

        if (draggedSlot == null)
        {
            Debug.Log("Dragged slot is null, aborting transfer");
            return;
        }

        string equipSlotType = GetEquipmentSlotType(targetSlot);
        if (equipSlotType != null)
        {
            Debug.Log($"Handling equipment slot: {equipSlotType}");
            EquipmentItem(equipSlotType);
        }
        else
        {
            inventoryManager.MoveItem(draggedSlot, targetSlot, amount);
        }

        draggedSlot = null;
        inventoryManager.UpdateAllUI();
    }

    private void EquipmentItem(string equipSlotType)
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
