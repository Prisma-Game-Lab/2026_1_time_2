using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public float attackSpeed = 5f;
    public float attackDamage = 10f;
    [SerializeField] private float attackDuration = 1.5f;

    [Header("Animação do Ataque")]
    public float rotationAngle = 60f;
    public bool isAttacking = false;
    public bool isHardAttack = false;

    [Header("Configuração de Troca de Arma")]
    public float attackLeveCooldown = 0.5f;
    public float attackPesadoCooldown = 1.5f;
    private float proximoAttackCooldown = 0f;

    private Camera mainCamera;

    // Evita bater múltiplas vezes no mesmo inimigo num único golpe
    private HashSet<Collider2D> inimigosAtingidos = new HashSet<Collider2D>();

    void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !isAttacking && Time.time >= proximoAttackCooldown)
        {
            StartCoroutine(weaponRotation());
            proximoAttackCooldown = Time.time + attackLeveCooldown;
        }
        if (Input.GetMouseButtonDown(1) && !isAttacking && Time.time >= proximoAttackCooldown)
        {
            StartCoroutine(SpecialWeaponRotation());
            proximoAttackCooldown = Time.time + attackPesadoCooldown;
        }

        if (Time.deltaTime > 0)
        {
            RotateTowardsMouse();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!isAttacking) return;
        if (inimigosAtingidos.Contains(other)) return; // já bateu nesse inimigo nesse golpe

        if (other.CompareTag("Enemy"))
        {
            BossSnakeAI boss = other.GetComponentInParent<BossSnakeAI>();
            if (boss != null)
            {
                boss.TakeDamage((int)attackDamage);
                inimigosAtingidos.Add(other);
            }
        }
    }

    IEnumerator weaponRotation()
    {
        isAttacking = true;
        inimigosAtingidos.Clear();

        //Calcula as rotações de início e fim do ataque

        Quaternion localOriginRotation = transform.localRotation;
        Quaternion startRotation = localOriginRotation * Quaternion.Euler(0, 0, rotationAngle / 2f);
        Quaternion endRotation = localOriginRotation * Quaternion.Euler(0, 0, -rotationAngle / 2f);

        //Faz a rotação do ataque funcionar, primeiro indo um pouco para um lado, depois para o outro e depois voltando para a posição original
        float t = 0;

        while (t < 1f)
        {
            t += Time.deltaTime * attackSpeed;
            transform.localRotation = Quaternion.Slerp(localOriginRotation, startRotation, t);
            yield return null;
        }

        t = 0;
        while (t < 1f)
        {
            t += Time.deltaTime * attackSpeed;
            transform.localRotation = Quaternion.Slerp(startRotation, endRotation, t);
            yield return null;
        }

        t = 0;
        while (t < 1f)
        {
            t += Time.deltaTime * attackSpeed;
            transform.localRotation = Quaternion.Slerp(endRotation, localOriginRotation, t);
            yield return null;
        }

        isAttacking = false;
    }

    IEnumerator SpecialWeaponRotation()
    {
        isAttacking = true;
        isHardAttack = true;

        Quaternion localOriginRotation = transform.localRotation;
        Quaternion startRotation = localOriginRotation * Quaternion.Euler(0, 0, rotationAngle / 2f);

        float t = 0;

        // Divide o tempo total
        float windUpDuration = attackDuration * 0.15f;
        float spinDuration = attackDuration * 0.70f;
        float recoveryDuration = attackDuration * 0.15f;

        while (t < 1f)
        {
            t += Time.deltaTime / windUpDuration;
            transform.localRotation = Quaternion.Slerp(localOriginRotation, startRotation, t);
            yield return null;
        }

        t = 0;
        float totalRotationAmount = -360f * 3f;

        while (t < 1f)
        {
            t += Time.deltaTime / spinDuration;

            float currentAngle = Mathf.Lerp(0, totalRotationAmount, t);
            transform.localRotation = startRotation * Quaternion.Euler(0, 0, currentAngle);
            yield return null;
        }

        t = 0;
        while (t < 1f)
        {
            t += Time.deltaTime / recoveryDuration;
            transform.localRotation = Quaternion.Slerp(startRotation, localOriginRotation, t);
            yield return null;
        }

        transform.localRotation = localOriginRotation;
        isAttacking = false;
        isHardAttack = false;
    }

    public bool IsWeaponAttacking()
    {
        return isAttacking;
    }

    public bool IsWeaponHardAttacking()
    {
        return isHardAttack;
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
}