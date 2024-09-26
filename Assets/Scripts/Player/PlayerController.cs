using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : Singleton<PlayerController>
{

    public bool onGround;

    private Vector2 inputMovement;
    Rigidbody2D rb;
    Animator anim;

    public bool inventoryActive;

    [HideInInspector]
    public Vector2 mousePos;
    public TerrainGeneration terrainGeneration;
    private Transform[] characterAttribute;

    [SerializeField] private PauseMenuInventoryManagementSlot weaponSlot;


    protected override void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = transform.GetChild(0).GetComponent<Animator>();
        characterAttribute = GetComponentsInChildren<Transform>();
    }

    //private void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.red;
    //    Gizmos.DrawWireSphere(transform.position, Settings.playerRange);
    //}

    private void FixedUpdate()
    {
        Vector2 moveMovement = inputMovement * Settings.moveSpeed;

        rb.velocity = new Vector2(moveMovement.x, rb.velocity.y);

        EquipWeapon();

        if (Input.GetMouseButtonDown(0))
        {
            anim.SetTrigger("Attack");
        }
    }

    

    private void OnMove(InputValue inputValue)
    {

        inputMovement = inputValue.Get<Vector2>();
        
        anim.SetFloat("xVelocity", Mathf.Abs(inputMovement.normalized.x / 2));
        Settings.moveSpeed = 2.5f;
        if (Input.GetKey(KeyCode.LeftShift))
        {
            Settings.moveSpeed = 5f;
            anim.SetFloat("xVelocity", Mathf.Abs(inputMovement.normalized.x));
        }

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
            float jumpForce = Mathf.Sqrt(Settings.jumpHeight * (Physics2D.gravity.y * rb.gravityScale) * -2) * rb.mass;
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
            if (Vector2.Distance(transform.position, mousePosition) <= Settings.playerRange && Vector2.Distance(transform.position, mousePos) > 1f)
            {
                anim.SetTrigger("Attack");
                terrainGeneration.MiningTile(mousePosX, mousePosY);

                if (InventoryManager.Instance.GetSelectedInventoryItemDetails(InventoryLocation.player) != null)
                {
                    if (InventoryManager.Instance.GetSelectedInventoryItemDetails(InventoryLocation.player).itemType == ItemType.Block)
                    {

                        if (TerrainGeneration.Instance.GetTileFromWorld(mousePosX, mousePosY) == null)
                        {
                            DropManager.instance.PlaceBlock(mousePosX, mousePosY, InventoryManager.Instance.GetSelectedInventoryItemDetails(InventoryLocation.player).itemName);

                        }

                    }

                }
            }


        }
    }


    private void OnAdvanceTime(InputValue inputValue)
    {
        float pressed = inputValue.Get<float>();
        if (pressed == 1f)
        {
            TimeManager.Instance.TestAdvanceGameMinute();
        }
    }

    private void OnAdvanceDay(InputValue inputValue)
    {
        float pressed = inputValue.Get<float>();
        if (pressed == 1f)
        {
            TimeManager.Instance.TestAdvanceGameDay();

        }
    }

    
    public void EquipWeapon()
    {
        
        foreach (Transform weapon in characterAttribute)
        {
            if(weaponSlot.itemDetails != null)
            {
                if (weapon.name == "Weapon")
                {
                    weapon.GetComponent<SpriteRenderer>().sprite = weaponSlot.itemDetails.itemSprite;

                }
            }
            
        }

    }



    public Vector3 GetPosition()
    {
        return transform.position;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Items items = collision.collider.GetComponent<Items>();

        if (items != null)
        {
            ItemDetails itemDetails = InventoryManager.Instance.GetItemDetails(items.ItemCode);

            if (itemDetails.canBePickedUp == true)
            {
                InventoryManager.Instance.AddItem(InventoryLocation.player, items, collision.gameObject);
            }

        }

    }




}
