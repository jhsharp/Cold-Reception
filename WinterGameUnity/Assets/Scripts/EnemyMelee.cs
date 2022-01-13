using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMelee : EnemyBase
{
    public float meleeRange, meleeDelay;
    private float meleeDelayTimer = 0;
    private bool meleeActive = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        strafe();
        melee();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPos.position, meleeRange);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, engageRange);
    }

    private void melee()
    {
        if (attackTimer <= 0 && !meleeActive && Vector3.Distance(this.transform.position, player.transform.position) <= attackRange)
        {
            meleeDelayTimer = meleeDelay;
            meleeActive = true;
            /*animator.SetTrigger("Melee");*/
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
