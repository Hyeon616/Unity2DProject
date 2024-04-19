using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : Singleton<InventoryManager>
{

    private Dictionary<int, ItemDetails> itemDetailsDictionary;
    public int[] selectedInventoryItem;

    public Dictionary<int, InventoryItem>[] inventoryDictionaries;
    // 0 플레이어
    // 1 창고

    [HideInInspector] public int[] inventoryListCapacityIntArray;

    [SerializeField] private SO_ItemList itemList = null;


    protected override void Awake()
    {
        base.Awake();
        // 인벤토리 리스트 생성
        CreateInventoryLists();
        // 아이템 딕셔너리 생성
        CreateItemDetailsDictionary();

        // 아이템 선택창 초기화
        selectedInventoryItem = new int[(int)InventoryLocation.Count];

        for (int i = 0; i < selectedInventoryItem.Length; i++)
        {
            selectedInventoryItem[i] = -1;
        }
    }

    private void CreateInventoryLists()
    {

        inventoryListCapacityIntArray = new int[(int)InventoryLocation.Count];
        // 플레이어 인벤토리 용량
        inventoryListCapacityIntArray[(int)InventoryLocation.player] = Settings.playerInitalInventoryCapacity;

        inventoryDictionaries = new Dictionary<int, InventoryItem>[(int)InventoryLocation.Count];
        // 플레이어 인벤토리 딕셔너리 생성
        Dictionary<int, InventoryItem> playerDict = new Dictionary<int, InventoryItem>();

        for (int i = 0; i < inventoryListCapacityIntArray[(int)InventoryLocation.player]; i++)
        {
            InventoryItem invItem;
            invItem.itemCode = 0;
            invItem.itemQuantity = 0;
            playerDict.Add(i, invItem);
        }

        inventoryDictionaries[(int)InventoryLocation.player] = playerDict;
        //----

    }

    private void CreateItemDetailsDictionary()
    {
        itemDetailsDictionary = new Dictionary<int, ItemDetails>();

        foreach (ItemDetails itemDetails in itemList.itemDetails)
        {
            itemDetailsDictionary.Add(itemDetails.itemCode, itemDetails);
        }
    }

    public void AddItem(InventoryLocation inventoryLocation, Items item, GameObject gameObjectToDelete)
    {
        AddItem(inventoryLocation, item);
        Destroy(gameObjectToDelete);
    }


    public void AddItem(InventoryLocation inventoryLocation, Items item)
    {
        int itemCode = item.ItemCode;
        Dictionary<int, InventoryItem> inventoryDict = inventoryDictionaries[(int)inventoryLocation];
        // 아이템이 없으면 -1반환
        int itemPosition = FindItemInInventory(inventoryLocation, itemCode);

        if (itemPosition != -1)
        {
            // 기존 아이템
            AddItemAtPosition(inventoryDict, itemCode, itemPosition);

        }
        else
        {
            // 새로운 아이템
            AddItemAtPosition(inventoryDict, itemCode);
        }

        EventHandler.CallInventoryUpdatedDictEvent(inventoryLocation, inventoryDictionaries[(int)inventoryLocation]);

    }

    private int GetFirstEmptyItemSlot(Dictionary<int, InventoryItem> inventoryDict)

    {

        foreach (KeyValuePair<int, InventoryItem> item in inventoryDict)

        {

            if (item.Value.itemCode == 0)
                return item.Key;

        }

        return -1;

    }


    public int FindItemInInventory(InventoryLocation inventoryLocation, int itemCode)
    {
        Dictionary<int, InventoryItem> inventoryDict = inventoryDictionaries[(int)inventoryLocation];

        foreach (KeyValuePair<int, InventoryItem> item in inventoryDict)
        {
            if (item.Value.itemCode == itemCode) return item.Key;
        }

        return -1;
    }


    private void AddItemAtPosition(Dictionary<int, InventoryItem> inventoryDict, int itemCode)
    {

        InventoryItem inventoryItem = new InventoryItem();

        int itemSlot = GetFirstEmptyItemSlot(inventoryDict);

        if (itemSlot != -1)
        {

            inventoryItem.itemCode = itemCode;

            inventoryItem.itemQuantity = 1;

            inventoryDict[itemSlot] = inventoryItem;

        }
    }

    private void AddItemAtPosition(Dictionary<int, InventoryItem> inventoryDict, int itemCode, int itemPosition)
    {

        InventoryItem inventoryItem = new InventoryItem();

        int quantity = inventoryDict[itemPosition].itemQuantity + 1;
        inventoryItem.itemQuantity = quantity;
        inventoryItem.itemCode = itemCode;
        inventoryDict[itemPosition] = inventoryItem;

    }


    // 아이템 코드에 맞는 아이템이 없으면 null
    public ItemDetails GetItemDetails(int itemCode)
    {
        ItemDetails itemDetails;

        if (itemDetailsDictionary.TryGetValue(itemCode, out itemDetails))
        {
            return itemDetails;
        }
        else
        {
            return null;
        }
    }

    public ItemDetails GetSelectedInventoryItemDetails(InventoryLocation inventoryLocation)
    {
        int itemCode = GetSelectedInventoryItem(inventoryLocation);

        if (itemCode == -1)
        {
            return null;

        }
        else
        {
            return GetItemDetails(itemCode);
        }

    }


    public int GetSelectedInventoryItem(InventoryLocation inventoryLocation)
    {
        return selectedInventoryItem[(int)inventoryLocation];
    }

    public string GetItemTypeDescription(ItemType itemType)
    {
        string itemTypeDescription;

        switch (itemType)
        {
            case ItemType.BreakingTool:
                itemTypeDescription = Settings.BreakingTool;
                break;

            case ItemType.ChoppingTool:
                itemTypeDescription = Settings.ChoppingTool;
                break;

            case ItemType.RepairingTool:
                itemTypeDescription = Settings.RepairingTool;
                break;

            case ItemType.Sword:
                itemTypeDescription = Settings.Sword;
                break;

            case ItemType.Bow:
                itemTypeDescription = Settings.Bow;
                break;

            default:
                itemTypeDescription = itemType.ToString();
                break;

        }
        return itemTypeDescription;
    }



    public void RemoveItem(InventoryLocation inventoryLocation, int itemCode)
    {

        Dictionary<int, InventoryItem> inventoryDict = inventoryDictionaries[(int)inventoryLocation];

        int itemPosition = FindItemInInventory(inventoryLocation, itemCode);

        if (itemPosition != -1)
        {
            RemoveItemAtPosition(inventoryDict, itemCode, itemPosition);
        }

        EventHandler.CallInventoryUpdatedDictEvent(inventoryLocation, inventoryDictionaries[(int)inventoryLocation]);

    }

    private void RemoveItemAtPosition(Dictionary<int, InventoryItem> inventoryDict, int itemCode, int itemPosition)
    {

        InventoryItem inventoryItem = new InventoryItem();

        int quantity = inventoryDict[itemPosition].itemQuantity - 1;

        if (quantity > 0)
        {
            inventoryItem.itemQuantity = quantity;

            inventoryItem.itemCode = itemCode;
        }
        else
        {
            inventoryItem.itemQuantity = 0;

            inventoryItem.itemCode = 0;
        }

        inventoryDict[itemPosition] = inventoryItem;

    }

    public void SwapInventoryItems(InventoryLocation inventoryLocation, int fromItem, int toItem)
    {

        if (fromItem != toItem && fromItem >= 0)
        {
            if (inventoryDictionaries[(int)inventoryLocation].ContainsKey(toItem))
            {
                InventoryItem fromInventoryItem = inventoryDictionaries[(int)inventoryLocation][fromItem];
                InventoryItem toInventoryItem = inventoryDictionaries[(int)inventoryLocation][toItem];

                inventoryDictionaries[(int)inventoryLocation][toItem] = fromInventoryItem;
                inventoryDictionaries[(int)inventoryLocation][fromItem] = toInventoryItem;

                EventHandler.CallInventoryUpdatedDictEvent(inventoryLocation, inventoryDictionaries[(int)inventoryLocation]);
            }
        }
    }

    public void ClearSelectedInventoryItem(InventoryLocation inventoryLocation)
    {
        selectedInventoryItem[(int)inventoryLocation] = -1;
    }

    public void SetSelectedInventoryItem(InventoryLocation inventoryLocation, int itemCode)
    {
        selectedInventoryItem[(int)inventoryLocation] = itemCode;
    }



}
