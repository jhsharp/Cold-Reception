using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombScript : MonoBehaviour
{
    public int damage;
    public GameObject explosion;

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.layer != LayerMask.NameToLayer("Enemy"))
        {
            GameObject explode = Instantiate(explosion, transform.position, transform.rotation);
            explode.GetComponent<ExplosionScript>().damage = damage;
            Destroy(this.gameObject);
        }
    }
}
