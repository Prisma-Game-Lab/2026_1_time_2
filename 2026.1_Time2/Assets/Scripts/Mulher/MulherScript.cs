using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MulherScript : MonoBehaviour
{
    public int maxHealth = 300;
    [SerializeField] private float currentHealth;
    private bool isDead = false;
    private bool canMove = true;
    public float tempoIniciarBoss = 2.0f;

    public PlayerAttack bastaoScript;

    [Header("Configurações do Ataque Laser (Raio Lunar)")]
    [SerializeField] private int quantidadeDeLasers = 3;
    [SerializeField] private float tempoDeAvisoLaser = 1.0f;
    [SerializeField] private float intervaloEntreLasers = 2f;
    [SerializeField] private float DanoLaser = 1f;
    private bool laserAtualLockado = false;
    [SerializeField] private GameObject pivotLaser;
    [SerializeField] private SpriteRenderer spriteLaser;
    [SerializeField] private Collider2D colliderDoLaser;

    [Header("Configurações do Ataque de Choro")]
    [SerializeField] private float tempoDeChoro = 10.0f;
    [SerializeField] private float forçaDoChoro = 3f;
    [SerializeField] private float CooldownPeixes = 1f;
    [SerializeField] private GameObject piranhas;
    [SerializeField] private Collider2D colliderPiranhas;
    [SerializeField] private GameObject peixePrefab;

    [Header("Configurações do Ataque Pirueta")]
    [SerializeField] private float tempoDePirueta = 5.0f;
    [SerializeField] private Collider2D colliderPirueta;
    [SerializeField] private Collider2D[] collidersParaDesativar;
    private bool forcarHitboxDesligada = false;
    private Collider2D[] collidersDoCorpo;

    [Header("Limites da Arena")]
    public float arenaLeft = -15.05f;
    public float arenaRight = 14.95f;
    public float arenaBottom = -10f;
    public float arenaTop = 10f;

    [Header("Configurações do player")]
    [SerializeField] private Transform player;
    [SerializeField] private Rigidbody2D playerRb;
    [SerializeField] private Player scriptDeMovimento;

    [Header("Piscar ao tomar dano")]
    public float flashInterval = 0.1f;
    private SpriteRenderer spriteRenderer;
    private Color corOriginal;

    [Header("Animação")]
    public Animator animMulher;
    public Animator animLaser;

    void Start()
    {
        //startCoroutine(ChooseAttack(tempoIniciarBoss));
        animMulher = GetComponent<Animator>();
        currentHealth = maxHealth;
        pivotLaser.SetActive(false);
        piranhas.SetActive(false);
        if (colliderDoLaser != null) colliderDoLaser.enabled = false;
        if (colliderPirueta != null) colliderPirueta.enabled = false;

        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null) corOriginal = spriteRenderer.color;

        collidersDoCorpo = GetComponentsInChildren<Collider2D>(true)
            .Where(c => c != colliderPirueta
                    && c != colliderDoLaser
                    && c != colliderPiranhas)
            .ToArray();

        if (AudioManager.Instance != null)
            AudioManager.Instance.TocarMusica(AudioManager.Instance.musicaFaseMulher);
    }

    void Update()
    {
        if (player != null && pivotLaser != null && !laserAtualLockado)
        {
            Vector3 direcao = player.position - pivotLaser.transform.position;
            float angulo = Mathf.Atan2(direcao.y, direcao.x) * Mathf.Rad2Deg + 100f;
            pivotLaser.transform.rotation = Quaternion.Euler(0f, 0f, angulo);
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            StartCoroutine(LaserAttack());
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            StartCoroutine(ChoroAttack());
        }
        if (Input.GetKeyDown(KeyCode.I))
        {
            StartCoroutine(PiruetaAttack());
        }

        if (canMove && player != null)
        {
            Move();
        }

        if (forcarHitboxDesligada)
        {
            foreach (Collider2D collider in collidersParaDesativar)
            {
                if (collider != null) collider.enabled = false;
            }
        }
    }

    public void Move()
    {
        Vector2 direcao = (player.position - transform.position).normalized;
        float velocidade = 2f;
        transform.position += (Vector3)(direcao * velocidade * Time.deltaTime);
    }

    public void TakeDamage(float damage)
    {
        if (isDead || forcarHitboxDesligada) return;
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

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayMulherMorte();

        Debug.Log("Mulher derrotada!");
        Destroy(gameObject);
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

    IEnumerator ChooseAttack(float delay)
    {
        yield return new WaitForSeconds(delay);

        Debug.Log("Iniciando ataques da Mulher");

        while (player != null)
        {
            int randomAttack = Random.Range(0, 3);
            switch (randomAttack)
            {
                case 0:
                    StartCoroutine(ChoroAttack());
                    break;
                case 1:
                    StartCoroutine(LaserAttack());
                    break;
                case 2:
                    StartCoroutine(PiruetaAttack());
                    break;
            }
            yield return new WaitForSeconds(10f);
        }
    }

    IEnumerator LaserAttack()
    {
        canMove = false;
        for (int i = 0; i < quantidadeDeLasers; i++)
        {
            animMulher.SetTrigger("AttackLaser");
            animLaser.SetTrigger("LaserPreparando");
            pivotLaser.SetActive(true);
            yield return new WaitForSeconds(tempoDeAvisoLaser - 0.5f);
            laserAtualLockado = true;
            yield return new WaitForSeconds(0.5f);

            spriteLaser.color = Color.red;
            colliderDoLaser.enabled = true;

            yield return new WaitForSeconds(0.5f);

            spriteLaser.color = Color.white;
            colliderDoLaser.enabled = false;

            laserAtualLockado = false;
            pivotLaser.SetActive(false);
            animMulher.SetTrigger("AttackLaserParando");
            yield return new WaitForSeconds(intervaloEntreLasers);
        }
        canMove = true;
        yield return new WaitForSeconds(5.0f);
    }

    IEnumerator ChoroAttack()
    {
        animMulher.SetTrigger("AttackChoroPreparando");
        canMove = false;
        transform.position = new Vector3(-15f, 0f, 0f);

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayMulherGrito();

        float tempoDecorrido = 0f;
        float tempoParaSpawnarPeixes = 0f;

        piranhas.SetActive(true);

        while (tempoDecorrido < tempoDeChoro)
        {
            if (tempoParaSpawnarPeixes >= CooldownPeixes)
            {
                SpawnarPeixes();
                tempoParaSpawnarPeixes = 0f;
            }

            if (scriptDeMovimento != null)
            {
                scriptDeMovimento.forcaExterna = Vector2.right * forçaDoChoro;
            }

            tempoParaSpawnarPeixes += Time.deltaTime;
            tempoDecorrido += Time.deltaTime;
            yield return null;
        }

        if (scriptDeMovimento != null)
        {
            scriptDeMovimento.forcaExterna = Vector2.zero;
        }
        piranhas.SetActive(false);
        canMove = true;
        animMulher.SetTrigger("AttackChoroParando");
    }

    void SpawnarPeixes()
    {
        var spawnPos = new Vector3(-10f, Random.Range(-15f, 15f), 0f);

        GameObject peixe = Instantiate(peixePrefab, spawnPos, Quaternion.identity);

        peixe.GetComponent<FishScript>().SetPlayer(player, playerRb);
    }

    IEnumerator PiruetaAttack()
    {
        canMove = false;
        animMulher.SetTrigger("AttackGiroPreparando");
        yield return new WaitForSeconds(1.0f);

        canMove = true;
        forcarHitboxDesligada = true;
        AlterarCollidersMulher(false);

        colliderPirueta.enabled = true;
        yield return new WaitForSeconds(tempoDePirueta);

        animMulher.SetTrigger("AttackGiroParando");
        canMove = false;
        yield return new WaitForSeconds(1.0f);
        colliderPirueta.enabled = false;
        forcarHitboxDesligada = false;
        canMove = true;
        AlterarCollidersMulher(true);
    }

    void AlterarCollidersMulher(bool estado)
    {
        Debug.Log("Alterando estado dos colliders do corpo para: " + estado);
        foreach (Collider2D col in collidersDoCorpo)
        {
            if (col != null)
            {
                col.enabled = estado;
                Debug.Log("Collider " + col.name + " alterado para: " + estado);
            }
        }
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
}