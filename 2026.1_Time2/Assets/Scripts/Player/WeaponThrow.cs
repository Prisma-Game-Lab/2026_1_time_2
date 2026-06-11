using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponThrow : MonoBehaviour
{
    public GameObject atlatl;
    public Transform lançamento;
    public int numMax = 3;
    public int numAtual = 3;
    public float cooldown = 5.0f;

    private SpriteRenderer meuSpriteRenderer;

    [Header("Configurações do Arremesso")]
    public float tempoEntreArremessos = 0.8f;
    private float proximoTempoDeArremesso = 0f;

    private float tempoParaProximaRecarga = 0f;

    private void Start()
    {
        meuSpriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnEnable()
    {
        AtualizarRecargaPassiva();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && numAtual > 0 && Time.time >= proximoTempoDeArremesso)
        {
            Instantiate(atlatl, lançamento.position, transform.rotation);

            if (numAtual == numMax)
            {
                tempoParaProximaRecarga = Time.time + cooldown;
            }

            numAtual -= 1;
            proximoTempoDeArremesso = Time.time + tempoEntreArremessos;
        }

        if (numAtual < numMax)
        {
            if (Time.time >= tempoParaProximaRecarga)
            {
                numAtual += 1;

                if (numAtual < numMax)
                {
                    tempoParaProximaRecarga = Time.time + cooldown;
                }
            }
        }

        if (meuSpriteRenderer != null)
        {
            meuSpriteRenderer.enabled = (numAtual > 0);
        }
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
}