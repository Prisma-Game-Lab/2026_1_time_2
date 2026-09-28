using System.Collections;
using UnityEngine;

public class SerpenteBossManager : MonoBehaviour
{
    public static SerpenteBossManager Instance { get; private set; } 

    [Header("Referências")]
    public SnakeManager snakeMovimento; 
    public BossSnakeAttacks ataques; 
    [SerializeField] private Player player;

    [Header("Limites da Tela")]
    public float limiteX = 15f; 
    public float limiteY = 10f; 

    [Header("Vida e Dano")]
    public int maxHealth = 300;
    private int currentHealth;
    public Color corHit = Color.red;
    public float duracaoFlashHit = 0.1f;

    [Header("Ritmo de Batalha")]
    public float delayEntreAtaques = 2.5f;
    public float multiplicadorVelocidadeTangente = 2.0f;
    private float velocidadeOriginal; 

    [Header("Controle de Ativação")]
    public bool isAtivo = false; 
    public float tempoDeEsperaInicial = 3f; 

    private bool isDead = false;
    private Vector3 pontoDeFuga;
    private Quaternion rotacaoDeFuga;
    
    private Vector3 centroDaArena; 

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        currentHealth = maxHealth;
        if (snakeMovimento != null) velocidadeOriginal = snakeMovimento.speed;
        
        centroDaArena = transform.position;

        StartCoroutine(RotinaDeEntrada());
    }

    IEnumerator RotinaDeEntrada()
    {
        while (snakeMovimento.GetCabeca() == null)
        {
            yield return null;
        }

        Transform cabeca = snakeMovimento.GetCabeca();

        Vector3 pontoDeNascer = new Vector3(-limiteX - 15f, centroDaArena.y, 0f);
        Quaternion viradaParaDireita = Quaternion.Euler(0, 0, 0); // 0 = Direita
        
        snakeMovimento.estaOrbitando = false;
        snakeMovimento.ReposicionarCobra(pontoDeNascer, viradaParaDireita);

        Debug.Log("Cobra spawnou fora da tela. Rastejando para o centro...");

        while (cabeca.position.x < centroDaArena.x)
        {
            yield return null;
        }

        cabeca.position = new Vector3(centroDaArena.x, cabeca.position.y, cabeca.position.z);
        snakeMovimento.estaOrbitando = true;
        
        Debug.Log("Cobra entrou em órbita!");

        StartCoroutine(CronometroDeAtivacao());
        StartCoroutine(AttackLoop());
    }

    IEnumerator CronometroDeAtivacao()
    {
        Debug.Log($"Boss iniciou pacífico. Aguardando {tempoDeEsperaInicial} segundos...");
        yield return new WaitForSeconds(tempoDeEsperaInicial);
        isAtivo = true;
        Debug.Log("Boss ATIVO! Os ataques vão começar.");
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;
        
        currentHealth -= damage;
        StartCoroutine(FlashHit());

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            isDead = true;
            Die();
        }
    }

    public int getCurrentHealth()
    {
        return currentHealth;
    }

    public int getMaxHealth()
    {
        return maxHealth;
    }

    IEnumerator FlashHit()
    {
        SpriteRenderer[] partesDoCorpo = GetComponentsInChildren<SpriteRenderer>();
        foreach (SpriteRenderer parte in partesDoCorpo) parte.color = corHit;
        
        yield return new WaitForSeconds(duracaoFlashHit);
        
        foreach (SpriteRenderer parte in partesDoCorpo)
        {
            if (parte != null) parte.color = Color.white;
        }
    }

    void Die()
    {
        StopAllCoroutines();
        if (ataques != null) ataques.StopAllCoroutines();
        
        Debug.Log("Serpente derrotada!");
        Destroy(gameObject, 0.5f); 
    }

    IEnumerator AttackLoop()
    {
        yield return new WaitUntil(() => isAtivo == true);
        yield return new WaitForSeconds(3f);

        while (!isDead && player!= null)
        {
            int ataqueSorteado = Random.Range(2, 3);
            
            GravarMemoriaDaOrbita();
            
            if (ataqueSorteado == 0 || ataqueSorteado == 1)
            {
                yield return StartCoroutine(FugirSemGravar());

                if (ataqueSorteado == 0) 
                {
                    yield return StartCoroutine(ataques.ExecutarDash());
                }
                else
                {
                    //yield return new WaitForSeconds(0.7f);
                    yield return StartCoroutine(ataques.ExecutarTornado());
                }
            }
            else if (ataqueSorteado == 2)
            {
                yield return StartCoroutine(ataques.ExecutarMordida());
                
                yield return StartCoroutine(FugirSemGravar());

                yield return new WaitForSeconds(1f);
            }

            yield return StartCoroutine(RetornarParaOrbita());
            yield return new WaitForSeconds(delayEntreAtaques);
        }
    }

    void GravarMemoriaDaOrbita()
    {
        Transform cabeca = snakeMovimento.GetCabeca();
        pontoDeFuga = cabeca.position;
        rotacaoDeFuga = cabeca.rotation;
    }

    IEnumerator FugirSemGravar()
    {
        snakeMovimento.estaOrbitando = false;
        snakeMovimento.speed = velocidadeOriginal * multiplicadorVelocidadeTangente;

        Transform cauda = snakeMovimento.GetCauda();
        if (cauda != null)
        {
            while (Mathf.Abs(cauda.position.x) < limiteX + 2f && Mathf.Abs(cauda.position.y) < limiteY + 2f)
            {
                yield return null; 
            }
        }
        
        yield return new WaitForSeconds(1f);
    }

    IEnumerator SairPelaTangente()
    {
        Transform cabeca = snakeMovimento.GetCabeca();
        pontoDeFuga = cabeca.position;
        rotacaoDeFuga = cabeca.rotation;

        snakeMovimento.estaOrbitando = false;
        snakeMovimento.speed = velocidadeOriginal * multiplicadorVelocidadeTangente;

        Transform cauda = snakeMovimento.GetCauda();
        if (cauda != null)
        {
            while (Mathf.Abs(cauda.position.x) < limiteX + 2f && Mathf.Abs(cauda.position.y) < limiteY + 2f)
            {
                yield return null; 
            }
        }
        
        yield return new WaitForSeconds(1f);
    }

    IEnumerator RetornarParaOrbita()
    {
        Transform cabeca = snakeMovimento.GetCabeca();

        float tempoNoFuturo = Random.Range(2.0f, 8.0f); 
        float precisaoSimulacao = 0.02f; 

        Vector3 pontoFuturo = pontoDeFuga;
        Quaternion rotacaoFutura = rotacaoDeFuga;
        float velocidadeRealPorSegundo = velocidadeOriginal * Time.fixedDeltaTime;

        for (float t = 0; t < tempoNoFuturo; t += precisaoSimulacao)
        {
            Vector3 direcaoSimulada = rotacaoFutura * Vector3.right;
            float forcaDaCurva = 1f + (Mathf.Abs(direcaoSimulada.y) * snakeMovimento.multiplicadorDaBorda);
            float giro = -snakeMovimento.rotationSpeed * forcaDaCurva * precisaoSimulacao;

            rotacaoFutura *= Quaternion.Euler(0, 0, giro);
            pontoFuturo += direcaoSimulada * (velocidadeRealPorSegundo * precisaoSimulacao);
        }

        Vector3 direcaoEntrada = rotacaoFutura * Vector3.right;
        
        float distanciaDeSpawn = Mathf.Max(limiteX, limiteY) + 8f; 
        Vector3 posicaoDeNascer = pontoFuturo - (direcaoEntrada * distanciaDeSpawn);

        snakeMovimento.ReposicionarCobra(posicaoDeNascer, rotacaoFutura);

        float distanciaAnterior = Mathf.Infinity;
        
        while (true)
        {
            float distanciaAtual = Vector3.Distance(cabeca.position, pontoFuturo);
            if (distanciaAtual <= 1.0f || distanciaAtual > distanciaAnterior) break;
            
            distanciaAnterior = distanciaAtual;
            yield return null; 
        }
        
        cabeca.position = pontoFuturo;
        cabeca.rotation = rotacaoFutura;
        
        snakeMovimento.estaOrbitando = true;
        snakeMovimento.speed = velocidadeOriginal;
    }
}