using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour {


    [SerializeField] int maxHealth;
    int currentHealth;


    [SerializeField] HealthBar healthBar;


    private void Start() {

        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
    }


    public void TakeDamage(int damage) {

        currentHealth -= damage;
        healthBar.SetHealth(currentHealth);
    }
}
