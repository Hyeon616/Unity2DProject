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
    public GameObject MixingUI;


    void Update()
    {
        if (Input.anyKeyDown)
        {
            switch (Input.inputString.ToLower())
            {
                case "i":
                    ToggleInventoryUI();
                    break;
                case "k":
                    ToggleSkillUI();
                    break;
                case "o":
                    ToggleOptionUI();
                    break;
                case "l":
                    ToggleTalentsUI();
                    break;
                case "p":
                    ToggleEquipUI();
                    break;
                case "j":
                    ToggleMixingUI();
                    break;
            }
        }

    }

    private void ToggleInventoryUI()
    {
        inventoryUI.SetActive(!inventoryUI.activeSelf);
    }

    private void ToggleSkillUI()
    {
        SkillUI.SetActive(!SkillUI.activeSelf);
    }

    private void ToggleOptionUI()
    {
        OptionUI.SetActive(!OptionUI.activeSelf);
    }

    private void ToggleTalentsUI()
    {
        TalentsUI.SetActive(!TalentsUI.activeSelf);
    }

    private void ToggleEquipUI()
    {
        EquipUI.SetActive(!EquipUI.activeSelf);
    }

    private void ToggleMixingUI()
    {
        MixingUI.SetActive(!MixingUI.activeSelf);
    }

}
