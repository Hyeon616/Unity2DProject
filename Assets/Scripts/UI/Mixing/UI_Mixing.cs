using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Mixing : MonoBehaviour
{
    private InventoryManager inventoryManager;
    private MixingItem mixingItem;

    public Button craftButton; 
    public Image resultImage;
    public TextMeshProUGUI AmountText;

    private Dictionary<string, Recipe> craftingRecipes; // 조합 레시피 저장 dictionary


    // 초기화 
    void Start()
    {
        inventoryManager = FindObjectOfType<InventoryManager>();
        mixingItem = GetComponent<MixingItem>();

        if (inventoryManager == null || mixingItem == null)
        {
            Debug.LogError("Required components not found!");
            return;
        }

        if (craftButton != null)
        {
            craftButton.onClick.AddListener(OnClickCrafting);
        }
        else
        {
            Debug.LogError("Craft button not assigned!");
        }

        inventoryManager.OnMixSlotChanged += UpdateCraftingUI;

        LoadRecipes();
        ClearCraftingResult();
    }

    void OnDestroy()
    {
        if (inventoryManager != null)
        {
            inventoryManager.OnMixSlotChanged -= UpdateCraftingUI;
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
            mixingItem.LoadRecipes(craftingRecipes);
        }
        else
        {
            Debug.LogError("Recipes JSON file not found!");
        }
    }
    // 조합
    public void OnClickCrafting()
    {
        int[,] currentGrid = GetCraftingGrid();
        if (mixingItem.ExcuteCrafting(currentGrid, out int resultItemId, out int count, out int minMaterials))
        {
            CraftItem(resultItemId, count, minMaterials);
            UpdateCraftingUI();
        }
        else
        {
            Debug.Log("No matching recipe found.");
        }
    }

    // 현재 조합창 그리드
    private int[,] GetCraftingGrid()
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
                grid[row, col] = 0;
            }
        }
        return grid;
    }

    // 아이템을 조합
    private void CraftItem(int resultItemId, int count, int minMaterials)
    {
        int maxCraftingAmount = CheckCraftingItems(minMaterials);
        int totalCraftedCount = count * maxCraftingAmount;

        if (inventoryManager.AddItem(resultItemId, totalCraftedCount))
        {
            for (int i = 0; i < 9; i++)
            {
                if (inventoryManager.mixSlots.TryGetValue(i, out InventorySlot slot) && !slot.IsEmpty)
                {
                    int useItemAmount = minMaterials * maxCraftingAmount;
                    int slotItemAmount = slot.amount - useItemAmount;

                    if (slotItemAmount > 0)
                    {
                        slot.amount = slotItemAmount;
                    }
                    else
                    {
                        slot.Clear();
                    }
                }
            }

            Debug.Log($"Successfully crafted {totalCraftedCount} of item with ID: {resultItemId}");
            inventoryManager.UpdateAllUI();
        }
        else
        {
            Debug.Log("Crafting successful, but inventory is full.");
        }
    }


    private int CheckCraftingItems(int minMaterials)
    {
        int maxTimes = int.MaxValue;

        for (int i = 0; i < 9; i++)
        {
            if (inventoryManager.mixSlots.TryGetValue(i, out InventorySlot slot) && !slot.IsEmpty)
            {
                int timesPossible = slot.amount / minMaterials;
                maxTimes = Mathf.Min(maxTimes, timesPossible);
            }
        }

        return maxTimes;
    }



    private void UpdateCraftingUI()
    {
        int[,] currentGrid = GetCraftingGrid();

        if (mixingItem.ExcuteCrafting(currentGrid, out int resultItemId, out int count, out int minMaterials))
        {
            int maxCraftingAmount = CheckCraftingItems(minMaterials);
            int totalCraftedCount = count * maxCraftingAmount;
            CraftingResult(resultItemId, totalCraftedCount);
        }
        else
        {
            ClearCraftingResult();
        }
    }

    private void CraftingResult(int itemId, int count)
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

    private void ClearCraftingResult()
    {
        resultImage.sprite = null;
        resultImage.gameObject.SetActive(false);
        AmountText.text = "";
        AmountText.gameObject.SetActive(false);
    }

}
