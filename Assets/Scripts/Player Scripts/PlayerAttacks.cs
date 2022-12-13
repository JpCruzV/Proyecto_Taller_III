using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttacks : MonoBehaviour
{

    Player player;

    [Header("Attacks references")]

    [SerializeField] Transform attackPoint;
    [SerializeField] Transform blastPoint;
    [SerializeField] Transform bottomAttackPoint;
    [SerializeField] Transform explosionCircleAttackPoint;

    [SerializeField] GameObject fireballPrefab;
    [SerializeField] GameObject explosionCirclePrefab;
    [SerializeField] GameObject blastSpellPrefab;
    [SerializeField] GameObject laserPrefab;


    [Header("Attacks variables")]

    [SerializeField] float fireballForce;
    [SerializeField] float fireballUpForce;

    [SerializeField] float laserForce;


    private void Start() {

        player = GetComponentInParent<Player>();
    }


    public void StandingFireballSpell() { 

        GameObject fireball = Instantiate(fireballPrefab, attackPoint);
        fireball.GetComponent<Fireball>().fireballID = player.id;
        Rigidbody firaballRb = fireball.GetComponent<Rigidbody>();
        Vector3 forceToAdd;
        forceToAdd = transform.forward * fireballForce + transform.up * fireballUpForce;
        firaballRb.AddForce(forceToAdd, ForceMode.Impulse);
    }



    public void PressedUpFireballSpell() {

        GameObject fireball = Instantiate(fireballPrefab, attackPoint);
        fireball.GetComponent<Fireball>().fireballID = player.id;
        Rigidbody firaballRb = fireball.GetComponent<Rigidbody>();
        Vector3 forceToAdd;

        if (player.crouching) {

            fireball.transform.localScale = new Vector3(.5f, .5f, .5f);
        }

        forceToAdd = transform.up * fireballUpForce * 1.5f;
        firaballRb.AddForce(forceToAdd, ForceMode.Impulse);
    }



    public void CrouchingFireballSpell() { 

        GameObject fireball = Instantiate(fireballPrefab, attackPoint);
        fireball.GetComponent<Fireball>().fireballID = player.id;
        Rigidbody firaballRb = fireball.GetComponent<Rigidbody>();
        Vector3 forceToAdd;
        fireball.transform.localScale = new Vector3(.5f, .5f, .5f);
        forceToAdd = transform.forward * fireballForce * 1.5f;
        firaballRb.AddForce(forceToAdd, ForceMode.Impulse);
    }



    public void IceSpell() {

        GameObject explosionCircle = Instantiate(explosionCirclePrefab, explosionCircleAttackPoint);
        explosionCircle.GetComponent<ExplosionCircle>().circleID = player.id;
    }



    public void LaserBeam() {

        GameObject laser = Instantiate(laserPrefab, attackPoint);
        laser.GetComponent<Laser>().laserID = player.id;
        Rigidbody laserRB = laser.GetComponent<Rigidbody>();
        Vector3 forceToAdd;
        forceToAdd = transform.right * laserForce + transform.up * -laserForce;
        laserRB.AddForce(forceToAdd, ForceMode.Impulse);
    }


    public void BlastSpell() {

        GameObject shotgunSpell = Instantiate(blastSpellPrefab, blastPoint);
        shotgunSpell.GetComponent<ShotgunSpell>().blastID = player.id;
    }



    public void PressedDownBlastSpell() {

        GameObject shotgunSpell = Instantiate(blastSpellPrefab, bottomAttackPoint.position, Quaternion.Euler(0f, 0f, -90f), this.transform);
        shotgunSpell.GetComponent<ShotgunSpell>().blastID = player.id;
        shotgunSpell.GetComponent<ShotgunSpell>().touchedGround = true;
    }



    public void DisableMovement() {

        player.disableMove = true;
    }



    public void EnableMovement() {

        player.disableMove = false;
    }
}
