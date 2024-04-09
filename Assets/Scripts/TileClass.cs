using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newtileclass", menuName = "Tile Class")]
public class TileClass : ScriptableObject
{
    public string tileName;

    public TileClass wallVariant;
    public Sprite[] tileSprites;
    public bool inBackground = false;

    public bool isDropItem = true;

}
