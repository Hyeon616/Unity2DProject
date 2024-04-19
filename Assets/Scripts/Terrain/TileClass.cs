using UnityEngine;

[CreateAssetMenu(fileName = "newtileclass", menuName = "Scriptable Objects/Tile/Tile Class")]
public class TileClass : ScriptableObject
{
    public TileClass wallVariant;
    public Sprite tileSprites;
    public TileClass tileDrop;
    public string tileName;
    public int tileHp;
    public bool inBackground = false;
    public bool naturallyPlaced = true;
    public bool isStackable = true;

    public static TileClass CreateInstance(TileClass tile, bool isNaturallyPlaced)
    {
        var thisTile = CreateInstance<TileClass>();

        thisTile.Init(tile, isNaturallyPlaced);
        
        return thisTile;

    }
    public void Init(TileClass tile, bool isNaturallyPlaced)
    {
        var thisTile = CreateInstance<TileClass>();

        tileName = tile.tileName;
        tileHp = tile.tileHp;
        tileSprites = tile.tileSprites;
        inBackground = tile.inBackground;
        isStackable = tile.isStackable;
        wallVariant = tile.wallVariant;
        tileDrop = tile.tileDrop;
        naturallyPlaced = isNaturallyPlaced;

    }
}
