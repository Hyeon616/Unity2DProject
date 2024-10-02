using System;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
    Consumable,
    Equipment,
    Material,
    Decorative
}


[Serializable]
public class ItemData 
{
    public int id;
    public string name;
    public string description;
    public string itemimage;
    public ItemType itemType;
    public int ad;
    public int ap;
    public int durability;
    public int defense;

}

[Serializable]
public class ItemDatabase
{
    public Dictionary<string, ItemData> items;
}

public class Item
{
    public ItemData data;
    public Sprite icon;

    public Item(ItemData data)
    {
        this.data = data;
        this.icon = Resources.Load<Sprite>(data.itemimage);
    }
}


[Serializable]
public class SkillDatabase
{
    public Dictionary<int, SkillData> skills;
}

[System.Serializable]
public class SkillData
{
    public int id;
    public string name;
    public string description;
    public string skillimage;
    public int damage;
    public int cost;
}



public class Skill
{
    public SkillData data;
    public Sprite icon;

    public Skill(SkillData data)
    {
        this.data = data;
        this.icon = Resources.Load<Sprite>(data.skillimage);
    }
}