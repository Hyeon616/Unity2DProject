using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
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
    public TextMeshProUGUI AmountText;

    private Dictionary<string, Recipe> craftingRecipes; // 조합 레시피 저장 dictionary

    
    // 초기화 
    void Start()
    {
        if (inventoryManager == null)
        {
            inventoryManager = FindObjectOfType<InventoryManager>();
            if (inventoryManager == null)
            {
                Debug.LogError("InventoryManager not found!");
                return;
            }
        }

        if (craftButton != null)
        {
            craftButton.onClick.AddListener(AttemptCrafting);
        }
        else
        {
            Debug.LogError("Craft button not assigned!");
        }

        inventoryManager.OnMixSlotChanged += UpdateCraftingPreview;

        LoadRecipes();
        ClearCraftingPreview();
    }

    void OnDestroy()
    {
        if (inventoryManager != null)
        {
            inventoryManager.OnMixSlotChanged -= UpdateCraftingPreview;
        }
    }

    // JSON 파일에서 레시피 로드
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

    // 조합
    public void AttemptCrafting()
    {
        int[,] currentGrid = GetCurrentCraftingGrid();
        List<int[,]> gridVariations = GetUniqueVariations(currentGrid);

        foreach (var recipe in craftingRecipes.Values)
        {
            if (MatchesRecipe(gridVariations, recipe, out int multiplier))
            {
                CraftItem(recipe.result, recipe.count, recipe.composition, multiplier);
                UpdateCraftingPreview();
                return;
            }
        }

        Debug.Log("No matching recipe found.");
    }

    // 현재 조합창 그리드
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

    // 그리드가 레시피와 일치하는지 확인
    private bool MatchesRecipe(List<int[,]> gridVariations, Recipe recipe, out int multiplier)
    {
        int[,] recipeGrid = ConvertPatternToGrid(recipe.composition);
        foreach (var variation in gridVariations)
        {
            if (GridsProportional(variation, recipeGrid, out multiplier))
            {
                return true;
            }
        }
        multiplier = 0;
        return false;
    }

    private bool GridsProportional(int[,] grid1, int[,] grid2, out int multiplier)
    {
        multiplier = int.MaxValue;
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                if (grid2[i, j] != 0)
                {
                    if (grid1[i, j] == 0 || grid1[i, j] < grid2[i, j])
                    {
                        multiplier = 0;
                        return false;
                    }
                    int currentMultiplier = grid1[i, j] / grid2[i, j];
                    multiplier = Mathf.Min(multiplier, currentMultiplier);
                }
                else if (grid1[i, j] != 0)
                {
                    multiplier = 0;
                    return false;
                }
            }
        }
        return multiplier > 0;
    }

    // 레시피의 그리드를 정수로 변환
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

    
    // 조합창과 레시피가 동일한지 확인
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

    // 아이템을 조합
    private void CraftItem(int resultItemId, int count, List<string> recipeComposition, int multiplier)
    {
        if (inventoryManager.AddItem(resultItemId, count))
        {
            Debug.Log($"Successfully crafted {count} of item with ID: {resultItemId}");

            // 레시피에 따라 재료 소비
            int[,] recipeGrid = ConvertPatternToGrid(recipeComposition);
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    int slotIndex = i * 3 + j;
                    if (inventoryManager.mixSlots.TryGetValue(slotIndex, out InventorySlot slot) && !slot.IsEmpty)
                    {
                        int amountToRemove = recipeGrid[i, j] * multiplier;
                        slot.amount -= amountToRemove;
                        if (slot.amount <= 0)
                        {
                            slot.Clear();
                        }
                    }
                }
            }

            // UI 업데이트
            inventoryManager.UpdateAllUI();
        }
        else
        {
            Debug.Log("Crafting successful, but inventory is full.");
        }
    }


    // 레시피의 다양한 형태 생성
    private List<int[,]> GetUniqueVariations(int[,] originalGrid)
    {
        List<int[,]> variations = new List<int[,]>();

        for (int rotation = 0; rotation < 4; rotation++)
        {
            int[,] rotatedGrid = RotateGrid(originalGrid, rotation);

            for (int verticalShift = 0; verticalShift < 3; verticalShift++)
            {
                for (int horizontalShift = 0; horizontalShift < 3; horizontalShift++)
                {
                    int[,] shiftedGrid = ShiftGrid(rotatedGrid, verticalShift, horizontalShift);
                    variations.Add(shiftedGrid);
                }
            }
        }

        return variations.Distinct(new GridEqualityComparer()).ToList();
    }

    
    // 그리드 이동
    

    private int[,] ShiftGrid(int[,] grid, int verticalShift, int horizontalShift)
    {
        int[,] shiftedGrid = new int[3, 3];
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                int newI = (i + verticalShift) % 3;
                int newJ = (j + horizontalShift) % 3;
                shiftedGrid[newI, newJ] = grid[i, j];
            }
        }
        return shiftedGrid;
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

    private void UpdateCraftingPreview()
    {
        int[,] currentGrid = GetCurrentCraftingGrid();

        // 모든 슬롯이 비어있는지 확인
        bool allSlotsEmpty = true;
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                if (currentGrid[i, j] != 0)
                {
                    allSlotsEmpty = false;
                    break;
                }
            }
            if (!allSlotsEmpty) break;
        }

        // 모든 슬롯이 비어있으면 미리보기 초기화
        if (allSlotsEmpty)
        {
            ClearCraftingPreview();
            return;
        }

        List<int[,]> gridVariations = GetUniqueVariations(currentGrid);

        foreach (var recipe in craftingRecipes.Values)
        {
            if (MatchesRecipe(gridVariations, recipe, out int multiplier))
            {
                ShowCraftingPreview(recipe.result, recipe.count);
                return;
            }
        }

        // 매칭되는 레시피가 없으면 미리보기 초기화
        ClearCraftingPreview();
    }

    private void ShowCraftingPreview(int itemId, int count)
    {
        Item item = ItemManager.Instance.GetItem(itemId);
        if (item != null)
        {
            resultImage.gameObject.SetActive(true);
            resultImage.sprite = item.icon;
            AmountText.gameObject.SetActive(true);
            AmountText.text = count.ToString();
        }
    }

    private void ClearCraftingPreview()
    {
        resultImage.sprite = null;
        resultImage.gameObject.SetActive(false);
        AmountText.text = "";
        AmountText.gameObject.SetActive(false);
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
