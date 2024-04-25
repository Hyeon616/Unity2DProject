using System;
using UnityEngine;


[Serializable]
public class ItemDetails
{

    public int itemCode;
    public ItemType itemType;
    public EquipmentType equipmentType;
    public WeaponType weaponType;
    public string itemName; // 설명
    public string itemDescription; // 설명
    public Sprite itemSprite;
    public int blockHp;
    public int attackDamage;
    public int abilityPower;
    public int miningDamage;
    public string itemLongDescription; // 긴설명
    public short itemUseGridRadius; // 아이템을 놓을 수 있는 사거리
    public float itemUseRadius;
    public bool isStartingItem;
    public bool canBePickedUp;
    public bool canBeDropped;
    public bool canBeEaten;


}
