using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{

    public float rotationAngle = 60f;
    public float attackSpeed = 5f;

    private bool isAttacking = false;

    // Start is called before the first frame update
    void Start()
    {


        
    }

    // Update is called once per frame
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

        Quaternion localOriginRotation = transform.localRotation;

        Quaternion startRotation = localOriginRotation * Quaternion.Euler(0, 0, rotationAngle / 2f);
        Quaternion endRotation = localOriginRotation * Quaternion.Euler(0, 0, -rotationAngle / 2f);

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
}
