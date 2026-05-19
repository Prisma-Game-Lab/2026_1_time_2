using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    public float movementSpeed = 5f;

    public Rigidbody2D rb;

    Vector2 movement;

    void Start()
    {
        
    }

    void Update()
    {
        // Input dos Movimentos Verticais e Horizontais
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");
    }
    void FixedUpdate()
    {
        // Movimentação do Player
        Vector2 move = movement;
        // Normaliza para não aumentar velocidade na diagonal
        if (move.sqrMagnitude > 1f) move = move.normalized;
        rb.MovePosition(rb.position + move * movementSpeed * Time.fixedDeltaTime);
    }

}
