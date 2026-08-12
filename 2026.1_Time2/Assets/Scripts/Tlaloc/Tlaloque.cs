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

        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
            }
        }

        BuscarReferenciaBastao();
    }

    private void BuscarReferenciaBastao()
    {
        if (bastaoScript != null) return;

        GameObject bastaoObj = GameObject.FindGameObjectWithTag("mataCavalo");
        if (bastaoObj != null)
        {
            bastaoScript = bastaoObj.GetComponent<PlayerAttack>();
            if (bastaoScript == null)
            {
                bastaoScript = bastaoObj.GetComponentInParent<PlayerAttack>();
            }
        }

        if (bastaoScript == null)
        {
            bastaoScript = FindObjectOfType<PlayerAttack>();
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
        if (collision.gameObject.CompareTag("mataCavalo"))
        {
            if (bastaoScript == null)
            {
                BuscarReferenciaBastao();
            }

            if (bastaoScript == null)
            {
                Debug.LogWarning("O Tlaloque colidiu com a arma, mas não encontrou o script PlayerAttack na cena!");
                return;
            }

            if (bastaoScript.IsWeaponAttacking())
            {
                if (bastaoScript.IsWeaponHardAttacking())
                {
                    Die();
                    return;
                }

                if (!isKnockbacked)
                {
                    Vector2 knockbackDirection = (transform.position - collision.transform.position).normalized;
                    StartCoroutine(ApplyKnockbackRoutine(knockbackDirection, knockbackForce));
                }
            }
        }
    }

    private void Die()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayTlaloqueMorte();
        }

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