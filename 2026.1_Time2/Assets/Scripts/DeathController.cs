using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        UnityEngine.SceneManagement.SceneManager.LoadScene("SampleScene"); //Não entendi esse  UnityEngine.SceneManagement
        Time.timeScale = 1f;
    }

    public void voltarMenu()
    {
        Time.timeScale = 1f; 
        UnityEngine.SceneManagement.SceneManager.LoadScene("Menu");
    }
}
