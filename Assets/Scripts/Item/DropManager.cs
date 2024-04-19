using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropManager : Singleton<DropManager>
{
    public static DropManager instance;


    [SerializeField] public string[] dropItemKeys;
    [SerializeField] public GameObject[] dropItemValues;
    public Dictionary<string, GameObject> dropItems;

    [SerializeField] public string[] placeBlockKeys;
    [SerializeField] public TileClass[] placeBlockValues;
    public Dictionary<string, TileClass> placeBlocks;

    private TerrainGeneration terrainGeneration;

    protected override void Awake()
    {
        base.Awake();

        instance = this;
        dropItems = new Dictionary<string, GameObject>();
        placeBlocks = new Dictionary<string, TileClass>();

        terrainGeneration = GetComponent<TerrainGeneration>();

        for (int i = 0; i < dropItemKeys.Length; i++)
        {
            dropItems.Add(dropItemKeys[i], dropItemValues[i]);
        }

        for (int i = 0; i < placeBlockKeys.Length; i++)
        {
            placeBlocks.Add(placeBlockKeys[i], placeBlockValues[i]);
        }
    }

    public void ItemDrop(int x, int y ,string _itemName)
    {
        if (dropItems.ContainsKey(_itemName))
        {
            
            GameObject itemDrop = dropItems[_itemName];
            itemDrop.layer = 10;
            itemDrop.transform.localScale = new Vector2(0.5f, 0.5f);
            
            Instantiate(itemDrop, new Vector2(x, y + 0.5f), Quaternion.identity);
        }
    }

    public void PlaceBlock(int x, int y, string _itemName)
    {
        if (placeBlocks.ContainsKey(_itemName))
        {

            //terrainGeneration.CheckTile(placeBlocks[_itemName].);

            //Instantiate(placeBlock, new Vector2(x, y), Quaternion.identity);
        }
    }


}
