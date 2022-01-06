using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    private PlayerControls controls;
    private Rigidbody2D rb;
    private Collider2D col;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    [SerializeField] private LayerMask ground;
    [SerializeField] private LayerMask enemy;
    [SerializeField] private Transform attackPos;

    private float moveInput;
    private Vector3 moveChange;
    [SerializeField] private float moveSpeed, jumpSpeed;
    [SerializeField] private float meleeCooldown, meleeRange;
    [SerializeField] private int meleeDamage;
    private float meleeTimer;

    private void Awake()
    {
        controls = new PlayerControls();
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnEnable()
    {
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Disable();
    }

    void Start()
    {
        controls.Player.Jump.performed += _ => jump();
    }

    void Update()
    {
        move();
        melee();
        if (isGrounded()) animator.SetBool("Grounded", true);
        else animator.SetBool("Grounded", false);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPos.position, meleeRange);
    }

    private void move()
    {
        // Read input
        moveInput = controls.Player.Move.ReadValue<float>();
        // Move the player
        moveChange = Vector3.zero;
        moveChange.x = moveInput * moveSpeed * Time.deltaTime;
        transform.position += moveChange;
        // Check for wall collisions
        if (collideWalls()) transform.position -= moveChange;
        // Animation
        if (moveInput != 0) animator.SetBool("Walk", true);
        else animator.SetBool("Walk", false);
        // Sprite Flip
        if (moveInput < 0) spriteRenderer.flipX = true;
        else if (moveInput > 0) spriteRenderer.flipX = false;
    }

    private void jump()
    {
        if (isGrounded())
        {
            rb.AddForce(new Vector2(0, jumpSpeed), ForceMode2D.Impulse);
            animator.SetTrigger("Jump");
        }
    }

    private bool isGrounded()
    {
        Vector2 topLeft = transform.position;
        topLeft.x -= col.bounds.extents.x;
        topLeft.y += col.bounds.extents.y;

        Vector2 bottomRight = transform.position;
        bottomRight.x += col.bounds.extents.x;
        bottomRight.y -= col.bounds.extents.y;

        return Physics2D.OverlapArea(topLeft, bottomRight, ground);
    }

    private bool collideWalls()
    {
        Vector2 topLeft = transform.position;
        topLeft.x -= col.bounds.extents.x;
        topLeft.y += col.bounds.extents.y - 0.1f;

        Vector2 bottomRight = transform.position;
        bottomRight.x += col.bounds.extents.x;
        bottomRight.y -= col.bounds.extents.y - 0.1f;

        return Physics2D.OverlapArea(topLeft, bottomRight, ground);
    }

    private void melee()
    {
        if (meleeTimer <= 0)
        {
            if (controls.Player.Melee.ReadValue<float>() > 0)
            {
                Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPos.position, meleeRange, enemy);
                for (int i = 0; i < hitEnemies.Length; i++)
                {
                    hitEnemies[i].GetComponent<EnemyBase>().takeDamage(meleeDamage);
                }
                meleeTimer = meleeCooldown;
                animator.SetTrigger("Melee");
            }
        }
        else meleeTimer -= Time.deltaTime;
    }
}