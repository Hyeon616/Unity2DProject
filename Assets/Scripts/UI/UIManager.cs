using System;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager>
{

    private bool _pauseMenuOn = false;
    [SerializeField] private UIInventoryBar uiInventoryBar = null;
    [SerializeField] private PauseMenuInventoryManagement pauseMenuInventoryManagement = null;

    [SerializeField] private GameObject pauseMenu = null;
    [SerializeField] private GameObject[] menuTabs = null;
    [SerializeField] private Button[] menuButtons = null;
    

    public bool PauseMenuOn { get { return _pauseMenuOn; } set { _pauseMenuOn = value; } }

    protected override void Awake()
    {
        base.Awake();

        pauseMenu.SetActive(false);

    }

    private void Update()
    {
        PauseMenu();
    }

    private void PauseMenu()
    {
        if(Input.GetKeyDown(KeyCode.I))
        {
            if(PauseMenuOn)
            {
                DisablePauseMenu();
            }
            else
            {
                EnablePauseMenu();
            }

        }


    }

    private void DisablePauseMenu()
    {
        pauseMenuInventoryManagement.DestoryCurrentlyDraggedItems();

        PauseMenuOn = false;
        Time.timeScale = 1;
        pauseMenu.SetActive(false);

    }

    private void EnablePauseMenu()
    {
        uiInventoryBar.DestroyCurrentlyDraggedItems();

        uiInventoryBar.ClearCurrentlySelectedItems();


        PauseMenuOn = true;
        Time.timeScale = 0;
        pauseMenu.SetActive(true);

        GC.Collect();

        HighlightButtonForSelectedTab();

    }

    private void HighlightButtonForSelectedTab()
    {
        for (int i = 0; i < menuTabs.Length; i++)
        {
            if (menuTabs[i].activeSelf)
            {
                SetButtonColorToActive(menuButtons[i]);
            }
            else
            {
                SetButtonColorToInactive(menuButtons[i]);

            }
        }
    }

    private void SetButtonColorToInactive(Button button)
    {
        ColorBlock colors = button.colors;

        colors.normalColor = colors.disabledColor;

        button.colors = colors;

    }

    private void SetButtonColorToActive(Button button)
    {
        ColorBlock colors = button.colors;

        colors.normalColor = colors.pressedColor;

        button.colors = colors;
    }

    public void SwitchPauseMenuTab(int tabNum)
    {
        for (int i = 0; i < menuTabs.Length; i++)
        {
            if(i != tabNum)
            {
                menuTabs[i].SetActive(false);

            }
            else
            {
                menuTabs[i].SetActive(true);
            }
        }
        HighlightButtonForSelectedTab();

    }

    public void QuitGame()
    {
        Application.Quit();
    }


}
