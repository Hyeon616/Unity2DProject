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
    public static int playerInitalInventoryCapacity = 24;
    public static int playerMaximumInventoryCapacity = 48;

    // Item
    public const string ChoppingTool = "Axe";
    public const string Sword = "Sword";
    public const string Bow = "Bow";
    public const string RepairingTool = "Hammer";
    public const string BreakingTool = "Pickaxe";

    // Time System (숫자가 클수록 느려짐)
    public const float secondsPerGameSecond = 0.004f;


}
