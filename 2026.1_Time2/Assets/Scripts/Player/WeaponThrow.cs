using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponThrow : MonoBehaviour
{

    public GameObject atlatl;
    public Transform lançamento;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Instantiate(atlatl, lançamento.position, transform.rotation);
        }
    }
}
