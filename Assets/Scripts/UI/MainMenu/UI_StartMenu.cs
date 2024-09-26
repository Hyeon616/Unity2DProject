using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UI_StartMenu : MonoBehaviour
{
    [Header("Btn")]
    [SerializeField] private Button StartBtn;
    [SerializeField] private Button LoadBtn;
    [SerializeField] private Button OptionBtn;
    [SerializeField] private Button ExitBtn;

    [SerializeField] private GameObject OptionPanel;

    private void OnEnable()
    {
        StartBtn.onClick.AddListener(OnClickStartButton);
        LoadBtn.onClick.AddListener(OnClickLoadButton);
        OptionBtn.onClick.AddListener(OnClickOptionButton);
        ExitBtn.onClick.AddListener(OnClickExitButton);



    }


    private void OnDisable()
    {
        StartBtn.onClick.RemoveListener(OnClickStartButton);
        LoadBtn.onClick.RemoveListener(OnClickLoadButton);
        OptionBtn.onClick.RemoveListener(OnClickOptionButton);
        ExitBtn.onClick.RemoveListener(OnClickExitButton);


    }


    private void OnClickStartButton()
    {
        SceneControllerManager.Instance.LoadScene("Scene2_InGame");
    }

    private void OnClickLoadButton()
    {
        Debug.Log("OnClickLoadButton");
    }
    private void OnClickOptionButton()
    {
        Debug.Log("OnClickOptionButton");
    }
    private void OnClickExitButton()
    {
        Debug.Log("OnClickExitButton");
        Application.Quit();
    }

}
