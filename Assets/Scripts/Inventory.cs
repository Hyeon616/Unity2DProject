using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    public int stackLimit = 64;

    public ToolClass tool;
    public Vector2 inventoryOffset;
    public Vector2 hotbarOffset;
    public Vector2 multiplier;
    public GameObject inventoryUI;
    public GameObject hotbarUI;
    public GameObject inventorySlotPrefab;

    public int inventoryWidth;
    public int inventoryHeight;


    public InventorySlot[,] inventorySlots;
    public InventorySlot[] hotbarSlots;

    public GameObject[,] uiSlots;
    public GameObject[] hotbarUISlots;


    private void Awake()
    {
        inventorySlots = new InventorySlot[inventoryWidth, inventoryHeight];
        uiSlots = new GameObject[inventoryWidth, inventoryHeight];
        hotbarSlots = new InventorySlot[inventoryWidth];
        hotbarUISlots = new GameObject[inventoryWidth];
    }

    private void Start()
    {
        SetUpUI();
        UpdateInventoryUI();
        AddItem(new ItemClass(tool));
    }

    private void FixedUpdate()
    {
        UpdateInventoryUI();
    }

    void SetUpUI()
    {
        // setup Inventory
        for (int x = 0; x < inventoryWidth; x++)
        {
            for (int y = 0; y < inventoryHeight; y++)
            {

                GameObject inventorySlot = Instantiate(inventorySlotPrefab, inventoryUI.transform);
                inventorySlot.GetComponent<RectTransform>().localPosition = new Vector3((x * multiplier.x) + inventoryOffset.x, (y * multiplier.y) + inventoryOffset.y);
                uiSlots[x, y] = inventorySlot;
                inventorySlots[x, y] = null;

            }
        }

        // setup hotbar

        for (int x = 0; x < inventoryWidth; x++)
        {

            GameObject hotbarSlot = Instantiate(inventorySlotPrefab, hotbarUI.transform);
            hotbarSlot.GetComponent<RectTransform>().position = new Vector3((x * multiplier.x) + hotbarOffset.x, hotbarOffset.y);
            hotbarUISlots[x] = hotbarSlot;
            hotbarSlots[x] = null;

        }


    }

    void UpdateInventoryUI()
    {
        // update Inventory
        for (int x = 0; x < inventoryWidth; x++)
        {
            for (int y = 0; y < inventoryHeight; y++)
            {
                if (inventorySlots[x, y] == null)
                {
                    uiSlots[x, y].transform.GetChild(0).GetComponent<Image>().sprite = null;
                    uiSlots[x, y].transform.GetChild(0).GetComponent<Image>().enabled = false;

                    uiSlots[x, y].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "0";
                    uiSlots[x, y].transform.GetChild(1).GetComponent<TextMeshProUGUI>().enabled = false;
                }
                else
                {
                    uiSlots[x, y].transform.GetChild(0).GetComponent<Image>().enabled = true;
                    uiSlots[x, y].transform.GetChild(0).GetComponent<Image>().sprite = inventorySlots[x, y].item.sprite;

                    uiSlots[x, y].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = inventorySlots[x, y].quantity.ToString();
                    uiSlots[x, y].transform.GetChild(1).GetComponent<TextMeshProUGUI>().enabled = true;
                }


            }
        }

        // update hotbar
        for (int x = 0; x < inventoryWidth; x++)
        {

            if (inventorySlots[x, inventoryHeight - 1] == null)
            {
                hotbarUISlots[x].transform.GetChild(0).GetComponent<Image>().sprite = null;
                hotbarUISlots[x].transform.GetChild(0).GetComponent<Image>().enabled = false;

                hotbarUISlots[x].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "0";
                hotbarUISlots[x].transform.GetChild(1).GetComponent<TextMeshProUGUI>().enabled = false;
            }
            else
            {
                hotbarUISlots[x].transform.GetChild(0).GetComponent<Image>().enabled = true;
                hotbarUISlots[x].transform.GetChild(0).GetComponent<Image>().sprite = inventorySlots[x, inventoryHeight - 1].item.sprite;

                hotbarUISlots[x].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = inventorySlots[x, inventoryHeight - 1].quantity.ToString();
                hotbarUISlots[x].transform.GetChild(1).GetComponent<TextMeshProUGUI>().enabled = true;
            }



        }


    }


    public bool AddItem(ItemClass item)
    {

        Vector2Int itemPos = ContainsItem(item);

        if (itemPos != Vector2Int.one * -1)
        {
            inventorySlots[itemPos.x, itemPos.y].quantity += 1;

            UpdateInventoryUI();
            return true;
        }
        else
        {
            for (int y = inventoryHeight - 1; y >= 0; y--)
            {
                for (int x = 0; x < inventoryWidth; x++)
                {
                    if (inventorySlots[x, y] == null)
                    {
                        inventorySlots[x, y] = new InventorySlot { item = item, position = new Vector2Int(x, y), quantity = 1 };
                        UpdateInventoryUI();
                        return true;
                    }
                }
            }
        }
        return false;
    }

    public Vector2Int ContainsItem(ItemClass item)
    {
        for (int y = inventoryHeight - 1; y >= 0; y--)
        {
            for (int x = 0; x < inventoryWidth; x++)
            {
                if (inventorySlots[x, y] != null)
                {
                    if (inventorySlots[x, y].item.itemName == item.itemName)
                    {
                        if (item.isStackable && inventorySlots[x, y].quantity < stackLimit)
                            return new Vector2Int(x, y);
                    }
                }
            }
        }
        return Vector2Int.one * -1;
    }

    public bool RemoveItem(ItemClass item)
    {
        for (int y = inventoryHeight - 1; y >= 0; y--)
        {
            for (int x = 0; x < inventoryWidth; x++)
            {
                if (inventorySlots[x, y].item.itemName == item.itemName)
                {
                    inventorySlots[x, y].quantity -= 1;
                    if (inventorySlots[x, y].quantity <=0)
                    {
                        item.itemType = ItemClass.ItemType.NULL;
                        inventorySlots[x, y] = null;
                        
                    }

                    UpdateInventoryUI();
                    return true;
                }
            }
        }
        return false;
    }

}
