using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrelScript : MonoBehaviour
{
    public GameObject explosion;
    public int damage;
    public float rollSpeed;
    [HideInInspector] public bool rolling = false;
    private int direction;
    private Animator animator;
    private Collider2D col;

    private void Start()
    {
        animator = GetComponent<Animator>();
        col = GetComponent<Collider2D>();
    }

    private void Update()
    {
        if (rolling)
        {
            transform.position += new Vector3(rollSpeed * direction * Time.deltaTime, 0, 0);
            if (collideWalls()) explode();

        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (rolling) explode();
    }

    public void push(int dir)
    {
        rolling = true;
        direction = dir;
        transform.position += new Vector3(rollSpeed * direction * Time.deltaTime, 0, 0);
        if (direction == 1) animator.SetTrigger("Right");
        else animator.SetTrigger("Left");
    }

    public void explode()
    {
        GameObject explode = Instantiate(explosion, transform.position, transform.rotation);
        explode.GetComponent<ExplosionScript>().damage = damage;
        Destroy(this.gameObject);
    }

    private bool collideWalls()
    {
        Vector2 topLeft = transform.position;
        topLeft.x -= col.bounds.extents.x + 0.001f;
        topLeft.y += col.bounds.extents.y - 0.1f;

        Vector2 bottomRight = transform.position;
        bottomRight.x += col.bounds.extents.x + 0.001f;
        bottomRight.y -= col.bounds.extents.y - 0.1f;

        return Physics2D.OverlapArea(topLeft, bottomRight, GameObject.Find("Player").GetComponent<PlayerController>().ground);
    }
}
