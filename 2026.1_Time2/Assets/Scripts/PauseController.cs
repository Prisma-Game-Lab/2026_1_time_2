using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseManager : MonoBehaviour
{
    [Header("Interface")]
    public GameObject painelDePause;

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
        painelDePause.SetActive(true); 
        Time.timeScale = 0f;           
        jogoPausado = true;
    }

    public void Despausar()
    {
        painelDePause.SetActive(false);
        Time.timeScale = 1f;           
        jogoPausado = false;
    }

    public void voltarMenu()
    {
        Time.timeScale = 1f; 
        UnityEngine.SceneManagement.SceneManager.LoadScene("Menu");
    }

    public void voltarMundo()
    {
        Time.timeScale = 1f;
        UnityEngine.SceneManagement.SceneManager.LoadScene("RoadMap");
    }
}
