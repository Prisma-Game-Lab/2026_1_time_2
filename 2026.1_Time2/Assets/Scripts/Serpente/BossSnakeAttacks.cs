using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSnakeAttacks : MonoBehaviour
{
    [Header("Referências")]
    public SnakeManager snakeMovimento;

    [Header("Configurações do Ataque Dash")]
    public int quantidadeDashes = 5;
    public float multiplicadorVelocidadeDash = 2.0f; 
    public float offsetForaDaTela = 5f;

    public GameObject avisoAtaquePrefab;
    public float tempoAviso = 1.0f; 
    public float tempoEntrePassagens = 1.5f;

    [Header("Configurações do Ataque Tornado")]
    public GameObject tornadoPrefab;
    public Transform player; // Arraste o jogador para cá no Inspector para os tornados mirarem nele
    public int minTornados = 10;
    public int maxTornados = 20;
    public float tornadoSpawnRadius = 2f;
    public float tornadoMinSize = 0.5f;
    public float tornadoMaxSize = 2f;
    public float tornadoMinSpeed = 1.5f;
    public float tornadoMaxSpeed = 5f;
    public float tornadoDamage = 1f;
    public float cadenciaTornado = 0.2f; // Tempo entre cuspir um tornado e outro

    [Header("Configurações do Bote (Mordida)")]
    public float tempoDeCompressao = 1.5f;
    public float velocidadeDoBote = 10f;
    private float limiteX = 15f;
    private float limiteY = 10f;

    public IEnumerator ExecutarDash()
    {
        Debug.Log("Iniciando Ataque: Dash (5 passagens)");
        
        float velocidadeOriginal = snakeMovimento.speed;
        snakeMovimento.estaOrbitando = false;
        snakeMovimento.speed = velocidadeOriginal * multiplicadorVelocidadeDash;

        for (int i = 0; i < quantidadeDashes; i++)
        {
            Vector2 direcaoAtaque = ObterDirecaoAleatoria();

            float distanciaSpawn = Mathf.Max(limiteX, limiteY) + offsetForaDaTela;
            Vector3 pontoDeNascer = (Vector3)(-direcaoAtaque * distanciaSpawn); 
            Vector3 pontoAviso = CalcularPosicaoBorda(-direcaoAtaque);

            float angulo = Mathf.Atan2(direcaoAtaque.y, direcaoAtaque.x) * Mathf.Rad2Deg;
            Quaternion rotacaoAtaque = Quaternion.Euler(0, 0, angulo);
            Quaternion rotacaoAviso = Quaternion.Euler(0, 0, angulo - 180f);

            GameObject aviso = null;
            if (avisoAtaquePrefab != null)
            {
                aviso = Instantiate(avisoAtaquePrefab, pontoAviso, rotacaoAviso);
            }

            yield return new WaitForSeconds(tempoAviso);

            if (aviso != null) Destroy(aviso);


            snakeMovimento.ReposicionarCobra(pontoDeNascer, rotacaoAtaque);

            Transform cauda = snakeMovimento.GetCauda();
            if (cauda != null)
            {
                while (Mathf.Abs(cauda.position.x) < limiteX + 5f && Mathf.Abs(cauda.position.y) < limiteY + 5f)
                {
                    yield return null; 
                }
            }
            
            yield return new WaitForSeconds(tempoEntrePassagens);
        }

        yield return new WaitForSeconds(1.0f);

        snakeMovimento.speed = velocidadeOriginal;
        Debug.Log("Dash finalizado!");
    }

    public IEnumerator ExecutarTornado()
    {
        Debug.Log("Iniciando Ataque: Tornado (Chuva Superior Fluida)");
        
        float velocidadeOriginal = snakeMovimento.speed;
        snakeMovimento.estaOrbitando = false;

        Transform cabeca = snakeMovimento.GetCabeca();
        float distanciaDeNascer = limiteY + (snakeMovimento.GetCauda().position - cabeca.position).magnitude + 10f;
        Vector3 pontoDeNascer = new Vector3(0, distanciaDeNascer, 0); 
        
        snakeMovimento.ReposicionarCobra(pontoDeNascer, Quaternion.Euler(0, 0, -90f));

        snakeMovimento.speed = velocidadeOriginal * 1.5f;
        float pontoDeParadaY = limiteY - 2f; 

        while (cabeca.position.y > pontoDeParadaY)
        {
            cabeca.rotation = Quaternion.Euler(0, 0, -90f);
            yield return null; 
        }

        snakeMovimento.speed = 0f;
        
        Rigidbody2D headRb = cabeca.GetComponent<Rigidbody2D>();
        if (headRb != null) headRb.velocity = Vector2.zero; 
        
        Vector3 posicaoTravada = cabeca.position; 

        int qtdTornados = Random.Range(minTornados, maxTornados + 1);
        int tornadosDisparados = 0;
        float cronometroTornado = cadenciaTornado;

        yield return new WaitForSeconds(0.5f); 


        while (tornadosDisparados < qtdTornados)
        {
            cabeca.position = posicaoTravada;

            Vector2 direction = Vector2.down; 
            
            if (player != null)
            {
                direction = (player.position - cabeca.position).normalized;
                
                float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                Quaternion targetRotation = Quaternion.Euler(0, 0, targetAngle);
                cabeca.rotation = Quaternion.Lerp(cabeca.rotation, targetRotation, 15f * Time.deltaTime);
            }

            cronometroTornado += Time.deltaTime;

            if (cronometroTornado >= cadenciaTornado)
            {
                cronometroTornado = 0f; 

                Vector2 spawnOffset = Random.insideUnitCircle.normalized * tornadoSpawnRadius;
                Vector3 spawnPos = cabeca.position + (Vector3)spawnOffset;

                float size = Random.Range(tornadoMinSize, tornadoMaxSize);
                float t = Mathf.InverseLerp(tornadoMinSize, tornadoMaxSize, size);
                float speedProj = Mathf.Lerp(tornadoMaxSpeed, tornadoMinSpeed, t);

                GameObject tornado = Instantiate(tornadoPrefab, spawnPos, Quaternion.identity);
                tornado.transform.localScale = Vector3.one * size;

                TornadoProjectile proj = tornado.GetComponent<TornadoProjectile>();
                if (proj != null) 
                {
                    proj.Init(direction, speedProj, tornadoDamage);
                }

                tornadosDisparados++;
            }

            yield return null; 
        }

        cabeca.rotation = Quaternion.Euler(0, 0, -90f);

        yield return new WaitForSeconds(2f); 

        snakeMovimento.speed = velocidadeOriginal * multiplicadorVelocidadeDash;

        Transform cauda = snakeMovimento.GetCauda();
        if (cauda != null)
        {
            while (cauda.position.y > -limiteY - 5f)
            {
                cabeca.rotation = Quaternion.Euler(0, 0, -90f);
                yield return null; 
            }
        }

        yield return new WaitForSeconds(2f);

        snakeMovimento.speed = velocidadeOriginal;
        Debug.Log("Tornado finalizado!");
    }

    public IEnumerator ExecutarMordida()
    {
        Debug.Log("Iniciando Ataque: Bote Perfurante (Reset de Memória)");
        
        float velocidadeOriginal = snakeMovimento.speed;
        snakeMovimento.estaOrbitando = false;
        snakeMovimento.speed = 0f;

        List<GameObject> corpo = snakeMovimento.GetCorpoCompleto();
        Transform cabeca = snakeMovimento.GetCabeca();
        
        Vector3[] posicoesIniciais = new Vector3[corpo.Count];
        Quaternion[] rotacoesIniciais = new Quaternion[corpo.Count];
        Vector3[] posicoesAlvo = new Vector3[corpo.Count];
        Quaternion[] rotacoesAlvo = new Quaternion[corpo.Count];
        
        for (int i = 0; i < corpo.Count; i++)
        {
            posicoesIniciais[i] = corpo[i].transform.position;
            rotacoesIniciais[i] = corpo[i].transform.rotation;
        }

        for (int i = 1; i < corpo.Count; i++)
        {
            float anguloZigueZague = (i % 2 == 0) ? -45f : 45f;
            rotacoesAlvo[i] = rotacoesIniciais[i] * Quaternion.Euler(0, 0, anguloZigueZague);
            posicoesAlvo[i] = Vector3.Lerp(posicoesIniciais[i], posicoesIniciais[i - 1], 0.6f);
        }

        if (AudioManager.Instance != null) AudioManager.Instance.PlaySerpenteAviso();

        float tempoDecorrido = 0f;
        while (tempoDecorrido < tempoDeCompressao)
        {
            tempoDecorrido += Time.deltaTime;
            float percentual = tempoDecorrido / tempoDeCompressao;
            float curvaSuave = Mathf.SmoothStep(0f, 1f, percentual);

            if (player != null)
            {
                Vector2 direcaoProPlayer = (player.position - cabeca.position).normalized;
                float anguloCabeca = Mathf.Atan2(direcaoProPlayer.y, direcaoProPlayer.x) * Mathf.Rad2Deg;
                cabeca.rotation = Quaternion.Lerp(cabeca.rotation, Quaternion.Euler(0, 0, anguloCabeca), 12f * Time.deltaTime);
            }

            for (int i = 1; i < corpo.Count; i++)
            {
                corpo[i].transform.rotation = Quaternion.Lerp(rotacoesIniciais[i], rotacoesAlvo[i], curvaSuave);
                corpo[i].transform.position = Vector3.Lerp(posicoesIniciais[i], posicoesAlvo[i], curvaSuave);
            }
            
            yield return null;
        }

        Vector3 alvoDoBote = player.position; 
        
        for (int i = 1; i < corpo.Count; i++)
        {
            corpo[i].transform.position = posicoesIniciais[i];
            corpo[i].transform.rotation = rotacoesIniciais[i];
        }

        for (int i = 0; i < corpo.Count; i++)
        {
            MarkerManager markM = corpo[i].GetComponent<MarkerManager>();
            if (markM != null)
            {
                markM.clearMarkerList();
            }
        }

        Vector2 direcaoBote = (alvoDoBote - cabeca.position).normalized;
        cabeca.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(direcaoBote.y, direcaoBote.x) * Mathf.Rad2Deg);

        for (int i = 1; i < corpo.Count; i++)
        {
            Vector3 direcaoParaOLider = (corpo[i - 1].transform.position - corpo[i].transform.position).normalized;
            corpo[i].transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(direcaoParaOLider.y, direcaoParaOLider.x) * Mathf.Rad2Deg);
        }

        float tempoAceleracao = 0.25f;
        float tempoAtual = 0f;
        
        Transform cauda = snakeMovimento.GetCauda();
        float limiteX = SerpenteBossManager.Instance.limiteX;
        float limiteY = SerpenteBossManager.Instance.limiteY;

        while (true)
        {
            if (tempoAtual < tempoAceleracao)
            {
                tempoAtual += Time.fixedDeltaTime;
                snakeMovimento.speed = Mathf.Lerp(0f, velocidadeDoBote, tempoAtual / tempoAceleracao);
            }
            else
            {
                snakeMovimento.speed = velocidadeDoBote;
            }

            if (cauda != null && (Mathf.Abs(cauda.position.x) > limiteX + 3f || Mathf.Abs(cauda.position.y) > limiteY + 3f))
            {
                break;
            }
            else if (cauda == null)
            {
                yield return new WaitForSeconds(2f);
                break;
            }

            yield return new WaitForFixedUpdate();
        }

        Debug.Log("Mordida atravessou a tela limpa! O Cérebro vai cuidar do retorno.");
    }

    private Vector2 ObterDirecaoAleatoria()
    {
        Vector2[] direcoes = {
            Vector2.up, Vector2.down, Vector2.left, Vector2.right,
            new Vector2(1, 1).normalized, new Vector2(-1, 1).normalized,
            new Vector2(1, -1).normalized, new Vector2(-1, -1).normalized
        };
        return direcoes[Random.Range(0, direcoes.Length)];
    }

    private Vector3 CalcularPosicaoBorda(Vector2 direcaoDeOrigem)
    {
        float x = Mathf.Clamp(direcaoDeOrigem.x * limiteX, -limiteX, limiteX);
        float y = Mathf.Clamp(direcaoDeOrigem.y * limiteY, -limiteY, limiteY);
        return new Vector3(x, y, 0f);
    }
}