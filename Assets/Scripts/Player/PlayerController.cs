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
    public Vector3 spawnPos;
    public Vector2 mousePos;
    public TerrainGeneration terrainGeneration;

    // public GameObject ui;

    private Transform[] characterAttribute;
    [SerializeField] private SpriteRenderer equippedItemSpriteRenderer = null;

    protected override void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = transform.GetChild(0).GetComponent<Animator>();

        characterAttribute = GetComponentsInChildren<Transform>();

       // characterAttributeCustomisationList = new List<CharacterAttribute>();

        
    }



    public void Spawn()
    {
        GetComponent<Transform>().position = spawnPos;


        //EquipWeapon();
    }

    void Update()
    {

        //ui.SetActive(inventoryActive);

    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, Settings.playerRange);
    }

    private void FixedUpdate()
    {
        Vector2 moveMovement = inputMovement * Settings.moveSpeed;

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
            anim.SetTrigger("Attack");

            if (Vector2.Distance(transform.position, mousePosition) <= Settings.playerRange && Vector2.Distance(transform.position, mousePos) > 1f)
            {
                terrainGeneration.MiningTile(mousePosX, mousePosY);

                //if (selectedItem != null)
                //{
                //    if (selectedItem.itemType == ItemClass.ItemType.BLOCK)
                //    {
                //        if(terrainGeneration.CheckTile(selectedItem.tile, mousePosX, mousePosY, false))
                //            inventory.RemoveItem(selectedItem);
                //    }
                //}

            }
        }
    }

    // 인벤토리
    private void OnInventory(InputValue inputValue)
    {
        float pressed = inputValue.Get<float>();
        if (pressed == 1f)
        {
            //inventoryActive = !inventoryActive;
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

    //public void DisablePlayerInputAndResetMovement()
    //{
    //    DisablePlayerInput();
    //}

    //public void DisablePlayerInput()
    //{
    //    PlayerInputIsDisabled = true;

    //}

    //public void EnablePlayerInput()
    //{
    //    PlayerInputIsDisabled = false;

    //}



    //public void EquipWeapon()
    //{
    //    if (equippedWeapon == null)
    //        return;

    //    foreach (Transform weapon in characterAttribute)
    //    {
    //        if (weapon.name == "Weapon")
    //        {
    //            weapon.GetComponent<SpriteRenderer>().sprite = equippedWeapon.GetComponent<SpriteRenderer>().sprite;
    //            //weaponEquipment = Instantiate(equippedWeapon, transform.position, Quaternion.identity);
    //            //weaponEquipment.transform.SetParent(weapon);
    //            //weaponEquipment.transform.localPosition = new Vector3(0, 0, 0);

    //        }
    //    }
    //}

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

            if(itemDetails.canBePickedUp == true)
            {
                InventoryManager.Instance.AddItem(InventoryLocation.player, items, collision.gameObject);
            }

        }

        //if (collision.gameObject.layer ==6)
        //{
            
        //    inventory.AcquireItem(collision.gameObject.GetComponent<ItemPickUp>().Item, 1);
        //    Destroy(collision.gameObject);


        //}

    }




}
