using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Inventory
{
    public event EventHandler OnItemListChanged;

    private List<Item> itemList;
    private Action<Item> useItemAction;

    public Inventory(Action<Item> useItemAction)
    {
        itemList = new List<Item>();

        AddItem(new Item { itemType = Item.ItemType.Sword, amount = 1 });
        AddItem(new Item { itemType = Item.ItemType.HealthPotion, amount = 1 });
        AddItem(new Item { itemType = Item.ItemType.ManaPotion, amount = 1 });
        this.useItemAction = useItemAction;
    }

    public void AddItem(Item item)
    {
        if(item.IsStackable())
        {
            bool itemAlreadyInInventory = false;
            foreach (Item inventoryITem in itemList)
            {
                if(inventoryITem.itemType == item.itemType)
                {
                    inventoryITem.amount += item.amount;
                }
            }
            if(!itemAlreadyInInventory)
            {
                itemList.Add(item);
            }
        }
        else
        {
            itemList.Add(item);

        }

        OnItemListChanged?.Invoke(this, EventArgs.Empty);
    }

    public void RemoveItem(Item item)
    {
        if (item.IsStackable())
        {
            Item itemInInventory = null;
            foreach (Item inventoryITem in itemList)
            {
                if (inventoryITem.itemType == item.itemType)
                {
                    inventoryITem.amount -= item.amount;
                    itemInInventory = inventoryITem;
                }
            }
            if (itemInInventory != null && itemInInventory.amount <= 0)
            {
                itemList.Remove(item);
            }
        }
        else
        {
            itemList.Remove(item);

        }

        OnItemListChanged?.Invoke(this, EventArgs.Empty);
    }

    public void UseItem(Item item)
    {
        useItemAction(item);
    }

    public List<Item> GetItemList() 
    {
        return itemList;
    }


}
