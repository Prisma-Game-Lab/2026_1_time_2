using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

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
    
    private int index;

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
            if (textComponent.text == falas[index].texto)
            {
                ProximaFala();
                //proximaImagem();
            }

            else
            {
                StopAllCoroutines();
                textComponent.text = falas[index].texto;
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
            //proximaCena();
        }
    }
}