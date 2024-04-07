using System.Collections.Generic;
using UnityEngine;

using Random = UnityEngine.Random;

public class TerrainGeneration : MonoBehaviour
{

    [Header("User")]
    public PlayerController player;


    [Header("Tile Atlas")]
    public TileAtlas tileAtlas;
    public float seed;

    [Header("Tree")]
    public int treeChance = 10;
    public int minTreeHeight = 3;
    public int maxTreeHeight = 6;

    [Header("Addons")]
    public int tallGrassChance = 10;

    [Header("Generation Settings")]
    
    public float surfaceValue = 0.3f;
    public float heightMultiplier = 25f;

    public int dirtLayerHeight = 8;
    public int stoneLayerHeight = 16;
    public int marsLayerHeight = 24;
    public int xenLayerHeight = 32;

    // worldSize는 chunkSize로 나누어 떨어져야한다.
    public int chunkSize = 20;
    public int worldSize = 100;

    public int heightAddition = 40;



    [Header("Noise Settings")]
    public float terrainFreq = 0.04f;

    [Header("Ore Settings")]
    public OreClass[] ores;

    private GameObject[,] worldChunks;
    private List<Vector2> worldTiles = new List<Vector2>();


    private void OnValidate()
    {
        DrawTextures();
    }

    private void Awake()
    {
        seed = Random.Range(-10000, 10000);
        DrawTextures();

    }

    private void Start()
    {
        CreateChunks();
        GenerateTerrain();
        player.Spawn();
    }

    public void DrawTextures()
    {

        for (int i = 0; i < ores.Length; i++)
        {
            ores[i].spreadTexture = new Texture2D(worldSize, worldSize);

        }

        for (int i = 0; i < ores.Length; i++)
        {

            GenerateNoiseTexture(ores[i].rarity, ores[i].size, ores[i].spreadTexture);
        }

    }

    public void CreateChunks()
    {
        int numChunksX = worldSize / chunkSize;
        int numChunksY = worldSize / xenLayerHeight;

        worldChunks = new GameObject[numChunksX, numChunksY];

        for (int i = 0; i < numChunksX; i++)
        {
            for (int j = 0; j < numChunksY; j++)
            {
                GameObject newChunk = new GameObject();
                newChunk.name = $"X{i}Y{j}";
                newChunk.transform.parent = transform;
                worldChunks[i, j] = newChunk;
            }

        }

    }

    public void GenerateTerrain()
    {
        Sprite[] tileSprites;
        for (int x = 0; x < worldSize; x++)
        {

            float height;

            for (int y = 0; y < worldSize; y++)
            {
                // PerlinNoise
                height = Mathf.PerlinNoise((x + seed) * terrainFreq, seed * terrainFreq) * heightMultiplier + heightAddition;

                // 플레이어의 시작 위치
                if (x == worldSize / 2)
                {
                    player.spawnPos = new Vector2(x, height + 2);
                }

                if (y < height)
                {
                    // 계층 별 맵 생성
                    if (y < height - xenLayerHeight)
                    {
                        tileSprites = tileAtlas.tech.tileSprites;
                        if (ores[3].spreadTexture.GetPixel(x, y).r > 0.5f && height - y > ores[3].maxSpawnHeight)
                        {
                            tileSprites = tileAtlas.diamond.tileSprites;
                        }
                    }
                    else if (y < height - marsLayerHeight)
                    {
                        tileSprites = tileAtlas.mars.tileSprites;

                        if (ores[2].spreadTexture.GetPixel(x, y).r > 0.5f && height - y < ores[2].maxSpawnHeight)
                        {
                            tileSprites = tileAtlas.gold.tileSprites;
                        }
                    }
                    else if (y < height - dirtLayerHeight)
                    {

                        tileSprites = tileAtlas.stone.tileSprites;

                        if (ores[1].spreadTexture.GetPixel(x, y).r > 0.5f && height - y > ores[1].maxSpawnHeight)
                        {
                            tileSprites = tileAtlas.iron.tileSprites;
                        }

                    }
                    else if (y < height - 1)
                    {
                        tileSprites = tileAtlas.dirt.tileSprites;


                        if (ores[0].spreadTexture.GetPixel(x, y).r > 0.5f && height - y < ores[0].maxSpawnHeight)
                        {
                            tileSprites = tileAtlas.coal.tileSprites;
                        }
                    }
                    else
                    {
                        // 지상
                        tileSprites = tileAtlas.grass.tileSprites;


                    }

                    PlaceTile(tileSprites, x, y, false);

                    if (y >= height - 1)
                    {
                        int tree = Random.Range(0, treeChance);
                        // 나무가 생성 될 확률 1/treeChance
                        if (tree == 1)
                        {
                            // 나무 생성
                            if (worldTiles.Contains(new Vector2(x, y)))
                            {
                                GenerateTree(Random.Range(minTreeHeight, maxTreeHeight), x, y + 1);

                            }
                        }
                        else
                        {
                            int i = Random.Range(0, tallGrassChance);
                            // 식물 생성
                            if (i == 1)
                            {
                                if (worldTiles.Contains(new Vector2(x, y)))
                                {
                                    if (tileAtlas.tallGrass != null)
                                        PlaceTile(tileAtlas.tallGrass.tileSprites, x, y, true);

                                }
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
    void GenerateTree(int treeHeight, int x, int y)
    {

        // 나무 기둥 생성
        for (int i = 0; i < treeHeight; i++)
        {
            PlaceTile(tileAtlas.log.tileSprites, x, y + i, true);
        }

        // 잎 생성
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                PlaceTile(tileAtlas.leaf.tileSprites, x + j - 1, y + treeHeight + i, true);

            }
        }
    }

    // 타일 생성
    public void PlaceTile(Sprite[] tileSprites, int x, int y, bool backGroundObject)
    {
        if (!worldTiles.Contains(new Vector2Int(x, y)) && InWorld(x, y))
        {
            GameObject newTile = new GameObject();

            int chunkCoordX = Mathf.RoundToInt(x / chunkSize) * chunkSize;
            int chunkCoordY = Mathf.RoundToInt(y / chunkSize) * chunkSize;

            

            chunkCoordX /= chunkSize;
            chunkCoordY /= chunkSize;

            chunkCoordX = Mathf.Clamp(chunkCoordX, 0, worldChunks.GetLength(0) - 1);
            chunkCoordY = Mathf.Clamp(chunkCoordY, 0, worldChunks.GetLength(1) - 1);

            newTile.transform.parent = worldChunks[chunkCoordX, chunkCoordY].transform;

            newTile.AddComponent<SpriteRenderer>();

            if (!backGroundObject)
            {
                newTile.AddComponent<BoxCollider2D>();
                newTile.GetComponent<BoxCollider2D>().size = Vector2.one;
                newTile.tag = "Ground";
            }



            int spriteIndex = Random.Range(0, tileSprites.Length);
            newTile.GetComponent<SpriteRenderer>().sprite = tileSprites[spriteIndex];

            newTile.name = tileSprites[0].name;
            newTile.transform.position = new Vector2(x + 0.5f, y + 0.5f);

            worldTiles.Add(newTile.transform.position - (Vector3.one * 0.5f));
        }

    }
    private bool InWorld(int x, int y)
    {
        return x >= 0 && x <= worldSize && y >= 0 && y <= worldSize;
    }


}
