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
    public float weaponSwitchCooldown = 0.5f;
    private float nextWeaponSwitchTime = 0f;

    private Coroutine rotinaRosto;

    // Efeito do choro da mulher
    [HideInInspector] public Vector2 forcaExterna;
    [HideInInspector] public bool isStunned = false;
    [HideInInspector] public float tempoStunAtual = 0f;
    [HideInInspector] public float tempoStun = 1f;

    [Header("Animação")]
    private Animator animator;

    [SerializeField] private PauseManager pauseManager;

    [Header("Knockback Settings")]
    [SerializeField] private float knockbackForce = 8f;
    [SerializeField] private float knockbackDuration = 0.2f;
    private bool isKnockbackActive = false;
    public bool IsKnockbackActive => isKnockbackActive;

    void Start()
    {
        mainCamera = Camera.main;
        currentHealth = maxHealth;
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
            corOriginal = spriteRenderer.color;

        animator = GetComponent<Animator>();
        if (rb == null)
            rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (!isStunned && !isKnockbackActive)
        {
            movement.x = Input.GetAxisRaw("Horizontal");
            movement.y = Input.GetAxisRaw("Vertical");
        }
        else
        {
            movement = Vector2.zero;

            if (isStunned)
            {
                tempoStunAtual += Time.deltaTime;
                if (tempoStunAtual >= tempoStun)
                {
                    isStunned = false;
                    tempoStunAtual = 0f;
                }
            }
        }

        // Animação do player
        if (animator != null)
        {
            float speed = movement.sqrMagnitude;
            bool pausado = (pauseManager != null) ? pauseManager.IsJogoPausado() : false;

            if (!pausado)
            {
                animator.SetFloat("Speed", speed);

                if (speed > 0.01f)
                {
                    animator.SetFloat("InputX", movement.x);
                    animator.SetFloat("InputY", movement.y);

                    animator.SetFloat("LastInputX", movement.x);
                    animator.SetFloat("LastInputY", movement.y);

                    animator.speed = 1f;
                }
                else
                {
                    animator.speed = 1f;
                }
            }
            else
            {
                animator.speed = 0f;
            }
        }

        if (currentHealth <= 0)
        {
            Destroy(gameObject);
        }

        if (Time.timeScale != 0f)
        {
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
        if (isKnockbackActive) return;

        Vector2 move = movement;
        if (move.sqrMagnitude > 1f) move = move.normalized;
        rb.MovePosition(rb.position + (move * movementSpeed + forcaExterna) * Time.fixedDeltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            TakeDamage(takenDamage, collision.transform.position);
        }
        if (collision.gameObject.CompareTag("Lava"))
        {
            estaNaLava = true;
        }
        if (collision.gameObject.CompareTag("Laser"))
        {
            TakeDamage(takenDamage);
        }
        if (collision.gameObject.CompareTag("Piranha"))
        {
            TakeDamage(takenDamage, collision.transform.position);
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
        if (collision.gameObject.CompareTag("Enemy"))
        {
            TakeDamage(takenDamage, collision.transform.position);
        }
        if (collision.gameObject.CompareTag("Tornado"))
        {
            TakeDamage(takenDamage, collision.transform.position);
        }
    }

    public void TakeDamage(int damage)
    {
        TakeDamage(damage, Vector2.zero, false);
    }

    public void TakeDamage(int damage, Vector2 damageSourcePosition, bool applyKnockback = true)
    {
        if (!isInvincible)
        {
            currentHealth -= damage;

            if (AudioManager.Instance != null)
                AudioManager.Instance.PlayPlayerDano();

            if (rotinaRosto != null)
                StopCoroutine(rotinaRosto);
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
                return;
            }

            if (applyKnockback)
            {
                StartCoroutine(Knockback(damageSourcePosition));
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

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayPlayerDash();

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

    private IEnumerator Knockback(Vector2 damageSourcePosition)
    {
        isKnockbackActive = true;

        Vector2 knockbackDirection = ((Vector2)transform.position - damageSourcePosition).normalized;

        if (knockbackDirection == Vector2.zero)
            knockbackDirection = Vector2.up;

        rb.velocity = Vector2.zero;
        rb.AddForce(knockbackDirection * knockbackForce, ForceMode2D.Impulse);

        yield return new WaitForSeconds(knockbackDuration);

        rb.velocity = Vector2.zero;
        isKnockbackActive = false;
    }
}