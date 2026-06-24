using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSnakeAttacks : MonoBehaviour
{
    [Header("Referências")]
    public Transform player;
    public GameObject tornadoPrefab;

    [Header("Ataque: Tornado")]
    public int tornadoCount = 1;
    public float tornadoSpawnRadius = 2f;
    // Cada tornado tem tamanho e velocidade variados — maior = mais lento
    public float tornadoMinSize = 0.5f;
    public float tornadoMaxSize = 2f;
    public float tornadoMinSpeed = 1.5f;
    public float tornadoMaxSpeed = 5f;
    public float tornadoDamage = 10f;

    [Header("Ataque: Mordida")]
    public float biteDashSpeed = 20f;
    public float biteStunDuration = 2f;
    public float biteDamage = 20f;
    private bool isStunned = false;

    [Header("Ataque: Saindo de Tela")]
    // Quantidade de vezes que o boss atravessa varia com a vida
    // Alta vida: 3-5, Média: 4-6, Baixa: 5-7
    public float dashOutSpeed = 15f;
    public float dashOutDamage = 25f;
    // Offset além da borda para o boss sumir de tela
    public float offscreenOffset = 3f;

    // Limites da arena
    private float arenaLeft = -15.05f;
    private float arenaRight = 14.95f;
    private float arenaBottom = -10f;
    private float arenaTop = 10f;

    private Vector3 originPosition;
    private bool isAttacking = false;

    void Start()
    {
        originPosition = transform.position;
    }

    public void AttackTornado()
    {
        if (isAttacking || tornadoPrefab == null) return;
        StartCoroutine(TornadoCoroutine());
    }

    IEnumerator TornadoCoroutine()
    {
        isAttacking = true;

        for (int i = 0; i < tornadoCount; i++)
        {
            // Posição de spawn ao redor do boss
            Vector2 spawnOffset = Random.insideUnitCircle.normalized * tornadoSpawnRadius;
            Vector3 spawnPos = transform.position + (Vector3)spawnOffset;

            // Tamanho aleatório
            float size = Random.Range(tornadoMinSize, tornadoMaxSize);

            // Velocidade inversamente proporcional ao tamanho
            float t = Mathf.InverseLerp(tornadoMinSize, tornadoMaxSize, size);
            float speed = Mathf.Lerp(tornadoMaxSpeed, tornadoMinSpeed, t);

            
            Vector2 direction = (player.position - (Vector3)spawnPos).normalized;

            // Instancia e inicializa
            GameObject tornado = Instantiate(tornadoPrefab, spawnPos, Quaternion.identity);
            tornado.transform.localScale = Vector3.one * size;

            TornadoProjectile proj = tornado.GetComponent<TornadoProjectile>();
            if (proj != null)
            {
                proj.Init(direction, speed, tornadoDamage);
            }

            yield return new WaitForSeconds(0.3f);
        }

        isAttacking = false;
    }

    public void AttackBite()
    {
        if (isAttacking || isStunned) return;
        StartCoroutine(BiteCoroutine());
    }

    IEnumerator BiteCoroutine()
    {
        isAttacking = true;

        Vector3 targetPos = player.position;
        Vector3 startPos = transform.position;
        Vector3 direction = (targetPos - startPos).normalized;

        // Avança até a posição alvo
        float distanceTraveled = 0f;
        float totalDistance = Vector3.Distance(startPos, targetPos);

        bool hitPlayer = false;

        while (distanceTraveled < totalDistance)
        {
            float step = biteDashSpeed * Time.deltaTime;
            transform.position += direction * step;
            distanceTraveled += step;

            // Verifica se acertou 
            Collider2D hit = Physics2D.OverlapCircle(transform.position, 0.5f);
            if (hit != null && hit.CompareTag("Player"))
            {
                hitPlayer = true;
                Player p = hit.GetComponent<Player>();
                if (p != null) p.TakeDamage((int)biteDamage);
                break;
            }

            yield return null;
        }

        if (!hitPlayer)
        {
            Debug.Log("Serpente errou a mordida! Stunada por " + biteStunDuration + "s");
            isStunned = true;
            yield return new WaitForSeconds(biteStunDuration);
            isStunned = false;
        }

        isAttacking = false;
    }

    public void AttackDashThrough(float healthPercent)
    {
        if (isAttacking || isStunned) return;
        StartCoroutine(DashThroughCoroutine(healthPercent));
    }

    IEnumerator DashThroughCoroutine(float healthPercent)
    {
        isAttacking = true;

        // Quantidade de passagens baseada na vida
        int minPasses, maxPasses;
        if (healthPercent > 0.66f) { minPasses = 3; maxPasses = 5; }
        else if (healthPercent > 0.33f) { minPasses = 4; maxPasses = 6; }
        else { minPasses = 5; maxPasses = 7; }

        int passes = Random.Range(minPasses, maxPasses + 1);

        for (int i = 0; i < passes; i++)
        {
            
            Vector2 enterDirection = GetRandomCardinalDirection();
            Vector3 entryPoint = GetOffscreenPosition(enterDirection);
            Vector3 exitPoint = GetOffscreenPosition(-enterDirection);

            // Avisa a direção 
            Debug.Log("Serpente vem de: " + enterDirection);
            
            transform.position = entryPoint;

            Vector3 direction = (exitPoint - entryPoint).normalized;
            float totalDist = Vector3.Distance(entryPoint, exitPoint);
            float traveled = 0f;

            while (traveled < totalDist)
            {
                float step = dashOutSpeed * Time.deltaTime;
                transform.position += direction * step;
                traveled += step;

                Collider2D hit = Physics2D.OverlapCircle(transform.position, 0.5f);
                if (hit != null && hit.CompareTag("Player"))
                {
                    Player p = hit.GetComponent<Player>();
                    if (p != null) p.TakeDamage((int)dashOutDamage);
                }

                yield return null;
            }

            // Pausa entre passagens
            yield return new WaitForSeconds(0.5f);
        }

        // Volta para a posição original após o ataque
        transform.position = originPosition;
        isAttacking = false;
    }

    Vector2 GetRandomCardinalDirection()
    {
        Vector2[] directions = {
            Vector2.up, Vector2.down, Vector2.left, Vector2.right,
            new Vector2(1, 1).normalized,
            new Vector2(-1, 1).normalized,
            new Vector2(1, -1).normalized,
            new Vector2(-1, -1).normalized
        };
        return directions[Random.Range(0, directions.Length)];
    }

    Vector3 GetOffscreenPosition(Vector2 direction)
    {
        float centerX = (arenaLeft + arenaRight) / 2f;
        float centerY = (arenaBottom + arenaTop) / 2f;

        float x = centerX + direction.x * ((arenaRight - arenaLeft) / 2f + offscreenOffset);
        float y = centerY + direction.y * ((arenaTop - arenaBottom) / 2f + offscreenOffset);

        return new Vector3(x, y, 0);
    }

    public bool IsAttacking() => isAttacking;
    public bool IsStunned() => isStunned;
}