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
    public GameObject hotbarUI;
    public GameObject inventoryUISlot;
    public GameObject equipmentUI;
    public GameObject mixingUI;

    public Dictionary<int, InventorySlot> hotbarItemSlots = new Dictionary<int, InventorySlot>();
    public Dictionary<int, InventorySlot> hotbarSkillSlots = new Dictionary<int, InventorySlot>();
    public Dictionary<int, InventorySlot> inventorySlots = new Dictionary<int, InventorySlot>();
    public Dictionary<string, InventorySlot> equipmentSlots = new Dictionary<string, InventorySlot>();
    public Dictionary<int, InventorySlot> mixSlots = new Dictionary<int, InventorySlot>();


    public bool IsInitialized { get; private set; }

    public event Action OnInventoryChanged;
    public event Action OnMixSlotChanged;

    void Awake()
    {
        Initialize();
    }

    private void Start()
    {
        AddItem(1, 2);
        AddItem(2, 2);
        AddItem(3, 2);
        UpdateAllUI();
    }

    public void Initialize()
    {
        if (!IsInitialized)
        {
            InitializeSlots();
            InitializeEquipmentSlots();
            InitializeMixSlots();
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

    void InitializeMixSlots()
    {
        for (int i = 0; i < 9; i++)
        {
            mixSlots[i] = new InventorySlot();
        }
    }

    public void UpdateAllUI()
    {
        UpdateHotbarUI();
        UpdateInventoryUI();
        UpdateEquipmentUI();
        UpdateMixUI();
    }

    private void UpdateHotbarUI()
    {
        if (hotbarUI == null) return;
        for (int i = 0; i < 6; i++)
        {
            UpdateSlotUI(GetUISlotAtIndex(hotbarUI, i), hotbarItemSlots[i]);
        }
        for (int i = 6; i < 12; i++)
        {
            UpdateSlotUI(GetUISlotAtIndex(hotbarUI, i), hotbarSkillSlots[i - 6]);
        }
    }

    private void UpdateInventoryUI()
    {
        if (inventoryUISlot == null) return;
        for (int i = 0; i < inventorySlots.Count; i++)
        {
            UpdateSlotUI(GetUISlotAtIndex(inventoryUISlot, i), inventorySlots[i]);
        }
    }

    private void UpdateEquipmentUI()
    {
        if (equipmentUI == null) return;
        foreach (var kvp in equipmentSlots)
        {
            Transform slotTransform = equipmentUI.transform.Find(kvp.Key);
            if (slotTransform != null)
            {
                UISlot uiSlot = slotTransform.GetComponent<UISlot>();
                if (uiSlot != null)
                {
                    UpdateSlotUI(uiSlot, kvp.Value);
                }
            }
        }
    }

    private void UpdateMixUI()
    {
        if (mixingUI == null) return;
        for (int i = 0; i < mixSlots.Count; i++)
        {
            UpdateSlotUI(GetUISlotAtIndex(mixingUI, i), mixSlots[i]);
        }
    }


    private void UpdateSlotUI(UISlot uiSlot, InventorySlot inventorySlot)
    {
        if (uiSlot != null && inventorySlot != null)
        {
            uiSlot.UpdateUI(inventorySlot);
        }
    }

    private UISlot GetUISlotAtIndex(GameObject parent, int index)
    {
        if (parent == null || index < 0 || index >= parent.transform.childCount)
        {
            return null;
        }
        return parent.transform.GetChild(index).GetComponent<UISlot>();
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

    public void MoveItem(InventorySlot fromSlot, InventorySlot toSlot, int amount)
    {
        if (fromSlot == null || toSlot == null || fromSlot.IsEmpty) return;

        // 스킬 슬롯으로의 아이템 이동 방지
        if (fromSlot.item != null && IsSkillSlot(toSlot))
        {
            Debug.Log("Cannot move items to skill slots.");
            return;
        }

        // 아이템 슬롯으로의 스킬 이동 방지
        if (fromSlot.skill != null && !IsSkillSlot(toSlot))
        {
            Debug.Log("Cannot move skills to item slots.");
            return;
        }

        bool isMixSlotAffected = mixSlots.ContainsValue(fromSlot) || mixSlots.ContainsValue(toSlot);

        if (fromSlot.item != null) // 아이템 이동
        {
            int amountToMove = Mathf.Min(amount, fromSlot.amount);
            if (toSlot.IsEmpty)
            {
                toSlot.SetItem(fromSlot.item, amountToMove);
                fromSlot.amount -= amountToMove;
                if (fromSlot.amount <= 0) fromSlot.Clear();
                Debug.Log($"Moved to empty slot. From slot amount: {fromSlot.amount}, To slot amount: {toSlot.amount}");
            }
            else if (toSlot.item != null && fromSlot.item.data.id == toSlot.item.data.id)
            {
                // 동일한 아이템인 경우 갯수 합치기
                int maxStack = GetMaxStackSize(fromSlot.item);
                int spaceAvailable = maxStack - toSlot.amount;
                int actualAmountToMove = Mathf.Min(amountToMove, spaceAvailable);

                toSlot.amount += actualAmountToMove;
                fromSlot.amount -= actualAmountToMove;
                if (fromSlot.amount <= 0) fromSlot.Clear();
                Debug.Log($"Stacked items. From slot amount: {fromSlot.amount}, To slot amount: {toSlot.amount}");
            }
            else
            {
                // 다른 아이템인 경우 교체
                Item tempItem = toSlot.item;
                int tempAmount = toSlot.amount;
                toSlot.SetItem(fromSlot.item, amountToMove);

                if (amountToMove < fromSlot.amount)
                {
                    fromSlot.amount -= amountToMove;
                }
                else
                {
                    fromSlot.SetItem(tempItem, tempAmount);
                }
            }
        }
        else if (fromSlot.skill != null) // 스킬 이동 (변경 없음)
        {
            if (toSlot.IsEmpty)
            {
                toSlot.SetSkill(fromSlot.skill);
                fromSlot.Clear();
            }
            else
            {
                // 스킬 교체
                Skill tempSkill = toSlot.skill;
                toSlot.SetSkill(fromSlot.skill);
                fromSlot.SetSkill(tempSkill);
            }
        }

        OnInventoryChanged?.Invoke();

        if (isMixSlotAffected)
        {
            OnMixSlotChanged?.Invoke();
        }
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

    private int GetMaxStackSize(Item item)
    {
        if (item.data.ItemTypes.HasFlag(ItemType.Consumable) ||
            item.data.ItemTypes.HasFlag(ItemType.Material))
        {
            return int.MaxValue;
        }
        return 1;
    }

    public bool IsItemSlot(InventorySlot slot)
    {
        return hotbarItemSlots.ContainsValue(slot) || inventorySlots.ContainsValue(slot);
    }

    public bool IsSkillSlot(InventorySlot slot)
    {
        return hotbarSkillSlots.ContainsValue(slot);
    }

    public bool IsHotbarSlot(InventorySlot slot)
    {
        return hotbarItemSlots.ContainsValue(slot) || hotbarSkillSlots.ContainsValue(slot);
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
