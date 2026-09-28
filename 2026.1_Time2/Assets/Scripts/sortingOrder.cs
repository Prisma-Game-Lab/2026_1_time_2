using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class SortingDinamicoPorY : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    [SerializeField] private int precisao = 100; // Multiplicador para casas decimais
    [SerializeField] private int offset = 0;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void LateUpdate()
    {
        // Multiplica por -1 para que Y menor resulte em maior sortingOrder (renderizado na frente)
        spriteRenderer.sortingOrder = Mathf.RoundToInt(-transform.position.y * precisao) + offset;
    }
}