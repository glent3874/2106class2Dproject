using UnityEngine;

public class Slash : MonoBehaviour
{
    [Header("´î½w³t«×"), Range(0, 1)]
    public float slowspeed;

    public float speed = 20f;
    public Rigidbody2D rb;

    private void Start()
    {
        rb.velocity = -transform.right * speed;
    }

    private void Update()
    {
        if (rb.velocity.x > 0)
        {
            if (rb.velocity.x < 1.2f)
            {
                Destroy(gameObject);
            }
        }
        if (rb.velocity.x < 0)
        {
            if (rb.velocity.x > -1.2f)
            {
                Destroy(gameObject);
            }
        }
    }

    private void FixedUpdate()
    {
        rb.velocity = rb.velocity * slowspeed;
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
