using System.Collections.Generic;
using UnityEngine;

public class UIInventoryBar : MonoBehaviour
{

    [SerializeField] private Sprite blank16x16sprite = null;
    [SerializeField] private UIInventorySlot[] inventorySlot = null;
    public GameObject inventoryBarDraggedItem;
    [HideInInspector] public GameObject inventoryTextBoxGameObject;



    private void OnDisable()
    {
        EventHandler.InventoryUpdatedDictEvent -= InventoryUpdated;

    }

    private void OnEnable()
    {
        EventHandler.InventoryUpdatedDictEvent += InventoryUpdated;
    }

    public void ClearHighlightOnInventorySlots()
    {
        if (inventorySlot.Length > 0)
        {
            for (int i = 0; i < inventorySlot.Length; i++)
            {
                if (inventorySlot[i].isSelected)
                {
                    inventorySlot[i].isSelected = false;
                    inventorySlot[i].inventorySlotHighlight.color = new Color(0f, 0f, 0f, 0f);

                    InventoryManager.Instance.ClearSelectedInventoryItem(InventoryLocation.player);
                }

            }
        }
    }


    private void ClearInventorySlots()
    {
        if (inventorySlot.Length > 0)
        {
            for (int i = 0; i < inventorySlot.Length; i++)
            {
                inventorySlot[i].inventorySlotImage.sprite = blank16x16sprite;
                inventorySlot[i].textMeshProUGUI.text = "";
                inventorySlot[i].itemDetails = null;
                inventorySlot[i].itemQuantity = 0;

                SetHighlightedInventorySlots(i);

            }

        }
    }

    private void InventoryUpdated(InventoryLocation inventoryLocation, Dictionary<int, InventoryItem> inventoryDict)
    {
        if (inventoryLocation == InventoryLocation.player)
        {
            ClearInventorySlots();

            if (inventorySlot.Length > 0 && inventoryDict.Count > 0)
            {
                for (int i = 0; i < inventorySlot.Length; i++)
                {
                    if (i < inventoryDict.Count)
                    {
                        int itemCode = inventoryDict[i].itemCode;

                        ItemDetails itemDetails = InventoryManager.Instance.GetItemDetails(itemCode);

                        if (itemDetails != null)
                        {
                            inventorySlot[i].inventorySlotImage.sprite = itemDetails.itemSprite;
                            inventorySlot[i].textMeshProUGUI.text = inventoryDict[i].itemQuantity.ToString();
                            inventorySlot[i].itemDetails = itemDetails;
                            inventorySlot[i].itemQuantity = inventoryDict[i].itemQuantity;

                            SetHighlightedInventorySlots(i);

                        }
                    }
                    else
                    {
                        break;
                    }
                }

            }

        }
    }


    public void SetHighlightedInventorySlots()
    {
        if (inventorySlot.Length > 0)
        {
            for (int i = 0; i < inventorySlot.Length; i++)
            {
                SetHighlightedInventorySlots(i);

            }
        }

    }

    public void SetHighlightedInventorySlots(int itemPosition)
    {
        if (inventorySlot.Length > 0 && inventorySlot[itemPosition].itemDetails != null)
        {
            if (inventorySlot[itemPosition].isSelected)
            {
                inventorySlot[itemPosition].inventorySlotHighlight.color = new Color(1f, 1f, 1f, 1f);

                InventoryManager.Instance.SetSelectedInventoryItem(InventoryLocation.player, inventorySlot[itemPosition].itemDetails.itemCode);

            }
        }

    }

    public void DestroyCurrentlyDraggedItems()
    {
        for (int i = 0; i < inventorySlot.Length; i++)
        {
            if (inventorySlot[i].draggedItem!=null)
            {
                Destroy(inventorySlot[i].draggedItem);
            }

        }

    }

    internal void ClearCurrentlySelectedItems()
    {
        for (int i = 0; i < inventorySlot.Length; i++)
        {
            inventorySlot[i].ClearSelectedItem();
        }
    }
}
