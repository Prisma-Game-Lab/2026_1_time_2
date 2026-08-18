using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piranha : MonoBehaviour
{
    private Animator animPeixe;
    // Start is called before the first frame update
    void Start()
    {
        animPeixe = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        animPeixe.SetTrigger("Idle");
    }
}
