using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Recipe
{
    public List<string> composition;
    public int result;          
    public int count;           
}


public class UI_Mixing : MonoBehaviour
{
    private InventoryManager inventoryManager; // 인벤토리 관리자 참조
    public Button craftButton;                // 조합 버튼 참조
    public Image resultImage;

    private Dictionary<string, Recipe> craftingRecipes; // 조합 레시피 저장 dictionary

    /// <summary>
    /// 초기화 메서드
    /// </summary>
    void Start()
    {
        // InventoryManager 찾기
        if (inventoryManager == null)
        {
            inventoryManager = FindObjectOfType<InventoryManager>();
            if (inventoryManager == null)
            {
                Debug.LogError("InventoryManager not found!");
                return;
            }
        }

        // 조합 버튼에 리스너 추가
        if (craftButton != null)
        {
            craftButton.onClick.AddListener(AttemptCrafting);
        }
        else
        {
            Debug.LogError("Craft button not assigned!");
        }

        // 레시피 로드
        LoadRecipes();
    }

    /// <summary>
    /// JSON 파일에서 레시피 로드
    /// </summary>
    void LoadRecipes()
    {
        TextAsset recipesJson = Resources.Load<TextAsset>("Data/recipes");
        if (recipesJson != null)
        {
            var recipeData = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, Recipe>>>(recipesJson.text);
            craftingRecipes = recipeData["recipes"];
        }
        else
        {
            Debug.LogError("Recipes JSON file not found!");
        }
    }

    /// <summary>
    /// 조합 시도 메서드
    /// </summary>
    public void AttemptCrafting()
    {
        int[,] currentGrid = GetCurrentCraftingGrid();
        List<int[,]> gridVariations = GetUniqueVariations(currentGrid);

        foreach (var recipe in craftingRecipes.Values)
        {
            if (MatchesRecipe(gridVariations, recipe))
            {
                CraftItem(recipe.result, recipe.count);
                return;
            }
        }

        Debug.Log("No matching recipe found.");
    }

    /// <summary>
    /// 현재 조합 그리드 상태를 가져오는 메서드
    /// </summary>
    private int[,] GetCurrentCraftingGrid()
    {
        int[,] grid = new int[3, 3];
        for (int i = 0; i < 9; i++)
        {
            int row = i / 3;
            int col = i % 3;
            if (inventoryManager.mixSlots.TryGetValue(i, out InventorySlot slot) && !slot.IsEmpty)
            {
                grid[row, col] = slot.item.data.id;
            }
            else
            {
                grid[row, col] = 0; // 빈 슬롯
            }
        }
        return grid;
    }

    /// <summary>
    /// 그리드 변형이 레시피와 일치하는지 확인하는 메서드
    /// </summary>
    private bool MatchesRecipe(List<int[,]> gridVariations, Recipe recipe)
    {
        int[,] recipeGrid = ConvertPatternToGrid(recipe.composition);
        return gridVariations.Any(variation => GridsEqual(variation, recipeGrid));
    }

    /// <summary>
    /// 문자열 패턴을 정수 그리드로 변환하는 메서드
    /// </summary>
    private int[,] ConvertPatternToGrid(List<string> pattern)
    {
        int[,] grid = new int[3, 3];
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                grid[i, j] = int.Parse(pattern[i][j].ToString());
            }
        }
        return grid;
    }

    /// <summary>
    /// 두 그리드가 동일한지 확인하는 메서드
    /// </summary>
    private static bool GridsEqual(int[,] grid1, int[,] grid2)
    {
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                if (grid1[i, j] != grid2[i, j])
                {
                    return false;
                }
            }
        }
        return true;
    }

    /// <summary>
    /// 아이템을 조합하는 메서드
    /// </summary>
    private void CraftItem(int resultItemId, int count)
    {
        if (inventoryManager.AddItem(resultItemId, count))
        {
            Debug.Log($"Successfully crafted {count} of item with ID: {resultItemId}");

            // Mix 슬롯 비우기
            for (int i = 0; i < 9; i++)
            {
                if (inventoryManager.mixSlots.TryGetValue(i, out InventorySlot slot))
                {
                    slot.Clear();
                }
            }

            // UI 업데이트
            inventoryManager.UpdateCrafting();
        }
        else
        {
            Debug.Log("Crafting successful, but inventory is full.");
        }
    }

    /// <summary>
    /// 주어진 그리드의 고유한 변형을 생성하는 메서드
    /// </summary>
    private List<int[,]> GetUniqueVariations(int[,] originalGrid)
    {
        List<int[,]> variations = new List<int[,]>();

        for (int rotation = 0; rotation < 4; rotation++)
        {
            int[,] rotatedGrid = RotateGrid(originalGrid, rotation);
            variations.Add(rotatedGrid);

            // 수평 이동
            int[,] horizontalShift = ShiftGridHorizontally(rotatedGrid);
            if (!GridsEqual(horizontalShift, rotatedGrid))
            {
                variations.Add(horizontalShift);
            }

            // 수직 이동
            int[,] verticalShift = ShiftGridVertically(rotatedGrid);
            if (!GridsEqual(verticalShift, rotatedGrid) && !GridsEqual(verticalShift, horizontalShift))
            {
                variations.Add(verticalShift);
            }
        }

        return variations.Distinct(new GridEqualityComparer()).ToList();
    }

    /// <summary>
    /// 그리드를 수평으로 이동하는 메서드
    /// </summary>
    private int[,] ShiftGridHorizontally(int[,] grid)
    {
        int[,] shifted = new int[3, 3];
        int firstNonEmptyColumn = FindFirstNonEmptyColumn(grid);

        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                int newJ = (j + firstNonEmptyColumn) % 3;
                shifted[i, j] = grid[i, newJ];
            }
        }

        return shifted;
    }

    /// <summary>
    /// 그리드를 수직으로 이동하는 메서드
    /// </summary>
    private int[,] ShiftGridVertically(int[,] grid)
    {
        int[,] shifted = new int[3, 3];
        int firstNonEmptyRow = FindFirstNonEmptyRow(grid);

        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                int newI = (i + firstNonEmptyRow) % 3;
                shifted[i, j] = grid[newI, j];
            }
        }

        return shifted;
    }

    private int[,] RotateGrid(int[,] grid, int rotations)
    {
        int[,] rotated = new int[3, 3];
        for (int r = 0; r < rotations; r++)
        {
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    rotated[j, 2 - i] = grid[i, j];
                }
            }
            grid = (int[,])rotated.Clone();
        }
        return grid;
    }


    /// <summary>
    /// 첫 번째 비어있지 않은 열을 찾는 메서드
    /// </summary>
    private int FindFirstNonEmptyColumn(int[,] grid)
    {
        for (int j = 0; j < 3; j++)
        {
            for (int i = 0; i < 3; i++)
            {
                if (grid[i, j] != 0)
                {
                    return j;
                }
            }
        }
        return 0;
    }

    /// <summary>
    /// 첫 번째 비어있지 않은 행을 찾는 메서드
    /// </summary>
    private int FindFirstNonEmptyRow(int[,] grid)
    {
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                if (grid[i, j] != 0)
                {
                    return i;
                }
            }
        }
        return 0;
    }

    private class GridEqualityComparer : IEqualityComparer<int[,]>
    {
        public bool Equals(int[,] x, int[,] y)
        {
            return GridsEqual(x, y);
        }

        public int GetHashCode(int[,] obj)
        {
            int hash = 17;
            foreach (var item in obj)
            {
                hash = hash * 31 + item.GetHashCode();
            }
            return hash;
        }
    }

}
