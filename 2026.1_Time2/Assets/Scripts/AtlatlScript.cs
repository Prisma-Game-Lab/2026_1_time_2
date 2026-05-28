using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AtlatlScript : MonoBehaviour
{
    public float speed = 4.5f;

    private Camera mainCamera;
    private Vector3 moveDirection;

    void Start()
    {
        mainCamera = Camera.main;

        if(mainCamera != null)
        {
            Vector3 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = 0;

            moveDirection = (mousePosition - transform.position).normalized;

            //float angle = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg;
            //transform.rotation = Quaternion.Euler(0, 0, angle);
        }

        Destroy(gameObject, 2f);
    }

    void Update()
    {
        transform.position += moveDirection * speed * Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        
        if (other.CompareTag("Enemy"))
        {
            Debug.Log("Inimigo atingido! Dano: ");
        }
        
    }
}
