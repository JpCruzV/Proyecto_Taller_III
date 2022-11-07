using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionCircle : MonoBehaviour {


    [SerializeField] float delay;
    [SerializeField] float radius;
    [SerializeField] float force;
    [SerializeField] int damage;
    [HideInInspector] public int circleID;

    float countdown;
    bool hasExploded;
    Vector3 pos1;
    Vector3 pos2;


    private void Start() {

        countdown = delay;
        transform.parent = null;
    }


    private void Update() {

        pos1 = new Vector3(transform.position.x, transform.position.y + 4, transform.position.z);
        pos2 = new Vector3(transform.position.x, transform.position.y + 1, transform.position.z);

        countdown -= Time.deltaTime;

        if (countdown <= 0f && !hasExploded) {

            hasExploded = true;
            Explode();
        }
    }


    void Explode() {

        Collider[] collliders = Physics.OverlapCapsule(pos2, pos1, radius);

        foreach(Collider col in collliders) {

            Rigidbody rb = col.GetComponent<Rigidbody>();

            if (rb != null && col.GetComponent<Health>() != null) {

                if (col.GetComponent<Player>().id != circleID) {

                    rb.AddForce(transform.up * force + transform.right * 1.2f, ForceMode.Impulse);
                    col.GetComponent<Health>().TakeDamage(damage);
                }
            }
        }

        Destroy(gameObject);
    }


    private void OnDrawGizmos() {

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(pos1, radius);
        Gizmos.DrawWireSphere(pos2, radius);
    }
}
