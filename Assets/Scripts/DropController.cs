using UnityEngine;

public class DropController : MonoBehaviour
{
    

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            //�κ��丮�� �߰�

                Destroy(gameObject);

        }
    }
}
