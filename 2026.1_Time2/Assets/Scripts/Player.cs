using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    public float movementSpeed = 5f;

    public Rigidbody2D rb;

    Vector2 movement;

    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        // Input dos Movimentos Verticais e Horizontais
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");


        RotateTowardsMouse();
    }


    void FixedUpdate()
    {
        // Movimentação do Player
        Vector2 move = movement;
        // Normaliza para não aumentar velocidade na diagonal
        if (move.sqrMagnitude > 1f) move = move.normalized;
        rb.MovePosition(rb.position + move * movementSpeed * Time.fixedDeltaTime);
    }

    void RotateTowardsMouse()
    {
        // Pega a posição do mouse na tela e converte para coordenadas do mundo
        Vector3 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);

        // Calcula a direção
        Vector2 direction = new Vector2(
            mousePosition.x - transform.position.x,
            mousePosition.y - transform.position.y
        );

        // Calcula o ângulo
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // 4. Aplica a rotação no eixo Z
        transform.rotation = Quaternion.Euler(0, 0, angle - 90f);
    }

}
