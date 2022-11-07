using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireball : MonoBehaviour {


    Rigidbody rb;
    float startCooldown = .5f;
    [HideInInspector] public int fireballID;

    [SerializeField] int damage;


    private void Awake() {

        rb = GetComponent<Rigidbody>();
        Destroy(gameObject, 3);
        transform.parent = null;
    }


    private void FixedUpdate() {

        startCooldown -= Time.fixedDeltaTime;

        if (rb.velocity.y == 0 && rb.velocity.x == 0 && startCooldown <= 0) {

            Destroy(gameObject);
        }
    }


    private void OnCollisionEnter(Collision collision) {

        if (collision.gameObject.tag != "Floor") {

            if (collision.gameObject.GetComponent<Health>() != null && collision.gameObject.GetComponent<Player>().id != fireballID)
            {
                collision.gameObject.GetComponent<Health>().TakeDamage(damage);
            }
            Destroy(gameObject);
        }
    }
}