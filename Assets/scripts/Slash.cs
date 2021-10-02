using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slash : MonoBehaviour
{
    public float speed = 20f;
    public Rigidbody2D rb;

    private void Start()
    {
        rb.velocity = -transform.right * speed;
    }

    private void OnTriggerEnter2D(Collider2D hitInfo)
    {
        BaseEnemy enemy = hitInfo.GetComponent<BaseEnemy>();
        player damage = GameObject.Find("ª±®a").GetComponent<player>();

        if(enemy != null)
        {
            enemy.enemyHurt(damage.attack);
        }
        Destroy(gameObject);
    }
}
