using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRanged : EnemyBase
{
    public GameObject fireball;
    public float shootDelay;
    private float shootDelayTimer = 0;
    private bool shootActive = false;

    private void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    private void Update()
    {
        base.Update();
        shoot();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, engageRange);
    }

    private void shoot()
    {
        if (attackTimer <= 0 && Vector3.Distance(this.transform.position, player.transform.position) <= engageRange)
        {
            shootDelayTimer = shootDelay;
            shootActive = true;
            attackTimer = attackCooldown;
            //animator.SetTrigger("Ranged");
        }

        if (shootActive)
        {
            if (shootDelayTimer <= 0)
            {
                GameObject newBall = Instantiate(fireball, attackPos.position, transform.rotation);
                if (!spriteRenderer.flipX) newBall.GetComponent<FireballScript>().moveSpeed *= -1;
                newBall.GetComponent<FireballScript>().damage = attackDamage;
                shootActive = false;
            }
            else shootDelayTimer -= Time.deltaTime;
        }
    }
}
