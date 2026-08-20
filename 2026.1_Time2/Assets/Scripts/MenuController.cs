using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    [SerializeField] private GameObject Menu;
    [SerializeField] private GameObject Config; 
    [SerializeField] private GameObject Controles;
    private bool primeiroPlay = true;
    

    [Header("Sliders de Volume")]
    [SerializeField] private Slider sliderGeral;
    [SerializeField] private Slider sliderMusica;
    [SerializeField] private Slider sliderSFX;

    private void Start() 
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.TocarMusica(AudioManager.Instance.musicaFaseSerpente);

            if (sliderGeral != null) sliderGeral.value = AudioManager.Instance.GetVolumeGeral();
            if (sliderMusica != null) sliderMusica.value = AudioManager.Instance.GetVolumeMusica();
            if (sliderSFX != null) sliderSFX.value = AudioManager.Instance.GetVolumeSFX();
        }

        Menu.SetActive(true);
        Config.SetActive(false);
        Controles.SetActive(false);
    }

    public void StartGame()
    {
        TocarSomBotao();
        if (primeiroPlay)
        {
            primeiroPlay = false;
            SceneManager.LoadScene("IntroInicial");
        }
        else
        {
            SceneManager.LoadScene("RoadMap");
        }
    }

    public void OpenSettings()
    {
        TocarSomBotao();
        Menu.SetActive(false);
        Config.SetActive(true);
    }

    public void BackToMenu()
    {
        TocarSomBotao();
        Config.SetActive(false);
        Menu.SetActive(true);
    }
    
    public void QuitGame()
    {
        TocarSomBotao();
        Application.Quit();
    }

    public void OpenControls()
    {
        TocarSomBotao();
        Config.SetActive(false);
        Controles.SetActive(true);
    }
    public void BackToSettings()
    {
        TocarSomBotao();
        Controles.SetActive(false);
        Config.SetActive(true);
    }

    // Funções para os Sliders de Áudio
    public void MudarVolumeGeral(float valor)
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.SetVolumeGeral(valor);
    }

    public void MudarVolumeMusica(float valor)
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.SetVolumeMusica(valor);
    }

    public void MudarVolumeSFX(float valor)
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.SetVolumeSFX(valor);
    }

    private void TocarSomBotao()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayCliqueBotao();
    }
}
