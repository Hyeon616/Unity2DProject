using System;
using System.Collections;
using System.Collections.Generic;

public static class EventHandler
{

    public static event Action<InventoryLocation, List<InventoryItem>> InventoryUpdatedEvent;

    public static void CallInventoryUpdatedEvent(InventoryLocation inventoryLocation, List<InventoryItem> inventoryList)
    {
        if(InventoryUpdatedEvent != null)
            InventoryUpdatedEvent(inventoryLocation, inventoryList);

    }


}
