using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int health = 3;   
    public int speed = 2;
     
    public GameObject player;

    void Start()
    {
        int enemyPosX = Random.Range(-8, 8);
        int enemyPosY = Random.Range(-4, 4);
        transform.position = new Vector2(enemyPosX, enemyPosY);
        print("Inimigo criado com " + health + " de vida.");
    }

    // Update is called once per frame
    void Update() 
    {
        //Pega posição do player
        float playerX = player.transform.position.x;
        float playerY = player.transform.position.y;

        //Move o inimigo em direção ao player X
        if (transform.position.x < playerX)
        {
            transform.Translate(Vector2.right * speed * Time.deltaTime);
        }
        else if (transform.position.x > playerX)
        {
            transform.Translate(Vector2.left * speed * Time.deltaTime);
        }

        //Move o inimigo em direção ao player Y
        if (transform.position.y < playerY)
        {
            transform.Translate(Vector2.up * speed * Time.deltaTime);
        }
        if (transform.position.y > playerY)
        {
            transform.Translate(Vector2.down * speed * Time.deltaTime);
        }

        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }
}
