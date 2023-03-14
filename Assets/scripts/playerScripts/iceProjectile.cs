using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class iceProjectile : projectileScript
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        
        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<PlayerController>().takeIceDamage(damage, owner);

        }
        transform.position = new Vector3(transform.position.x, transform.position.y + 100, transform.position.z);
        Destroy(gameObject);
    }

}