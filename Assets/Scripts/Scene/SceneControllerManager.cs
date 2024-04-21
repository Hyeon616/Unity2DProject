using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneControllerManager : Singleton<SceneControllerManager>
{

    private bool isFading;
    [SerializeField] private float fadeDuration = 1.0f;
    [SerializeField] private CanvasGroup faderCanvasGroup = null;
    [SerializeField] private Image faderImage = null;

    public SceneName startingSceneName;

    private IEnumerator Fade(float finalAlpha)
    {
        // fading ����
        isFading = true;

        // Ŭ������
        faderCanvasGroup.blocksRaycasts = true;

        float fadeSpeed = Mathf.Abs(faderCanvasGroup.alpha - finalAlpha) / fadeDuration;

        // faderCanvasGroup.alpha �� finalAlpha �� ������ ����
        while (!Mathf.Approximately(faderCanvasGroup.alpha, finalAlpha))
        {
            faderCanvasGroup.alpha = Mathf.MoveTowards(faderCanvasGroup.alpha, finalAlpha, fadeSpeed * Time.deltaTime);

            // �����Ӹ�ŭ ��ٷȴٸ� ����
            yield return null;

        }

        // fading�� �����ٸ�
        isFading = false;

        // Ŭ�������� ����
        faderCanvasGroup.blocksRaycasts = false;

    }

    private IEnumerator FadeAndSwitchScenes(string sceneName, Vector3 spawnPosition)
    {
        // Scene �ε� ����
        EventHandler.CallBeforeSceneUnloadFadeOutEvent();

        // Scene �ε��� ȭ�� ��İ� (���İ�)
        yield return StartCoroutine(Fade(1f));

        //PlayerController.Instance.gameObject.transform.position = spawnPosition;

        // Scene�� �ε���
        EventHandler.CallBeforeSceneUnloadEvent();
        // scene�� �ε��ɶ�����
        yield return SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene().buildIndex);

        // Scene �ε��� ����
        yield return StartCoroutine(LoadSceneAndSetActive(sceneName));

        // Scene�� �ε��Ǹ�
        EventHandler.CallAfterSceneLoadEvent();

        // Scene �ε��� ������ ȭ�� �Ͼ�� (���İ�)
        yield return StartCoroutine(Fade(0f));

        EventHandler.CallAfterSceneLoadFadeInEvent();
    }




    private IEnumerator LoadSceneAndSetActive(string sceneName)
    {
        // Scene sceneName �ε�
        yield return SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

        // �ֱ� �ε�� ���� ã�� newlyLoadedScene�� �Ҵ�
        Scene newlyLoadedScene = SceneManager.GetSceneAt(SceneManager.sceneCount - 1);

        SceneManager.SetActiveScene(newlyLoadedScene);
    }

    private IEnumerator Start()
    {
        faderImage.color = new Color(0f, 0f, 0f, 1f);
        faderCanvasGroup.alpha = 1f;

        yield return StartCoroutine(LoadSceneAndSetActive(startingSceneName.ToString()));

        EventHandler.CallAfterSceneLoadEvent();

        StartCoroutine(Fade(0f));

    }

    public void FadeAndLoadScene(string sceneName, Vector3 spawnPosition)
    {
        if (!isFading)
        {
            StartCoroutine(FadeAndSwitchScenes(sceneName, spawnPosition));
        }

    }

   

}
