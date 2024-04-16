using UnityEngine;

public class GroundCheck : MonoBehaviour
{

    public bool onGround;
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 3)
        {
            onGround = true;
        }

        transform.parent.GetComponent<PlayerController>().onGround = onGround;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 3)
        {
            onGround = false;
        }

        transform.parent.GetComponent<PlayerController>().onGround = onGround;
    }
}
