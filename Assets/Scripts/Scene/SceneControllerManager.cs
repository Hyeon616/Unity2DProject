using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneControllerManager : Singleton<SceneControllerManager>
{

    public float fadeDuration = 1f;
    private CanvasGroup fadeCanvasGroup;

    protected override void Awake()
    {
        base.Awake();
        if (Instance == this)
        {
            DontDestroyOnLoad(gameObject);
            InitializeCanvas();
        }
    }

    private void InitializeCanvas()
    {
        GameObject fadeCanvas = new GameObject("Fade Canvas");
        fadeCanvas.AddComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
        fadeCanvas.AddComponent<CanvasScaler>();
        fadeCanvas.AddComponent<GraphicRaycaster>();
        fadeCanvasGroup = fadeCanvas.AddComponent<CanvasGroup>();

        GameObject imageObj = new GameObject("Fade Image");
        imageObj.transform.SetParent(fadeCanvas.transform, false);
        Image image = imageObj.AddComponent<Image>();
        image.color = Color.black;
        RectTransform rectTransform = image.rectTransform;
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;
        rectTransform.sizeDelta = Vector2.zero;

        fadeCanvasGroup.alpha = 0f;
        DontDestroyOnLoad(fadeCanvas);
    }

    public void LoadScene(string sceneName)
    {
        StartCoroutine(FadeAndLoadScene(sceneName));
    }

    private IEnumerator FadeAndLoadScene(string sceneName)
    {
        // Fade out
        yield return StartCoroutine(Fade(0f, 1f));

        // Load new scene
        SceneManager.LoadScene(sceneName);

        // Fade in
        yield return StartCoroutine(Fade(1f, 0f));
    }

    private IEnumerator Fade(float startAlpha, float endAlpha)
    {
        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / fadeDuration);
            fadeCanvasGroup.alpha = alpha;
            yield return null;
        }
        fadeCanvasGroup.alpha = endAlpha;
    }


}
