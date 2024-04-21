using UnityEngine;

public class CopyPosition : MonoBehaviour
{
    [SerializeField] private bool x, y, z;
    [SerializeField] Transform target;

    void Update()
    {
        if (!target)
            return;

        transform.position = new Vector3(x ? target.position.x : transform.position.x, y ? target.position.y+0.8f : transform.position.y, z ? target.position.z : transform.position.z);

    }
}
