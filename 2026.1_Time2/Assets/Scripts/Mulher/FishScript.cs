using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishScript : MonoBehaviour
{
    [SerializeField] private float velocidade = 3f;
    private Transform player;
    private Rigidbody2D playerRb;

    public void SetPlayer (Transform alvo, Rigidbody2D rb)
    {
        player = alvo;
        playerRb = rb;
    }
    
    void Update()
    { 
        //O peixe só move-se em direção ao player se o player tiver um x maior que o peixe, ou seja, se o player estiver à direita do peixe, caso contrário, o peixe só se move para a direita, e não para a esquerda, evitando que o peixe se mova para trás.
        if (player != null && player.position.x > transform.position.x)
        {
            Vector3 direcao = (player.position - transform.position).normalized;
            transform.position += direcao * velocidade * Time.deltaTime;
        }
        else
        {
            transform.position += Vector3.right * velocidade * Time.deltaTime;
        }

        //Se o peixe sair da tela, ele é destruído.
        if (transform.position.x < -100f)
        {
            Destroy(gameObject);
        }
    }
}
