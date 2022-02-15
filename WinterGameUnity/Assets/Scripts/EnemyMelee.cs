using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMelee : EnemyBase
{
    public float meleeRange, meleeDelay;
    private float meleeDelayTimer = 0;
    private bool meleeActive = false;

    private void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    private void Update()
    {
        if (kickTimer > 0)
        {
            kick();
            return;
        }
        base.Update();
        if (!deathActive)
        {
            strafe();
            melee();
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPos.position, meleeRange);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, engageRange);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position + new Vector3(-kickRange, 0, 0), kickRange);
    }

    private void melee()
    {
        if (attackTimer <= 0 && !meleeActive && Mathf.Abs(this.transform.position.x - player.transform.position.x) <= attackRange)
        {
            meleeDelayTimer = meleeDelay;
            meleeActive = true;
            animator.SetTrigger("Attack");
        }

        if (meleeActive)
        {
            if (meleeDelayTimer <= 0)
            {
                if (Vector3.Distance(player.transform.position, attackPos.position) <= meleeRange) player.GetComponent<PlayerController>().takeDamage(attackDamage);
                meleeActive = false;
                attackTimer = attackCooldown;
            }
            else meleeDelayTimer -= Time.deltaTime;
        }
    }
}
