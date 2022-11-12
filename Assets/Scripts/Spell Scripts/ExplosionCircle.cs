using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class ExplosionCircle : MonoBehaviour {

    [SerializeField] ParticleSystem psAborb;
    [SerializeField] VisualEffect _VFX_Ice;

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
        _VFX_Ice.SetBool("Active", false);
    }


    private void Update() {

        pos1 = new Vector3(transform.position.x, transform.position.y + 4, transform.position.z);
        pos2 = new Vector3(transform.position.x, transform.position.y + 1, transform.position.z);

        countdown -= Time.deltaTime;
        if (countdown <= 1) {

            _VFX_Ice.SetBool("Active", true);
        }

        if (countdown <= .2f && !hasExploded) {

            hasExploded = true;
            Explode();
        }
        else if (countdown <= 0f) {

            Destroy(gameObject);
        }
    }


    void Explode() {

        Collider[] collliders = Physics.OverlapCapsule(pos2, pos1, radius);

        foreach(Collider col in collliders) {

            Rigidbody rb = col.GetComponent<Rigidbody>();

            if (rb != null && col.GetComponent<Player>() != null) {

                if (col.GetComponent<Player>().id != circleID) {

                    rb.AddForce(transform.up * force + transform.right * 1.2f, ForceMode.Impulse);
                    col.GetComponent<Player>().TakeDamage(damage);
                }
            }
        }
    }


    private void OnDrawGizmos() {

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(pos1, radius);
        Gizmos.DrawWireSphere(pos2, radius);
    }
}
