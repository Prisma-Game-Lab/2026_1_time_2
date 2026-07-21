using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponThrow : MonoBehaviour
{
    public GameObject atlatl;
    public Transform lançamento;
    public int numMax = 3;
    public int numAtual = 3;
    public GameObject[] lancasUI;
    public float cooldown = 5.0f;

    private SpriteRenderer meuSpriteRenderer;

    [Header("Configurações do Arremesso")]
    public float tempoEntreArremessos = 0.8f;
    private float proximoTempoDeArremesso = 0f;
    public float tempoEntreLançaEOutra = 0.1f; // Tempo entre cada lança ao disparar todas

    private float tempoParaProximaRecarga = 0f;

    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;
        meuSpriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnEnable()
    {
        AtualizarRecargaPassiva();
    }

    void Update()
    {
        // 1. Sistema de Arremesso
        if (Input.GetMouseButtonDown(0) && numAtual > 0 && Time.time >= proximoTempoDeArremesso)
        {
            DispararUmaLança();
            proximoTempoDeArremesso = Time.time + tempoEntreArremessos;
        }
        if(Input.GetMouseButtonDown(1) && numAtual > 0 && Time.time >= proximoTempoDeArremesso)
        {
            StartCoroutine(DispararTodasAsLanças());

            proximoTempoDeArremesso = Time.time + tempoEntreArremessos;
        }

        // 2. Sistema de Recarga por Tempo
        if (numAtual < numMax)
        {
            if (Time.time >= tempoParaProximaRecarga)
            {
                numAtual += 1;
                lancasUI[numAtual - 1].SetActive(true);
                if (numAtual < numMax)
                {
                    tempoParaProximaRecarga = Time.time + cooldown;
                }
            }
        }

        // 3. Controle do Sprite
        if (meuSpriteRenderer != null)
        {
            meuSpriteRenderer.enabled = (numAtual > 0);
        }

        if(Time.deltaTime > 0)
        {
            RotateTowardsMouse();
        }
    }

    IEnumerator DispararTodasAsLanças()
    {
        int lançasParaDisparar = numAtual;
        for (int i = 0; i < lançasParaDisparar; i++)
        {
            DispararUmaLança();
            yield return new WaitForSeconds(tempoEntreLançaEOutra); // Espera 0.1 segundos entre cada lançamento
        }
    }

    private void DispararUmaLança()
    {
        // referência do objeto instanciado
        GameObject novoAtlatl = Instantiate(atlatl, lançamento.position, transform.rotation);

        // passa este script para o Atlatl saber quem o criou
        AtlatlScript scriptAtlatl = novoAtlatl.GetComponent<AtlatlScript>();
        if (scriptAtlatl != null)
        {
            scriptAtlatl.ConfigurarOrigem(this);
        }

        if (numAtual == numMax)
        {
            tempoParaProximaRecarga = Time.time + cooldown;
        }

        numAtual -= 1;
        lancasUI[numAtual].SetActive(false);
    }

    private void AtualizarRecargaPassiva()
    {
        if (numAtual >= numMax || tempoParaProximaRecarga == 0) return;

        while (Time.time >= tempoParaProximaRecarga && numAtual < numMax)
        {
            numAtual += 1;
            tempoParaProximaRecarga += cooldown;
        }
    }

    public void RecuperarMunicaoDoChao()
    {
        if (numAtual < numMax)
        {
            numAtual += 1;
            lancasUI[numAtual - 1].SetActive(true);
            tempoParaProximaRecarga += cooldown;
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
}