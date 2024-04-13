using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class ItemClass
{
    public enum ItemType
    {
        NULL,
        BLOCK,
        TOOL
    
    }

    public enum ToolType
    {
        NULL,
        AXE,
        PICKAXE,
        HAMMER,
        

    }

    

    public ItemType itemType;
    public ToolType toolType;

    public TileClass tile;
    public ToolClass tool;

    public string itemName;
    public Sprite sprite;
    public bool isStackable;

    public ItemClass(TileClass _tile)
    {
        itemName = _tile.tileName;
        sprite = _tile.tileDrop.tileSprites[0];
        isStackable = _tile.isStackable;
        itemType = ItemType.BLOCK;
        toolType = ToolType.NULL;
        tile = _tile;
    }

    public ItemClass(ToolClass _tool)
    {
        itemName= _tool.name;
        sprite = _tool.sprite;
        isStackable = false;
        itemType = ItemType.TOOL;
        toolType = _tool.toolType;
        tool = _tool;
    }

}
