using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputKey : MonoBehaviour
{
    public GameObject inventoryUI;
    public GameObject SkillUI;
    public GameObject OptionUI;
    public GameObject TalentsUI;
    public GameObject EquipUI;
    // 조합 추가


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

}
