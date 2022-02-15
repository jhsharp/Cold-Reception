using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionScript : MonoBehaviour
{
    public float damageRange;
    public int damage;
    private List<GameObject> alreadyDamaged = new List<GameObject>();
    private GameObject player;
    private Animator animator;

    private void Start()
    {
        player = GameObject.Find("Player");
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!alreadyDamaged.Contains(player) && Vector3.Distance(this.transform.position, player.transform.position) <= damageRange)
        {
            player.GetComponent<PlayerController>().takeDamage(damage);
            alreadyDamaged.Add(player);
        }

        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, damageRange, player.GetComponent<PlayerController>().enemy);
        for (int i = 0; i < hitEnemies.Length; i++)
        {
            if (!alreadyDamaged.Contains(hitEnemies[i].gameObject))
            {
                hitEnemies[i].GetComponent<EnemyBase>().takeDamage(damage);
                alreadyDamaged.Add(hitEnemies[i].gameObject);
            }
        }

        Collider2D[] hitBarrels = Physics2D.OverlapCircleAll(transform.position, damageRange, player.GetComponent<PlayerController>().barrel);
        for (int i = 0; i < hitBarrels.Length; i++)
        {
            hitBarrels[i].GetComponent<BarrelScript>().explode();
        }

        if (animator.GetCurrentAnimatorStateInfo(0).length <= animator.GetCurrentAnimatorStateInfo(0).normalizedTime) Destroy(this.gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, damageRange);
    }
}
