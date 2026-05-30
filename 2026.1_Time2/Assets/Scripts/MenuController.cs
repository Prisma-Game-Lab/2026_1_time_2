using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    [SerializeField] private GameObject Menu;
    [SerializeField] private GameObject Config; 

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
}
