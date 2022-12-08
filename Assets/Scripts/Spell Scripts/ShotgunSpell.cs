using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotgunSpell : MonoBehaviour {


    [SerializeField] float force;
    [SerializeField] int damage;

    [HideInInspector] public int blastID;
    [HideInInspector] public bool touchedGround = false;


    private void Start() {

        Destroy(gameObject, .5f);
    }


    private void OnTriggerEnter(Collider other) {

        if (other.tag != "Floor") {

            Rigidbody rb = other.GetComponent<Rigidbody>();

            if (rb != null && other.GetComponent<Player>() != null && other.GetComponent<Player>().id != blastID) {

                rb.AddForce(transform.right * force * 5 + transform.up * 2, ForceMode.Impulse);
                other.GetComponent<Player>().TakeDamage(damage);
            }
        }
        else if (other.tag == "Floor" && touchedGround) {

            Destroy(gameObject);
        }
    }
}
