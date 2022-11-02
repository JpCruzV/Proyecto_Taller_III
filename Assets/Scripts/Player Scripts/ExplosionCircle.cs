using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionCircle : MonoBehaviour {


    [SerializeField] float delay = 2;
    [SerializeField] float radius = 3;
    [SerializeField] float force = 15;

    float countdown;
    bool hasExploded;


    private void Start() {

        countdown = delay;
        transform.parent = null;
    }


    private void Update() {

        countdown -= Time.deltaTime;

        if (countdown <= 0f && !hasExploded) {

            hasExploded = true;
            Explode();
        }
    }


    void Explode() {

        Collider[] collliders = Physics.OverlapSphere(transform.position, radius);

        foreach(Collider col in collliders) {

            Rigidbody rb = col.GetComponent<Rigidbody>();

            if (rb != null) {

                rb.AddForce(transform.up * force, ForceMode.Impulse);
            }
        }

        Destroy(gameObject);
    }
}
