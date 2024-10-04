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
        Initialize();

        if (inventoryManager != null)
        {
            inventoryManager.OnInventoryChanged += UpdateAllUI;
        }
        else
        {
            Debug.LogError("InventoryManager not found!");
        }

    }

    void OnDestroy()
    {
        if (inventoryManager != null)
        {
            inventoryManager.OnInventoryChanged -= UpdateAllUI;
        }
    }

    private void Initialize()
    {
        
        if (inventoryManager == null)
        {
            Debug.LogError("InventoryManager not found on the same GameObject as UIManager!");
            return;
        }

        if (!inventoryManager.IsInitialized)
        {
            inventoryManager.Initialize();
        }

    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            ToggleInventoryUI();
        }
    }

    private void ToggleInventoryUI()
    {
        inventoryUI.SetActive(!inventoryUI.activeSelf);
    }

    public void UpdateAllUI()
    {
        if (inventoryManager == null) return;

        UpdateHotbarUI();
        UpdateInventoryUI();
        UpdateEquipmentUI();
    }

    private void UpdateHotbarUI()
    {
        if (hotbarUI == null) return;

        for (int i = 0; i < 6; i++)
        {
            if (inventoryManager.hotbarItemSlots.TryGetValue(i, out InventorySlot slot))
            {
                UpdateSlotUI(GetUISlotAtIndex(hotbarUI, i), slot);
            }
        }
        for (int i = 6; i < 12; i++)
        {
            if (inventoryManager.hotbarSkillSlots.TryGetValue(i - 6, out InventorySlot slot))
            {
                UpdateSlotUI(GetUISlotAtIndex(hotbarUI, i), slot);
            }
        }
    }

    private void UpdateInventoryUI()
    {
        if (inventoryUI == null) return;

        for (int i = 0; i < inventoryManager.inventorySlots.Count; i++)
        {
            if (inventoryManager.inventorySlots.TryGetValue(i, out InventorySlot slot))
            {
                UpdateSlotUI(GetUISlotAtIndex(inventoryUI, i), slot);
            }
        }
    }

    private void UpdateEquipmentUI()
    {
        if (equipmentUI == null) return;

        foreach (var kvp in inventoryManager.equipmentSlots)
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
}
