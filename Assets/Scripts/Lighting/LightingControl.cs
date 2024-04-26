using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;


public class LightingControl : MonoBehaviour
{

    [SerializeField] private LightingSchedule lightingSchedule;

    private Light2D light2D;
    private Dictionary<string, float> lightingBrightnessDictionary = new Dictionary<string, float>();
    private float currentLightIntensity;
    private Coroutine fadeInLightRoutine;



    private void Awake()
    {
        
        light2D = GetComponentInChildren<Light2D>();

        if (light2D == null)
        {
            enabled = false;
        }

        foreach (LightingBrightness lightingBrightness in lightingSchedule.lightingBrightnessArray)
        {
            // 중복검사 추가 필요
            string key = lightingBrightness.season.ToString() + lightingBrightness.hour.ToString();

            lightingBrightnessDictionary.Add(key, lightingBrightness.lightIntensity);


        }
    }

    private void OnEnable()
    {
        EventHandler.AdvanceGameHourEvent += EventHandler_AdvanceGameHourEvent;
        EventHandler.AfterSceneLoadEvent += EventHandler_AfterSceneLoadEvent;
    }


    private void OnDisable()
    {
        EventHandler.AdvanceGameHourEvent -= EventHandler_AdvanceGameHourEvent;
        EventHandler.AfterSceneLoadEvent -= EventHandler_AfterSceneLoadEvent;
    }

    private void EventHandler_AfterSceneLoadEvent()
    {
        SetLightingAfterSceneLoaded();
    }



    private void EventHandler_AdvanceGameHourEvent(int gameYear, Season gameSeason, int gameDay, string gameDayOfWeek, int gameHour, int gameMinute, int gameSecond)
    {
        SetLightingIntensity(gameSeason, gameHour, true);
    }


    private void LateUpdate()
    {
        light2D.intensity = currentLightIntensity;
        
    }

    private void SetLightingAfterSceneLoaded()
    {
        Season gameSeason = TimeManager.Instance.GetGameSeason();
        int gameHour = TimeManager.Instance.GetGameTime().Hours;

        SetLightingIntensity(gameSeason, gameHour, false);
    }

    private void SetLightingIntensity(Season gameSeason, int gameHour, bool fadein)
    {
        int index = 0;

        while (index <= 23)
        {
            string key = gameSeason.ToString() + (gameHour).ToString();

            if (lightingBrightnessDictionary.TryGetValue(key, out float targetLightIntensity))
            {

                if (fadein)
                {
                    if (fadeInLightRoutine != null)
                    {
                        StopCoroutine(fadeInLightRoutine);
                    }

                    fadeInLightRoutine = StartCoroutine(FadeLightRoutine(targetLightIntensity));
                    
                }
                else
                {
                    currentLightIntensity = targetLightIntensity;
                    
                }
                break;
            }

            index++;

            gameHour--;

            if (gameHour < 0)
            {
                gameHour = 23;
            }

        }
    }

    private IEnumerator FadeLightRoutine(float targetLightIntensity)
    {
        float fadeDuration = 5f;

        float fadeSpeed = Mathf.Abs(currentLightIntensity - targetLightIntensity) / fadeDuration;

        while (!Mathf.Approximately(currentLightIntensity, targetLightIntensity))
        {
            currentLightIntensity = Mathf.MoveTowards(currentLightIntensity, targetLightIntensity, fadeSpeed * Time.deltaTime);

            yield return null;
        }

        currentLightIntensity = targetLightIntensity;

    }

}
