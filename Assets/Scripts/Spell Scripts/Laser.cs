using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour {


    Rigidbody rb;
    float startCooldown = .5f;
    bool hasbounced = false;
    [HideInInspector] public int laserID;

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

        if (!hasbounced) {

            hasbounced = true;
        }
        else if (hasbounced) {

            Destroy(gameObject);
        }

        if (collision.gameObject.tag != "Floor") {

            if (collision.gameObject.GetComponent<Player>() != null && collision.gameObject.GetComponent<Player>().id != laserID) {

                collision.gameObject.GetComponent<Player>().TakeDamage(damage);
            }
            Destroy(gameObject);
        }
    }
}
