using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

[System.Serializable]
public struct LinhaDialogoFinal
{
    [TextArea(2, 5)]
    public string texto;
}

public class DialogoFinal : MonoBehaviour
{
    [Header("Configurações de Texto")]
    public TextMeshProUGUI textComponent;
    public LinhaDialogoFinal[] falas;
    public float velocidadeTexto;
    public string proximaCena;

    [Header("Imagens da Cutscene")]
    public GameObject imagem1;
    public GameObject imagem2;

    private int index;

    void Start()
    {
        textComponent.text = string.Empty;
        index = 0; 
        StartCoroutine(DigitaFala());

        if (imagem1 != null) imagem1.SetActive(true);
        if (imagem2 != null) imagem2.SetActive(false);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (textComponent.text == falas[index].texto)
            {
                ProximaFala();
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

            if (index == 1)
            {
                if (imagem1 != null) imagem1.SetActive(false);
                if (imagem2 != null) imagem2.SetActive(true);
            }
        }
        else
        {
            SceneManager.LoadScene("Menu");
        }
    }
}