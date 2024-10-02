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
        amount = (newItem.data.itemType == ItemType.Consumable || newItem.data.itemType == ItemType.Material) ? quantity : 1;
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

    void Start()
    {
        InitializeSlots();
        InitializeEquipmentSlots();
    }

    void InitializeSlots()
    {
        for (int i = 0; i < 6; i++)
            hotbarItemSlots[i] = new InventorySlot();

        for (int i = 0; i < 6; i++)
            hotbarSkillSlots[i] = new InventorySlot();

        for (int i = 0; i < 35; i++)
            inventorySlots[i] = new InventorySlot();
    }

    void InitializeEquipmentSlots()
    {
        string[] equipmentTypes = { "Weapon", "Head", "Shoulder", "Belt", "Pants", "Shoes", "Gloves", "Accessory1", "Accessory2", "Accessory3", "Accessory4" };
        foreach (string type in equipmentTypes)
        {
            equipmentSlots[type] = new InventorySlot();
        }
    }

    public bool AddItem(int itemId, int amount = 1)
    {
        Item itemToAdd = ItemManager.Instance.GetItem(itemId);
        if (itemToAdd == null) return false;

        if (itemToAdd.data.itemType == ItemType.Consumable || itemToAdd.data.itemType == ItemType.Material)
        {
            // 소비 아이템과 재료는 중첩 가능
            foreach (var slot in inventorySlots.Values)
            {
                if (!slot.IsEmpty && slot.item != null && slot.item.data.id == itemId)
                {
                    slot.amount += amount;
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
                return true;
            }
        }

        Debug.Log("Inventory is full!");
        return false;
    }

    public bool AddSkill(int skillId)
    {
        Skill skillToAdd = ItemManager.Instance.GetSkill(skillId);
        if (skillToAdd == null) return false;

        // 핫바의 스킬 슬롯에 추가
        foreach (var slot in hotbarSkillSlots.Values)
        {
            if (slot.IsEmpty)
            {
                slot.SetSkill(skillToAdd);
                return true;
            }
        }

        Debug.Log("No empty skill slot in hotbar!");
        return false;
    }

    public void MoveItem(InventorySlot fromSlot, InventorySlot toSlot)
    {
        if (fromSlot.IsEmpty) return;

        if (fromSlot.item != null) // 아이템 이동
        {
            if (toSlot.IsEmpty)
            {
                toSlot.SetItem(fromSlot.item, fromSlot.amount);
                fromSlot.Clear();
            }
            else if (toSlot.item != null && fromSlot.item.data.id == toSlot.item.data.id &&
                     (fromSlot.item.data.itemType == ItemType.Consumable || fromSlot.item.data.itemType == ItemType.Material))
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
    }

    public void EquipItem(InventorySlot fromSlot, string equipSlot)
    {
        if (fromSlot.IsEmpty || !equipmentSlots.ContainsKey(equipSlot) || fromSlot.item == null) return;

        if (fromSlot.item.data.itemType == ItemType.Equipment)
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
}
