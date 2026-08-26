using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    [Header("Vida")]
    public int maxHealth = 3;
    private int currentHealth;
    public int takenDamage = 1;
    public GameObject[] coracoes;
    private bool estaNaLava = false;

    [Header("Invincibility")]
    public float invincibleTime = 3.0f;
    private bool isInvincible = false;
    private float invincibleCurrentTime = 0.0f;

    [Header("Piscar ao tomar dano")]
    public float flashInterval = 0.1f;
    private SpriteRenderer spriteRenderer;
    private Color corOriginal;
    private Coroutine flashCoroutine;

    [Header("Speed e Dash")]
    public float movementSpeed = 5f;
    public float dashSpeed = 7f;
    public float dashDuration = 0.5f;
    public float dashCooldown = 2.0f;

    [Header("Armas")]
    public GameObject[] weapons;

    [Header("Outros")]
    public Rigidbody2D rb;
    Vector2 movement;
    private Camera mainCamera;
    public GameObject deathScreen;

    [Header("Rosto UI")]
    public Image rostoNaTela;
    public Sprite rostoNormal;
    public Sprite rostoMachucado;

    [Header("Configuração de Troca de Arma")]
    public float weaponSwitchCooldown = 0.5f; // Tempo de espera entre as trocas em segundos
    private float nextWeaponSwitchTime = 0f; // Guarda quando a próxima troca será permitida

    private Coroutine rotinaRosto;

    // Efeito do choro da mulher
    [HideInInspector] public Vector2 forcaExterna;
    [HideInInspector] public bool isStunned = false;
    [HideInInspector] public float tempoStunAtual = 0f;
    [HideInInspector] public float tempoStun = 1f;

    [Header("Animação")]
    private Animator animator;

    [SerializeField] private PauseManager pauseManager;


    void Start()
    {
        mainCamera = Camera.main;
        currentHealth = maxHealth;
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
            corOriginal = spriteRenderer.color;

        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (!isStunned)
        {
            movement.x = Input.GetAxisRaw("Horizontal");
            movement.y = Input.GetAxisRaw("Vertical");
        }
        else
        {
            movement = Vector2.zero;
            tempoStunAtual += Time.deltaTime;
            if (tempoStunAtual >= tempoStun)
            {
                isStunned = false;
                tempoStunAtual = 0f;
            }
        }

        //Animação do player
        if (animator != null)
        {
            float speed = movement.sqrMagnitude;

            if (speed > 0.01f && !pauseManager.IsJogoPausado())
            {
                // Se estiver andando, envia a nova direção para a Blend Tree
                animator.SetFloat("InputX", movement.x);
                animator.SetFloat("InputY", movement.y);
                animator.speed = 1f; // Animação roda normalmente
            }
            else
            {
                animator.speed = 0f; // Congela a animação no último frame gravado
            }
        }

        if (currentHealth <= 0)
        {
            Destroy(gameObject);
        }

        if (Time.timeScale != 0f)
        {
            //RotateTowardsMouse();
            SelectWeapon();
            PlayerDash();
            if (isInvincible)
            {
                UpdateInvencibility();
            }
        }

        if (estaNaLava)
        {
            TakeDamage(takenDamage);
        }
    }

    void FixedUpdate()
    {
        Vector2 move = movement;
        if (move.sqrMagnitude > 1f) move = move.normalized;
        rb.MovePosition(rb.position + (move * movementSpeed + forcaExterna) * Time.fixedDeltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            TakeDamage(takenDamage);
        }
        if(collision.gameObject.CompareTag("Lava"))
        {
            estaNaLava = true;
        }
        if (collision.gameObject.CompareTag("Laser"))
        {
            TakeDamage(takenDamage);
        }
        if (collision.gameObject.CompareTag("Piranha"))
        {
            TakeDamage(takenDamage);
        }
        if (collision.gameObject.CompareTag("Peixe"))
        {
            isStunned = true;
            tempoStunAtual = 0f;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Lava"))
        {
            estaNaLava = false;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Tornado"))
        {
            TakeDamage(takenDamage);
        }
    }

    public void TakeDamage(int damage)
    {
        if (!isInvincible)
        {
            currentHealth -= damage;

            if (AudioManager.Instance != null)
                AudioManager.Instance.PlayPlayerDano();

            if (rotinaRosto != null)
            {
                StopCoroutine(rotinaRosto);
            }
            rotinaRosto = StartCoroutine(EfeitoRostoDano());
            if (currentHealth >= 0 && currentHealth < coracoes.Length)
                Destroy(coracoes[currentHealth]);

            isInvincible = true;

            if (flashCoroutine != null) StopCoroutine(flashCoroutine);
            flashCoroutine = StartCoroutine(FlashRed());

            if (currentHealth <= 0)
            {
                if (spriteRenderer != null) spriteRenderer.color = corOriginal;
                deathScreen.SetActive(true);
                Destroy(gameObject);
            }
        }
    }

    IEnumerator FlashRed()
    {
        while (isInvincible)
        {
            if (spriteRenderer != null)
                spriteRenderer.color = Color.red;
            yield return new WaitForSeconds(flashInterval);
            if (spriteRenderer != null)
                spriteRenderer.color = corOriginal;
            yield return new WaitForSeconds(flashInterval);
        }
        if (spriteRenderer != null)
            spriteRenderer.color = corOriginal;
    }

    void RotateTowardsMouse()
    {
        Vector3 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = new Vector2(
            mousePosition.x - transform.position.x,
            mousePosition.y - transform.position.y
        );
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle - 90f);
    }

    void SelectWeapon()
    {
        if (Input.GetKeyDown(KeyCode.E) && Time.time >= nextWeaponSwitchTime)
        {
            PlayerAttack bastaoScript = GetComponentInChildren<PlayerAttack>();
            if (bastaoScript == null || !bastaoScript.IsWeaponAttacking())
            {
                for (int i = 0; i < weapons.Length; i++)
                {
                    if (weapons[i].activeSelf)
                    {
                        weapons[i].SetActive(false);
                        int nextIndex = (i + 1) % weapons.Length;
                        weapons[nextIndex].SetActive(true);

                        nextWeaponSwitchTime = Time.time + weaponSwitchCooldown;
                        break;
                    }
                }
            }
        }
    }

    private bool isDashing = false;
    void PlayerDash()
    {
        bool dashInput = Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.LeftShift);
        if (dashInput && !isDashing)
        {
            StartCoroutine(DashCoroutine());
        }
    }

    IEnumerator DashCoroutine()
    {
        isDashing = true;
        float originalSpeed = movementSpeed;
        movementSpeed += dashSpeed;
        yield return new WaitForSeconds(dashDuration);
        movementSpeed = originalSpeed;
        float tempoRestanteDoCooldown = dashCooldown - dashDuration;
        if (tempoRestanteDoCooldown > 0)
        {
            yield return new WaitForSeconds(tempoRestanteDoCooldown);
        }
        isDashing = false;
    }

    void UpdateInvencibility()
    {
        invincibleCurrentTime += Time.deltaTime;
        if (invincibleCurrentTime >= invincibleTime)
        {
            isInvincible = false;
            invincibleCurrentTime = 0;
        }
    }

        private IEnumerator EfeitoRostoDano()
    {
        rostoNaTela.sprite = rostoMachucado;

        yield return new WaitForSeconds(3f);

        rostoNaTela.sprite = rostoNormal;
    }
}