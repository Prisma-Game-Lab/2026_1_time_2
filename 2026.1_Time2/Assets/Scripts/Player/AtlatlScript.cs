using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AtlatlScript : MonoBehaviour
{
    public float speed = 4.5f;
    public float attackDamage = 10f;
    public float tempoGrudado = 1.5f;
    public float tempoNoChao = 8.0f;
    private Vector3 moveDirection;
    private bool estaVoando = true;
    private bool noChao = false;
    private Rigidbody2D rb;
    private Collider2D meuCollider;
    private WeaponThrow armaOrigem;
    private Transform inimigoAlvo;
    private Vector3 offsetGrudado;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        meuCollider = GetComponent<Collider2D>();
        if (rb != null)
        {
            rb.gravityScale = 0f;
            rb.bodyType = RigidbodyType2D.Kinematic;
        }
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayAtlatlVoando();
        Camera mainCamera = Camera.main;
        if (mainCamera != null)
        {
            Vector3 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = 0;
            moveDirection = (mousePosition - transform.position).normalized;
            float angle = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle - 90);
        }

        //Invoke("CairNoChao", 3.0f); // Se voar por 3s sem acertar nada, cai no chão
    }

    public void ConfigurarOrigem(WeaponThrow origem)
    {
        armaOrigem = origem;
    }

    void Update()
    {
        if (estaVoando)
        {
            transform.position += moveDirection * speed * Time.deltaTime;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (estaVoando && !noChao && other.CompareTag("Enemy"))
        {
            estaVoando = false;

            BossSnakeAI boss = other.GetComponentInParent<BossSnakeAI>();
            if (boss != null)
            {
                AudioClip som = AudioManager.Instance != null ? AudioManager.Instance.somAtlatlAcerto : null;
                boss.TakeDamage((int)attackDamage, som);
            }

            StartCoroutine(GrudarNoInimigo(other.transform));
            return;
        }

        if (estaVoando && (other.CompareTag("Obstaculo") || other.CompareTag("Cenario")))
        {
            CairNoChao();
            return;
        }

        if (noChao && other.CompareTag("Player"))
        {
            ColetarPeloPlayer();
        }
    }

    IEnumerator GrudarNoInimigo(Transform inimigo)
    {
        estaVoando = false;
        inimigoAlvo = inimigo;
        offsetGrudado = transform.position - inimigo.position;
        if (rb != null) rb.velocity = Vector2.zero;

        float tempoDecorrido = 0f;
        while (tempoDecorrido < tempoGrudado)
        {
            if (inimigoAlvo == null)
            {
                break;
            }
            transform.position = inimigoAlvo.position + offsetGrudado;
            tempoDecorrido += Time.deltaTime;
            yield return null;
        }
        inimigoAlvo = null;
        CairNoChao();
    }

    void CairNoChao()
    {
        estaVoando = false;
        noChao = true;
        if (meuCollider != null) meuCollider.isTrigger = true;
        if (rb != null) rb.velocity = Vector2.zero;
        Invoke("DestruirPorTempo", tempoNoChao);
    }

    void ColetarPeloPlayer()
    {
        if (armaOrigem != null)
        {
            armaOrigem.RecuperarMunicaoDoChao();
        }
        Destroy(gameObject);
    }

    void DestruirPorTempo()
    {
        Destroy(gameObject);
    }
}