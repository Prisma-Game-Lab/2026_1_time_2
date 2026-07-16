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

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        meuCollider = GetComponent<Collider2D>();

        Camera mainCamera = Camera.main;
        if (mainCamera != null)
        {
            Vector3 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = 0;
            moveDirection = (mousePosition - transform.position).normalized;

            // Rotacionar o sprite para apontar para a direção do vôo
            float angle = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle - 90);
        }
    }

    // Método para a arma se registrar como "dona" deste projétil
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
        // 1. Acertou o Inimigo enquanto voava
        if (estaVoando && other.CompareTag("Enemy"))
        {
            Debug.Log("Inimigo atingido! Dano: " + attackDamage);
            // aplicar dano no script do inimigo, ex: other.GetComponent<Enemy>().TakeDamage(attackDamage);
            //
            //

            StartCoroutine(GrudarNoInimigo(other.transform));
        }

        // 2. O Player passou por cima dele no chão para coletar
        if (noChao && other.CompareTag("Player"))
        {
            ColetarPeloPlayer();
        }
    }

    IEnumerator GrudarNoInimigo(Transform inimigo)
    {
        estaVoando = false;

        // Gruda o atlatl no transform do inimigo para ele andar junto
        transform.SetParent(inimigo);

        // Aguarda o tempo espetado no inimigo
        yield return new WaitForSeconds(tempoGrudado);

        // Desgruda do inimigo para cair no chão
        transform.SetParent(null);
        CairNoChao();
    }

    void CairNoChao()
    {
        noChao = true;

        // Ajusta as colisões para virar um "coletável" do chão
        if (meuCollider != null) meuCollider.isTrigger = true;

        // Se tiver física de Rigidbody, pode pará-lo no chão
        if (rb != null) rb.velocity = Vector2.zero;

        // Inicia a contagem regressiva para sumir da cena se o player ignorar
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