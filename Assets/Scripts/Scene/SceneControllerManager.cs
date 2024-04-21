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
        // fading 시작
        isFading = true;

        // 클릭방지
        faderCanvasGroup.blocksRaycasts = true;

        float fadeSpeed = Mathf.Abs(faderCanvasGroup.alpha - finalAlpha) / fadeDuration;

        // faderCanvasGroup.alpha 와 finalAlpha 이 같으면 종료
        while (!Mathf.Approximately(faderCanvasGroup.alpha, finalAlpha))
        {
            faderCanvasGroup.alpha = Mathf.MoveTowards(faderCanvasGroup.alpha, finalAlpha, fadeSpeed * Time.deltaTime);

            // 프레임만큼 기다렸다면 진행
            yield return null;

        }

        // fading이 끝났다면
        isFading = false;

        // 클릭방지를 해제
        faderCanvasGroup.blocksRaycasts = false;

    }

    private IEnumerator FadeAndSwitchScenes(string sceneName, Vector3 spawnPosition)
    {
        // Scene 로딩 시작
        EventHandler.CallBeforeSceneUnloadFadeOutEvent();

        // Scene 로딩중 화면 까맣게 (알파값)
        yield return StartCoroutine(Fade(1f));

        //PlayerController.Instance.gameObject.transform.position = spawnPosition;

        // Scene이 로딩중
        EventHandler.CallBeforeSceneUnloadEvent();
        // scene이 로딩될때까지
        yield return SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene().buildIndex);

        // Scene 로딩이 끝남
        yield return StartCoroutine(LoadSceneAndSetActive(sceneName));

        // Scene이 로딩되면
        EventHandler.CallAfterSceneLoadEvent();

        // Scene 로딩이 끝나면 화면 하얗게 (알파값)
        yield return StartCoroutine(Fade(0f));

        EventHandler.CallAfterSceneLoadFadeInEvent();
    }




    private IEnumerator LoadSceneAndSetActive(string sceneName)
    {
        // Scene sceneName 로드
        yield return SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

        // 최근 로드된 씬을 찾고 newlyLoadedScene에 할당
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
