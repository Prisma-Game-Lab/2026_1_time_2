using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// IA da Serpente: gerencia vida e dispara a sequência fixa de ataques
// (tornado -> mordida -> dash -> repete), ficando mais frenética
// (menos espera entre ataques + mais força) conforme a vida cai.
[RequireComponent(typeof(BossSnakeAttacks))]
public class BossSnakeAI : MonoBehaviour
{
    [Header("Vida")]
    public int maxHealth = 300;
    private int currentHealth;

    [Header("Tempo entre ataques (segundos)")]
    public float delayVidaAlta = 2.5f;   // > 66% de vida
    public float delayVidaMedia = 1.5f;  // 33% - 66% de vida
    public float delayVidaBaixa = 0.7f;  // < 33% de vida

    [Header("Multiplicador de força por fase")]
    // Multiplica dano e velocidade dos ataques conforme a vida cai
    public float forcaVidaAlta = 1f;
    public float forcaVidaMedia = 1.3f;
    public float forcaVidaBaixa = 1.6f;

    private BossSnakeAttacks attacks;
    private bool isDead = false;

    // Sequência fixa: 0 = tornado, 1 = mordida, 2 = dash
    private int attackIndex = 0;

    void Start()
    {
        currentHealth = maxHealth;
        attacks = GetComponent<BossSnakeAttacks>();
        StartCoroutine(AttackLoop());
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            isDead = true;
            Die();
        }
    }

    void Die()
    {
        StopAllCoroutines();
        Debug.Log("Serpente derrotada!");
        // Lugar pra disparar animação de morte, drop de itens, etc.
        Destroy(gameObject);
    }

    float GetHealthPercent()
    {
        return (float)currentHealth / maxHealth;
    }

    // Retorna o delay atual baseado na fase de vida
    float GetCurrentDelay()
    {
        float percent = GetHealthPercent();
        if (percent > 0.66f) return delayVidaAlta;
        if (percent > 0.33f) return delayVidaMedia;
        return delayVidaBaixa;
    }

    // Retorna o multiplicador de força atual baseado na fase de vida
    float GetCurrentForceMultiplier()
    {
        float percent = GetHealthPercent();
        if (percent > 0.66f) return forcaVidaAlta;
        if (percent > 0.33f) return forcaVidaMedia;
        return forcaVidaBaixa;
    }

    IEnumerator AttackLoop()
    {
        // Pequeno delay inicial antes do primeiro ataque
        yield return new WaitForSeconds(1f);

        while (!isDead)
        {
            // Espera enquanto o boss estiver atacando ou stunado
            yield return new WaitUntil(() => !attacks.IsAttacking() && !attacks.IsStunned());

            float forceMultiplier = GetCurrentForceMultiplier();

            switch (attackIndex)
            {
                case 0:
                    attacks.AttackTornado();
                    break;
                case 1:
                    attacks.AttackBite();
                    break;
                case 2:
                    attacks.AttackDashThrough(GetHealthPercent());
                    break;
            }

            // Avança para o próximo ataque da sequência fixa
            attackIndex = (attackIndex + 1) % 3;

            // Espera o ataque terminar antes de contar o delay
            yield return new WaitUntil(() => !attacks.IsAttacking());

            // Delay entre ataques, escalando com a vida (mais frenético = menor delay)
            yield return new WaitForSeconds(GetCurrentDelay());
        }
    }

}