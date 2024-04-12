using UnityEngine;

public class DropController : MonoBehaviour
{
    //public bool droppable;

    public ItemClass item;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            //인벤토리에 추가

            if(collision.GetComponent<Inventory>().AddItem(item))
                Destroy(gameObject);

        }
    }
}
