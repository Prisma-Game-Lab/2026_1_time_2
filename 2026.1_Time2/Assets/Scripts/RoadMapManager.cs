using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RoadMapManager : MonoBehaviour
{
    [Header("Bandeiras (Sprite Renderers)")]
    [SerializeField] private SpriteRenderer bandeiraSerpente;
    [SerializeField] private SpriteRenderer bandeiraMulher;
    [SerializeField] private SpriteRenderer bandeiraTlaloc;

    [Header("Configuração de Cor")]
    [SerializeField] private Color corDourada = new Color(1f, 0.84f, 0f, 1f);

    [Header("Cutscene Final")]
    [SerializeField] private string nomeCenaCutsceneFinal = "CutsceneFinal";
    [SerializeField] private float delayAntesDaCutscene = 1.5f;

    void Start()
    {
        AtualizarBandeiras();

        if (Progresso.TodasFasesCompletas())
        {
            StartCoroutine(IniciarCutsceneFinal());
        }
    }

    void AtualizarBandeiras()
    {
        if (bandeiraSerpente != null && Progresso.serpenteDerrotada)
            bandeiraSerpente.color = corDourada;

        if (bandeiraMulher != null && Progresso.mulherDerrotada)
            bandeiraMulher.color = corDourada;

        if (bandeiraTlaloc != null && Progresso.tlalocDerrotado)
            bandeiraTlaloc.color = corDourada;
    }

    IEnumerator IniciarCutsceneFinal()
    {
        yield return new WaitForSeconds(delayAntesDaCutscene);
        SceneManager.LoadScene(nomeCenaCutsceneFinal);
    }
}