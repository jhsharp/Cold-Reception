using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnowballScript : MonoBehaviour
{
    public float moveSpeed, fallTime, fallSpeed;
    public int damage;
    private float fallTimer = 0;
    private Rigidbody2D rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // Move Sideways
        Vector3 moveChange = Vector3.zero;
        moveChange.x = moveSpeed * Time.deltaTime;
        transform.position += moveChange;
        // Fall
        if (fallTimer < fallTime) fallTimer += Time.deltaTime;
        else
        {
            rb.gravityScale = fallSpeed;
        }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.GetComponent<EnemyBase>() != null)
        {
            col.GetComponent<EnemyBase>().takeDamage(damage);
        }
        if (col.gameObject.layer != LayerMask.NameToLayer("Player")) Destroy(this.gameObject);
    }
}
