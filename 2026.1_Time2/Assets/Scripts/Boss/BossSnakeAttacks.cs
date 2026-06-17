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
    // Tempo que o boss fica parado no ponto de entrada antes de avançar, dando tempo do player desviar
    public float dashWarningDelay = 0.6f;

    // Limites da arena
    private float arenaLeft = -15.05f;
    private float arenaRight = 14.95f;
    private float arenaBottom = -10f;
    private float arenaTop = 10f;

    private Vector3 originPosition;
    private bool isAttacking = false;
    private Vector2 lastDashDirection = Vector2.zero;
    private int sameDashDirectionCount = 0;

    void Start()
    {
        originPosition = transform.position;
    }

    // -------------------------------------------------------
    // ATAQUE 1: TORNADO
    // Lança tornadoCount tornados em direção ao player,
    // com tamanhos e velocidades proporcionalmente inversos.
    // forceMultiplier aumenta o dano conforme a vida do boss cai.
    // -------------------------------------------------------
    public void AttackTornado(float forceMultiplier = 1f)
    {
        if (isAttacking || tornadoPrefab == null) return;
        StartCoroutine(TornadoCoroutine(forceMultiplier));
    }

    IEnumerator TornadoCoroutine(float forceMultiplier)
    {
        isAttacking = true;

        for (int i = 0; i < tornadoCount; i++)
        {
            // Posição de spawn ao redor do boss
            Vector2 spawnOffset = Random.insideUnitCircle.normalized * tornadoSpawnRadius;
            Vector3 spawnPos = transform.position + (Vector3)spawnOffset;

            // Tamanho aleatório
            float size = Random.Range(tornadoMinSize, tornadoMaxSize);

            // Velocidade inversamente proporcional ao tamanho, escalando com a fase
            float t = Mathf.InverseLerp(tornadoMinSize, tornadoMaxSize, size);
            float speed = Mathf.Lerp(tornadoMaxSpeed, tornadoMinSpeed, t) * forceMultiplier;

            // Direção em relação ao player
            Vector2 direction = ((Vector2)player.position - (Vector2)spawnPos).normalized;

            // Instancia e inicializa
            GameObject tornado = Instantiate(tornadoPrefab, spawnPos, Quaternion.identity);
            tornado.transform.localScale = Vector3.one * size;

            TornadoProjectile proj = tornado.GetComponent<TornadoProjectile>();
            if (proj != null)
            {
                proj.Init(direction, speed, tornadoDamage * forceMultiplier);
            }

            yield return new WaitForSeconds(0.3f);
        }

        isAttacking = false;
    }

    // -------------------------------------------------------
    // ATAQUE 2: MORDIDA (bote de cobra)
    // Boss avança em direção ao player. Se errar, fica stunado.
    // -------------------------------------------------------
    public void AttackBite(float forceMultiplier = 1f)
    {
        if (isAttacking || isStunned) return;
        StartCoroutine(BiteCoroutine(forceMultiplier));
    }

    IEnumerator BiteCoroutine(float forceMultiplier)
    {
        isAttacking = true;

        Vector3 targetPos = player.position;
        Vector3 startPos = transform.position;
        Vector3 direction = (targetPos - startPos).normalized;

        float currentBiteSpeed = biteDashSpeed * forceMultiplier;

        // Avança até a posição alvo
        float distanceTraveled = 0f;
        float totalDistance = Vector3.Distance(startPos, targetPos);

        bool hitPlayer = false;

        while (distanceTraveled < totalDistance)
        {
            float step = currentBiteSpeed * Time.deltaTime;
            transform.position += direction * step;
            distanceTraveled += step;

            // Verifica se acertou (via overlap)
            Collider2D hit = Physics2D.OverlapCircle(transform.position, 0.5f);
            if (hit != null && hit.CompareTag("Player"))
            {
                hitPlayer = true;
                Player p = hit.GetComponent<Player>();
                if (p != null) p.TakeDamage((int)(biteDamage * forceMultiplier));
                break;
            }

            yield return null;
        }

        // Se errou: stun
        if (!hitPlayer)
        {
            Debug.Log("Serpente errou a mordida! Stunada por " + biteStunDuration + "s");
            isStunned = true;
            yield return new WaitForSeconds(biteStunDuration);
            isStunned = false;
        }

        isAttacking = false;
    }

    // -------------------------------------------------------
    // ATAQUE 3: SAINDO DE TELA
    // Boss sai pela borda, avisa com direção, e volta de outra
    // A quantidade de passagens varia com a vida (passada por parâmetro)
    // -------------------------------------------------------
    public void AttackDashThrough(float healthPercent, float forceMultiplier = 1f)
    {
        if (isAttacking || isStunned) return;
        StartCoroutine(DashThroughCoroutine(healthPercent, forceMultiplier));
    }

    IEnumerator DashThroughCoroutine(float healthPercent, float forceMultiplier)
    {
        isAttacking = true;

        // Quantidade de passagens baseada na vida
        int minPasses, maxPasses;
        if (healthPercent > 0.66f) { minPasses = 3; maxPasses = 5; }
        else if (healthPercent > 0.33f) { minPasses = 4; maxPasses = 6; }
        else { minPasses = 5; maxPasses = 7; }

        int passes = Random.Range(minPasses, maxPasses + 1);
        float currentDashSpeed = dashOutSpeed * forceMultiplier;

        for (int i = 0; i < passes; i++)
        {
            // Escolhe uma das 8 direções cardeais/colaterais, evitando repetir 2x seguidas
            Vector2 enterDirection = GetNextDashDirection();
            Vector3 entryPoint = GetOffscreenPosition(enterDirection);
            Vector3 exitPoint = GetOffscreenPosition(-enterDirection);

            // Avisa a direção (pode trocar pelo efeito visual de folhas depois)
            Debug.Log("Serpente vem de: " + enterDirection);

            // Move para o ponto de entrada fora da tela
            transform.position = entryPoint;

            // Pequena pausa avisando de onde o ataque vem, dando tempo do player desviar
            yield return new WaitForSeconds(dashWarningDelay);

            // Atravessa até o ponto de saída
            Vector3 direction = (exitPoint - entryPoint).normalized;
            float totalDist = Vector3.Distance(entryPoint, exitPoint);
            float traveled = 0f;

            while (traveled < totalDist)
            {
                float step = currentDashSpeed * Time.deltaTime;
                transform.position += direction * step;
                traveled += step;

                // Dano ao player se passar perto
                Collider2D hit = Physics2D.OverlapCircle(transform.position, 0.5f);
                if (hit != null && hit.CompareTag("Player"))
                {
                    Player p = hit.GetComponent<Player>();
                    if (p != null) p.TakeDamage((int)(dashOutDamage * forceMultiplier));
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

    // Retorna uma das 8 direções (cardeais + colaterais) aleatória
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

    // Escolhe a próxima direção do dash, evitando repetir a mesma
    // direção mais de duas vezes seguidas
    Vector2 GetNextDashDirection()
    {
        Vector2 direction = GetRandomCardinalDirection();

        if (direction == lastDashDirection)
        {
            sameDashDirectionCount++;
            // Se já repetiu 2x, força uma direção diferente
            if (sameDashDirectionCount >= 2)
            {
                Vector2 newDirection;
                do
                {
                    newDirection = GetRandomCardinalDirection();
                } while (newDirection == lastDashDirection);

                direction = newDirection;
                sameDashDirectionCount = 0;
            }
        }
        else
        {
            sameDashDirectionCount = 0;
        }

        lastDashDirection = direction;
        return direction;
    }

    // Retorna um ponto fora da arena na direção dada
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