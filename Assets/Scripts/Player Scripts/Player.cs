using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour {


    public int id;
    Animator anim;

    [HideInInspector] public bool pressedUp = false;
    [HideInInspector] public bool pressedDown = false;

    [HideInInspector] public bool disableMove = false;



    private void Start() {

        GetComponent<PlayerInput>().SwitchCurrentControlScheme(Keyboard.current, Mouse.current);

        rb = GetComponent<Rigidbody>();
        collider = GetComponent<CapsuleCollider>();
        startYScale = collider.height;
        rb.freezeRotation = true;
        modelStartPos = modelPos.position;

        SetHp();

        anim = GetComponentInChildren<Animator>();
    }



    private void Update() {

        Flip();
        Vector3 velocity = rb.velocity.normalized;

        backwards = anim.GetBool("Backwards");
        running = anim.GetBool("isRunning");
        anim.SetBool("isCrouching", crouching);
        anim.SetBool("isGrounded", grounded);
        anim.SetFloat("yVelocity", velocity.y);
    }



    private void FixedUpdate() {

        Movement();

        // ground check
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, floor);
    }



    #region Movement


    Rigidbody rb;
    CapsuleCollider collider;


    [Header("Movement")]

    [SerializeField] float moveSpeed;
    [SerializeField] GameObject otherPlayer;
    float movement;
    bool running;
    bool facingRight = true;
    bool backwards;


    [Header("BackDash")]

    [SerializeField] float backDashDist;
    [SerializeField] float backDashCD;
    bool readyToBackDash = true;


    [Header("Jumping")]

    [SerializeField] float jumpForce;
    [SerializeField] float jumpCooldown;
    bool readyToJump = true;


    [Header("Crouching")]

    [SerializeField] float crouchSpeed;
    [SerializeField] float crouchYScale;
    float startYScale;
    [HideInInspector] public bool crouching = false;
    [SerializeField] Transform modelPos;
    Vector3 modelStartPos; 


    [Header("GroundCheck")]

    [SerializeField] float playerHeight;
    [SerializeField] LayerMask floor;
    bool grounded;



    public void MovementValue(InputAction.CallbackContext context) {

        movement = context.ReadValue<float>();
    }



    void Movement() {

        if (knockbackCD <= 0 && !disableMove) {


            if ((movement > 0 && facingRight) || (movement < 0 && !facingRight)) {

                anim.SetBool("Backwards", false);
                anim.SetBool("Forward", true);
            }
            else if ((movement < 0 && facingRight) || (movement > 0 && !facingRight)) {

                anim.SetBool("Forward", false);
                anim.SetBool("Backwards", true);
            }
            else {

                anim.SetBool("Forward", false);
                anim.SetBool("Backwards", false);
            }


            if (crouching && !running) {

                rb.velocity = new Vector3(movement * crouchSpeed, rb.velocity.y, 0);
            }
            else if (running && !crouching && !backwards) {

                rb.velocity = new Vector3(movement * moveSpeed * 2, rb.velocity.y, 0);
            }
            else if (backwards && !crouching) {

                rb.velocity = new Vector3(movement * moveSpeed * .5f, rb.velocity.y, 0);
            }
            else {

                rb.velocity = new Vector3(movement * moveSpeed, rb.velocity.y, 0);
            }
        }
    }



    public void BackDashAndRunning(InputAction.CallbackContext context) {

        if (context.performed && backwards && readyToBackDash && !disableMove) {

            readyToBackDash = false;


            if (facingRight)
                transform.position = new Vector3(transform.position.x - backDashDist, transform.position.y, transform.position.z);
            else
                transform.position = new Vector3(transform.position.x + backDashDist, transform.position.y, transform.position.z);


            Invoke(nameof(ResetBackDash), backDashCD);
        }
        else if (context.performed) {

            anim.SetBool("isRunning", true);
        }
        else {

            anim.SetBool("isRunning", false);
        }
    }



    public void Crouch(InputAction.CallbackContext context) {

        if (context.performed)
            pressedDown = true;


        if (context.performed && grounded && !disableMove) {

            crouching = true;

            collider.height = crouchYScale;
            rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
            modelPos.position = new Vector3(modelPos.position.x, 0f, modelPos.position.z);
        }
    }



    public void StandingUp(InputAction.CallbackContext context) {

        if (context.performed && grounded) {

            crouching = false;
            pressedDown = false;

            collider.height = startYScale;
            modelPos.position = new Vector3(modelPos.position.x, -.1f, modelPos.position.z);
        }
    }



    public void Jump(InputAction.CallbackContext context) {

        if (context.performed && readyToJump && grounded && !crouching && !disableMove) {

            readyToJump = false;

            rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
            rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);

            Invoke(nameof(ResetJump), jumpCooldown);
        }

        if (context.performed)
            pressedUp = true;
        else
            pressedUp = false;
    }



    void ResetBackDash() {

        readyToBackDash = true;
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



    #region Abilities


    [Header("Attacks variables")]

    [SerializeField] float fireballCD;
    [SerializeField] float laserCD;
    [SerializeField] float circleCD;
    [SerializeField] float blastCD;

    bool readyToThrow = true;
    bool readyToUse = true;



    public void FireballThrow(InputAction.CallbackContext context) {

        if (context.performed && readyToThrow && !shielding && !disableMove) {

            readyToThrow = false;

            if (pressedUp) {

                anim.SetTrigger("UpwardsFireball");
            }
            else if (crouching && !pressedUp) {

                anim.SetTrigger("CrouchingFireball");
            }
            else {

                anim.SetTrigger("Fireball");
            }

            Invoke(nameof(ResetThrow), fireballCD);
        }
    }



    public void SpecialAction(InputAction.CallbackContext context) {

        if (context.performed && grounded && readyToUse && !backwards && !disableMove) {

            readyToUse = false;
            anim.SetTrigger("Ice");

            Invoke(nameof(ResetAbilityUse), circleCD);
        }
        else if (context.performed && !grounded) {

            readyToUse = false;
            anim.SetTrigger("Laser");

            Invoke(nameof(ResetAbilityUse), laserCD);
        }
        else if (context.performed && grounded && backwards) {

            shielding = true;
            rb.isKinematic = true;
        }
        else {

            rb.isKinematic = false;
            shielding = false;
        }
    }



    public void BlastSpell(InputAction.CallbackContext context) {

        if (context.performed && !shielding && readyToThrow && !disableMove) {

            readyToThrow = false;

            if (pressedDown && !grounded) {

                //falta implementacion
                anim.SetTrigger("DownwardsBlast");
            }
            else {

                anim.SetTrigger("Blast");
            }
        }

        Invoke(nameof(ResetThrow), blastCD);
    }



    void ResetThrow() {

        readyToThrow = true;
    }



    void ResetAbilityUse() {

        readyToUse = true;
    }
    #endregion



    #region Hp


    [Header("Hp")]

    [SerializeField] HealthBar healthBar;
    [SerializeField] int maxHealth;
    int currentHealth;
    bool shielding = false;

    float knockbackCD = 0;



    void SetHp() {

        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
    }



    public void TakeDamage(int damage) {

        if (!shielding) {

            currentHealth -= damage;
            healthBar.SetHealth(currentHealth);
            StartCoroutine(Knockback());
        }
    }



    IEnumerator Knockback() {

        knockbackCD = .2f;
        while (knockbackCD  > 0) {

            knockbackCD -= Time.deltaTime;
            yield return null;
        }
    }
    #endregion
}