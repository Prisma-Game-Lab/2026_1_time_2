using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro; 

[RequireComponent(typeof(BossSnakeAttacks))]
public class BossSnakeAI : MonoBehaviour
{
    public enum AttackType { Tornado, Bite, DashThrough }

    [Header("Vida")]
    public int maxHealth = 300;
    private int currentHealth;

    [Header("Tempo entre ataques (segundos)")]
    public float delayVidaAlta = 2.5f;
    public float delayVidaMedia = 1.5f;
    public float delayVidaBaixa = 0.7f;

    [Header("Multiplicador de força por fase")]
    public float forcaVidaAlta = 1f;
    public float forcaVidaMedia = 1.3f;
    public float forcaVidaBaixa = 1.6f;

    [Header("Feedback de Hit")]
    public Color corHit = Color.red;
    public float duracaoFlashHit = 0.1f;
    private SpriteRenderer spriteRenderer;
    private Color corOriginal;
    private Coroutine hitFlashCoroutine;

    [Header("Tela de Vitória")]
    public GameObject textoVitoria;
    public float delayAntesDoMenu = 5f;
    public string nomeCenaMenu = "Menu";

    private BossSnakeAttacks attacks;
    private bool isDead = false;
    private int consecutiveTornados = 0;
    private int consecutiveBites = 0;
    private int mergulhoCooldown = 0;

    void Start()
    {
        currentHealth = maxHealth;
        attacks = GetComponent<BossSnakeAttacks>();

        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
            corOriginal = spriteRenderer.color;

        if (textoVitoria != null)
            textoVitoria.SetActive(false);

        StartCoroutine(AttackLoop());
    }

    public void TakeDamage(int damage, AudioClip somDeAcerto = null)
    {
        if (isDead) return;
        currentHealth -= damage;

        if (!attacks.IsStunned())
        {
            if (hitFlashCoroutine != null) StopCoroutine(hitFlashCoroutine);
            hitFlashCoroutine = StartCoroutine(FlashHit());

            if (somDeAcerto != null && AudioManager.Instance != null)
                AudioManager.Instance.PlaySFX(somDeAcerto);
        }

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            isDead = true;
            Die();
        }
    }

    IEnumerator FlashHit()
    {
        if (spriteRenderer == null) yield break;

        spriteRenderer.color = corHit;
        yield return new WaitForSeconds(duracaoFlashHit);

        if (!attacks.IsStunned())
            spriteRenderer.color = corOriginal;
    }

    void Die()
    {
        StopAllCoroutines();
        attacks.StopAllCoroutines();

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlaySerpenteMorte();

        if (spriteRenderer != null)
            spriteRenderer.enabled = false;

        Collider2D[] colliders = GetComponentsInChildren<Collider2D>();
        foreach (Collider2D col in colliders)
            col.enabled = false;

        Debug.Log("Serpente derrotada!");
        StartCoroutine(VitoriaCoroutine());
    }

    IEnumerator VitoriaCoroutine()
    {
        if (textoVitoria != null)
            textoVitoria.SetActive(true);

        yield return new WaitForSeconds(delayAntesDoMenu);

        Destroy(gameObject);
        SceneManager.LoadScene(nomeCenaMenu);
    }

    float GetHealthPercent()
    {
        return (float)currentHealth / maxHealth;
    }

    float GetCurrentDelay()
    {
        float percent = GetHealthPercent();
        if (percent > 0.66f) return delayVidaAlta;
        if (percent > 0.33f) return delayVidaMedia;
        return delayVidaBaixa;
    }

    AttackType GetNextAttack()
    {
        List<AttackType> validAttacks = new List<AttackType>();
        if (consecutiveTornados < 2)
            validAttacks.Add(AttackType.Tornado);
        if (consecutiveBites < 3)
            validAttacks.Add(AttackType.Bite);
        if (mergulhoCooldown <= 0)
            validAttacks.Add(AttackType.DashThrough);
        if (validAttacks.Count == 0) validAttacks.Add(AttackType.Bite);

        AttackType chosenAttack = validAttacks[Random.Range(0, validAttacks.Count)];
        if (chosenAttack == AttackType.Tornado)
        {
            consecutiveTornados++;
            consecutiveBites = 0;
            if (mergulhoCooldown > 0) mergulhoCooldown--;
        }
        else if (chosenAttack == AttackType.Bite)
        {
            consecutiveBites++;
            consecutiveTornados = 0;
            if (mergulhoCooldown > 0) mergulhoCooldown--;
        }
        else if (chosenAttack == AttackType.DashThrough)
        {
            consecutiveTornados = 0;
            consecutiveBites = 0;
            mergulhoCooldown = 2;
        }
        return chosenAttack;
    }

    IEnumerator AttackLoop()
    {
        yield return new WaitForSeconds(1f);
        while (!isDead)
        {
            yield return new WaitUntil(() => !attacks.IsAttacking() && !attacks.IsStunned());
            AttackType proximoAtaque = GetNextAttack();
            switch (proximoAtaque)
            {
                case AttackType.Tornado:
                    attacks.AttackTornado();
                    break;
                case AttackType.Bite:
                    attacks.AttackBite();
                    break;
                case AttackType.DashThrough:
                    attacks.AttackDashThrough(GetHealthPercent());
                    break;
            }
            yield return new WaitUntil(() => !attacks.IsAttacking());
            yield return new WaitForSeconds(GetCurrentDelay());
        }
    }
}