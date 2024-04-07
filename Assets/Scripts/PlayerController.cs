using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpHeight = 3f;


    [SerializeField] private bool onGround;

    private Vector2 inputMovement = Vector2.zero;
    Rigidbody2D rb;
    Animator anim;

    public Vector2 spawnPos;
    public void Spawn()
    {
        GetComponent<Transform>().position = spawnPos;
        rb = GetComponent<Rigidbody2D>();
        anim = transform.GetChild(0).GetComponent<Animator>();
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
}
