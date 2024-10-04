using System;
using System.Collections.Generic;
using UnityEngine;

public class InventorySlot
{
    public Item item;
    public Skill skill;
    public int amount;

    public bool IsEmpty => item == null && skill == null;

    public void Clear()
    {
        item = null;
        skill = null;
        amount = 0;
    }

    public void SetItem(Item newItem, int quantity = 1)
    {
        item = newItem;
        skill = null;
        amount = (newItem.data.ItemTypes.HasFlag(ItemType.Consumable) || newItem.data.ItemTypes.HasFlag(ItemType.Material)) ? quantity : 1;
    }

    public void SetSkill(Skill newSkill)
    {
        skill = newSkill;
        item = null;
        amount = 1;
    }
}



public class InventoryManager : MonoBehaviour
{
    public Dictionary<int, InventorySlot> hotbarItemSlots = new Dictionary<int, InventorySlot>();
    public Dictionary<int, InventorySlot> hotbarSkillSlots = new Dictionary<int, InventorySlot>();
    public Dictionary<int, InventorySlot> inventorySlots = new Dictionary<int, InventorySlot>();
    public Dictionary<string, InventorySlot> equipmentSlots = new Dictionary<string, InventorySlot>();

    public bool IsInitialized { get; private set; }

    public event Action OnInventoryChanged;

    void Awake()
    {
        Initialize();
    }

    private void Start()
    {
        AddItem(1,10);
    }

    public void Initialize()
    {
        if (!IsInitialized)
        {
            InitializeSlots();
            InitializeEquipmentSlots();
            IsInitialized = true;
        }
    }

    void InitializeSlots()
    {
        for (int i = 0; i < 6; i++)
        {
            hotbarItemSlots[i] = new InventorySlot();
            hotbarSkillSlots[i] = new InventorySlot();
        }

        for (int i = 0; i < 35; i++)
        {
            inventorySlots[i] = new InventorySlot();
        }
    }

    void InitializeEquipmentSlots()
    {
        string[] equipmentTypes = { "Weapon", "Head", "Shoulder", "Belt", "Pants", "Shoes", "Gloves", "Ring", "Neck", "Jewelery", "Bracelet", "Trinket" };
        foreach (string type in equipmentTypes)
        {
            equipmentSlots[type] = new InventorySlot();
        }
    }

    public bool AddItem(int itemId, int amount = 1)
    {
        Item itemToAdd = ItemManager.Instance.GetItem(itemId);
        if (itemToAdd == null)
        {
            Debug.LogWarning($"Item with ID {itemId} not found.");
            return false;
        }

        if (itemToAdd.data.ItemTypes.HasFlag(ItemType.Consumable) || itemToAdd.data.ItemTypes.HasFlag(ItemType.Material))
        {
            // 소비 아이템과 재료는 중첩 가능
            foreach (var slot in inventorySlots.Values)
            {
                if (!slot.IsEmpty && slot.item != null && slot.item.data.id == itemId)
                {
                    slot.amount += amount;
                    OnInventoryChanged?.Invoke();
                    return true;
                }
            }
        }

        // 새 슬롯에 아이템 추가
        foreach (var slot in inventorySlots.Values)
        {
            if (slot.IsEmpty)
            {
                slot.SetItem(itemToAdd, amount);
                OnInventoryChanged?.Invoke();
                return true;
            }
        }

        Debug.Log("Inventory is full!");
        return false;
    }

    public void MoveItem(InventorySlot fromSlot, InventorySlot toSlot)
    {
        if (fromSlot == null || toSlot == null || fromSlot.IsEmpty) return;

        if (fromSlot.item != null) // 아이템 이동
        {
            if (toSlot.IsEmpty)
            {
                toSlot.SetItem(fromSlot.item, fromSlot.amount);
                fromSlot.Clear();
            }
            else if (toSlot.item != null && fromSlot.item.data.id == toSlot.item.data.id &&
                     (fromSlot.item.data.ItemTypes.HasFlag(ItemType.Consumable) || fromSlot.item.data.ItemTypes.HasFlag(ItemType.Material)))
            {
                toSlot.amount += fromSlot.amount;
                fromSlot.Clear();
            }
            else
            {
                // 비소모품 아이템은 교체
                Item tempItem = toSlot.item;
                int tempAmount = toSlot.amount;
                toSlot.SetItem(fromSlot.item, fromSlot.amount);
                fromSlot.SetItem(tempItem, tempAmount);
            }
        }
        else if (fromSlot.skill != null) // 스킬 이동
        {
            if (toSlot.IsEmpty && IsSkillSlot(toSlot))
            {
                toSlot.SetSkill(fromSlot.skill);
                fromSlot.Clear();
            }
            else if (!toSlot.IsEmpty && IsSkillSlot(toSlot))
            {
                // 스킬 교체
                Skill tempSkill = toSlot.skill;
                toSlot.SetSkill(fromSlot.skill);
                fromSlot.SetSkill(tempSkill);
            }
        }

        OnInventoryChanged?.Invoke();
    }

    public void EquipItem(InventorySlot fromSlot, string equipSlot)
    {
        if (fromSlot == null || fromSlot.IsEmpty || !equipmentSlots.ContainsKey(equipSlot) || fromSlot.item == null) return;

        if (fromSlot.item.data.ItemTypes.HasFlag(ItemType.Equipment))
        {
            InventorySlot toSlot = equipmentSlots[equipSlot];
            Item tempItem = toSlot.item;
            toSlot.SetItem(fromSlot.item);
            if (tempItem != null)
            {
                fromSlot.SetItem(tempItem);
            }
            else
            {
                fromSlot.Clear();
            }
            OnInventoryChanged?.Invoke();
        }
        else
        {
            Debug.Log("This item cannot be equipped.");
        }
    }

    public bool IsItemSlot(InventorySlot slot)
    {
        return hotbarItemSlots.ContainsValue(slot) || inventorySlots.ContainsValue(slot);
    }

    public bool IsSkillSlot(InventorySlot slot)
    {
        return hotbarSkillSlots.ContainsValue(slot);
    }

    public InventorySlot GetSlotByIndex(Dictionary<int, InventorySlot> slotDictionary, int index)
    {
        if (slotDictionary.TryGetValue(index, out InventorySlot slot))
        {
            return slot;
        }
        return null;
    }
}
