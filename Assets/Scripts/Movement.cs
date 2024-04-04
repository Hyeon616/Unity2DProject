using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Movement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    private Vector2 inputMovement = Vector2.zero;

    private void Awake()
    {
        
    }

    void Start()
    {
        
    }

    private void FixedUpdate()
    {
        Vector2 moveMovement = inputMovement * moveSpeed * Time.deltaTime;
        transform.Translate(moveMovement);
    }

    void Update()
    {
        
        

    }

    private void OnMove(InputValue inputValue)
    {
        inputMovement = inputValue.Get<Vector2>();

    }
}
