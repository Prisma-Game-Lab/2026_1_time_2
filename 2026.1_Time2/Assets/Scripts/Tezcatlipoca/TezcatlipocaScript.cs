using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TezcatlipocaScript : MonoBehaviour
{
    public int maxHealth = 300;
    [SerializeField] private float currentHealth;
    private bool isDead = false;
    public float tempoIniciarBoss = 5f;
    public PlayerAttack bastaoScript;

    [Header("Tela de Vitória")]
    [SerializeField] private GameObject textoVitoria;
    [SerializeField] private CanvasGroup canvasGroupVitoria;
    [SerializeField] private float duracaoFade = 1.5f;
    [SerializeField] private float delayAntesDoMenu = 2.0f;
    [SerializeField] private string nomeCenaMenu = "Menu";

    [Header("Piscar ao tomar dano")]
    public float flashInterval = 0.1f;
    private SpriteRenderer spriteRenderer;
    private Color corOriginal;

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;

        if (textoVitoria != null)
            textoVitoria.SetActive(false);

        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            corOriginal = spriteRenderer.color;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;
        currentHealth -= damage;
        StartCoroutine(FlashRed());
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
        if (AudioManager.Instance != null) AudioManager.Instance.PlayTlalocMorte();
        Debug.Log("Tezcatlipoca derrotado!");

        if (spriteRenderer != null)
            spriteRenderer.enabled = false;

        Collider2D[] colliders = GetComponentsInChildren<Collider2D>();
        foreach (Collider2D col in colliders)
            col.enabled = false;

        StartCoroutine(VitoriaCoroutine());
    }

    IEnumerator VitoriaCoroutine()
    {
        if (textoVitoria != null)
        {
            textoVitoria.SetActive(true);

            if (canvasGroupVitoria != null)
            {
                canvasGroupVitoria.alpha = 0f;

                float tempoPassado = 0f;
                while (tempoPassado < duracaoFade)
                {
                    tempoPassado += Time.deltaTime;
                    canvasGroupVitoria.alpha = Mathf.Clamp01(tempoPassado / duracaoFade);
                    yield return null;
                }

                canvasGroupVitoria.alpha = 1f;
            }
        }

        yield return new WaitForSeconds(delayAntesDoMenu);

        Destroy(gameObject);
        Progresso.tezcaDerrotado = true;
        SceneManager.LoadScene(nomeCenaMenu);
    }

    IEnumerator FlashRed()
    {
        if (spriteRenderer != null)
        {

            spriteRenderer.color = Color.red;
            yield return new WaitForSeconds(flashInterval);

            spriteRenderer.color = corOriginal;
            yield return new WaitForSeconds(flashInterval);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.gameObject.CompareTag("atlatl"))
        {
            Debug.Log("Dano de Atlatl");
            TakeDamage(5);
        }
        if (collision.gameObject.CompareTag("mataCavalo"))
        {
            if (bastaoScript.IsWeaponAttacking())
            {
                Debug.Log("Dano de mataCavalo");
                TakeDamage(2.5f);
            }
        }
    }
}
