using Unity.VisualScripting;

public class CraftingSystem
{
    public const int GRID_SIZE = 9;

    private Item[,] itemArray;

    public CraftingSystem()
    {
        itemArray = new Item[GRID_SIZE, GRID_SIZE];
    }

    private bool IsEmpty(int x, int y)
    {
        return itemArray[x, y] == null;
    }

    private Item GetItem(int x, int y)
    {
        return itemArray[x, y];
    }

    private void SetItem(Item item, int x, int y)
    {
        itemArray[x, y] = item;
    }

    //private void InCreaseItemAmount(int x, int y)
    //{
    //    GetItem(x, y).amount++;
    //}

    private void RemoveItem(int x, int y)
    {
        SetItem(null, x, y);
    }

    private bool TryAddItem(Item item, int x, int y)
    {
        if(IsEmpty(x, y))
        {
            SetItem(item, x, y);
            return true;
        }
        else
        {
            if(item.itemType == GetItem(x,y).itemType)
            {
                //InCreaseItemAmount(x,y);
                return true;
            }
            else
            {
                return false;
            }
        }
    }


}
