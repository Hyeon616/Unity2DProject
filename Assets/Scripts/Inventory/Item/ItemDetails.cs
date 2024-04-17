using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;


[Serializable]
public class ItemDetails
{


    public int itemCode;
    public ItemType itemType;
    public string itemDescription; // ����
    public Sprite itemSprite;
    public string itemLongDescription; // �伳��
    public short itemUseGridRadius; // �������� ���� �� �ִ� ��Ÿ�
    public float itemUseRadius;
    public bool isStartingItem;
    public bool canBePickedUp;
    public bool canBeDropped;
    public bool canBeEaten;




}
