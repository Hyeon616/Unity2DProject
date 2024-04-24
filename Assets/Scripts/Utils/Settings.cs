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
    public const string Equipment = "���";
    public const string Food = "����";
    public const string Block = "����";
    public const string Ingredient = "���";
    public const string Furniture = "����";

    // Time System (���ڰ� Ŭ���� ������)
    public const float secondsPerGameSecond = 0.004f;


}
