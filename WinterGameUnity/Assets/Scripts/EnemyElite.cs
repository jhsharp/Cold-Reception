using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyElite : EnemyBase
{
    public float retreatRange, throwDelay, throwForceHor, throwForceVert;
    private float throwDelayTimer = 0;
    private bool throwActive = false;
    public GameObject bomb;

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
            throwBomb();
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(attackPos.position, retreatRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, engageRange);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position + new Vector3(-kickRange, 0, 0), kickRange);
    }

    override public void strafe()
    {
        if (Vector3.Distance(this.transform.position, player.transform.position) <= engageRange)
        {
            Vector3 moveChange = Vector3.zero;
            if (Vector3.Distance(this.transform.position, player.transform.position) > retreatRange) moveChange.x = Mathf.Sign(player.transform.position.x - this.transform.position.x) * moveSpeed * Time.deltaTime;
            else moveChange.x = Mathf.Sign(this.transform.position.x - player.transform.position.x) * retreatSpeed * Time.deltaTime;

            // Check for wall collisions
            if (collideWalls()) moveChange = Vector3.zero;
            transform.position += moveChange;

            // Animation
            if (moveChange != Vector3.zero) animator.SetBool("Walk", true);
            else animator.SetBool("Walk", false);

            // Sprite Flip
            if (player.transform.position.x < this.transform.position.x)
            {
                spriteRenderer.flipX = false;
                attackPos.transform.localPosition = attackBackward;
            }
            else
            {
                spriteRenderer.flipX = true;
                attackPos.transform.localPosition = attackForward;
            }
        }
        else animator.SetBool("Walk", false);
    }

    private void throwBomb()
    {
        if (attackTimer <= 0 && Vector3.Distance(this.transform.position, player.transform.position) <= engageRange)
        {
            throwDelayTimer = throwDelay;
            throwActive = true;
            attackTimer = attackCooldown;
            animator.SetTrigger("Attack");
        }

        if (throwActive)
        {
            if (throwDelayTimer <= 0)
            {
                GameObject newBomb = Instantiate(bomb, attackPos.position, transform.rotation);
                if (spriteRenderer.flipX) newBomb.GetComponent<Rigidbody2D>().AddForce(new Vector2(throwForceHor, throwForceVert), ForceMode2D.Impulse);
                else newBomb.GetComponent<Rigidbody2D>().AddForce(new Vector2(-throwForceHor, throwForceVert), ForceMode2D.Impulse);
                newBomb.GetComponent<BombScript>().damage = attackDamage;
                throwActive = false;
            }
            else throwDelayTimer -= Time.deltaTime;
        }
    }
}
