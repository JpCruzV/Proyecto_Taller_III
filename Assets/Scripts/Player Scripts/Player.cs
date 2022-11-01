using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {


    Rigidbody rb;


    //[Header("Anims")]

    //[SerializeField] Animator anim;


    [Header("Movement")]

    [SerializeField] float moveSpeed;
    [SerializeField] GameObject otherPlayer;
    Vector3 movement;
    bool running;
    bool facingRight = true;
    bool backwards;


    [Header("Jumping")]

    [SerializeField] float jumpForce;
    [SerializeField] float jumpCooldown;
    bool readyToJump = true;


    [Header("Crouching")]

    [SerializeField] float crouchSpeed;
    [SerializeField] float crouchYScale;
    float startYScale;
    bool crouching;


    [Header("GroundCheck")]

    [SerializeField] float playerHeight;
    [SerializeField] LayerMask floor;
    bool grounded;


    //Double Tap
    float ButtonCooler  = 0.5f;
    int ButtonCount = 0;



    private void Start() {

        rb = GetComponent<Rigidbody>();
        startYScale = transform.localScale.y;
        rb.freezeRotation = true;
    }



    private void Update() {

        MyInput();
        Flip();
    }



    private void FixedUpdate() {

        MovePlayer();

        // ground check
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, floor);
    }



    void MyInput() {

        movement.x = Input.GetAxisRaw("Horizontal");


        //Double tap for running
        if ( ( Input.GetKeyDown(KeyCode.D) && facingRight ) || ( Input.GetKeyDown(KeyCode.A) && !facingRight ) ) {

            backwards = false;

            if (ButtonCooler > 0 && ButtonCount == 1) {

                running = true;
            }
            else {

                ButtonCooler = 0.5f;
                ButtonCount++;
            }
        } 
        else if ( Input.GetKeyUp(KeyCode.D) || Input.GetKeyUp(KeyCode.A) ) {

            running = false;
        }


        //Moving Backwards
        if (Input.GetKeyDown(KeyCode.A) && facingRight) {

            backwards = true;
        }
        else if (Input.GetKeyDown(KeyCode.D) && !facingRight) {

            backwards = true;
        }


        //Jump
        if (Input.GetKey( KeyCode.W ) && readyToJump && grounded) {

            readyToJump = false;
            Jump();

            Invoke(nameof(ResetJump), jumpCooldown);
        }


        //Crouch
        if (Input.GetKey(KeyCode.S) && grounded) {

            crouching = true;
            Crouch();
        }
        else if (Input.GetKeyUp(KeyCode.S)) {

            crouching = false;
            Crouch();
        }


        //Button cooldown
        if (ButtonCooler > 0) {

            ButtonCooler -= 1 * Time.deltaTime;
        }
        else {
            ButtonCount = 0;
        }
    }



    void MovePlayer() {

        if (crouching) {

            rb.velocity = new Vector3(movement.x * crouchSpeed, rb.velocity.y, 0);
        }
        else if (running && !crouching) {

            rb.velocity = new Vector3(movement.x * moveSpeed * 2, rb.velocity.y, 0);
        }
        else if (backwards && !running && !crouching) {

            rb.velocity = new Vector3(movement.x * moveSpeed * .5f, rb.velocity.y, 0);
        }
        else {

            rb.velocity = new Vector3(movement.x * moveSpeed, rb.velocity.y, 0);
        }
    }



    void Crouch() {

        if (crouching) {

            transform.localScale = new Vector3(transform.localScale.x, crouchYScale, transform.localScale.z);
            rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
            crouching = true;
        }
        else if (!crouching) {

            transform.localScale = new Vector3(transform.localScale.x, startYScale, transform.localScale.z);
            crouching = false;
        }
    }



    void Jump() {

        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }



    void ResetJump() {

        readyToJump = true;
    }



    void Flip() {

        if (otherPlayer.transform.position.x < transform.position.x && facingRight) {

            facingRight = !facingRight;
            transform.RotateAround(transform.position, transform.up, 180f);
        }
        else if (otherPlayer.transform.position.x > transform.position.x && !facingRight) {

            facingRight = !facingRight;
            transform.RotateAround(transform.position, transform.up, 180f);
        }
    }
}