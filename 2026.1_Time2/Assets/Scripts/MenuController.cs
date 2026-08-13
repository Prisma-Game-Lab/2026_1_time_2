using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    [SerializeField] private GameObject Menu;
    [SerializeField] private GameObject Config; 
    [SerializeField] private GameObject Controles;
    
    private void Start() 
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.TocarMusica(AudioManager.Instance.musicaFaseSerpente);

        Menu.SetActive(true);
        Config.SetActive(false);
        Controles.SetActive(false);
    }

    public void StartGame()
    {
        TocarSomBotao();
        SceneManager.LoadScene("RoadMap");
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
