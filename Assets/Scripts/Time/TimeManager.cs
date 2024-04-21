using System;
using UnityEngine;

public class TimeManager : Singleton<TimeManager>
{
    private int gameYear = 1;
    private Season gameSeason = Season.Summer;
    private int gameDay = 1;
    private int gameHour = 1;
    private int gameMinute = 30;
    private int gameSecond = 0;
    private string gameDayOfWeek = "월";

    private bool gameClockPaused = false;

    private float gameTick = 0f;


    private void Start()
    {
        EventHandler.CallAdvanceGameMinuteEvent(gameYear, gameSeason, gameDay, gameDayOfWeek, gameHour, gameMinute, gameSecond);
    }

    private void Update()
    {
        if (!gameClockPaused)
        {
            GameTick();
        }
        
    }

    private void GameTick()
    {
        gameTick += Time.deltaTime;

        if ((gameTick >= Settings.secondsPerGameSecond))
        {
            gameTick -= Settings.secondsPerGameSecond;

            UpdateGameSecond();
        }
    }

    private void UpdateGameSecond()
    {
        gameSecond++;

        if (gameSecond > 59)
        {
            gameSecond = 0;
            gameMinute++;

            if (gameMinute > 59)
            {
                gameMinute = 0;
                gameHour++;


                if (gameHour > 23)
                {
                    gameHour = 0;
                    gameDay++;

                    if (gameDay > 30)
                    {
                        gameDay = 1;

                        int gs = (int)gameSeason;
                        gs++;

                        gameSeason = (Season)gs;

                        if (gs > 3)
                        {
                            gs = 0;
                            gameSeason = (Season)gs;

                            gameYear++;

                            if (gameYear > 9999)
                                gameYear = 1;

                            EventHandler.CallAdvanceGameYearEvent(gameYear, gameSeason, gameDay, gameDayOfWeek, gameHour, gameMinute, gameSecond);
                        }

                        EventHandler.CallAdvanceGameSeasonEvent(gameYear, gameSeason, gameDay, gameDayOfWeek, gameHour, gameMinute, gameSecond);
                    }

                    gameDayOfWeek = GetDayOfWeek();
                    EventHandler.CallAdvanceGameDayEvent(gameYear, gameSeason, gameDay, gameDayOfWeek, gameHour, gameMinute, gameSecond);

                }

                EventHandler.CallAdvanceGameHourEvent(gameYear, gameSeason, gameDay, gameDayOfWeek, gameHour, gameMinute, gameSecond);

            }
            EventHandler.CallAdvanceGameMinuteEvent(gameYear, gameSeason, gameDay, gameDayOfWeek, gameHour, gameMinute, gameSecond);
            
        }


    }

    private string GetDayOfWeek()
    {
        int totalDays = (((int)gameSeason) * 30) + gameDay;
        int dayOfWeek = totalDays % 7;

        switch (dayOfWeek)
        {
            case 1:
                return "월";

            case 2:
                return "화";

            case 3:
                return "수";

            case 4:
                return "목";

            case 5:
                return "금";

            case 6:
                return "토";

            case 0:
                return "일";

            default:
                return "";
        }
    }

    public TimeSpan GetGameTime()
    {
        TimeSpan gameTime = new TimeSpan(gameHour, gameMinute, gameSecond);

        return gameTime;
    }

    public int GetGameHour()
    {
        return gameHour;
    }
    public Season GetGameSeason()
    {

        return gameSeason;
    }
    public string GetGameSeasonString()
    {

        switch (gameSeason)
        {
            case Season.Spring:
                return "봄";
            case Season.Summer:
                return "여름";
            case Season.Autumn:
                return "가을";
            case Season.Winter:
                return "겨울";
        }

        return "";
    }

    public void TestAdvanceGameMinute()
    {
        for (int i = 0; i < 600; i++)
        {
            UpdateGameSecond();
        }
    }

    public void TestAdvanceGameDay()
    {
        for (int i = 0; i < 86400; i++)
        {
            UpdateGameSecond();
        }
    }

}
