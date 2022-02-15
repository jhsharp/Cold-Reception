using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    [HideInInspector] public Collider2D col;
    [HideInInspector] public Animator animator;
    [HideInInspector] public SpriteRenderer spriteRenderer;
    [HideInInspector] public Rigidbody2D rb;
    public LayerMask ground;
    public Transform attackPos;
    [HideInInspector] public Vector3 attackForward, attackBackward = Vector3.zero;

    public int baseHealth;
    public float moveSpeed, retreatSpeed, engageRange, attackRange, attackCooldown, kickRange, kickDelay, deathDelay;
    public int attackDamage;
    [HideInInspector] public int health;
    [HideInInspector] public float attackTimer = 0, kickTimer = 0, deathDelayTimer = 0;
    [HideInInspector] public bool deathActive = false;
    [HideInInspector] public GameObject player;

    internal void Start()
    {
        health = baseHealth;
        player = GameObject.Find("Player");
        col = GetComponent<Collider2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();

        attackForward.x = Mathf.Abs(attackPos.localPosition.x);
        attackBackward.x = -attackForward.x;
    }

    internal void Update()
    {
        if (kickTimer > 0)
        {
            kick();
            return;
        }
        if (!deathActive)
        {
            if (attackTimer > 0) attackTimer -= Time.deltaTime;
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
            kick();
        }
        else die();
    }

    virtual public void takeDamage(int damage)
    {
        health -= damage;
        animator.SetTrigger("Hurt");
        if (health <= 0)
        {
            deathActive = true;
            deathDelayTimer = deathDelay;
            col.enabled = false;
            rb.gravityScale = 0;

            animator.SetTrigger("Dead");
        }
    }

    virtual public void strafe()
    {
        if (Vector3.Distance(this.transform.position, player.transform.position) <= engageRange)
        {
            Vector3 moveChange = Vector3.zero;
            if (attackTimer <= 0) moveChange.x = Mathf.Sign(player.transform.position.x - this.transform.position.x) * moveSpeed * Time.deltaTime;
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

    public void kick()
    {
        if (kickTimer <= 0 && Vector3.Distance(this.transform.position, player.transform.position) <= engageRange)
        {
            Collider2D[] kickBarrels;
            if (spriteRenderer.flipX) kickBarrels = Physics2D.OverlapCircleAll(transform.position + new Vector3(kickRange, 0, 0), kickRange, player.GetComponent<PlayerController>().barrel);
            else kickBarrels = Physics2D.OverlapCircleAll(transform.position + new Vector3(-kickRange, 0, 0), kickRange, player.GetComponent<PlayerController>().barrel);
            for (int i = 0; i < kickBarrels.Length; i++)
            {
                if (!kickBarrels[i].GetComponent<BarrelScript>().rolling)
                {
                    if (!spriteRenderer.flipX) kickBarrels[i].GetComponent<BarrelScript>().push(-1);
                    else kickBarrels[i].GetComponent<BarrelScript>().push(1);
                    kickTimer = kickDelay;
                    animator.SetTrigger("Kick");
                }
            }
        }
        else if (kickTimer > 0) kickTimer -= Time.deltaTime;
    }    

    public bool collideWalls()
    {
        Vector2 topLeft = transform.position;
        topLeft.x -= col.bounds.extents.x;
        topLeft.y += col.bounds.extents.y - 0.1f;

        Vector2 bottomRight = transform.position;
        bottomRight.x += col.bounds.extents.x;
        bottomRight.y -= col.bounds.extents.y - 0.1f;

        return Physics2D.OverlapArea(topLeft, bottomRight, ground);
    }

    public void die()
    {
        if (deathActive)
        {
            if (deathDelayTimer > 0) deathDelayTimer -= Time.deltaTime;
            else Destroy(this.gameObject);
        }
    }
}
