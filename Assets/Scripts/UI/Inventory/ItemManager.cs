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

    void LoadItems()
    {
        string jsonText = File.ReadAllText(Application.dataPath + "/Resources/items.json");
        ItemDatabase itemDatabase = JsonUtility.FromJson<ItemDatabase>(jsonText);

        foreach (var itemData in itemDatabase.items.Values)
        {
            items.Add(itemData.id, new Item(itemData));
        }

        Debug.Log($"Loaded {items.Count} items.");
    }

    void LoadSkills()
    {
        string jsonText = File.ReadAllText(Application.dataPath + "/Resources/skills.json");
        SkillDatabase skillDatabase = JsonUtility.FromJson<SkillDatabase>(jsonText);

        foreach (var skillData in skillDatabase.skills.Values)
        {
            skills.Add(skillData.id, new Skill(skillData));
        }

        Debug.Log($"Loaded {skills.Count} skills.");
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

    public Skill GetSkill(int id)
    {
        if (skills.TryGetValue(id, out Skill skill))
        {
            return skill;
        }
        Debug.LogWarning($"Skill with id {id} not found.");
        return null;
    }

    public List<Item> GetAllItems()
    {
        return new List<Item>(items.Values);
    }

    public List<Skill> GetAllSkills()
    {
        return new List<Skill>(skills.Values);
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

    public Skill GetSkillByName(string name)
    {
        foreach (var skill in skills.Values)
        {
            if (skill.data.name.ToLower() == name.ToLower())
            {
                return skill;
            }
        }
        Debug.LogWarning($"Skill with name '{name}' not found.");
        return null;
    }
}
