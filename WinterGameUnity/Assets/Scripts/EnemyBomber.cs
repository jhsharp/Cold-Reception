using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBomber : EnemyBase
{
    public GameObject explosion;
    public float explodeDelay;
    private bool explodeActive = false;

    private void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    private void Update()
    {
        base.Update();
        if (!deathActive)
        {
            strafe();
            explode();
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, engageRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosion.GetComponent<ExplosionScript>().damageRange);
    }

    private void explode()
    {
        if (!explodeActive && Vector3.Distance(this.transform.position, player.transform.position) <= attackRange)
        {
            explodeActive = true;
        }

        if (explodeActive)
        {
            if (explodeDelay <= 0)
            {
                GameObject explode = Instantiate(explosion, attackPos.position, attackPos.rotation);
                explode.GetComponent<ExplosionScript>().damage = attackDamage;
                Destroy(this.gameObject);
            }
            else explodeDelay -= Time.deltaTime;
        }
    }

    override public void takeDamage(int damage)
    {
        health -= damage;
        animator.SetTrigger("Hurt");
        if (health <= 0)
        {
            GameObject explode = Instantiate(explosion, attackPos.position, attackPos.rotation);
            explode.GetComponent<ExplosionScript>().damage = attackDamage;
            Destroy(this.gameObject);
        }
    }
}
