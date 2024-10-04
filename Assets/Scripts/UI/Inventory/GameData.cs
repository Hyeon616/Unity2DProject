using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;

[Flags]
public enum ItemType
{
    None = 0,
    Consumable = 1 << 0,
    Equipment = 1 << 1,
    Material = 1 << 2,
    Craftable = 1 << 3,
    Decorative = 1 << 4
}


[Serializable]
public class ItemData 
{
    public int id;
    public string name;
    public string description;
    public string itemImage;
    public List<string> itemType;

    [JsonIgnore]
    private ItemType? _ItemTypes;

    [JsonIgnore]
    public ItemType ItemTypes
    {
        get
        {
            if (_ItemTypes == null)
            {
                _ItemTypes = ItemType.None;
                foreach (string type in itemType)
                {
                    if (Enum.TryParse(type, true, out ItemType parsedType))
                    {
                        _ItemTypes |= parsedType;
                    }
                }
            }
            return _ItemTypes.Value;
        }
    }
}

public class ConsumableItemData : ItemData
{
    public int healthRestore;
    public int manaRestore;
    public float buffDuration;
}

public class EquipmentItemData : ItemData
{
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
        LoadIcon();
    }

    private void LoadIcon()
    {
        if (!string.IsNullOrEmpty(data.itemImage))
        {
            icon = Resources.Load<Sprite>($"Items/{data.itemImage}");
            if (icon == null)
            {
                Debug.LogWarning($"Failed to load icon for item: {data.name}, Path: Items/{data.itemImage}");
            }
        }
        else
        {
            Debug.LogWarning($"Item image path is empty for item: {data.name}");
        }
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