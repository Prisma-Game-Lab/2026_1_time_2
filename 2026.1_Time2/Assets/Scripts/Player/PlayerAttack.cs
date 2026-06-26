using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{

    public float attackSpeed = 5f;
    public float attackDamage = 10f;

    [Header("Animação do Ataque")]
    public float rotationAngle = 60f;
    private bool isAttacking = false;

    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        if(Input.GetMouseButtonDown(0) && !isAttacking)
        {
            StartCoroutine(weaponRotation());
        }

        if(Time.deltaTime > 0)
        {
            RotateTowardsMouse();
        }
    }


    IEnumerator weaponRotation()
    {

        isAttacking = true;

        //Calcula as rotações de início e fim do ataque

        Quaternion localOriginRotation = transform.localRotation;
        Quaternion startRotation = localOriginRotation * Quaternion.Euler(0, 0, rotationAngle / 2f);
        Quaternion endRotation = localOriginRotation * Quaternion.Euler(0, 0, -rotationAngle / 2f);

        //Faz a rotação do ataque funcionar, primeiro indo um pouco para um lado, depois para o outro e depois voltando para a posição original
        float t = 0;

        while(t < 1f)
        {
            t += Time.deltaTime * attackSpeed;
            transform.localRotation = Quaternion.Slerp(localOriginRotation, startRotation, t);
            yield return null;
        }

        t = 0;
        while (t < 1f)
        {
            t += Time.deltaTime * attackSpeed;
            transform.localRotation = Quaternion.Slerp(startRotation, endRotation, t);
            yield return null;
        }

        t = 0;
        while (t < 1f)
        {
            t += Time.deltaTime * attackSpeed;
            transform.localRotation = Quaternion.Slerp(endRotation, localOriginRotation, t);
            yield return null;
        }

        isAttacking = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {

        if (isAttacking)
        {
            if (other.CompareTag("Enemy"))
            {
                Debug.Log("Inimigo atingido! Dano: " + attackDamage);
            }
        }
    }

    public bool IsWeaponAttacking()
    {
        return isAttacking;
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
        // Aplica a rotação no eixo Z
        transform.rotation = Quaternion.Euler(0, 0, angle - 90f);
    }


}
