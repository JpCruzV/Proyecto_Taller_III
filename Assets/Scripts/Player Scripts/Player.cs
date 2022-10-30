using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {


    Rigidbody rb;


    //[Header("Anims")]

    //[SerializeField] Animator anim;


    [Header("Movement")]

    [SerializeField] float moveSpeed;
    Vector3 movement;
    //bool running;


    [Header("Jumping")]

    [SerializeField] float jumpForce;
    [SerializeField] float jumpCooldown;
    bool readyToJump;


    [Header("Crouching")]

    [SerializeField] float crouchSpeed;
    [SerializeField] float crouchYScale;
    float startYScale;
    bool crouching;


    [Header("GroundCheck")]

    [SerializeField] float playerHeight;
    [SerializeField] LayerMask floor;
    bool grounded;
    //bool IsFalling;


    private void Start() {

        rb = GetComponent<Rigidbody>();
        startYScale = transform.localScale.y;
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

        movement.x = Input.GetAxisRaw("Horizontal");


        //when to jump
        if (Input.GetKeyDown( KeyCode.W ) && readyToJump && grounded) {

            readyToJump = false;
            Jump();

            Invoke(nameof(ResetJump), jumpCooldown);
        }

        if (Input.GetKey(KeyCode.S) && grounded) {

            Crouch();
        }
        if (Input.GetKeyUp(KeyCode.S)) {

            StandUp();
        }
    }


    void Crouch() {

        transform.localScale = new Vector3(transform.localScale.x, crouchYScale, transform.localScale.z);
        rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
        crouching = true;
    }


    void StandUp() {

        transform.localScale = new Vector3(transform.localScale.x, startYScale, transform.localScale.z);
        crouching = false;
    }


    void MovePlayer() {

        if (crouching) {

            rb.velocity = new Vector3(movement.x * crouchSpeed, rb.velocity.y, 0);
        }
        else {

            rb.velocity = new Vector3(movement.x * moveSpeed, rb.velocity.y, 0);
        }

        /*
        if (running) {

            rb.velocity = new Vector3(movement.x * moveSpeed * 2, rb.velocity.y, 0);
        }
        */
    }



    void Jump() {

        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }


    void ResetJump() {

        readyToJump = true;
    }
}
