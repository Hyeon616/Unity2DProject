using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] private SpriteRenderer backGroundColor;


    protected override void Awake()
    {

        base.Awake();

        Screen.SetResolution(1280, 720, false);
    }

    private void FixedUpdate()
    {
        backGroundColor.color = new Color(0f, 0f, 0f, GetCurrentTimeAlpha());
    }

    private float GetCurrentTimeAlpha()
    {
        float currentTime = TimeManager.Instance.GetGameHour();

        if (currentTime < 3 || currentTime >= 22)
        {
            return Settings.backgroundAlphaAt22to3;
        }
        else if ((currentTime >= 3 && currentTime < 5) || (currentTime >= 20 && currentTime < 22))
        {
            return Settings.backgroundAlphaAt3to5And20to22;
        }
        else if ((currentTime >= 5 && currentTime < 7) || (currentTime >= 18 && currentTime < 20))
        {
            return Settings.backgroundAlphaAt5to7And18to20;
        }
        else if ((currentTime >= 7 && currentTime < 9) || (currentTime >= 16 && currentTime < 18))
        {
            return Settings.backgroundAlphaAt7to9And16to18;
        }
        else if ((currentTime >= 9 && currentTime < 11) || (currentTime >= 14 && currentTime < 16))
        {
            return Settings.backgroundAlphaAt9to11And14to16;
        }
        else if (currentTime >= 11 && currentTime < 14)
        {
            return Settings.backgroundAlphaAt11to14;
        }
        return 0;
    }
}
