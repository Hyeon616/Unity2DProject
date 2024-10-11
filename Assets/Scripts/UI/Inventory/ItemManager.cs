using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    public static ItemManager Instance { get; private set; }
    private Dictionary<int, Item> items = new Dictionary<int, Item>();
    private Dictionary<int, Skill> skills = new Dictionary<int, Skill>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadItems();
            LoadSkills();

        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void LoadSkills()
    {
     
    }

    void LoadItems()
    {
        TextAsset jsonFile = Resources.Load<TextAsset>("Data/items");
        if (jsonFile != null)
        {
            ItemDatabase database = JsonConvert.DeserializeObject<ItemDatabase>(jsonFile.text);
            foreach (var kvp in database.items)
            {
                int id = int.Parse(kvp.Key);
                ItemData itemData = kvp.Value;

                // ItemData 타입에 따라 적절한 Item 객체 생성
                Item item;
                if (itemData is ConsumableItemData)
                {
                    item = new Item(itemData as ConsumableItemData);
                }
                else if (itemData is EquipmentItemData)
                {
                    item = new Item(itemData as EquipmentItemData);
                }
                else
                {
                    item = new Item(itemData);
                }

                items[id] = item;
            }
        }
        else
        {
            Debug.LogError("Failed to load items.json");
        }
    }

    public Item GetItem(int id)
    {
        if (items.TryGetValue(id, out Item item))
        {
            return item;
        }
        Debug.LogWarning($"Item with id {id} not found.");
        return null;
    }

    public List<Item> GetAllItems()
    {
        return new List<Item>(items.Values);
    }

    public Item GetItemByName(string name)
    {
        foreach (var item in items.Values)
        {
            if (item.data.name.ToLower() == name.ToLower())
            {
                return item;
            }
        }
        Debug.LogWarning($"Item with name '{name}' not found.");
        return null;
    }

}
