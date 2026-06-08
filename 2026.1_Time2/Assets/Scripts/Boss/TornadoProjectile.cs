using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class TornadoProjectile : MonoBehaviour
{
    [HideInInspector] public float speed;
    [HideInInspector] public float damage;
    public float lifetime = 3f;
    private float arenaLeft = -15.05f;
    private float arenaRight = 14.95f;
    private float arenaBottom = -10f;
    private float arenaTop = 10f;
    private Vector2 moveDirection;
    public void Init(Vector2 direction, float spd, float dmg)
    {
        moveDirection = direction.normalized;
        speed = spd;
        damage = dmg;
        Destroy(gameObject, lifetime);
    }
    void Update()
    {
        transform.position += (Vector3)(moveDirection * speed * Time.deltaTime);
        Ricochet();
    }
    void Ricochet()
    {
        Vector3 pos = transform.position;
        if (pos.x <= arenaLeft || pos.x >= arenaRight)
        {
            moveDirection.x *= -1;
            pos.x = Mathf.Clamp(pos.x, arenaLeft, arenaRight);
            transform.position = pos;
        }
        if (pos.y <= arenaBottom || pos.y >= arenaTop)
        {
            moveDirection.y *= -1;
            pos.y = Mathf.Clamp(pos.y, arenaBottom, arenaTop);
            transform.position = pos;
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Player player = other.GetComponent<Player>();
            if (player != null)
            {
                player.TakeDamage((int)damage);
            }
        }
    }
}