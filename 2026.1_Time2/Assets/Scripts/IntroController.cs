using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroController : MonoBehaviour
{
    [SerializeField] private GameObject imagemIntro;
    [SerializeField] private GameObject[] imagensParadas;
    [SerializeField] private GameObject introPanel;
    [SerializeField] private float velocidadePanel;
    
    void Start()
    {
        StartCoroutine(moverImagem(imagensParadas, velocidadePanel));
    }

    void Update()
    {
    }

    IEnumerator moverImagem(GameObject[] imagens, float velocidade)
    {
        for (int i = 0; i < imagens.Length; i++)
        {
            while (!Input.GetMouseButtonDown(0))
            {
                yield return null; 
            }

            Vector3 posInicial = imagemIntro.transform.position;
            Vector3 posFinal = imagens[i].transform.position;
            
            Vector3 escalaInicial = imagemIntro.transform.localScale;
            Vector3 escalaFinal = imagens[i].transform.localScale;

            float distancia = Vector3.Distance(posInicial, posFinal);
            float tempoDeViagem = distancia / velocidade;
            float tempoDecorrido = 0f;

            if (tempoDeViagem > 0)
            {
                while (tempoDecorrido < tempoDeViagem)
                {
                    tempoDecorrido += Time.deltaTime;
                    
                    float progresso = tempoDecorrido / tempoDeViagem;

                    imagemIntro.transform.position = Vector3.Lerp(posInicial, posFinal, progresso);
                    imagemIntro.transform.localScale = Vector3.Lerp(escalaInicial, escalaFinal, progresso);

                    yield return null;
                }
            }

            imagemIntro.transform.position = posFinal;
            imagemIntro.transform.localScale = escalaFinal;

            Debug.Log("Imagem " + i + " chegou ao destino. Aguardando clique do jogador...");
            
            while (!Input.GetMouseButtonDown(0))
            {
                yield return null; 
            }
        }
        Debug.Log("Todas as imagens foram passadas!");
    }
}