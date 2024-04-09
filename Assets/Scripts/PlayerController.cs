using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpHeight = 3f;

    [SerializeField] private bool onGround;

    
    private Vector2 inputMovement;
    Rigidbody2D rb;
    Animator anim;

    public GameObject equippedWeapon;

    [HideInInspector]
    public Vector3 spawnPos;
    public Vector2 mousePos;
    public TerrainGeneration terrainGeneration;
    public GameObject dropItem;
    public TileClass selectTile;

    public void Spawn()
    {
        GetComponent<Transform>().position = spawnPos;
        rb = GetComponent<Rigidbody2D>();
        anim = transform.GetChild(0).GetComponent<Animator>();
        
        EquipWeapon(gameObject);
    }

    void Update()
    {



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
        if (onGround)
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
        anim.SetTrigger("Attack");

        mousePos = Mouse.current.position.ReadValue();

        PlayerTileControll();


    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ground"))
        {
            onGround = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Ground"))
        {
            onGround = false;
        }
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

    public void PlayerTileControll()
    {
        terrainGeneration.removeTiles.Clear();

        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(mousePos);
        int mousePosX = Mathf.RoundToInt(mousePosition.x);
        int mousePosY = Mathf.RoundToInt(mousePosition.y);


        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        RaycastHit2D[] hits = Physics2D.GetRayIntersectionAll(ray);

        if (hits.Length > 0)
        {
            foreach (RaycastHit2D hit in hits)
            {
                GameObject obj = hit.collider.gameObject;
                Vector2 objPos = obj.transform.position;
                if (obj.CompareTag("Ground") || obj.CompareTag("Tree"))
                {

                    terrainGeneration.removeTiles.Add(objPos);
                    Destroy(obj);

                    // 추후에 드랍되는아이템, 드랍되지않는아이템 구분 
                    GameObject dropTile = Instantiate(dropItem, new Vector2(objPos.x, objPos.y + 0.5f), Quaternion.identity);
                    dropTile.GetComponent<SpriteRenderer>().sprite = obj.GetComponent<SpriteRenderer>().sprite;

                }
                else
                {

                    if (obj.CompareTag("Player"))
                        return;
                    if (Vector2.Distance(mousePosition, gameObject.transform.position) > 1f)
                    {

                        terrainGeneration.PlaceTile(selectTile, mousePosX, mousePosY);
                    }
                }
            }

            foreach (var obj in terrainGeneration.removeTiles)
            {
                terrainGeneration.worldTiles.Remove(obj);

            }
        }
    }





}
