using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RoadMapManager : MonoBehaviour
{
    [Header("Bandeiras (Sprite Renderers)")]
    [SerializeField] private GameObject bandeiraSerpente;
    [SerializeField] private GameObject bandeiraMulher;
    [SerializeField] private GameObject bandeiraTlaloc;

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
            bandeiraSerpente.SetActive(false);

        if (bandeiraMulher != null && Progresso.mulherDerrotada)
            bandeiraMulher.SetActive(false);

        if (bandeiraTlaloc != null && Progresso.tlalocDerrotado)
            bandeiraTlaloc.SetActive(false);
    }

    IEnumerator IniciarCutsceneFinal()
    {
        yield return new WaitForSeconds(delayAntesDaCutscene);
        SceneManager.LoadScene(nomeCenaCutsceneFinal);
    }
}