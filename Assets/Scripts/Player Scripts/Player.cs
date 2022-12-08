using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour {


    public int id;

    Animator anim;



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

        running = anim.GetBool("isRunning");
        anim.SetBool("isCrouching", crouching);
        backwards = anim.GetBool("WalkingBackwards");
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
    bool crouching = false;
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

        if (knockbackCD <= 0) {


            if ((movement > 0 && facingRight) || (movement < 0 && !facingRight)) {

                anim.SetBool("WalkingBackwards", false);
                anim.SetBool("WalkingForward", true);
            }
            else if ((movement < 0 && facingRight) || (movement > 0 && !facingRight)) {

                anim.SetBool("WalkingForward", false);
                anim.SetBool("WalkingBackwards", true);
            }
            else {

                anim.SetBool("WalkingForward", false);
                anim.SetBool("WalkingBackwards", false);
            }


            if (crouching) {

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

        if (context.performed && backwards && readyToBackDash) {

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


        if (context.performed && grounded) {

            crouching = true;

            collider.height = crouchYScale;
            rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
            modelPos.position = new Vector3(modelPos.position.x, 0f, modelPos.position.z);
        }
    }



    public void StandingUp(InputAction.CallbackContext context) {

        if (context.performed) {

            crouching = false;
            pressedDown = false;

            collider.height = startYScale;
            modelPos.position = new Vector3(modelPos.position.x, modelStartPos.y, modelPos.position.z);
        }
    }



    public void Jump(InputAction.CallbackContext context) {

        if (context.performed && readyToJump && grounded && !crouching) {

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


    [Header("Attacks references")]

    [SerializeField] Transform attackPoint;
    [SerializeField] Transform bottomAttackPoint;
    [SerializeField] Transform explosionCircleAttackPoint;

    [SerializeField] GameObject fireballPrefab;
    [SerializeField] GameObject explosionCirclePrefab;
    [SerializeField] GameObject blastSpellPrefab;
    [SerializeField] GameObject laserPrefab;


    [Header("Attacks variables")]

    [SerializeField] float fireballCD;
    [SerializeField] float fireballForce;
    [SerializeField] float fireballUpForce;

    [SerializeField] float laserCD;
    [SerializeField] float laserForce;

    [SerializeField] float circleCD;
    [SerializeField] float blastCD;
    bool readyToThrow = true;
    bool readyToUse = true;

    bool pressedUp = false;
    bool pressedDown = false;



    public void FireballThrow(InputAction.CallbackContext context) {

        if (context.performed && readyToThrow && !shielding) {

            readyToThrow = false;
            GameObject fireball = Instantiate(fireballPrefab, attackPoint);
            fireball.GetComponent<Fireball>().fireballID = id;
            Rigidbody firaballRb = fireball.GetComponent<Rigidbody>();
            Vector3 forceToAdd;

            anim.SetTrigger("FireBall");


            if (pressedUp) {

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
    }



    public void SpecialAction(InputAction.CallbackContext context) {

        if (context.performed && grounded && readyToUse && !backwards) {

            readyToUse = false;
            GameObject explosionCircle = Instantiate(explosionCirclePrefab, explosionCircleAttackPoint);
            explosionCircle.GetComponent<ExplosionCircle>().circleID = id;

            anim.SetTrigger("EarthAttack");


            Invoke(nameof(ResetAbilityUse), circleCD);
        }
        else if (context.performed && grounded && backwards) {

            shielding = true;
            rb.isKinematic = true;
        }
        else if (context.performed && !grounded) {

            LaserBeam();
        }
        else {

            rb.isKinematic = false;
            shielding = false;
        }
    }



    void LaserBeam() {

        readyToUse = false;
        GameObject laser = Instantiate(laserPrefab, attackPoint);
        laser.GetComponent<Laser>().laserID = id;
        Rigidbody laserRB = laser.GetComponent<Rigidbody>();
        Vector3 forceToAdd;
        forceToAdd = transform.right * laserForce + transform.up * -laserForce;
        laserRB.AddForce(forceToAdd, ForceMode.Impulse);

        Invoke(nameof(ResetAbilityUse), laserCD);
    }



    public void BlastSpell(InputAction.CallbackContext context) {

        if (context.performed && pressedDown && !shielding && readyToThrow) {

            readyToThrow = false;

            GameObject shotgunSpell = Instantiate(blastSpellPrefab, bottomAttackPoint.position, Quaternion.Euler(0f, 0f, -90f), this.transform);
            shotgunSpell.GetComponent<ShotgunSpell>().blastID = id;
            shotgunSpell.GetComponent<ShotgunSpell>().touchedGround = true;

        }
        else if (readyToThrow && !shielding) {

            readyToThrow = false;

            GameObject shotgunSpell = Instantiate(blastSpellPrefab, attackPoint);
            shotgunSpell.GetComponent<ShotgunSpell>().blastID = id;

            anim.SetTrigger("BlastSpell");
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