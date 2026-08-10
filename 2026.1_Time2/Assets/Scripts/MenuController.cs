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
        SceneManager.LoadScene("RoadMap");
    }

    public void OpenSettings()
    {
        Menu.SetActive(false);
        Config.SetActive(true);
    }

    public void BackToMenu()
    {
        Config.SetActive(false);
        Menu.SetActive(true);
    }
    
    public void QuitGame()
    {
        Application.Quit();
    }

    public void OpenControls()
    {
        Config.SetActive(false);
        Controles.SetActive(true);
    }
    public void BackToSettings()
    {
        Controles.SetActive(false);
        Config.SetActive(true);
    }
}
