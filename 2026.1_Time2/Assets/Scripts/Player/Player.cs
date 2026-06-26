using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Player : MonoBehaviour
{
    [Header("Vida")]
    public int maxHealth = 3;
    private int currentHealth;
    public int takenDamage = 1;

    /* O tal do invencibility frame*/
    public float invincibleTime = 3.0f;

    private bool isInvincible = false;

    private float invincibleCurrentTime = 0.0f;


    public GameObject[] coracoes;

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

    void Start()
    {
        mainCamera = Camera.main;
        currentHealth = maxHealth;
    }
    void Update()
    {
        // Input dos Movimentos Verticais e Horizontais
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");
        if (currentHealth <= 0)
        {
            Destroy(gameObject);
        }
        if (Time.timeScale != 0f) // Evita que o jogador gire ou ataque quando o jogo estiver pausado
        {
            SelectWeapon();
            PlayerDash();
            if (isInvincible)
            {
                UpdateInvencibility();
            }
        }
        Debug.Log("Invincible: " + isInvincible);
    }

    void FixedUpdate()
    {
        // Movimentação do Player
        Vector2 move = movement;
        // Normaliza para não aumentar velocidade na diagonal
        if (move.sqrMagnitude > 1f) move = move.normalized;
        rb.MovePosition(rb.position + move * movementSpeed * Time.fixedDeltaTime);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            TakeDamage(takenDamage);
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
            Destroy(coracoes[currentHealth]);
            isInvincible = true;
            if (currentHealth <= 0)
            {
                Destroy(gameObject);
                deathScreen.SetActive(true);
            }
        }
    }
    void RotateTowardsMouse()
    {
        // Pega a posição do mouse na tela e converte para coordenadas do mundo
        Vector3 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        // Calcula a direção
        Vector2 direction = new Vector2(
            mousePosition.x - transform.position.x,
            mousePosition.y - transform.position.y
        );
        // Calcula o ângulo
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        // Aplica a rotação no eixo Z
        transform.rotation = Quaternion.Euler(0, 0, angle - 90f);
    }
    void SelectWeapon()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            PlayerAttack bastaoScript = GetComponentInChildren<PlayerAttack>();
            if (bastaoScript == null || !bastaoScript.IsWeaponAttacking())
            {
                // Ativa a próxima arma
                for (int i = 0; i < weapons.Length; i++)
                {
                    if (weapons[i].activeSelf)
                    {
                        weapons[i].SetActive(false);
                        int nextIndex = (i + 1) % weapons.Length;
                        weapons[nextIndex].SetActive(true);
                        break;
                    }
                }
            }
        }
    }
    private bool isDashing = false;
    void PlayerDash()
    {
        // Espaço ou L-Shift para dar dash
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
        invincibleCurrentTime += Time.deltaTime * 1.0f; // Incrementa o tempo de invencibilidade
        if (invincibleCurrentTime >= invincibleTime)
        {
            isInvincible = false;
            invincibleCurrentTime = 0;
        }
    }
}