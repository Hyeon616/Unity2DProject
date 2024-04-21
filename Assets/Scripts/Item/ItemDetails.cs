using System;
using UnityEngine;


[Serializable]
public class ItemDetails
{

    public int itemCode;
    public ItemType itemType;
    public string itemName; // ����
    public string itemDescription; // ����
    public Sprite itemSprite;
    public int blockHp;
    public string itemLongDescription; // �伳��
    public short itemUseGridRadius; // �������� ���� �� �ִ� ��Ÿ�
    public float itemUseRadius;
    public bool isStartingItem;
    public bool canBePickedUp;
    public bool canBeDropped;
    public bool canBeEaten;


}