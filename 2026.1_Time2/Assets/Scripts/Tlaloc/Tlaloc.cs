using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tlaloc : MonoBehaviour
{
    public int Health = 100;
    public GameObject[] lavas;

    [Header("Configurações do Ataque de Raios")]
    [SerializeField] private int quantidadeDeRaios = 5;
    [SerializeField] private float intervaloEntreRaios = 0.3f;
    [SerializeField] private float tempoDeAviso = 1.0f;
    [SerializeField] private float raioDoDano = 1.5f;
    [SerializeField] private int danoDoRaio = 1;

    [Header("Limites do Topo do Vulcão")]
    [SerializeField] private float raioHorizontalX;
    [SerializeField] private float raioVerticalY;
    [SerializeField] private Vector2 centroDaCratera;
    [SerializeField] private float alturaDasNuvensY;

    [Header("Prefabs e Camadas")]
    [SerializeField] private GameObject prefabIndicador;
    [SerializeField] private GameObject prefabRaio;
    [SerializeField] private LayerMask camadaDoChao;
    [SerializeField] private LayerMask camadaDoPlayer;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            StartCoroutine(lavaAttackAtivation(Health));
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            ThunderAttack();
        }
    }

    IEnumerator lavaAttackAtivation(int health)
    {
        lavaAttack(health);
        yield return new WaitForSeconds(1f);
        foreach (GameObject lava in lavas)
        {
            lava.SetActive(false);
        }
    }

    void lavaAttack(int vida)
    {
        List<int> numerosSorteados = SortearNumeros(4, 0, 6);

        if(vida <= 100 && vida > 75)
        {
            lavas[numerosSorteados[0]].SetActive(true);
        }
        else if(vida <= 75 && vida > 50)
        {
            lavas[numerosSorteados[0]].SetActive(true);
            lavas[numerosSorteados[1]].SetActive(true);
        }
        else if (vida <= 50 && vida > 25)
        {
            lavas[numerosSorteados[0]].SetActive(true);
            lavas[numerosSorteados[1]].SetActive(true);
            lavas[numerosSorteados[2]].SetActive(true);
        }
        else if (vida <= 25 && vida > 0)
        {
            lavas[numerosSorteados[0]].SetActive(true);
            lavas[numerosSorteados[1]].SetActive(true);
            lavas[numerosSorteados[2]].SetActive(true);
            lavas[numerosSorteados[3]].SetActive(true);
        }
    }

    List<int> SortearNumeros(int quantidade, int min, int max)
    {
        HashSet<int> numeros = new HashSet<int>();

        while (numeros.Count < quantidade)
        {
            int numeroAleatorio = Random.Range(min, max);
            numeros.Add(numeroAleatorio);
        }

        return new List<int>(numeros);
    }

    void ThunderAttack()
    {
        StartCoroutine(ThunderAttackCoroutine());
    }

    IEnumerator ThunderAttackCoroutine()
    {
        for (int i = 0; i < quantidadeDeRaios; i++)
        {
            Vector2 pontoNoChao = SortearPontoNaElipse();

            StartCoroutine(SpawnarRaioIndividual(pontoNoChao));

            yield return new WaitForSeconds(intervaloEntreRaios);
        }
    }

    Vector2 SortearPontoNaElipse()
    {
        float angulo = Random.Range(0f, Mathf.PI * 2f);

        float distancia = Mathf.Sqrt(Random.Range(0f, 1f));

        float x = Mathf.Cos(angulo) * raioHorizontalX * distancia;
        float y = Mathf.Sin(angulo) * raioVerticalY * distancia;

        // Retorna o ponto somado à posição real do centro da cratera
        return centroDaCratera + new Vector2(x, y);
    }

    IEnumerator SpawnarRaioIndividual(Vector2 pontoImpactoChao)
    {
        // O aviso
        GameObject indicador = Instantiate(prefabIndicador, pontoImpactoChao, Quaternion.identity);

        yield return new WaitForSeconds(tempoDeAviso);
        Destroy(indicador);

        // CONFIGURAÇÃO DO RAIO CAINDO DAS NUVENS:
        Vector3 posicaoNuvem = new Vector3(pontoImpactoChao.x, alturaDasNuvensY, 0);
        GameObject raioVisual = Instantiate(prefabRaio, posicaoNuvem, Quaternion.identity);

        float distanciaAteOChao = alturaDasNuvensY - pontoImpactoChao.y;

        raioVisual.transform.localScale = new Vector3(raioVisual.transform.localScale.x, distanciaAteOChao, 1);

        VerificarDanoNoPlayer(pontoImpactoChao);

        Destroy(raioVisual, 0.5f);
    }

    void VerificarDanoNoPlayer(Vector2 pontoImpacto)
    {
        Collider2D playerAtingido = Physics2D.OverlapCircle(pontoImpacto, raioDoDano, camadaDoPlayer);

        if (playerAtingido != null)
        {
            Debug.Log("O Player foi atingido pelo raio!");
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Matrix4x4 matrizOriginal = Gizmos.matrix;

        Gizmos.matrix = Matrix4x4.TRS(centroDaCratera, Quaternion.identity, new Vector3(raioHorizontalX, raioVerticalY, 1));
        Gizmos.DrawWireSphere(Vector3.zero, 1f);

        Gizmos.matrix = matrizOriginal;
        Gizmos.color = Color.red;
        Gizmos.DrawLine(new Vector3(centroDaCratera.x - raioHorizontalX, alturaDasNuvensY, 0),
                        new Vector3(centroDaCratera.x + raioHorizontalX, alturaDasNuvensY, 0));
    }
}
