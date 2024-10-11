using UnityEngine;
using Random = UnityEngine.Random;

public class TerrainGeneration : Singleton<TerrainGeneration>
{
    #region Hierarchy

    [Header("Player")]
    [SerializeField] private PlayerController player;

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

    #endregion

    private GameObject[,] worldChunks;

    private GameObject[,] world_ForegroundObjects;
    private GameObject[,] world_BackgroundObjects;
    private TileClass[,] world_BackgroundTiles;
    private TileClass[,] world_ForegroundTiles;

    protected override void Awake()
    {

        base.Awake();

        world_ForegroundTiles = new TileClass[worldSize, worldSize];
        world_BackgroundTiles = new TileClass[worldSize, worldSize];
        world_ForegroundObjects = new GameObject[worldSize, worldSize];
        world_BackgroundObjects = new GameObject[worldSize, worldSize];


        seed = Random.Range(-10000, 10000);
        DrawTextures();

    }

    private void Start()
    {
        // Map 초기화
        CreateChunks();
        GenerateTerrain();
    }

    private void FixedUpdate()
    {
         RefreshChunk();

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
                newChunk.transform.position = new Vector2((i * chunkSize) + chunkSize / 2, (j * chunkSize) + chunkSize / 2);
                worldChunks[i, j] = newChunk;
            }
        }
    }

    void RefreshChunk()
    {

        foreach (Transform child in transform)
        {

            float distance = Vector2.Distance(child.transform.position, player.transform.position);

            if (distance >= chunkSize + 5)
            {
                child.gameObject.SetActive(false);
            }
            else
            {
                child.gameObject.SetActive(true);

            }
        }

    }


    public void GenerateTerrain()
    {

        for (int x = 0; x < worldSize - 1; x++)
        {
            for (int y = 0; y < worldSize; y++)
            {

                if (x < (worldSize / 2) - 10)
                {
                    GenerateTerrainTileClass(x, y, terrainFreq);
                }
                else if (x >= (worldSize / 2) - 10 && x <= (worldSize / 2) + 10)
                {
                    GenerateTerrainTileClass(x, y, 0);
                }
                else if (x > (worldSize / 2) + 10)
                {
                    GenerateTerrainTileClass(x, y, terrainFreq);
                }

            }
        }
    }

    public void GenerateTerrainTileClass(int x, int y, float Freq)
    {
        TileClass tileClass;
        float height;

        // PerlinNoise
        // 그리드의 x축에 대해서 1차원 PerlinNosie
        height = Mathf.PerlinNoise((x + seed) * Freq, seed * Freq) * heightMultiplier + heightAddition;
        

        if (y < height)
        {
            // 계층 별 맵 생성
            if (y < height - xenLayerHeight)
            {
                tileClass = tileAtlas.tech;

                if (ores[3].spreadTexture.GetPixel(x, y).r > 0.5f && height - y > ores[3].maxSpawnHeight)
                {
                    tileClass = tileAtlas.diamond;
                }

            }
            else if (y < height - marsLayerHeight)
            {
                tileClass = tileAtlas.mars;

                if (ores[2].spreadTexture.GetPixel(x, y).r > 0.5f && height - y < ores[2].maxSpawnHeight)
                {
                    tileClass = tileAtlas.gold;
                }

            }
            else if (y < height - dirtLayerHeight)
            {

                tileClass = tileAtlas.stone;

                if (ores[1].spreadTexture.GetPixel(x, y).r > 0.5f && height - y > ores[1].maxSpawnHeight)
                {
                    tileClass = tileAtlas.iron;
                }

            }
            else if (y < height - 1)
            {
                tileClass = tileAtlas.dirt;


                if (ores[0].spreadTexture.GetPixel(x, y).r > 0.5f && height - y < ores[0].maxSpawnHeight)
                {
                    tileClass = tileAtlas.coal;
                }

            }
            else
            {
                // 지상
                tileClass = tileAtlas.grass;
            }


            PlaceTile(tileClass, x, y, true);

            if (y >= height - 1)
            {
                int tree = Random.Range(0, treeChance);
                // 나무가 생성 될 확률 1/treeChance
                if (tree == 1)
                {
                    // 나무 생성
                    if (GetTileFromWorld(x, y))
                    {
                        GenerateTree(Random.Range(minTreeHeight, maxTreeHeight), x, y + 1);

                    }
                }
                else
                {
                    int i = Random.Range(0, tallGrassChance + 1);
                    // 식물 생성
                    if (i == 1)
                    {
                        if ((GetTileFromWorld(x, y)))
                        {
                            if (tileAtlas.tallGrass != null)
                                PlaceTile(tileAtlas.tallGrass, x, y + 1, true);

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
            PlaceTile(tileAtlas.log, x, y + i, true);
        }

        // 잎 생성
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                PlaceTile(tileAtlas.leaf, x + j - 1, y + treeHeight + i, true);

            }
        }
    }


    // 타일 생성
    public void PlaceTile(TileClass tile, int x, int y, bool isNaturallyPlaced)
    {
        if (InWorld(x, y))
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
            newTile.GetComponent<SpriteRenderer>().sprite = tile.tileSprites;
            newTile.name = tile.tileSprites.name;

            if (tile.inBackground)
            {
                newTile.GetComponent<SpriteRenderer>().sortingOrder = -10;

                if (tile.name.ToLower().Contains("wall"))
                {
                    newTile.GetComponent<SpriteRenderer>().color = new Color(0.5f, 0.5f, 0.5f);

                }
            }
            else
            {
                newTile.GetComponent<SpriteRenderer>().sortingOrder = -5;
                newTile.AddComponent<BoxCollider2D>();
                newTile.GetComponent<BoxCollider2D>().size = Vector2.one;
                newTile.layer = 6;
            }

            newTile.tag = $"{newTile.name}";

            newTile.transform.position = new Vector2(x, y);

            TileClass newTileClass = TileClass.CreateInstance(tile, isNaturallyPlaced);

            AddObjectToWorld(x, y, newTile, newTileClass);
            AddTileToWorld(x, y, newTileClass);


        }
    }

    public void RemoveTile(int x, int y)
    {

        if (GetTileFromWorld(x, y) && InWorld(x, y))
        {
            TileClass tile = GetTileFromWorld(x, y);
            if (tile.tileHp <= 0)
            {
                RemoveTileFromWorld(x, y);
            }

            if (tile.wallVariant != null)
            {

                if (tile.naturallyPlaced)
                {
                    PlaceTile(tile.wallVariant, x, y, true);
                }
            }

            DropManager.instance.ItemDrop(x, y, tile.tileName);

            Destroy(GetObjectFromWorld(x, y));
            RemoveObjectFromWorld(x, y);
        }
    }

    public void MiningTile(int x, int y)
    {
        if (GetTileFromWorld(x, y) && InWorld(x, y))
        {
            TileClass tile = GetTileFromWorld(x, y);
            tile.tileHp -= 1;

            if (tile.tileHp <= 0)
            {
                RemoveTile(x, y);
            }
        }
    }

    public bool CheckTile(TileClass tile, int x, int y, bool isNaturallyPlaced)
    {
        if (x >= 0 && x <= worldSize && y >= 0 && y <= worldSize)
        {
            if (tile.inBackground)
            {
                if (GetTileFromWorld(x + 1, y) || GetTileFromWorld(x - 1, y) || GetTileFromWorld(x, y + 1) || GetTileFromWorld(x, y - 1))
                {
                    if (!GetTileFromWorld(x, y))
                    {

                        PlaceTile(tile, x, y, isNaturallyPlaced);

                        return true;
                    }
                    else
                    {
                        if (!GetTileFromWorld(x, y).inBackground)
                        {

                            PlaceTile(tile, x, y, isNaturallyPlaced);

                            return true;
                        }
                    }
                }
            }
            else
            {
                if (GetTileFromWorld(x + 1, y) || GetTileFromWorld(x - 1, y) || GetTileFromWorld(x, y + 1) || GetTileFromWorld(x, y - 1))
                {
                    if (!GetTileFromWorld(x, y))
                    {

                        PlaceTile(tile, x, y, isNaturallyPlaced);

                        return true;
                    }
                    else
                    {
                        if (GetTileFromWorld(x, y).inBackground)
                        {

                            PlaceTile(tile, x, y, isNaturallyPlaced);

                            return true;
                        }
                    }
                }
            }
        }

        return false;
    }


    void AddTileToWorld(int x, int y, TileClass tile)
    {
        if (tile.inBackground)
        {
            world_BackgroundTiles[x, y] = tile;
        }
        else
        {
            world_ForegroundTiles[x, y] = tile;
        }
    }

    void RemoveTileFromWorld(int x, int y)
    {
        if (world_ForegroundTiles[x, y] != null)
        {
            world_ForegroundTiles[x, y] = null;
        }
        else if (world_BackgroundTiles[x, y] != null)
        {
            world_BackgroundTiles[x, y] = null;
        }
    }

    public TileClass GetTileFromWorld(int x, int y)
    {
        if (world_ForegroundTiles[x, y] != null)
        {
            return world_ForegroundTiles[x, y];
        }
        else if (world_BackgroundTiles[x, y] != null)
        {
            return world_BackgroundTiles[x, y];
        }
        return null;
    }

    void AddObjectToWorld(int x, int y, GameObject tileObject, TileClass tile)
    {
        if (tile.inBackground)
        {
            world_BackgroundObjects[x, y] = tileObject;
        }
        else
        {
            world_ForegroundObjects[x, y] = tileObject;
        }
    }

    void RemoveObjectFromWorld(int x, int y)
    {
        if (world_ForegroundObjects[x, y] != null)
        {
            world_ForegroundObjects[x, y] = null;
        }
        else if (world_BackgroundObjects[x, y] != null)
        {
            world_BackgroundObjects[x, y] = null;
        }
    }

    GameObject GetObjectFromWorld(int x, int y)
    {
        if (world_ForegroundObjects[x, y] != null)
        {
            return world_ForegroundObjects[x, y];
        }
        else if (world_BackgroundObjects[x, y] != null)
        {
            return world_BackgroundObjects[x, y];
        }

        return null;
    }

    private bool InWorld(int x, int y)
    {
        return x >= 0 && x < worldSize && y >= 0 && y < worldSize;
    }

}
