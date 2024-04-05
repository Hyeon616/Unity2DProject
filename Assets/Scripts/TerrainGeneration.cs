using System.Collections.Generic;
using UnityEngine;

using Random = UnityEngine.Random;

public class TerrainGeneration : MonoBehaviour
{
    [Header("Tile Atlas")]
    public TileAtlas tileAtlas;
    public float seed;

    public BiomeClass[] biomes;


    [Header("Biomes")]
    public float biomeFrequency;
    public Gradient biomeGradient;
    public Texture2D biomeMap;


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
    public int heightAddition = 40;

    public float surfaceValue = 0.3f;
    public float heightMultiplier = 25f;
    public bool generateCaves = false;

    public int dirtLayerHeight = 8;
    public int stoneLayerHeight = 16;
    public int marsLayerHeight = 24;
    public int xenLayerHeight = 32;

    [Header("Noise Settings")]

    public float terrainFreq = 0.04f;
    public float caveFreq = 0.08f;
    public Texture2D caveNoiseTexture;

    [Header("Ore Settings")]
    public OreClass[] ores;

    private GameObject[] worldChunks;
    private List<Vector2> worldTiles = new List<Vector2>();
    public Color[] biomeColors;
    private BiomeClass curBiome;


    private void OnValidate()
    {

        DrawTextures();
    }


    private void Start()
    {
        seed = Random.Range(-10000, 10000);

        DrawTextures();
        CreateChunks();
        GenerateTerrain();
    }

    public void DrawTextures()
    {
        biomeMap = new Texture2D(worldSize, worldSize);
        DrawBiomeTexture();

        for (int i = 0; i < biomes.Length; i++)
        {

            biomes[i].caveNoiseTexture = new Texture2D(worldSize, worldSize);

            for (int j = 0; j < biomes[i].ores.Length; j++)
            {
                biomes[i].ores[j].spreadTexture = new Texture2D(worldSize, worldSize);

            }

            GenerateNoiseTexture(biomes[i].caveFreq, biomes[i].surfaceValue, biomes[i].caveNoiseTexture);

            for (int j = 0; j < biomes[i].ores.Length; j++)
            {

                GenerateNoiseTexture(biomes[i].ores[j].rarity, biomes[i].ores[j].size, biomes[i].ores[j].spreadTexture);
            }

        }

    }

    public void DrawBiomeTexture()
    {
        Texture2D tempTexture = new Texture2D(worldSize, worldSize);

        for (int x = 0; x < biomeMap.width; x++)
        {
            for (int y = 0; y < biomeMap.height; y++)
            {
                float v = Mathf.PerlinNoise((x + seed) * biomeFrequency, (y + seed) * biomeFrequency);

                Color color = biomeGradient.Evaluate(v);
                biomeMap.SetPixel(x, y, color);


            }
        }
        biomeMap.Apply();
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

    public BiomeClass GetCurrentBiome(int x, int y)
    {
        // 바이옴 탐색

        for (int i = 0; i < biomes.Length; i++)
        {
            if (biomes[i].biomeColor == biomeMap.GetPixel(x, y))
            {
                return biomes[i];
            }

        }

        return curBiome;
    }


    public void GenerateTerrain()
    {
        Sprite[] tileSprites;
        for (int x = 0; x < worldSize; x++)
        {
            curBiome = GetCurrentBiome(x, 0);
            float height = Mathf.PerlinNoise((x + seed) * curBiome.terrainFreq, seed * curBiome.terrainFreq) * curBiome.heightMultiplier + heightAddition;

            for (int y = 0; y < height; y++)
            {
                // 계층 별 맵 생성
                if (y < height - xenLayerHeight)
                {
                    tileSprites = curBiome.tileAtlas.tech.tileSprites;
                    if (ores[3].spreadTexture.GetPixel(x, y).r > 0.5f && height - y > ores[3].maxSpawnHeight)
                    {
                        tileSprites = tileAtlas.diamond.tileSprites;
                    }
                }
                else if (y < height - marsLayerHeight)
                {
                    tileSprites = curBiome.tileAtlas.mars.tileSprites;

                    if (ores[2].spreadTexture.GetPixel(x, y).r > 0.5f && height - y < ores[2].maxSpawnHeight)
                    {
                        tileSprites = tileAtlas.gold.tileSprites;
                    }
                }
                else if (y < height - dirtLayerHeight)
                {

                    tileSprites = curBiome.tileAtlas.stone.tileSprites;

                    if (ores[1].spreadTexture.GetPixel(x, y).r > 0.5f && height - y > ores[1].maxSpawnHeight)
                    {
                        tileSprites = tileAtlas.iron.tileSprites;
                    }

                }
                else if (y < height - 1)
                {
                    tileSprites = curBiome.tileAtlas.dirt.tileSprites;


                    if (ores[0].spreadTexture.GetPixel(x, y).r > 0.5f && height - y < ores[0].maxSpawnHeight)
                    {
                        tileSprites = tileAtlas.coal.tileSprites;
                    }
                }
                else
                {
                    // 지상
                    tileSprites = curBiome.tileAtlas.grass.tileSprites;


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
                    int tree = Random.Range(0, curBiome.treeChance);
                    // 나무가 생성 될 확률 1/treeChance
                    if (tree == 1)
                    {
                        // 나무 생성
                        if (worldTiles.Contains(new Vector2(x, y)) && x > 1 && x < worldSize - 1)
                        {
                            GenerateTree(Random.Range(curBiome.minTreeHeight, curBiome.maxTreeHeight), x, y + 1);

                        }
                    }
                    else
                    {
                        int i = Random.Range(0, curBiome.tallGrassChance);
                        // 식물 생성
                        if (i == 1)
                        {
                            if (worldTiles.Contains(new Vector2(x, y)))
                            {
                                if (curBiome.tileAtlas.tallGrass != null)
                                    PlaceTile(curBiome.tileAtlas.tallGrass.tileSprites, x, y);

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
        // 높이 랜덤


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
        if (!worldTiles.Contains(new Vector2Int(x, y)))
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

}
