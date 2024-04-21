using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class BtnType : MonoBehaviour
{
    public BTNType currentType;
   // [SerializeField] private SceneName sceneNameGoto = SceneName.Scene1_MainMenu;

    public void OnBtnClick()
    {
        switch (currentType)
        {
            case BTNType.Start:
                SceneManager.LoadSceneAsync(1);
                break;

            case BTNType.Option:
                GameObject.Find("Main").transform.Find("Main").gameObject.SetActive(false);
                GameObject.Find("Option").transform.Find("Option").gameObject.SetActive(true);
                break;
            case BTNType.Quit:
                Application.Quit();
                break;
            case BTNType.Back:
                GameObject.Find("Main").transform.Find("Main").gameObject.SetActive(true);
                GameObject.Find("Option").transform.Find("Option").gameObject.SetActive(false);
                break;

        }
    }

    

}
