using System.Collections.Generic;
using UnityEngine;

using Random = UnityEngine.Random;

public class TerrainGeneration : MonoBehaviour
{

    [Header("Tile Atlas")]
    public TileAtlas tileAtlas;

    [Header("Tree")]
    public int treeChance = 10;
    public int minTreeHeight = 3;
    public int maxTreeHeight = 6;

    [Header("Addons")]
    public int tallGrassChance = 10;

    [Header("Generation Settings")]
    // worldSize는 chunkSize로 나누어 떨어져야한다.
    public int chunkSize = 20;
    public int worldSize = 100;
    public float surfaceValue = 0.3f;
    public int dirtLayerHeight = 5;
    public float heightMultiplier = 25f;
    public int heightAddition = 25;
    public bool generateCaves = false;

    [Header("Noise Settings")]
    public float seed;
    public float terrainFreq = 0.04f;
    public float caveFreq = 0.08f;
    public Texture2D caveNoiseTexture;

    [Header("Ore Settings")]
    public OreClass[] ores;

    private GameObject[] worldChunks;
    private List<Vector2> worldTiles = new List<Vector2>();

    private void OnValidate()
    {

        caveNoiseTexture = new Texture2D(worldSize, worldSize);
        ores[0].spreadTexture = new Texture2D(worldSize, worldSize);
        ores[1].spreadTexture = new Texture2D(worldSize, worldSize);
        ores[2].spreadTexture = new Texture2D(worldSize, worldSize);
        ores[3].spreadTexture = new Texture2D(worldSize, worldSize);

        GenerateNoiseTexture(caveFreq, surfaceValue, caveNoiseTexture);

        GenerateNoiseTexture(ores[0].rarity, ores[0].size, ores[0].spreadTexture);
        GenerateNoiseTexture(ores[1].rarity, ores[1].size, ores[1].spreadTexture);
        GenerateNoiseTexture(ores[2].rarity, ores[2].size, ores[2].spreadTexture);
        GenerateNoiseTexture(ores[3].rarity, ores[3].size, ores[3].spreadTexture);
    }


    private void Start()
    {
        seed = Random.Range(-10000, 10000);


        caveNoiseTexture = new Texture2D(worldSize, worldSize);
        ores[0].spreadTexture = new Texture2D(worldSize, worldSize);
        ores[1].spreadTexture = new Texture2D(worldSize, worldSize);
        ores[2].spreadTexture = new Texture2D(worldSize, worldSize);
        ores[3].spreadTexture = new Texture2D(worldSize, worldSize);
        GenerateNoiseTexture(caveFreq, surfaceValue, caveNoiseTexture);

        GenerateNoiseTexture(ores[0].rarity, ores[0].size, ores[0].spreadTexture);
        GenerateNoiseTexture(ores[1].rarity, ores[1].size, ores[1].spreadTexture);
        GenerateNoiseTexture(ores[2].rarity, ores[2].size, ores[2].spreadTexture);
        GenerateNoiseTexture(ores[3].rarity, ores[3].size, ores[3].spreadTexture);

        CreateChunks();
        GenerateTerrain();
    }

    public void CreateChunks()
    {
        int numChunks = worldSize / chunkSize;
        worldChunks = new GameObject[numChunks];

        for (int i = 0; i < numChunks; i++)
        {
            GameObject newChunk = new GameObject();
            newChunk.name = i.ToString();
            newChunk.transform.parent = transform;
            worldChunks[i] = newChunk;
        }

    }

    public void GenerateTerrain()
    {
        for (int x = 0; x < worldSize; x++)
        {
            float height = Mathf.PerlinNoise((x + seed) * terrainFreq, seed * terrainFreq) * heightMultiplier + heightAddition;

            for (int y = 0; y < height; y++)
            {
                Sprite[] tileSprites;

                // 높이 별 타일 생성
                if (y < height - dirtLayerHeight)
                {
                    tileSprites = tileAtlas.stone.tileSprites;

                    if (ores[0].spreadTexture.GetPixel(x, y).r > 0.5f && height - y > ores[0].maxSpawnHeight)
                    {
                        tileSprites = tileAtlas.coal.tileSprites;
                    }
                    if (ores[1].spreadTexture.GetPixel(x, y).r > 0.5f && height - y > ores[1].maxSpawnHeight)
                    {
                        tileSprites = tileAtlas.iron.tileSprites;
                    }
                    if (ores[2].spreadTexture.GetPixel(x, y).r > 0.5f && height - y > ores[2].maxSpawnHeight)
                    {
                        tileSprites = tileAtlas.gold.tileSprites;
                    }
                    if (ores[3].spreadTexture.GetPixel(x, y).r > 0.5f && height - y > ores[3].maxSpawnHeight)
                    {
                        tileSprites = tileAtlas.diamond.tileSprites;
                    }

                }
                else if (y < height - 1)
                {
                    tileSprites = tileAtlas.dirt.tileSprites;
                }
                else
                {
                    // 지상
                    tileSprites = tileAtlas.grass.tileSprites;

                }

                if (generateCaves)
                {
                    if (caveNoiseTexture.GetPixel(x, y).r > 0.5f)
                    {
                        PlaceTile(tileSprites, x, y);

                    }
                }
                else
                {
                    PlaceTile(tileSprites, x, y);
                }

                if (y >= height - 1)
                {
                    int tree = Random.Range(0, treeChance);
                    // 나무가 생성 될 확률 1/treeChance
                    if (tree == 1)
                    {
                        // 나무 생성
                        if (worldTiles.Contains(new Vector2(x, y)))
                        {
                            GenerateTree(x, y + 1);

                        }
                    }
                    else
                    {
                        int i = Random.Range(0, tallGrassChance);
                        // 식물 생성
                        if(i == 1)
                        {
                            if (worldTiles.Contains(new Vector2(x, y)))
                            {
                                PlaceTile(tileAtlas.tallGrass.tileSprites, x, y + 1);

                            }
                        }
                        
                    }
                }


            }
        }
    }


    public void GenerateNoiseTexture(float frequeny, float limit, Texture2D noiseTexture)
    {


        for (int x = 0; x < noiseTexture.width; x++)
        {
            for (int y = 0; y < noiseTexture.height; y++)
            {
                float v = Mathf.PerlinNoise((x + seed) * frequeny, (y + seed) * frequeny);
                if (v > limit)
                    noiseTexture.SetPixel(x, y, Color.white);
                else
                    noiseTexture.SetPixel(x, y, Color.black);

            }
        }

        noiseTexture.Apply();
    }

    // 나무 생성
    void GenerateTree(int x, int y)
    {
        // 높이 랜덤
        int treeHeight = Random.Range(minTreeHeight, maxTreeHeight);

        // 나무 기둥 생성
        for (int i = 0; i < treeHeight; i++)
        {
            PlaceTile(tileAtlas.log.tileSprites, x, y + i);

        }

        // 잎 생성
        PlaceTile(tileAtlas.leaf.tileSprites, x, y + treeHeight);
        PlaceTile(tileAtlas.leaf.tileSprites, x, y + treeHeight + 1);
        PlaceTile(tileAtlas.leaf.tileSprites, x, y + treeHeight + 2);

        PlaceTile(tileAtlas.leaf.tileSprites, x - 1, y + treeHeight);
        PlaceTile(tileAtlas.leaf.tileSprites, x - 1, y + treeHeight + 1);

        PlaceTile(tileAtlas.leaf.tileSprites, x + 1, y + treeHeight);
        PlaceTile(tileAtlas.leaf.tileSprites, x + 1, y + treeHeight + 1);

    }


    public void PlaceTile(Sprite[] tileSprites, int x, int y)
    {
        GameObject newTile = new GameObject();

        float chunkCoord = Mathf.Round(x / chunkSize) * chunkSize;
        chunkCoord /= chunkSize;

        newTile.transform.parent = worldChunks[(int)chunkCoord].transform;


        newTile.AddComponent<SpriteRenderer>();

        int spriteIndex = Random.Range(0, tileSprites.Length);
        newTile.GetComponent<SpriteRenderer>().sprite = tileSprites[spriteIndex];


        newTile.name = tileSprites[0].name;
        newTile.transform.position = new Vector2(x + 0.5f, y + 0.5f);

        worldTiles.Add(newTile.transform.position - (Vector3.one * 0.5f));
    }

}
