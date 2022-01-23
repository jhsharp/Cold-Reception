using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    [HideInInspector] public Collider2D col;
    [HideInInspector] public Animator animator;
    [HideInInspector] public SpriteRenderer spriteRenderer;
    public LayerMask ground;
    public Transform attackPos;
    [HideInInspector] public Vector3 attackForward, attackBackward = Vector3.zero;

    public int baseHealth;
    public float moveSpeed, retreatSpeed, engageRange, attackRange, attackCooldown;
    public int attackDamage;
    [HideInInspector] public int health;
    [HideInInspector] public float attackTimer = 0;
    [HideInInspector] public GameObject player;

    internal void Start()
    {
        health = baseHealth;
        player = GameObject.Find("Player");
        col = GetComponent<Collider2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        attackForward.x = Mathf.Abs(attackPos.localPosition.x);
        attackBackward.x = -attackForward.x;
    }

    internal void Update()
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
    }

    virtual public void takeDamage(int damage)
    {
        health -= damage;
        if (health <= 0) Destroy(this.gameObject);
    }

    public void strafe()
    {
        if (Vector3.Distance(this.transform.position, player.transform.position) <= engageRange)
        {
            Vector3 moveChange = Vector3.zero;
            if (attackTimer <= 0) moveChange.x = Mathf.Sign(player.transform.position.x - this.transform.position.x) * moveSpeed * Time.deltaTime;
            else moveChange.x = Mathf.Sign(this.transform.position.x - player.transform.position.x) * retreatSpeed * Time.deltaTime;
            transform.position += moveChange;
            // Check for wall collisions
            if (collideWalls()) transform.position -= moveChange;
            moveChange = Vector3.zero;
           /* // Animation
            animator.SetBool("Walk", true);
            else animator.SetBool("Walk", false);*/
            // Sprite Flip
            if (moveChange.x < 0)
            {
                spriteRenderer.flipX = true;
                attackPos.transform.localPosition = attackBackward;
            }
            else if (moveChange.x > 0)
            {
                spriteRenderer.flipX = false;
                attackPos.transform.localPosition = attackForward;
            }
        }
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
}
