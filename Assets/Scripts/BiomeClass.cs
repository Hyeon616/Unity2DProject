using UnityEngine;

[System.Serializable]
public class BiomeClass
{
    public string biomeName;
    public Color biomeColor;

    public TileAtlas tileAtlas;

    [Header("Noise Settings")]
    public float terrainFreq = 0.04f;
    public float caveFreq = 0.08f;
    public Texture2D caveNoiseTexture;

    [Header("Generation Settings")]
    public float surfaceValue = 0.3f;
    public float heightMultiplier = 25f;
    public bool generateCaves = false;

    public int dirtLayerHeight = 8;
    public int stoneLayerHeight = 16;
    public int marsLayerHeight = 24;
    public int xenLayerHeight = 32;

    [Header("Tree")]
    public int treeChance = 10;
    public int minTreeHeight = 3;
    public int maxTreeHeight = 6;

    [Header("Addons")]
    public int tallGrassChance = 10;

    [Header("Ore Settings")]
    public OreClass[] ores;

}
