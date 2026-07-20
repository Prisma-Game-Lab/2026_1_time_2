using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSnakeAttacks : MonoBehaviour
{
    [Header("Refer�ncias")]
    public Transform player;
    public GameObject tornadoPrefab;

    [Header("Ataque: Tornado")]
    public int tornadoCount = 1;
    public float tornadoSpawnRadius = 2f;
    public float tornadoMinSize = 0.5f;
    public float tornadoMaxSize = 2f;
    public float tornadoMinSpeed = 1.5f;
    public float tornadoMaxSpeed = 5f;
    public float tornadoDamage = 10f;

    [Header("Ataque: Bote")]
    public float biteDashSpeed = 20f;
    public float biteStunDuration = 3f;
    public float biteDamage = 20f;
    private int missedBitesCount = 0; 

    [Header("Ataque: Mergulho (Dash Through)")]
    public float dashOutSpeed = 15f;
    public float dashOutDamage = 25f;
    public float dashTiredDuration = 4f; 
    public float offscreenOffset = 3f;

    private float arenaLeft = -15.05f;
    private float arenaRight = 14.95f;
    private float arenaBottom = -10f;
    private float arenaTop = 10f;

    private Vector3 originPosition;
    private bool isAttacking = false;
    private bool isStunned = false;

    public bool IsAttacking() => isAttacking;
    public bool IsStunned() => isStunned;

    void Start()
    {
        originPosition = transform.position;
    }


    public void AttackTornado()
    {
        if (isAttacking || isStunned || tornadoPrefab == null) return;
        StartCoroutine(TornadoCoroutine());
    }

    IEnumerator TornadoCoroutine()
    {
        isAttacking = true;
        for (int i = 0; i < tornadoCount; i++)
        {
            Vector2 spawnOffset = Random.insideUnitCircle.normalized * tornadoSpawnRadius;
            Vector3 spawnPos = transform.position + (Vector3)spawnOffset;

            float size = Random.Range(tornadoMinSize, tornadoMaxSize);
            float t = Mathf.InverseLerp(tornadoMinSize, tornadoMaxSize, size);
            float speed = Mathf.Lerp(tornadoMaxSpeed, tornadoMinSpeed, t);

            Vector2 direction = (player.position - spawnPos).normalized;

            GameObject tornado = Instantiate(tornadoPrefab, spawnPos, Quaternion.identity);
            tornado.transform.localScale = Vector3.one * size;


            TornadoProjectile proj = tornado.GetComponent<TornadoProjectile>();
            if (proj != null) proj.Init(direction, speed, tornadoDamage);

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
        transform.up = -direction;

        float distanceTraveled = 0f;
        float totalDistance = Vector3.Distance(startPos, targetPos);
        bool hitPlayer = false;

        while (distanceTraveled < totalDistance)
        {
            float step = biteDashSpeed * Time.deltaTime;
            transform.position += direction * step;
            distanceTraveled += step;

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
            missedBitesCount++;
            Debug.Log($"Serpente errou o bote! Erros acumulados: {missedBitesCount}/3");

            if (missedBitesCount >= 3)
            {
                Debug.Log("Serpente bateu a cabe�a e est� DESNORTEADA!");
                isStunned = true;
                // TODO: Chamar Trigger do Animator para anima��o de Stun aqui
                yield return new WaitForSeconds(biteStunDuration);
                isStunned = false;
                missedBitesCount = 0; // Reseta ap�s sofrer o stun
            }
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

 
        int minPasses = 5, maxPasses = 7; 
        if (healthPercent > 0.66f) { minPasses = 3; maxPasses = 5; }      
        else if (healthPercent > 0.33f) { minPasses = 4; maxPasses = 6; } 

        int passes = Random.Range(minPasses, maxPasses + 1);

        for (int i = 0; i < passes; i++)
        {
            Vector2 enterDirection = GetRandomAllDirections(); 
            Vector3 entryPoint = GetOffscreenPosition(enterDirection);
            Vector3 exitPoint = GetOffscreenPosition(-enterDirection);

            // GDD: Anima��o das folhas para avisar o player de onde ela vem
            DispararAnimacaoFolhas(enterDirection);

            yield return new WaitForSeconds(0.5f);

            transform.position = entryPoint;
            Vector3 direction = (exitPoint - entryPoint).normalized;
            float totalDist = Vector3.Distance(entryPoint, exitPoint);
            transform.up = -direction;

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
            yield return new WaitForSeconds(0.2f);
        }

        
        transform.position = originPosition; 
        transform.rotation = Quaternion.identity;

        Debug.Log("Serpente terminou os mergulhos e est� EXAUSTA!");
        isStunned = true;

        // TODO: Chamar Trigger do Animator para anima��o de Exaust�o aqui
        yield return new WaitForSeconds(dashTiredDuration);
        isStunned = false;

        isAttacking = false;
    }


    Vector2 GetRandomAllDirections()
    {
        
        Vector2[] directions = {
            Vector2.up, Vector2.down, Vector2.left, Vector2.right,
            new Vector2(1, 1).normalized, new Vector2(-1, 1).normalized,
            new Vector2(1, -1).normalized, new Vector2(-1, -1).normalized
        };
        return directions[Random.Range(0, directions.Length)];
    }

    Vector3 GetOffscreenPosition(Vector2 dir)
    {
        
        float centerX = (arenaLeft + arenaRight) / 2f;
        float centerY = (arenaTop + arenaBottom) / 2f;

        float targetX = centerX;
        float targetY = centerY;
   
        if (dir.x > 0.1f) targetX = arenaRight + offscreenOffset;
        else if (dir.x < -0.1f) targetX = arenaLeft - offscreenOffset;

        if (dir.y > 0.1f) targetY = arenaTop + offscreenOffset;
        else if (dir.y < -0.1f) targetY = arenaBottom - offscreenOffset;

        return new Vector3(targetX, targetY, 0f);
    }

    void DispararAnimacaoFolhas(Vector2 direcaoEntrada)
    {
        Debug.Log($"[VFX] Balan�ar �rvores na dire��o: {direcaoEntrada}");
    }
}