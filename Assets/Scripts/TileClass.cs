using UnityEngine;

[CreateAssetMenu(fileName = "newtileclass", menuName = "Tile Class")]
public class TileClass : ScriptableObject
{
    public string tileName;
    public TileClass wallVariant;

    public Sprite[] tileSprites;
    public bool inBackground = false;
    public Sprite tileDrop;
    public bool naturallyPlaced = true;

    public static TileClass CreateInstance(TileClass tile, bool isNaturallyPlaced)
    {
        var thisTile = ScriptableObject.CreateInstance<TileClass>();

        thisTile.Init(tile, isNaturallyPlaced);
        
        return thisTile;

    }
    public void Init(TileClass tile, bool isNaturallyPlaced)
    {
        var thisTile = ScriptableObject.CreateInstance<TileClass>();

        tileName = tile.tileName;
        tileSprites = tile.tileSprites;
        inBackground = tile.inBackground;
        wallVariant = tile.wallVariant;
        tileDrop = tile.tileDrop;
        naturallyPlaced = isNaturallyPlaced;


    }
}
