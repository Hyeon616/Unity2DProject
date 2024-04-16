using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class UI_CraftingSystem : MonoBehaviour
{
    
    [SerializeField] private GameObject go_SlotsParent;
    private Slot[] slots;

    private Transform outputSlotTransform;
    private Transform itemContainer;

    private void Awake()
    {
        slots = go_SlotsParent.GetComponentsInChildren<Slot>();

        
    }


    public void AcquireItem(Item _item, int _count = 1)
    {
        if (Item.ItemType.Equipment != _item.itemType)
        {
            for (int i = 0; i < slots.Length; i++)
            {
                if (slots[i].item != null)
                {
                    if (slots[i].item.itemName == _item.itemName)
                    {
                        slots[i].SetSlotCount(_count);
                        return;
                    }
                }
            }
        }

        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].item == null)
            {
                slots[i].AddItem(_item, _count);
                return;
            }
        }
    }
    //private void CreateItem(int x, int y, Item item)
    //{
    //    Transform itemTransform = Instantiate(item,itemContainer);
    //    RectTransform itemRectTransform = itemTransform.GetComponent<RectTransform>();
    //    itemRectTransform.anchoredPosition = slotTransformArray[x, y].GetComponent<RectTransform>().anchoredPosition;
    //    itemTransform.GetComponent<UI_Item>().SetItem(item);


    //}

    //private void CreateItemOutput(Item item)
    //{
    //    Transform itemTransform = Instantiate(item, itemContainer);
    //    RectTransform itemRectTransform = itemTransform.GetComponent<RectTransform>();
    //    itemRectTransform.anchoredPosition = outputSlotTransform.GetComponent<RectTransform>().anchoredPosition;
    //    itemTransform.GetComponent<UI_Item>().SetItem(item);


    //}

}
