using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterController : MonoBehaviour
{
    private Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        GetComponent<PlayerInput>().SwitchCurrentControlScheme(Keyboard.current, Mouse.current);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Move(InputAction.CallbackContext context)
    {
        if(context.performed)
        {
            print("MOVE!!!");
            Vector2 movement = context.ReadValue<Vector2>();
            rb.velocity = new Vector3(movement.x * 20, rb.velocity.y, movement.y * 20);
        }
        else
        {
            rb.velocity = new Vector3(0, rb.velocity.y, 0);
        }    
    }

    public void Fire1(InputAction.CallbackContext context)
    {
        if(context.performed)
        {
            print("FIRE 1 !!!!");
        }
    }

    public void Fire(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            print("FIRE!!!!");
        }
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if(context.performed)
        {
            print("JUMP!!!!");
            rb.velocity += Vector3.up * 10;
        }
        
    }
}
