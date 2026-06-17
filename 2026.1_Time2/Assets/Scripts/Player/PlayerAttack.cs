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

    void Start()
    {
          
    }

    void Update()
    {
        if(Input.GetMouseButtonDown(0) && !isAttacking)
        {
            StartCoroutine(weaponRotation());
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
                BossSnakeAI boss = other.GetComponent<BossSnakeAI>();
                if (boss != null)
                {
                    boss.TakeDamage((int)attackDamage);
                }
            }
        }
    }

    public bool IsWeaponAttacking()
    {
        return isAttacking;
    }


}
