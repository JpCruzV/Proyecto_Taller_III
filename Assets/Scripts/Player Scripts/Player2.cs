using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player2 : MonoBehaviour {


    [Header("Anims")]

    [SerializeField] Animator anim;



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

        movement.x = Input.GetAxisRaw("Horizontal2");


        //Double tap for running
        if ((Input.GetKeyDown(KeyCode.RightArrow) && facingRight) || (Input.GetKeyDown(KeyCode.LeftArrow) && !facingRight)) {

            backwards = false;

            if (ButtonCooler > 0 && ButtonCount == 1) {

                running = true;
            }
            else {

                ButtonCooler = 0.2f;
                ButtonCount++;
            }
        }
        else if (Input.GetKeyUp(KeyCode.RightArrow) || Input.GetKeyUp(KeyCode.LeftArrow)) {

            running = false;
        }


        //Moving Backwards and dashing
        if (Input.GetKeyDown(KeyCode.LeftArrow) && facingRight || Input.GetKeyDown(KeyCode.RightArrow) && !facingRight) {

            backwards = true;

            if (ButtonCooler > 0 && ButtonCount == 1) {

                BackDash();
            }
            else {

                ButtonCooler = 0.2f;
                ButtonCount++;
            }
        }


        //Jump
        if (Input.GetKey(KeyCode.UpArrow) && readyToJump && grounded) {

            readyToJump = false;
            Jump();

            Invoke(nameof(ResetJump), jumpCooldown);
        }


        //Crouch
        if (Input.GetKey(KeyCode.DownArrow) && grounded) {

            crouching = true;
            Crouch();
        }
        else if (Input.GetKeyUp(KeyCode.DownArrow)) {

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


        //Throw Fireball, Explosion Circle and Shotgun spell
        if (Input.GetMouseButton(0) && readyToThrow) {

            FireballThrow();
        }
        else if (Input.GetMouseButton(1) && readyToUse && grounded) {

            CreateExplosionCircle();
        }
        else if (Input.GetMouseButton(2) && readyToThrow) {

            DoShotgunSpell();
        }
    }



    #region Movement


    Rigidbody rb;


    [Header("Movement")]

    [SerializeField] float moveSpeed;
    [SerializeField] float backDashForce;
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
    float ButtonCooler = 0.5f;
    int ButtonCount = 0;



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



    void BackDash() {

        Vector3 backDashDir = transform.right * -backDashForce + transform.up * 2f;
        rb.AddForce(backDashDir, ForceMode.Impulse);
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
    #endregion



    #region Attacks


    [Header("Attacks references")]

    [SerializeField] Transform attackPoint;
    [SerializeField] Transform bottomAttackPoint;
    [SerializeField] Transform explosionCircleAttackPoint;
    [SerializeField] GameObject fireballPrefab;
    [SerializeField] GameObject explosionCirclePrefab;
    [SerializeField] GameObject shotgunSpellPrefab;


    [Header("Attacks variables")]

    [SerializeField] float fireballCD;
    [SerializeField] float fireballForce;
    [SerializeField] float fireballUpForce;

    [SerializeField] float circleCD;
    [SerializeField] float shotgunCD;
    bool readyToThrow = true;
    bool readyToUse = true;



    void FireballThrow() {

        readyToThrow = false;
        GameObject fireball = Instantiate(fireballPrefab, attackPoint);
        Rigidbody firaballRb = fireball.GetComponent<Rigidbody>();
        Vector3 forceToAdd;


        if (Input.GetKey(KeyCode.UpArrow)) {

            if (crouching) {

                fireball.transform.localScale = new Vector3(.5f, .5f, .5f);
            }

            forceToAdd = transform.up * fireballUpForce * 1.5f;
            firaballRb.AddForce(forceToAdd, ForceMode.Impulse);
        }
        else if (crouching) {

            fireball.transform.localScale = new Vector3(.5f, .5f, .5f);
            forceToAdd = transform.right * fireballForce * 1.5f;
            firaballRb.AddForce(forceToAdd, ForceMode.Impulse);
        }
        else {

            forceToAdd = transform.right * fireballForce + transform.up * fireballUpForce;
            firaballRb.AddForce(forceToAdd, ForceMode.Impulse);
        }

        Invoke(nameof(ResetThrow), fireballCD);
    }



    void CreateExplosionCircle() {

        readyToUse = false;
        GameObject explosionCircle = Instantiate(explosionCirclePrefab, explosionCircleAttackPoint);

        Invoke(nameof(ResetAbilityUse), circleCD);
    }



    void DoShotgunSpell() {

        readyToThrow = false;
        if (Input.GetKey(KeyCode.DownArrow) && !grounded) {

            GameObject shotgunSpell = Instantiate(shotgunSpellPrefab, bottomAttackPoint.position, Quaternion.Euler(0f, 0f, -90f), this.transform);
        }
        else {

            GameObject shotgunSpell = Instantiate(shotgunSpellPrefab, attackPoint);
        }

        Invoke(nameof(ResetThrow), shotgunCD);
    }



    void ResetThrow() {

        readyToThrow = true;
    }



    void ResetAbilityUse() {

        readyToUse = true;
    }
    #endregion
}