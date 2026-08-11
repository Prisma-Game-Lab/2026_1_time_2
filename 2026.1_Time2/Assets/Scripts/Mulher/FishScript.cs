using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishScript : MonoBehaviour
{
    [SerializeField] private float velocidade = 3f;
    private Transform player;
    private Rigidbody2D playerRb;

    public void SetPlayer(Transform alvo, Rigidbody2D rb)
    {
        player = alvo;
        playerRb = rb;
    }

    void Update()
    {
        if (player != null && player.position.x > transform.position.x)
        {
            Vector3 direcao = (player.position - transform.position).normalized;
            transform.position += direcao * velocidade * Time.deltaTime;
        }
        else
        {
            transform.position += Vector3.right * velocidade * Time.deltaTime;
        }

        if (transform.position.x < -100f)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Destroy(gameObject);
        }
    }
}