using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class Recipe
{
    public List<string> composition;
    public int result;
    public int count;
}



public class MixingItem : MonoBehaviour
{
    private Dictionary<string, Recipe> craftingRecipes;

    public void LoadRecipes(Dictionary<string, Recipe> recipes)
    {
        craftingRecipes = recipes;
    }

    public bool ExcuteCrafting(int[,] currentGrid, out int resultItemId, out int count, out int minMaterials)
    {
        List<int[,]> gridVariations = GetGrid(currentGrid);

        foreach (var recipe in craftingRecipes.Values)
        {
            if (MatchesRecipe(gridVariations, recipe, out minMaterials))
            {
                resultItemId = recipe.result;
                count = recipe.count * minMaterials;
                return true;
            }
        }

        resultItemId = 0;
        count = 0;
        minMaterials = 0;
        return false;
    }

    private bool MatchesRecipe(List<int[,]> gridVariations, Recipe recipe, out int minMaterials)
    {
        int[,] recipeGrid = ConvertRecipes(recipe.composition);
        foreach (var variation in gridVariations)
        {
            if (CheckGridsItems(variation, recipeGrid, out minMaterials))
            {
                return true;
            }
        }
        minMaterials = 0;
        return false;
    }

    private bool CheckGridsItems(int[,] grid1, int[,] grid2, out int minMaterials)
    {
        minMaterials = int.MaxValue;
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                if (grid2[i, j] != 0)
                {
                    if (grid1[i, j] == 0)
                    {
                        minMaterials = 0;
                        return false;
                    }
                    int currentMultiplier = grid1[i, j] / grid2[i, j];
                    if (currentMultiplier == 0)
                    {
                        minMaterials = 0;
                        return false;
                    }
                    minMaterials = Mathf.Min(minMaterials, currentMultiplier);
                }
                else if (grid1[i, j] != 0)
                {
                    minMaterials = 0;
                    return false;
                }
            }
        }
        return minMaterials > 0;
    }

    private int[,] ConvertRecipes(List<string> composition)
    {
        int[,] grid = new int[3, 3];
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                grid[i, j] = int.Parse(composition[i][j].ToString());
            }
        }
        return grid;
    }

    private List<int[,]> GetGrid(int[,] recipeGrid)
    {
        List<int[,]> variations = new List<int[,]>();

        for (int rotation = 0; rotation < 4; rotation++)
        {
            int[,] rotatedGrid = RotateGrid(recipeGrid, rotation);

            for (int verticalShift = 0; verticalShift < 3; verticalShift++)
            {
                for (int horizontalShift = 0; horizontalShift < 3; horizontalShift++)
                {
                    int[,] shiftedGrid = ShiftGrid(rotatedGrid, verticalShift, horizontalShift);
                    variations.Add(shiftedGrid);
                }
            }
        }

        return variations;
    }

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
}
