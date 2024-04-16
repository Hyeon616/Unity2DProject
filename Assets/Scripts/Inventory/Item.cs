using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="New Item", menuName ="New Item/item")]
public class Item : ScriptableObject
{

    public string itemName;
    public Sprite itemImage;
    public GameObject itemPrefab;

    public string weaponType;
    public ItemType itemType;
    public int amount;

    public enum ItemType
    {
        Equipment,
        Used,
        Ingredient,
        ETC,

    }

    
}
