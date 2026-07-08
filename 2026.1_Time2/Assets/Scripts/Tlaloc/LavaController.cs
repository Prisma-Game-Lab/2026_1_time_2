using System.Collections;
using UnityEngine;

public class LavaController : MonoBehaviour
{
    [Header("Configurações da Máscara")]
    [SerializeField] private Transform lavaMask;

    [Tooltip("Posição da máscara onde a lava fica TOTALMENTE INVISÍVEL")]
    [SerializeField] private Vector3 posicaoEscondida;

    [Tooltip("Posição da máscara onde a lava fica TOTALMENTE VISÍVEL")]
    [SerializeField] private Vector3 posicaoPreenchida;

    private Collider2D colisorLava;

    void Awake()
    {
        colisorLava = GetComponent<Collider2D>();
        // Garante que comece escondida ao iniciar o jogo
        if (lavaMask != null) lavaMask.localPosition = posicaoEscondida;
        if (colisorLava != null) colisorLava.enabled = false;
    }

    // Coroutine responsável por fazer ESTA lava escorrer e sumir
    public IEnumerator FluxoLavaCoroutine(float velocidade, float tempoAtiva)
    {
        float t = 0;

        // 1. ESCORRER (Aparecer)
        if (colisorLava != null) colisorLava.enabled = false; // Desativado enquanto escorre

        while (t < 1f)
        {
            t += Time.deltaTime * velocidade;
            lavaMask.localPosition = Vector3.Lerp(posicaoEscondida, posicaoPreenchida, t);
            yield return null;
        }
        lavaMask.localPosition = posicaoPreenchida;

        // 2. ATIVAR DANOS
        if (colisorLava != null) colisorLava.enabled = true;

        // Espera o tempo que ela fica ativa no chão
        yield return new WaitForSeconds(tempoAtiva);

        // 3. SUMIR (Retrair)
        t = 0;
        if (colisorLava != null) colisorLava.enabled = false; // Desativa dano ao começar a sumir

        while (t < 1f)
        {
            t += Time.deltaTime * velocidade;
            lavaMask.localPosition = Vector3.Lerp(posicaoPreenchida, posicaoEscondida, t);
            yield return null;
        }
        lavaMask.localPosition = posicaoEscondida;
    }
}