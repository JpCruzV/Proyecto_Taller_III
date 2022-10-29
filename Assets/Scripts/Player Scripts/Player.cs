using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {


    Rigidbody rb;


    //[Header("Anims")]

    //[SerializeField] Animator anim;


    [Header("Movement")]

    [SerializeField] float moveSpeed;
    [SerializeField] float jumpForce;
    [SerializeField] float jumpCooldown;

    Vector3 movement;
    bool readyToJump;
    //bool IsFalling;

    float horizontalInput;
    float verticalInput;
    bool running;


    [Header("GroundCheck")]

    [SerializeField] float playerHeight;
    [SerializeField] LayerMask floor;
    bool grounded;


    private void Start() {

        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        readyToJump = true;
        //IsFalling = true;
    }


    private void Update() {

        MyInput();
    }


    private void FixedUpdate() {

        MovePlayer();

        // ground check
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, floor);
    }


    void MyInput() {

        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");
        running = Input.GetButton("Crouch");


        //when to jump
        if (Input.GetButton("Jump") && readyToJump && grounded) {

            readyToJump = false;
            Jump();

            Invoke(nameof(ResetJump), jumpCooldown);
        }
    }


    void MovePlayer() {

        if (running) {

            rb.velocity = new Vector3(movement.x * moveSpeed * 2, rb.velocity.y, 0);
        }

        else {

            rb.velocity = new Vector3(movement.x * moveSpeed, rb.velocity.y, 0);
        }
    }



    void Jump() {

        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }


    void ResetJump() {

        readyToJump = true;
    }
}
