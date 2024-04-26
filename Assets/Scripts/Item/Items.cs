using UnityEngine;

public class Items : MonoBehaviour
{

    [ItemCodeDescription]
    [SerializeField]
    private int _itemCode;
    private ItemType _itemType;
    private SpriteRenderer spriteRenderer;

    public int ItemCode { get { return _itemCode; } set { _itemCode = value; } }
    public ItemType ItemType { get { return _itemType; } set { _itemType = value; } }

    private void Awake()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    private void Start()
    {
        if (ItemCode != 0)
        {
            Init(ItemCode, ItemType);
        }
    }


    public void Init(int itemCodeParam, ItemType itemType)
    {
        if (itemCodeParam != 0)
        {
            ItemCode = itemCodeParam;
            ItemType = itemType;
            ItemDetails itemDetails = InventoryManager.Instance.GetItemDetails(ItemCode);

            spriteRenderer.sprite = itemDetails.itemSprite;
            

        }
    }

}
