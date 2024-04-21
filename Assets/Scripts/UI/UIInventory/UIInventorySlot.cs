using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class UIInventorySlot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    private Camera mainCamera;
    private Canvas parentCanvas;
    private Transform parentItem;
    public GameObject draggedItem;

    public Image inventorySlotHighlight;
    public Image inventorySlotImage;
    public TextMeshProUGUI textMeshProUGUI;

    [SerializeField] private UIInventoryBar inventoryBar = null;
    [SerializeField] private GameObject inventoryTextBoxPrefab = null;
    [HideInInspector] public bool isSelected = false;
    [HideInInspector] public ItemDetails itemDetails;
    [SerializeField] private GameObject itemPrefab = null;
    [HideInInspector] public int itemQuantity;
    [SerializeField] private int slotNumber = 0;

    private void Awake()
    {
        parentCanvas = GetComponentInParent<Canvas>();
    }

    private void OnDisable()
    {
        EventHandler.AfterSceneLoadEvent -= SceneLoaded;
    }

    // Scene이 Load된 후 찾기
    private void SceneLoaded()
    {
        parentItem = GameObject.FindGameObjectWithTag(Tags.ItemsParentTransform).transform;
    }

    private void OnEnable()
    {
        EventHandler.AfterSceneLoadEvent += SceneLoaded;
    }


    private void Start()
    {
        mainCamera = Camera.main;
        
    }

    private void SetSelectedItem()
    {
        inventoryBar.ClearHighlightOnInventorySlots();

        isSelected = true;

        inventoryBar.SetHighlightedInventorySlots();

        InventoryManager.Instance.SetSelectedInventoryItem(InventoryLocation.player, itemDetails.itemCode);


    }

    public void ClearSelectedItem()
    {
        inventoryBar.ClearHighlightOnInventorySlots();

        isSelected = false;

        InventoryManager.Instance.ClearSelectedInventoryItem(InventoryLocation.player);

    }

    // 마우스위치에 아이템 드랍
    private void DropSelectedItemAtMousePosition()
    {
        if (itemDetails != null && isSelected)
        {
            Vector3 worldPosition = mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, mainCamera.transform.position.z + 20f));

            GameObject itemGameObject = Instantiate(itemPrefab, worldPosition, Quaternion.identity, parentItem);
            itemGameObject.transform.localScale = new Vector2(0.5f, 0.5f);
            Items item = itemGameObject.GetComponent<Items>();
            item.ItemCode = itemDetails.itemCode;

            // 플레이어 인벤토리로 부터 제거
            InventoryManager.Instance.RemoveItem(InventoryLocation.player, item.ItemCode);

            if (InventoryManager.Instance.FindItemInInventory(InventoryLocation.player, item.ItemCode) == -1)
            {
                ClearSelectedItem();
            }
        }
    }

    


    public void OnBeginDrag(PointerEventData eventData)
    {
        if (itemDetails != null)
        {

            draggedItem = Instantiate(inventoryBar.inventoryBarDraggedItem, inventoryBar.transform);

            Image draggedItemImage = draggedItem.GetComponentInChildren<Image>();
            draggedItemImage.sprite = inventorySlotImage.sprite;


            SetSelectedItem();

        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (draggedItem != null)
        {
            draggedItem.transform.position = Input.mousePosition;


        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (draggedItem != null)
        {
            Destroy(draggedItem);

            if (eventData.pointerCurrentRaycast.gameObject != null && eventData.pointerCurrentRaycast.gameObject.GetComponent<UIInventorySlot>() != null)
            {
                int toSlotNumber = eventData.pointerCurrentRaycast.gameObject.GetComponent<UIInventorySlot>().slotNumber;

                InventoryManager.Instance.SwapInventoryItems(InventoryLocation.player, slotNumber, toSlotNumber);

                DestoryInventoryTextBox();

                ClearSelectedItem();

            }
            else
            {
                if (itemDetails.canBeDropped)
                {
                    DropSelectedItemAtMousePosition();
                }
            }

        }
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (itemQuantity != 0)
        {

            inventoryBar.inventoryTextBoxGameObject = Instantiate(inventoryTextBoxPrefab, transform.position, Quaternion.identity);
            inventoryBar.inventoryTextBoxGameObject.transform.SetParent(parentCanvas.transform, false);

            UIInventoryTextBox inventoryTextBox = inventoryBar.inventoryTextBoxGameObject.GetComponent<UIInventoryTextBox>();

            string itemTypeDescription = InventoryManager.Instance.GetItemTypeDescription(itemDetails.itemType);

            inventoryTextBox.SetTextboxText(itemDetails.itemDescription, itemTypeDescription, "", itemDetails.itemLongDescription, "", "");

            inventoryBar.inventoryTextBoxGameObject.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0f);
            inventoryBar.inventoryTextBoxGameObject.transform.position = new Vector3(transform.position.x, transform.position.y + 50, transform.position.z);

        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        DestoryInventoryTextBox();
    }

    private void DestoryInventoryTextBox()
    {
        if (inventoryBar.inventoryTextBoxGameObject != null)
        {
            inventoryBar.inventoryTextBoxGameObject.SetActive(false);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            if (isSelected == true)
            {
                ClearSelectedItem();
            }
            else
            {
                if (itemQuantity > 0)
                {

                    SetSelectedItem();
                }
            }
        }
    }


}
