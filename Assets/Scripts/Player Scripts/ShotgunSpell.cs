using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotgunSpell : MonoBehaviour {


    [SerializeField] float force;


    private void Start() {

        Destroy(gameObject, 1.5f);
    }


    private void OnTriggerEnter(Collider other) {

        if (other.tag != "Floor") {

            Rigidbody rb = other.GetComponent<Rigidbody>();

            if (rb != null) {

                rb.AddForce(transform.right * force + transform.up * 2, ForceMode.Impulse);
            }
        }
        else if (other.tag == "Floor") {

            Destroy(gameObject);
        }
    }
}
