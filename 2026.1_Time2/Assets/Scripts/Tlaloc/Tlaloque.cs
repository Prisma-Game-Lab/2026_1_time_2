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
    private Tlaloc tlalocBoss;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // 1. Busca o Player pela Tag se não tiver sido configurado no Inspector
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
            }
        }

        // 2. Busca o script do bastão/arma na cena
        if (bastaoScript == null)
        {
            // Tenta achar o componente PlayerAttack no objeto com tag "mataCavalo" ou na cena
            GameObject bastaoObj = GameObject.FindGameObjectWithTag("mataCavalo");
            if (bastaoObj != null)
            {
                bastaoScript = bastaoObj.GetComponent<PlayerAttack>();

                // Se o script estiver no objeto Pai do colisor
                if (bastaoScript == null)
                {
                    bastaoScript = bastaoObj.GetComponentInParent<PlayerAttack>();
                }
            }

            // Caso não encontre pela tag, busca em qualquer lugar da cena
            if (bastaoScript == null)
            {
                bastaoScript = FindObjectOfType<PlayerAttack>();
            }
        }
    }

    public void SetBossReference(Tlaloc boss)
    {
        tlalocBoss = boss;
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
        // Se colidiu com a arma
        if (collision.gameObject.CompareTag("mataCavalo"))
        {
            // Valida se temos a referência do script da arma
            if (bastaoScript != null && bastaoScript.IsWeaponAttacking())
            {
                if (isKnockbacked) return;

                // Se for ataque pesado: notifica o boss e destrói
                if (bastaoScript.IsWeaponHardAttacking())
                {
                    Die();
                    return;
                }

                // Se for ataque leve: aplica knockback
                Vector2 knockbackDirection = (transform.position - collision.transform.position).normalized;
                StartCoroutine(ApplyKnockbackRoutine(knockbackDirection, knockbackForce));
            }
        }
    }

    private void Die()
    {
        if (tlalocBoss != null)
        {
            tlalocBoss.NotificarMorteTlaloquinho();
        }
        Destroy(gameObject);
    }

    private IEnumerator ApplyKnockbackRoutine(Vector2 direction, float force)
    {
        isKnockbacked = true;

        rb.velocity = Vector2.zero;
        rb.AddForce(direction * force, ForceMode2D.Impulse);

        yield return new WaitForSeconds(knockbackDuration);

        rb.velocity = Vector2.zero;
        isKnockbacked = false;
    }
}