using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public GameObject hotbarUI;
    public GameObject inventoryUI;
    public GameObject equipmentUI;

    private InventoryManager inventoryManager;

    void Start()
    {
        inventoryManager = GetComponent<InventoryManager>();
        UpdateAllUI();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            ToggleInventoryUI();
        }
    }

    void ToggleInventoryUI()
    {
        inventoryUI.SetActive(!inventoryUI.activeSelf);
    }

    public void UpdateAllUI()
    {
        UpdateHotbarUI();
        UpdateInventoryUI();
        UpdateEquipmentUI();
    }

    void UpdateHotbarUI()
    {
        for (int i = 0; i < 6; i++)
        {
            UpdateSlotUI(hotbarUI.transform.GetChild(i).GetComponent<UISlot>(), inventoryManager.hotbarItemSlots[i]);
        }
        for (int i = 6; i < 12; i++)
        {
            UpdateSlotUI(hotbarUI.transform.GetChild(i).GetComponent<UISlot>(), inventoryManager.hotbarSkillSlots[i - 6]);
        }
    }

    void UpdateInventoryUI()
    {
        for (int i = 0; i < inventoryManager.inventorySlots.Count; i++)
        {
            UpdateSlotUI(inventoryUI.transform.GetChild(i).GetComponent<UISlot>(), inventoryManager.inventorySlots[i]);
        }
    }

    void UpdateEquipmentUI()
    {
        foreach (var kvp in inventoryManager.equipmentSlots)
        {
            UpdateSlotUI(equipmentUI.transform.Find(kvp.Key).GetComponent<UISlot>(), kvp.Value);
        }
    }

    void UpdateSlotUI(UISlot uiSlot, InventorySlot inventorySlot)
    {
        uiSlot.UpdateUI(inventorySlot);
    }
}
