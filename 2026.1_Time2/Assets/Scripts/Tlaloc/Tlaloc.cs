using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Tlaloc : MonoBehaviour
{
    public int maxHealth = 300;
    [SerializeField] private float currentHealth;
    private bool isDead = false;
    public LavaController[] lavas;
    private PlayerAttack script;

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

    [Header("Ataque de Porrada")]
    public float[] rotationAngle;
    public float attackSpeed = 5f;
    [SerializeField] private Transform[] porradasObjs;
    [SerializeField] private GameObject[] porradass;

    [Header("Configurações Visuais da Lava")]
    [SerializeField] private float velocidadeDoFluxo = 2f;
    [SerializeField] private float tempoDaLavaNoChao = 20f;


    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;

        GameObject jogadorObjeto = GameObject.FindWithTag("Player");
        script = jogadorObjeto.GetComponent<PlayerAttack>();


        StartCoroutine(chooseAttack());
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            StartCoroutine(lavaAttackAtivation(currentHealth));
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            ThunderAttack();
        }
        if(Input.GetKeyDown(KeyCode.I))
        {
            PorradaAttack();
        }

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
        Debug.Log("Tlaloc derrotado!");
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
            if (script.isAttacking)
            {
                Debug.Log("Dano de mataCavalo");
                TakeDamage(2.5f);
            }
        }
    }

    IEnumerator chooseAttack()
    {
        while (true)
        {
            int randomAttack = Random.Range(0, 3);
            switch (randomAttack)
            {
                case 0:
                    StartCoroutine(lavaAttackAtivation(currentHealth));
                    break;
                case 1:
                    ThunderAttack();
                    break;
                case 2:
                    PorradaAttack();
                    break;
            }
            yield return new WaitForSeconds(5f);
        }
    }


    //ATAQUE DE LAVA
    IEnumerator lavaAttackAtivation(float health)
    {
        lavaAttack(health);
        yield return new WaitForSeconds(1f);
    }

    void lavaAttack(float vida)
    {
        List<int> numerosSorteados = SortearNumeros(4, 0, 6);

        if(vida <= 300 && vida > 225)
        {
            StartCoroutine(lavas[numerosSorteados[0]].FluxoLavaCoroutine(velocidadeDoFluxo, tempoDaLavaNoChao));
        }
        else if(vida <= 225 && vida > 150)
        {
            StartCoroutine(lavas[numerosSorteados[0]].FluxoLavaCoroutine(velocidadeDoFluxo, tempoDaLavaNoChao));
            StartCoroutine(lavas[numerosSorteados[1]].FluxoLavaCoroutine(velocidadeDoFluxo, tempoDaLavaNoChao));
        }
        else if (vida <= 150 && vida > 75)
        {
            StartCoroutine(lavas[numerosSorteados[0]].FluxoLavaCoroutine(velocidadeDoFluxo, tempoDaLavaNoChao));
            StartCoroutine(lavas[numerosSorteados[1]].FluxoLavaCoroutine(velocidadeDoFluxo, tempoDaLavaNoChao));
            StartCoroutine(lavas[numerosSorteados[2]].FluxoLavaCoroutine(velocidadeDoFluxo, tempoDaLavaNoChao));
        }
        else if (vida <= 75 && vida > 0)
        {
            StartCoroutine(lavas[numerosSorteados[0]].FluxoLavaCoroutine(velocidadeDoFluxo, tempoDaLavaNoChao));
            StartCoroutine(lavas[numerosSorteados[1]].FluxoLavaCoroutine(velocidadeDoFluxo, tempoDaLavaNoChao));
            StartCoroutine(lavas[numerosSorteados[2]].FluxoLavaCoroutine(velocidadeDoFluxo, tempoDaLavaNoChao));
            StartCoroutine(lavas[numerosSorteados[3]].FluxoLavaCoroutine(velocidadeDoFluxo, tempoDaLavaNoChao));
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

    //ATAQUE DE RAIO
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
        Collider2D colisorAtingido = Physics2D.OverlapCircle(pontoImpacto, raioDoDano, camadaDoPlayer);

        if (colisorAtingido != null)
        {
            Player playerScript = colisorAtingido.GetComponent<Player>();

            if (playerScript == null)
            {
                return;
            }

            Debug.Log("O Player foi atingido pelo raio!");
            playerScript.TakeDamage(danoDoRaio);
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

    //ATAQUE DE PORRADA
    void PorradaAttack()
    {
        int num = SortearNumeros(1, 0, 3)[0];
        StartCoroutine(PorradaAttackCoroutine(rotationAngle[num], porradasObjs[num], num));
    }

    IEnumerator PorradaAttackCoroutine(float rotationAngle, Transform porrada, int numero)
    {
        porradass[numero].SetActive(true);
        StartCoroutine(lavas[0].FluxoLavaCoroutine(velocidadeDoFluxo, tempoDaLavaNoChao));
        StartCoroutine(lavas[4].FluxoLavaCoroutine(velocidadeDoFluxo, tempoDaLavaNoChao));
        StartCoroutine(lavas[5].FluxoLavaCoroutine(velocidadeDoFluxo, tempoDaLavaNoChao));

        yield return new WaitForSeconds(1.0f);


        Quaternion localOriginRotation = porrada.localRotation;
        Quaternion endRotation = localOriginRotation * Quaternion.Euler(0, 0, rotationAngle / 2f);

        float t = 0;

        t = 0;
        while (t < 1f)
        {
            t += Time.deltaTime * attackSpeed;
            porrada.localRotation = Quaternion.Slerp(localOriginRotation, endRotation, t);
            yield return null;
        }

        t = 0;
        while (t < 1f)
        {
            t += Time.deltaTime * attackSpeed;
            porrada.localRotation = Quaternion.Slerp(endRotation, localOriginRotation, t);
            yield return null;
        }

        porradass[numero].SetActive(false);
        yield return new WaitForSeconds(5.0f);

    }
}
