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
    public GameObject indicadorClique;

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
        indicadorClique.SetActive(false);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            indicadorClique.SetActive(false);
            if (textComponent.text == falas[index].texto)
            {
                ProximaFala();
            }
            else 
            {
                StopAllCoroutines();
                textComponent.text = falas[index].texto;
                indicadorClique.SetActive(true);
                StartCoroutine(ShakeIndicador());
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
        indicadorClique.SetActive(true);
        StartCoroutine(ShakeIndicador());
    }

    void ProximaFala()
    {
        if (index < falas.Length - 1)
        {
            index++;
            textComponent.text = string.Empty;
            StartCoroutine(DigitaFala());
            StopCoroutine(ShakeIndicador());

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

    IEnumerator ShakeIndicador()
    {
        Debug.Log("Animação do indicador de clique subindo e descendo!");
        while (true)
        {
            float newY = -25f + Mathf.Sin(Time.time * 5f) * 7f; 
            indicadorClique.transform.localPosition = new Vector3(indicadorClique.transform.localPosition.x, newY, indicadorClique.transform.localPosition.z);
            yield return null;
        }
    }
}