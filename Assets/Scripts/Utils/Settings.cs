using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Settings
{
    // Player
    public static float moveSpeed = 5f;
    public static float jumpHeight = 2.5f;
    public static int playerRange = 3;


    // Inventory
    public static int playerInitalInventoryCapacity = 48;
    public static int playerMaximumInventoryCapacity = 48;

    // Item
    public const string Equipment = "장비";
    public const string Food = "음식";
    public const string Block = "지형";
    public const string Ingredient = "재료";
    public const string Furniture = "가구";

    // Time System (숫자가 클수록 느려짐)
    public const float secondsPerGameSecond = 0.004f;


}
