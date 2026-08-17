using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEditor;
using UnityEngine;

public class PauseManager : MonoBehaviour
{
    [Header("Interface")]
    public GameObject painelDePause;
    [SerializeField] private GameObject Config;
    [SerializeField] private GameObject Controles;

    private bool jogoPausado = false;

    private void Start()
    {
        painelDePause.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (jogoPausado)
            {
                Despausar();
            }
            else
            {
                Pausar();
            }
        }
    }

    public void Pausar()
    {
        TocarSomBotao();
        painelDePause.SetActive(true); 
        Time.timeScale = 0f;           
        jogoPausado = true;
    }

    public void Despausar()
    {
        TocarSomBotao();
        painelDePause.SetActive(false);
        Time.timeScale = 1f;           
        jogoPausado = false;
    }

    public void voltarMenu()
    {
        TocarSomBotao();
        Time.timeScale = 1f; 
        UnityEngine.SceneManagement.SceneManager.LoadScene("Menu");
    }

    public void voltarMundo()
    {
        TocarSomBotao();
        Time.timeScale = 1f;
        UnityEngine.SceneManagement.SceneManager.LoadScene("RoadMap");
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

    public void BackToPause()
    {
        TocarSomBotao();
        painelDePause.SetActive(true);
        Config.SetActive(false);
    }

    public void OpenSettings()
    {
        TocarSomBotao();
        painelDePause.SetActive(false);
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
