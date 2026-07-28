using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MulherScript : MonoBehaviour
{
    public int maxHealth = 300;
    [SerializeField] private float currentHealth;
    private bool isDead = false;
    private bool canMove = true;

    [Header("Configurações do Ataque Laser")]
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



    [Header("Configurações do player")]
    [SerializeField] private Transform player;
    [SerializeField] private Rigidbody2D playerRb;
    [SerializeField] private Player scriptDeMovimento;
    
    void Start()
    {
        currentHealth = maxHealth;
        pivotLaser.SetActive(false);
        piranhas.SetActive(false);
        if(colliderDoLaser != null) colliderDoLaser.enabled = false;
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

        if (canMove && player != null)
        {
            Move();
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
        if (isDead) return;
        currentHealth -= damage;
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
            PlayerAttack script = collision.gameObject.GetComponent<PlayerAttack>();

            if (script != null && script.isAttacking)
            {
                Debug.Log("Dano de mataCavalo");
                TakeDamage(2.5f);
            }
        }
    }

    IEnumerator ChooseAttack()
    {
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
                    break;
            }
            yield return new WaitForSeconds(5f);
        }
    }

    IEnumerator LaserAttack()
    {
        canMove = false;
        for (int i = 0; i < quantidadeDeLasers; i++)
        {
            pivotLaser.SetActive(true);
            yield return new WaitForSeconds(tempoDeAvisoLaser-0.5f);
            laserAtualLockado = true;
            yield return new WaitForSeconds(0.5f);

            spriteLaser.color = Color.red;
            colliderDoLaser.enabled = true;


            yield return new WaitForSeconds(0.5f);


            spriteLaser.color = Color.white;
            colliderDoLaser.enabled = false;

            laserAtualLockado = false;
            pivotLaser.SetActive(false);
            yield return new WaitForSeconds(intervaloEntreLasers);
        }
        canMove = true;
        yield return new WaitForSeconds(5.0f);
    }

    IEnumerator ChoroAttack()
    {
        canMove = false;
        transform.position = new Vector3(-15f, 0f, 0f);
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
    }

    //Função para spawnar peixes durante o ataque do choro
    void SpawnarPeixes ()
    {
        var spawnPos = new Vector3(-10f, Random.Range(-20f, 20f), 0f);

        //Peguei do código do diogo
        GameObject peixe = Instantiate(peixePrefab, spawnPos, Quaternion.identity);

        peixe.GetComponent<FishScript>().SetPlayer(player, playerRb);
    }
}