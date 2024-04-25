using System;
using UnityEngine;


[Serializable]
public class ItemDetails
{

    public int itemCode;
    public ItemType itemType;
    public EquipmentType equipmentType;
    public WeaponType weaponType;
    public string itemName; // ����
    public string itemDescription; // ����
    public Sprite itemSprite;
    public int blockHp;
    public int attackDamage;
    public int abilityPower;
    public int miningDamage;
    public string itemLongDescription; // �伳��
    public short itemUseGridRadius; // �������� ���� �� �ִ� ��Ÿ�
    public float itemUseRadius;
    public bool isStartingItem;
    public bool canBePickedUp;
    public bool canBeDropped;
    public bool canBeEaten;


}
