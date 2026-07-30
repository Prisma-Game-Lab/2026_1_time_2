using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; 

public class DeathController : MonoBehaviour
{
    [Header("Interface")]
    public GameObject paineldeMorte;

    private bool morto = false;

    private void Start()
    {
        paineldeMorte.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (morto)
            {
                Respawn();
            }
            else
            {
                matar();
            }
        }
    }

    public void matar()
    {
        paineldeMorte.SetActive(true); 
        Time.timeScale = 0f;           
        morto = true;
    }

    public void Respawn()
    {
        paineldeMorte.SetActive(false);     
        morto = false;

        Scene cenaAtual = SceneManager.GetActiveScene();
        
        SceneManager.LoadScene(cenaAtual.name); 

        Time.timeScale = 1f;
    }

    public void voltarMenu()
    {
        Time.timeScale = 1f; 
        SceneManager.LoadScene("Menu");
    }

    public void voltarMundo()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("RoadMap");
    }
}