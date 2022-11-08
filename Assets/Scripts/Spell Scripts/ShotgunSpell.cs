using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotgunSpell : MonoBehaviour {


    [SerializeField] float force;
    [SerializeField] int damage;

    [HideInInspector] public int blastID;


    private void Start() {

        Destroy(gameObject, 1.5f);
    }


    private void OnTriggerEnter(Collider other) {

        if (other.tag != "Floor") {

            Rigidbody rb = other.GetComponent<Rigidbody>();

            if (rb != null && other.GetComponent<Player>() != null && other.GetComponent<Player>().id != blastID) {

                rb.AddForce(transform.right * force + transform.up * 2, ForceMode.Impulse);
                other.GetComponent<Player>().TakeDamage(damage);
            }
        }
        else if (other.tag == "Floor") {

            Destroy(gameObject);
        }
    }
}
