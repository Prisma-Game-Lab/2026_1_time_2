using UnityEngine;

public class BossSnakeTest : MonoBehaviour
{
    private BossSnakeAttacks attacks;

    void Start()
    {
        attacks = GetComponent<BossSnakeAttacks>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
            attacks.AttackTornado();

        if (Input.GetKeyDown(KeyCode.Alpha2))
            attacks.AttackBite();

        if (Input.GetKeyDown(KeyCode.Alpha3))
            attacks.AttackDashThrough(1f); // 1f = vida cheia
    }
}