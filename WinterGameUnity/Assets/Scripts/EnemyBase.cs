using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    [SerializeField] private int baseHealth;
    private int health;

    void Start()
    {
        health = baseHealth;   
    }

    private void Update()
    {
        if (health <= 0) Destroy(this.gameObject);
    }

    public void takeDamage(int damage)
    {
        health -= damage;
    }
}
