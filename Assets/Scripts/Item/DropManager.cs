using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropManager : Singleton<DropManager>
{
    public static DropManager instance;


    [SerializeField] public string[] dropItemKeys;
    [SerializeField] public GameObject[] dropItemValues;
    public Dictionary<string, GameObject> dropItems;


    protected override void Awake()
    {
        base.Awake();

        instance = this;
        dropItems = new Dictionary<string, GameObject>();

        for (int i = 0; i < dropItemKeys.Length; i++)
        {
            dropItems.Add(dropItemKeys[i], dropItemValues[i]);
        }

    }

    public void ItemDrop(int x, int y ,string _itemName)
    {
        

        if (dropItems.ContainsKey(_itemName))
        {
            
            GameObject itemDrop = dropItems[_itemName];
            itemDrop.layer = 7;
            itemDrop.transform.localScale = new Vector2(0.5f, 0.5f);
            
            Instantiate(itemDrop, new Vector2(x, y + 0.5f), Quaternion.identity);


        }



    }




}
