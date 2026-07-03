using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tlaloc : MonoBehaviour
{
    public int Health = 100;
    public GameObject[] lavas;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            StartCoroutine(lavaAttackAtivation(Health));
        }
    }

    IEnumerator lavaAttackAtivation(int health)
    {
        lavaAttack(health);
        yield return new WaitForSeconds(1f);
        foreach (GameObject lava in lavas)
        {
            lava.SetActive(false);
        }
    }

    void lavaAttack(int vida)
    {
        List<int> numerosSorteados = SortearNumeros(4, 0, 6);

        if(vida <= 100 && vida > 75)
        {
            lavas[numerosSorteados[0]].SetActive(true);
        }
        else if(vida <= 75 && vida > 50)
        {
            lavas[numerosSorteados[0]].SetActive(true);
            lavas[numerosSorteados[1]].SetActive(true);
        }
        else if (vida <= 50 && vida > 25)
        {
            lavas[numerosSorteados[0]].SetActive(true);
            lavas[numerosSorteados[1]].SetActive(true);
            lavas[numerosSorteados[2]].SetActive(true);
        }
        else if (vida <= 25 && vida > 0)
        {
            lavas[numerosSorteados[0]].SetActive(true);
            lavas[numerosSorteados[1]].SetActive(true);
            lavas[numerosSorteados[2]].SetActive(true);
            lavas[numerosSorteados[3]].SetActive(true);
        }
    }

    List<int> SortearNumeros(int quantidade, int min, int max)
    {
        HashSet<int> numeros = new HashSet<int>();

        while (numeros.Count < quantidade)
        {
            int numeroAleatorio = Random.Range(min, max);
            numeros.Add(numeroAleatorio);
        }

        return new List<int>(numeros);
    }

    void ThunderAttack()
    {

    }
}
