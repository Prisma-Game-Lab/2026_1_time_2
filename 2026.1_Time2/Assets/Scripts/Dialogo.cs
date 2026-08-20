using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

[System.Serializable]
public struct LinhaDialogo
{
    [TextArea(2, 5)]
    public string texto;
}

public class Dialogo : MonoBehaviour
{
    public TextMeshProUGUI textComponent;
    public LinhaDialogo[] falas;
    public float velocidadeTexto;
    [SerializeField] public GameObject canvas;
    [SerializeField] public IntroController introScript; 
    private int index;
    private bool esperandoPrimeiroMovimento = true; 

    void Start()
    {
        textComponent.text = string.Empty;
        index = 0; 
        
        StartCoroutine(DigitaFala());     
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            bool digitando = (textComponent.text != falas[index].texto); 
            bool transicao = (introScript != null && introScript.emTransicao); 

            if (esperandoPrimeiroMovimento)
            {
                esperandoPrimeiroMovimento = false;
                if (digitando)
                {
                    StopAllCoroutines();
                    textComponent.text = falas[index].texto;
                }
                else
                {
                    ProximaFala();
                    if (introScript != null) introScript.AvancarImagem();
                }
                
                return; 
            }

            if (digitando && transicao)
            {
                StopAllCoroutines();
                textComponent.text = falas[index].texto;
                introScript.PularTransicao();
            }
            else if (!digitando && transicao)
            {
                introScript.PularTransicao();
            }
            else if (digitando && !transicao)
            {
                StopAllCoroutines();
                textComponent.text = falas[index].texto;
            }
            else 
            {
                ProximaFala();
                if (introScript != null) introScript.AvancarImagem();
            }
        }
    }

    IEnumerator DigitaFala()
    {
        foreach (char c in falas[index].texto.ToCharArray())
        {
            textComponent.text += c;
            yield return new WaitForSeconds(velocidadeTexto);
        }
    }

    void ProximaFala()
    {
        if (index < falas.Length - 1)
        {
            index++;
            textComponent.text = string.Empty;
            StartCoroutine(DigitaFala());
        }
        else
        {
            SceneManager.LoadScene("RoadMap");
        }
    }
}