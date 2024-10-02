using System;
using System.Collections.Generic;

public static class EventHandler
{
    // �κ��丮 �̺�Ʈ
    // �κ��丮 ������Ʈ
    //public static event Action<InventoryLocation, Dictionary<int, InventoryItem>> InventoryUpdatedDictEvent;

    //public static void CallInventoryUpdatedDictEvent(InventoryLocation inventoryLocation, Dictionary<int, InventoryItem> inventoryDict)
    //{
    //    if (InventoryUpdatedDictEvent != null)
    //        InventoryUpdatedDictEvent(inventoryLocation, inventoryDict);

    //}


    // �ð� �̺�Ʈ
    // ���� ��
    public static event Action<int, Season, int, string, int, int, int> AdvanceGameMinuteEvent;

    public static void CallAdvanceGameMinuteEvent(int gameYear, Season gameSeason, int gameDay, string gameDayOfWeek, int gameHour, int gameMinute, int gameSecond)
    {
        if (AdvanceGameMinuteEvent != null)
            AdvanceGameMinuteEvent(gameYear, gameSeason, gameDay, gameDayOfWeek, gameHour, gameMinute, gameSecond);

    }

    // ���� �ð�
    public static event Action<int, Season, int, string, int, int, int> AdvanceGameHourEvent;

    public static void CallAdvanceGameHourEvent(int gameYear, Season gameSeason, int gameDay, string gameDayOfWeek, int gameHour, int gameMinute, int gameSecond)
    {
        if (AdvanceGameHourEvent != null)
            AdvanceGameHourEvent(gameYear, gameSeason, gameDay, gameDayOfWeek, gameHour, gameMinute, gameSecond);

    }

    // ���� ��
    public static event Action<int, Season, int, string, int, int, int> AdvanceGameDayEvent;

    public static void CallAdvanceGameDayEvent(int gameYear, Season gameSeason, int gameDay, string gameDayOfWeek, int gameHour, int gameMinute, int gameSecond)
    {
        if (AdvanceGameDayEvent != null)
            AdvanceGameDayEvent(gameYear, gameSeason, gameDay, gameDayOfWeek, gameHour, gameMinute, gameSecond);

    }

    // ���� ����
    public static event Action<int, Season, int, string, int, int, int> AdvanceGameSeasonEvent;

    public static void CallAdvanceGameSeasonEvent(int gameYear, Season gameSeason, int gameDay, string gameDayOfWeek, int gameHour, int gameMinute, int gameSecond)
    {
        if (AdvanceGameSeasonEvent != null)
            AdvanceGameSeasonEvent(gameYear, gameSeason, gameDay, gameDayOfWeek, gameHour, gameMinute, gameSecond);

    }

    // ���� ��
    public static event Action<int, Season, int, string, int, int, int> AdvanceGameYearEvent;

    public static void CallAdvanceGameYearEvent(int gameYear, Season gameSeason, int gameDay, string gameDayOfWeek, int gameHour, int gameMinute, int gameSecond)
    {
        if (AdvanceGameYearEvent != null)
            AdvanceGameYearEvent(gameYear, gameSeason, gameDay, gameDayOfWeek, gameHour, gameMinute, gameSecond);

    }


    // Scene �ε� �̺�Ʈ
    // scene unload fade out
    public static event Action BeforeSceneUnloadFadeOutEvent;

    public static void CallBeforeSceneUnloadFadeOutEvent()
    {
        if(BeforeSceneUnloadFadeOutEvent != null)
        {
            BeforeSceneUnloadFadeOutEvent();
        }
    }

    // scene unload
    public static event Action BeforeSceneUnloadEvent;

    public static void CallBeforeSceneUnloadEvent()
    {
        if (BeforeSceneUnloadEvent != null)
        {
            BeforeSceneUnloadEvent();
        }
    }
    // scene load
    public static event Action AfterSceneLoadEvent;

    public static void CallAfterSceneLoadEvent()
    {
        if (AfterSceneLoadEvent != null)
        {
            AfterSceneLoadEvent();
        }

    }

    // scene load fade in
    public static event Action AfterSceneLoadFadeInEvent;

    public static void CallAfterSceneLoadFadeInEvent()
    {
        if (AfterSceneLoadFadeInEvent != null)
        {
            AfterSceneLoadFadeInEvent();
        }

    }

}
