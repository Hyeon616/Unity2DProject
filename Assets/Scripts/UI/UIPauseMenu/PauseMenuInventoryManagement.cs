using System.Collections.Generic;
using UnityEngine;

public class PauseMenuInventoryManagement : MonoBehaviour
{

    [SerializeField] private PauseMenuInventoryManagementSlot[] inventoryManagementSlot = null;
    public GameObject inventoryManagementDraggedItemPrefab;

    [SerializeField] private Sprite transparent16x16 = null;

    [HideInInspector] public GameObject inventoryTextBoxGameObject;


    private void OnEnable()
    {
        
        EventHandler.InventoryUpdatedDictEvent += PopulatePlayerInventory;

        if (InventoryManager.Instance != null)
        {
            PopulatePlayerInventory(InventoryLocation.player, InventoryManager.Instance.inventoryDictionaries[(int)InventoryLocation.player]);
        }
    }

    private void OnDisable()
    {
        EventHandler.InventoryUpdatedDictEvent -= PopulatePlayerInventory;

        DestroyInventoryTextBoxGameobject();
    }

    
    public void DestroyInventoryTextBoxGameobject()
    {
        if (inventoryTextBoxGameObject != null)
        {
            Destroy(inventoryTextBoxGameObject);
        }
    }

    public void DestoryCurrentlyDraggedItems()
    {
        
        for (int i = 0; i < InventoryManager.Instance.inventoryDictionaries[(int)InventoryLocation.player].Count; i++)
        {
            if (inventoryManagementSlot[i].draggedItem != null)
            {
                Destroy(inventoryManagementSlot[i].draggedItem);
            }
        }
    }

    private void PopulatePlayerInventory(InventoryLocation inventoryLocation, Dictionary<int, InventoryItem> playerInventoryDict)
    {
        
        if(inventoryLocation == InventoryLocation.player)
        {
            InitializeInventoryManagementSlots();

            // 아이템 전부 순회
            for (int i = 0; i < InventoryManager.Instance.inventoryDictionaries[(int)InventoryLocation.player].Count; i++)
            {
                inventoryManagementSlot[i].itemDetails = InventoryManager.Instance.GetItemDetails(playerInventoryDict[i].itemCode);
                inventoryManagementSlot[i].itemQuantity = playerInventoryDict[i].itemQuantity;

                if (inventoryManagementSlot[i].itemDetails != null)
                {
                    inventoryManagementSlot[i].inventoryManagementSlotImage.sprite = inventoryManagementSlot[i].itemDetails.itemSprite;
                    inventoryManagementSlot[i].textMeshProUGUI.text = inventoryManagementSlot[i].itemQuantity.ToString();
                }

            }
        }
    }

    

    private void InitializeInventoryManagementSlots()
    {

        // 인벤토리 초기화
        for (int i = 0; i < Settings.playerMaximumInventoryCapacity; i++)
        {
            inventoryManagementSlot[i].greyedOutImageGO.SetActive(false);
            inventoryManagementSlot[i].itemDetails = null;
            inventoryManagementSlot[i].itemQuantity = 0;
            inventoryManagementSlot[i].inventoryManagementSlotImage.sprite = transparent16x16;
            inventoryManagementSlot[i].textMeshProUGUI.text = "";
        }


        for (int i = InventoryManager.Instance.inventoryListCapacityIntArray[(int)InventoryLocation.player]; i < Settings.playerMaximumInventoryCapacity; i++)
        {
            inventoryManagementSlot[i].greyedOutImageGO.SetActive(true);
        }

    }
}
