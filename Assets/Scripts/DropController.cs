using UnityEngine;

public class DropController : MonoBehaviour
{
    
    public ItemClass item;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            //�κ��丮�� �߰�

            if(collision.GetComponent<Inventory>().AddItem(item))
                Destroy(gameObject);

        }
    }
}
