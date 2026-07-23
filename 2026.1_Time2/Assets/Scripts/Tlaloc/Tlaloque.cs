using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tlaloque : MonoBehaviour
{
    [SerializeField] private Transform player;
    public PlayerAttack bastaoScript;
    [SerializeField] private float knockbackForce = 10f;
    [SerializeField] private float knockbackDuration = 0.2f;

    private Rigidbody2D rb;
    private bool isKnockbacked = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (!isKnockbacked)
        {
            Move();
        }
    }

    public void Move()
    {
        if (player == null) return;

        Vector2 direcao = (player.position - transform.position).normalized;
        float velocidade = 2f;
        transform.position += (Vector3)(direcao * velocidade * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("mataCavalo") && bastaoScript.IsWeaponAttacking())
        {
            if (isKnockbacked) return;

            if (bastaoScript.IsWeaponHardAttacking())
            {
                Destroy(gameObject);
                return;
            }

            Vector2 knockbackDirection = (transform.position - collision.transform.position).normalized;
            StartCoroutine(ApplyKnockbackRoutine(knockbackDirection, knockbackForce));
        }
    }

    private IEnumerator ApplyKnockbackRoutine(Vector2 direction, float force)
    {
        isKnockbacked = true;

        rb.velocity = Vector2.zero;
        rb.AddForce(direction * force, ForceMode2D.Impulse);

        yield return new WaitForSeconds(0.25f);
        rb.velocity = Vector2.zero;

        isKnockbacked = false;
    }
}