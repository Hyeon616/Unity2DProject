using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpHeight = 3f;
    private int playerRange = 3;

    public bool onGround;
    public ItemClass selectedItem;

    private Vector2 inputMovement;
    Rigidbody2D rb;
    Animator anim;

    public bool inventoryActive;
    public GameObject equippedWeapon;
    [HideInInspector]
    public Vector3 spawnPos;
    public Vector2 mousePos;
    public TerrainGeneration terrainGeneration;

    public Inventory inventory;
    public GameObject hotBarSelectItem;
    public int selectedSlotIndex = 0;

    public void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = transform.GetChild(0).GetComponent<Animator>();
        inventory = GetComponent<Inventory>();
    }

    public void Spawn()
    {
        GetComponent<Transform>().position = spawnPos;


        //EquipWeapon(gameObject);
    }

    void Update()
    {
        inventory.inventoryUI.SetActive(inventoryActive);

    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, playerRange);
    }

    private void FixedUpdate()
    {
        Vector2 moveMovement = inputMovement * moveSpeed;

        rb.velocity = new Vector2(moveMovement.x, rb.velocity.y);
    }


    private void OnMove(InputValue inputValue)
    {

        inputMovement = inputValue.Get<Vector2>();
        anim.SetFloat("xVelocity", Mathf.Abs(inputMovement.x));

        if (inputMovement.x < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
        if (inputMovement.x > 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
    }

    private void OnJump(InputValue inputValue)
    {
        float pressed = inputValue.Get<float>();
        if (onGround && pressed == 1f)
        {
            float jumpForce = Mathf.Sqrt(jumpHeight * (Physics2D.gravity.y * rb.gravityScale) * -2) * rb.mass;
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
    }



    private void OnMouse(InputValue inputValue)
    {
        mousePos = inputValue.Get<Vector2>();

    }

    private void OnAttack(InputValue inputValue)
    {
        float pressed = inputValue.Get<float>();
        mousePos = Mouse.current.position.ReadValue();
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(mousePos);
        int mousePosX = Mathf.RoundToInt(mousePosition.x);
        int mousePosY = Mathf.RoundToInt(mousePosition.y);

        if (pressed == 1f)
        {
            anim.SetTrigger("Attack");

            if (Vector2.Distance(transform.position, mousePosition) <= playerRange && Vector2.Distance(transform.position, mousePos) > 1f)
            {
                terrainGeneration.MiningTile(mousePosX, mousePosY);

                if (selectedItem != null)
                {
                    if (selectedItem.itemType == ItemClass.ItemType.BLOCK)
                    {
                        if(terrainGeneration.CheckTile(selectedItem.tile, mousePosX, mousePosY, false))
                            inventory.RemoveItem(selectedItem);
                    }
                }

            }
        }
    }

    // 인벤토리
    private void OnInventory(InputValue inputValue)
    {
        float pressed = inputValue.Get<float>();
        if (pressed == 1f)
        {
            inventoryActive = !inventoryActive;
        }
    }

    // 핫키
    private void OnHotbar(InputValue inputValue)
    {

        Vector2 scrollDelta = inputValue.Get<Vector2>();

        if (scrollDelta.normalized.y > 0)
        {
            if (selectedSlotIndex < inventory.inventoryWidth)
            {
                selectedSlotIndex++;
                if (selectedSlotIndex >= inventory.inventoryWidth)
                {
                    selectedSlotIndex = 0;
                }
            }
        }
        else if (scrollDelta.normalized.y < 0)
        {
            if (selectedSlotIndex >= 0)
            {
                selectedSlotIndex--;

                if (selectedSlotIndex < 0)
                {
                    selectedSlotIndex = inventory.inventoryWidth - 1;
                }
            }
        }

        float hotBarPosX = inventory.hotbarUISlots[selectedSlotIndex].transform.position.x;
        float hotBarPosY = inventory.hotbarUISlots[selectedSlotIndex].transform.position.y;

        
        // select slot UI
        hotBarSelectItem.transform.position = new Vector2(hotBarPosX - 20, hotBarPosY - 20);

        if (inventory.inventorySlots[selectedSlotIndex, inventory.inventoryHeight - 1] != null)
        {
            selectedItem = inventory.inventorySlots[selectedSlotIndex, inventory.inventoryHeight - 1].item;
           
        }
        else
        {
            selectedItem = null;

        }
        //select item
    }

    public void EquipWeapon(GameObject player)
    {
        if (equippedWeapon == null)
            return;

        Transform[] AllData = player.GetComponentsInChildren<Transform>();

        foreach (Transform weapon in AllData)
        {
            if (weapon.name == "Rig Weapon")
            {

                GameObject temp = Instantiate(equippedWeapon, transform.position, Quaternion.identity);
                temp.transform.SetParent(weapon);
                temp.transform.localPosition = new Vector3(0, 0, 0);

            }
        }
    }







}
