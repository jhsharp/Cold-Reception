using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    [SerializeField] private float attackCooldown, meleeRange;
    [SerializeField] private int meleeDamage;
    [SerializeField] private GameObject snowball;
    [SerializeField] private float meleeDelay, shootDelay, deathDelay;
    private float attackTimer, meleeDelayTimer, shootDelayTimer, deathDelayTimer;
    private bool meleeActive, shootActive, deathActive = false;
    private Vector3 attackForward, attackBackward = Vector3.zero;

    [SerializeField] private int health;

    public GameManager gameMan;

    private void Awake()
    {
        controls = new PlayerControls();
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        gameMan = FindObjectOfType<GameManager>();

        attackForward.x = Mathf.Abs(attackPos.localPosition.x);
        attackBackward.x = -attackForward.x;
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
        if (!deathActive)
        {
            move();
            attack();
        }
        die();
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
        if (moveInput < 0)
        {
            spriteRenderer.flipX = true;
            attackPos.transform.localPosition = attackBackward;
        }
        else if (moveInput > 0)
        {
            spriteRenderer.flipX = false;
            attackPos.transform.localPosition = attackForward;
        }
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

    private void attack()
    {
        if (attackTimer <= 0)
        {
            if (controls.Player.Melee.ReadValue<float>() > 0)
            {
                meleeDelayTimer = meleeDelay;
                meleeActive = true;
                attackTimer = attackCooldown;
                animator.SetTrigger("Melee");
            }
            else if (controls.Player.Shoot.ReadValue<float>() > 0)
            {
                shootDelayTimer = shootDelay;
                shootActive = true;
                attackTimer = attackCooldown;
                animator.SetTrigger("Ranged");
            }
        }
        else attackTimer -= Time.deltaTime;

        if (meleeActive)
        {
            if (meleeDelayTimer <= 0)
            {
                Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPos.position, meleeRange, enemy);
                for (int i = 0; i < hitEnemies.Length; i++)
                {
                    hitEnemies[i].GetComponent<EnemyBase>().takeDamage(meleeDamage);
                }
                meleeActive = false;
            }
            else meleeDelayTimer -= Time.deltaTime;
        }

        if (shootActive)
        {
            if (shootDelayTimer <= 0)
            {
                GameObject newBall = Instantiate(snowball, attackPos.position, transform.rotation);
                if (spriteRenderer.flipX) newBall.GetComponent<SnowballScript>().moveSpeed *= -1;
                shootActive = false;
            }
            else shootDelayTimer -= Time.deltaTime;
        }
    }

    public void takeDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            animator.SetTrigger("Dead");
            deathDelayTimer = deathDelay;
            deathActive = true;
        }
        else animator.SetTrigger("Hurt");
    }

    public void die()
    {
        if (deathActive)
        {
            if (deathDelayTimer > 0) deathDelayTimer -= Time.deltaTime;
            else gameMan.loadScene(SceneManager.GetActiveScene().name);
        }
    }
}
