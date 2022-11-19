using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {


    public int id;


    [Header("Anims")]

    [SerializeField] Animator anim;



    private void Start() {

        rb = GetComponent<Rigidbody>();
        startYScale = transform.localScale.y;
        rb.freezeRotation = true;

        InputSetUp();
        SetHp();
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



    #region Inputs


    [Header("Inputs Config")]

    [SerializeField] KeyCode leftKey;
    [SerializeField] KeyCode rightKey;
    [SerializeField] KeyCode jumpKey;
    [SerializeField] KeyCode crouchKey;

    [SerializeField] KeyCode fireballKey;
    [SerializeField] KeyCode blastKey;
    [SerializeField] KeyCode circleKey;

    void MyInput() {


        //Double tap for running
        if ((Input.GetKeyDown(rightKey) && facingRight) || (Input.GetKeyDown(leftKey) && !facingRight)) {

            backwards = false;

            if (ButtonCooler > 0 && ButtonCount == 1) {

                running = true;
            }
            else {

                ButtonCooler = 0.2f;
                ButtonCount++;
            }
        }
        else if (Input.GetKeyUp(rightKey) || Input.GetKeyUp(leftKey)) {

            running = false;
        }


        //Moving Backwards and dashing
        if (Input.GetKeyDown(leftKey) && facingRight || Input.GetKeyDown(rightKey) && !facingRight) {

            backwards = true;

            if (ButtonCooler > 0 && ButtonCount == 1 && grounded) {

                BackDash();
            }
            else {

                ButtonCooler = 0.2f;
                ButtonCount++;
            }
        }
        

        //Jump
        if (Input.GetKey(jumpKey) && readyToJump && grounded) {

            readyToJump = false;
            Jump();

            Invoke(nameof(ResetJump), jumpCooldown);
        }


        //Crouch
        if (Input.GetKey(crouchKey) && grounded) {

            crouching = true;
            Crouch();
        }
        else if (Input.GetKeyUp(crouchKey)) {

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
        if (id == 1) {

            if (Input.GetKey(fireballKey) && readyToThrow && !shielding) {

                FireballThrow();
            }
            else if (Input.GetKey(circleKey) && readyToUse) {

                if (grounded)
                {
                    if (backwards)
                    {

                        shielding = true;
                        rb.isKinematic = true;
                    }
                    else
                    {

                        CreateExplosionCircle();
                    }
                }
                else {

                    LaserBeam();
                }
            }
            else if (Input.GetKeyDown(blastKey) && readyToThrow && !shielding) {

                BlastSpell();
            }
            else if (Input.GetKeyUp(circleKey)) {

                shielding = false;
                rb.isKinematic = false;
            }
        }
        else {

            if (Input.GetMouseButton(0) && readyToThrow && !shielding) {

                FireballThrow();
            }
            else if (Input.GetMouseButton(1) && readyToUse) {

                if (grounded) {

                    if (backwards) {

                        shielding = true;
                        rb.isKinematic = true;
                    }
                    else {

                        CreateExplosionCircle();
                    }
                }
                else {

                    LaserBeam();
                }
            }
            else if (Input.GetMouseButton(2) && readyToThrow && !shielding) {

                BlastSpell();
            }
            else if (Input.GetMouseButtonUp(1)) {

                shielding = false;
                rb.isKinematic = false;
            }
        }
    }



    void InputSetUp() {

        if (id == 1) {

            leftKey = KeyCode.A;
            rightKey = KeyCode.D;
            jumpKey = KeyCode.W;
            crouchKey = KeyCode.S;

            fireballKey = KeyCode.U;
            blastKey = KeyCode.I;
            circleKey = KeyCode.O;
        }
        else if (id == 2) {

            leftKey = KeyCode.LeftArrow;
            rightKey = KeyCode.RightArrow;
            jumpKey = KeyCode.UpArrow;
            crouchKey = KeyCode.DownArrow;

            fireballKey = KeyCode.None;
            blastKey = KeyCode.None;
            circleKey = KeyCode.None;
        }
    }
    #endregion



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
    float ButtonCooler  = 0.5f;
    int ButtonCount = 0;



    void MovePlayer() {


        if (Input.GetKey(rightKey)) {

            movement.x = 1;
        }
        else if (Input.GetKey(leftKey)) {

            movement.x = -1;
        }
        else {

            movement.x = 0;
        }
        if (Input.GetKeyUp(rightKey) || Input.GetKeyUp(leftKey)){

            movement.x = 0;
        }

        if (knockbackCD <= 0) {

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
    }



    void BackDash() {

        if (facingRight)
            transform.position = new Vector3(transform.position.x - 2, transform.position.y, transform.position.z);
        else
            transform.position = new Vector3(transform.position.x + 2, transform.position.y, transform.position.z);

        //Vector3 backDashDir = transform.right * -backDashForce + transform.up * 2f;
        //rb.AddForce(backDashDir, ForceMode.Impulse);
    }



    void Crouch() {

        if (crouching) {

            transform.localScale = new Vector3(transform.localScale.x, crouchYScale, transform.localScale.z);
            rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
        }
        else if (!crouching) {

            transform.localScale = new Vector3(transform.localScale.x, startYScale, transform.localScale.z);
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



    void FireballThrow() {

        readyToThrow = false;
        GameObject fireball = Instantiate(fireballPrefab, attackPoint);
        fireball.GetComponent<Fireball>().fireballID = id;
        Rigidbody firaballRb = fireball.GetComponent<Rigidbody>();
        Vector3 forceToAdd;


        if (Input.GetKey(jumpKey)) {

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
        explosionCircle.GetComponent<ExplosionCircle>().circleID = id;


        Invoke(nameof(ResetAbilityUse), circleCD);
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



    void BlastSpell() {

        readyToThrow = false;
        if (Input.GetKey(crouchKey) && !grounded){

            GameObject shotgunSpell = Instantiate(blastSpellPrefab, bottomAttackPoint.position, Quaternion.Euler(0f, 0f, -90f), this.transform);
            shotgunSpell.GetComponent<ShotgunSpell>().blastID = id;
            shotgunSpell.GetComponent<ShotgunSpell>().touchedGround = true;
        }
        else {

            GameObject shotgunSpell = Instantiate(blastSpellPrefab, attackPoint);
            shotgunSpell.GetComponent<ShotgunSpell>().blastID = id;
        }

        Invoke(nameof(ResetThrow), blastCD);
    }



    void ResetThrow()
    {
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