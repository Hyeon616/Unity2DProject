using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Settings
{
    // Player
    public static float moveSpeed = 2.5f;
    public static float jumpHeight = 2.5f;
    public static int playerRange = 3;


    // Inventory
    public static int playerInitalInventoryCapacity = 53;
    public static int playerMaximumInventoryCapacity = 53;

    // Item
    public const string Equipment = "장비";
    public const string Food = "음식";
    public const string Block = "지형";
    public const string Ingredient = "재료";
    public const string Furniture = "가구";

    // Time System (숫자가 클수록 느려짐)
    public const float secondsPerGameSecond = 0.004f;

    // BackgroundColor
    public const float backgroundAlphaAt22to3 = 0.9f;
    public const float backgroundAlphaAt3to5And20to22 = 0.8f;
    public const float backgroundAlphaAt5to7And18to20 = 0.6f;
    public const float backgroundAlphaAt7to9And16to18 = 0.4f;
    public const float backgroundAlphaAt9to11And14to16 = 0.2f;
    public const float backgroundAlphaAt11to14 = 0f;

}
