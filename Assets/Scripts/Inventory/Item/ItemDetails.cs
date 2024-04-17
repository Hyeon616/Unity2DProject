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
    public string itemDescription; // 설명
    public Sprite itemSprite;
    public string itemLongDescription; // 긴설명
    public short itemUseGridRadius; // 아이템을 놓을 수 있는 사거리
    public float itemUseRadius;
    public bool isStartingItem;
    public bool canBePickedUp;
    public bool canBeDropped;
    public bool canBeEaten;




}
